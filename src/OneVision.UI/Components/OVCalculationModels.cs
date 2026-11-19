using System.Globalization;

namespace OneVision.UI.Components;

/// <summary>The tier gates editing controls; it never changes expression meaning or persisted syntax.</summary>
public enum OVCalculationTier { Simple = 1, Extended = 2, Advanced = 3 }
public enum OVCalculationTokenKind { Operand, Number, Operator, OpenParenthesis, CloseParenthesis, Function }
public enum OVCalculationOperandSource { Core, External }
public enum OVCalculationValidationStatus { Valid, Empty, Incomplete, Invalid, UnsupportedTier, MissingOperand, Unavailable }

public sealed record OVCalculationOperand(
    string Key, string Label, OVCalculationOperandSource Source = OVCalculationOperandSource.Core,
    string? CategoryKey = null, string? Description = null, bool Enabled = true);

public sealed record OVCalculationOperandCategory(string Key, string Label, IReadOnlyList<OVCalculationOperand> Operands);
public sealed record OVCalculationFunction(string Key, string Label);

/// <summary>Each token has a stable editing identity; Key stores an operand/function/operator identifier.</summary>
public sealed record OVCalculationToken(string Id, OVCalculationTokenKind Kind, string? Key = null, decimal? Number = null);

public sealed record OVCalculationDefinition(IReadOnlyList<OVCalculationToken> Tokens)
{
    public static OVCalculationDefinition Empty { get; } = new([]);
}

public sealed record OVCalculationDiagnostic(OVCalculationValidationStatus Status, string Message)
{
    public bool IsValid => Status == OVCalculationValidationStatus.Valid;
}

/// <summary>Format is independent of the expression, and never modifies its numeric result.</summary>
public sealed record OVCalculationFormat(string FormatId, string? Mask = null);
public sealed record OVCalculationFormatOption(string Id, string Label, string Description, IReadOnlyList<string>? Masks = null);

public sealed record OVCalculationPreview(
    string? DisplayValue, string? RawValue = null, string? Explanation = null,
    bool Loading = false, string? Error = null);

/// <summary>
/// Structural validation only: this UI helper does not evaluate formulas or define aggregation semantics.
/// Application validation remains authoritative.
/// </summary>
public static class OVCalculationSyntax
{
    public static OVCalculationTier RequiredTier(OVCalculationDefinition? definition,
        IEnumerable<OVCalculationOperand> operands)
    {
        var externalKeys = operands.Where(o => o.Source == OVCalculationOperandSource.External)
            .Select(o => o.Key).ToHashSet(StringComparer.Ordinal);
        if (definition?.Tokens.Any(t => t.Kind == OVCalculationTokenKind.Function) == true)
            return OVCalculationTier.Advanced;
        if (definition?.Tokens.Any(t => t.Kind == OVCalculationTokenKind.Operand && t.Key is not null && externalKeys.Contains(t.Key)) == true)
            return OVCalculationTier.Extended;
        return OVCalculationTier.Simple;
    }

    public static OVCalculationDiagnostic Validate(OVCalculationDefinition? definition, OVCalculationTier tier,
        IEnumerable<OVCalculationOperand> operands, IEnumerable<OVCalculationFunction>? functions = null)
    {
        if (definition?.Tokens is not { Count: > 0 } tokens)
            return new(OVCalculationValidationStatus.Empty, "Add an operand or a number to begin.");
        var operandMap = operands.ToDictionary(o => o.Key, StringComparer.Ordinal);
        var functionKeys = (functions ?? []).Select(f => f.Key).ToHashSet(StringComparer.Ordinal);
        var ids = new HashSet<string>(StringComparer.Ordinal);
        var expectedValue = true;
        var requiresFunctionParenthesis = false;
        var depth = 0;
        foreach (var t in tokens)
        {
            if (requiresFunctionParenthesis && t.Kind != OVCalculationTokenKind.OpenParenthesis)
                return new(OVCalculationValidationStatus.Invalid, "A function must be followed by an opening parenthesis.");
            requiresFunctionParenthesis = false;
            if (string.IsNullOrWhiteSpace(t.Id) || !ids.Add(t.Id))
                return new(OVCalculationValidationStatus.Invalid, "Each token requires a unique stable identity.");
            switch (t.Kind)
            {
                case OVCalculationTokenKind.Operand:
                    if (string.IsNullOrWhiteSpace(t.Key) || !operandMap.TryGetValue(t.Key, out var operand) || !operand.Enabled)
                        return new(OVCalculationValidationStatus.MissingOperand, "A referenced operand is unavailable.");
                    if (operand.Source == OVCalculationOperandSource.External && tier < OVCalculationTier.Extended)
                        return new(OVCalculationValidationStatus.UnsupportedTier, "This expression requires Tier 2 or higher.");
                    if (!expectedValue) return new(OVCalculationValidationStatus.Invalid, "An operator is required between values.");
                    expectedValue = false;
                    break;
                case OVCalculationTokenKind.Number:
                    if (t.Number is null) return new(OVCalculationValidationStatus.Invalid, "A number token requires a value.");
                    if (!expectedValue) return new(OVCalculationValidationStatus.Invalid, "An operator is required between values.");
                    expectedValue = false;
                    break;
                case OVCalculationTokenKind.Operator:
                    if (expectedValue || t.Key is not ("+" or "-" or "*" or "/"))
                        return new(OVCalculationValidationStatus.Invalid, "An operator must follow a value.");
                    expectedValue = true;
                    break;
                case OVCalculationTokenKind.Function:
                    if (tier < OVCalculationTier.Advanced)
                        return new(OVCalculationValidationStatus.UnsupportedTier, "Aggregation functions require Tier 3.");
                    if (!expectedValue || t.Key is null || !functionKeys.Contains(t.Key))
                        return new(OVCalculationValidationStatus.Invalid, "The function is not available in this expression.");
                    requiresFunctionParenthesis = true;
                    expectedValue = true;
                    break;
                case OVCalculationTokenKind.OpenParenthesis:
                    if (!expectedValue) return new(OVCalculationValidationStatus.Invalid, "An operator is required before an opening parenthesis.");
                    depth++;
                    expectedValue = true;
                    break;
                case OVCalculationTokenKind.CloseParenthesis:
                    if (expectedValue || depth == 0)
                        return new(OVCalculationValidationStatus.Invalid, "A closing parenthesis requires a complete expression.");
                    depth--;
                    expectedValue = false;
                    break;
                default:
                    return new(OVCalculationValidationStatus.Invalid, "Unknown expression token.");
            }
        }
        if (depth != 0 || expectedValue)
            return new(OVCalculationValidationStatus.Incomplete, "Complete the expression and close all parentheses.");
        return new(OVCalculationValidationStatus.Valid, "Expression structure is valid; execution is application-owned.");
    }

    public static string TokenLabel(OVCalculationToken token, IEnumerable<OVCalculationOperand> operands,
        IEnumerable<OVCalculationFunction> functions)
        => token.Kind switch
        {
            OVCalculationTokenKind.Operand => operands.FirstOrDefault(o => o.Key == token.Key)?.Label ?? "Missing operand",
            OVCalculationTokenKind.Number => token.Number?.ToString(CultureInfo.InvariantCulture) ?? "?",
            OVCalculationTokenKind.Operator => token.Key switch { "*" => "×", "/" => "÷", _ => token.Key ?? "?" },
            OVCalculationTokenKind.OpenParenthesis => "(",
            OVCalculationTokenKind.CloseParenthesis => ")",
            OVCalculationTokenKind.Function => functions.FirstOrDefault(f => f.Key == token.Key)?.Label ?? token.Key ?? "Function",
            _ => "?"
        };
}
