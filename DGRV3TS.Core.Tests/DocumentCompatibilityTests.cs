using System.Text.Json.Nodes;
using DGRV3TS.Core;

internal static class DocumentCompatibilityTests
{
    public static void Run(string root)
    {
        var directory = Path.Combine(root, "document-compatibility");
        Directory.CreateDirectory(directory);
        var txt = Path.Combine(directory, "old-mac.txt");
        File.WriteAllText(txt, "{\rfirst\r\rlast\r}\r");
        var doc = TranslationDocument.Load(txt);
        Equal(3, doc.Entries.Count);
        Equal("", doc.Entries[1].Original);
        doc.UpdateTranslation(doc.Entries[2], "ultimo");
        var output = Path.Combine(directory, "saved.txt"); doc.Save(output);
        Equal("{\rfirst\r\rultimo\r}\r", File.ReadAllText(output));
        doc.Save(Path.Combine(directory, "old-mac.json"));
        TranslationDocument.Load(Path.Combine(directory, "old-mac.json")).Save(Path.Combine(directory, "again.txt"));
        Equal(File.ReadAllText(output), File.ReadAllText(Path.Combine(directory, "again.txt")));

        var po = Path.Combine(directory, "old-mac.po");
        File.WriteAllText(po, "msgctxt \"001\"\rmsgid \"Source\"\rmsgstr \"Traduzione\"\r");
        var oldPo = TranslationDocument.Load(po); Equal("Traduzione", oldPo.Entries.Single().Translation);
        oldPo.Save(Path.Combine(directory, "output.vo"));
        var vo = File.ReadAllText(Path.Combine(directory, "output.vo")).Replace("\n", "\r");
        // Existing CR-only output needs no further normalization.
        File.WriteAllText(Path.Combine(directory, "mac.vo"), vo);
        Equal("Traduzione", TranslationDocument.Load(Path.Combine(directory, "mac.vo")).Entries.Single().Translation);

        foreach (var content in new[] { "{}", "[]", "{\"sdk\":{\"version\":\"8.0\"}}", "{\"Version\":1}" })
        {
            var invalid = Path.Combine(directory, "not-a-project.json"); File.WriteAllText(invalid, content);
            try { TranslationDocument.Load(invalid); throw new Exception("Unrelated JSON was accepted as an empty translation project."); }
            catch (InvalidDataException) { }
        }

        // Restore language columns from the embedded workbook when migrating old rewrite JSON.
        var source = Path.Combine(root, "xlsx-language-extent", "languages.xlsx");
        var languageDoc = TranslationDocument.Load(source);
        var migration = Path.Combine(directory, "old-language-project.json");
        languageDoc.Save(migration);
        var json = JsonNode.Parse(File.ReadAllText(migration))!.AsObject();
        json.Remove("LanguageCount");
        foreach (var row in json["Languages"]!.AsArray()) while (row!.AsArray().Count > 1) row.AsArray().RemoveAt(1);
        foreach (var row in json["Translated"]!.AsArray()) while (row!.AsArray().Count > 1) row.AsArray().RemoveAt(1);
        File.WriteAllText(migration, json.ToJsonString());
        var restored = TranslationDocument.Load(migration);
        Equal(2, restored.LanguageCount); restored.SelectLanguage(1);
        Equal("", restored.Entries[0].Translation);
    }

    private static void Equal<T>(T expected, T actual)
    { if (!EqualityComparer<T>.Default.Equals(expected, actual)) throw new Exception($"Expected [{expected}], actual [{actual}]"); }
}
