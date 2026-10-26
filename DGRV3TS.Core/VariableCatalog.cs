using System.Globalization;
using System.Text;

namespace DGRV3TS.Core;

public sealed record VariableDefinition(string Token, string Value, string Category, string? AlternateValue = null)
{
    public string Comment { get; init; } = "";
    public bool IsTemporary { get; init; }
}

/// <summary>The external v25 variable vocabulary; raw tokens remain the identity of every row.</summary>
public sealed partial class VariableCatalog
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
        foreach (var row in variables)
            if (TryParseMacroSignature(row.Token, out var name, out var parameter))
                macros.Add(name, new Macro(row, name, parameter));
        candidates = variables.Where(v => !TryParseMacroSignature(v.Token, out _, out _))
            .GroupBy(v => v.Token[0]).ToDictionary(g => g.Key,
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
            // Notes can accompany bare controls too, and can themselves contain commas.
            var comment = "";
            var pipe = line.IndexOf(" | ", StringComparison.Ordinal);
            if (pipe >= 0) { comment = line[(pipe + 3)..].Trim(); line = line[..pipe].TrimEnd(); }
            var comma = line.IndexOf(',');
            var token = (comma < 0 ? line : line[..comma]).Trim();
            if (token.Length == 0 || token.Any(char.IsWhiteSpace))
            {
                warnings.Add($"Riga {lineNumber}: token vuoto o contenente spazi; riga ignorata.");
                continue;
            }
            var macroName = "";
            if (token.StartsWith("MAKE_", StringComparison.Ordinal) && token.Contains('(')
                && !TryParseMacroSignature(token, out macroName, out _))
            {
                warnings.Add($"Riga {lineNumber}: definizione MAKE non valida; atteso MAKE_NOME(PARAMETRO).");
                continue;
            }
            if (!tokens.Add(macroName.Length > 0 ? macroName : token))
            {
                warnings.Add($"Riga {lineNumber}: token duplicato '{token}'; mantenuta la prima definizione.");
                continue;
            }
            var value = comma < 0 ? token : line[(comma + 1)..].Trim();
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
            if (catalog.HasCycle(row.Token))
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
        return ResolveText(text, alternate, new ResolutionState(), 0);
    }

    /// <summary>Restores v25's session-only case variants without editing the external dictionary.</summary>
    public (VariableCatalog Catalog, VariableDefinition Variant, bool Created) WithCaseVariant(string token, bool uppercase)
    {
        if (!byToken.TryGetValue(token, out var source)) throw new ArgumentException("Variabile non trovata.", nameof(token));
        if (TryParseMacroSignature(token, out _, out _))
            throw new NotSupportedException("Crea la variante della variabile da passare al MAKE, oppure definisci un MAKE separato nel dizionario.");
        var suffix = uppercase ? "_MS" : "_MN";
        // Prefer exact external definitions, including the bracketed convention accepted by the rewrite.
        if (token.EndsWith(suffix, StringComparison.Ordinal)) return (this, source, false);
        if (byToken.TryGetValue(token + suffix, out var existing)) return (this, existing, false);
        if (token.EndsWith('>') && byToken.TryGetValue(token[..^1] + suffix + ">", out existing)) return (this, existing, false);
        if (token.EndsWith(suffix + ">", StringComparison.Ordinal)) return (this, source, false);

        string ChangeCase(string value) => uppercase ? value.ToUpperInvariant() : value.ToLowerInvariant();
        var resolved = Resolve(token);
        var alternateResolved = Resolve(token, alternate: true);
        var variant = new VariableDefinition(token + suffix, ChangeCase(resolved), source.Category,
            alternateResolved == resolved ? null : ChangeCase(alternateResolved))
        { Comment = source.Comment, IsTemporary = true };
        var rows = Variables.ToList();
        rows.Add(variant);
        return (new VariableCatalog(FilePath, rows, Warnings.ToList()), variant, true);
    }

    internal static bool IsWord(char value) => char.IsLetterOrDigit(value) || char.IsSurrogate(value) || value == '_'
        || char.GetUnicodeCategory(value) is UnicodeCategory.NonSpacingMark or UnicodeCategory.SpacingCombiningMark or UnicodeCategory.EnclosingMark;
}
