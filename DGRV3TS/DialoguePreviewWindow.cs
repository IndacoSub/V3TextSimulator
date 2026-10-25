using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace DGRV3TS;

/// <summary>The optional landscape image/audio preview; independent from the file's vertical summary.</summary>
internal sealed class DialoguePreviewWindow : Form
{
    private static readonly Color CanvasColor = Color.FromArgb(27, 35, 49);
    private readonly PictureBox picture = new() { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom, BackColor = CanvasColor };
    private readonly ComboBox games = new() { Width = 66, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox backgrounds = new() { Width = 190, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly NumericUpDown size = new() { Minimum = 8, Maximum = 90, Value = 29, Width = 58 };
    private readonly CheckBox spriteVisible = new() { Text = "Personaggio", Checked = true, AutoSize = true, Padding = new Padding(0, 5, 0, 0) };
    private readonly CheckBox colorTags = new() { Text = "Tag CLT", Checked = true, AutoSize = true, Padding = new Padding(0, 5, 0, 0) };
    private readonly CheckBox adaptiveHeight = new() { Text = "Interlinea adattiva", AutoSize = true, Padding = new Padding(0, 5, 0, 0) };
    private readonly Label status = new() { Dock = DockStyle.Fill, AutoEllipsis = true, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.DimGray };
    private readonly TextBox language = new() { Text = "it-IT", Width = 64 };
    private readonly SoundPlayer player = new();
    private readonly System.Windows.Forms.Timer renderTimer = new() { Interval = 90 };
    private PrivateFontCollection? customFont;
    private FontFamily family = FontFamily.GenericSansSerif;
    private bool ownsFamily;
    private MediaAssets assets = new(AppContext.BaseDirectory);
    private string text = "", speaker = "", expression = "", voice = "";
    private string? loadedDefaultFont;
    private object? speech;
    private bool binding;

    public event Action<string>? GameChanged;

    public DialoguePreviewWindow()
    {
        Text = "Anteprima grafica — V3 Text";
        Size = new Size(1120, 760);
        MinimumSize = new Size(700, 500);
        StartPosition = FormStartPosition.CenterParent;
        AutoScaleMode = AutoScaleMode.Dpi;
        Font = new Font("Segoe UI", 9);
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(8) };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 78));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        var controls = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = true, AutoScroll = true };
        games.Items.AddRange(new object[] { "DR", "AI" });
        games.SelectedIndex = 0;
        controls.Controls.AddRange(new Control[]
        {
            games, backgrounds, Button("Risorse…", ChooseRoot), Button("Ricarica", ReloadAssets), spriteVisible,
            colorTags, adaptiveHeight, Button("Font…", ChooseFont), Button("TTF/OTF…", LoadFontFile), size,
            Button("Voce WAV", PlayVoice), language, Button("Leggi testo", SpeakText), Button("Stop audio", StopAudio),
            Button("Copia immagine", CopyImage), Button("Salva PNG…", SaveImage)
        });
        layout.Controls.Add(controls, 0, 0);
        layout.Controls.Add(picture, 0, 1);
        layout.Controls.Add(status, 0, 2);
        Controls.Add(layout);
        games.SelectedIndexChanged += (_, _) => { GameChanged?.Invoke(Game); ReloadAssets(); };
        backgrounds.SelectedIndexChanged += (_, _) => QueueRender();
        size.ValueChanged += (_, _) => QueueRender();
        spriteVisible.CheckedChanged += (_, _) => QueueRender();
        colorTags.CheckedChanged += (_, _) => QueueRender();
        adaptiveHeight.CheckedChanged += (_, _) => QueueRender();
        renderTimer.Tick += (_, _) => { renderTimer.Stop(); Render(); };
        ReloadAssets();
    }

    public bool IsSpeaking
    {
        get
        {
            if (speech is null) return false;
            object? state = null;
            try { state = ((dynamic)speech).Status; return (int)((dynamic)state).RunningState == 2; }
            catch (COMException) { return false; }
            finally { if (state is not null && Marshal.IsComObject(state)) Marshal.FinalReleaseComObject(state); }
        }
    }

    public void SetAssetRoot(string path)
    {
        Safe(() => { assets = new MediaAssets(path); loadedDefaultFont = null; ReloadAssets(); });
    }

    public void SetGame(string game)
    {
        var index = string.Equals(game, "AI", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
        if (games.SelectedIndex != index) games.SelectedIndex = index;
    }

    public void SetDialogue(string text, string speaker, string expression, string voice)
    {
        var changed = this.voice != voice || this.speaker != speaker;
        this.text = text ?? "";
        this.speaker = speaker ?? "";
        this.expression = expression ?? "";
        this.voice = voice ?? "";
        if (changed) StopAudio();
        QueueRender();
    }

    public void ReloadAssets()
    {
        Safe(() =>
        {
            var selected = (backgrounds.SelectedItem as BackgroundItem)?.Path;
            binding = true;
            try
            {
                backgrounds.Items.Clear();
                backgrounds.Items.Add(new BackgroundItem(null));
                foreach (var path in assets.Backgrounds(Game)) backgrounds.Items.Add(new BackgroundItem(path));
                backgrounds.SelectedIndex = 0;
                for (var i = 1; i < backgrounds.Items.Count; i++)
                    if (backgrounds.Items[i] is BackgroundItem item && item.Path == selected) backgrounds.SelectedIndex = i;
                if (backgrounds.SelectedIndex == 0 && backgrounds.Items.Count > 1) backgrounds.SelectedIndex = 1;
            }
            finally { binding = false; }
            var defaultFont = assets.DefaultFont();
            if (defaultFont is not null && loadedDefaultFont != defaultFont)
            {
                LoadCustomFont(defaultFont);
                loadedDefaultFont = defaultFont;
            }
            Render();
        });
    }

    public void PlayVoice()
    {
        Safe(() =>
        {
            StopAudio();
            var file = assets.Voice(Game, voice);
            if (file is null) { status.Text = "Nessun file WAV corrispondente alla voce di questa battuta."; return; }
            player.SoundLocation = file;
            player.Play();
            status.Text = "Riproduzione: " + Path.GetFileName(file);
        });
    }

    public void SpeakText()
    {
        Safe(() =>
        {
            StopAudio();
            var type = Type.GetTypeFromProgID("SAPI.SpVoice") ?? throw new NotSupportedException("Sintesi vocale Windows non disponibile.");
            speech ??= Activator.CreateInstance(type) ?? throw new NotSupportedException("Impossibile inizializzare la sintesi vocale.");
            var culture = CultureInfo.GetCultureInfo(language.Text.Trim());
            dynamic synthesizer = speech;
            // Let installed SAPI voices determine availability; do not invent gender/age metadata.
            dynamic voices = synthesizer.GetVoices("Language=" + culture.LCID.ToString("X", CultureInfo.InvariantCulture), "");
            try
            {
                if ((int)voices.Count > 0) synthesizer.Voice = voices.Item(0);
                else status.Text = "Voce per " + culture.Name + " non installata: uso della voce predefinita.";
            }
            finally { if (Marshal.IsComObject(voices)) Marshal.FinalReleaseComObject(voices); }
            synthesizer.Speak(PlainText(text), 1);
        });
    }

    public void StopAudio()
    {
        player.Stop();
        if (speech is null) return;
        try { ((dynamic)speech).Speak("", 2); }
        catch (COMException) { /* An unavailable audio device must not affect the translation editor. */ }
    }

    public void CopyImage()
    {
        Safe(() => { Render(); if (picture.Image is not null) Clipboard.SetImage(picture.Image); });
    }

    public void SaveImage()
    {
        Safe(() =>
        {
            using var dialog = new SaveFileDialog { Filter = "Immagine PNG (*.png)|*.png", FileName = "dialogo.png", OverwritePrompt = true };
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            Render();
            picture.Image?.Save(dialog.FileName, ImageFormat.Png);
            status.Text = "Immagine salvata.";
        });
    }

    private string Game => games.SelectedItem?.ToString() ?? "DR";

    private static Button Button(string label, Action action)
    {
        var button = new Button { Text = label, AutoSize = true, Height = 28, Margin = new Padding(3, 0, 3, 5) };
        button.Click += (_, _) => action();
        return button;
    }

    private void ChooseRoot()
    {
        using var dialog = new FolderBrowserDialog { Description = "Cartella che contiene Graphics e Sound", UseDescriptionForTitle = true, SelectedPath = assets.Root };
        if (dialog.ShowDialog(this) == DialogResult.OK) SetAssetRoot(dialog.SelectedPath);
    }

    private void ChooseFont()
    {
        Safe(() =>
        {
            using var current = NewFont((float)size.Value);
            using var dialog = new FontDialog { Font = current, ShowEffects = false };
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            ClearFamily();
            family = new FontFamily(dialog.Font.FontFamily.Name);
            ownsFamily = true;
            size.Value = Math.Clamp((decimal)dialog.Font.SizeInPoints * 96 / 72, size.Minimum, size.Maximum);
            QueueRender();
        });
    }

    private void LoadFontFile()
    {
        using var dialog = new OpenFileDialog { Filter = "Font TrueType/OpenType|*.ttf;*.otf", Title = "Carica font per l'anteprima" };
        if (dialog.ShowDialog(this) == DialogResult.OK) Safe(() => { LoadCustomFont(dialog.FileName); QueueRender(); });
    }

    private void LoadCustomFont(string path)
    {
        var collection = new PrivateFontCollection();
        try
        {
            collection.AddFontFile(path);
            if (collection.Families.Length == 0) throw new InvalidDataException("Il file non contiene un font utilizzabile.");
            ClearFamily();
            customFont = collection;
            family = collection.Families[0];
        }
        catch { collection.Dispose(); throw; }
    }

    private void ClearFamily()
    {
        if (ownsFamily) family.Dispose();
        ownsFamily = false;
        customFont?.Dispose();
        customFont = null;
        family = FontFamily.GenericSansSerif;
    }

    private Font NewFont(float pixels)
    {
        var style = family.IsStyleAvailable(FontStyle.Regular) ? FontStyle.Regular : family.IsStyleAvailable(FontStyle.Bold) ? FontStyle.Bold : FontStyle.Italic;
        return new Font(family, Math.Clamp(pixels, 8, 120), style, GraphicsUnit.Pixel);
    }

    private void QueueRender()
    {
        if (binding || IsDisposed) return;
        renderTimer.Stop();
        renderTimer.Start();
    }

    private void Render()
    {
        if (IsDisposed) return;
        Safe(() =>
        {
            var image = new Bitmap(1280, 720, PixelFormat.Format32bppArgb);
            try
            {
                using var graphics = Graphics.FromImage(image);
                graphics.Clear(CanvasColor);
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                var backgroundPath = (backgrounds.SelectedItem as BackgroundItem)?.Path;
                if (backgroundPath is not null)
                {
                    using var background = MediaAssets.LoadImage(backgroundPath);
                    graphics.DrawImage(background, new Rectangle(0, 0, 1280, 720));
                }
                if (spriteVisible.Checked)
                {
                    var path = assets.Sprite(Game, speaker, expression);
                    if (path is not null)
                    {
                        using var sprite = MediaAssets.LoadImage(path);
                        var ratio = Math.Min(440f / sprite.Width, 680f / sprite.Height);
                        var width = sprite.Width * ratio;
                        var height = sprite.Height * ratio;
                        graphics.DrawImage(sprite, new RectangleF(0, 720 - height, width, height));
                    }
                }
                var isAiTutorial = Game == "AI" && Path.GetFileNameWithoutExtension(backgroundPath)?.Contains("Tutorial", StringComparison.OrdinalIgnoreCase) == true;
                var isToWitter = Game == "AI" && Path.GetFileNameWithoutExtension(backgroundPath)?.Equals("ToWitter", StringComparison.OrdinalIgnoreCase) == true;
                var bounds = isAiTutorial ? new RectangleF(335, 280, 850, 390) :
                    isToWitter ? new RectangleF(830, 175, 400, 470) :
                    Game == "AI" ? new RectangleF(355, 560, 880, 150) : new RectangleF(260, 575, 980, 140);
                if (backgroundPath is null)
                {
                    using var box = new SolidBrush(Color.FromArgb(225, 14, 21, 32));
                    graphics.FillRectangle(box, new RectangleF(220, 510, 1040, 195));
                    using var nameFont = NewFont(24);
                    using var nameBrush = new SolidBrush(Color.FromArgb(103, 224, 206));
                    graphics.DrawString(speaker, nameFont, nameBrush, 260, 530);
                }
                DrawText(graphics, text, bounds, isToWitter ? Color.Black : Color.White);
                var old = picture.Image;
                picture.Image = image;
                old?.Dispose();
                status.Text = (backgroundPath is null ? "Anteprima di base · nessuno sfondo caricato" : Path.GetFileName(backgroundPath)) +
                    " · " + (string.IsNullOrWhiteSpace(speaker) ? "nessun personaggio" : speaker) +
                    (string.IsNullOrWhiteSpace(expression) ? "" : " / " + expression);
            }
            catch { image.Dispose(); throw; }
        });
    }

    private void DrawText(Graphics graphics, string input, RectangleF bounds, Color initialColor)
    {
        // CLT changes are display-only. The source strings and raw tags remain in the document.
        input = ExpandNewlines(input);
        var matches = Regex.Matches(input, @"</?CLT(?:=[^<>\r\n]*)?>|<PLATFORM_(?:PC|CONSOLE)>|<SIGNAL_[^<>\r\n]+>", RegexOptions.CultureInvariant);
        var position = 0;
        var x = bounds.Left;
        var y = bounds.Top;
        var fontSize = (float)size.Value;
        var color = initialColor;
        var lineHeight = adaptiveHeight.Checked ? fontSize * 1.42f : 41;
        var state = graphics.Save();
        graphics.SetClip(bounds);
        using var format = new StringFormat(StringFormat.GenericTypographic) { FormatFlags = StringFormatFlags.MeasureTrailingSpaces };
        try
        {
            foreach (Match match in matches)
            {
                DrawRun(input[position..match.Index]);
                if (match.Value.StartsWith("<CLT", StringComparison.Ordinal) || match.Value == "</CLT>") ApplyTag(match.Value);
                position = match.Index + match.Length;
            }
            DrawRun(input[position..]);
        }
        finally { graphics.Restore(state); }

        void ApplyTag(string tag)
        {
            if (!colorTags.Checked) return;
            if (tag is "<CLT>" or "</CLT>" or "<CLT=cltNORMAL>" or "<CLT=typeNORMAL>")
            { color = initialColor; fontSize = (float)size.Value; return; }
            color = tag switch
            {
                "<CLT=cltSYSTEM>" => Color.Green,
                "<CLT=cltMIND>" => Color.Aqua,
                "<CLT=cltSTRONG>" => Color.Orange,
                "<CLT=cltWEAK>" => Color.Gold,
                _ => color
            };
            if (tag.StartsWith("<CLT=size", StringComparison.Ordinal) &&
                float.TryParse(tag[9..^1], NumberStyles.Float, CultureInfo.InvariantCulture, out var extra))
                fontSize = (float)size.Value + (extra switch
                {
                    1 => 0, 1.1f => .5f, 1.2f => .7f, 1.5f => 1,
                    _ => Math.Clamp(extra, 0, 60)
                });
        }

        void DrawRun(string run)
        {
            using var font = NewFont(fontSize);
            using var brush = new SolidBrush(color);
            foreach (Match word in Regex.Matches(run, @"\n|[^\S\n]+|[^\s]+", RegexOptions.CultureInvariant))
            {
                var token = word.Value;
                if (token == "\n") { NewLine(); continue; }
                var width = graphics.MeasureString(token, font, int.MaxValue, format).Width;
                if (x > bounds.Left && x + width > bounds.Right) NewLine();
                if (width <= bounds.Width)
                {
                    graphics.DrawString(token, font, brush, new PointF(x, y), format);
                    x += width;
                    if (adaptiveHeight.Checked) lineHeight = Math.Max(lineHeight, font.GetHeight(graphics));
                    continue;
                }
                var elements = StringInfo.GetTextElementEnumerator(token);
                while (elements.MoveNext())
                {
                    var glyph = elements.GetTextElement();
                    var glyphWidth = graphics.MeasureString(glyph, font, int.MaxValue, format).Width;
                    if (x > bounds.Left && x + glyphWidth > bounds.Right) NewLine();
                    graphics.DrawString(glyph, font, brush, new PointF(x, y), format);
                    x += glyphWidth;
                    if (adaptiveHeight.Checked) lineHeight = Math.Max(lineHeight, font.GetHeight(graphics));
                }
            }
        }

        void NewLine() { x = bounds.Left; y += lineHeight; lineHeight = adaptiveHeight.Checked ? fontSize * 1.42f : 41; }
    }

    private static string ExpandNewlines(string text)
    {
        var output = new StringBuilder(text.Length);
        for (var i = 0; i < text.Length; i++)
        {
            if (text[i] == '\\' && i + 1 < text.Length)
            {
                if (text[i + 1] == '\\') { output.Append('\\'); i++; continue; }
                if (text[i + 1] == 'n') { output.Append('\n'); i++; continue; }
                if (text[i + 1] == '"') { output.Append('"'); i++; continue; }
            }
            output.Append(text[i]);
        }
        return output.ToString().Replace("\r\n", "\n").Replace('\r', '\n');
    }

    private static string PlainText(string text) => Regex.Replace(ExpandNewlines(text), @"<[^<>\r\n]+>", "");

    private void Safe(Action action)
    {
        try { action(); }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or ArgumentException or ExternalException or InvalidOperationException or NotSupportedException or System.Reflection.TargetInvocationException)
        { status.Text = "Anteprima: " + error.Message; }
    }

    private sealed record BackgroundItem(string? Path)
    {
        public override string ToString() => Path is null ? "Sfondo di base" : System.IO.Path.GetFileNameWithoutExtension(Path);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            renderTimer.Stop();
            renderTimer.Dispose();
            StopAudio();
            player.Dispose();
            picture.Image?.Dispose();
            picture.Image = null;
            ClearFamily();
            if (speech is not null && Marshal.IsComObject(speech)) Marshal.FinalReleaseComObject(speech);
            speech = null;
        }
        base.Dispose(disposing);
    }
}
