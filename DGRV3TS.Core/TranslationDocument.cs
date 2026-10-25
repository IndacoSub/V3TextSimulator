using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DGRV3TS.Core;

/// <summary>Translation data independent of the UI, with explicit source text and safe persistence.</summary>
public sealed partial class TranslationDocument
{
    public List<TranslationEntry> Entries { get; } = [];
    public string? FilePath { get; private set; }
    public string FormatName { get; private set; } = "Progetto traduzione";
    public bool IsDirty { get; private set; }
    public int SelectedLanguage { get; private set; }
    public int LanguageCount => Math.Max(minimumLanguageCount, languages.Count == 0 ? 1 : languages.Max(x => x.Count));
    private int minimumLanguageCount = 1;
    private readonly List<List<string>> languages = [];
    private readonly List<List<bool>> translated = [];
    private string? diskHash;
    private string? sourceExtension;
    private byte[]? sourceBytes;
    private string newline = "\n";

    public static TranslationDocument Create() => new();
    public void MarkDirty() => IsDirty = true;

    public void AddEntry(string original)
    {
        ArgumentNullException.ThrowIfNull(original);
        var id = Entries.Count + 1;
        while (Entries.Any(e => e.Id == id.ToString())) id++;
        Add(new TranslationEntry { Id = id.ToString(), Original = original });
        IsDirty = true;
    }

    public void UpdateTranslation(TranslationEntry entry, string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        var index = Entries.IndexOf(entry);
        if (index < 0) throw new ArgumentException("Battuta estranea al documento.", nameof(entry));
        if (entry.Translation == value && entry.HasTranslation) return;
        entry.Translation = value;
        entry.HasTranslation = true;
        SyncLanguages();
        IsDirty = true;
    }

    public void SelectLanguage(int index)
    {
        if (index < 0 || index >= LanguageCount) throw new ArgumentOutOfRangeException(nameof(index));
        SyncLanguages();
        SelectedLanguage = index;
        for (var i = 0; i < Entries.Count; i++)
        {
            Entries[i].Translation = languages[i].ElementAtOrDefault(index) ?? "";
            Entries[i].HasTranslation = translated[i].ElementAtOrDefault(index);
        }
    }

    public void ApplyWrdMetadata(WrdMetadata metadata)
    {
        ArgumentNullException.ThrowIfNull(metadata);
        for (var i = 0; i < Entries.Count; i++)
        {
            var entry = Entries[i];
            var lineId = int.TryParse(entry.Id, out var parsed) ? parsed : i;
            var value = metadata.ForLine(lineId);
            entry.Speaker = value.Speaker; entry.Expression = value.Expression; entry.Voice = value.Voice;
        }
        IsDirty = true;
    }

    public IReadOnlyList<string> GetAllOriginalText() => Entries.Select(e => e.Original).ToList();
    public IReadOnlyList<string> GetAllTranslatedText() => Entries.Select(e => e.Translation).ToList();
    public IReadOnlyList<string> GetAllSpeakers() => Entries.Select(e => e.Speaker).ToList();

    public static TranslationDocument Load(string path)
    {
        path = Path.GetFullPath(path);
        var bytes = File.ReadAllBytes(path);
        var extension = Path.GetExtension(path).ToLowerInvariant();
        var document = new TranslationDocument { FilePath = path, diskHash = Hash(bytes) };
        if (extension == ".json") document.ReadProject(DecodeText(bytes));
        else document.ReadSource(extension, bytes);
        return document;
    }

    private void ReadSource(string extension, byte[] bytes)
    {
        sourceExtension = extension;
        sourceBytes = bytes;
        var text = extension is ".stx" or ".xlsx" ? "" : DecodeText(bytes);
        newline = text.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : text.Contains('\r') ? "\r" : "\n";
        switch (extension)
        {
            case ".po": ReadPo(text); break;
            case ".vo": ReadVo(text); break;
            case ".txt": ReadTxt(text); break;
            case ".stx": ReadStx(bytes); break;
            case ".xlsx": ReadXlsx(bytes); break;
            default: throw new NotSupportedException("Formato non supportato: usa JSON, PO, VO, TXT, STX o XLSX.");
        }
        FormatName = extension[1..].ToUpperInvariant();
    }

    public void Save(string path)
    {
        path = Path.GetFullPath(path);
        var extension = Path.GetExtension(path).ToLowerInvariant();
        SyncLanguages();
        var bytes = extension switch
        {
            ".json" => WriteProject(), ".po" => Utf8(WritePo()), ".vo" => Utf8(WriteVo()),
            ".txt" => Utf8(WriteTxt()), ".stx" => WriteStx(), ".xlsx" => WriteXlsx(),
            _ => throw new NotSupportedException("Estensione non supportata: usa .json, .po, .vo, .txt, .stx o .xlsx.")
        };
        var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        if (FilePath is not null && string.Equals(path, FilePath, comparison)
            && (!File.Exists(path) || Hash(File.ReadAllBytes(path)) != diskHash))
            throw new IOException("Il file è stato modificato o rimosso da un altro programma. Salva con un altro nome.");
        var directory = Path.GetDirectoryName(path)!;
        Directory.CreateDirectory(directory);
        var temporary = Path.Combine(directory, "." + Path.GetFileName(path) + "." + Guid.NewGuid().ToString("N") + ".tmp");
        try
        {
            using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                stream.Write(bytes);
                stream.Flush(true);
            }
            if (File.Exists(path))
            {
                File.Copy(path, path + ".bak", true);
                File.Move(temporary, path, true);
            }
            else File.Move(temporary, path);
        }
        finally { if (File.Exists(temporary)) File.Delete(temporary); }
        FilePath = path;
        diskHash = Hash(bytes);
        FormatName = extension == ".json" ? "Progetto traduzione" : extension[1..].ToUpperInvariant();
        IsDirty = false;
    }

    private void Add(TranslationEntry entry, List<string>? translations = null)
    {
        Entries.Add(entry);
        var values = translations ?? [entry.Translation];
        languages.Add(values);
        translated.Add(values.Select(value => value.Length > 0).ToList());
        entry.HasTranslation = translated[^1].ElementAtOrDefault(SelectedLanguage);
    }

    private void SyncLanguages()
    {
        while (languages.Count < Entries.Count)
        {
            languages.Add([Entries[languages.Count].Translation]);
            translated.Add([Entries[translated.Count].HasTranslation]);
        }
        for (var i = 0; i < Entries.Count; i++)
        {
            while (languages[i].Count <= SelectedLanguage) languages[i].Add("");
            languages[i][SelectedLanguage] = Entries[i].Translation;
            while (translated[i].Count <= SelectedLanguage) translated[i].Add(false);
            translated[i][SelectedLanguage] = Entries[i].HasTranslation || Entries[i].Translation.Length > 0;
        }
    }

    private sealed class Project
    {
        public int Version { get; set; } = 1;
        public List<TranslationEntry> Entries { get; set; } = [];
        public List<List<string>> Languages { get; set; } = [];
        public int SelectedLanguage { get; set; }
        public int LanguageCount { get; set; } = 1;
        public List<List<bool>> Translated { get; set; } = [];
        public string? SourceExtension { get; set; }
        public byte[]? SourceBytes { get; set; }
    }

    private void ReadProject(string text)
    {
        using (var json = JsonDocument.Parse(text))
        {
            if (json.RootElement.ValueKind != JsonValueKind.Object
                || !json.RootElement.TryGetProperty("Version", out _)
                || !json.RootElement.TryGetProperty("Entries", out var rows)
                || rows.ValueKind != JsonValueKind.Array)
                throw new InvalidDataException("Il file JSON non è un progetto V3 Text.");
        }
        var project = JsonSerializer.Deserialize<Project>(text) ?? throw new InvalidDataException("Progetto vuoto.");
        if (project.SelectedLanguage < 0) throw new InvalidDataException("Indice lingua non valido.");
        if (project.Version != 1) throw new NotSupportedException("Versione del progetto non supportata.");
        if (project.Entries is null || project.Languages is null || project.Translated is null) throw new InvalidDataException("Struttura del progetto non valida.");
        if (project.SourceBytes is not null)
        {
            if (project.SourceExtension is null or ".json") throw new InvalidDataException("Sorgente del progetto non valida.");
            ReadSource(project.SourceExtension, project.SourceBytes);
            if (project.Entries.Count < Entries.Count) throw new InvalidDataException("Battute del file sorgente mancanti.");
            minimumLanguageCount = LanguageCount;
            languages.Clear(); translated.Clear();
        }
        minimumLanguageCount = Math.Max(minimumLanguageCount, project.LanguageCount);
        if (minimumLanguageCount is < 1 or > 16382) throw new InvalidDataException("Numero di lingue non valido.");
        for (var i = 0; i < project.Entries.Count; i++)
        {
            var saved = project.Entries[i];
            var savedHasTranslation = saved?.HasTranslation ?? false;
            if (saved is null || saved.Id is null || saved.Context is null || saved.Origin is null || saved.Speaker is null
                || saved.Expression is null || saved.Voice is null || saved.Original is null || saved.Translation is null)
                throw new InvalidDataException("Battuta del progetto non valida.");
            var translations = i < project.Languages.Count ? project.Languages[i] : [saved.Translation];
            if (translations is null || translations.Any(t => t is null)) throw new InvalidDataException("Lingue del progetto non valide.");
            while (translations.Count < minimumLanguageCount) translations.Add("");
            if (i < Entries.Count)
            {
                var target = Entries[i];
                target.Id = saved.Id; target.Context = saved.Context; target.Origin = saved.Origin;
                target.Speaker = saved.Speaker; target.Expression = saved.Expression; target.Voice = saved.Voice;
                target.Original = saved.Original; target.Translation = saved.Translation; target.HasTranslation = saved.HasTranslation;
                languages.Add(translations);
                translated.Add(translations.Select(value => value.Length > 0).ToList());
            }
            else Add(saved, translations);
            if (i < project.Translated.Count)
            {
                var flags = project.Translated[i] ?? throw new InvalidDataException("Stato traduzioni non valido.");
                if (flags.Count > translations.Count) throw new InvalidDataException("Stato lingue non valido.");
                for (var language = 0; language < flags.Count; language++) translated[i][language] = flags[language];
            }
            else if (project.SelectedLanguage < translated[i].Count)
                translated[i][project.SelectedLanguage] |= savedHasTranslation;
        }
        if (project.SelectedLanguage < 0 || project.SelectedLanguage >= LanguageCount) throw new InvalidDataException("Indice lingua non valido.");
        SelectedLanguage = project.SelectedLanguage;
        for (var i = 0; i < Entries.Count; i++)
        {
            Entries[i].Translation = languages[i].ElementAtOrDefault(SelectedLanguage) ?? "";
            Entries[i].HasTranslation = translated[i].ElementAtOrDefault(SelectedLanguage);
        }
        FormatName = "Progetto traduzione";
    }

    private byte[] WriteProject() => JsonSerializer.SerializeToUtf8Bytes(new Project
    {
        Entries = Entries, Languages = languages, Translated = translated, SelectedLanguage = SelectedLanguage,
        LanguageCount = LanguageCount,
        SourceExtension = sourceExtension, SourceBytes = sourceBytes
    }, new JsonSerializerOptions { WriteIndented = true });

    private static string Hash(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes));
    private static byte[] Utf8(string text) => new UTF8Encoding(false, true).GetBytes(text);
    private static string DecodeText(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        using var reader = new StreamReader(stream, new UTF8Encoding(false, true), true);
        return reader.ReadToEnd();
    }
}
