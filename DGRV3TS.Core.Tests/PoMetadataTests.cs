using DGRV3TS.Core;

internal static class PoMetadataTests
{
    public static void Run(string root)
    {
        var directory = Path.Combine(root, "po-metadata");
        Directory.CreateDirectory(directory);
        string PathOf(string name) => Path.Combine(directory, name);
        const string metadata = "#. keep this note\n#: chapter:10\nmsgctxt \"0010 | chapter | OLD | anim000 | vic_OLD\"\nmsgid \"Original\"\nmsgstr \"Traduzione\"\n";
        File.WriteAllText(PathOf("original.po"), metadata);

        foreach (var throughProject in new[] { false, true })
        {
            var doc = TranslationDocument.Load(PathOf("original.po"));
            var entry = doc.Entries.Single();
            entry.Id = "0042"; entry.Origin = "new-chapter"; entry.Speaker = "NEW";
            entry.Expression = "anim005"; entry.Voice = "vic_NEW";
            doc.MarkDirty();
            if (throughProject)
            {
                doc.Save(PathOf("metadata.json"));
                doc = TranslationDocument.Load(PathOf("metadata.json"));
            }
            var path = PathOf(throughProject ? "project-export.po" : "direct-export.po");
            doc.Save(path);
            var read = TranslationDocument.Load(path).Entries.Single();
            Equal("0042", read.Id); Equal("new-chapter", read.Origin); Equal("NEW", read.Speaker);
            Equal("anim005", read.Expression); Equal("vic_NEW", read.Voice);
            Equal("Original", read.Original); Equal("Traduzione", read.Translation);
            Check(File.ReadAllText(path).Contains("#. keep this note\n#: chapter:10"), "Metadata edits lost PO comments.");
        }

        const string arbitrary = "msgid \"\"\nmsgstr \"Content-Type: text/plain; charset=UTF-8\\n\"\n\n#. arbitrary context note\nmsgctxt \"menu/intro: custom|context\"\nmsgid \"Intro\"\nmsgstr \"Prima\"\n";
        File.WriteAllText(PathOf("arbitrary.po"), arbitrary);
        var untouched = TranslationDocument.Load(PathOf("arbitrary.po"));
        untouched.UpdateTranslation(untouched.Entries[0], "Nuova");
        untouched.Save(PathOf("arbitrary-export.po"));
        Equal(arbitrary.Replace("Prima", "Nuova").TrimEnd(), File.ReadAllText(PathOf("arbitrary-export.po")).TrimEnd());

        foreach (var voice in new[] { "vic_VOICE", "custom_audio_name" })
        {
            File.WriteAllText(PathOf("no-context.po"), "#. no-context note\nmsgid \"Hello\"\nmsgstr \"Ciao\"\n");
            var doc = TranslationDocument.Load(PathOf("no-context.po"));
            var entry = doc.Entries.Single();
            entry.Speaker = "SPEAKER"; entry.Origin = "chapter"; entry.Voice = voice;
            doc.Save(PathOf("voice-only.po"));
            var read = TranslationDocument.Load(PathOf("voice-only.po")).Entries.Single();
            Equal("SPEAKER", read.Speaker); Equal("", read.Expression); Equal(voice, read.Voice);
            Check(File.ReadAllText(PathOf("voice-only.po")).Contains("#. no-context note"), "Adding context lost the preceding comment.");
        }

        File.WriteAllText(PathOf("multiline.po"), "#. multiline note\nmsgctxt \"\"\n\"0010 | chapter | \"\n\"OLD | anim000 | vic_OLD\"\nmsgid \"Original\"\nmsgstr \"\"\n\"Tradu\"\n\"zione\"\n");
        var multiline = TranslationDocument.Load(PathOf("multiline.po"));
        multiline.Entries[0].Voice = "vic_REPLACED";
        multiline.UpdateTranslation(multiline.Entries[0], "Nuova traduzione");
        multiline.Save(PathOf("multiline-export.po"));
        var rewritten = TranslationDocument.Load(PathOf("multiline-export.po")).Entries.Single();
        Equal("vic_REPLACED", rewritten.Voice); Equal("Nuova traduzione", rewritten.Translation);
        Equal("OLD", rewritten.Speaker); Equal("anim000", rewritten.Expression);
    }

    private static void Check(bool value, string message) { if (!value) throw new Exception(message); }
    private static void Equal(string expected, string actual)
    { if (expected != actual) throw new Exception($"Expected [{expected}], actual [{actual}]."); }
}
