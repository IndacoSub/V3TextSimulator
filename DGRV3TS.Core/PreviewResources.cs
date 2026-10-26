namespace DGRV3TS.Core;

public readonly record struct PreviewFileVersion(string Path, long Length, long ModifiedTicks)
{
    public static PreviewFileVersion Read(string path)
    {
        var file = new FileInfo(path);
        var identity = OperatingSystem.IsWindows() ? file.FullName.ToUpperInvariant() : file.FullName;
        return new PreviewFileVersion(identity, file.Length, file.LastWriteTimeUtc.Ticks);
    }
}

public static class PreviewResources
{
    /// <summary>A null selected path represents the user's basic-background choice.</summary>
    public static string? BackgroundAfterReload(IEnumerable<string> available, string? selected, bool hadSelection)
    {
        var paths = available.ToArray();
        if (hadSelection && selected is null) return null;
        var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        return paths.FirstOrDefault(p => string.Equals(p, selected, comparison)) ?? paths.FirstOrDefault();
    }
}
