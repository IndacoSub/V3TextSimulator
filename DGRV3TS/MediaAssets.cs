using System.Drawing.Imaging;

namespace DGRV3TS;

/// <summary>Optional user-supplied assets. Missing files never prevent translation.</summary>
internal sealed class MediaAssets
{
    public string Root { get; }

    public MediaAssets(string root) => Root = Path.GetFullPath(root);

    public IReadOnlyList<string> Backgrounds(string game)
    {
        var directories = game == "AI" ? new[] { "AI" } : new[] { "DR", "Danganronpa" };
        return directories.SelectMany(d => Enumerate(Path.Combine(Root, "Graphics", "Backgrounds", d), ".png"))
            .Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    public string? Sprite(string game, string speaker, string expression)
    {
        if (string.IsNullOrWhiteSpace(speaker)) return null;
        var sprites = Path.Combine(Root, "Graphics", "Sprites");
        if (!SafeName(speaker) || (!string.IsNullOrEmpty(expression) && !SafeName(expression))) return null;
        var character = Path.Combine(sprites, game, speaker);
        if (speaker is "chara_Blank" or "chara_Hatena")
            return Existing(Path.Combine(sprites, speaker, "image.png"));
        var animated = string.IsNullOrWhiteSpace(expression) ? null : Existing(Path.Combine(character, expression + ".png"));
        return animated ?? Existing(Path.Combine(character, "default.png"))
            ?? Existing(Path.Combine(sprites, "chara_Hatena", "image.png"));
    }

    public string? Voice(string game, string voice)
    {
        if (string.IsNullOrWhiteSpace(voice) || !SafeName(voice)) return null;
        var name = voice.EndsWith(".wav", StringComparison.OrdinalIgnoreCase) ? voice : voice + ".wav";
        var folder = Path.Combine(Root, "Sound", "Voices", game);
        var direct = Existing(Path.Combine(folder, name));
        if (direct is not null) return direct;
        return Enumerate(folder, ".wav").FirstOrDefault(path =>
            string.Equals(Path.GetFileName(path), name, StringComparison.OrdinalIgnoreCase))
            ?? Enumerate(Root, ".wav").FirstOrDefault(path =>
                string.Equals(Path.GetFileName(path), name, StringComparison.OrdinalIgnoreCase));
    }

    public string? DefaultFont() => Existing(Path.Combine(Root, "font.otf")) ?? Existing(Path.Combine(Root, "font.ttf"));

    public static int ImportSprites(string source, string assetRoot)
    {
        if (!Directory.Exists(source)) throw new DirectoryNotFoundException("Cartella degli sprite non trovata.");
        var mapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Shuichi Saihara"] = "C000_Saiha", ["Kaito Momota"] = "C001_Momot", ["Ryoma Hoshi"] = "C002_Hoshi",
            ["Rantaro Amami"] = "C003_Amami", ["Gonta Gokuhara"] = "C004_Gokuh", ["Kokichi Oma"] = "C005_Oma__",
            ["Korekiyo Shinguji"] = "C006_Shing", ["Keebo"] = "C007_Ki-Bo", ["Kirumi Toujo"] = "C008_Tojo_",
            ["Kirumi Tojo"] = "C008_Tojo_", ["Himiko Yumeno"] = "C009_Yumen", ["Maki Harukawa"] = "C010_Haruk",
            ["Tenko Chabashira"] = "C011_Chaba", ["Tsumugi Shirogane"] = "C012_Shiro", ["Angie Yonaga"] = "C013_Yonag",
            ["Miu Iruma"] = "C014_Iruma", ["Kaede Akamatsu"] = "C015_Akama", ["Monokuma"] = "C020_Monok",
            ["Monotaro"] = "C021_Monot", ["Monosuke"] = "C022_Msuke", ["Monophanie"] = "C023_Mfunn",
            ["Monodam"] = "C024_Mdam_", ["Monokid"] = "C025_Mkid_"
        };
        var copied = 0;
        foreach (var file in Enumerate(source, ".png"))
        {
            var name = Path.GetFileName(file);
            if (!name.StartsWith("stand", StringComparison.OrdinalIgnoreCase)) continue;
            if (file.Contains("Tsumugi Cosplay Sprites", StringComparison.OrdinalIgnoreCase)) continue;
            string? character = null;
            if (file.Contains("Fixed Sprites", StringComparison.OrdinalIgnoreCase))
                character = name.ToLowerInvariant() switch
                {
                    "stand_000_320.png" or "stand_000_331.png" => "C000_Saiha",
                    "stand_025_010.png" => "C025_Mkid_", _ => null
                };
            else if (file.Contains("Tsumugi Cosplay", StringComparison.OrdinalIgnoreCase)) character = "C012_Shiro";
            else character = mapping.FirstOrDefault(pair => file.Contains(pair.Key, StringComparison.OrdinalIgnoreCase)).Value;
            var separator = name.LastIndexOf('_');
            if (character is null || separator < 0 || separator == name.Length - 1) continue;
            var directory = Path.Combine(Path.GetFullPath(assetRoot), "Graphics", "Sprites", "DR", character);
            var destination = Path.Combine(directory, "anim" + name[(separator + 1)..]);
            if (File.Exists(destination)) continue;
            Directory.CreateDirectory(directory);
            File.Copy(file, destination, false);
            copied++;
        }
        return copied;
    }

    public static Bitmap LoadImage(string path)
    {
        // Decode into an owned bitmap so replacing an asset externally is not blocked by an open file.
        using var stream = File.OpenRead(path);
        using var decoded = Image.FromStream(stream, false, true);
        if ((long)decoded.Width * decoded.Height > 64_000_000) throw new InvalidDataException("Immagine troppo grande per l'anteprima.");
        var result = new Bitmap(decoded.Width, decoded.Height, PixelFormat.Format32bppArgb);
        using var graphics = Graphics.FromImage(result);
        graphics.DrawImageUnscaled(decoded, 0, 0);
        return result;
    }

    private static bool SafeName(string name) => name != "." && name != ".." &&
        name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0 && !name.Contains('/') && !name.Contains('\\');

    private static string? Existing(string path) => File.Exists(path) ? path : null;

    private static IEnumerable<string> Enumerate(string directory, string extension)
    {
        if (!Directory.Exists(directory)) return Array.Empty<string>();
        return Directory.EnumerateFiles(directory, "*", new EnumerationOptions
        {
            RecurseSubdirectories = true, IgnoreInaccessible = true, AttributesToSkip = FileAttributes.ReparsePoint
        }).Where(p => string.Equals(Path.GetExtension(p), extension, StringComparison.OrdinalIgnoreCase));
    }
}
