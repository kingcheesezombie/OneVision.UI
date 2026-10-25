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
