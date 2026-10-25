using System.Buffers.Binary;
using System.Text;

namespace DGRV3TS.Core;

/// <summary>
/// Read-only v25 WRD metadata. Header integers are little endian; opcode operands are big endian.
/// The parser follows the known opcode sizes instead of scanning inside operands for 0x70.
/// </summary>
public sealed class WrdMetadata
{
    private sealed record Snapshot(string Speaker, string Expression, string Voice);
    private readonly SortedList<int, Snapshot> lines = new();

    public static WrdMetadata Load(string path)
    {
        var data = File.ReadAllBytes(path);
        if (data.Length < 32) throw new InvalidDataException("Intestazione WRD incompleta.");
        var parameterCount = BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(4, 2));
        uint Pointer(int offset) => BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset, 4));
        var opcodeEnd = Pointer(16);
        var parameterStart = Pointer(24);
        if (opcodeEnd < 32 || opcodeEnd > data.Length) throw new InvalidDataException("Limite opcode WRD non valido.");
        foreach (var offset in new[] { 12, 16, 20, 24, 28 })
        {
            var pointer = Pointer(offset);
            if (pointer != 0 && (pointer < 32 || pointer > data.Length)) throw new InvalidDataException("Puntatore WRD fuori dai limiti.");
        }
        if (parameterCount > 0 && (parameterStart < opcodeEnd || parameterStart >= data.Length))
            throw new InvalidDataException("Tabella parametri WRD non valida.");
        var parameters = new string[parameterCount];
        var parameterPosition = checked((int)parameterStart);
        var parameterEnd = data.Length;
        foreach (var offset in new[] { 12, 16, 20, 28 })
        {
            var pointer = Pointer(offset);
            if (pointer > parameterStart) parameterEnd = Math.Min(parameterEnd, checked((int)pointer));
        }
        for (var i = 0; i < parameterCount; i++)
        {
            if (parameterPosition >= parameterEnd) throw new InvalidDataException("Parametri WRD incompleti.");
            var length = data[parameterPosition++];
            if (length + 1 > parameterEnd - parameterPosition) throw new InvalidDataException("Parametro WRD troncato.");
            try { parameters[i] = new UTF8Encoding(false, true).GetString(data, parameterPosition, length); }
            catch (DecoderFallbackException exception) { throw new InvalidDataException("Parametro WRD non UTF-8.", exception); }
            parameterPosition += length;
            if (data[parameterPosition++] != 0) throw new InvalidDataException("Parametro WRD senza terminatore.");
        }
        string Parameter(int index)
        {
            if (index < 0 || index >= parameters.Length) throw new InvalidDataException("Indice parametro WRD fuori dai limiti.");
            return parameters[index];
        }

        var result = new WrdMetadata();
        var speaker = 0;
        int? voice = null;
        var animations = new Dictionary<int, string>();
        for (var position = 32; position < opcodeEnd;)
        {
            // WRD command sections can contain NUL alignment padding.
            if (data[position] == 0) { position++; continue; }
            if (data[position++] != 0x70) throw new InvalidDataException("Prefisso opcode WRD non valido.");
            if (position >= opcodeEnd) throw new InvalidDataException("Opcode WRD incompleto.");
            var opcode = data[position++];
            var size = OperandSize(opcode);
            if (size < 0) throw new NotSupportedException($"Opcode WRD 0x{opcode:X2} non supportato; lettura interrotta per evitare metadati errati.");
            if (size > opcodeEnd - position) throw new InvalidDataException("Operandi WRD troncati.");
            int Operand(int offset) => BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(position + offset, 2));
            switch (opcode)
            {
                case 0x0e: // KNM: character and animation are operands 2 and 3.
                    var animatedCharacter = Operand(4);
                    Parameter(animatedCharacter);
                    animations[animatedCharacter] = Parameter(Operand(6));
                    break;
                case 0x19: // VOI: a voice is associated with the next LOC only.
                    voice = Operand(0);
                    Parameter(voice.Value);
                    break;
                case 0x1d: // CHN: active speaker.
                    speaker = Operand(0);
                    Parameter(speaker);
                    break;
                case 0x22: // CHR: flags, character, initial animation.
                    speaker = Operand(2);
                    Parameter(speaker);
                    animations[speaker] = Parameter(Operand(4));
                    break;
                case 0x46: // LOC: string ID.
                    var id = Operand(0);
                    result.lines[id] = new Snapshot(Parameter(speaker), animations.GetValueOrDefault(speaker, ""),
                        voice.HasValue ? Parameter(voice.Value) : "");
                    voice = null;
                    break;
            }
            position += size;
        }
        return result;
    }

    /// <summary>Speaker/expression follow the last LOC at or before this ID; voice requires an exact ID.</summary>
    public (string Speaker, string Expression, string Voice) ForLine(int lineId)
    {
        var low = 0;
        var high = lines.Count - 1;
        var selected = -1;
        while (low <= high)
        {
            var middle = low + (high - low) / 2;
            if (lines.Keys[middle] <= lineId) { selected = middle; low = middle + 1; }
            else high = middle - 1;
        }
        if (selected < 0) return ("", "", "");
        var snapshot = lines.Values[selected];
        return (snapshot.Speaker, snapshot.Expression, lines.Keys[selected] == lineId ? snapshot.Voice : "");
    }

    private static int OperandSize(byte opcode) => opcode switch
    {
        0x00 => 4,
        0x01 or 0x02 or 0x03 => 6,
        0x04 or 0x05 => 2,
        0x06 => 6,
        0x07 => 0,
        0x08 => 8,
        0x09 or 0x0a => 2,
        0x0b => 4,
        0x0c or 0x0d => 0,
        0x0e => 10,
        0x0f => 0,
        0x10 => 4,
        0x11 => 0,
        0x12 => 4,
        0x13 => 0,
        0x14 or 0x15 => 2,
        0x16 => 4,
        0x17 => 8,
        0x18 => 12,
        0x19 => 4,
        0x1a => 6,
        0x1b or 0x1c => 4,
        0x1d => 2,
        0x1e or 0x1f => 6,
        0x20 => 0,
        0x21 or 0x22 => 6,
        0x23 => 8,
        0x24 => 4,
        0x25 => 10,
        0x26 => 0,
        0x27 or 0x28 => 6,
        0x29 => 16,
        0x2a => 14,
        0x2b => 10,
        0x2c => 2,
        0x2d => 6,
        0x2e => 4,
        0x2f => 8,
        0x30 or 0x31 => 0,
        0x32 or 0x33 => 10,
        0x34 => 4,
        0x35 => 10,
        0x36 => 0,
        0x37 => 2,
        0x38 => 20,
        0x39 => 8,
        0x3a or 0x3b or 0x3c => 2,
        0x3d => 6,
        0x3e => 10,
        0x3f => 8,
        0x40 => 20,
        0x41 => 6,
        0x42 => 18,
        0x43 => 2,
        0x44 or 0x45 => 0,
        0x46 => 2,
        0x47 or 0x48 or 0x49 => 0,
        0x4a or 0x4b => 2,
        _ => -1
    };
}
