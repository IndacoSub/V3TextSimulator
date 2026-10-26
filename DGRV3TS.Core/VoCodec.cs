namespace DGRV3TS.Core;

public sealed partial class TranslationDocument
{
    private readonly Dictionary<TranslationEntry, List<string>> voComments = [];
    private string voHeader = "";

    private void ReadVo(string text)
    {
        var lines = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        var position = Array.FindIndex(lines, l => l.StartsWith("Lines: ", StringComparison.Ordinal));
        if (position < 0) throw new InvalidDataException("Nessuna battuta VO trovata.");
        voHeader = string.Join(newline, lines.Take(position)).TrimEnd('\r', '\n');
        while (position < lines.Length)
        {
            while (position < lines.Length && lines[position].Length == 0) position++;
            if (position == lines.Length) break;
            string Field(string prefix)
            {
                if (position >= lines.Length || !lines[position].StartsWith(prefix, StringComparison.Ordinal)) throw new InvalidDataException("Campo VO atteso: " + prefix);
                return lines[position++][prefix.Length..];
            }
            int Count(string prefix)
            {
                if (!int.TryParse(Field(prefix), out var count) || count < 0) throw new InvalidDataException("Conteggio VO non valido: " + prefix);
                return count;
            }
            Count("Lines: ");
            var commentCount = Count("Number of comments: ");
            Field("Comments:");
            var comments = new List<string>();
            for (var i = 0; i < commentCount; i++)
            {
                if (position >= lines.Length) throw new InvalidDataException("Commenti VO incompleti.");
                comments.Add(lines[position++]);
            }
            var origin = Field("File: "); var id = Field("Line: "); var speaker = Field("Character: ");
            Count("Original message length: ");
            var original = Unquote(Field("Original message: "), true);
            var translationCount = Count("Number of translations: ");
            Field("Translations:");
            var translations = new List<string>();
            for (var i = 0; i < translationCount; i++)
            {
                if (position >= lines.Length) throw new InvalidDataException("Traduzioni VO incomplete.");
                translations.Add(Unquote(lines[position++], true));
            }
            if (translations.Count == 0) translations.Add("");
            var entry = new TranslationEntry { Id = id, Origin = origin, Speaker = speaker, Original = original, Translation = translations[0] };
            Add(entry, translations); voComments[entry] = comments;
        }
    }

    private string WriteVo()
    {
        var lines = new List<string>();
        if (voHeader.Length > 0) { lines.Add(voHeader); lines.Add(""); }
        for (var i = 0; i < Entries.Count; i++)
        {
            var entry = Entries[i]; var translations = languages[i];
            if (entry.Original == "(blank)" || translations.Contains("(blank)")) throw new NotSupportedException("Il testo letterale (blank) è riservato dal VO legacy: salvalo come JSON.");
            var comments = voComments.GetValueOrDefault(entry) ?? [];
            lines.Add("Lines: " + (10 + comments.Count + translations.Count));
            lines.Add("Number of comments: " + comments.Count); lines.Add("Comments:"); lines.AddRange(comments);
            lines.Add("File: " + entry.Origin); lines.Add("Line: " + entry.Id); lines.Add("Character: " + entry.Speaker);
            lines.Add("Original message length: " + entry.Original.Length); lines.Add("Original message: " + Quote(entry.Original));
            lines.Add("Number of translations: " + translations.Count); lines.Add("Translations:"); lines.AddRange(translations.Select(Quote)); lines.Add("");
        }
        return string.Join(newline, lines);
    }
}
