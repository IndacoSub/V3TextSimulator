using System.Text;
using DGRV3TS.Core;

namespace DGRV3TS;

internal sealed partial class Operations
{
    private void OpenFile()
    {
        StopAutoplay();
        using var d = new OpenFileDialog { Title = "Apri file di traduzione", Filter = "Traduzioni|*.po;*.vo;*.txt;*.stx;*.xlsx;*.json|Tutti i file|*.*" };
        if (d.ShowDialog(this) == DialogResult.OK) OpenPath(d.FileName);
    }

    private void OpenPath(string path)
    {
        if (_busy || !ConfirmChanges()) return;
        Safe(() =>
        {
            var loaded = TranslationDocument.Load(path);
            var directory = Path.GetDirectoryName(Path.GetFullPath(path))!;
            if (Directory.Exists(Path.Combine(directory, "Graphics"))) _assetRoot = directory;
            else if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, "Graphics"))) _assetRoot = Environment.CurrentDirectory;
            _document = loaded;
            _memory = null;
            FindVariables(!_catalogIsExplicit);
            ShowDocument();
            _preview?.SetAssetRoot(_assetRoot);
            if (Path.GetExtension(path).Equals(".stx", StringComparison.OrdinalIgnoreCase))
            {
                var wrd = Path.ChangeExtension(path, ".wrd");
                if (File.Exists(wrd)) Safe(() => ApplyWrd(wrd));
            }
            if (Directory.Exists(Path.Combine(_assetRoot, "Graphics"))) ShowPreview();
            _status.Text = $"Aperto {Path.GetFileName(path)} · {_document.FormatName}";
            _ = ReuseOnOpenAsync(loaded);
        });
    }

    private void NewProject()
    {
        StopAutoplay();
        if (_busy || !ConfirmChanges()) return;
        _document = TranslationDocument.Create(); _memory = null;
        ShowDocument(); AddEntry();
    }

    private void AddEntry()
    {
        StopAutoplay();
        if (_document.FilePath is not null && !Path.GetExtension(_document.FilePath).Equals(".json", StringComparison.OrdinalIgnoreCase))
        { MessageBox.Show(this, "Salva prima una copia come progetto JSON per aggiungere dialoghi."); return; }
        using var d = new TextInputDialog("Aggiungi dialogo", "Testo originale", "", true);
        if (d.ShowDialog(this) != DialogResult.OK || d.Value.Length == 0) return;
        _document.AddEntry(Normalize(d.Value));
        _vertical?.SetDocument(_document, _document.Entries.Last());
        SelectIndex(_document.Entries.Count - 1);
        ShowVertical();
    }

    private bool ConfirmChanges()
    {
        if (!_document.IsDirty) return true;
        return MessageBox.Show(this, "Salvare le modifiche prima di continuare?", "Modifiche non salvate", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) switch
        { DialogResult.Yes => Save(false), DialogResult.No => true, _ => false };
    }

    private bool Save(bool saveAs)
    {
        StopAutoplay();
        var path = _document.FilePath;
        if (saveAs || path is null)
        {
            using var d = new SaveFileDialog
            {
                Title = "Salva progetto o converti formato", OverwritePrompt = true,
                Filter = "Progetto completo JSON|*.json|Gettext PO|*.po|Testo TXT|*.txt|STX|*.stx|VO|*.vo|Excel XLSX|*.xlsx",
                FileName = path is null ? "traduzione.json" : Path.GetFileNameWithoutExtension(path) + ".json",
                InitialDirectory = path is null ? "" : Path.GetDirectoryName(path)
            };
            if (d.ShowDialog(this) != DialogResult.OK) return false;
            path = d.FileName;
        }
        var ext = Path.GetExtension(path).ToLowerInvariant();
        if ((ext is ".txt" or ".stx" || (ext == ".po" && _document.LanguageCount > 1)) && MessageBox.Show(this,
            "TXT e STX salvano solo il testo finale; PO salva una sola lingua. Le battute non tradotte mantengono l'originale; quelle segnate vuote vengono svuotate.\n\n" +
            "Per conservare originali, traduzioni e tutte le lingue, salva anche un progetto JSON. Continuare?",
            "Conservazione del lavoro", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) != DialogResult.OK) return false;
        return Safe(() => { _document.Save(path); UpdateStatus(); _status.Text = $"Salvato {Path.GetFileName(path)}"; });
    }

    private void LoadWrd()
    {
        StopAutoplay();
        if (_document.Entries.Count == 0) return;
        using var d = new OpenFileDialog { Title = "Apri WRD associato", Filter = "WRD|*.wrd" };
        if (d.ShowDialog(this) == DialogResult.OK) Safe(() => ApplyWrd(d.FileName));
    }

    private void ApplyWrd(string path)
    {
        var metadata = WrdMetadata.Load(path);
        var mapped = _document.Entries.Select(e => (Entry: e, Metadata: metadata.ForLine(int.TryParse(e.Id, out var id) ? id : _document.Entries.IndexOf(e)))).ToArray();
        foreach (var (entry, info) in mapped)
        { entry.Speaker = info.Speaker; entry.Expression = info.Expression; entry.Voice = info.Voice; entry.Origin = Path.GetFileNameWithoutExtension(path); }
        _document.MarkDirty();
        _vertical?.SetDocument(_document, Current);
        SelectIndex(_index);
        _status.Text = $"Metadati WRD caricati: {Path.GetFileName(path)}";
    }

    private void ExportOriginal() => ExportText(_document.Entries, true, "Esporta gli originali");

    private void ExportVoiced()
    {
        var entries = _document.Entries.Where(e => e.Voice.Length > 0).ToArray();
        if (entries.Length == 0) { MessageBox.Show(this, "Nessuna battuta con metadati voce nel documento."); return; }
        ExportText(entries, false, "Esporta battute con voce");
    }

    private void ExportText(IEnumerable<TranslationEntry> entries, bool original, string title)
    {
        StopAutoplay();
        using var d = new SaveFileDialog { Title = title, Filter = "Testo|*.txt", FileName = original ? "originali.txt" : "voci.txt" };
        if (d.ShowDialog(this) != DialogResult.OK) return;
        if (_document.FilePath is not null && Path.GetFullPath(d.FileName).Equals(Path.GetFullPath(_document.FilePath), StringComparison.OrdinalIgnoreCase))
        { MessageBox.Show(this, "Scegli un nome diverso dal documento aperto."); return; }
        Safe(() => File.WriteAllLines(d.FileName, entries.Select(e => Normalize(e.DisplayText(original)).Replace("\n", "\\n")), new UTF8Encoding(false)));
    }
}
