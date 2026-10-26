using DGRV3TS.Core;

namespace DGRV3TS;

internal sealed class TextInputDialog : Form
{
    private readonly TextBox _text;
    public string Value => _text.Text;
    public TextInputDialog(string title, string prompt, string value = "", bool multiline = false)
    {
        Text = title; Size = new Size(580, multiline ? 360 : 190); MinimumSize = new Size(400, 180);
        StartPosition = FormStartPosition.CenterParent;
        _text = new TextBox { Text = value, Multiline = multiline, Dock = DockStyle.Fill, ScrollBars = multiline ? ScrollBars.Vertical : ScrollBars.None };
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, Padding = new Padding(16) };
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)); panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        panel.Controls.Add(Ui.Label(prompt), 0, 0); panel.Controls.Add(_text, 0, 1);
        var actions = new FlowLayoutPanel { Dock = DockStyle.Fill };
        var ok = Ui.Button("OK", () => DialogResult = DialogResult.OK); var cancel = Ui.Button("Annulla", () => DialogResult = DialogResult.Cancel);
        actions.Controls.AddRange(new Control[] { ok, cancel }); panel.Controls.Add(actions, 0, 2);
        if (!multiline) AcceptButton = ok; CancelButton = cancel;
        Controls.Add(panel);
    }
}

internal sealed class ReplaceDialog : Form
{
    private readonly TextBox _find = new() { Dock = DockStyle.Fill };
    private readonly TextBox _replacement = new() { Dock = DockStyle.Fill };
    private readonly CheckBox _all = new() { Text = "Tutte le battute della lingua corrente", AutoSize = true };
    private readonly CheckBox _case = new() { Text = "Distingui maiuscole/minuscole", AutoSize = true };
    public string Find => _find.Text;
    public string Replacement => _replacement.Text;
    public bool All => _all.Checked;
    public bool MatchCase => _case.Checked;
    public ReplaceDialog()
    {
        Text = "Sostituisci nelle traduzioni"; ClientSize = new Size(500, 255); StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false;
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 7, Padding = new Padding(16) };
        panel.Controls.Add(Ui.Label("Cerca testo letterale"), 0, 0); panel.Controls.Add(_find, 0, 1);
        panel.Controls.Add(Ui.Label("Sostituisci con"), 0, 2); panel.Controls.Add(_replacement, 0, 3);
        panel.Controls.Add(_case, 0, 4); panel.Controls.Add(_all, 0, 5);
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
        var ok = Ui.Button("Sostituisci", () => { if (_find.TextLength > 0) DialogResult = DialogResult.OK; }); ok.Enabled = false;
        _find.TextChanged += (_, _) => ok.Enabled = _find.TextLength > 0;
        var cancel = Ui.Button("Annulla", () => DialogResult = DialogResult.Cancel);
        buttons.Controls.AddRange(new Control[] { ok, cancel }); panel.Controls.Add(buttons, 0, 6);
        AcceptButton = ok; CancelButton = cancel; Controls.Add(panel);
    }
}

internal sealed class SuggestionDialog : Form
{
    private readonly ListBox _list = new() { Dock = DockStyle.Fill, IntegralHeight = false, HorizontalScrollbar = true };
    private readonly RichTextBox _text = Ui.Editor(true);
    private readonly IReadOnlyList<TranslationSuggestion> _suggestions;
    public string? SelectedText => _list.SelectedIndex < 0 ? null : _suggestions[_list.SelectedIndex].Text;
    public SuggestionDialog(IReadOnlyList<TranslationSuggestion> suggestions)
    {
        _suggestions = suggestions; Text = "Scegli una traduzione esistente"; Size = new Size(760, 480); StartPosition = FormStartPosition.CenterParent;
        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, Padding = new Padding(16) };
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 45)); panel.RowStyles.Add(new RowStyle(SizeType.Percent, 55)); panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        foreach (var s in suggestions) _list.Items.Add($"{Path.GetFileName(s.SourceFile)} · {s.Speaker} · {s.Text.Replace('\n', ' ')}");
        _list.SelectedIndexChanged += (_, _) => _text.Text = SelectedText ?? "";
        _list.DoubleClick += (_, _) => { if (SelectedText is not null) DialogResult = DialogResult.OK; };
        panel.Controls.Add(_list, 0, 0); panel.Controls.Add(_text, 0, 1);
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill };
        var ok = Ui.Button("Usa traduzione", () => { if (SelectedText is not null) DialogResult = DialogResult.OK; });
        var cancel = Ui.Button("Annulla", () => DialogResult = DialogResult.Cancel);
        buttons.Controls.AddRange(new Control[] { ok, cancel }); panel.Controls.Add(buttons, 0, 2);
        AcceptButton = ok; CancelButton = cancel; Controls.Add(panel);
        if (_list.Items.Count > 0) _list.SelectedIndex = 0;
    }
}
