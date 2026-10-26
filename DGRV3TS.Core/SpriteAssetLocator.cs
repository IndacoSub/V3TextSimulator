namespace DGRV3TS.Core;

/// <summary>Locates optional sprite files without decoding images or changing dialogue metadata.</summary>
public static class SpriteAssetLocator
{
    public static string? Find(string root, string game, string speaker, string expression)
    {
        if (string.IsNullOrWhiteSpace(speaker) || !SafeName(game) || !SafeName(speaker)
            || (!string.IsNullOrEmpty(expression) && !SafeName(expression))) return null;
        var sprites = Path.Combine(root, "Graphics", "Sprites");
        string? Placeholder(string name) => Existing(Path.Combine(sprites, name, "image.png"))
            ?? Existing(Path.Combine(sprites, game, name, "image.png"));
        if (speaker is "chara_Blank" or "chara_Hatena")
            return Placeholder(speaker);

        // v25 rendering and sprite import used two names for the same character.
        string[] names = game == "DR" ? speaker switch
        {
            "C021_Mtaro" => [speaker, "C021_Monot"],
            "C021_Monot" => [speaker, "C021_Mtaro"],
            _ => [speaker]
        } : [speaker];
        if (!string.IsNullOrWhiteSpace(expression))
            foreach (var name in names)
                if (Existing(Path.Combine(sprites, game, name, expression + ".png")) is { } animated) return animated;
        foreach (var name in names)
            if (Existing(Path.Combine(sprites, game, name, "default.png")) is { } portrait) return portrait;
        return Placeholder("chara_Hatena");
    }

    private static string? Existing(string path) => File.Exists(path) ? path : null;
    private static bool SafeName(string name) => name.Length > 0 && name != "." && name != ".."
        && name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0 && !name.Contains('/') && !name.Contains('\\');
}
