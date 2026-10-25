namespace DGRV3TS.Core;

/// <summary>A destination rectangle in the v25 preview's unscaled 1280×720 coordinate space.</summary>
public readonly record struct SpritePlacement(int X, int Y, int Width, int Height);

/// <summary>Game-specific text defaults from the original v25 preview.</summary>
public readonly record struct PreviewTextProfile(
    int X, int Y, int NewlineHeight, float DefaultFontSize, bool DarkText, bool StripToLastTag);

/// <summary>
/// The original layout rules, without Windows drawing dependencies. Window zooming must apply
/// after composing the full canvas: resizing a preview window must not alter these coordinates.
/// </summary>
public static class PreviewLayout
{
    public static SpritePlacement Sprite(string speaker, string expression, int width, int height, int canvasHeight = 720)
    {
        ArgumentNullException.ThrowIfNull(speaker);
        ArgumentNullException.ThrowIfNull(expression);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(canvasHeight);

        // v25: when no expression is supplied, default portraits fit a 280×280 box.
        if (expression.Length == 0)
        {
            var portrait = Fit(width, height, 280, 280);
            return new SpritePlacement(0, canvasHeight - portrait.Height, portrait.Width, portrait.Height);
        }

        // These images were already cropped in the source assets. Do not rescale them as full bodies.
        if (speaker.Contains("_kak", StringComparison.Ordinal)
            || speaker is "chara_Blank" or "chara_Hatena" or "non" or "None" or "" or "C010_Haruk")
            return new SpritePlacement(0, canvasHeight - height, width, height);

        if (speaker is "C020_Monok" or "C021_Mtaro" or "C022_Msuke" or "C023_Mfunn" or "C024_Mdam_" or "C025_Mkid_")
        {
            var mascot = Fit(width, height, 440, 440);
            return new SpritePlacement(-120, canvasHeight - mascot.Height / 2 - 100, mascot.Width, mascot.Height);
        }

        var body = Fit(width, height, 430, 680);
        return new SpritePlacement(-60, canvasHeight - body.Height / 2 - 10, body.Width, body.Height);
    }

    public static PreviewTextProfile Text(string game, string? backgroundName)
    {
        ArgumentNullException.ThrowIfNull(game);
        if (!game.Equals("AI", StringComparison.OrdinalIgnoreCase))
            return new PreviewTextProfile(260, 575, 41, 29, false, false);
        if (string.Equals(backgroundName, "ToWitter", StringComparison.OrdinalIgnoreCase))
            return new PreviewTextProfile(830, 175, 11, 10, true, true);
        if (string.Equals(backgroundName, "AITutorial", StringComparison.OrdinalIgnoreCase)
            || string.Equals(backgroundName, "AI_Tutorial", StringComparison.OrdinalIgnoreCase))
            return new PreviewTextProfile(335, 280, 41, 25, false, false);
        return new PreviewTextProfile(355, 560, 41, 28, false, false);
    }

    private static (int Width, int Height) Fit(int width, int height, int maxWidth, int maxHeight)
    {
        // Keep v25's double arithmetic and truncation, rather than rounding or fitting with floats.
        var ratio = Math.Min((double)maxWidth / width, (double)maxHeight / height);
        var scaledWidth = (int)(width * ratio);
        var scaledHeight = (int)(height * ratio);
        if (scaledWidth == 0 || scaledHeight == 0)
            throw new ArgumentException("Sprite dimensions are too disproportionate to fit the preview.");
        return (scaledWidth, scaledHeight);
    }
}
