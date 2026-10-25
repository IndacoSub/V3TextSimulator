using System.Text;
using System.Text.RegularExpressions;

namespace DGRV3TS.Core;

public sealed record TranslationIssue(string Message);

/// <summary>Checks raw translation tokens and produces a separate readable preview.</summary>
public static class TranslationTools
{
    private static readonly Regex Tokens = new(
        @"<[^<>\r\n]+>|(?<!\{)\{(?:[0-9]+|[\p{L}_][\p{L}\p{N}_]*)(?:,-?[0-9]+)?(?::[^{}\r\n]+)?\}(?!\})|%%|%(?:[0-9]+\$)?[-+#0 ']*(?:\*|[0-9]+)?(?:\.(?:\*|[0-9]+))?(?:hh|ll|[hlLjzt])?[diuoxXfFeEgGaAcspn]",
        RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));
    private static readonly Regex PreviewControls = new(
        @"</?CLT(?:=[^<>\r\n]*)?>|<PLATFORM_(?:PC|CONSOLE)>|<SIGNAL_(?:NDR|ALT|NOSWAP|CBL|OE|AutoTL)>",
        RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));

    public static IReadOnlyList<TranslationIssue> Validate(string original, string translation, IEnumerable<string> variableTokens)
    {
        ArgumentNullException.ThrowIfNull(original);
        ArgumentNullException.ThrowIfNull(translation);
        ArgumentNullException.ThrowIfNull(variableTokens);
        if (string.IsNullOrWhiteSpace(translation)) return Array.Empty<TranslationIssue>();
        var variables = variableTokens.Where(t => !string.IsNullOrWhiteSpace(t)).Distinct(StringComparer.Ordinal)
            .OrderByDescending(t => t.Length).ThenBy(t => t, StringComparer.Ordinal).ToArray();
        var before = Count(original, variables);
        var after = Count(translation, variables);
        return before.Keys.Union(after.Keys, StringComparer.Ordinal).OrderBy(t => t, StringComparer.Ordinal)
            .Where(t => before.GetValueOrDefault(t) != after.GetValueOrDefault(t))
            .Select(t => new TranslationIssue($"Token {t}: {before.GetValueOrDefault(t)} nell'originale, {after.GetValueOrDefault(t)} nella traduzione."))
            .ToArray();
    }

    private static Dictionary<string, int> Count(string text, string[] variables)
    {
        var counts = new Dictionary<string, int>(StringComparer.Ordinal);
        var occupied = new bool[text.Length];
        foreach (Match match in Tokens.Matches(text))
        {
            if (match.Value != "%%") Add(match.Value);
            Array.Fill(occupied, true, match.Index, match.Length);
        }
        foreach (var token in variables)
            for (var index = 0; index <= text.Length - token.Length;)
            {
                var found = text.IndexOf(token, index, StringComparison.Ordinal);
                if (found < 0) break;
                var end = found + token.Length;
                if ((!VariableCatalog.IsWord(token[0]) || found == 0 || !VariableCatalog.IsWord(text[found - 1]))
                    && (!VariableCatalog.IsWord(token[^1]) || end == text.Length || !VariableCatalog.IsWord(text[end]))
                    && !occupied.AsSpan(found, token.Length).Contains(true))
                {
                    Add(token);
                    Array.Fill(occupied, true, found, token.Length);
                }
                index = end;
            }
        return counts;
        void Add(string token) => counts[token] = counts.GetValueOrDefault(token) + 1;
    }

    public static string PreviewText(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        text = PreviewControls.Replace(text, "");
        var result = new StringBuilder(text.Length);
        for (var index = 0; index < text.Length; index++)
        {
            if (text[index] == '\\' && index + 1 < text.Length)
            {
                if (text[index + 1] == '\\') { result.Append('\\'); index++; continue; }
                if (text[index + 1] == 'n') { result.Append('\n'); index++; continue; }
                if (text[index + 1] == '"') { result.Append('"'); index++; continue; }
                if (text[index + 1] == 'r' && index + 3 < text.Length && text[index + 2] == '\\' && text[index + 3] == 'n')
                { result.Append('\n'); index += 3; continue; }
            }
            result.Append(text[index]);
        }
        return result.ToString().Replace("\r\n", "\n").Replace('\r', '\n');
    }
}
