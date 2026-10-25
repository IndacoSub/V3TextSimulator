using System.Text;
using DGRV3TS.Core;

namespace DGRV3TS;

internal sealed partial class Operations
{
    private static string Normalize(string text) => text.Replace("\r\n", "\n").Replace('\r', '\n');

    private void SelectIndex(int index, bool scrollVertical = true)
    {
        _index = _document.Entries.Count == 0 ? -1 : Math.Clamp(index, 0, _document.Entries.Count - 1);
        var changedSelection = !ReferenceEquals(_displayedEntry, Current) || _displayedLanguage != _document.SelectedLanguage;
        _binding = true;
        try
        {
            var original = Current?.Original ?? "";
            var translation = Current?.Translation ?? "";
            if (Normalize(_source.Text) != Normalize(original)) _source.Text = original;
            if (changedSelection || Normalize(_translation.Text) != Normalize(translation)) _translation.Text = translation;
            // A click on the selected summary or a boundary navigation must preserve caret and undo.
            if (changedSelection) _translation.ClearUndo();
            _translation.Enabled = Current is not null;
            _line.Maximum = Math.Max(1, _document.Entries.Count);
            _line.Value = Math.Max(1, _index + 1);
            _metadata.Text = Current is { } e ? $"{_index + 1}/{_document.Entries.Count} · ID {e.Id} · {e.Speaker} · {e.Origin} · {e.Expression} · {e.Voice}" : "Apri un file per iniziare a tradurre.";
            _displayedEntry = Current;
            _displayedLanguage = _document.SelectedLanguage;
        }
        finally { _binding = false; }
        _vertical?.SelectEntry(Current, scrollVertical);
        UpdateChecks(); UpdatePreview(); UpdateStatus();
        if (changedSelection && Current is not null && (_voiceOnNavigate || _ttsOnNavigate))
        {
            if (_preview is null) ShowPreview();
            if (_voiceOnNavigate && _preview?.HasVoiceFile == true) _preview.PlayVoice();
            else if (_ttsOnNavigate) _preview?.SpeakText();
        }
    }

    private void ShowDocument()
    {
        StopAutoplay();
        _binding = true;
        _language.Items.Clear();
        for (int i = 0; i < _document.LanguageCount; i++) _language.Items.Add($"Traduzione {i + 1}");
        _language.SelectedIndex = _document.SelectedLanguage;
        _language.Enabled = _document.LanguageCount > 1;
        _binding = false;
        SelectIndex(0);
        if (_document.FilePath is not null || _document.Entries.Count > 0) ShowVertical();
        else _vertical?.SetDocument(_document, null);
    }

    private void UpdateChecks()
    {
        if (Current is not { } entry) { _checks.Text = ""; return; }
        var issues = TranslationTools.Validate(TranslationReuse.CandidateText(entry.Original),
            TranslationReuse.CandidateText(entry.Translation), _catalog.Variables.Select(v => v.Token));
        _checks.Text = issues.Count == 0 ? $"{entry.Translation.EnumerateRunes().Count()} caratteri · nessuna differenza nei segnaposto" : string.Join("  ·  ", issues.Take(3).Select(i => i.Message));
    }

    private void UpdateStatus()
    {
        Text = $"{(_document.IsDirty ? "● " : "")}{(_document.FilePath is null ? "Nuovo progetto" : Path.GetFileName(_document.FilePath))} — V3 Text v25";
        _progress.Text = $"{_document.Entries.Count(IsTranslated)}/{_document.Entries.Count} tradotti";
    }

    private static bool IsTranslated(TranslationEntry entry) => entry.HasTranslation || !string.IsNullOrWhiteSpace(entry.Translation);

    private void Navigate(int delta) { StopAutoplay(); SelectIndex(_index + delta); }

    private void FindNext()
    {
        StopAutoplay();
        var query = _query.Text;
        for (int offset = 1; offset <= _document.Entries.Count; offset++)
        {
            var index = (_index + offset) % _document.Entries.Count;
            var e = _document.Entries[index];
            if ((!_untranslated.Checked || !IsTranslated(e)) &&
                (query.Length == 0 || new[] { e.Id, e.Original, e.Translation, e.Speaker, e.Context }.Any(s => s.Contains(query, StringComparison.OrdinalIgnoreCase))))
            { SelectIndex(index); return; }
        }
        _status.Text = "Nessun dialogo corrispondente.";
    }

    private void NextUntranslated()
    {
        StopAutoplay();
        for (int offset = 1; offset <= _document.Entries.Count; offset++)
        {
            var index = (_index + offset) % _document.Entries.Count;
            if (!IsTranslated(_document.Entries[index])) { SelectIndex(index); return; }
        }
        _status.Text = "Tutti i dialoghi hanno una traduzione.";
    }

    private void StartAutoplay()
    {
        if (_busy || Current is null) return;
        _autoplay.Interval = (int)_interval.Value * 1000;
        _autoplay.Start(); _autoplayPaused = false;
        _status.Text = "Scorrimento automatico avviato.";
    }

    private void PauseAutoplay()
    {
        if (_autoplay.Enabled) { _autoplay.Stop(); _autoplayPaused = true; _status.Text = "Scorrimento in pausa."; }
        else if (_autoplayPaused) StartAutoplay();
    }

    private void StopAutoplay() { _autoplay.Stop(); _autoplayPaused = false; _preview?.StopAudio(); }

    private void StopActivities()
    {
        StopAutoplay();
        _memoryScan?.Cancel();
        _status.Text = _busy ? "Annullamento dell'indicizzazione…" : "Riproduzione interrotta.";
    }

    private void CopySource()
    {
        StopAutoplay();
        if (Current is null) return;
        if (_translation.TextLength > 0 && MessageBox.Show(this, "Sostituire la traduzione con l'originale?", "Copia originale", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
        _translation.SelectAll(); _translation.SelectedText = Current.Original;
    }

    private void MarkEmpty()
    {
        StopAutoplay();
        if (Current is not { } entry) return;
        if (entry.Translation.Length > 0 && MessageBox.Show(this, "Svuotare intenzionalmente questa traduzione?", "Traduzione vuota", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
        _document.UpdateTranslation(entry, "");
        _vertical?.RefreshEntry(entry);
        SelectIndex(_index);
    }
}
