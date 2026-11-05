namespace OneVision.UI.Components;

/// <summary>A direction-based reordering request; applications persist the resulting order.</summary>
public sealed record OVMappingRuleConditionMoveRequest(string GroupKey, string ConditionKey, int Offset);

/// <summary>Application-supplied read-only explanation for one evaluated condition.</summary>
public sealed record OVMappingRuleTestConditionResult(
    string Key, string Description, bool Matched, bool Skipped = false);

/// <summary>
/// Display-only rule evaluation result. Does not create a candidate, record a
/// decision, approve an outcome, or write to an allocation authority.
/// </summary>
public sealed record OVMappingRuleTestResult(
    string Input,
    bool Matched,
    IReadOnlyList<OVMappingRuleTestConditionResult> Conditions,
    string? Outcome = null,
    bool Ambiguous = false,
    string? ErrorMessage = null);
