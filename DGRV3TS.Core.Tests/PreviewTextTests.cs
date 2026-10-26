using DGRV3TS.Core;

internal static class PreviewTextTests
{
    public static void Run()
    {
        var raw = "<CLT=cltSTRONG>Ciao\\nmondo<SIGNAL_AutoTL>original untranslated tail";
        var content = PreviewTextRules.Prepare(raw, "DR", "default_background");
        Equal("<CLT=cltSTRONG>Ciao\nmondo", content.Text);
        Equal(true, content.AutoTranslated);
        Equal("<CLT=cltSTRONG>Ciao\\nmondo<SIGNAL_AutoTL>original untranslated tail", raw);
        Equal("Messaggio\nseconda riga", PreviewTextRules.Prepare("<speaker>Kaede<message>Messaggio\\nseconda riga", "AI", "ToWitter.png").Text);
        Equal("plain message", PreviewTextRules.Prepare("plain message", "AI", "ToWitter").Text);
        Equal("no hidden suffix", PreviewTextRules.Prepare("<name>no hidden suffix<SIGNAL_AutoTL>metadata", "AI", "ToWitter").Text);
        Equal("Ciao mondo", PreviewTextRules.Speech("<CLT=cltMIND>Ciao\\nmondo<SIGNAL_NDR>do not read<SIGNAL_ALT>other platform"));
        Equal("First", PreviewTextRules.Speech("First<SIGNAL_ALT>second<SIGNAL_NDR>third"));
        Equal("Ciao", PreviewTextRules.Speech("Ciao<SIGNAL_AutoTL>source"));
        Equal("VAR_NAME", PreviewTextRules.Speech("VAR_NAME"));
        Equal("\\n", PreviewTextRules.Prepare("\\\\n", "DR", "").Text);
        Equal(29.7f, PreviewTextRules.FontSize("<CLT=size1.2>", 29));
        Equal(31.2f, PreviewTextRules.FontSize("<CLT=size2.2>", 29));
        Equal(35f, PreviewTextRules.FontSize("<CLT=size6>", 29));
        Equal(29f, PreviewTextRules.FontSize("<CLT=unknown>", 29));
    }

    private static void Equal<T>(T expected, T actual)
    { if (!EqualityComparer<T>.Default.Equals(expected, actual)) throw new Exception($"Expected [{expected}], actual [{actual}]"); }
}
