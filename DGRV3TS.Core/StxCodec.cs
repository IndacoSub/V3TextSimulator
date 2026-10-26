using System.Text;

namespace DGRV3TS.Core;

public sealed partial class TranslationDocument
{
    private byte[]? stxHeader;
    private readonly Dictionary<TranslationEntry, uint> stxIds = [];

    private void ReadStx(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        using var reader = new BinaryReader(stream);
        if (bytes.Length < 32 || reader.ReadUInt32() != 0x54585453) throw new InvalidDataException("Magic STX non valido.");
        stream.Position = 8;
        if (reader.ReadUInt32() != 1) throw new NotSupportedException("STX con più tabelle non supportato.");
        var headerLength = reader.ReadUInt32(); var recordSize = reader.ReadUInt32(); var count = reader.ReadUInt32();
        if (headerLength < 32 || headerLength % 2 != 0 || headerLength > bytes.Length || recordSize != 8 || count > (bytes.Length - headerLength) / 8)
            throw new InvalidDataException("Tabella STX non valida.");
        stxHeader = bytes[..(int)headerLength];
        for (var i = 0; i < count; i++)
        {
            stream.Position = headerLength + i * 8L;
            var id = reader.ReadUInt32(); var offset = reader.ReadUInt32();
            if (offset < headerLength + count * 8L || offset >= bytes.Length || offset % 2 != 0) throw new InvalidDataException("Puntatore STX fuori dai limiti.");
            stream.Position = offset;
            var data = new List<byte>(); var terminated = false;
            while (stream.Position + 2 <= bytes.Length)
            {
                var code = reader.ReadUInt16();
                if (code == 0) { terminated = true; break; }
                data.Add((byte)code); data.Add((byte)(code >> 8));
            }
            if (!terminated) throw new InvalidDataException("Stringa STX senza terminatore.");
            var entry = new TranslationEntry { Id = id.ToString(), Original = new UnicodeEncoding(false, false, true).GetString(data.ToArray()), Origin = Path.GetFileNameWithoutExtension(FilePath ?? "") };
            Add(entry); stxIds[entry] = id;
        }
    }

    private byte[] WriteStx()
    {
        using var stream = new MemoryStream(); using var writer = new BinaryWriter(stream, Encoding.Unicode, true);
        if (stxHeader is not null) writer.Write(stxHeader);
        else { writer.Write(0x54585453u); writer.Write(0x4C4C504Au); writer.Write(1u); writer.Write(32u); writer.Write(8u); writer.Write(0u); writer.Write(0L); }
        stream.Position = 20; writer.Write((uint)Entries.Count); stream.Position = stream.Length;
        var table = stream.Position;
        foreach (var _ in Entries) writer.Write(0L);
        var offsets = new Dictionary<string, uint>(StringComparer.Ordinal); var pointers = new List<uint>();
        foreach (var entry in Entries)
        {
            var value = ExportText(entry);
            if (value.Contains('\0')) throw new InvalidDataException("STX non può contenere caratteri NUL.");
            if (!offsets.TryGetValue(value, out var offset))
            {
                offset = checked((uint)stream.Position); offsets[value] = offset;
                writer.Write(new UnicodeEncoding(false, false, true).GetBytes(value)); writer.Write((ushort)0);
            }
            pointers.Add(offset);
        }
        stream.Position = table;
        for (var i = 0; i < Entries.Count; i++)
        {
            var entry = Entries[i];
            writer.Write(stxIds.TryGetValue(entry, out var id) ? id : uint.TryParse(entry.Id, out id) ? id : (uint)i);
            writer.Write(pointers[i]);
        }
        writer.Flush(); return stream.ToArray();
    }
}
