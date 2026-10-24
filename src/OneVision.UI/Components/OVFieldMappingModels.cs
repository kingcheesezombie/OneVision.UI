namespace OneVision.UI.Components;

public sealed record OVFieldMappingTargetField(
    string Key,
    string Label,
    string? Description = null,
    bool Required = false,
    bool Disabled = false,
    bool ReadOnly = false);

public sealed record OVFieldMappingEntry(
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

public sealed record OVFieldMappingRequest(
    OVStructuredFieldNode Source,
    OVFieldMappingTargetField Target,
    OVFieldMappingEntry? ExistingMapping = null);

public sealed record OVFieldMappingConstantRequest(
    OVFieldMappingTargetField Target,
    OVFieldMappingEntry? ExistingMapping = null);
