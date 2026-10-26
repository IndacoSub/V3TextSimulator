using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using DGRV3TS.Core;

namespace DGRV3TS;

/// <summary>The optional landscape image/audio preview; independent from the file's vertical summary.</summary>
internal sealed class DialoguePreviewWindow : Form
{
    private static readonly Color CanvasColor = Color.FromArgb(27, 35, 49);
    private readonly PreviewImageBox picture = new() { Dock = DockStyle.Fill, Margin = Padding.Empty, BackColor = CanvasColor };
    private readonly ComboBox games = new() { Width = 66, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox backgrounds = new() { Width = 190, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly NumericUpDown size = new() { Minimum = 1, Maximum = 100, Value = (decimal)PreviewFonts.V3Size, Width = 58 };
    private readonly ComboBox hummingWeight = new() { Width = 135, DropDownStyle = ComboBoxStyle.DropDownList, AccessibleName = "Peso FOT-HummingStd" };
    private string? hummingDirectory;
    private bool bindingFont;
    private int activeHummingIndex = (int)HummingWeight.Medium;
    private readonly CheckBox spriteVisible = new() { Text = "Personaggio", Checked = true, AutoSize = true, Padding = new Padding(0, 5, 0, 0) };
    private readonly CheckBox colorTags = new() { Text = "Tag CLT", Checked = true, AutoSize = true, Padding = new Padding(0, 5, 0, 0) };
    private readonly CheckBox adaptiveHeight = new() { Text = "Interlinea adattiva", AutoSize = true, Padding = new Padding(0, 5, 0, 0) };
    private readonly CheckBox wordWrap = new() { Text = "A capo automatico", AutoSize = true, Padding = new Padding(0, 5, 0, 0) };
    private readonly CheckBox autoFrame = new() { Text = "Inquadratura automatica", Checked = true, AutoSize = true, Padding = new Padding(0, 5, 0, 0) };
    private readonly Dictionary<string, (long Modified, long Length, SpriteRegion Bounds)> spriteBounds = new(StringComparer.OrdinalIgnoreCase);
    private readonly Label status = new() { Dock = DockStyle.Fill, AutoEllipsis = true, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.DimGray };
    private readonly TextBox language = new() { Text = "it-IT", Width = 64 };
    private SoundPlayer? activePlayer;
    private Task? voicePlayback;
    private CancellationTokenSource? voiceCancellation;
    private int audioGeneration;
    private readonly System.Windows.Forms.Timer renderTimer = new() { Interval = 90 };
    private PrivateFontCollection? customFont;
    private FontFamily family = FontFamily.GenericSansSerif;
    private bool ownsFamily;
    private FontStyle selectedStyle = FontStyle.Regular;
    private bool useOriginalLanguage;
    private MediaAssets assets = new(AppContext.BaseDirectory);
    private string text = "", speaker = "", expression = "", voice = "", origin = "", original = "";
    private PreviewFileVersion? loadedDefaultFont;
    private string? activeFontFile;
    private PreviewFileVersion? activeFontVersion;
    private string loadedGame = "";
    private object? speech;
    private object? defaultSpeechVoice;
    private bool binding;

    public event Action<string>? GameChanged;
    public event Action<string>? AssetRootChanged;

    public DialoguePreviewWindow()
    {
        Text = "Anteprima grafica — V3 Text";
        Size = new Size(960, 650);
        MinimumSize = new Size(480, 300);
        StartPosition = FormStartPosition.CenterParent;
        AutoScaleMode = AutoScaleMode.Dpi;
        Font = new Font("Segoe UI", 9);
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(8, 8, 8, 0) };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 78));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var controls = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = true, AutoScroll = true };
        games.Items.AddRange(new object[] { "DR", "AI" });
        games.SelectedIndex = 0;
        hummingWeight.Items.AddRange(new object[] { "Humming · Leggero", "Humming · Medio", "Humming · Grassetto" });
        hummingWeight.SelectedIndex = (int)HummingWeight.Medium;
        controls.Controls.AddRange(new Control[]
        {
            games, backgrounds, Button("Risorse…", ChooseRoot), Button("Ricarica", ReloadAssets), spriteVisible,
            colorTags, adaptiveHeight, wordWrap, autoFrame, Button("Font…", ChooseFont), Button("TTF/OTF…", LoadFontFile), hummingWeight, size,
            Button("Voce WAV", PlayVoice), language, Button("Leggi testo", SpeakText), Button("Stop audio", StopAudio),
            Button("Copia immagine", CopyImage), Button("Salva PNG…", SaveImage)
        });
        layout.Controls.Add(controls, 0, 0);
        layout.Controls.Add(status, 0, 1);
        layout.Controls.Add(picture, 0, 2);
        Controls.Add(layout);
        games.SelectedIndexChanged += (_, _) => { GameChanged?.Invoke(Game); ReloadAssets(); };
        backgrounds.SelectedIndexChanged += (_, _) =>
        {
            if (binding) return;
            if (Game == "AI") size.Value = (decimal)PreviewLayout.Text(Game, (backgrounds.SelectedItem as BackgroundItem)?.ToString()).DefaultFontSize;
            QueueRender();
        };
        size.ValueChanged += (_, _) => QueueRender();
        spriteVisible.CheckedChanged += (_, _) => QueueRender();
        colorTags.CheckedChanged += (_, _) => QueueRender();
        adaptiveHeight.CheckedChanged += (_, _) => QueueRender();
        wordWrap.CheckedChanged += (_, _) => QueueRender();
        autoFrame.CheckedChanged += (_, _) => QueueRender();
        hummingWeight.SelectedIndexChanged += (_, _) =>
        {
            if (bindingFont || hummingWeight.SelectedIndex < 0) return;
            Safe(() =>
            {
                try
                {
                    var requested = (HummingWeight)hummingWeight.SelectedIndex;
                    var path = hummingDirectory is null ? null : PreviewFonts.FindFile(hummingDirectory, requested);
                    path ??= PreviewFonts.FindFile(assets.Root, requested);
                    if (path is not null) LoadCustomFont(path);
                    else if (!TryInstalledHumming(requested))
                    {
                        status.Text = "Variante FOT-HummingStd non disponibile: carica il relativo file TTF/OTF.";
                        return;
                    }
                    activeHummingIndex = (int)requested;
                    QueueRender();
                }
                finally
                {
                    bindingFont = true;
                    try { hummingWeight.SelectedIndex = activeHummingIndex; }
                    finally { bindingFont = false; }
                }
            });
        };
        renderTimer.Tick += (_, _) => { renderTimer.Stop(); Render(); };
        SetDefaultFamily();
        ReloadAssets();
    }

    public bool IsSpeaking
    {
        get
        {
            if (voicePlayback is { IsCompleted: false }) return true;
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

    public void SetDialogue(string text, string speaker, string expression, string voice, string origin = "", string original = "", bool sourceLanguage = false)
    {
        var changed = this.voice != voice || this.speaker != speaker;
        this.text = text ?? "";
        this.speaker = speaker ?? "";
        this.expression = expression ?? "";
        this.voice = voice ?? "";
        this.origin = origin ?? "";
        this.original = original ?? "";
        useOriginalLanguage = sourceLanguage;
        if (changed) StopAudio();
        QueueRender();
    }

    public void ReloadAssets()
    {
        spriteBounds.Clear();
        Safe(() =>
        {
            var selected = (backgrounds.SelectedItem as BackgroundItem)?.Path;
            var hadSelection = backgrounds.SelectedIndex >= 0;
            var available = assets.Backgrounds(Game);
            var restored = PreviewResources.BackgroundAfterReload(available, selected, hadSelection);
            binding = true;
            try
            {
                backgrounds.Items.Clear();
                backgrounds.Items.Add(new BackgroundItem(null));
                foreach (var path in available) backgrounds.Items.Add(new BackgroundItem(path));
                backgrounds.SelectedIndex = 0;
                for (var i = 1; i < backgrounds.Items.Count; i++)
                    if (backgrounds.Items[i] is BackgroundItem item && item.Path == restored) backgrounds.SelectedIndex = i;
            }
            finally { binding = false; }
            var currentPath = (backgrounds.SelectedItem as BackgroundItem)?.Path;
            if (loadedGame != Game || Game == "AI" && selected != currentPath)
                size.Value = (decimal)PreviewLayout.Text(Game, (backgrounds.SelectedItem as BackgroundItem)?.ToString()).DefaultFontSize;
            loadedGame = Game;
            var defaultFont = assets.DefaultFont();
            var fontVersion = defaultFont is null ? (PreviewFileVersion?)null : PreviewFileVersion.Read(defaultFont);
            if (defaultFont is not null && loadedDefaultFont?.Path != fontVersion?.Path)
            {
                LoadCustomFont(defaultFont);
            }
            else if (activeFontFile is not null && File.Exists(activeFontFile)
                && activeFontVersion != PreviewFileVersion.Read(activeFontFile)) LoadCustomFont(activeFontFile);
            loadedDefaultFont = fontVersion;
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
            var player = new SoundPlayer(file);
            try { player.Load(); }
            catch { player.Dispose(); throw; }
            activePlayer = player;
            voiceCancellation = new CancellationTokenSource();
            voicePlayback = PlayWaveAsync(player, voiceCancellation, audioGeneration);
            status.Text = "Riproduzione: " + Path.GetFileName(file);
        });
    }

    public bool HasVoiceFile => assets.Voice(Game, voice) is not null;

    private async Task PlayWaveAsync(SoundPlayer player, CancellationTokenSource cancellation, int generation)
    {
        try
        {
            await Task.Run(() => { cancellation.Token.ThrowIfCancellationRequested(); player.PlaySync(); }, cancellation.Token);
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested) { }
        catch (Exception error) when (error is IOException or InvalidOperationException or ExternalException)
        { if (!IsDisposed && generation == audioGeneration) status.Text = "Audio: " + error.Message; }
        finally
        {
            if (ReferenceEquals(activePlayer, player)) activePlayer = null;
            if (ReferenceEquals(voiceCancellation, cancellation)) voiceCancellation = null;
            player.Dispose();
            cancellation.Dispose();
        }
    }

    public void SpeakText()
    {
        Safe(() =>
        {
            StopAudio();
            var type = Type.GetTypeFromProgID("SAPI.SpVoice") ?? throw new NotSupportedException("Sintesi vocale Windows non disponibile.");
            if (speech is null)
            {
                speech = Activator.CreateInstance(type) ?? throw new NotSupportedException("Impossibile inizializzare la sintesi vocale.");
                defaultSpeechVoice = ((dynamic)speech).Voice;
            }
            var culture = CultureInfo.GetCultureInfo(useOriginalLanguage ? "en-US" : language.Text.Trim());
            dynamic synthesizer = speech;
            var preferred = Game == "DR" ? VoiceProfile.ForSpeaker(speaker, origin) : null;
            var attributes = "Language=" + culture.LCID.ToString("X", CultureInfo.InvariantCulture);
            dynamic voices = synthesizer.GetVoices(attributes,
                preferred is null ? "" : "Gender=" + preferred.Gender + ";Age=" + preferred.Age);
            try
            {
                if ((int)voices.Count > 0) synthesizer.Voice = voices.Item(0);
                else
                {
                    if (defaultSpeechVoice is not null) synthesizer.Voice = defaultSpeechVoice;
                    status.Text = "Voce per " + culture.Name + " non installata: uso della voce predefinita.";
                }
            }
            finally { if (Marshal.IsComObject(voices)) Marshal.FinalReleaseComObject(voices); }
            synthesizer.Speak(PreviewTextRules.Speech(text), 1);
        });
    }

    public void StopAudio()
    {
        audioGeneration++;
        voiceCancellation?.Cancel();
        try { activePlayer?.Stop(); }
        catch (ObjectDisposedException) { }
        activePlayer = null;
        if (speech is null) return;
        try { ((dynamic)speech).Speak("", 2); }
        catch (COMException) { /* An unavailable audio device must not affect the translation editor. */ }
    }

    public void CopyImage()
    {
        Safe(() => { if (Render() && picture.Image is not null) Clipboard.SetImage(picture.Image); });
    }

    public void SaveImage()
    {
        Safe(() =>
        {
            using var dialog = new SaveFileDialog { Filter = "Immagine PNG (*.png)|*.png", FileName = "dialogo.png", OverwritePrompt = true };
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            if (!Render() || picture.Image is null) return;
            picture.Image.Save(dialog.FileName, ImageFormat.Png);
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
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            SetAssetRoot(dialog.SelectedPath);
            AssetRootChanged?.Invoke(assets.Root);
        }
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
            selectedStyle = dialog.Font.Style;
            size.Value = Math.Clamp((decimal)dialog.Font.SizeInPoints, size.Minimum, size.Maximum);
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
            var version = PreviewFileVersion.Read(path);
            collection.AddFontFile(path);
            if (collection.Families.Length == 0) throw new InvalidDataException("Il file non contiene un font utilizzabile.");
            ClearFamily();
            customFont = collection;
            family = collection.Families[0];
            activeFontFile = Path.GetFullPath(path);
            activeFontVersion = version;
            var weight = PreviewFonts.WeightFromName(path) ?? PreviewFonts.WeightFromName(family.Name);
            if (weight.HasValue)
            {
                hummingDirectory = Path.GetDirectoryName(Path.GetFullPath(path));
                activeHummingIndex = (int)weight.Value;
                bindingFont = true;
                try { hummingWeight.SelectedIndex = (int)weight.Value; }
                finally { bindingFont = false; }
            }
        }
        catch { collection.Dispose(); throw; }
    }

    private void ClearFamily()
    {
        if (ownsFamily) family.Dispose();
        ownsFamily = false;
        customFont?.Dispose();
        customFont = null;
        activeFontFile = null;
        activeFontVersion = null;
        family = FontFamily.GenericSansSerif;
        selectedStyle = FontStyle.Regular;
    }

    private void SetDefaultFamily()
    {
        foreach (var weight in new[] { HummingWeight.Medium, HummingWeight.Light, HummingWeight.Bold })
            if (TryInstalledHumming(weight))
            {
                activeHummingIndex = (int)weight;
                bindingFont = true;
                try { hummingWeight.SelectedIndex = activeHummingIndex; }
                finally { bindingFont = false; }
                return;
            }
        try { family = new FontFamily("Calibri"); ownsFamily = true; }
        catch (ArgumentException) { family = FontFamily.GenericSansSerif; ownsFamily = false; }
    }

    private bool TryInstalledHumming(HummingWeight weight)
    {
        foreach (var name in PreviewFonts.InstalledNames(weight))
        {
            FontFamily candidate;
            try { candidate = new FontFamily(name); }
            catch (ArgumentException) { continue; }
            ClearFamily();
            family = candidate;
            ownsFamily = true;
            selectedStyle = FontStyle.Regular;
            return true;
        }
        return false;
    }

    private Font NewFont(float points)
    {
        var style = family.IsStyleAvailable(selectedStyle) ? selectedStyle : family.IsStyleAvailable(FontStyle.Regular) ? FontStyle.Regular : family.IsStyleAvailable(FontStyle.Bold) ? FontStyle.Bold : FontStyle.Italic;
        return new Font(family, Math.Clamp(points, 1, 120), style, GraphicsUnit.Point);
    }

    private void QueueRender()
    {
        if (binding || IsDisposed) return;
        renderTimer.Stop();
        renderTimer.Start();
    }

    private bool Render()
    {
        if (IsDisposed) return false;
        return Safe(() =>
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
                var backgroundName = Path.GetFileNameWithoutExtension(backgroundPath) ?? "";
                var profile = PreviewLayout.Text(Game, backgroundName);
                var bounds = new RectangleF(profile.X, profile.Y, 1280 - profile.X, 720 - profile.Y);
                // Draw artwork before the dialogue. Even manual/legacy framing cannot paint over glyphs.
                if (spriteVisible.Checked)
                {
                    var path = assets.Sprite(Game, speaker, expression);
                    if (path is not null)
                    {
                        using var sprite = MediaAssets.LoadImage(path);
                        if (autoFrame.Checked && Game == "DR")
                        {
                            var info = new FileInfo(path);
                            if (!spriteBounds.TryGetValue(path, out var cached) || cached.Modified != info.LastWriteTimeUtc.Ticks || cached.Length != info.Length)
                                spriteBounds[path] = cached = (info.LastWriteTimeUtc.Ticks, info.Length, MediaAssets.VisibleBounds(sprite));
                            if (cached.Bounds.Width > 0 && cached.Bounds.Height > 0)
                            {
                                var portrait = Path.GetFileName(path).Equals("default.png", StringComparison.OrdinalIgnoreCase)
                                    || speaker is "chara_Blank" or "chara_Hatena";
                                var frame = PreviewLayout.ReadableSprite(cached.Bounds, portrait, profile.X);
                                var target = frame.Destination; var source = frame.Source;
                                graphics.DrawImage(sprite, new Rectangle(target.X, target.Y, target.Width, target.Height),
                                    source.X, source.Y, source.Width, source.Height, GraphicsUnit.Pixel);
                            }
                        }
                        else
                        {
                            var placement = PreviewLayout.Sprite(speaker, expression, sprite.Width, sprite.Height);
                            graphics.DrawImage(sprite, new Rectangle(placement.X, placement.Y, placement.Width, placement.Height));
                        }
                    }
                }
                if (backgroundPath is null)
                {
                    using var box = new SolidBrush(Color.FromArgb(225, 14, 21, 32));
                    graphics.FillRectangle(box, new RectangleF(220, 510, 1040, 195));
                    using var nameFont = NewFont(24);
                    using var nameBrush = new SolidBrush(Color.FromArgb(103, 224, 206));
                    graphics.DrawString(speaker, nameFont, nameBrush, 260, 530);
                }
                var content = PreviewTextRules.Prepare(text, Game, backgroundName);
                DrawText(graphics, content.Text, bounds, profile.DarkText ? Color.Black : Color.White,
                    profile.NewlineHeight, content.AutoTranslated);
                var old = picture.Image;
                picture.Image = image;
                old?.Dispose();
                status.Text = (backgroundPath is null ? "Anteprima di base · nessuno sfondo caricato" : Path.GetFileName(backgroundPath)) +
                    " · " + (string.IsNullOrWhiteSpace(speaker) ? "nessun personaggio" : speaker) +
                    (string.IsNullOrWhiteSpace(expression) ? "" : " / " + expression) +
                    $" · {family.Name} · {size.Value} pt";
            }
            catch { image.Dispose(); throw; }
        });
    }

    private void DrawText(Graphics graphics, string input, RectangleF bounds, Color initialColor,
        int fixedLineHeight, bool automatic)
    {
        // v25 uses point-sized fonts, explicit line breaks and no automatic reflow by default.
        var matches = Regex.Matches(input, @"</?CLT(?:=[^<>\r\n]*)?>|<PLATFORM_(?:PC|CONSOLE)>|<SIGNAL_[^<>\r\n]+>", RegexOptions.CultureInvariant);
        var position = 0;
        var x = bounds.Left;
        var y = bounds.Top;
        var fontSize = (float)size.Value;
        var color = initialColor;
        var lineHeight = (float)fixedLineHeight;
        var state = graphics.Save();
        graphics.SetClip(bounds);
        // CLT boundaries must not introduce GDI+'s default per-run padding.
        using var format = new StringFormat(StringFormat.GenericTypographic);
        format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
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
            if (tag.StartsWith("<CLT=size", StringComparison.Ordinal))
                fontSize = PreviewTextRules.FontSize(tag, (float)size.Value);
        }

        void DrawRun(string run)
        {
            using var font = NewFont(fontSize);
            using var brush = new SolidBrush(automatic ? Color.HotPink : color);
            var lines = run.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                if (i > 0) NewLine(font);
                if (lines[i].Length == 0) continue;
                if (!wordWrap.Checked)
                {
                    graphics.DrawString(lines[i], font, brush, new PointF(x, y), format);
                    x += graphics.MeasureString(lines[i], font, int.MaxValue, format).Width;
                    continue;
                }
                foreach (Match part in Regex.Matches(lines[i], @"\s+|\S+", RegexOptions.CultureInvariant))
                {
                    var width = graphics.MeasureString(part.Value, font, int.MaxValue, format).Width;
                    if (x > bounds.Left && x + width > bounds.Right) NewLine(font);
                    graphics.DrawString(part.Value, font, brush, new PointF(x, y), format);
                    x += width;
                }
            }
        }

        void NewLine(Font font)
        {
            lineHeight = adaptiveHeight.Checked ? graphics.MeasureString("\n", font).Height : fixedLineHeight;
            x = bounds.Left;
            y += lineHeight;
        }
    }

    private bool Safe(Action action)
    {
        try { action(); return true; }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or ArgumentException or ExternalException or InvalidOperationException or NotSupportedException or System.Reflection.TargetInvocationException)
        { status.Text = "Anteprima: " + error.Message; return false; }
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
            picture.Image?.Dispose();
            picture.Image = null;
            ClearFamily();
            if (defaultSpeechVoice is not null && Marshal.IsComObject(defaultSpeechVoice)) Marshal.FinalReleaseComObject(defaultSpeechVoice);
            defaultSpeechVoice = null;
            if (speech is not null && Marshal.IsComObject(speech)) Marshal.FinalReleaseComObject(speech);
            speech = null;
        }
        base.Dispose(disposing);
    }
}
