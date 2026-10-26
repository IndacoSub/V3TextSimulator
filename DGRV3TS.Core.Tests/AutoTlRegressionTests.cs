using DGRV3TS.Core;

internal static class AutoTlRegressionTests
{
    public static void Run(string root)
    {
        var directory = Path.Combine(root, "autotl-regressions");
        Directory.CreateDirectory(directory);
        string Donor(string name, string original, string speaker, string translation)
        {
            var document = TranslationDocument.Create();
            document.AddEntry(original);
            document.Entries[0].Speaker = speaker;
            document.UpdateTranslation(document.Entries[0], translation);
            var path = Path.Combine(directory, name + ".json");
            document.Save(path);
            return path;
        }

        var current = Donor("a-current", "Duplicate source", "HERO", "Traduzione duplicata");
        var external = Donor("b-external", "Duplicate source", "HERO", "Traduzione duplicata");
        Donor("c-invalid", "Hello <NAME>", "HERO", "Ciao senza variabile");
        Donor("d-valid", "Hello <NAME>", "OTHER", "Ciao <NAME>");
        Donor("e-source-copy", "Unchanged source", "HERO", "Unchanged source");
        Donor("f-translated", "Unchanged source", "OTHER", "Originale tradotto");
        Donor("g-variant", "Hello <NAME>_MS", "HERO", "Ciao <NAME>_MN");
        var memory = TranslationMemory.Scan(directory);

        var duplicate = TranslationReuse.FindSuggestions(memory,
            new TranslationEntry { Original = "Duplicate source", Speaker = "HERO" }, 0, [], excludedFile: current);
        Check(duplicate.Count == 1 && duplicate[0].SourceFile == external,
            "Excluding the current file must retain an identical translation from another file.");

        var fallback = TranslationReuse.FindSuggestions(memory,
            new TranslationEntry { Original = "Hello <NAME>", Speaker = "HERO" }, 0, ["<NAME>"]);
        Check(fallback.Count == 1 && fallback[0].Text == "Ciao <NAME>",
            "An incompatible same-speaker candidate must not hide a usable different-speaker candidate.");
        var unchanged = TranslationReuse.FindSuggestions(memory,
            new TranslationEntry { Original = "Unchanged source", Speaker = "HERO" }, 0, []);
        Check(unchanged.Count == 1 && unchanged[0].Text == "Originale tradotto",
            "An untranslated source copy must not hide an actual translation by another speaker.");

        string[] tokens = ["<NAME>", "<NAME>_MS", "<NAME>_MN", "NAME"];
        Check(TranslationTools.Validate("Hello <NAME>_MS", "Ciao <NAME>_MN", tokens).Count == 2,
            "Whole-token case suffixes must be validated as distinct variables.");
        Check(TranslationTools.Validate("Hello <NAME>_MS", "Ciao <NAME>_MS", tokens).Count == 0,
            "Unchanged whole-token variants must remain compatible.");
        Check(TranslationTools.Validate("Hello <NAME>", "Ciao NAME", tokens).Count > 0,
            "A dictionary name inside a tag must not hide removal of the tag delimiters.");
        Check(TranslationReuse.FindSuggestions(memory,
            new TranslationEntry { Original = "Hello <NAME>_MS", Speaker = "HERO" }, 0, tokens).Count == 0,
            "AutoTL must reject a donor that changes a protected case variant.");
    }

    private static void Check(bool value, string message) { if (!value) throw new Exception(message); }
}
