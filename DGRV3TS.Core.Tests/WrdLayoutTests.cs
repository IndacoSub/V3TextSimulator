using System.Buffers.Binary;
using System.Text;
using DGRV3TS.Core;

internal static class WrdLayoutTests
{
    public static void Run(string root)
    {
        byte[] Op(byte code, params ushort[] operands)
        {
            var bytes = new byte[2 + operands.Length * 2];
            bytes[0] = 0x70; bytes[1] = code;
            for (var i = 0; i < operands.Length; i++) BinaryPrimitives.WriteUInt16BigEndian(bytes.AsSpan(2 + i * 2), operands[i]);
            return bytes;
        }

        byte[] Build(byte[] commands, bool withLabelTables)
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            var end = 32 + commands.Length;
            writer.Write((ushort)10); writer.Write((ushort)(withLabelTables ? 1 : 0));
            writer.Write((ushort)6); writer.Write((ushort)(withLabelTables ? 2 : 0)); writer.Write(0u);
            writer.Write(withLabelTables ? (uint)end : 0u);
            writer.Write((uint)(end + (withLabelTables ? 8 : 0)));
            writer.Write(withLabelTables ? (uint)(end + 10) : 0u);
            writer.Write((uint)(end + (withLabelTables ? 16 : 0))); writer.Write(0u);
            writer.Write(commands);
            if (withLabelTables)
            {
                // These are table bytes, including an opcode-looking byte, never commands.
                writer.Write(new byte[] { 0x70, 0xff, 0, 0, 0, 0, 0, 0 });
                writer.Write((ushort)0);
                writer.Write(new byte[] { 4, (byte)'d', (byte)'e', (byte)'m', (byte)'o', 0 });
            }
            foreach (var parameter in new[] { "UNRELATED", "ACTOR_DEMO", "anim_demo", "vic_demo", "unused", "anim_next" })
            {
                var bytes = Encoding.UTF8.GetBytes(parameter);
                writer.Write((byte)bytes.Length); writer.Write(bytes); writer.Write((byte)0);
            }
            return stream.ToArray();
        }

        var commands = new[]
        {
            Op(0x46, 3), // No character has been selected yet.
            Op(0x22, 0, 1, 2, 4, 4), // CHR has five operands, not three.
            Op(0x00, 0x7000, 0x0070), // Opcode-looking bytes inside operands are not delimiters.
            Op(0x19, 3, 0), Op(0x46, 7),
            Op(0x0e, 0, 0, 1, 5, 0), Op(0x46, 9), Op(0x11)
        }.SelectMany(x => x).ToArray();
        var path = Path.Combine(root, "invented-wrd-layout.wrd");
        foreach (var withLabelTables in new[] { false, true })
        {
            File.WriteAllBytes(path, Build(commands, withLabelTables));
            var metadata = WrdMetadata.Load(path);
            Equal(("", "", ""), metadata.ForLine(3));
            Equal(("ACTOR_DEMO", "anim_demo", "vic_demo"), metadata.ForLine(7));
            Equal(("ACTOR_DEMO", "anim_next", ""), metadata.ForLine(9));
            Equal(("", "", ""), metadata.ForLine(8));
            Equal(("", "", ""), metadata.ForLine(10));

            var document = TranslationDocument.Create();
            document.AddEntry("Invented source"); document.Entries[0].Id = "7";
            document.AddEntry("Unused invented source"); document.Entries[1].Id = "8";
            document.ApplyWrdMetadata(metadata);
            var project = Path.Combine(root, "invented-wrd-layout.json");
            document.Save(project);
            var restored = TranslationDocument.Load(project);
            var po = Path.Combine(root, "invented-wrd-layout.po");
            restored.Save(po);
            var entries = TranslationDocument.Load(po).Entries;
            Equal(("ACTOR_DEMO", "anim_demo", "vic_demo"), (entries[0].Speaker, entries[0].Expression, entries[0].Voice));
            Equal(("", "", ""), (entries[1].Speaker, entries[1].Expression, entries[1].Voice));
        }

        File.WriteAllBytes(path, Build(Op(0x22, 0, 1, 2), withLabelTables: true));
        Reject(() => WrdMetadata.Load(path));
        var invalidPointer = Build(commands, withLabelTables: true);
        BinaryPrimitives.WriteUInt32LittleEndian(invalidPointer.AsSpan(12), (uint)invalidPointer.Length + 1);
        File.WriteAllBytes(path, invalidPointer);
        Reject(() => WrdMetadata.Load(path));
    }

    private static void Reject(Action action)
    {
        try { action(); } catch (InvalidDataException) { return; }
        throw new Exception("Expected invalid WRD data to be rejected.");
    }
    private static void Equal<T>(T expected, T actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual)) throw new Exception($"Expected [{expected}], actual [{actual}].");
    }
}
