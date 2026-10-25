namespace DGRV3TS.Core;

/// <summary>Voice-selection hints from v25. They describe the old preview's choices, not canonical character ages.</summary>
public sealed record VoiceProfile(string Gender, string Age)
{
    public static VoiceProfile ForSpeaker(string speaker, string origin = "")
    {
        ArgumentNullException.ThrowIfNull(speaker);
        ArgumentNullException.ThrowIfNull(origin);
        var gender = speaker switch
        {
            "C008_Tojo_" or "C009_Yumen" or "C010_Haruk" or "C011_Chaba" or "C012_Shiro" or
            "C013_Yonag" or "C014_Iruma" or "C015_Akama" or "C023_Mfunn" or "C037_Expin" => "Female",
            "chara_Blank" or "non" or "None" or "" =>
                origin.StartsWith("c00", StringComparison.Ordinal) || origin.StartsWith("c01", StringComparison.Ordinal) ? "Female" : "Male",
            _ => "Male"
        };
        var age = speaker switch
        {
            "C004_Gokuh" or "C005_Oma__" or "C009_Yumen" or "C011_Chaba" or "C013_Yonag" => "Child",
            "C002_Hoshi" or "C006_Shing" or "C008_Tojo_" or "C020_Monok" => "Adult",
            "C026_Eguis" or "C035_Exred" or "C036_Exyel" or "C037_Expin" or "C038_Exgre" or "C039_Exblu" => "Senior",
            "C000_Saiha" or "C001_Momot" or "C003_Amami" or "C007_Ki-Bo" or "C010_Haruk" or "C012_Shiro" or
            "C014_Iruma" or "C015_Akama" or "C021_Mtaro" or "C022_Msuke" or "C023_Mfunn" or "C024_Mdam_" or
            "C025_Mkid_" or "C027_Mono5" or "chara_Blank" or "chara_Hatena" or "non" or "None" or "" => "Teen",
            _ => "Adult"
        };
        return new VoiceProfile(gender, age);
    }
}
