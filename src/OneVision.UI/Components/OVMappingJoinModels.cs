using Microsoft.AspNetCore.Components;

namespace OneVision.UI.Components;

public sealed record OVMappingJoinItem(
    string Key,
    string Label,
    string? Description = null,
    bool Disabled = false,
    bool ReadOnly = false,
    bool Required = false,
    string? Group = null);

public sealed record OVMappingJoinDefinition(
    string Key,
    string LeftKey,
    string RightKey,
    string MethodKey,
    bool IsValid = true,
    string? ValidationMessage = null,
    bool ReadOnly = false);

public sealed record OVMappingJoinRequest(
    OVMappingJoinItem Left,
    OVMappingJoinItem Right,
    string MethodKey,
    OVMappingJoinDefinition? ExistingJoin = null);

public sealed record OVMappingJoinMethodChange(
    OVMappingJoinDefinition Join,
    string MethodKey);

public sealed record OVMappingJoinCatalogueContext(
    string? SearchText,
    string? SelectedKey,
    EventCallback<string?> SelectedKeyChanged,
    bool ReadOnly);
