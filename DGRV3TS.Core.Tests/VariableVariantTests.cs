using DGRV3TS.Core;

internal static class VariableVariantTests
{
    public static void Run(string root)
    {
        var path = Path.Combine(root, "case-variants.txt");
        const string content = "// Names\nVAR_NAME, Kaede : KAEDE ALT | protagonist\nVAR_NAME_MS, EXPLICIT UPPER\n<BRACKET>, Original\n<BRACKET_MN>, explicit lower\n";
        File.WriteAllText(path, content);
        var original = VariableCatalog.Load(path);
        var (catalog, lower, created) = original.WithCaseVariant("VAR_NAME", false);
        Check(created && lower.IsTemporary && lower.Token == "VAR_NAME_MN", "Missing variant should be generated with the v25 suffix.");
        Check(catalog.Resolve("VAR_NAME_MN VAR_NAME") == "kaede Kaede", "Generated token must beat the shorter base token.");
        Check(catalog.Resolve("VAR_NAME_MN", true) == "kaede alt", "Alternative convention must be case-converted too.");
        Check(lower.Comment == "protagonist" && lower.Category == "Names", "Variant must retain dictionary context.");
        Check(original.Resolve("VAR_NAME_MN") == "VAR_NAME_MN", "Generating a variant must not mutate the original catalog.");
        var repeated = catalog.WithCaseVariant("VAR_NAME", false);
        Check(!repeated.Created && ReferenceEquals(catalog, repeated.Catalog), "Repeated generation must reuse the existing definition.");
        var upper = catalog.WithCaseVariant("VAR_NAME", true);
        Check(!upper.Created && upper.Variant.Value == "EXPLICIT UPPER", "External values must take precedence.");
        var bracket = catalog.WithCaseVariant("<BRACKET>", false);
        Check(!bracket.Created && bracket.Variant.Token == "<BRACKET_MN>", "Existing bracket variants must remain compatible.");
        var bracketUpper = catalog.WithCaseVariant("<BRACKET>", true);
        Check(bracketUpper.Created && bracketUpper.Variant.Token == "<BRACKET>_MS", "New bracket variants must follow the original whole-token suffix.");
        Check(bracketUpper.Catalog.Resolve("<BRACKET>_MS") == "ORIGINAL", "Whole-token suffixed variants must resolve as a unit.");
        Check(File.ReadAllText(path) == content, "Session variants must never modify the external file.");
        Check(!VariableCatalog.Load(path).Variables.Any(v => v.Token == "VAR_NAME_MN"), "Reload must remove temporary variants.");
    }

    private static void Check(bool value, string message) { if (!value) throw new Exception(message); }
}
