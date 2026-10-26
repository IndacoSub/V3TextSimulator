using System.Text;

namespace DGRV3TS.Core;

public sealed partial class VariableCatalog
{
    private sealed record Macro(VariableDefinition Definition, string Name, string Parameter);
    private readonly Dictionary<string, Macro> macros = new(StringComparer.Ordinal);

    private sealed class ResolutionState
    {
        public readonly HashSet<string> Visiting = new(StringComparer.Ordinal);
        public int RemainingExpansions = 10_000;
        public int RemainingCharacters = 1_000_000;
    }

    private static bool TryParseMacroSignature(string token, out string name, out string parameter)
    {
        name = parameter = "";
        var opening = token.IndexOf('(');
        if (!token.StartsWith("MAKE_", StringComparison.Ordinal) || opening <= 5 || !token.EndsWith(')')) return false;
        var candidateName = token[..opening];
        var candidateParameter = token[(opening + 1)..^1];
        if (!candidateName.All(IsWord) || candidateParameter.Length == 0 || !candidateParameter.All(IsWord)
            || char.IsDigit(candidateParameter[0])) return false;
        name = candidateName;
        parameter = candidateParameter;
        return true;
    }

    private bool TryMatchMacro(string text, int position, out Macro macro, out int opening)
    {
        macro = null!;
        opening = position;
        if (text[position] != 'M' || position > 0 && IsWord(text[position - 1])) return false;
        while (opening < text.Length && IsWord(text[opening])) opening++;
        return opening < text.Length && text[opening] == '(' && macros.TryGetValue(text[position..opening], out macro!);
    }

    private static int ClosingParenthesis(string text, int opening)
    {
        var balance = 1;
        for (var i = opening + 1; i < text.Length; i++)
        {
            if (text[i] == '(') balance++;
            else if (text[i] == ')' && --balance == 0) return i;
        }
        return -1;
    }

    private VariableDefinition? MatchVariable(string text, int position)
    {
        if (!candidates.TryGetValue(text[position], out var options)) return null;
        foreach (var option in options)
        {
            var end = position + option.Token.Length;
            if (end > text.Length || !text.AsSpan(position, option.Token.Length).SequenceEqual(option.Token.AsSpan())) continue;
            if (IsWord(option.Token[0]) && position > 0 && IsWord(text[position - 1])) continue;
            if (IsWord(option.Token[^1]) && end < text.Length && IsWord(text[end])) continue;
            return option;
        }
        return null;
    }

    private string ResolveText(string text, bool alternate, ResolutionState state, int depth)
    {
        // Limits are guards against malformed/cyclic dictionaries, not a three-pass expansion rule.
        // Keep the unresolved source expression whenever a guard is reached.
        if (depth >= 128 || state.RemainingExpansions <= 0 || text.Length > 1_000_000) return text;
        var result = new StringBuilder(text.Length);
        for (var position = 0; position < text.Length;)
        {
            string replacement;
            int length;
            if (TryMatchMacro(text, position, out var macro, out var opening))
            {
                var closing = ClosingParenthesis(text, opening);
                if (closing < 0) { result.Append(text.AsSpan(position)); break; }
                length = closing + 1 - position;
                var raw = text.Substring(position, length);
                if (state.RemainingExpansions <= 0 || state.Visiting.Contains(macro.Definition.Token)) replacement = raw;
                else
                {
                    state.RemainingExpansions--;
                    // Evaluate arguments first: MAKE_FRAME(MAKE_FRAME(x)) is finite,
                    // even though recursive calls from MAKE_FRAME's own body are not.
                    var argument = ResolveText(text[(opening + 1)..closing], alternate, state, depth + 1);
                    var body = alternate ? macro.Definition.AlternateValue ?? macro.Definition.Value : macro.Definition.Value;
                    var substituted = SubstituteParameter(body, macro.Parameter, argument);
                    if (substituted is null) replacement = raw;
                    else
                    {
                        state.Visiting.Add(macro.Definition.Token);
                        replacement = ResolveText(substituted, alternate, state, depth + 1);
                        state.Visiting.Remove(macro.Definition.Token);
                    }
                }
            }
            else if (MatchVariable(text, position) is { } matched)
            {
                length = matched.Token.Length;
                var body = alternate ? matched.AlternateValue ?? matched.Value : matched.Value;
                if (body == matched.Token || state.RemainingExpansions <= 0 || !state.Visiting.Add(matched.Token)) replacement = matched.Token;
                else
                {
                    state.RemainingExpansions--;
                    replacement = ResolveText(body, alternate, state, depth + 1);
                    state.Visiting.Remove(matched.Token);
                }
            }
            else { result.Append(text[position++]); continue; }

            if (replacement.Length > state.RemainingCharacters)
            {
                replacement = text.Substring(position, length);
                state.RemainingExpansions = 0;
            }
            else state.RemainingCharacters -= replacement.Length;
            if (result.Length + replacement.Length > 1_000_000) return text;
            result.Append(replacement);
            position += length;
        }
        return result.Length > 1_000_000 ? text : result.ToString();
    }

    private static string? SubstituteParameter(string body, string parameter, string argument)
    {
        if (body.Length > 1_000_000) return null;
        var result = new StringBuilder();
        for (var position = 0; position < body.Length;)
        {
            var end = position + parameter.Length;
            if (end <= body.Length && body.AsSpan(position, parameter.Length).SequenceEqual(parameter.AsSpan())
                && (position == 0 || !IsWord(body[position - 1])) && (end == body.Length || !IsWord(body[end])))
            {
                if (result.Length + argument.Length > 1_000_000) return null;
                result.Append(argument);
                position = end;
            }
            else
            {
                if (result.Length >= 1_000_000) return null;
                result.Append(body[position++]);
            }
        }
        return result.ToString();
    }

    private bool HasCycle(string token)
    {
        // Iterative graph walk also detects cycles deeper than the old three-level limit.
        var pending = new Stack<(string Token, bool Exit)>();
        var visiting = new HashSet<string>(StringComparer.Ordinal);
        var completed = new HashSet<string>(StringComparer.Ordinal);
        pending.Push((token, false));
        while (pending.TryPop(out var current))
        {
            if (current.Exit) { visiting.Remove(current.Token); completed.Add(current.Token); continue; }
            if (completed.Contains(current.Token)) continue;
            if (!visiting.Add(current.Token)) return true;
            pending.Push((current.Token, true));
            var row = byToken[current.Token];
            foreach (var body in new[] { row.Value, row.AlternateValue }.OfType<string>())
            {
                if (body == row.Token && !TryParseMacroSignature(row.Token, out _, out _)) continue;
                foreach (var reference in References(body)) pending.Push((reference, false));
            }
        }
        return false;
    }

    private IEnumerable<string> References(string text)
    {
        for (var position = 0; position < text.Length; position++)
        {
            if (TryMatchMacro(text, position, out var macro, out _))
            {
                yield return macro.Definition.Token;
                position += macro.Name.Length - 1;
            }
            else if (MatchVariable(text, position) is { } row)
            {
                yield return row.Token;
                position += row.Token.Length - 1;
            }
        }
    }
}
