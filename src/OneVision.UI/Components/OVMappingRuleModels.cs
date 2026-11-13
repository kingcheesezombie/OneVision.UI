namespace OneVision.UI.Components;

public enum OVMappingRuleJoin
{
    And,
    Or
}

/// <summary>Semantic right-hand operand, independent of Direct or Associated resolution.</summary>
public enum OVMappingRuleOperandKind
{
    Field,
    Literal,
    None
}

public sealed record OVMappingRuleCondition(
    string Key,
    string? OperandKey = null,
    string? OperatorKey = null,
    string? Value = null,
    bool Disabled = false,
    bool IsValid = true,
    string? ValidationMessage = null,
    [property: System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
    OVMappingRuleOperandKind? OperandKind = null);

public sealed record OVMappingRuleGroup(
    string Key,
    OVMappingRuleJoin Join,
    IReadOnlyList<OVMappingRuleCondition> Conditions,
    IReadOnlyList<OVMappingRuleGroup> Groups)
{
    public OVMappingRuleGroup(string key, OVMappingRuleJoin join, IReadOnlyList<OVMappingRuleCondition> conditions)
        : this(key, join, conditions, Array.Empty<OVMappingRuleGroup>()) { }
}

public sealed record OVMappingRuleConditionChange(string GroupKey, OVMappingRuleCondition Condition);
/// <summary>Draft is optional for older AddConditionRequested consumers; no rule mutation happens inside the library.</summary>
public sealed record OVMappingRuleConditionRequest(string GroupKey, string? ConditionKey = null, OVMappingRuleCondition? Draft = null);
public sealed record OVMappingRuleGroupRequest(string ParentGroupKey, string? GroupKey = null);
public sealed record OVMappingRuleGroupJoinChange(string GroupKey, OVMappingRuleJoin Join);
