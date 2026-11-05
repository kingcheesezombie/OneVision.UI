using System.Globalization;

namespace OneVision.UI.Components;

/// <summary>Source value types used only to constrain rule-editor choices and validation.</summary>
public enum OVMappingRuleFieldType
{
    Text,
    Number,
    Date,
    Boolean
}

/// <summary>The consuming application's evaluation engine must implement the selected policy.</summary>
public enum OVMappingRuleCasePolicy
{
    ApplicationDefault,
    CaseSensitive,
    CaseInsensitive
}

/// <summary>Application-supplied metadata for an eligible rule source field.</summary>
public sealed record OVMappingRuleFieldDefinition(
    string Key,
    string Label,
    OVMappingRuleFieldType Type = OVMappingRuleFieldType.Text,
    IReadOnlyCollection<string>? AllowedOperatorKeys = null,
    IReadOnlyList<OVOption<string>>? ComparisonValues = null);

/// <summary>Stable operator metadata. UI validation is not runtime matching.</summary>
public sealed record OVMappingRuleOperatorDefinition(
    string Key,
    string Label,
    IReadOnlyCollection<OVMappingRuleFieldType> SupportedTypes,
    bool RequiresValue = true,
    string? Description = null);

public sealed record OVMappingRuleConditionValidation(bool IsValid, string? Message = null)
{
    public static readonly OVMappingRuleConditionValidation Valid = new(true);
}

/// <summary>
/// Portable rule-editor operator contract. Applications remain responsible for executing
/// conditions and must apply the same documented operator, null and case semantics.
/// </summary>
public static class OVMappingRuleOperatorContract
{
    public const string Equals = "equals";
    public const string NotEquals = "not-equals";
    public const string Contains = "contains";
    public const string NotContains = "not-contains";
    public const string StartsWith = "starts-with";
    public const string EndsWith = "ends-with";
    public const string Wildcard = "wildcard";
    public const string IsEmpty = "is-empty";
    public const string IsNotEmpty = "is-not-empty";

    private static readonly OVMappingRuleFieldType[] AllTypes =
        [OVMappingRuleFieldType.Text, OVMappingRuleFieldType.Number, OVMappingRuleFieldType.Date, OVMappingRuleFieldType.Boolean];
    private static readonly OVMappingRuleFieldType[] TextOnly = [OVMappingRuleFieldType.Text];

    public static IReadOnlyList<OVMappingRuleOperatorDefinition> StandardOperators { get; } =
    [
        new(Equals, "Equals", AllTypes),
        new(NotEquals, "Not equals", AllTypes),
        new(Contains, "Contains", TextOnly),
        new(NotContains, "Does not contain", TextOnly),
        new(StartsWith, "Starts with", TextOnly),
        new(EndsWith, "Ends with", TextOnly),
        new(Wildcard, "Wildcard pattern", TextOnly,
            Description: @"SQL LIKE-style pattern: % matches zero or more characters, _ matches one, and \ escapes %, _ or \."),
        new(IsEmpty, "Is empty", AllTypes, RequiresValue: false),
        new(IsNotEmpty, "Is not empty", AllTypes, RequiresValue: false)
    ];

    public static IReadOnlyList<OVMappingRuleOperatorDefinition> ForField(
        OVMappingRuleFieldDefinition field,
        IReadOnlyList<OVMappingRuleOperatorDefinition>? operators = null)
    {
        ArgumentNullException.ThrowIfNull(field);
        return (operators ?? StandardOperators)
            .Where(op => op.SupportedTypes.Contains(field.Type)
                && (field.AllowedOperatorKeys is null
                    || field.AllowedOperatorKeys.Contains(op.Key, StringComparer.Ordinal)))
            .ToArray();
    }

    /// <summary>
    /// Validates a condition for editing. This does not evaluate an input record or
    /// determine whether the rule matches. Disabled conditions do not participate.
    /// </summary>
    public static OVMappingRuleConditionValidation Validate(
        OVMappingRuleCondition condition,
        OVMappingRuleFieldDefinition? field,
        IReadOnlyList<OVMappingRuleOperatorDefinition>? operators = null)
    {
        ArgumentNullException.ThrowIfNull(condition);
        if (condition.Disabled)
            return OVMappingRuleConditionValidation.Valid;

        if (field is null || !string.Equals(condition.OperandKey, field.Key, StringComparison.Ordinal))
            return new(false, "Select an available source field.");

        var op = ForField(field, operators)
            .FirstOrDefault(value => string.Equals(value.Key, condition.OperatorKey, StringComparison.Ordinal));
        if (op is null)
            return new(false, "Select an operator supported by this field.");

        if (!op.RequiresValue)
            return string.IsNullOrEmpty(condition.Value)
                ? OVMappingRuleConditionValidation.Valid
                : new(false, "This operator does not accept a comparison value.");

        if (string.IsNullOrEmpty(condition.Value))
            return new(false, "Enter a comparison value.");

        if (field.ComparisonValues is not null && !field.ComparisonValues.Any(
            value => !value.Disabled && string.Equals(value.Value, condition.Value, StringComparison.Ordinal)))
            return new(false, "Select an available comparison value.");

        if (op.Key == Wildcard && !TryValidateWildcard(condition.Value, out var wildcardError))
            return new(false, wildcardError);

        if (field.Type == OVMappingRuleFieldType.Number
            && !decimal.TryParse(condition.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out _))
            return new(false, "Enter a number using invariant decimal notation.");

        if (field.Type == OVMappingRuleFieldType.Date
            && !DateOnly.TryParseExact(condition.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            return new(false, "Enter a date in yyyy-MM-dd format.");

        if (field.Type == OVMappingRuleFieldType.Boolean
            && !bool.TryParse(condition.Value, out _))
            return new(false, "Enter true or false.");

        return OVMappingRuleConditionValidation.Valid;
    }

    /// <summary>
    /// Validates wildcard escaping, not wildcard execution. Only %, _ and \ may
    /// follow a backslash. Non-escaped % and _ retain their LIKE-style meaning.
    /// </summary>
    public static bool TryValidateWildcard(string? expression, out string? error)
    {
        if (string.IsNullOrEmpty(expression))
        {
            error = "Enter a wildcard pattern.";
            return false;
        }

        for (var i = 0; i < expression.Length; i++)
        {
            if (expression[i] != '\\')
                continue;

            if (++i >= expression.Length || expression[i] is not ('%' or '_' or '\\'))
            {
                error = @"Use \%, \_ or \\ to escape a wildcard or backslash.";
                return false;
            }
        }

        error = null;
        return true;
    }
}
