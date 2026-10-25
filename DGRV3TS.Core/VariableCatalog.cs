using System.Globalization;
using System.Text;

namespace DGRV3TS.Core;

public sealed record VariableDefinition(string Token, string Value, string Category, string? AlternateValue = null)
{
    public string Comment { get; init; } = "";
}

/// <summary>The external v25 variable vocabulary; raw tokens remain the identity of every row.</summary>
public sealed class VariableCatalog
{
    public static VariableCatalog Empty { get; } = new(null, [], []);
    public IReadOnlyList<VariableDefinition> Variables { get; }
    public IReadOnlyList<string> Warnings { get; }
    public string? FilePath { get; }
    private readonly Dictionary<char, VariableDefinition[]> candidates;
    private readonly Dictionary<string, VariableDefinition> byToken;

    private VariableCatalog(string? filePath, List<VariableDefinition> variables, List<string> warnings)
    {
        FilePath = filePath;
        Variables = variables.AsReadOnly();
        Warnings = warnings.AsReadOnly();
        byToken = variables.ToDictionary(v => v.Token, StringComparer.Ordinal);
        candidates = variables.GroupBy(v => v.Token[0]).ToDictionary(g => g.Key,
            g => g.OrderByDescending(v => v.Token.Length).ToArray());
    }

    public static VariableCatalog Load(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        var rows = new List<VariableDefinition>();
        var warnings = new List<string>();
        var tokens = new HashSet<string>(StringComparer.Ordinal);
        var category = "Generali";
        var lineNumber = 0;
        foreach (var input in File.ReadLines(path, new UTF8Encoding(false, true)))
        {
            lineNumber++;
            var line = input.Trim();
            if (line.Length == 0) continue;
            if (line.StartsWith("//", StringComparison.Ordinal))
            {
                var heading = line[2..].Trim();
                if (heading.Length > 0) category = heading;
                continue;
            }
            var comma = line.IndexOf(',');
            var token = (comma < 0 ? line : line[..comma]).Trim();
            if (token.Length == 0 || token.Any(char.IsWhiteSpace))
            {
                warnings.Add($"Riga {lineNumber}: token vuoto o contenente spazi; riga ignorata.");
                continue;
            }
            if (!tokens.Add(token))
            {
                warnings.Add($"Riga {lineNumber}: token duplicato '{token}'; mantenuta la prima definizione.");
                continue;
            }
            var value = comma < 0 ? token : line[(comma + 1)..].Trim();
            var comment = "";
            var pipe = value.IndexOf(" | ", StringComparison.Ordinal);
            if (pipe >= 0) { comment = value[(pipe + 3)..].Trim(); value = value[..pipe].Trim(); }
            string? alternate = null;
            var colon = value.IndexOf(" : ", StringComparison.Ordinal);
            if (colon >= 0) { alternate = value[(colon + 3)..].Trim(); value = value[..colon].Trim(); }
            // Signals and CLT tags are interpreted by the preview, never renamed by a dictionary.
            if (token.StartsWith("<SIGNAL", StringComparison.Ordinal) || token.StartsWith("<PLATFORM", StringComparison.Ordinal)
                || token.StartsWith("<CLT", StringComparison.Ordinal) || token.StartsWith("SIGNAL_", StringComparison.Ordinal)
                || token.StartsWith("PLATFORM_", StringComparison.Ordinal)) { value = token; alternate = null; }
            rows.Add(new VariableDefinition(token, value, category, alternate) { Comment = comment });
        }
        var catalog = new VariableCatalog(Path.GetFullPath(path), rows, warnings);
        foreach (var row in rows)
            if (catalog.HasCycle(row.Token, new HashSet<string>(StringComparer.Ordinal), 0))
                warnings.Add($"Riferimento ciclico nella variabile '{row.Token}'; l'anteprima mantiene visibili i token non risolti.");
        return catalog;
    }

    public static string? FindDefaultFile(params string[] directories)
    {
        foreach (var directory in directories)
        {
            var found = FindFile("vars_bak.txt", directory) ?? FindFile("VARS_BAK", directory);
            if (found is not null) return found;
        }
        return null;
    }

    public static string? FindFile(string fileName, params string[] directories)
    {
        foreach (var directory in directories)
        {
            if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory)) continue;
            var path = Directory.EnumerateFiles(directory).OrderBy(p => p, StringComparer.Ordinal)
                .FirstOrDefault(p => string.Equals(Path.GetFileName(p), fileName, StringComparison.OrdinalIgnoreCase));
            if (path is not null) return path;
        }
        return null;
    }

    public string Resolve(string text, bool alternate = false)
    {
        ArgumentNullException.ThrowIfNull(text);
        return ResolveText(text, alternate, new HashSet<string>(StringComparer.Ordinal), 0);
    }

    private string ResolveText(string text, bool alternate, HashSet<string> visiting, int depth)
    {
        var result = new StringBuilder(text.Length);
        for (var position = 0; position < text.Length;)
        {
            VariableDefinition? matched = null;
            if (candidates.TryGetValue(text[position], out var options))
                foreach (var option in options)
                {
                    var end = position + option.Token.Length;
                    if (end > text.Length || !text.AsSpan(position, option.Token.Length).SequenceEqual(option.Token.AsSpan())) continue;
                    if (IsWord(option.Token[0]) && position > 0 && IsWord(text[position - 1])) continue;
                    if (IsWord(option.Token[^1]) && end < text.Length && IsWord(text[end])) continue;
                    matched = option;
                    break;
                }
            if (matched is null) { result.Append(text[position++]); continue; }
            var replacement = alternate ? matched.AlternateValue ?? matched.Value : matched.Value;
            // Resolve known nested references only; missing _MN/_MS variants are never invented.
            if (replacement != matched.Token && depth < 2)
            {
                if (!visiting.Add(matched.Token)) replacement = matched.Token;
                else
                {
                    replacement = ResolveText(replacement, alternate, visiting, depth + 1);
                    visiting.Remove(matched.Token);
                }
            }
            result.Append(replacement);
            position += matched.Token.Length;
        }
        return result.ToString();
    }

    private bool HasCycle(string token, HashSet<string> visiting, int depth)
    {
        if (!visiting.Add(token)) return true;
        // The preview expands at most three nested definitions, so deeper chains stay literal.
        if (depth >= 3) { visiting.Remove(token); return false; }
        var row = byToken[token];
        foreach (var value in new[] { row.Value, row.AlternateValue }.OfType<string>())
        {
            if (value == token) continue;
            foreach (var target in References(value))
                if (HasCycle(target, visiting, depth + 1)) { visiting.Remove(token); return true; }
        }
        visiting.Remove(token);
        return false;
    }

    private IEnumerable<string> References(string text)
    {
        for (var position = 0; position < text.Length; position++)
        {
            if (!candidates.TryGetValue(text[position], out var choices)) continue;
            foreach (var choice in choices)
            {
                var end = position + choice.Token.Length;
                if (end > text.Length || !text.AsSpan(position, choice.Token.Length).SequenceEqual(choice.Token.AsSpan())) continue;
                if (IsWord(choice.Token[0]) && position > 0 && IsWord(text[position - 1])) continue;
                if (IsWord(choice.Token[^1]) && end < text.Length && IsWord(text[end])) continue;
                yield return choice.Token;
                position = end - 1;
                break;
            }
        }
    }

    internal static bool IsWord(char value) => char.IsLetterOrDigit(value) || char.IsSurrogate(value) || value == '_'
        || char.GetUnicodeCategory(value) is UnicodeCategory.NonSpacingMark or UnicodeCategory.SpacingCombiningMark or UnicodeCategory.EnclosingMark;
}
