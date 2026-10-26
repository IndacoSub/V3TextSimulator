using DGRV3TS.Core;

internal static class PreviewLayoutTests
{
    public static void Run()
    {
        // Oracles copied from the explicit v25 ScaleImage/V3Draw operations, not the rewrite's old general fit.
        Equal(new SpritePlacement(-60, 370, 340, 680), PreviewLayout.Sprite("C000_Saiha", "anim000", 1000, 2000));
        Equal(new SpritePlacement(-60, 603, 430, 215), PreviewLayout.Sprite("C015_Akama", "anim000", 1000, 500));
        Equal(new SpritePlacement(-60, 427, 430, 566), PreviewLayout.Sprite("C000_Saiha", "anim003", 501, 660));

        foreach (var speaker in new[] { "C020_Monok", "C021_Mtaro", "C022_Msuke", "C023_Mfunn", "C024_Mdam_", "C025_Mkid_" })
            Equal(new SpritePlacement(-120, 400, 440, 440), PreviewLayout.Sprite(speaker, "anim001", 800, 800));
        Equal(new SpritePlacement(-120, 400, 220, 440), PreviewLayout.Sprite("C020_Monok", "anim001", 400, 800));

        Equal(new SpritePlacement(0, 80, 320, 640), PreviewLayout.Sprite("C010_Haruk", "anim002", 320, 640));
        Equal(new SpritePlacement(0, -80, 500, 800), PreviewLayout.Sprite("C000_Saiha_kak", "anim002", 500, 800));
        foreach (var speaker in new[] { "chara_Blank", "chara_Hatena", "non", "None", "" })
            Equal(new SpritePlacement(0, 320, 300, 400), PreviewLayout.Sprite(speaker, "nonempty", 300, 400));

        // An empty expression wins over the character-specific full-body special cases.
        Equal(new SpritePlacement(0, 440, 140, 280), PreviewLayout.Sprite("C010_Haruk", "", 500, 1000));
        Equal(new SpritePlacement(0, 440, 280, 280), PreviewLayout.Sprite("C020_Monok", "", 800, 800));
        Equal(new SpritePlacement(0, 440, 140, 280), PreviewLayout.Sprite("C000_Saiha_kak", "", 500, 1000));
        Equal(new SpritePlacement(0, 580, 280, 140), PreviewLayout.Sprite("chara_Hatena", "", 800, 400));
        // The expression string, not whether an image file happened to fall back, controls geometry.
        Equal(new SpritePlacement(-60, 495, 430, 430), PreviewLayout.Sprite("C000_Saiha", "missing_animation", 300, 300));
        Equal(new SpritePlacement(-60, 730, 340, 680), PreviewLayout.Sprite("C000_Saiha", "anim000", 1000, 2000, 1080));

        Throws<ArgumentOutOfRangeException>(() => PreviewLayout.Sprite("C000_Saiha", "anim000", 0, 10));
        Throws<ArgumentOutOfRangeException>(() => PreviewLayout.Sprite("C000_Saiha", "anim000", 10, -1));

        Equal(new PreviewTextProfile(260, 575, 41, 22, false, false), PreviewLayout.Text("DR", "ToWitter"));
        Equal(new PreviewTextProfile(355, 560, 41, 28, false, false), PreviewLayout.Text("AI", "AIBlank"));
        Equal(new PreviewTextProfile(335, 280, 41, 25, false, false), PreviewLayout.Text("AI", "AITutorial"));
        Equal(new PreviewTextProfile(830, 175, 11, 10, true, true), PreviewLayout.Text("AI", "ToWitter"));
        Equal(PreviewLayout.Text("AI", "AITutorial"), PreviewLayout.Text("AI", "AI_Tutorial"));
    }

    private static void Equal<T>(T expected, T actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
            throw new Exception($"Expected [{expected}], actual [{actual}].");
    }

    private static void Throws<T>(Action action) where T : Exception
    {
        try { action(); } catch (T) { return; }
        throw new Exception("Expected " + typeof(T).Name);
    }
}
