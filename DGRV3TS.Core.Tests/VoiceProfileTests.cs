using DGRV3TS.Core;

internal static class VoiceProfileTests
{
    public static void Run()
    {
        Equal(new VoiceProfile("Male", "Teen"), VoiceProfile.ForSpeaker("C000_Saiha"));
        Equal(new VoiceProfile("Male", "Adult"), VoiceProfile.ForSpeaker("C002_Hoshi"));
        Equal(new VoiceProfile("Male", "Child"), VoiceProfile.ForSpeaker("C004_Gokuh"));
        Equal(new VoiceProfile("Female", "Child"), VoiceProfile.ForSpeaker("C009_Yumen"));
        Equal(new VoiceProfile("Female", "Adult"), VoiceProfile.ForSpeaker("C008_Tojo_"));
        Equal(new VoiceProfile("Female", "Teen"), VoiceProfile.ForSpeaker("C015_Akama"));
        Equal(new VoiceProfile("Male", "Adult"), VoiceProfile.ForSpeaker("C020_Monok"));
        Equal(new VoiceProfile("Female", "Teen"), VoiceProfile.ForSpeaker("C023_Mfunn"));
        Equal(new VoiceProfile("Male", "Senior"), VoiceProfile.ForSpeaker("C026_Eguis"));
        Equal(new VoiceProfile("Female", "Senior"), VoiceProfile.ForSpeaker("C037_Expin"));
        Equal(new VoiceProfile("Female", "Teen"), VoiceProfile.ForSpeaker("chara_Blank", "c00_001"));
        Equal(new VoiceProfile("Female", "Teen"), VoiceProfile.ForSpeaker("non", "c01_001"));
        Equal(new VoiceProfile("Male", "Teen"), VoiceProfile.ForSpeaker("", "c02_001"));
        Equal(new VoiceProfile("Male", "Teen"), VoiceProfile.ForSpeaker("chara_Hatena", "c00_001"));
        Equal(new VoiceProfile("Male", "Adult"), VoiceProfile.ForSpeaker("UNKNOWN", "c00_001"));
    }

    private static void Equal(VoiceProfile expected, VoiceProfile actual)
    {
        if (actual != expected) throw new Exception($"Expected {expected}, actual {actual}.");
    }
}
