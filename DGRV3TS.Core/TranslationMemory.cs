namespace DGRV3TS.Core;

/// <summary>A previous human translation, retaining its source for review.</summary>
public sealed record TranslationSuggestion(string Text, string SourceFile, string Speaker);

/// <summary>
/// Indexes completed translations in local files. No network translation is performed and
/// source documents are never changed while scanning. Suggestions require an exact original.
/// </summary>
public sealed class TranslationMemory
{
    private static readonly HashSet<string> Formats = new(StringComparer.OrdinalIgnoreCase)
    {
        ".po", ".vo", ".xlsx", ".json"
    };

    private static readonly HashSet<string> ExcludedDirectories = new(StringComparer.OrdinalIgnoreCase)
    {
        ".git", ".vs", ".cloud-setup", "bin", "obj", "assets", "Graphics", "Sound"
    };

    private readonly Dictionary<(string Original, int Language), List<TranslationSuggestion>> suggestions;

    public IReadOnlyList<string> Warnings { get; }

    private TranslationMemory(
        Dictionary<(string Original, int Language), List<TranslationSuggestion>> suggestions,
        List<string> warnings)
    {
        this.suggestions = suggestions;
        Warnings = warnings.AsReadOnly();
    }

    /// <summary>
    /// Recursively scans a selected directory, excluding the current document, application
    /// assets, build outputs and symbolic links. Invalid or unreadable files become warnings.
    /// Cancellation propagates to the caller without returning a partially built index.
    /// </summary>
    public static TranslationMemory Scan(string directory, string? excludedFile = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);
        cancellationToken.ThrowIfCancellationRequested();
        directory = Path.GetFullPath(directory);
        if (!Directory.Exists(directory)) throw new DirectoryNotFoundException("La cartella della memoria di traduzione non esiste.");
        var excludedPath = string.IsNullOrWhiteSpace(excludedFile) ? null : Path.GetFullPath(excludedFile);
        var comparer = OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
        var warnings = new List<string>();
        var entries = new Dictionary<(string Original, int Language), List<TranslationSuggestion>>();
        var pending = new Stack<string>();
        pending.Push(directory);

        while (pending.TryPop(out var currentDirectory))
        {
            cancellationToken.ThrowIfCancellationRequested();
            string[] paths;
            try
            {
                paths = Directory.GetFileSystemEntries(currentDirectory);
                Array.Sort(paths, StringComparer.Ordinal);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                warnings.Add($"Cartella ignorata '{Path.GetRelativePath(directory, currentDirectory)}': {exception.Message}");
                continue;
            }

            foreach (var path in paths)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    var attributes = File.GetAttributes(path);
                    // Following linked directories could visit the same files indefinitely.
                    if ((attributes & FileAttributes.ReparsePoint) != 0) continue;
                    if ((attributes & FileAttributes.Directory) != 0)
                    {
                        if (!ExcludedDirectories.Contains(Path.GetFileName(path))) pending.Push(path);
                        continue;
                    }
                    if (!Formats.Contains(Path.GetExtension(path)) || comparer.Equals(path, excludedPath)) continue;

                    cancellationToken.ThrowIfCancellationRequested();
                    var document = TranslationDocument.Load(path);
                    for (var language = 0; language < document.LanguageCount; language++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        document.SelectLanguage(language);
                        foreach (var entry in document.Entries)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            if (string.IsNullOrWhiteSpace(entry.Original) || string.IsNullOrWhiteSpace(entry.Translation)) continue;
                            var key = (entry.Original, language);
                            if (!entries.TryGetValue(key, out var candidates)) entries[key] = candidates = new();
                            // Keep donor identities until exclusions and speaker preference are applied.
                            if (candidates.Any(candidate => candidate.Text == entry.Translation
                                && candidate.Speaker == entry.Speaker && comparer.Equals(candidate.SourceFile, path))) continue;
                            candidates.Add(new TranslationSuggestion(entry.Translation, path, entry.Speaker));
                        }
                    }
                }
                catch (Exception exception) when (exception is not OutOfMemoryException and not StackOverflowException and not OperationCanceledException)
                {
                    warnings.Add($"File ignorato '{Path.GetRelativePath(directory, path)}': {exception.Message}");
                }
            }
        }

        cancellationToken.ThrowIfCancellationRequested();
        return new TranslationMemory(entries, warnings);
    }

    /// <summary>
    /// Returns all distinct translations for this exact original and language. When possible,
    /// the same speaker takes precedence. Multiple different texts remain explicit choices.
    /// </summary>
    public IReadOnlyList<TranslationSuggestion> Find(string original, string speaker = "", int language = 0) =>
        Find(original, speaker, language, static _ => true);

    internal IReadOnlyList<TranslationSuggestion> Find(string original, string speaker, int language,
        Func<TranslationSuggestion, bool> eligible)
    {
        ArgumentNullException.ThrowIfNull(original);
        ArgumentNullException.ThrowIfNull(speaker);
        if (language < 0) throw new ArgumentOutOfRangeException(nameof(language));
        if (!suggestions.TryGetValue((original, language), out var candidates)) return Array.Empty<TranslationSuggestion>();
        IEnumerable<TranslationSuggestion> matches = candidates.Where(eligible).ToArray();
        if (speaker.Length != 0)
        {
            var sameSpeaker = matches.Where(candidate => string.Equals(candidate.Speaker, speaker, StringComparison.Ordinal)).ToArray();
            if (sameSpeaker.Length != 0) matches = sameSpeaker;
        }
        return matches.OrderBy(candidate => candidate.SourceFile, StringComparer.Ordinal)
            .DistinctBy(candidate => candidate.Text, StringComparer.Ordinal).ToArray();
    }
}
