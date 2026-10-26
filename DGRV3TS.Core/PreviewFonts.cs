namespace DGRV3TS.Core;

public enum HummingWeight { Light, Medium, Bold }

/// <summary>Optional external V3 font discovery. No font files are bundled.</summary>
public static class PreviewFonts
{
    public const float V3Size = 22;
    public static string Suffix(HummingWeight weight) => weight switch
    { HummingWeight.Light => "L", HummingWeight.Medium => "M", HummingWeight.Bold => "B", _ => throw new ArgumentOutOfRangeException(nameof(weight)) };

    public static string[] InstalledNames(HummingWeight weight) =>
        ["FOT-Humming Std " + Suffix(weight), "FOT-HummingStd " + Suffix(weight), "FOT-HummingStd-" + Suffix(weight), "HummingStd-" + Suffix(weight)];

    public static HummingWeight? WeightFromName(string name)
    {
        var normalized = Normalize(Path.GetFileNameWithoutExtension(name));
        foreach (var weight in Enum.GetValues<HummingWeight>())
            if (normalized == "FOTHUMMINGSTD" + Suffix(weight) || normalized == "HUMMINGSTD" + Suffix(weight)) return weight;
        return null;
    }

    public static string? FindFile(string root, HummingWeight weight)
    {
        foreach (var directory in new[] { root, Path.Combine(root, "Fonts"), Path.Combine(root, "fonts") }.Distinct(StringComparer.Ordinal))
        {
            if (!Directory.Exists(directory)) continue;
            var found = Directory.EnumerateFiles(directory).OrderBy(p => p, StringComparer.Ordinal)
                .FirstOrDefault(p => (Path.GetExtension(p).Equals(".otf", StringComparison.OrdinalIgnoreCase)
                    || Path.GetExtension(p).Equals(".ttf", StringComparison.OrdinalIgnoreCase)) && WeightFromName(p) == weight);
            if (found is not null) return found;
        }
        return null;
    }

    public static string? DefaultFile(string root)
    {
        foreach (var name in new[] { "font.otf", "font.ttf" })
            if (VariableCatalog.FindFile(name, root) is { } explicitFont) return explicitFont;
        foreach (var weight in new[] { HummingWeight.Medium, HummingWeight.Light, HummingWeight.Bold })
            if (FindFile(root, weight) is { } file) return file;
        return null;
    }

    private static string Normalize(string value) => new(value.Where(char.IsLetterOrDigit).Select(char.ToUpperInvariant).ToArray());
}
