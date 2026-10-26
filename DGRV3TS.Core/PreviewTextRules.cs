using System.Text;
using System.Text.RegularExpressions;

namespace DGRV3TS.Core;

public readonly record struct PreviewContent(string Text, bool AutoTranslated);

/// <summary>v25 display-only rules. These never rewrite the document's raw game text.</summary>
public static class PreviewTextRules
{
    public static PreviewContent Prepare(string text, string game, string background)
    {
        ArgumentNullException.ThrowIfNull(text);
        var marker = text.IndexOf("<SIGNAL_AutoTL>", StringComparison.Ordinal);
        var automatic = marker >= 0;
        if (automatic) text = text[..marker];
        if (game.Equals("AI", StringComparison.OrdinalIgnoreCase)
            && Path.GetFileNameWithoutExtension(background).Equals("ToWitter", StringComparison.OrdinalIgnoreCase))
        {
            var lastTag = text.LastIndexOf('>');
            if (lastTag > 0) text = text[(lastTag + 1)..];
        }
        return new PreviewContent(ExpandEscapes(text), automatic);
    }

    public static string Speech(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        var end = text.Length;
        foreach (var token in new[] { "<SIGNAL_NDR>", "<SIGNAL_ALT>", "<SIGNAL_AutoTL>" })
        {
            var index = text.IndexOf(token, StringComparison.Ordinal);
            if (index >= 0) end = Math.Min(end, index);
        }
        text = TranslationTools.PreviewText(text[..end]);
        text = Regex.Replace(text, @"<[^<>\r\n]+>", "", RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));
        return text.Replace('\n', ' ').Replace('\r', ' ');
    }

    public static float FontSize(string tag, float baseline) => tag switch
    {
        "<CLT=size1>" => baseline,
        "<CLT=size1.1>" => baseline + .5f,
        "<CLT=size1.2>" => baseline + .7f,
        "<CLT=size1.5>" => baseline + 1,
        "<CLT=size2>" => baseline + 2,
        "<CLT=size2.2>" => baseline + 2.2f,
        "<CLT=size3>" => baseline + 3,
        "<CLT=size4>" => baseline + 4,
        "<CLT=size6>" => baseline + 6,
        _ => baseline
    };

    private static string ExpandEscapes(string text)
    {
        var result = new StringBuilder(text.Length);
        for (var i = 0; i < text.Length; i++)
        {
            if (text[i] == '\\' && i + 1 < text.Length)
            {
                if (text[i + 1] == '\\') { result.Append('\\'); i++; continue; }
                if (text[i + 1] == 'n') { result.Append('\n'); i++; continue; }
                if (text[i + 1] == '"') { result.Append('"'); i++; continue; }
                if (text[i + 1] == 'r' && i + 3 < text.Length && text[i + 2] == '\\' && text[i + 3] == 'n')
                { result.Append('\n'); i += 3; continue; }
            }
            result.Append(text[i]);
        }
        return result.ToString().Replace("\r\n", "\n").Replace('\r', '\n');
    }
}
