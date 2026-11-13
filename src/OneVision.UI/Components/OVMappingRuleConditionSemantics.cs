namespace OneVision.UI.Components;

/// <summary>
/// Editor-only operand classification. Runtime semantics and executable operators are app-owned.
/// Existing 0.1.52 field operators remain classified through the supplied operator catalogue.
/// </summary>
public static class OVMappingRuleConditionSemantics
{
    public static OVMappingRuleOperandKind? Resolve(
        OVMappingRuleCondition condition,
        IReadOnlyList<OVMappingRuleOperatorDefinition>? operators,
        IReadOnlyCollection<string>? fieldOperatorKeys)
    {
        if (condition.OperandKind is { } explicitKind) return explicitKind;
        var definition = (operators ?? OVMappingRuleOperatorContract.StandardOperators)
            .FirstOrDefault(x => string.Equals(x.Key, condition.OperatorKey, StringComparison.Ordinal));
        if (definition?.OperandKind is { } configured) return configured;
        if (definition is { RequiresValue: false }) return OVMappingRuleOperandKind.None;
        if (fieldOperatorKeys?.Contains(condition.OperatorKey ?? "", StringComparer.Ordinal) == true)
            return OVMappingRuleOperandKind.Field;
        if (definition is not null) return OVMappingRuleOperandKind.Literal;
        return null; // Unrecognised legacy definitions remain visible for application correction.
    }

    public static bool Supports(
        OVMappingRuleOperandKind kind,
        IReadOnlyList<OVMappingRuleOperatorDefinition>? definitions,
        IReadOnlyCollection<string>? fieldOperatorKeys)
    {
        var operators = definitions ?? OVMappingRuleOperatorContract.StandardOperators;
        return operators.Any(op =>
            op.OperandKind == kind ||
            (op.OperandKind is null &&
                (kind == OVMappingRuleOperandKind.None ? !op.RequiresValue :
                 kind == OVMappingRuleOperandKind.Field ?
                    fieldOperatorKeys?.Contains(op.Key, StringComparer.Ordinal) == true :
                    op.RequiresValue && fieldOperatorKeys?.Contains(op.Key, StringComparer.Ordinal) != true)));
    }
}
