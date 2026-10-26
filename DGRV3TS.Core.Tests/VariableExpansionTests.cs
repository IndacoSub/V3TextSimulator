using DGRV3TS.Core;

internal static class VariableExpansionTests
{
    public static void Run(string root)
    {
        var path = Path.Combine(root, "invented-expansion-dictionary.txt");
        var content = """
            // Invented glossary
            VAR_FRUIT, mela : pera
            VAR_TITLE, Piccola VAR_FRUIT : Grande VAR_FRUIT
            VAR_ALIAS, VAR_FRUIT
            VAR_MESSAGE, MAKE_FRAME(VAR_TITLE)
            MAKE_FRAME(MY_ARG), [MY_ARG] : {MY_ARG}
            MAKE_TWICE(MY_ARG), MY_ARG / MY_ARG
            MAKE_LABEL(ITEM), Etichetta ITEM e XITEM ITEM2
            MAKE_NEST(MY_ARG), MAKE_FRAME(MAKE_TWICE(MY_ARG))
            MAKE_LOOP(MY_ARG), MAKE_LOOP(MY_ARG)
            VAR_CYCLE_A, VAR_CYCLE_B
            VAR_CYCLE_B, VAR_CYCLE_C
            VAR_CYCLE_C, VAR_CYCLE_D
            VAR_CYCLE_D, VAR_CYCLE_A
            VAR_GROW, VAR_GROW VAR_GROW
            VAR_MIXED, MAKE_FRAME(VAR_MIXED)
            <CLT=cltSTRONG>, do not replace
            <CLT=cltNORMAL> | invented note, with punctuation
            <SIGNAL_CUSTOM> | another invented note
            MAKE_BROKEN(), invalid definition
            """;
        content += "\n" + string.Join("\n", Enumerable.Range(0, 12)
            .Select(i => $"VAR_CHAIN_{i}, " + (i == 11 ? "VAR_FRUIT" : $"VAR_CHAIN_{i + 1}")));
        File.WriteAllText(path, content);
        var catalog = VariableCatalog.Load(path);

        Equal("Piccola mela", catalog.Resolve("VAR_TITLE"));
        Equal("Grande pera", catalog.Resolve("VAR_TITLE", true));
        Equal("mela", catalog.Resolve("VAR_CHAIN_0"));
        Equal("pera", catalog.Resolve("VAR_CHAIN_0", true));
        Equal("[mela]", catalog.Resolve("MAKE_FRAME(VAR_FRUIT)"));
        Equal("[[mela]]", catalog.Resolve("MAKE_FRAME(MAKE_FRAME(VAR_FRUIT))"));
        Equal("[mela / mela]", catalog.Resolve("MAKE_NEST(VAR_FRUIT)"));
        Equal("{pera / pera}", catalog.Resolve("MAKE_NEST(VAR_FRUIT)", true));
        Equal("[Piccola mela]", catalog.Resolve("VAR_MESSAGE"));
        Equal("{Grande pera}", catalog.Resolve("VAR_MESSAGE", true));
        Equal("pera", catalog.WithCaseVariant("VAR_ALIAS", false).Catalog.Resolve("VAR_ALIAS_MN", true));
        Equal("invented note, with punctuation", catalog.Variables.Single(v => v.Token == "<CLT=cltNORMAL>").Comment);
        Equal("<CLT=cltNORMAL><SIGNAL_CUSTOM>", catalog.Resolve("<CLT=cltNORMAL><SIGNAL_CUSTOM>"));
        Equal("[(uno, due)] poi [tre]", catalog.Resolve("MAKE_FRAME((uno, due)) poi MAKE_FRAME(tre)"));
        Equal("[]", catalog.Resolve("MAKE_FRAME()"));
        Equal("Etichetta mela e XITEM ITEM2", catalog.Resolve("MAKE_LABEL(VAR_FRUIT)"));
        Equal("XVAR_FRUIT VAR_FRUIT2 VAR_FRUIT́ XMAKE_FRAME(mela)",
            catalog.Resolve("XVAR_FRUIT VAR_FRUIT2 VAR_FRUIT́ XMAKE_FRAME(VAR_FRUIT)"));
        Equal("MAKE_UNKNOWN(mela)", catalog.Resolve("MAKE_UNKNOWN(VAR_FRUIT)"));
        Equal("MAKE_FRAME(VAR_FRUIT", catalog.Resolve("MAKE_FRAME(VAR_FRUIT"));
        Equal("<CLT=cltSTRONG>[mela]<CLT=cltNORMAL><SIGNAL_NDR>",
            catalog.Resolve("<CLT=cltSTRONG>MAKE_FRAME(VAR_FRUIT)<CLT=cltNORMAL><SIGNAL_NDR>"));
        Check(catalog.Resolve("VAR_CYCLE_A").Contains("VAR_CYCLE_"), "Long cycles must remain visible.");
        Equal("MAKE_LOOP(mela)", catalog.Resolve("MAKE_LOOP(VAR_FRUIT)"));
        Check(catalog.Resolve("VAR_MIXED").Contains("VAR_MIXED"), "Mixed macro/variable cycles must terminate.");
        Check(catalog.Resolve("VAR_GROW").Length < 100, "Self-expansion must not grow indefinitely.");
        Check(catalog.Warnings.Count >= 7, "Malformed macros and cycles must produce warnings.");
        Equal(content, File.ReadAllText(path));

        var expansionPath = Path.Combine(root, "invented-expansion-limits.txt");
        File.WriteAllText(expansionPath, string.Join("\n", Enumerable.Range(0, 24)
            .Select(i => $"VAR_DOUBLE_{i}, " + (i == 23 ? "end" : $"VAR_DOUBLE_{i + 1} VAR_DOUBLE_{i + 1}"))));
        var bounded = VariableCatalog.Load(expansionPath).Resolve("VAR_DOUBLE_0");
        Check(bounded.Length <= 1_000_000 && bounded.Contains("VAR_DOUBLE_"), "Exponential expansion must stay bounded and retain unresolved tokens.");
        var nested = "x";
        for (var i = 0; i < 24; i++) nested = "MAKE_TWICE(" + nested + ")";
        var macroBounded = catalog.Resolve(nested);
        Check(macroBounded.Length <= 1_000_000 && macroBounded.Contains("MAKE_TWICE(") && !macroBounded.Contains("MY_ARG"),
            "Macro expansion limits must retain calls instead of leaking template parameter names.");

        Check(TranslationTools.Validate("MAKE_FRAME(VAR_FRUIT)", "VAR_FRUIT", ["VAR_FRUIT", "MAKE_FRAME(MY_ARG)"]).Count > 0,
            "Removing a MAKE invocation must be reported by placeholder validation.");
        Equal(0, TranslationTools.Validate("MAKE_FRAME(VAR_FRUIT)", "MAKE_FRAME(VAR_FRUIT)", ["VAR_FRUIT", "MAKE_FRAME(MY_ARG)"]).Count);

        var document = TranslationDocument.Create();
        document.AddEntry("MAKE_FRAME(VAR_FRUIT)");
        const string rawTranslation = "MAKE_NEST(VAR_TITLE)";
        document.UpdateTranslation(document.Entries[0], rawTranslation);
        Equal("[Piccola mela / Piccola mela]", catalog.Resolve(document.Entries[0].Translation));
        var projectPath = Path.Combine(root, "invented-macro-project.json");
        document.Save(projectPath);
        var restored = TranslationDocument.Load(projectPath);
        var poPath = Path.Combine(root, "invented-macro-export.po");
        restored.Save(poPath);
        Equal(rawTranslation, TranslationDocument.Load(poPath).Entries[0].Translation);
        Equal("MAKE_FRAME(VAR_FRUIT)", TranslationDocument.Load(poPath).Entries[0].Original);
        Equal(content, File.ReadAllText(path));
    }

    private static void Equal<T>(T expected, T actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual)) throw new Exception($"Expected [{expected}], actual [{actual}]");
    }
    private static void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
}
