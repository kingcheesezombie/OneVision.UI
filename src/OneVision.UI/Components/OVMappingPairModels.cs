namespace OneVision.UI.Components;

public sealed record OVMappingPairTarget(
    string Key,
    string Label,
    string? Description = null,
    bool Required = false,
    bool Disabled = false,
    bool ReadOnly = false);

public sealed record OVMappingPairEntry(
    string TargetKey,
    string? SourceKey = null,
    string? SourcePath = null,
    string? ConstantValue = null,
    bool IsValid = true,
    string? ValidationMessage = null,
    bool CanConfigure = false)
{
    public bool IsConstant => ConstantValue is not null;
}

public sealed record OVMappingPairRequest(
    OVStructuredFieldNode Source,
    OVMappingPairTarget Target,
    OVMappingPairEntry? ExistingMapping = null);

public sealed record OVMappingPairConstantRequest(
    OVMappingPairTarget Target,
    OVMappingPairEntry? ExistingMapping = null);
