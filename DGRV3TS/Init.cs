using System.Text;
using DGRV3TS.Core;

namespace DGRV3TS;

internal sealed partial class Operations
{
    private void InitializeWorkspace(string? initialPath)
    {
        BindCommands();
        BindEvents();
        ShowDocument();
        Shown += (_, _) =>
        {
            if (!string.IsNullOrWhiteSpace(initialPath)) OpenPath(initialPath);
            else FindVariables();
        };
    }

    private void BindCommands()
    {
        var commands = new Dictionary<string, Action>(StringComparer.Ordinal)
        {
            ["Open"] = OpenFile, ["New"] = NewProject, ["Save"] = () => Save(false),
            ["SaveAs"] = () => Save(true), ["AddEntry"] = AddEntry, ["LoadWrd"] = LoadWrd,
            ["ExportOriginal"] = ExportOriginal, ["Exit"] = Close, ["FocusSearch"] = () => _query.Focus(),
            ["Replace"] = Replace, ["Previous"] = () => Navigate(-1), ["Next"] = () => Navigate(1),
            ["NextUntranslated"] = NextUntranslated, ["MarkEmpty"] = MarkEmpty,
            ["Reset"] = () => SelectIndex(0), ["FastRead"] = () => SelectIndex(_document.Entries.Count - 1),
            ["ScanMemory"] = ScanMemory, ["Suggest"] = Suggest, ["FillMemory"] = FillFromMemory,
            ["ShowVertical"] = ShowVertical, ["ShowPreview"] = ShowPreview,
            ["ChooseAssetRoot"] = ChooseAssetRoot, ["ImportSprites"] = ImportSprites,
            ["ChooseVariables"] = ChooseVariables, ["ReloadVariables"] = ReloadVariables,
            ["ExportVoiced"] = ExportVoiced,
            ["PlayVoice"] = () => { ShowPreview(); _preview?.PlayVoice(); },
            ["Speak"] = () => { ShowPreview(); _preview?.SpeakText(); },
            ["CopyImage"] = () => { ShowPreview(); _preview?.CopyImage(); },
            ["SaveImage"] = () => { ShowPreview(); _preview?.SaveImage(); },
            ["Help"] = ShowHelp, ["GoToLine"] = () => SelectIndex((int)_line.Value - 1),
            ["FindNext"] = FindNext, ["StartAutoplay"] = StartAutoplay,
            ["PauseAutoplay"] = PauseAutoplay, ["StopAutoplay"] = StopActivities,
            ["CopySource"] = CopySource, ["UpdatePreview"] = UpdatePreview,
            ["InsertVariable"] = InsertVariable, ["CopyVariableToken"] = () => CopyVariable(""),
            ["CopyVariableValue"] = () =>
            {
                if (_variables.SelectedItem is VariableItem v && v.Definition.Value.Length > 0)
                    Clipboard.SetText(v.Definition.Value);
            },
            ["InsertLowerVariant"] = () => InsertVariant("_MN"),
            ["InsertUpperVariant"] = () => InsertVariant("_MS")
        };

        void Dispatch(object? sender, EventArgs args)
        {
            var command = sender switch { Control c => c.Tag as string, ToolStripItem i => i.Tag as string, _ => null };
            if (command is null || !commands.TryGetValue(command, out var action)) return;
            if (_busy && ChangesDocument(command))
            {
                _status.Text = "Indicizzazione in corso. Premi Stop per annullarla prima di cambiare documento.";
                return;
            }
            // One route for manual actions: modal dialogs and navigation cannot race autoplay.
            if (command is not "StartAutoplay" and not "PauseAutoplay") StopAutoplay();
            Safe(action);
        }

        void BindItems(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                if (item.Tag is string) item.Click += Dispatch;
                if (item is ToolStripDropDownItem dropdown) BindItems(dropdown.DropDownItems);
            }
        }

        void BindControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is Button && control.Tag is string) control.Click += Dispatch;
                if (control is ToolStrip strip) BindItems(strip.Items);
                BindControls(control);
            }
        }

        BindControls(this);
        BindItems(_variableMenu.Items);
    }

    private static bool ChangesDocument(string command) => command is
        "Open" or "New" or "Save" or "SaveAs" or "AddEntry" or "LoadWrd" or "ScanMemory";

    private void RefreshCommandState()
    {
        void RefreshItems(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                if (item.Tag is string command && ChangesDocument(command)) item.Enabled = !_busy;
                if (item is ToolStripDropDownItem dropdown) RefreshItems(dropdown.DropDownItems);
            }
        }

        void RefreshControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is Button && control.Tag is string command && ChangesDocument(command)) control.Enabled = !_busy;
                if (control is ToolStrip strip) RefreshItems(strip.Items);
                RefreshControls(control);
            }
        }

        RefreshControls(this);
    }

    private void BindEvents()
    {
        _translation.Enter += (_, _) => StopAutoplay();
        _translation.KeyDown += (_, _) => StopAutoplay();
        _translation.MouseDown += (_, _) => StopAutoplay();
        _translation.TextChanged += (_, _) =>
        {
            if (_binding || Current is not { } entry) return;
            _document.UpdateTranslation(entry, Normalize(_translation.Text));
            _vertical?.RefreshEntry(entry);
            UpdateChecks(); UpdatePreview(); UpdateStatus();
        };
        _language.SelectedIndexChanged += (_, _) =>
        {
            if (_binding || _language.SelectedIndex < 0) return;
            StopAutoplay();
            _document.SelectLanguage(_language.SelectedIndex);
            _vertical?.SetDocument(_document, Current);
            SelectIndex(_index);
        };
        _game.SelectedIndex = 0;
        _game.SelectedIndexChanged += (_, _) =>
        {
            StopAutoplay();
            _catalogIsExplicit = false;
            FindVariables(true);
            _preview?.SetGame(_game.SelectedIndex == 0 ? "DR" : "AI");
            UpdatePreview();
        };
        _autoVoiceItem.CheckedChanged += (_, _) => _voiceOnNavigate = _autoVoiceItem.Checked;
        _autoTtsItem.CheckedChanged += (_, _) => _ttsOnNavigate = _autoTtsItem.Checked;
        _variableQuery.TextChanged += (_, _) => FilterVariables();
        _category.SelectedIndexChanged += (_, _) => FilterVariables();
        _variables.DoubleClick += (_, _) => InsertVariable();
        _variables.SelectedIndexChanged += (_, _) => _variableNote.Text = _variables.SelectedItem is VariableItem item
            ? $"{item.Definition.Token}\n{item.Definition.Comment}" : $"{_catalog.Variables.Count} variabili · {_catalog.Warnings.Count} avvisi";
        _variableMenu.Opening += (_, e) => { StopAutoplay(); e.Cancel = _variables.SelectedIndex < 0; };
        _variables.MouseDown += (_, e) =>
        {
            StopAutoplay();
            if (e.Button == MouseButtons.Right) _variables.SelectedIndex = _variables.IndexFromPoint(e.Location);
        };
        _resolve.CheckedChanged += (_, _) => UpdatePreview();
        _alternate.CheckedChanged += (_, _) => UpdatePreview();
        _previewOriginal.CheckedChanged += (_, _) => UpdatePreview();
        _query.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) { FindNext(); e.SuppressKeyPress = true; } };
        _line.Enter += (_, _) => StopAutoplay();
        _query.Enter += (_, _) => StopAutoplay();
        _line.KeyDown += (_, e) =>
        {
            if (e.KeyCode != Keys.Enter) return;
            StopAutoplay(); SelectIndex((int)_line.Value - 1); e.SuppressKeyPress = true;
        };
        _autoplay.Tick += (_, _) =>
        {
            if (_preview is { IsDisposed: false, IsSpeaking: true }) return;
            if (_index + 1 >= _document.Entries.Count) { StopAutoplay(); return; }
            SelectIndex(_index + 1);
        };
        FormClosing += (_, e) =>
        {
            StopAutoplay();
            if (_busy)
            {
                _closeWhenScanStops = true;
                _memoryScan?.Cancel();
                _status.Text = "Annullamento dell'indicizzazione prima della chiusura…";
                e.Cancel = true;
            }
            else if (!ConfirmChanges()) e.Cancel = true;
        };
    }

    private void ShowHelp() => MessageBox.Show(this,
        "V3 Text — riscrittura della versione v25\n\n" +
        "La finestra principale serve per tradurre: originale, textbox e variabili esterne.\n" +
        "F6 apre il riepilogo verticale separato: originale e traduzione di tutte le battute. Clic per selezionare, tasto destro per copia/incolla.\n" +
        "F7 apre l'anteprima grafica separata con sfondi, sprite, font, audio e screenshot.\n\n" +
        "Ctrl+S salva; il file precedente viene conservato in .bak. JSON mantiene il progetto completo.\n" +
        "Ctrl+↑/↓ cambia battuta; F8 cerca la prossima da tradurre. Ctrl+Z/Ctrl+Y agiscono nella textbox corrente.\n" +
        "La memoria riusa traduzioni esistenti, senza servizi esterni. I conflitti vengono mostrati per scegliere.\n\n" +
        "Le risorse del gioco non sono incluse. Puoi tradurre senza averle installate.", "Guida", MessageBoxButtons.OK, MessageBoxIcon.Information);
}
