using System.Text;
using DGRV3TS.Core;

namespace DGRV3TS;

internal sealed partial class Operations
{
    private sealed record VariableItem(VariableDefinition Definition, string DisplayValue)
    { public override string ToString() => $"{Definition.Token}  →  {DisplayValue}"; }

    private void FindVariables(bool force = false)
    {
        if (!force && _catalog.FilePath is not null) return;
        Safe(() =>
        {
            var directories = new[] { _document.FilePath is null ? null : Path.GetDirectoryName(_document.FilePath), _assetRoot, AppContext.BaseDirectory, Environment.CurrentDirectory }
                .Where(p => !string.IsNullOrWhiteSpace(p)).Cast<string>().Distinct().ToArray();
            string? path = _game.SelectedIndex == 0 ? VariableCatalog.FindDefaultFile(directories) : directories
                .Where(Directory.Exists).SelectMany(d => Directory.EnumerateFiles(d)).FirstOrDefault(p => Path.GetFileName(p).Equals("vars_ai.txt", StringComparison.OrdinalIgnoreCase));
            if (path is not null) LoadVariables(path);
            else if (force) { _catalog = VariableCatalog.Empty; BindCatalog(); }
        });
    }

    private void ChooseVariables()
    {
        StopAutoplay();
        using var d = new OpenFileDialog { Title = "Carica VARS_BAK o VARS_AI", Filter = "File di testo|*.txt;VARS_BAK;VARS_AI|Tutti i file|*.*" };
        if (d.ShowDialog(this) == DialogResult.OK)
            Safe(() => { LoadVariables(d.FileName); _catalogIsExplicit = true; });
    }

    private void ReloadVariables()
    {
        StopAutoplay();
        if (_catalog.FilePath is null) ChooseVariables(); else Safe(() => LoadVariables(_catalog.FilePath));
    }

    private void LoadVariables(string path)
    {
        var loaded = VariableCatalog.Load(path);
        _catalog = loaded;
        _hiddenVariableTokens.Clear();
        BindCatalog();
        if (loaded.Warnings.Count > 0) MessageBox.Show(this, string.Join("\n", loaded.Warnings.Take(30)), "Avvisi dizionario", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void BindCatalog()
    {
        _category.Items.Clear(); _category.Items.Add("Tutte le categorie");
        foreach (var c in _catalog.Variables.Select(v => v.Category).Distinct().OrderBy(s => s)) _category.Items.Add(c);
        _category.SelectedIndex = 0;
        _variableNote.Text = $"{_catalog.Variables.Count} variabili · {_catalog.Warnings.Count} avvisi";
        FilterVariables(); UpdateChecks(); UpdatePreview();
    }

    private void FilterVariables()
    {
        var selectedToken = (_variables.SelectedItem as VariableItem)?.Definition.Token;
        _variables.BeginUpdate();
        try
        {
            _variables.Items.Clear();
            foreach (var item in _catalog.Variables
                .Where(v => !_hiddenVariableTokens.Contains(v.Token) && (_category.SelectedIndex <= 0 || v.Category == _category.SelectedItem?.ToString()))
                .Select(v => new VariableItem(v, _catalog.Resolve(v.Token, _alternate.Checked)))
                .Where(v => v.Definition.Token.Contains(_variableQuery.Text, StringComparison.OrdinalIgnoreCase)
                    || v.DisplayValue.Contains(_variableQuery.Text, StringComparison.OrdinalIgnoreCase)
                    || v.Definition.Comment.Contains(_variableQuery.Text, StringComparison.OrdinalIgnoreCase))
                .OrderBy(v => v.Definition.Token, StringComparer.OrdinalIgnoreCase)) _variables.Items.Add(item);
            for (int i = 0; i < _variables.Items.Count; i++)
                if (_variables.Items[i] is VariableItem item && item.Definition.Token == selectedToken) { _variables.SelectedIndex = i; break; }
        }
        finally { _variables.EndUpdate(); }
    }

    private void InsertVariable()
    {
        if (Current is null || _variables.SelectedItem is not VariableItem v) return;
        _translation.SelectedText = _game.SelectedIndex == 0 ? v.Definition.Token : v.DisplayValue;
        _translation.Focus();
    }

    private void CopyVariable(string suffix)
    {
        if (_variables.SelectedItem is not VariableItem v) return;
        var text = _game.SelectedIndex == 0 ? v.Definition.Token + suffix : v.DisplayValue;
        if (text.Length == 0) return;
        Safe(() => { Clipboard.SetText(text); _status.Text = "Variabile copiata negli appunti."; });
    }

    private void InsertVariant(string suffix)
    {
        if (_variables.SelectedItem is not VariableItem v) return;
        var result = _catalog.WithCaseVariant(v.Definition.Token, uppercase: suffix == "_MS");
        _catalog = result.Catalog;
        Clipboard.SetText(result.Variant.Token);
        if (Current is not null) { _translation.SelectedText = result.Variant.Token; _translation.Focus(); }
        FilterVariables();
        UpdateChecks(); UpdatePreview();
        _status.Text = result.Created
            ? $"{result.Variant.Token}: variante temporanea creata e copiata. Aggiungila a VARS_BAK per conservarla dopo la ricarica."
            : $"{result.Variant.Token}: variante esistente copiata.";
    }

    private void HideVariable()
    {
        if (_variables.SelectedItem is not VariableItem v) return;
        _hiddenVariableTokens.Add(v.Definition.Token);
        FilterVariables();
        _status.Text = "Variabile rimossa dalla lista. Il file e la risoluzione dei token rimangono disponibili; ricarica per ripristinare la lista.";
    }

    private void ShowVariableInfo()
    {
        if (_variables.SelectedItem is not VariableItem v) return;
        MessageBox.Show(this, $"Token: {v.Definition.Token}\nCategoria: {v.Definition.Category}\n" +
            $"Valore: {_catalog.Resolve(v.Definition.Token)}\nAlternativo: {_catalog.Resolve(v.Definition.Token, true)}\n" +
            $"Note: {v.Definition.Comment}\nTemporanea: {(v.Definition.IsTemporary ? "sì" : "no")}", "Informazioni variabile");
    }

    private void ShowAlternateVariable()
    {
        if (_variables.SelectedItem is not VariableItem v) return;
        MessageBox.Show(this, _catalog.Resolve(v.Definition.Token, !_alternate.Checked), "Valore nella convenzione alternativa");
    }
}
