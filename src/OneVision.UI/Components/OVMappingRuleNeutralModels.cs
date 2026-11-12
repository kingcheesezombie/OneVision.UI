namespace OneVision.UI.Components;

/// <summary>Application-supplied authority that scopes the fields and permissible configuration of a rule.</summary>
public sealed record OVMappingGoverningContract(
    string Key,
    string Name,
    string SourceLabel,
    string SourceType,
    string SourceOrigin,
    string TargetLabel,
    string TargetType,
    bool Enabled = true,
    bool IsValid = true,
    string? ValidationMessage = null,
    string? Revision = null);

/// <summary>Application-defined labels. Defaults are portable; EAM may override them at composition.</summary>
public sealed record OVMappingRuleTerminology(
    string Contract = "Governing Contract",
    string Source = "Source",
    string Target = "Target",
    string AssociatedFields = "Associated Fields",
    string Outcome = "Outcome",
    string ProcessingEligible = "Processing eligible",
    string AutomaticProcessingEligible = "Automatic processing eligible",
    string RelationshipReference = "Relationship reference",
    string DirectGuidance = "Compare fields from the source against fields held directly by the target.",
    string AssociatedGuidance = "Compare source fields against fields available through records associated with the target.");

/// <summary>Stable source/right-hand identities and data types; display-only catalogue metadata.</summary>
public sealed record OVMappingComparisonField(
    string Key,
    string Label,
    OVMappingRuleFieldType Type = OVMappingRuleFieldType.Text,
    IReadOnlyCollection<string>? AllowedOperators = null,
    bool Disabled = false,
    string? Description = null);

/// <summary>
/// Optional catalogues for the existing rule condition model. Tree node keys and
/// condition OperandKey/Value values share stable application-supplied identities.
/// </summary>
public sealed record OVMappingResolutionFields(
    IReadOnlyList<OVStructuredFieldNode> SourceTree,
    IReadOnlyList<OVStructuredFieldNode> TargetTree,
    IReadOnlyList<OVStructuredFieldNode> AssociatedTree,
    IReadOnlyList<OVMappingComparisonField> SourceFields,
    IReadOnlyList<OVMappingComparisonField> TargetFields,
    IReadOnlyList<OVMappingComparisonField> AssociatedFields,
    bool Loading = false,
    string? Error = null,
    string? Revision = null);
