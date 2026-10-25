using System.Text;
using DGRV3TS.Core;

namespace DGRV3TS;

internal sealed partial class Operations
{
    private async void ScanMemory()
    {
        StopAutoplay();
        if (_busy) return;
        using var d = new FolderBrowserDialog { Description = "Cartella di traduzioni esistenti (incluse le sottocartelle)", UseDescriptionForTitle = true };
        if (d.ShowDialog(this) != DialogResult.OK) return;
        using var cancellation = new CancellationTokenSource();
        _memoryScan = cancellation;
        _busy = true; UseWaitCursor = true; RefreshCommandState();
        var file = _document.FilePath;
        var directory = d.SelectedPath;
        try
        {
            _status.Text = "Indicizzazione della memoria di traduzione…";
            var memory = await Task.Run(() => TranslationMemory.Scan(directory, file, cancellation.Token), cancellation.Token);
            _memory = memory;
            _status.Text = $"Memoria pronta · {memory.Warnings.Count} file con avvisi.";
            if (memory.Warnings.Count > 0) MessageBox.Show(this, string.Join("\n", memory.Warnings.Take(20)), "Avvisi memoria");
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        { _status.Text = "Indicizzazione annullata. La memoria precedente è stata conservata."; }
        catch (Exception e) { MessageBox.Show(this, e.Message, "Memoria non disponibile"); }
        finally
        {
            _memoryScan = null;
            _busy = false; UseWaitCursor = false; RefreshCommandState();
            if (_closeWhenScanStops)
            {
                _closeWhenScanStops = false;
                BeginInvoke(new Action(Close));
            }
        }
    }

    private void Suggest()
    {
        StopAutoplay();
        if (Current is not { } entry) return;
        if (_memory is null) { ScanMemory(); return; }
        var suggestions = _memory.Find(entry.Original, entry.Speaker, _document.SelectedLanguage);
        if (suggestions.Count == 0) { _status.Text = "Nessuna traduzione esistente per questo originale."; return; }
        using var d = new SuggestionDialog(suggestions);
        if (d.ShowDialog(this) == DialogResult.OK && d.SelectedText is not null && _document.Entries.Contains(entry))
        {
            _document.UpdateTranslation(entry, d.SelectedText);
            _vertical?.RefreshEntry(entry);
            SelectIndex(_document.Entries.IndexOf(entry));
        }
    }

    private void FillFromMemory()
    {
        StopAutoplay();
        if (_memory is null) { ScanMemory(); return; }
        if (MessageBox.Show(this, "Completare le traduzioni vuote con i soli suggerimenti univoci? Le traduzioni esistenti non saranno sostituite.", "Memoria di traduzione", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
        int changed = 0, conflicts = 0;
        foreach (var e in _document.Entries.Where(e => !IsTranslated(e)))
        {
            var suggestions = _memory.Find(e.Original, e.Speaker, _document.SelectedLanguage);
            if (suggestions.Count == 1) { _document.UpdateTranslation(e, suggestions[0].Text); changed++; }
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
