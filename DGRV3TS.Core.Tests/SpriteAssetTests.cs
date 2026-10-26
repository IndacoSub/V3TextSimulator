using DGRV3TS.Core;

internal static class SpriteAssetTests
{
    public static void Run(string root)
    {
        var assets = Path.Combine(root, "invented-sprite-assets");
        string Marker(params string[] parts)
        {
            var path = Path.Combine(new[] { assets, "Graphics", "Sprites" }.Concat(parts).ToArray());
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            // Synthetic marker files test lookup only, not PNG decoding or transparency.
            File.WriteAllBytes(path, []);
            return path;
        }
        var question = Marker("chara_Hatena", "image.png");
        var blank = Marker("chara_Blank", "image.png");
        var portrait = Marker("DR", "ACTOR_DEMO", "default.png");
        var animated = Marker("DR", "ACTOR_DEMO", "anim_demo.png");
        Equal(animated, SpriteAssetLocator.Find(assets, "DR", "ACTOR_DEMO", "anim_demo"));
        Equal(portrait, SpriteAssetLocator.Find(assets, "DR", "ACTOR_DEMO", "unknown"));
        Equal(portrait, SpriteAssetLocator.Find(assets, "DR", "ACTOR_DEMO", ""));
        Equal(question, SpriteAssetLocator.Find(assets, "DR", "UNKNOWN_ACTOR", "anim_demo"));
        Equal(question, SpriteAssetLocator.Find(assets, "DR", "chara_Hatena", "ignored"));
        Equal(blank, SpriteAssetLocator.Find(assets, "DR", "chara_Blank", "ignored"));
        Equal<string?>(null, SpriteAssetLocator.Find(assets, "DR", "", "anim_demo"));
        Equal<string?>(null, SpriteAssetLocator.Find(assets, "DR", "../outside", "anim_demo"));
        Equal<string?>(null, SpriteAssetLocator.Find(assets, "DR", "ACTOR_DEMO", "../outside"));

        var imported = Marker("DR", "C021_Monot", "anim_demo.png");
        Equal(imported, SpriteAssetLocator.Find(assets, "DR", "C021_Mtaro", "anim_demo"));
        var legacy = Marker("DR", "C021_Mtaro", "anim_demo.png");
        Equal(legacy, SpriteAssetLocator.Find(assets, "DR", "C021_Mtaro", "anim_demo"));
        Equal(imported, SpriteAssetLocator.Find(assets, "DR", "C021_Monot", "anim_demo"));
        File.Delete(imported);
        Equal(legacy, SpriteAssetLocator.Find(assets, "DR", "C021_Monot", "anim_demo"));
        var importedPortrait = Marker("DR", "C021_Monot", "default.png");
        Equal(legacy, SpriteAssetLocator.Find(assets, "DR", "C021_Monot", "anim_demo"));
        Equal(importedPortrait, SpriteAssetLocator.Find(assets, "DR", "C021_Mtaro", "missing_animation"));
        Equal(new SpritePlacement(-120, 400, 440, 440), PreviewLayout.Sprite("C021_Monot", "anim_demo", 800, 800));
        Equal(PreviewLayout.Sprite("C021_Mtaro", "anim_demo", 800, 800), PreviewLayout.Sprite("C021_Monot", "anim_demo", 800, 800));
        Equal(new SpritePlacement(0, 440, 280, 280), PreviewLayout.Sprite("C021_Monot", "", 800, 800));

        File.Delete(question);
        Equal<string?>(null, SpriteAssetLocator.Find(assets, "DR", "UNKNOWN_ACTOR", ""));
        File.Delete(blank);
        Equal<string?>(null, SpriteAssetLocator.Find(assets, "DR", "chara_Blank", "ignored"));
        var perGameQuestion = Marker("DR", "chara_Hatena", "image.png");
        var perGameBlank = Marker("DR", "chara_Blank", "image.png");
        Equal(perGameQuestion, SpriteAssetLocator.Find(assets, "DR", "chara_Hatena", "ignored"));
        Equal(perGameBlank, SpriteAssetLocator.Find(assets, "DR", "chara_Blank", ""));
        Equal(perGameQuestion, SpriteAssetLocator.Find(assets, "DR", "UNKNOWN_ACTOR", "missing_animation"));
        Equal<string?>(null, SpriteAssetLocator.Find(assets, "AI", "chara_Hatena", "ignored"));
        Marker("chara_Hatena", "image.png");
        Equal(question, SpriteAssetLocator.Find(assets, "DR", "chara_Hatena", "ignored"));
        Equal(question, SpriteAssetLocator.Find(assets, "DR", "UNKNOWN_ACTOR", ""));
    }

    private static void Equal<T>(T expected, T actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual)) throw new Exception($"Expected [{expected}], actual [{actual}].");
    }
}
