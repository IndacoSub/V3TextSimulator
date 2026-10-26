namespace DGRV3TS.Core;

/// <summary>v25 translation-memory reuse without changing existing work or duplicating provenance.</summary>
public static class TranslationReuse
{
    public const string Marker = "<SIGNAL_AutoTL>";

    public static bool IsUntranslated(TranslationEntry entry) =>
        !entry.HasTranslation && string.IsNullOrWhiteSpace(entry.Translation);

    public static bool SupportsAutomaticReuse(string? path) => Path.GetExtension(path ?? "").ToLowerInvariant()
        is ".po" or ".vo" or ".xlsx";

    /// <summary>Keep all game controls. Only an earlier AutoTL marker and its provenance tail are removed.</summary>
    public static string CandidateText(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        var marker = text.IndexOf(Marker, StringComparison.Ordinal);
        return marker < 0 ? text : text[..marker];
    }

    public static string PrepareTranslation(string text)
    {
        var body = CandidateText(text);
        if (string.IsNullOrWhiteSpace(body) || body.Contains("[EMPTY_LINE]", StringComparison.Ordinal))
            throw new ArgumentException("Il suggerimento non contiene una traduzione utilizzabile.", nameof(text));
        return body + Marker;
    }

    public static IReadOnlyList<TranslationSuggestion> FindSuggestions(
        TranslationMemory memory, TranslationEntry entry, int language,
        IEnumerable<string> variableTokens, bool requireCompatible = true, string? excludedFile = null)
    {
        ArgumentNullException.ThrowIfNull(memory);
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(variableTokens);
        if (entry.Original.Length <= 1 || entry.Original.Contains("[EMPTY_LINE]", StringComparison.Ordinal)) return [];
        var tokens = variableTokens.ToArray();
        var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        var excludedPath = excludedFile is null ? null : Path.GetFullPath(excludedFile);
        var source = CandidateText(entry.Original);
        // Filter before speaker preference and deduplication: unusable/current-file rows
        // must not hide a usable translation from another donor.
        return memory.Find(entry.Original, entry.Speaker, language, s =>
            {
                if (excludedPath is not null && string.Equals(Path.GetFullPath(s.SourceFile), excludedPath, comparison)) return false;
                var text = CandidateText(s.Text);
                return !string.IsNullOrWhiteSpace(text) && !text.Contains("[EMPTY_LINE]", StringComparison.Ordinal)
                    && !string.Equals(source, text, StringComparison.Ordinal)
                    && (!requireCompatible || TranslationTools.Validate(source, text, tokens).Count == 0);
            })
            .Select(s => s with { Text = CandidateText(s.Text) })
            .DistinctBy(s => s.Text, StringComparer.Ordinal).ToArray();
    }
}
