using System.Text;
using DGRV3TS.Core;

internal static class PoOptionalMetadataTests
{
    private sealed record Row(string? Context, string Origin, string Speaker, string Expression, string Voice);

    public static void Run(string root)
    {
        Row[] rows =
        [
            new("0010 | demo_scene | ACTOR_DEMO | anim_demo | vic_demo", "demo_scene", "ACTOR_DEMO", "anim_demo", "vic_demo"),
            new("0011 | demo_scene | ACTOR_DEMO", "demo_scene", "ACTOR_DEMO", "", ""),
            new("0012 | demo_scene | ACTOR_DEMO | vic_demo", "demo_scene", "ACTOR_DEMO", "", "vic_demo"),
            new("0013 | demo_scene | ACTOR_DEMO | anim_demo", "demo_scene", "ACTOR_DEMO", "anim_demo", ""),
            new("0014 | demo_scene", "demo_scene", "", "", ""),
            new("0015", "", "", "", ""),
            new(null, "", "", "", ""),
            new("0017 | demo_scene |  |  | custom_demo_voice", "demo_scene", "", "", "custom_demo_voice"),
            new("0018 | demo_scene |  | anim_demo", "demo_scene", "", "anim_demo", ""),
            new("0019 | demo_scene | ACTOR_OTHER | anim_other | vic_other", "demo_scene", "ACTOR_OTHER", "anim_other", "vic_other"),
            new("0020 | demo_scene |  |  | ", "demo_scene", "", "", ""),
            new("", "", "", "", "")
        ];
        var text = new StringBuilder("msgid \"\"\nmsgstr \"\"\n\"Content-Type: text/plain; charset=UTF-8\\n\"\n\n");
        for (var i = 0; i < rows.Length; i++)
        {
            text.AppendLine($"#. Invented metadata case {i}");
            if (rows[i].Context is { } context) text.AppendLine($"msgctxt \"{context}\"");
            text.AppendLine($"msgid \"\"\n\"Invented source \"\n\"{i}\"");
            text.AppendLine($"msgstr \"\"\n\"Invented translation {i}\\n\"\n\"Second line\"\n");
        }
        var path = Path.Combine(root, "invented-optional-metadata.po");
        File.WriteAllText(path, text.ToString());
        var document = TranslationDocument.Load(path);
        Check(document);

        // Translation edits alone must never fill metadata from a filename or adjacent row.
        for (var i = 0; i < document.Entries.Count; i++)
            document.UpdateTranslation(document.Entries[i], document.Entries[i].Translation + " edited");
        var direct = Path.Combine(root, "invented-optional-metadata-direct.po");
        document.Save(direct);
        Check(TranslationDocument.Load(direct), edited: true);
        var project = Path.Combine(root, "invented-optional-metadata.json");
        document.Save(project);
        var restored = TranslationDocument.Load(project);
        Check(restored, edited: true);
        var exported = Path.Combine(root, "invented-optional-metadata-export.po");
        restored.Save(exported);
        Check(TranslationDocument.Load(exported), edited: true);
        Equal(rows.Count(r => r.Context is not null), File.ReadLines(exported).Count(l => l.StartsWith("msgctxt ", StringComparison.Ordinal)));
        Equal(text.ToString(), File.ReadAllText(path));

        void Check(TranslationDocument doc, bool edited = false)
        {
            Equal(rows.Length, doc.Entries.Count);
            for (var i = 0; i < rows.Length; i++)
            {
                var entry = doc.Entries[i];
                var expected = rows[i];
                Equal(expected.Context ?? "", entry.Context);
                Equal(expected.Origin, entry.Origin);
                Equal(expected.Speaker, entry.Speaker);
                Equal(expected.Expression, entry.Expression);
                Equal(expected.Voice, entry.Voice);
                Equal($"Invented source {i}", entry.Original);
                Equal($"Invented translation {i}\nSecond line" + (edited ? " edited" : ""), entry.Translation);
            }
        }
    }

    private static void Equal<T>(T expected, T actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
            throw new Exception($"Expected [{expected}], actual [{actual}].");
    }
}
