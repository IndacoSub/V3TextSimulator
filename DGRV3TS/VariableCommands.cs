using System.Text;
using DGRV3TS.Core;

namespace DGRV3TS;

internal sealed partial class Operations
{
    private sealed record VariableItem(VariableDefinition Definition)
    { public override string ToString() => $"{Definition.Token}  →  {Definition.Value}"; }

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
        _catalog = loaded; BindCatalog();
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
        _variables.BeginUpdate();
        try
        {
            _variables.Items.Clear();
            foreach (var v in _catalog.Variables.Where(v => (_category.SelectedIndex <= 0 || v.Category == _category.SelectedItem?.ToString()) &&
                (v.Token.Contains(_variableQuery.Text, StringComparison.OrdinalIgnoreCase) || v.Value.Contains(_variableQuery.Text, StringComparison.OrdinalIgnoreCase)))
                .OrderBy(v => v.Token, StringComparer.OrdinalIgnoreCase)) _variables.Items.Add(new VariableItem(v));
        }
        finally { _variables.EndUpdate(); }
    }

    private void InsertVariable()
    { if (Current is not null && _variables.SelectedItem is VariableItem v) { _translation.SelectedText = v.Definition.Token; _translation.Focus(); } }

    private void CopyVariable(string suffix)
    { if (_variables.SelectedItem is VariableItem v) Safe(() => Clipboard.SetText(v.Definition.Token + suffix)); }

    private void InsertVariant(string suffix)
    {
        if (Current is null || _variables.SelectedItem is not VariableItem v) return;
        var baseToken = v.Definition.Token;
        var token = baseToken.EndsWith('>') ? baseToken[..^1] + suffix + ">" : baseToken + suffix;
        if (!_catalog.Variables.Any(x => x.Token == token))
        { MessageBox.Show(this, $"La variante {token} non è definita nel dizionario esterno. Aggiungila al file e ricarica."); return; }
        _translation.SelectedText = token; _translation.Focus();
    }
}
