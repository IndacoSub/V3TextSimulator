using DGRV3TS.Core;

internal static class PreviewResourceTests
{
    public static void Run(string root)
    {
        string[] backgrounds = ["first.png", "second.png"];
        Equal("first.png", PreviewResources.BackgroundAfterReload(backgrounds, null, hadSelection: false));
        Equal<string?>(null, PreviewResources.BackgroundAfterReload(backgrounds, null, hadSelection: true));
        Equal("second.png", PreviewResources.BackgroundAfterReload(backgrounds, "second.png", hadSelection: true));
        Equal("first.png", PreviewResources.BackgroundAfterReload(backgrounds, "removed.png", hadSelection: true));
        Equal<string?>(null, PreviewResources.BackgroundAfterReload([], "removed.png", hadSelection: true));

        var path = Path.Combine(root, "invented-reload-font.otf");
        File.WriteAllText(path, "first version");
        var before = PreviewFileVersion.Read(path);
        Equal(before, PreviewFileVersion.Read(path));
        File.WriteAllText(path, "other version"); // Same length, different timestamp.
        File.SetLastWriteTimeUtc(path, new DateTime(before.ModifiedTicks, DateTimeKind.Utc).AddSeconds(2));
        var changed = PreviewFileVersion.Read(path);
        Check(changed != before && changed.Length == before.Length, "A same-path, same-size external edit must invalidate the font version.");
        File.WriteAllText(path, "longer external font version");
        File.SetLastWriteTimeUtc(path, new DateTime(changed.ModifiedTicks, DateTimeKind.Utc));
        var resized = PreviewFileVersion.Read(path);
        Check(resized != changed && resized.ModifiedTicks == changed.ModifiedTicks, "A size change must invalidate even with a retained timestamp.");
    }

    private static void Equal<T>(T expected, T actual)
    { if (!EqualityComparer<T>.Default.Equals(expected, actual)) throw new Exception($"Expected [{expected}], actual [{actual}]."); }
    private static void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
}
