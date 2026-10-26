using System.Runtime.InteropServices;
using DGRV3TS.Core;

namespace DGRV3TS;

/// <summary>The secondary, full-document vertical dialogue window from the v25 workflow.</summary>
internal sealed class VerticalView : Form
{
    private readonly CardCanvas _cards = new() { Dock = DockStyle.Fill };
    private readonly Label _count = new()
    {
        Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft,
        ForeColor = Color.FromArgb(38, 52, 66), Padding = new Padding(10, 0, 0, 0)
    };
    private readonly ContextMenuStrip _menu = new();
    private readonly ToolStripMenuItem _copyOriginal = new("Copia originale");
    private readonly ToolStripMenuItem _copyTranslation = new("Copia traduzione");
    private readonly ToolStripMenuItem _pasteTranslation = new("Incolla nella traduzione");
    private TranslationDocument? _document;

    public event Action<TranslationEntry>? EntrySelected;
    public event Action<TranslationEntry, string>? TranslationPasted;

    public VerticalView()
    {
        Text = "Dialoghi — vista verticale";
        Size = new Size(340, 760);
        MinimumSize = new Size(280, 400);
        StartPosition = FormStartPosition.Manual;
        Font = new Font("Segoe UI", 10);
        BackColor = Color.FromArgb(240, 243, 247);
        AutoScaleMode = AutoScaleMode.Dpi;
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = Padding.Empty
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
        layout.Controls.Add(_count, 0, 0);
        layout.Controls.Add(_cards, 0, 1);
        layout.Controls.Add(new Label
        {
            Text = "Clic: modifica il dialogo nella finestra principale.\nTasto destro: copia o incolla la traduzione.",
            Dock = DockStyle.Fill, Padding = new Padding(10, 7, 8, 4),
            ForeColor = Color.FromArgb(82, 99, 115), TextAlign = ContentAlignment.MiddleLeft
        }, 0, 2);
        Controls.Add(layout);

        _menu.Items.AddRange(new ToolStripItem[] { _copyOriginal, _copyTranslation, new ToolStripSeparator(), _pasteTranslation });
        _cards.ContextMenuStrip = _menu;
        _menu.Opening += (_, e) =>
        {
            e.Cancel = _cards.SelectedEntry is null;
            _copyOriginal.Enabled = !string.IsNullOrEmpty(_cards.SelectedEntry?.Original);
            _copyTranslation.Enabled = !string.IsNullOrEmpty(_cards.SelectedEntry?.Translation);
            _pasteTranslation.Enabled = HasClipboardText();
        };
        _copyOriginal.Click += (_, _) => Copy(true);
        _copyTranslation.Click += (_, _) => Copy(false);
        _pasteTranslation.Click += (_, _) => Paste();
        _cards.EntrySelected += entry => EntrySelected?.Invoke(entry);
        _cards.CopyRequested += original => Copy(original);
        _cards.PasteRequested += Paste;
        SetDocumentEmpty();
    }

    public void SetDocument(TranslationDocument document, TranslationEntry? selected)
    {
        ArgumentNullException.ThrowIfNull(document);
        var changedDocument = !ReferenceEquals(_document, document);
        _document = document;
        _cards.SetEntries(document.Entries, preserveScroll: !changedDocument);
        _cards.SelectEntry(selected, scrollIntoView: changedDocument);
        _count.Text = $"DIALOGHI · {document.Entries.Count} battute · traduzione {document.SelectedLanguage + 1}";
    }

    public void RefreshEntry(TranslationEntry entry) => _cards.RefreshEntry(entry);

    public void SelectEntry(TranslationEntry? entry, bool scrollIntoView = true) => _cards.SelectEntry(entry, scrollIntoView);

    private void SetDocumentEmpty() => _count.Text = "DIALOGHI · apri un file per iniziare";

    private static bool HasClipboardText()
    {
        try { return Clipboard.ContainsText(); }
        catch (ExternalException) { return false; }
    }

    private void Copy(bool original)
    {
        var entry = _cards.SelectedEntry;
        if (entry is null) return;
        var text = original ? entry.Original : entry.Translation;
        if (text.Length == 0) return;
        ClipboardAction(() => Clipboard.SetText(text));
    }

    private void Paste()
    {
        var entry = _cards.SelectedEntry;
        if (entry is null) return;
        ClipboardAction(() =>
        {
            if (!Clipboard.ContainsText()) return;
            // The owning editor performs the change and dirty tracking; this window never mutates the model.
            TranslationPasted?.Invoke(entry, Clipboard.GetText().Replace("\r\n", "\n").Replace('\r', '\n'));
        });
    }

    private void ClipboardAction(Action action)
    {
        try { action(); }
        catch (ExternalException)
        {
            MessageBox.Show(this, "Gli appunti sono occupati da un altro programma. Riprova.", "Appunti", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _menu.Dispose();
        base.Dispose(disposing);
    }

    /// <summary>Draws only visible cards, with measured heights and no per-entry native controls.</summary>
    private sealed class CardCanvas : Control
    {
        private const int Gap = 10;
        private const int Inset = 12;
        private const int TitleHeight = 30;
        private const int LabelHeight = 22;
        private const int DividerHeight = 17;
        private const int FooterHeight = 12;
        private readonly VScrollBar _scroll = new() { Dock = DockStyle.Right };
        private readonly Font _titleFont = new("Segoe UI", 10, FontStyle.Bold);
        private readonly Font _labelFont = new("Segoe UI", 8, FontStyle.Bold);
        private TranslationEntry[] _entries = Array.Empty<TranslationEntry>();
        private CardLayout[] _layouts = Array.Empty<CardLayout>();
        private readonly Dictionary<TranslationEntry, int> _indices = new();
        private int _totalHeight;
        private bool _layingOut;
        public TranslationEntry? SelectedEntry { get; private set; }
        public event Action<TranslationEntry>? EntrySelected;
        public event Action<bool>? CopyRequested;
        public event Action? PasteRequested;

        private int ViewWidth => Math.Max(1, ClientSize.Width - _scroll.Width);
        private int TextWidth => Math.Max(1, ViewWidth - Gap * 2 - Inset * 2);
        private int MaximumOffset => Math.Max(0, _totalHeight - ClientSize.Height);
        private const TextFormatFlags BodyFlags = TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl |
            TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding;

        public CardCanvas()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserPaint | ControlStyles.Selectable | ControlStyles.ResizeRedraw, true);
            TabStop = true;
            BackColor = Color.FromArgb(240, 243, 247);
            Font = new Font("Segoe UI", 10);
            AccessibleName = "Dialoghi verticali: originale e traduzione";
            Controls.Add(_scroll);
            _scroll.ValueChanged += (_, _) => Invalidate();
            _scroll.TabStop = false;
        }

        public void SetEntries(IEnumerable<TranslationEntry> entries, bool preserveScroll)
        {
            var anchor = preserveScroll ? CaptureAnchor() : default;
            _entries = entries.ToArray();
            _indices.Clear();
            for (var i = 0; i < _entries.Length; i++) _indices[_entries[i]] = i;
            if (SelectedEntry is not null && !_indices.ContainsKey(SelectedEntry)) SelectedEntry = null;
            Recalculate(anchor);
        }

        public void RefreshEntry(TranslationEntry entry)
        {
            if (!_indices.TryGetValue(entry, out var index)) return;
            var anchor = CaptureAnchor();
            var changed = MeasureCard(_layouts[index].Top, entry);
            var difference = changed.Height - _layouts[index].Height;
            _layouts[index] = changed;
            if (difference != 0)
            {
                for (var next = index + 1; next < _layouts.Length; next++)
                    _layouts[next] = _layouts[next] with { Top = _layouts[next].Top + difference };
                _totalHeight += difference;
                RestoreScroll(anchor);
            }
            Invalidate();
        }

        public void SelectEntry(TranslationEntry? entry, bool scrollIntoView)
        {
            if (entry is not null && !_indices.ContainsKey(entry)) return;
            SelectedEntry = entry;
            if (scrollIntoView && entry is not null)
            {
                var card = _layouts[_indices[entry]];
                var offset = _scroll.Value;
                if (card.Top < offset) SetOffset(card.Top - Gap);
                else if (card.Top + card.Height > offset + ClientSize.Height)
                    SetOffset(card.Height >= ClientSize.Height ? card.Top - Gap : card.Top + card.Height - ClientSize.Height + Gap);
            }
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_scroll is not null && !_layingOut) Recalculate(CaptureAnchor());
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            if (_scroll is not null && !_layingOut) Recalculate(CaptureAnchor());
        }

        private void Recalculate(ScrollAnchor anchor)
        {
            if (_layingOut) return;
            _layingOut = true;
            try
            {
                _layouts = new CardLayout[_entries.Length];
                int top = Gap;
                for (var i = 0; i < _entries.Length; i++)
                {
                    _layouts[i] = MeasureCard(top, _entries[i]);
                    top += _layouts[i].Height + Gap;
                }
                _totalHeight = _entries.Length == 0 ? 0 : top;
                RestoreScroll(anchor);
                Invalidate();
            }
            finally { _layingOut = false; }
        }

        private CardLayout MeasureCard(int top, TranslationEntry entry)
        {
            var original = MeasureBody(entry.Original);
            var translation = MeasureBody(TranslationText(entry));
            return new CardLayout(top, TitleHeight + LabelHeight * 2 + original + DividerHeight + translation + FooterHeight,
                original, translation);
        }

        private int MeasureBody(string text) => Math.Max(Font.Height + 3,
            TextRenderer.MeasureText(text.Length == 0 ? " " : text, Font,
                new Size(TextWidth, int.MaxValue), BodyFlags).Height + 3);

        private static string TranslationText(TranslationEntry entry) => entry.Translation.Length == 0
            ? entry.HasTranslation ? "(traduzione intenzionalmente vuota)" : "(da tradurre)"
            : entry.Translation;

        private ScrollAnchor CaptureAnchor()
        {
            if (_layouts.Length == 0) return default;
            var index = FindCardAt(_scroll.Value);
            return new ScrollAnchor(_entries[index], _scroll.Value - _layouts[index].Top);
        }

        private void RestoreScroll(ScrollAnchor anchor)
        {
            var offset = anchor.Entry is not null && _indices.TryGetValue(anchor.Entry, out var index)
                ? _layouts[index].Top + Math.Min(anchor.Offset, _layouts[index].Height + Gap - 1) : 0;
            _scroll.Minimum = 0;
            _scroll.LargeChange = Math.Max(1, ClientSize.Height);
            _scroll.SmallChange = Math.Max(1, Font.Height * 3);
            _scroll.Maximum = Math.Max(_scroll.LargeChange - 1, _totalHeight - 1);
            _scroll.Enabled = MaximumOffset > 0;
            SetOffset(offset);
        }

        private void SetOffset(int value) => _scroll.Value = Math.Clamp(value, 0, MaximumOffset);

        private int FindCardAt(int y)
        {
            int low = 0, high = _layouts.Length - 1;
            while (low < high)
            {
                var middle = low + (high - low) / 2;
                if (_layouts[middle].Top + _layouts[middle].Height + Gap <= y) low = middle + 1;
                else high = middle;
            }
            return low;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_entries.Length == 0)
            {
                TextRenderer.DrawText(e.Graphics, "Nessun dialogo.\nApri un file nella finestra principale.", Font,
                    new Rectangle(20, 30, Math.Max(1, ViewWidth - 40), Math.Max(1, Height - 60)),
                    Color.FromArgb(92, 108, 124), BodyFlags);
                return;
            }
            var offset = _scroll.Value;
            for (var index = FindCardAt(offset); index < _entries.Length; index++)
            {
                var layout = _layouts[index];
                var top = layout.Top - offset;
                if (top > ClientSize.Height) break;
                DrawCard(e.Graphics, _entries[index], layout, top);
            }
        }

        private void DrawCard(Graphics graphics, TranslationEntry entry, CardLayout layout, int top)
        {
            bool selected = ReferenceEquals(entry, SelectedEntry);
            var rectangle = new Rectangle(Gap, top, Math.Max(1, ViewWidth - Gap * 2), layout.Height);
            using var fill = new SolidBrush(selected ? Color.FromArgb(236, 247, 247) : Color.White);
            graphics.FillRectangle(fill, rectangle);
            using var border = new Pen(selected ? Color.FromArgb(0, 112, 116) : Color.FromArgb(214, 223, 231), selected ? 2 : 1);
            graphics.DrawRectangle(border, rectangle.X, rectangle.Y, rectangle.Width - 1, rectangle.Height - 1);
            var color = SpeakerColor(entry.Speaker);
            using var stripe = new SolidBrush(color);
            graphics.FillRectangle(stripe, rectangle.X, rectangle.Y, 4, rectangle.Height);
            int left = rectangle.Left + Inset;
            var title = entry.Id + (string.IsNullOrWhiteSpace(entry.Speaker) ? "" : " · " + entry.Speaker);
            TextRenderer.DrawText(graphics, title, _titleFont,
                new Rectangle(left, top + 6, TextWidth, TitleHeight - 6), color,
                TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding);
            int y = top + TitleHeight;
            DrawLabel("ORIGINALE", y);
            y += LabelHeight;
            DrawBody(entry.Original, y, layout.OriginalHeight, Color.FromArgb(40, 53, 67));
            y += layout.OriginalHeight;
            using var divider = new Pen(Color.FromArgb(219, 226, 233));
            graphics.DrawLine(divider, left, y + DividerHeight / 2, left + TextWidth, y + DividerHeight / 2);
            y += DividerHeight;
            DrawLabel("TRADUZIONE", y);
            y += LabelHeight;
            DrawBody(TranslationText(entry), y, layout.TranslationHeight,
                entry.Translation.Length == 0 ? Color.FromArgb(125, 135, 145) : color);

            void DrawLabel(string text, int labelY) => TextRenderer.DrawText(graphics, text, _labelFont,
                new Rectangle(left, labelY, TextWidth, LabelHeight), Color.FromArgb(100, 116, 131),
                TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding);
            void DrawBody(string text, int bodyY, int height, Color ink) => TextRenderer.DrawText(graphics, text, Font,
                new Rectangle(left, bodyY, TextWidth, height), ink, BodyFlags);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button is not (MouseButtons.Left or MouseButtons.Right) || _entries.Length == 0 || e.X >= ViewWidth) return;
            var y = e.Y + _scroll.Value;
            var index = FindCardAt(y);
            var layout = _layouts[index];
            if (y < layout.Top || y > layout.Top + layout.Height) return;
            Focus();
            SelectEntry(_entries[index], false);
            EntrySelected?.Invoke(_entries[index]);
        }

        protected override bool IsInputKey(Keys keyData) => (keyData & Keys.KeyCode) is
            Keys.Up or Keys.Down or Keys.Home or Keys.End or Keys.PageUp or Keys.PageDown || base.IsInputKey(keyData);

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Control && e.KeyCode == Keys.C)
            {
                CopyRequested?.Invoke(e.Shift);
                e.SuppressKeyPress = true;
                return;
            }
            if (e.Control && e.KeyCode == Keys.V)
            {
                PasteRequested?.Invoke();
                e.SuppressKeyPress = true;
                return;
            }
            if (_entries.Length == 0) return;
            var selected = SelectedEntry is not null && _indices.TryGetValue(SelectedEntry, out var current) ? current : -1;
            var next = e.KeyCode switch
            {
                Keys.Up => Math.Max(0, selected - 1),
                Keys.Down => Math.Min(_entries.Length - 1, selected + 1),
                Keys.Home => 0,
                Keys.End => _entries.Length - 1,
                _ => -1
            };
            if (next >= 0)
            {
                SelectEntry(_entries[next], true);
                EntrySelected?.Invoke(_entries[next]);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode is Keys.PageUp or Keys.PageDown)
            {
                SetOffset(_scroll.Value + (e.KeyCode == Keys.PageUp ? -1 : 1) * Math.Max(1, Height - Font.Height));
                e.SuppressKeyPress = true;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            var lines = SystemInformation.MouseWheelScrollLines;
            var amount = lines < 0 ? Height : Math.Max(1, lines) * Font.Height;
            SetOffset(_scroll.Value - e.Delta / SystemInformation.MouseWheelScrollDelta * amount);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { _titleFont.Dispose(); _labelFont.Dispose(); }
            base.Dispose(disposing);
        }

        private readonly record struct CardLayout(int Top, int Height, int OriginalHeight, int TranslationHeight);
        private readonly record struct ScrollAnchor(TranslationEntry? Entry, int Offset);

        private static Color SpeakerColor(string speaker) => speaker switch
        {
            "C000_Saiha" => Color.MediumBlue,
            "C001_Momot" => Color.FromArgb(148, 65, 157),
            "C002_Hoshi" => Color.Navy,
            "C003_Amami" => Color.DarkGreen,
            "C004_Gokuh" => Color.ForestGreen,
            "C005_Oma__" => Color.Purple,
            "C006_Shing" => Color.Brown,
            "C007_Ki-Bo" => Color.SteelBlue,
            "C008_Tojo_" => Color.DarkCyan,
            "C009_Yumen" => Color.DarkRed,
            "C010_Haruk" => Color.IndianRed,
            "C011_Chaba" => Color.FromArgb(28, 103, 176),
            "C012_Shiro" => Color.MidnightBlue,
            "C013_Yonag" => Color.FromArgb(154, 88, 0),
            "C014_Iruma" or "C023_Mfunn" or "C037_Expin" => Color.FromArgb(175, 20, 104),
            "C015_Akama" => Color.FromArgb(168, 48, 111),
            "C021_Mtaro" or "C035_Exred" => Color.Firebrick,
            "C022_Msuke" or "C036_Exyel" => Color.FromArgb(164, 80, 0),
            "C024_Mdam_" or "C038_Exgre" => Color.Green,
            "C025_Mkid_" or "C039_Exblu" => Color.Blue,
            "C026_Eguis" or "C027_Mono5" => Color.DimGray,
            "C020_Monok" or "" => Color.FromArgb(38, 52, 66),
            _ => StableSpeakerColor(speaker)
        };

        private static Color StableSpeakerColor(string speaker)
        {
            uint hash = 2166136261;
            foreach (var character in speaker) hash = (hash ^ character) * 16777619;
            var palette = new[] { Color.Teal, Color.DarkSlateBlue, Color.SaddleBrown, Color.DarkOliveGreen, Color.DarkMagenta, Color.Maroon };
            return palette[hash % (uint)palette.Length];
        }
    }
}
