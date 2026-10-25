using System.Text;
using System.Text.RegularExpressions;

namespace DGRV3TS.Core;

public sealed partial class TranslationDocument
{
    private sealed record PoMetadata(string Id, string Origin, string Speaker, string Expression, string Voice);
    private sealed record PoRecord(List<string> Lines, int Start, int End, int ContextStart, int ContextEnd, int OriginalStart, string Context, PoMetadata Metadata);
    private readonly Dictionary<TranslationEntry, PoRecord> poRecords = [];
    private readonly List<string> poHeaders = [];
    private bool txtBraces;
    private bool txtFinalNewline;

    private void ReadPo(string text)
    {
        foreach (var block in Regex.Split(text.Replace("\r\n", "\n").Replace('\r', '\n'), "\n[\\t ]*\n"))
        {
            var lines = block.Split('\n').ToList();
            while (lines.Count > 0 && lines[^1].Length == 0) lines.RemoveAt(lines.Count - 1);
            if (lines.Count == 0) continue;
            if (lines.Any(l => l.StartsWith("msgid_plural ", StringComparison.Ordinal) || l.StartsWith("msgstr[", StringComparison.Ordinal)))
                throw new NotSupportedException("PO con plurali non supportato: le traduzioni non verranno scartate.");
            var original = PoField(lines, "msgid", out var originalStart, out _);
            var translation = PoField(lines, "msgstr", out var start, out var end);
            var context = PoField(lines, "msgctxt", out var contextStart, out var contextEnd);
            if (original is null || start < 0)
            {
                if (lines.All(l => l.TrimStart().StartsWith('#'))) { poHeaders.Add(string.Join(newline, lines)); continue; }
                throw new InvalidDataException("Blocco PO senza msgid/msgstr.");
            }
            if (original.Length == 0 && context is null)
            {
                var charset = Regex.Match(translation ?? "", @"charset\s*=\s*([^\s;]+)", RegexOptions.IgnoreCase);
                if (charset.Success && !charset.Groups[1].Value.Equals("UTF-8", StringComparison.OrdinalIgnoreCase)
                    && !charset.Groups[1].Value.Equals("UTF8", StringComparison.OrdinalIgnoreCase))
                    throw new NotSupportedException("Converti il PO in UTF-8 prima di aprirlo (charset " + charset.Groups[1].Value + ").");
                poHeaders.Add(string.Join(newline, lines));
                continue;
            }
            var pieces = (context ?? "").Split(" | ", StringSplitOptions.None);
            var expression = pieces.ElementAtOrDefault(3) ?? "";
            var voice = pieces.ElementAtOrDefault(4) ?? "";
            // v25 ReadMessageContext treats a fourth vic_* field as voice rather than animation.
            if (expression.StartsWith("vic", StringComparison.Ordinal)) { voice = expression; expression = ""; }
            var entry = new TranslationEntry
            {
                Id = pieces[0].Length == 0 ? (Entries.Count + 1).ToString() : pieces[0], Context = context ?? "",
                Origin = pieces.ElementAtOrDefault(1) ?? "", Speaker = pieces.ElementAtOrDefault(2) ?? "",
                Expression = expression, Voice = voice, Original = original, Translation = translation ?? ""
            };
            Add(entry);
            poRecords.Add(entry, new PoRecord(lines, start, end, contextStart, contextEnd, originalStart, context ?? "", MetadataOf(entry)));
        }
    }

    private static string? PoField(List<string> lines, string name, out int start, out int end)
    {
        start = end = -1;
        if (lines.Count(l => l.StartsWith(name + " ", StringComparison.Ordinal)) > 1) throw new InvalidDataException("Campo PO duplicato: " + name);
        for (var i = 0; i < lines.Count; i++)
        {
            if (!lines[i].StartsWith(name + " ", StringComparison.Ordinal)) continue;
            start = i; end = i + 1;
            var result = new StringBuilder(Unquote(lines[i][name.Length..].Trim()));
            while (end < lines.Count && lines[end].TrimStart().StartsWith('"')) result.Append(Unquote(lines[end++].Trim()));
            return result.ToString();
        }
        return null;
    }

    private static string Unquote(string value, bool legacyVo = false)
    {
        if (value.Length < 2 || value[0] != '"' || value[^1] != '"') throw new InvalidDataException("Stringa non quotata.");
        var inner = value[1..^1];
        if (legacyVo && inner == "(blank)") return "";
        var result = new StringBuilder();
        for (var i = 0; i < inner.Length; i++)
        {
            var c = inner[i];
            if (c == '"' && !legacyVo) throw new InvalidDataException("Virgolette PO non precedute da escape.");
            if (c != '\\') { result.Append(c); continue; }
            if (++i >= inner.Length)
            {
                if (legacyVo) { result.Append('\\'); break; }
                throw new InvalidDataException("Escape incompleto.");
            }
            switch (inner[i])
            {
                case 'n': result.Append('\n'); break; case 'r': result.Append('\r'); break; case 't': result.Append('\t'); break;
                case 'b': result.Append('\b'); break; case 'f': result.Append('\f'); break; case 'v': result.Append('\v'); break;
                case 'a': result.Append('\a'); break; case '"': result.Append('"'); break; case '\\': result.Append('\\'); break;
                default:
                    if (legacyVo) result.Append('\\').Append(inner[i]);
                    else throw new NotSupportedException("Escape PO non supportato: \\" + inner[i]);
                    break;
            }
        }
        return result.ToString();
    }

    private static string Quote(string text) => "\"" + text.Replace("\\", "\\\\").Replace("\"", "\\\"")
        .Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t").Replace("\b", "\\b")
        .Replace("\f", "\\f").Replace("\v", "\\v").Replace("\a", "\\a") + "\"";

    private static PoMetadata MetadataOf(TranslationEntry entry) => new(entry.Id, entry.Origin, entry.Speaker, entry.Expression, entry.Voice);

    private static string EntryContext(TranslationEntry entry, bool preserveContext = true)
    {
        if (preserveContext && entry.Context.Length > 0) return entry.Context;
        var fields = new List<string> { entry.Id };
        if (entry.Origin.Length > 0 || entry.Speaker.Length > 0 || entry.Expression.Length > 0 || entry.Voice.Length > 0) fields.Add(entry.Origin);
        if (entry.Speaker.Length > 0 || entry.Expression.Length > 0 || entry.Voice.Length > 0) fields.Add(entry.Speaker);
        // v25 accepts vic_* as the fourth field. Other voice names need an empty expression field.
        if (entry.Expression.Length == 0 && entry.Voice.StartsWith("vic", StringComparison.Ordinal))
        { fields.Add(entry.Voice); return string.Join(" | ", fields); }
        if (entry.Expression.Length > 0 || entry.Voice.Length > 0) fields.Add(entry.Expression);
        if (entry.Voice.Length > 0) fields.Add(entry.Voice);
        return string.Join(" | ", fields);
    }

    private string WritePo()
    {
        var blocks = new List<string>(poHeaders);
        if (blocks.Count == 0) blocks.Add("msgid \"\"" + newline + "msgstr \"Content-Type: text/plain; charset=UTF-8\\n\"");
        foreach (var entry in Entries)
        {
            if (poRecords.TryGetValue(entry, out var raw))
            {
                var lines = raw.Lines.ToList();
                var replacements = new List<(int Start, int End, string Text)>
                { (raw.Start, raw.End, "msgstr " + Quote(entry.Translation)) };
                var metadataChanged = MetadataOf(entry) != raw.Metadata;
                if (metadataChanged || entry.Context != raw.Context)
                {
                    var context = metadataChanged ? EntryContext(entry, preserveContext: false) : entry.Context;
                    var contextStart = raw.ContextStart >= 0 ? raw.ContextStart : raw.OriginalStart;
                    var contextEnd = raw.ContextStart >= 0 ? raw.ContextEnd : raw.OriginalStart;
                    replacements.Add((contextStart, contextEnd, "msgctxt " + Quote(context)));
                }
                // Apply edits back-to-front so multiline contexts and translations keep stable offsets.
                foreach (var replacement in replacements.OrderByDescending(r => r.Start))
                {
                    lines.RemoveRange(replacement.Start, replacement.End - replacement.Start);
                    lines.Insert(replacement.Start, replacement.Text);
                }
                blocks.Add(string.Join(newline, lines));
            }
            else blocks.Add("msgctxt " + Quote(EntryContext(entry)) + newline + "msgid " + Quote(entry.Original) + newline + "msgstr " + Quote(entry.Translation));
        }
        return string.Join(newline + newline, blocks) + newline;
    }

    private void ReadTxt(string text)
    {
        txtFinalNewline = text.EndsWith('\n') || text.EndsWith('\r');
        var lines = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n').ToList();
        if (txtFinalNewline) lines.RemoveAt(lines.Count - 1);
        txtBraces = lines.Count >= 2 && lines[0] == "{" && lines[^1] == "}";
        if (txtBraces) { lines.RemoveAt(lines.Count - 1); lines.RemoveAt(0); }
        if (text.Length == 0) lines.Clear();
        foreach (var line in lines) Add(new TranslationEntry { Id = (Entries.Count + 1).ToString(), Original = line });
    }

    private string WriteTxt()
    {
        var lines = Entries.Select(e => ExportText(e).Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\\n")).ToList();
        // Keep imported TXT wrappers; v25 export from structured formats uses braces.
        if (txtBraces || sourceExtension != ".txt") { lines.Insert(0, "{"); lines.Add("}"); }
        return string.Join(newline, lines) + (txtFinalNewline ? newline : "");
    }

    private static string ExportText(TranslationEntry entry) => entry.HasTranslation || entry.Translation.Length > 0 ? entry.Translation : entry.Original;
}
