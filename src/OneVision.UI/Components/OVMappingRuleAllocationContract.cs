namespace OneVision.UI.Components;

/// <summary>Governed matching-rule compositions. No application execution semantics.</summary>
public enum OVMappingRuleComposition
{
    /// <summary>Application-neutral governing-contract composition (the default).</summary>
    ContractDriven = 0,
    /// <summary>Compatibility alias for the 0.1.51 EAM-facing composition name.</summary>
    AllocationContract = ContractDriven,
    Legacy = 1
}

/// <summary>Application-supplied resolution methodology; not a second allocation contract.</summary>
public enum OVMappingRuleResolutionType
{
    Direct,
    Associated
}

/// <summary>Read-only EAM allocation-rule relationship to which a matching rule is bound.</summary>
public sealed record OVMappingAllocationContract(
    string RuleKey,
    string RuleName,
    string SourceClass,
    string SourceType,
    string Source,
    string TargetProvider,
    string TargetType,
    bool Enabled = true,
    bool IsValid = true,
    string? ValidationMessage = null,
    string? Revision = null);

/// <summary>Display-only evidence supplied by an application's matching engine.</summary>
public sealed record OVMappingRuleEvidence(
    string Field,
    string? Value,
    string? OriginEntity = null,
    string? AllocationReference = null);

/// <summary>Application-calculated matching and allocation eligibility status.</summary>
public sealed record OVMappingRuleEligibility(
    bool ConditionMatched,
    bool AllocationEligible,
    bool AutomaticProcessingEligible,
    string? Explanation = null);
