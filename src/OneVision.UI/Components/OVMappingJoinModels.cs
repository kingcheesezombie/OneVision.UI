namespace OneVision.UI.Components;

public sealed record OVMappingJoinFieldState(
    string Key,
    bool Required = false,
    bool ReadOnly = false);

public sealed record OVMappingJoinDefinition(
    string Key,
    string LeftKey,
    string RightKey,
    string MethodKey,
    bool IsValid = true,
    string? ValidationMessage = null,
    bool ReadOnly = false);

public sealed record OVMappingJoinRequest(
    OVStructuredFieldNode Left,
    OVStructuredFieldNode Right,
    string MethodKey,
    OVMappingJoinDefinition? ExistingJoin = null);

public sealed record OVMappingJoinMethodChange(
    OVMappingJoinDefinition Join,
    string MethodKey);

public enum OVMappingStructureSide
{
    Left,
    Right
}

public sealed record OVMappingStructureType(
    string Value,
    string Label,
    bool RequiresDefinition = false,
    string? DefinitionLabel = null,
    string? SelectDefinitionText = null,
    string? NoDefinitionsText = null,
    string? NoFieldsText = null,
    bool Disabled = false);

public sealed record OVMappingStructureDefinition(
    string Value,
    string Label,
    bool Disabled = false);

public sealed record OVMappingStructureState
{
    public string TypeLabel { get; init; } = "Structure type";
    public IReadOnlyList<OVMappingStructureType> Types { get; init; } = Array.Empty<OVMappingStructureType>();
    public string? SelectedType { get; init; }
    public IReadOnlyList<OVMappingStructureDefinition> Definitions { get; init; } = Array.Empty<OVMappingStructureDefinition>();
    public string? SelectedDefinition { get; init; }
    public bool DefinitionsLoading { get; init; }
    public bool StructureLoading { get; init; }
    public IReadOnlyList<OVStructuredFieldNode> Fields { get; init; } = Array.Empty<OVStructuredFieldNode>();
    public string? NoDefinitionsText { get; init; }
    public string? SelectDefinitionText { get; init; }
    public string? NoFieldsText { get; init; }
}

public sealed record OVMappingStructureSelectionChange(
    OVMappingStructureSide Side,
    string? TypeKey,
    string? DefinitionKey,
    IReadOnlyList<string> InvalidJoinKeys);
