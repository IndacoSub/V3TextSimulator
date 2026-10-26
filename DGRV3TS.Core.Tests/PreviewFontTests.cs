using DGRV3TS.Core;

internal static class PreviewFontTests
{
    public static void Run(string root)
    {
        var directory = Path.Combine(root, "invented-font-selection");
        var fonts = Path.Combine(directory, "Fonts"); Directory.CreateDirectory(fonts);
        Equal(22f, PreviewFonts.V3Size);
        Equal<HummingWeight?>(HummingWeight.Bold, PreviewFonts.WeightFromName("FOT-Humming Std B"));
        Equal<HummingWeight?>(HummingWeight.Light, PreviewFonts.WeightFromName("FOT-HummingStd-L.otf"));
        Equal<HummingWeight?>(HummingWeight.Medium, PreviewFonts.WeightFromName("HummingStd-M"));
        Equal<HummingWeight?>(null, PreviewFonts.WeightFromName("AnotherFont-B.otf"));
        Equal<string?>(null, PreviewFonts.FindFile(directory, HummingWeight.Bold));
        var medium = Path.Combine(fonts, "FOT-HummingStd-M.otf");
        var light = Path.Combine(fonts, "FOT-HummingStd-L.otf");
        var bold = Path.Combine(fonts, "FOT-HummingStd-B.otf");
        // File-discovery markers only; no external font data is in the test fixture.
        foreach (var path in new[] { medium, light, bold }) File.WriteAllBytes(path, []);
        Equal(medium, PreviewFonts.DefaultFile(directory));
        Equal(light, PreviewFonts.FindFile(directory, HummingWeight.Light));
        Equal(bold, PreviewFonts.FindFile(directory, HummingWeight.Bold));
        var rootBold = Path.Combine(directory, "fot-hummingstd-b.OTF");
        File.WriteAllBytes(rootBold, []);
        Equal(rootBold, PreviewFonts.FindFile(directory, HummingWeight.Bold));
        var explicitFont = Path.Combine(directory, "font.ttf"); File.WriteAllBytes(explicitFont, []);
        Equal(explicitFont, PreviewFonts.DefaultFile(directory));
        File.Delete(explicitFont); File.Delete(medium);
        Equal(light, PreviewFonts.DefaultFile(directory));
        Equal<string?>(null, PreviewFonts.FindFile(directory, HummingWeight.Medium));
    }

    private static void Equal<T>(T expected, T actual)
    { if (!EqualityComparer<T>.Default.Equals(expected, actual)) throw new Exception($"Expected [{expected}], actual [{actual}]."); }
}
