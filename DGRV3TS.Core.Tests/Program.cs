using System.IO.Compression;
using System.Text;
using System.Xml.Linq;
using DGRV3TS.Core;

var folder = Path.Combine(Path.GetTempPath(), "v3-v25-tests-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(folder);
var passed = 0; var failed = 0;
string PathOf(string name) => Path.Combine(folder, name);
string Write(string name, string text) { var path = PathOf(name); File.WriteAllText(path, text); return path; }
void Equal<T>(T expected, T actual)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual)) throw new Exception($"Expected [{expected}], actual [{actual}]");
}
void Check(bool value, string message) { if (!value) throw new Exception(message); }
void Throws<T>(Action action) where T : Exception
{
    try { action(); } catch (T) { return; }
    throw new Exception("Expected " + typeof(T).Name);
}
void Test(string name, Action body)
{
    try { body(); passed++; Console.WriteLine("PASS " + name); }
    catch (Exception exception) { failed++; Console.WriteLine("FAIL " + name + ": " + exception); }
}
try
{
    Test("v25 sprite coordinates and DR/AI text profiles", PreviewLayoutTests.Run);
    Test("V3 canonical font size, real weight names and optional external font discovery", () => PreviewFontTests.Run(folder));
    Test("Readable sprite framing, transparent margins and protected dialogue area", ReadableSpriteTests.Run);
    Test("Sprite lookup, placeholders, portrait fallback and Monotaro aliases", () => SpriteAssetTests.Run(folder));
    Test("v25 signal tails, AutoTL preview and spoken text", PreviewTextTests.Run);
    Test("v25 character and narrator voice preferences", VoiceProfileTests.Run);
    Test("XLSX blank language columns and sparse worksheet preservation", () => XlsxLanguageTests.Run(folder));
    Test("Legacy line endings, native-project validation and language migration", () => DocumentCompatibilityTests.Run(folder));
    Test("PO metadata edits and WRD imports survive PO/JSON export", () => PoMetadataTests.Run(folder));
    Test("PO optional metadata stays absent across translation edits and PO/JSON exports", () => PoOptionalMetadataTests.Run(folder));
    Test("WRD five-operand CHR, label-table boundaries and exact LOC metadata", () => WrdLayoutTests.Run(folder));
    Test("v25 automatic translation reuse and marker safety", () => TranslationReuseTests.Run(folder));
    Test("AutoTL donor filtering, speaker fallback and whole-token case variants", () => AutoTlRegressionTests.Run(folder));
    Test("Recursive variables, nested MAKE calls, alternate values and expansion safety", () => VariableExpansionTests.Run(folder));
    Test("v25 PO context: arbitrary line IDs, origin, speaker, expression and voice at EOF", () =>
    {
        var path = Write("source.po", "msgid \"\"\nmsgstr \"Content-Type: text/plain; charset=UTF-8\\n\"\n\n#. context note\n#: chapter:9\nmsgctxt \"107 | c00_001 | C000_Saiha | anim005 | vic_Saiha_001\"\nmsgid \"\"\n\"Hello \\\"world\\\"\\n\"\n\"<NAME> \\\\ end\"\nmsgstr \"Ciao\"\n\nmsgctxt \"55555 | AI_intro | AI_Aiba | vic_Aiba_001\"\nmsgid \"Second\"\nmsgstr \"\"");
        var doc = TranslationDocument.Load(path);
        Equal(2, doc.Entries.Count); Equal("107", doc.Entries[0].Id); Equal("c00_001", doc.Entries[0].Origin);
        Equal("C000_Saiha", doc.Entries[0].Speaker); Equal("anim005", doc.Entries[0].Expression); Equal("vic_Saiha_001", doc.Entries[0].Voice);
        Equal("55555", doc.Entries[1].Id); Equal("", doc.Entries[1].Expression); Equal("vic_Aiba_001", doc.Entries[1].Voice);
        Equal("Hello \"world\"\n<NAME> \\ end", doc.Entries[0].Original);
        doc.UpdateTranslation(doc.Entries[0], "Ciao \"mondo\"\n<NAME> \\ fine");
        doc.Save(PathOf("roundtrip.po"));
        var read = TranslationDocument.Load(PathOf("roundtrip.po"));
        Equal(doc.Entries[0].Translation, read.Entries[0].Translation); Equal(doc.Entries[0].Context, read.Entries[0].Context);
        var output = File.ReadAllText(PathOf("roundtrip.po"));
        Check(output.Contains("#. context note") && output.Contains("#: chapter:9") && output.Contains("Content-Type:"), "PO metadata lost");
    });
    Test("Native project preserves originals, extras and source metadata before re-export", () =>
    {
        var doc = TranslationDocument.Load(PathOf("source.po"));
        doc.UpdateTranslation(doc.Entries[0], "Tradotto"); doc.Save(PathOf("project.json"));
        Equal(false, doc.IsDirty); Equal("Progetto traduzione", doc.FormatName);
        var read = TranslationDocument.Load(PathOf("project.json"));
        Equal("anim005", read.Entries[0].Expression); Equal("vic_Saiha_001", read.Entries[0].Voice);
        Equal("Tradotto", read.Entries[0].Translation); Equal(doc.Entries[0].Original, read.Entries[0].Original);
        read.Save(PathOf("project-export.po"));
        Check(File.ReadAllText(PathOf("project-export.po")).Contains("#. context note"), "PO template lost through JSON");
    });
    Test("PO unsupported plurals, duplicate fields, charset and invalid escapes reject", () =>
    {
        Throws<NotSupportedException>(() => TranslationDocument.Load(Write("plural.po", "msgid \"a\"\nmsgid_plural \"b\"\nmsgstr[0] \"c\"")));
        Throws<InvalidDataException>(() => TranslationDocument.Load(Write("duplicate.po", "msgid \"a\"\nmsgid \"b\"\nmsgstr \"c\"")));
        Throws<NotSupportedException>(() => TranslationDocument.Load(Write("escape.po", "msgid \"a\\z\"\nmsgstr \"b\"")));
        Throws<NotSupportedException>(() => TranslationDocument.Load(Write("charset.po", "msgid \"\"\nmsgstr \"Content-Type: text/plain; charset=ISO-8859-1\\n\"")));
    });
    Test("v25 TXT wrappers, blank rows and literal game tags survive unchanged", () =>
    {
        var path = Write("braces.txt", "{\r\nfirst <CLT=size2>\\nnext\r\n\r\nlast\r\n}");
        var doc = TranslationDocument.Load(path); Equal(3, doc.Entries.Count); Equal("", doc.Entries[1].Original);
        doc.Save(PathOf("txt-copy.txt")); Equal(File.ReadAllText(path), File.ReadAllText(PathOf("txt-copy.txt")));
        doc.UpdateTranslation(doc.Entries[2], "ultimo"); doc.Save(PathOf("txt-edited.txt"));
        Equal("{\r\nfirst <CLT=size2>\\nnext\r\n\r\nultimo\r\n}", File.ReadAllText(PathOf("txt-edited.txt")));
        Equal(0, TranslationDocument.Load(Write("empty.txt", "")).Entries.Count);
    });
    Test("v25 FileHelpers equivalent respects selected language and keeps originals separate", () =>
    {
        var doc = TranslationDocument.Load(PathOf("source.po"));
        Equal(2, doc.GetAllOriginalText().Count); Equal("Ciao", doc.GetAllTranslatedText()[0]);
        Equal("C000_Saiha", doc.GetAllSpeakers()[0]);
        var original = doc.Entries[0].Original; doc.UpdateTranslation(doc.Entries[0], "edited");
        Equal(original, doc.GetAllOriginalText()[0]); Equal("edited", doc.GetAllTranslatedText()[0]);
    });
    Test("v25 STX nonsequential IDs, duplicates, empty source, language header and UTF16", () =>
    {
        using (var stream = File.Create(PathOf("source.stx")))
        using (var writer = new BinaryWriter(stream))
        {
            writer.Write(0x54585453u); writer.Write(0x12345678u); writer.Write(1u); writer.Write(32u); writer.Write(8u); writer.Write(3u); writer.Write(0x1122334455667788L);
            writer.Write(42u); writer.Write(56u); writer.Write(99u); writer.Write(58u); writer.Write(100u); writer.Write(58u);
            writer.Write((ushort)0); writer.Write(Encoding.Unicode.GetBytes("日本\n<NAME>")); writer.Write((ushort)0);
        }
        var doc = TranslationDocument.Load(PathOf("source.stx"));
        Equal("", doc.Entries[0].Original); Equal("日本\n<NAME>", doc.Entries[1].Original);
        doc.UpdateTranslation(doc.Entries[1], "Italia\n<NAME>"); doc.Save(PathOf("roundtrip.stx"));
        var read = TranslationDocument.Load(PathOf("roundtrip.stx"));
        Equal("42", read.Entries[0].Id); Equal("99", read.Entries[1].Id); Equal("100", read.Entries[2].Id);
        Equal("", read.Entries[0].Original); Equal("Italia\n<NAME>", read.Entries[1].Original); Equal("日本\n<NAME>", read.Entries[2].Original);
        Equal(0x12345678u, BitConverter.ToUInt32(File.ReadAllBytes(PathOf("roundtrip.stx")), 4));
        doc.Save(PathOf("stx-project.json")); TranslationDocument.Load(PathOf("stx-project.json")).Save(PathOf("stx-json-export.stx"));
        Equal("99", TranslationDocument.Load(PathOf("stx-json-export.stx")).Entries[1].Id);
    });
    Test("STX corrupted tables, invalid pointers and missing string terminator reject", () =>
    {
        var bytes = File.ReadAllBytes(PathOf("source.stx")); File.WriteAllBytes(PathOf("short.stx"), bytes[..30]);
        Throws<InvalidDataException>(() => TranslationDocument.Load(PathOf("short.stx")));
        bytes[^1] = 1; File.WriteAllBytes(PathOf("unterminated.stx"), bytes);
        Throws<InvalidDataException>(() => TranslationDocument.Load(PathOf("unterminated.stx")));
        bytes = File.ReadAllBytes(PathOf("source.stx")); BitConverter.GetBytes(1u).CopyTo(bytes, 36); File.WriteAllBytes(PathOf("pointer.stx"), bytes);
        Throws<InvalidDataException>(() => TranslationDocument.Load(PathOf("pointer.stx")));
    });
    Test("STX to PO export keeps empty/duplicate strings through unique contexts", () =>
    {
        var doc = TranslationDocument.Load(PathOf("source.stx")); doc.Save(PathOf("stx-export.po"));
        var read = TranslationDocument.Load(PathOf("stx-export.po")); Equal(3, read.Entries.Count);
        Equal("42", read.Entries[0].Id); Equal("", read.Entries[0].Original);
        Equal("99", read.Entries[1].Id); Equal("100", read.Entries[2].Id);
    });
    Test("v25 VO comments, origin, speaker, final record and multiple translations", () =>
    {
        var path = Write("source.vo", "v25 header\n\nLines: 14\nNumber of comments: 1\nComments:\nkeep this note\nFile: c00\nLine: 777\nCharacter: Kaede\nOriginal message length: 5\nOriginal message: \"Hello\"\nNumber of translations: 2\nTranslations:\n\"Ciao\"\n\"Bonjour\"");
        var doc = TranslationDocument.Load(path); Equal(1, doc.Entries.Count); Equal(2, doc.LanguageCount);
        Equal("777", doc.Entries[0].Id); Equal("c00", doc.Entries[0].Origin); Equal("Kaede", doc.Entries[0].Speaker);
        doc.SelectLanguage(1); Equal("Bonjour", doc.Entries[0].Translation); doc.UpdateTranslation(doc.Entries[0], "Salut");
        doc.Save(PathOf("roundtrip.vo")); var read = TranslationDocument.Load(PathOf("roundtrip.vo")); Equal("Ciao", read.Entries[0].Translation);
        read.SelectLanguage(1); Equal("Salut", read.Entries[0].Translation);
        Check(File.ReadAllText(PathOf("roundtrip.vo")).Contains("keep this note"), "VO comments lost");
        doc.Save(PathOf("vo-project.json")); read = TranslationDocument.Load(PathOf("vo-project.json"));
        Equal(1, read.SelectedLanguage); Equal("Salut", read.Entries[0].Translation);
        var legacy = Write("blank-legacy.vo", File.ReadAllText(path).Replace("\"Hello\"", "\"(blank)\"").Replace("\"Ciao\"", "\"(blank)\""));
        var blank = TranslationDocument.Load(legacy); Equal("", blank.Entries[0].Original); Equal("", blank.Entries[0].Translation); Equal(false, blank.Entries[0].HasTranslation);
    });
    Test("VO quote codec roundtrips slashes, quotes, literal escapes and long originals", () =>
    {
        var doc = TranslationDocument.Create(); doc.AddEntry(@"C:\folder\new literal \n ""quotes""");
        doc.UpdateTranslation(doc.Entries[0], "Ciao\n\"mondo\" \\n"); doc.Save(PathOf("slashes.vo"));
        var read = TranslationDocument.Load(PathOf("slashes.vo")); Equal(doc.Entries[0].Original, read.Entries[0].Original); Equal(doc.Entries[0].Translation, read.Entries[0].Translation);
    });
    Test("v25 XLSX OFFSET/ORIGINAL/translation columns and unrelated zip metadata", () =>
    {
        var doc = TranslationDocument.Load(PathOf("source.vo")); doc.Save(PathOf("source.xlsx"));
        using (var archive = ZipFile.Open(PathOf("source.xlsx"), ZipArchiveMode.Update))
        using (var stream = archive.CreateEntry("custom/metadata.txt").Open())
        using (var writer = new StreamWriter(stream)) writer.Write("retain");
        var read = TranslationDocument.Load(PathOf("source.xlsx")); Equal(1, read.Entries.Count); Equal(2, read.LanguageCount);
        Equal("777", read.Entries[0].Id); Equal("Hello", read.Entries[0].Original); Equal("Ciao", read.Entries[0].Translation);
        read.SelectLanguage(1); Equal("Bonjour", read.Entries[0].Translation); read.UpdateTranslation(read.Entries[0], "日本\n<NAME>");
        read.Save(PathOf("roundtrip.xlsx")); var again = TranslationDocument.Load(PathOf("roundtrip.xlsx")); again.SelectLanguage(1); Equal("日本\n<NAME>", again.Entries[0].Translation);
        using var archive2 = ZipFile.OpenRead(PathOf("roundtrip.xlsx")); Check(archive2.GetEntry("custom/metadata.txt") is not null, "XLSX metadata lost");
    });
    Test("XLSX sparse rows and formula cells remain safe when adding a native row", () =>
    {
        File.Copy(PathOf("source.xlsx"), PathOf("sparse.xlsx")); XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        using (var zip = ZipFile.Open(PathOf("sparse.xlsx"), ZipArchiveMode.Update))
        {
            var entry = zip.GetEntry("xl/worksheets/sheet1.xml")!; XDocument sheet;
            using (var stream = entry.Open()) sheet = XDocument.Load(stream);
            var row = sheet.Descendants(ns + "row").First(r => (int?)r.Attribute("r") == 2); row.SetAttributeValue("r", 9);
            foreach (var cell in row.Elements(ns + "c")) cell.SetAttributeValue("r", ((string)cell.Attribute("r")!)[..^1] + "9");
            var formula = row.Elements(ns + "c").First(c => (string?)c.Attribute("r") == "C9"); formula.RemoveNodes(); formula.SetAttributeValue("t", "str");
            formula.Add(new XElement(ns + "f", "CONCAT(\"Ci\",\"ao\")"), new XElement(ns + "v", "Ciao"));
            sheet.Descendants(ns + "sheetData").Single().Add(new XElement(ns + "row", new XAttribute("r", 20)));
            entry.Delete(); using var output = zip.CreateEntry("xl/worksheets/sheet1.xml").Open(); sheet.Save(output);
        }
        var doc = TranslationDocument.Load(PathOf("sparse.xlsx")); doc.AddEntry("Added source"); doc.UpdateTranslation(doc.Entries[^1], "Added translation");
        doc.Save(PathOf("sparse-project.json")); var project = TranslationDocument.Load(PathOf("sparse-project.json")); project.Save(PathOf("sparse-export.xlsx"));
        var read = TranslationDocument.Load(PathOf("sparse-export.xlsx")); Equal(2, read.Entries.Count); Equal("Added source", read.Entries[1].Original); Equal("Added translation", read.Entries[1].Translation);
        using (var zip = ZipFile.OpenRead(PathOf("sparse-export.xlsx")))
        using (var stream = zip.GetEntry("xl/worksheets/sheet1.xml")!.Open())
        {
            var sheet = XDocument.Load(stream); Check(sheet.Descendants(ns + "f").Any(), "Unchanged formula removed");
            Check(sheet.Descendants(ns + "row").Any(r => (int?)r.Attribute("r") == 21), "New row collided with formatted blank row");
        }
        project.UpdateTranslation(project.Entries[0], "Change formula"); Throws<NotSupportedException>(() => project.Save(PathOf("formula-edit.xlsx")));
        Equal(false, File.Exists(PathOf("formula-edit.xlsx")));
    });
    Test("Bundled v25 document uses real contexts and external variable comments", () =>
    {
        var repository = new DirectoryInfo(AppContext.BaseDirectory);
        while (repository is not null && !File.Exists(Path.Combine(repository.FullName, "DGRV3TS.sln"))) repository = repository.Parent;
        Check(repository is not null, "Repository root missing");
        var examples = Path.Combine(repository!.FullName, "examples");
        var doc = TranslationDocument.Load(Path.Combine(examples, "dialoghi.po"));
        var variables = VariableCatalog.Load(VariableCatalog.FindDefaultFile(examples)!);
        Equal(3, doc.Entries.Count); Equal(5, variables.Variables.Count); Equal(0, variables.Warnings.Count);
        Equal("C015_Akama", doc.Entries[0].Speaker); Equal("anim000", doc.Entries[0].Expression); Equal("vic_Akama_01_001", doc.Entries[0].Voice);
        Equal("Mi chiamo Kaede Akamatsu.", variables.Resolve(doc.Entries[0].Translation));
        Equal("Nome della protagonista", variables.Variables[0].Comment);
        doc.UpdateTranslation(doc.Entries[1], "Benvenuti a <CLT=cltSTRONG><ACADEMY><CLT>.");
        doc.Save(PathOf("example.json")); var read = TranslationDocument.Load(PathOf("example.json"));
        read.Save(PathOf("example.po"));
        Equal(doc.Entries[1].Translation, TranslationDocument.Load(PathOf("example.po")).Entries[1].Translation);
        Equal("vic_Akama_01_001", TranslationDocument.Load(PathOf("example.po")).Entries[0].Voice);
    });
    Test("Translation memory scans recursive v25 files without modifying source", () => TranslationMemoryTests.Run(folder));
    Test("v25 variables, control codes and WRD metadata", () => V25ExtraTests.Run(PathOf("v25-extra")));
    Test("Intentional blank translations stay distinct from untranslated rows in every language", () =>
    {
        var doc = TranslationDocument.Load(PathOf("source.stx"));
        Equal(false, doc.Entries[1].HasTranslation);
        doc.UpdateTranslation(doc.Entries[1], ""); Equal(true, doc.Entries[1].HasTranslation);
        doc.Save(PathOf("blank-project.json")); var native = TranslationDocument.Load(PathOf("blank-project.json"));
        Equal(true, native.Entries[1].HasTranslation); Equal(false, native.Entries[2].HasTranslation);
        native.Save(PathOf("blank-export.stx")); var read = TranslationDocument.Load(PathOf("blank-export.stx"));
        Equal("", read.Entries[1].Original); Equal("日本\n<NAME>", read.Entries[2].Original);
        var multi = TranslationDocument.Load(PathOf("source.vo"));
        multi.SelectLanguage(1); multi.UpdateTranslation(multi.Entries[0], "");
        multi.SelectLanguage(0); Equal("Ciao", multi.Entries[0].Translation); Equal(true, multi.Entries[0].HasTranslation);
        multi.SelectLanguage(1); Equal("", multi.Entries[0].Translation); Equal(true, multi.Entries[0].HasTranslation);
        multi.Save(PathOf("blank-multi.json")); read = TranslationDocument.Load(PathOf("blank-multi.json"));
        Equal(1, read.SelectedLanguage); Equal("", read.Entries[0].Translation); Equal(true, read.Entries[0].HasTranslation);
    });
    Test("Editor display and text export preserve intentionally empty translations across languages", () =>
    {
        var doc = TranslationDocument.Load(PathOf("source.vo"));
        doc.UpdateTranslation(doc.Entries[0], "");
        Equal("", doc.Entries[0].DisplayText());
        Equal("Hello", doc.Entries[0].DisplayText(original: true));
        doc.Save(PathOf("editor-empty.json"));
        var restored = TranslationDocument.Load(PathOf("editor-empty.json"));
        File.WriteAllLines(PathOf("editor-voiced-export.txt"), restored.Entries.Select(e => e.DisplayText()));
        Equal("", File.ReadAllLines(PathOf("editor-voiced-export.txt"))[0]);
        restored.SelectLanguage(1);
        Equal("Bonjour", restored.Entries[0].DisplayText());
        restored.SelectLanguage(0);
        Equal("", restored.Entries[0].DisplayText());
        var untouched = TranslationDocument.Load(Write("editor-untranslated.txt", "Keep <TOKEN>"));
        Equal("Keep <TOKEN>", untouched.Entries[0].DisplayText());
        untouched.UpdateTranslation(untouched.Entries[0], "Traduzione <TOKEN>");
        Equal("Traduzione <TOKEN>", untouched.Entries[0].DisplayText());
        Equal("Keep <TOKEN>", untouched.Entries[0].DisplayText(original: true));
    });
    Test("v25 TXT conversions wrap structured sources and preserve imported TXT layout", () =>
    {
        var structured = TranslationDocument.Load(Write("legacy-wrapper.po", "msgctxt \"1\"\nmsgid \"Original\"\nmsgstr \"Traduzione\"\n\nmsgctxt \"2\"\nmsgid \"Second original\"\nmsgstr \"\""));
        structured.UpdateTranslation(structured.Entries[1], "");
        structured.Save(PathOf("legacy-wrapper.txt"));
        Equal("{\nTraduzione\n\n}", File.ReadAllText(PathOf("legacy-wrapper.txt")));
        var exported = TranslationDocument.Load(PathOf("legacy-wrapper.txt"));
        Equal(2, exported.Entries.Count);
        Equal("", exported.Entries[1].Original);

        const string unwrapped = "alpha\n\nomega\n";
        var plain = TranslationDocument.Load(Write("legacy-unwrapped.txt", unwrapped));
        plain.Save(PathOf("legacy-unwrapped.json"));
        TranslationDocument.Load(PathOf("legacy-unwrapped.json")).Save(PathOf("legacy-unwrapped-copy.txt"));
        Equal(unwrapped, File.ReadAllText(PathOf("legacy-unwrapped-copy.txt")));

        var native = TranslationDocument.Create();
        native.AddEntry("Native original");
        native.Save(PathOf("legacy-native.json"));
        TranslationDocument.Load(PathOf("legacy-native.json")).Save(PathOf("legacy-native.txt"));
        Equal("{\nNative original\n}", File.ReadAllText(PathOf("legacy-native.txt")));
    });
    Test("v25 variable case variants preserve session-only definitions and explicit overrides", () => VariableVariantTests.Run(folder));
    Test("Atomic save backup and conflict check protect external changes", () =>
    {
        var path = Write("conflict.txt", "original"); var doc = TranslationDocument.Load(path); doc.UpdateTranslation(doc.Entries[0], "first"); doc.Save(path);
        Equal("original", File.ReadAllText(path + ".bak")); doc.UpdateTranslation(doc.Entries[0], "second"); File.WriteAllText(path, "external");
        Throws<IOException>(() => doc.Save(path)); Equal("external", File.ReadAllText(path)); Equal(true, doc.IsDirty);
    });
    Test("New entries and selected language survive JSON before exporting STX/VO", () =>
    {
        foreach (var extension in new[] { "stx", "vo" })
        {
            var doc = TranslationDocument.Load(PathOf("source." + extension)); var count = doc.Entries.Count;
            if (doc.LanguageCount > 1) doc.SelectLanguage(1);
            doc.AddEntry("Added original"); doc.UpdateTranslation(doc.Entries[^1], "Added translation"); doc.Save(PathOf("added-" + extension + ".json"));
            var read = TranslationDocument.Load(PathOf("added-" + extension + ".json")); Equal(count + 1, read.Entries.Count); Equal(doc.SelectedLanguage, read.SelectedLanguage); Equal("Added translation", read.Entries[^1].Translation);
            read.Save(PathOf("added." + extension)); var exported = TranslationDocument.Load(PathOf("added." + extension)); Equal(count + 1, exported.Entries.Count);
            if (extension == "vo") exported.SelectLanguage(1);
            Equal("Added translation", extension == "stx" ? exported.Entries[^1].Original : exported.Entries[^1].Translation);
        }
    });
}
finally { Directory.Delete(folder, true); }
Console.WriteLine($"{passed} passed, {failed} failed");
Environment.ExitCode = failed == 0 ? 0 : 1;
