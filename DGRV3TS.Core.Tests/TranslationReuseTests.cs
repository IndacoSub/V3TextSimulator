using DGRV3TS.Core;

internal static class TranslationReuseTests
{
    public static void Run(string root)
    {
        var directory = Path.Combine(root, "reuse-policy");
        Directory.CreateDirectory(directory);
        var donor = TranslationDocument.Create();
        donor.AddEntry("Hello <NAME> {0}");
        donor.Entries[0].Speaker = "C000_Saiha";
        donor.UpdateTranslation(donor.Entries[0], "Ciao <NAME> {0}<SIGNAL_AutoTL>old provenance");
        donor.Save(Path.Combine(directory, "good.json"));
        var conflicting = TranslationDocument.Create();
        conflicting.AddEntry("Hello <NAME> {0}");
        conflicting.Entries[0].Speaker = "C000_Saiha";
        conflicting.UpdateTranslation(conflicting.Entries[0], "Salve <NAME> {0}");
        conflicting.Save(Path.Combine(directory, "other.json"));
        var broken = TranslationDocument.Create();
        broken.AddEntry("Hello <NAME> {0}");
        broken.Entries[0].Speaker = "C000_Saiha";
        broken.UpdateTranslation(broken.Entries[0], "Missing placeholders");
        broken.Save(Path.Combine(directory, "broken.json"));
        var unchanged = TranslationDocument.Create();
        unchanged.AddEntry("Hello <NAME> {0}");
        unchanged.Entries[0].Speaker = "C000_Saiha";
        unchanged.UpdateTranslation(unchanged.Entries[0], "Hello <NAME> {0}");
        unchanged.Save(Path.Combine(directory, "unchanged.json"));

        var target = new TranslationEntry { Original = "Hello <NAME> {0}", Speaker = "C000_Saiha" };
        var memory = TranslationMemory.Scan(directory);
        var automatic = TranslationReuse.FindSuggestions(memory, target, 0, ["<NAME>"]);
        Check(automatic.Count == 2, "Conflicting valid translations must remain separate choices.");
        Check(automatic.Any(s => s.Text == "Ciao <NAME> {0}"), "AutoTL provenance must be removed before proposing reuse.");
        Check(TranslationReuse.FindSuggestions(memory, target, 0, ["<NAME>"], false).Count == 3,
            "Explicit review may still inspect an incompatible suggestion; original copies are not translations.");
        Check(TranslationReuse.FindSuggestions(memory, target, 0, ["<NAME>"], excludedFile: Path.Combine(directory, "good.json")).Count == 1,
            "A reused index must not suggest the current document's own rows.");
        Check(TranslationReuse.IsUntranslated(target), "A new empty row should be eligible.");
        target.HasTranslation = true;
        Check(!TranslationReuse.IsUntranslated(target), "An intentionally empty translation must never be overwritten.");
        target.HasTranslation = false; target.Translation = "Already translated";
        Check(!TranslationReuse.IsUntranslated(target), "Existing text must never be automatically overwritten.");
        Check(TranslationReuse.PrepareTranslation("Ciao<SIGNAL_AutoTL>old") == "Ciao<SIGNAL_AutoTL>", "Reuse must append exactly one marker.");
        Check(TranslationReuse.PrepareTranslation("Ciao<SIGNAL_ALT>preserve this tail") == "Ciao<SIGNAL_ALT>preserve this tail<SIGNAL_AutoTL>",
            "Other game-signal suffixes must survive reuse.");
        Check(TranslationReuse.SupportsAutomaticReuse("chapter.PO") && TranslationReuse.SupportsAutomaticReuse("chapter.vo")
            && TranslationReuse.SupportsAutomaticReuse("chapter.xlsx") && !TranslationReuse.SupportsAutomaticReuse("chapter.stx")
            && !TranslationReuse.SupportsAutomaticReuse("chapter.json"), "Automatic triggers must match v25 supported translation formats.");
        Check(File.ReadAllText(Path.Combine(directory, "good.json")).Contains("old provenance"), "Memory cleanup must not rewrite donor files.");
        var threw = false;
        try { TranslationReuse.PrepareTranslation("<SIGNAL_AutoTL>only metadata"); } catch (ArgumentException) { threw = true; }
        Check(threw, "An empty candidate must not become a marker-only translation.");
    }

    private static void Check(bool value, string message) { if (!value) throw new Exception(message); }
}
