namespace OneVision.UI.Components;

public sealed record OVSelectionOption(
    string Key,
    string Label,
    string? Group = null,
    bool Disabled = false,
    string? SearchText = null);
