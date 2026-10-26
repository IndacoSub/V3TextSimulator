using System.Text.Json;
using DGRV3TS.Core;

internal static class ActiveLanguageEditTests
{
    public static void Run(string root)
    {
        var path = Path.Combine(root, "invented-language-batch.json");
        var rows = Enumerable.Range(0, 1200).Select(i => new TranslationEntry { Id = i.ToString(), Original = "Invented source " + i }).ToArray();
        File.WriteAllText(path, JsonSerializer.Serialize(new
        {
            Version = 1, Entries = rows, LanguageCount = 3, SelectedLanguage = 1,
            Languages = rows.Select((_, i) => new[] { "First " + i, "", "Third " + i }).ToArray()
        }));
        var document = TranslationDocument.Load(path);
        for (var i = 0; i < rows.Length; i++) document.UpdateTranslation(document.Entries[i], "Second " + i);
        document.UpdateTranslation(document.Entries[0], "");
        document.AddEntry("Invented added row");
        document.UpdateTranslation(document.Entries[^1], "Added second language");
        document.SelectLanguage(0);
        Equal("First 0", document.Entries[0].Translation);
        Equal("First 1199", document.Entries[1199].Translation);
        document.UpdateTranslation(document.Entries[1], "Edited first");
        document.SelectLanguage(2);
        Equal("Third 0", document.Entries[0].Translation);
        document.SelectLanguage(1);
        Equal("", document.Entries[0].Translation); Equal(true, document.Entries[0].HasTranslation);
        Equal("Second 1199", document.Entries[1199].Translation);
        Equal("Added second language", document.Entries[^1].Translation);
        document.UpdateTranslation(document.Entries[1], "Last edit before save");
        var saved = Path.Combine(root, "invented-language-batch-saved.json");
        document.Save(saved);
        var read = TranslationDocument.Load(saved);
        Equal(3, read.LanguageCount);
        Equal("Last edit before save", read.Entries[1].Translation);
        Equal(true, read.Entries[0].HasTranslation);
        var po = Path.Combine(root, "invented-language-batch.po"); read.Save(po);
        Equal("Last edit before save", TranslationDocument.Load(po).Entries[1].Translation);
        read.SelectLanguage(0); Equal("Edited first", read.Entries[1].Translation);
        read.SelectLanguage(2); Equal("Third 1", read.Entries[1].Translation);
    }

    private static void Equal<T>(T expected, T actual)
    { if (!EqualityComparer<T>.Default.Equals(expected, actual)) throw new Exception($"Expected [{expected}], actual [{actual}]."); }
}
