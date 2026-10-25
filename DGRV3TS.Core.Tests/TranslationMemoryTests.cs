using System.Text.Json;
using DGRV3TS.Core;

internal static class TranslationMemoryTests
{
    public static void Run(string temporaryRoot)
    {
        var directory = Path.Combine(temporaryRoot, "translation-memory");
        Directory.CreateDirectory(directory);
        string WriteProject(string relativePath, (string Original, string Speaker, string First, string Second)[] rows)
        {
            var path = Path.Combine(directory, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            var project = new
            {
                Version = 1,
                Entries = rows.Select((row, index) => new TranslationEntry
                {
                    Id = (index + 1).ToString(), Original = row.Original, Speaker = row.Speaker, Translation = row.First
                }).ToArray(),
                Languages = rows.Select(row => new[] { row.First, row.Second }).ToArray(),
                SelectedLanguage = 0
            };
            File.WriteAllText(path, JsonSerializer.Serialize(project));
            return path;
        }

        var first = WriteProject("first.json",
        [
            ("Hello", "HERO", "Ciao", "Bonjour"),
            ("Hi <NAME>", "HERO", "Ciao <NAME>", "Salut <NAME>"),
            ("Empty translation", "HERO", "", ""),
            ("", "HERO", "Should not be indexed", "")
        ]);
        WriteProject("second.json",
        [
            ("Hello", "OTHER", "Salve", ""),
            ("Hello", "HERO", "Ciao", "Bonjour"),
            ("Hello", "HERO", "Ehilà", "")
        ]);
        var excluded = WriteProject("current.json", [("Excluded", "HERO", "Esclusa", "")]);
        WriteProject(Path.Combine("nested", "translated.json"), [("Nested", "HERO", "Inclusa", "")]);
        foreach (var folder in new[] { "bin", "obj", ".git", "Graphics", "Sound", "assets" })
            WriteProject(Path.Combine(folder, "ignored.json"), [("Ignored", "HERO", "Non indicizzare", "")]);
        File.WriteAllText(Path.Combine(directory, "broken.po"), "not a PO entry");
        File.WriteAllText(Path.Combine(directory, "previous.po"), "msgctxt \"0001 | previous | HERO\"\nmsgid \"PO source\"\nmsgstr \"Dal PO\"\n");
        File.WriteAllText(Path.Combine(directory, "broken.json"), "{");
        File.WriteAllText(Path.Combine(directory, "ignored.json.bak"), "{");
        File.WriteAllText(Path.Combine(directory, "source-only.txt"), "TXT is not translation memory");
        var originalBytes = File.ReadAllBytes(first);

        using (var cancellation = new CancellationTokenSource())
        {
            cancellation.Cancel();
            ExpectCanceled(() => TranslationMemory.Scan(directory, excluded, cancellation.Token), cancellation.Token);
            // A missing directory would throw DirectoryNotFoundException if scanning touched the filesystem first.
            ExpectCanceled(() => TranslationMemory.Scan(Path.Combine(directory, "does-not-exist"), cancellationToken: cancellation.Token), cancellation.Token);
            if (!originalBytes.AsSpan().SequenceEqual(File.ReadAllBytes(first)))
                throw new Exception("Canceled scanning changed a source file.");
        }

        var memory = TranslationMemory.Scan(directory, excluded);
        Equal(2, memory.Warnings.Count, "Invalid translation files should be reported, not abort scanning.");
        Equal("Ciao|Ehilà", string.Join("|", memory.Find("Hello", "HERO").Select(suggestion => suggestion.Text)), "Same-speaker candidates should take precedence and duplicates should collapse.");
        Equal(3, memory.Find("Hello", "UNKNOWN").Count, "Conflicting translations should remain choices when the speaker is unknown.");
        Equal("Bonjour", memory.Find("Hello", "HERO", 1).Single().Text, "Languages must be indexed independently.");
        Equal(0, memory.Find("hello", "HERO").Count, "Original matching must be ordinal and exact.");
        Equal("Ciao <NAME>", memory.Find("Hi <NAME>", "HERO").Single().Text, "Raw game tokens must remain unchanged.");
        Equal("Dal PO", memory.Find("PO source", "HERO").Single().Text, "Translations from supported source formats must be indexed alongside projects.");
        Equal(0, memory.Find("Excluded").Count, "Current document must not suggest its own translations.");
        Equal("Inclusa", memory.Find("Nested").Single().Text, "Legacy recursive scanning must include subdirectories.");
        Equal(0, memory.Find("Ignored").Count, "Build, repository and media directories must be skipped.");
        Equal(0, memory.Find("Empty translation").Count, "Untranslated entries must not become suggestions.");
        Equal(0, memory.Find("").Count, "Header or empty originals must not become suggestions.");
        if (!originalBytes.AsSpan().SequenceEqual(File.ReadAllBytes(first))) throw new Exception("Scanning changed a source file.");
        if (memory.Find("Hello", "HERO").Any(suggestion => !Path.IsPathRooted(suggestion.SourceFile)))
            throw new Exception("Suggestion sources must be identifiable absolute paths.");

        using (var cancellation = new CancellationTokenSource())
        {
            var uncanceled = TranslationMemory.Scan(directory, excluded, cancellation.Token);
            Equal(memory.Warnings.Count, uncanceled.Warnings.Count, "An uncanceled token should preserve scan results.");
            Equal("Ciao|Ehilà", string.Join("|", uncanceled.Find("Hello", "HERO").Select(suggestion => suggestion.Text)), "An uncanceled scan should build the complete index.");
        }

        static void ExpectCanceled(Action action, CancellationToken expectedToken)
        {
            try { action(); }
            catch (OperationCanceledException exception)
            {
                Equal(expectedToken, exception.CancellationToken, "Cancellation must retain the caller's token.");
                return;
            }
            throw new Exception("Canceled scanning must propagate OperationCanceledException instead of returning warnings or a partial index.");
        }

        static void Equal<T>(T expected, T actual, string message)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
                throw new Exception($"{message} Expected [{expected}], actual [{actual}].");
        }
    }
}
