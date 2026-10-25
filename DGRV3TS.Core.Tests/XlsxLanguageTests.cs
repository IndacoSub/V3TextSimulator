using System.IO.Compression;
using System.Xml.Linq;
using DGRV3TS.Core;

internal static class XlsxLanguageTests
{
    private static readonly XNamespace Ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

    public static void Run(string root)
    {
        var directory = Path.Combine(root, "xlsx-language-extent");
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, "languages.xlsx");
        var source = TranslationDocument.Create();
        source.AddEntry("First original"); source.UpdateTranslation(source.Entries[0], "Ciao");
        source.AddEntry("Second original"); source.UpdateTranslation(source.Entries[1], "Seconda");
        source.Save(path);

        var originalHeader = "";
        using (var archive = ZipFile.Open(path, ZipArchiveMode.Update))
        {
            var old = archive.GetEntry("xl/worksheets/sheet1.xml")!;
            XDocument xml;
            using (var stream = old.Open()) xml = XDocument.Load(stream);
            var data = xml.Root!.Element(Ns + "sheetData")!;
            var header = data.Elements(Ns + "row").Single(r => (int?)r.Attribute("r") == 1);
            header.Add(Cell("D1", "French"));
            // A large formatted extent must not create 16,382 fake language choices.
            header.Add(new XElement(Ns + "c", new XAttribute("r", "XFD1"), new XAttribute("s", "1")));
            foreach (var row in data.Elements(Ns + "row").Where(r => (int?)r.Attribute("r") != 1))
                row.Add(new XElement(Ns + "c", new XAttribute("r", "XFD" + (string)row.Attribute("r")!), new XAttribute("s", "1")));
            // Deliberately put cells and rows out of order. Language detection must follow references.
            var first = data.Elements(Ns + "row").Single(r => (int?)r.Attribute("r") == 2);
            first.ReplaceNodes(first.Elements().Reverse().ToArray());
            header.Remove(); data.Add(header);
            originalHeader = header.ToString(SaveOptions.DisableFormatting);
            old.Delete();
            using var output = archive.CreateEntry("xl/worksheets/sheet1.xml").Open();
            xml.Save(output);
        }

        var doc = TranslationDocument.Load(path);
        Equal(2, doc.LanguageCount, "Header-only language must remain selectable.");
        doc.SelectLanguage(1);
        Equal("", doc.Entries[0].Translation, "Missing French cell should be empty.");
        Equal("", doc.Entries[1].Translation, "Every row must have the global language extent.");
        doc.UpdateTranslation(doc.Entries[1], "Bonjour");
        doc.Save(Path.Combine(directory, "project.json"));
        var restored = TranslationDocument.Load(Path.Combine(directory, "project.json"));
        Equal(2, restored.LanguageCount, "Native project must retain the previously blank language.");
        Equal("Bonjour", restored.Entries[1].Translation, "Selected language must survive project reload.");
        var exported = Path.Combine(directory, "export.xlsx");
        restored.Save(exported);
        using (var archive = ZipFile.OpenRead(exported))
        using (var stream = archive.GetEntry("xl/worksheets/sheet1.xml")!.Open())
        {
            var xml = XDocument.Load(stream);
            Equal(originalHeader, xml.Descendants(Ns + "row").Single(r => (int?)r.Attribute("r") == 1).ToString(SaveOptions.DisableFormatting),
                "Original headers and formatting must be preserved.");
        }
        var reimported = TranslationDocument.Load(exported);
        reimported.SelectLanguage(0); Equal("Ciao", reimported.Entries[0].Translation, "Writing French must preserve Italian.");
        reimported.SelectLanguage(1); Equal("Bonjour", reimported.Entries[1].Translation, "Written French should reimport.");

        // A populated later row may introduce a language even without a header cell.
        using (var archive = ZipFile.Open(exported, ZipArchiveMode.Update))
        {
            var old = archive.GetEntry("xl/worksheets/sheet1.xml")!;
            XDocument xml;
            using (var stream = old.Open()) xml = XDocument.Load(stream);
            var first = xml.Descendants(Ns + "row").Single(r => (int?)r.Attribute("r") == 2);
            var third = xml.Descendants(Ns + "row").Single(r => (int?)r.Attribute("r") == 3);
            third.Add(Cell("E3", "Hola"));
            // Empty string values beyond actual data must be ignored just like style-only cells.
            first.Add(Cell("XFC2", ""));
            old.Delete();
            using var output = archive.CreateEntry("xl/worksheets/sheet1.xml").Open();
            xml.Save(output);
        }
        var sparse = TranslationDocument.Load(exported);
        Equal(3, sparse.LanguageCount, "A populated later-row language must pad earlier rows.");
        sparse.SelectLanguage(2);
        Equal("", sparse.Entries[0].Translation, "Earlier sparse row should have a blank third language.");
        Equal("Hola", sparse.Entries[1].Translation, "Later sparse translation must be assigned by column reference.");
        sparse.SelectLanguage(1);
        Equal("Bonjour", sparse.Entries[1].Translation, "Sparse column padding must not shift existing languages.");

        var blankVo = Path.Combine(directory, "blank-language.vo");
        File.WriteAllText(blankVo, "Lines: 12\nNumber of comments: 0\nComments:\nFile: c00\nLine: 1\nCharacter: Kaede\nOriginal message length: 5\nOriginal message: \"Hello\"\nNumber of translations: 2\nTranslations:\n\"Ciao\"\n\"\"\n");
        var voDocument = TranslationDocument.Load(blankVo);
        Equal(2, voDocument.LanguageCount, "VO explicitly declares its blank second language.");
        var voExport = Path.Combine(directory, "vo-export.xlsx");
        voDocument.Save(voExport);
        var voRestored = TranslationDocument.Load(voExport);
        Equal(2, voRestored.LanguageCount, "New XLSX exports must name every language, including blank VO columns.");
        voRestored.SelectLanguage(1);
        Equal("", voRestored.Entries[0].Translation, "Blank language must remain selectable after VO to XLSX conversion.");
    }

    private static XElement Cell(string reference, string value) => new(Ns + "c", new XAttribute("r", reference), new XAttribute("t", "inlineStr"),
        new XElement(Ns + "is", new XElement(Ns + "t", value)));

    private static void Equal<T>(T expected, T actual, string message)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual)) throw new Exception($"{message} Expected [{expected}], actual [{actual}].");
    }
}
