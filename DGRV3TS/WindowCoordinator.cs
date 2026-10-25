using System.Text;
using DGRV3TS.Core;

namespace DGRV3TS;

internal sealed partial class Operations
{
    private void ShowVertical()
    {
        if (_vertical is null || _vertical.IsDisposed)
        {
            _vertical = new VerticalView();
            _vertical.FormClosed += (_, _) => _vertical = null;
            _vertical.EntrySelected += entry => { StopAutoplay(); SelectIndex(_document.Entries.IndexOf(entry), false); };
            _vertical.TranslationPasted += (entry, text) =>
            {
                StopAutoplay();
                if (!_document.Entries.Contains(entry)) return;
                _document.UpdateTranslation(entry, Normalize(text));
                _vertical.RefreshEntry(entry);
                SelectIndex(_document.Entries.IndexOf(entry), false);
            };
            _vertical.SetDocument(_document, Current);
            _vertical.Show(this);
        }
        else { _vertical.SetDocument(_document, Current); _vertical.Show(); }
        _vertical.Activate();
    }

    private void ShowPreview()
    {
        if (_preview is null || _preview.IsDisposed)
        {
            _preview = new DialoguePreviewWindow();
            _preview.FormClosed += (_, _) => _preview = null;
            _preview.GameChanged += game => _game.SelectedIndex = game == "AI" ? 1 : 0;
            _preview.SetAssetRoot(_assetRoot);
            _preview.SetGame(_game.SelectedIndex == 0 ? "DR" : "AI");
            _preview.Show(this);
        }
        UpdatePreview();
        _preview.Activate();
    }

    private void UpdatePreview()
    {
        if (_preview is null || _preview.IsDisposed) return;
        var entry = Current;
        var text = entry?.DisplayText(_previewOriginal.Checked) ?? "";
        if (_resolve.Checked) text = _catalog.Resolve(text, _alternate.Checked);
        _preview.SetDialogue(text, entry?.Speaker ?? "", entry?.Expression ?? "", entry?.Voice ?? "");
    }

    private void ChooseAssetRoot()
    {
        StopAutoplay();
        using var d = new FolderBrowserDialog { Description = "Cartella che contiene Graphics e Sound", UseDescriptionForTitle = true };
        if (d.ShowDialog(this) != DialogResult.OK) return;
        _assetRoot = d.SelectedPath; _preview?.SetAssetRoot(_assetRoot); FindVariables(!_catalogIsExplicit); UpdatePreview();
    }

    private void ImportSprites()
    {
        StopAutoplay();
        using var d = new FolderBrowserDialog { Description = "Cartella degli sprite estratti (stand_*.png)", UseDescriptionForTitle = true };
        if (d.ShowDialog(this) != DialogResult.OK) return;
        Safe(() => { var count = MediaAssets.ImportSprites(d.SelectedPath, _assetRoot); _preview?.ReloadAssets(); _status.Text = $"Importati {count} sprite. I file esistenti sono stati conservati."; });
    }
}
