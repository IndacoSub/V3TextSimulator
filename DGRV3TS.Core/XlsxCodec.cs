using System.IO.Compression;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace DGRV3TS.Core;

public sealed partial class TranslationDocument
{
    private static readonly XNamespace Spreadsheet = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
    private byte[]? workbookBytes;
    private string worksheetPath = "xl/worksheets/sheet1.xml";
    private readonly List<int> worksheetRows = [];
    private readonly List<List<string>> originalCells = [];

    private static XDocument ReadXml(ZipArchive archive, string path)
    {
        var entry = archive.GetEntry(path) ?? throw new InvalidDataException("Parte XLSX mancante: " + path);
        if (entry.Length > 64 * 1024 * 1024) throw new InvalidDataException("Parte XLSX troppo grande.");
        using var stream = entry.Open();
        using var reader = XmlReader.Create(stream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null });
        return XDocument.Load(reader, LoadOptions.PreserveWhitespace);
    }

    private void ReadXlsx(byte[] bytes)
    {
        workbookBytes = bytes;
        using var stream = new MemoryStream(bytes); using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
        var workbook = ReadXml(archive, "xl/workbook.xml");
        var sheet = workbook.Descendants(Spreadsheet + "sheet").FirstOrDefault() ?? throw new InvalidDataException("Cartella XLSX senza fogli.");
        XNamespace relationships = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
        var id = (string?)sheet.Attribute(relationships + "id");
        var relation = ReadXml(archive, "xl/_rels/workbook.xml.rels").Root?.Elements().FirstOrDefault(e => (string?)e.Attribute("Id") == id)
            ?? throw new InvalidDataException("Relazione foglio XLSX mancante.");
        if ((string?)relation.Attribute("TargetMode") == "External") throw new NotSupportedException("Foglio XLSX esterno non supportato.");
        var target = (string?)relation.Attribute("Target") ?? throw new InvalidDataException("Destinazione foglio mancante.");
        worksheetPath = target.StartsWith('/') ? target.TrimStart('/') : "xl/" + target;
        if (worksheetPath.Contains("..", StringComparison.Ordinal)) throw new NotSupportedException("Percorso XLSX relativo non supportato.");
        var shared = archive.GetEntry("xl/sharedStrings.xml") is null ? [] : ReadXml(archive, "xl/sharedStrings.xml")
            .Descendants(Spreadsheet + "si").Select(s => string.Concat(s.Descendants(Spreadsheet + "t").Select(t => t.Value))).ToList();
        var worksheet = ReadXml(archive, worksheetPath);
        var rows = new List<(int Number, Dictionary<int, string> Values)>();
        var maximum = 3;
        foreach (var row in worksheet.Descendants(Spreadsheet + "row"))
        {
            if (!int.TryParse((string?)row.Attribute("r"), out var number) || number < 1 || number > 1048576) throw new InvalidDataException("Numero riga XLSX non valido.");
            var values = new Dictionary<int, string>();
            var hasFormula = false;
            foreach (var cell in row.Elements(Spreadsheet + "c"))
            {
                var reference = (string?)cell.Attribute("r") ?? throw new InvalidDataException("Riferimento cella mancante.");
                var column = ColumnNumber(reference); var value = cell.Element(Spreadsheet + "v")?.Value ?? "";
                var type = (string?)cell.Attribute("t");
                if (type == "s")
                {
                    if (!int.TryParse(value, out var index) || index < 0 || index >= shared.Count) throw new InvalidDataException("Indice stringa XLSX non valido.");
                    value = shared[index];
                }
                else if (type == "inlineStr") value = string.Concat(cell.Descendants(Spreadsheet + "t").Select(t => t.Value));
                values[column] = value;
                var formula = cell.Element(Spreadsheet + "f") is not null;
                hasFormula |= formula;
                // A named language exists even when every data cell in that column is empty.
                // Formatting-only cells (often extending to XFD) are not translation languages.
                if (value.Length > 0 || formula) maximum = Math.Max(maximum, column);
            }
            if (number == 1 || values.Count == 0 || values.Values.All(string.IsNullOrEmpty) && !hasFormula) continue;
            rows.Add((number, values));
        }
        // Use the entire sheet's language extent for every row, independent of sparse XML order.
        minimumLanguageCount = Math.Max(minimumLanguageCount, maximum - 2);
        foreach (var (number, values) in rows)
        {
            var translations = Enumerable.Range(3, maximum - 2).Select(c => values.GetValueOrDefault(c, "")).ToList();
            Add(new TranslationEntry { Id = values.GetValueOrDefault(1, number.ToString()), Original = values.GetValueOrDefault(2, ""), Translation = translations[0] }, translations);
            worksheetRows.Add(number); originalCells.Add(translations.ToList());
        }
    }

    private static int ColumnNumber(string reference)
    {
        var column = 0;
        foreach (var c in reference)
        {
            if (c is >= 'A' and <= 'Z') column = checked(column * 26 + c - 'A' + 1); else break;
        }
        if (column is < 1 or > 16384) throw new InvalidDataException("Colonna XLSX non valida.");
        return column;
    }

    private static string ColumnName(int column)
    {
        var name = "";
        while (column > 0) { column--; name = (char)('A' + column % 26) + name; column /= 26; }
        return name;
    }

    private static void SetCell(XElement row, int column, int number, string value)
    {
        var reference = ColumnName(column) + number;
        var cell = row.Elements(Spreadsheet + "c").FirstOrDefault(c => (string?)c.Attribute("r") == reference);
        if (cell?.Element(Spreadsheet + "f") is not null) throw new NotSupportedException("La traduzione " + reference + " contiene una formula: salva un progetto JSON.");
        if (cell is null)
        {
            cell = new XElement(Spreadsheet + "c", new XAttribute("r", reference));
            var next = row.Elements(Spreadsheet + "c").FirstOrDefault(c => ColumnNumber((string)c.Attribute("r")!) > column);
            if (next is null) row.Add(cell); else next.AddBeforeSelf(cell);
        }
        cell.SetAttributeValue("t", "inlineStr");
        cell.Elements().Where(e => e.Name == Spreadsheet + "v" || e.Name == Spreadsheet + "is").Remove();
        cell.Add(new XElement(Spreadsheet + "is", new XElement(Spreadsheet + "t", new XAttribute(XNamespace.Xml + "space", "preserve"), value)));
    }

    private byte[] WriteXlsx()
    {
        using var memory = new MemoryStream();
        if (workbookBytes is not null) memory.Write(workbookBytes);
        using (var archive = new ZipArchive(memory, workbookBytes is null ? ZipArchiveMode.Create : ZipArchiveMode.Update, true))
        {
            XDocument worksheet;
            if (workbookBytes is not null) worksheet = ReadXml(archive, worksheetPath);
            else
            {
                ZipText(archive, "[Content_Types].xml", "<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\"><Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/><Default Extension=\"xml\" ContentType=\"application/xml\"/><Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/><Override PartName=\"/xl/worksheets/sheet1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/></Types>");
                ZipText(archive, "_rels/.rels", "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"xl/workbook.xml\"/></Relationships>");
                ZipText(archive, "xl/workbook.xml", "<workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\"><sheets><sheet name=\"Hoja 1\" sheetId=\"1\" r:id=\"rId1\"/></sheets></workbook>");
                ZipText(archive, "xl/_rels/workbook.xml.rels", "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet1.xml\"/></Relationships>");
                worksheet = new XDocument(new XElement(Spreadsheet + "worksheet", new XElement(Spreadsheet + "sheetData")));
                var header = new XElement(Spreadsheet + "row", new XAttribute("r", 1));
                SetCell(header, 1, 1, "OFFSET"); SetCell(header, 2, 1, "ORIGINAL"); SetCell(header, 3, 1, "TRADUCCIÓN");
                for (var language = 1; language < LanguageCount; language++) SetCell(header, language + 3, 1, "TRANSLATION " + (language + 1));
                worksheet.Root!.Element(Spreadsheet + "sheetData")!.Add(header);
            }
            worksheet.Root!.Element(Spreadsheet + "dimension")?.Remove();
            var data = worksheet.Root.Element(Spreadsheet + "sheetData") ?? throw new InvalidDataException("Dati foglio mancanti.");
            var lastRow = data.Elements(Spreadsheet + "row").Select(r => (int?)r.Attribute("r") ?? 0).DefaultIfEmpty(1).Max();
            for (var i = 0; i < Entries.Count; i++)
            {
                var number = i < worksheetRows.Count ? worksheetRows[i] : lastRow + i - worksheetRows.Count + 1;
                if (number > 1048576) throw new InvalidDataException("Superato il limite di righe XLSX.");
                var row = data.Elements(Spreadsheet + "row").FirstOrDefault(r => (int?)r.Attribute("r") == number);
                if (row is null) { row = new XElement(Spreadsheet + "row", new XAttribute("r", number)); data.Add(row); }
                if (workbookBytes is null || i >= worksheetRows.Count) { SetCell(row, 1, number, Entries[i].Id); SetCell(row, 2, number, Entries[i].Original); }
                for (var language = 0; language < languages[i].Count; language++)
                {
                    if (i < originalCells.Count && language < originalCells[i].Count && languages[i][language] == originalCells[i][language]) continue;
                    SetCell(row, language + 3, number, languages[i][language]);
                }
            }
            if (workbookBytes is not null) archive.GetEntry(worksheetPath)?.Delete();
            using var stream = archive.CreateEntry(worksheetPath).Open(); worksheet.Save(stream);
        }
        return memory.ToArray();
    }

    private static void ZipText(ZipArchive archive, string path, string text)
    {
        using var stream = archive.CreateEntry(path).Open(); using var writer = new StreamWriter(stream, new UTF8Encoding(false)); writer.Write(text);
    }
}
