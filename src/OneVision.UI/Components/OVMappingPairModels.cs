namespace OneVision.UI.Components;

public enum OVMappingTargetKind
{
    Contract,
    StandardDetail
}

public sealed record OVMappingPairTarget(
    string Key,
    string Label,
    string? Description = null,
    bool Required = false,
    bool Disabled = false,
    bool ReadOnly = false,
    OVMappingTargetKind Kind = OVMappingTargetKind.Contract);

public sealed record OVStandardDetailDefinition(
    string Id,
    string Name,
    string? Description = null,
    string? DataType = null,
    bool Enabled = true);

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

public static class OVMappingPairValidation
{
    public static bool IsValid(
        IEnumerable<OVMappingPairTarget> requiredTargets,
        IEnumerable<OVStandardDetailDefinition> standardDetails,
        IEnumerable<string> selectedStandardDetailIds,
        IEnumerable<OVMappingPairEntry> mappings)
    {
        var entries = mappings.ToDictionary(mapping => mapping.TargetKey, StringComparer.Ordinal);

        foreach (var target in requiredTargets.Where(target => target.Required))
        {
            if (!entries.TryGetValue(target.Key, out var mapping) || !mapping.IsValid)
                return false;
        }

        var enabledDetails = standardDetails.Where(detail => detail.Enabled).Select(detail => detail.Id).ToHashSet(StringComparer.Ordinal);
        foreach (var detailId in selectedStandardDetailIds.Where(enabledDetails.Contains))
        {
            if (!entries.TryGetValue(detailId, out var mapping) || !mapping.IsValid)
                return false;
        }

        return true;
    }
}
