namespace DGRV3TS.Core;

/// <summary>A v25 dialogue row; raw game tokens remain in the text, never replaced on disk.</summary>
public sealed class TranslationEntry
{
    public string Id { get; set; } = "";
    public string Context { get; set; } = "";
    public string Origin { get; set; } = "";
    public string Speaker { get; set; } = "";
    public string Expression { get; set; } = "";
    public string Voice { get; set; } = "";
    public string Original { get; set; } = "";
    public string Translation { get; set; } = "";
    public bool HasTranslation { get; set; }

    /// <summary>The final text, preserving an intentional blank instead of falling back to the original.</summary>
    public string DisplayText(bool original = false) => original || !HasTranslation && Translation.Length == 0 ? Original : Translation;
}
