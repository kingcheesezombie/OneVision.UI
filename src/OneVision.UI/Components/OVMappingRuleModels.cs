namespace OneVision.UI.Components;

public enum OVMappingRuleJoin
{
    And,
    Or
}

public sealed record OVMappingRuleCondition(
    string Key,
    string? OperandKey = null,
    string? OperatorKey = null,
    string? Value = null,
    bool Disabled = false,
    bool IsValid = true,
    string? ValidationMessage = null);

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
public sealed record OVMappingRuleConditionRequest(string GroupKey, string? ConditionKey = null);
public sealed record OVMappingRuleGroupRequest(string ParentGroupKey, string? GroupKey = null);
public sealed record OVMappingRuleGroupJoinChange(string GroupKey, OVMappingRuleJoin Join);
