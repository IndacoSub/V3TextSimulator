namespace DGRV3TS;

internal static class Ui
{
    internal static readonly Color Ink = Color.FromArgb(29, 43, 57);
    internal static readonly Color Muted = Color.FromArgb(85, 102, 121);
    internal static readonly Color Paper = Color.FromArgb(244, 247, 250);
    internal static readonly Color Accent = Color.FromArgb(0, 111, 113);

    internal static Label Label(string text) => new()
    { Text = text, Dock = DockStyle.Fill, ForeColor = Muted, TextAlign = ContentAlignment.MiddleLeft, AutoEllipsis = true };

    internal static Button Button(string text, Action action)
    {
        var b = new Button { Text = text, AutoSize = true, MinimumSize = new Size(65, 31), FlatStyle = FlatStyle.Flat, BackColor = Color.White, Margin = new Padding(0, 2, 6, 2) };
        b.FlatAppearance.BorderColor = Color.FromArgb(205, 215, 225);
        b.Click += (_, _) => action();
        return b;
    }

    internal static RichTextBox Editor(bool readOnly = false) => new()
    {
        Dock = DockStyle.Fill, Font = new Font("Segoe UI", 12), ReadOnly = readOnly,
        BackColor = readOnly ? Paper : Color.White, ForeColor = Ink,
        DetectUrls = false, BorderStyle = BorderStyle.FixedSingle, HideSelection = false, WordWrap = true,
        AccessibleName = readOnly ? "Originale" : "Textbox traduzione"
    };

    internal static ToolStripMenuItem MenuItem(string text, Action action, Keys keys = Keys.None)
    {
        var item = new ToolStripMenuItem(text) { ShortcutKeys = keys };
        item.Click += (_, _) => action();
        return item;
    }
}
