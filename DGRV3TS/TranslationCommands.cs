using System.Text;
using DGRV3TS.Core;

namespace DGRV3TS;

internal sealed partial class Operations
{
    private async void ScanMemory()
    {
        await ScanMemoryAsync();
    }

    private async Task<bool> ScanMemoryAsync()
    {
        StopAutoplay();
        if (_busy) return false;
        using var d = new FolderBrowserDialog { Description = "Cartella di traduzioni esistenti (incluse le sottocartelle)", UseDescriptionForTitle = true };
        if (d.ShowDialog(this) != DialogResult.OK) return false;
        using var cancellation = new CancellationTokenSource();
        _memoryScan = cancellation;
        _busy = true; UseWaitCursor = true; RefreshCommandState();
        var file = _document.FilePath;
        var directory = d.SelectedPath;
        try
        {
            _status.Text = "Indicizzazione della memoria di traduzione…";
            var memory = await Task.Run(() => TranslationMemory.Scan(directory, file, cancellation.Token), cancellation.Token);
            cancellation.Token.ThrowIfCancellationRequested();
            _memory = memory;
            _status.Text = $"Memoria pronta · {memory.Warnings.Count} file con avvisi.";
            if (memory.Warnings.Count > 0) MessageBox.Show(this, string.Join("\n", memory.Warnings.Take(20)), "Avvisi memoria");
            return true;
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        { _status.Text = "Indicizzazione annullata. La memoria precedente è stata conservata."; return false; }
        catch (Exception e) { MessageBox.Show(this, e.Message, "Memoria non disponibile"); return false; }
        finally { FinishMemoryOperation(); }
    }

    private void FinishMemoryOperation()
    {
        _memoryScan = null;
        _automaticMemoryScan = false;
        _busy = false;
        if (IsDisposed) return;
        UseWaitCursor = false; RefreshCommandState();
        if (_closeWhenScanStops)
        {
            _closeWhenScanStops = false;
            if (IsHandleCreated) BeginInvoke(new Action(Close));
        }
    }

    /// <summary>The v25 automatic local-memory pass, after opening PO/VO/XLSX.</summary>
    private async Task ReuseOnOpenAsync(TranslationDocument openedDocument)
    {
        if (!_reuseOnOpen || _busy || !ReferenceEquals(openedDocument, _document)
            || !TranslationReuse.SupportsAutomaticReuse(openedDocument.FilePath)
            || !openedDocument.Entries.Any(TranslationReuse.IsUntranslated)) return;
        StopAutoplay();
        _automaticMemoryScan = true;
        using var cancellation = new CancellationTokenSource();
        _memoryScan = cancellation;
        _busy = true; UseWaitCursor = true; RefreshCommandState();
        var language = openedDocument.SelectedLanguage;
        var changed = 0;
        try
        {
            var memory = _memory;
            if (memory is null)
            {
                var path = openedDocument.FilePath!;
                var directory = Path.GetDirectoryName(Path.GetFullPath(path))!;
                _status.Text = "Ricerca automatica delle traduzioni nella cartella del documento…";
                memory = await Task.Run(() => TranslationMemory.Scan(directory, path, cancellation.Token), cancellation.Token);
            }
            cancellation.Token.ThrowIfCancellationRequested();
            if (IsDisposed || !ReferenceEquals(openedDocument, _document)) return;
            _memory = memory;
            foreach (var entry in openedDocument.Entries)
            {
                cancellation.Token.ThrowIfCancellationRequested();
                if (!_reuseOnOpen || openedDocument.SelectedLanguage != language) break;
                if (!TranslationReuse.IsUntranslated(entry)) continue;
                var suggestions = TranslationReuse.FindSuggestions(memory, entry, language,
                    _catalog.Variables.Select(v => v.Token), excludedFile: openedDocument.FilePath);
                if (suggestions.Count == 0) continue;
                string? selected;
                if (suggestions.Count == 1) selected = suggestions[0].Text;
                else
                {
                    using var dialog = new SuggestionDialog(suggestions)
                    { Text = $"Traduzioni in conflitto · battuta {entry.Id} · {entry.Speaker}" };
                    if (dialog.ShowDialog(this) != DialogResult.OK) break;
                    selected = dialog.SelectedText;
                }
                cancellation.Token.ThrowIfCancellationRequested();
                // A scan never reserves a row: edits made while it ran always take precedence.
                if (selected is null || !TranslationReuse.IsUntranslated(entry)
                    || openedDocument.SelectedLanguage != language) continue;
                openedDocument.UpdateTranslation(entry, TranslationReuse.PrepareTranslation(selected));
                changed++;
            }
            _status.Text = $"Memoria automatica: {changed} battute riutilizzate; {memory.Warnings.Count} avvisi di lettura.";
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        { if (!IsDisposed) _status.Text = $"Ricerca automatica annullata. {changed} modifiche già applicate restano in memoria."; }
        catch (Exception error) when (error is not OutOfMemoryException and not StackOverflowException)
        { if (!IsDisposed) MessageBox.Show(this, error.Message, "Memoria automatica non disponibile"); }
        finally
        {
            if (!IsDisposed && ReferenceEquals(openedDocument, _document))
            { _vertical?.SetDocument(openedDocument, Current); SelectIndex(_index); }
            FinishMemoryOperation();
        }
    }

    private async void Suggest()
    {
        StopAutoplay();
        if (Current is not { } entry) return;
        var document = _document;
        var language = document.SelectedLanguage;
        if (_memory is null && !await ScanMemoryAsync()) return;
        if (_memory is null || !ReferenceEquals(document, _document) || language != document.SelectedLanguage) return;
        var suggestions = TranslationReuse.FindSuggestions(_memory, entry, language,
            _catalog.Variables.Select(v => v.Token), requireCompatible: false, excludedFile: _document.FilePath);
        if (suggestions.Count == 0) { _status.Text = "Nessuna traduzione esistente per questo originale."; return; }
        using var d = new SuggestionDialog(suggestions)
        { Text = $"Suggerimenti · battuta {entry.Id} · {entry.Speaker}" };
        if (d.ShowDialog(this) == DialogResult.OK && d.SelectedText is not null && _document.Entries.Contains(entry))
        {
            var issues = TranslationTools.Validate(TranslationReuse.CandidateText(entry.Original), d.SelectedText, _catalog.Variables.Select(v => v.Token));
            if (issues.Count > 0 && MessageBox.Show(this,
                string.Join("\n", issues.Select(issue => issue.Message)) + "\n\nUsare comunque questo suggerimento?",
                "Segnaposto da controllare", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            _document.UpdateTranslation(entry, TranslationReuse.PrepareTranslation(d.SelectedText));
            _vertical?.RefreshEntry(entry);
            SelectIndex(_document.Entries.IndexOf(entry));
        }
    }

    private async void FillFromMemory()
    {
        StopAutoplay();
        var document = _document;
        var language = document.SelectedLanguage;
        if (_memory is null && !await ScanMemoryAsync()) return;
        if (_memory is null || !ReferenceEquals(document, _document) || language != document.SelectedLanguage) return;
        if (MessageBox.Show(this, "Completare le traduzioni vuote con i soli suggerimenti univoci? Le traduzioni esistenti non saranno sostituite.", "Memoria di traduzione", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
        int changed = 0, conflicts = 0;
        foreach (var e in _document.Entries.Where(e => !IsTranslated(e)))
        {
            var suggestions = TranslationReuse.FindSuggestions(_memory, e, language,
                _catalog.Variables.Select(v => v.Token), excludedFile: _document.FilePath);
            if (suggestions.Count == 1) { _document.UpdateTranslation(e, TranslationReuse.PrepareTranslation(suggestions[0].Text)); changed++; }
            else if (suggestions.Count > 1) conflicts++;
        }
        _vertical?.SetDocument(_document, Current); SelectIndex(_index);
        _status.Text = $"Completate {changed} battute · {conflicts} conflitti lasciati alla revisione manuale.";
    }

    private void Replace()
    {
        StopAutoplay();
        if (Current is null) return;
        using var d = new ReplaceDialog();
        if (d.ShowDialog(this) != DialogResult.OK) return;
        if (d.All && MessageBox.Show(this, "Sostituire in tutte le traduzioni della lingua selezionata?", "Sostituisci tutto", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
        var entries = d.All ? _document.Entries.ToArray() : new[] { Current };
        int count = 0;
        foreach (var e in entries)
        {
            var text = e.Translation.Replace(d.Find, d.Replacement, d.MatchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
            if (text == e.Translation) continue;
            _document.UpdateTranslation(e, text); count++;
        }
        _vertical?.SetDocument(_document, Current); SelectIndex(_index);
        _status.Text = $"Sostituzione completata in {count} battute.";
    }
}
