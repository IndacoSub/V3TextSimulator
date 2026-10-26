using System.Buffers.Binary;
using System.Text;
using DGRV3TS.Core;
public static class V25ExtraTests
{
    public static void Run(string root)
    {
        Directory.CreateDirectory(root);
        void Equal<T>(T expected,T actual) { if (!EqualityComparer<T>.Default.Equals(expected,actual)) throw new Exception($"Expected [{expected}], actual [{actual}]"); }
        void True(bool condition) {if(!condition)throw new Exception("Condition failed");}
        void Reject<T>(Action action)where T:Exception {try{action();}catch(T){return;}throw new Exception("Expected "+typeof(T).Name);}
        var path=Path.Combine(root,"VaRs_BaK.TxT");
        File.WriteAllText(path,"// Title\n// Names\nVAR_NAME, Kaede : 楓 | Heroine\nVAR_NAME_MN, kaede\nVAR_NAME_MS, KAEDE\nVAR_NESTED, VAR_NAME!\nVAR_DUP, Kaede\nVAR_CYCLE_A, VAR_CYCLE_B\nVAR_CYCLE_B, VAR_CYCLE_A\n<SIGNAL_NDR>, ignored\n<CLT=cltMIND>, ignored\ninvalid line\n, broken\nVAR_NAME, duplicate\n");
        var catalog=VariableCatalog.Load(path);
        Equal(9,catalog.Variables.Count); Equal(5,catalog.Warnings.Count);
        Equal("Names",catalog.Variables[0].Category);Equal("Heroine",catalog.Variables[0].Comment);
        Equal("楓",catalog.Resolve("VAR_NAME",true));
        Equal("Kaede! kaede KAEDE VAR_UNKNOWN XVAR_NAME VAR_NAME2",catalog.Resolve("VAR_NESTED VAR_NAME_MN VAR_NAME_MS VAR_UNKNOWN XVAR_NAME VAR_NAME2"));
        Equal("VAR_NAMÉ",catalog.Resolve("VAR_NAMÉ"));
        Equal("<SIGNAL_NDR> <CLT=cltMIND>",catalog.Resolve("<SIGNAL_NDR> <CLT=cltMIND>"));
        True(catalog.Resolve("VAR_CYCLE_A").Contains("VAR_CYCLE_"));
        Equal(path,VariableCatalog.FindDefaultFile(root));
        File.WriteAllText(Path.Combine(root,"vars_ai.txt"),"AI_NAME, Mizuki");
        Equal(Path.Combine(root,"vars_ai.txt"),VariableCatalog.FindFile("VARS_AI.TXT",root));
        Equal(0,TranslationTools.Validate("<CLT=cltMIND>VAR_NAME {0} %02d","<CLT=cltMIND>VAR_NAME %02d {0}",new[]{"VAR_NAME"}).Count);
        Equal(2,TranslationTools.Validate("VAR_NAME VAR_NAME {0}","VAR_NAME",new[]{"VAR_NAME"}).Count);
        Equal(0,TranslationTools.Validate("a","",Array.Empty<string>()).Count);
        Equal("A\nB<unknown>\nC",TranslationTools.PreviewText("<CLT=cltMIND>A\\nB<unknown><SIGNAL_NDR>\\r\\nC"));
        Equal("\\n",TranslationTools.PreviewText("\\\\n"));
        byte[] Commands(params byte[][] commands)=>commands.SelectMany(x=>x).ToArray();
        byte[] Op(byte code,params ushort[] operands)
        {
            var result=new byte[2+2*operands.Length];result[0]=0x70;result[1]=code;
            for(var i=0;i<operands.Length;i++)BinaryPrimitives.WriteUInt16BigEndian(result.AsSpan(2+2*i,2),operands[i]);
            return result;
        }
        byte[] Wrd(byte[] commands)
        {
            using var stream=new MemoryStream();using var writer=new BinaryWriter(stream);
            writer.Write((ushort)0);writer.Write((ushort)0);writer.Write((ushort)4);writer.Write((ushort)0);writer.Write(0u);
            writer.Write(0u);writer.Write((uint)(32+commands.Length));writer.Write(0u);writer.Write((uint)(32+commands.Length));writer.Write(0u);
            writer.Write(commands);
            foreach(var value in new[]{"C015_Akama","anim001","vic_Akama_01_001","anim002"}){var encoded=Encoding.UTF8.GetBytes(value);writer.Write((byte)encoded.Length);writer.Write(encoded);writer.Write((byte)0);}
            writer.Flush();return stream.ToArray();
        }
        var wrdPath=Path.Combine(root,"sample.wrd");
        var valid=Wrd(Commands(Op(0x22,0,0,1,0,0),Op(0x19,2,100),Op(0x46,7),Op(0x0e,0,0,0,3,0),Op(0x46,9)));
        File.WriteAllBytes(wrdPath,valid);
        var metadata=WrdMetadata.Load(wrdPath);
        Equal(("C015_Akama","anim001","vic_Akama_01_001"),metadata.ForLine(7));
        Equal(("","",""),metadata.ForLine(8));
        Equal(("C015_Akama","anim002",""),metadata.ForLine(9));
        Equal(("","",""),metadata.ForLine(6));
        File.WriteAllBytes(wrdPath,Wrd(Op(0xff)));Reject<NotSupportedException>(()=>WrdMetadata.Load(wrdPath));
        File.WriteAllBytes(wrdPath,Wrd(new byte[]{0x70,0x22,0}));Reject<InvalidDataException>(()=>WrdMetadata.Load(wrdPath));
        File.WriteAllBytes(wrdPath,Wrd(Op(0x1d,99)));Reject<InvalidDataException>(()=>WrdMetadata.Load(wrdPath));
        File.WriteAllBytes(wrdPath,valid[..20]);Reject<InvalidDataException>(()=>WrdMetadata.Load(wrdPath));
        Console.WriteLine("V25 variable, translation tool and WRD checks passed");
    }
}
