using DGRV3TS.Core;

internal static class ReadableSpriteTests
{
    public static void Run()
    {
        var visible = new SpriteRegion(120, 70, 600, 1600);
        var bust = PreviewLayout.ReadableSprite(visible);
        Equal(new SpriteRegion(120, 70, 600, 840), bust.Source);
        Equal(new SpritePlacement(16, 440, 200, 280), bust.Destination);
        Equal(visible, PreviewLayout.ReadableSprite(visible, portrait: true).Source);

        // Transparent canvas margins must not change the framing of the same visible artwork.
        var padded = PreviewLayout.ReadableSprite(visible with { X = 700, Y = 800 });
        Equal(bust.Destination, padded.Destination);
        Equal(new SpritePlacement(12, 564, 208, 156), PreviewLayout.ReadableSprite(new SpriteRegion(0, 0, 320, 240)).Destination);
        foreach (var region in new[]
        {
            new SpriteRegion(0, 0, 320, 240), new SpriteRegion(80, 10, 500, 1100),
            new SpriteRegion(50, 100, 900, 1900), new SpriteRegion(5, 6, 180, 210),
            new SpriteRegion(0, 0, 100, 300), new SpriteRegion(0, 0, 1, 1)
        })
            foreach (var portrait in new[] { false, true })
                foreach (var textLeft in new[] { 220, 260, 355 })
                {
                    var frame = PreviewLayout.ReadableSprite(region, portrait, textLeft);
                    var source = frame.Source; var target = frame.Destination;
                    Check(source.X >= region.X && source.Y >= region.Y && source.X + source.Width <= region.X + region.Width
                        && source.Y + source.Height <= region.Y + region.Height, "Framing must sample only the supplied artwork.");
                    Check(target.Width > 0 && target.Height > 0 && target.X >= 0 && target.Y >= 0
                        && target.X + target.Width <= textLeft - 40 && target.Y + target.Height == 720,
                        "The complete sprite must stay left of text with a gap and align to the canvas bottom.");
                    Check(Math.Abs((double)target.Width / target.Height - (double)source.Width / source.Height) < .02,
                        "Framing must preserve source proportions, allowing integer rounding.");
                }
    }

    private static void Equal<T>(T expected, T actual)
    { if (!EqualityComparer<T>.Default.Equals(expected, actual)) throw new Exception($"Expected [{expected}], actual [{actual}]."); }
    private static void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
}
