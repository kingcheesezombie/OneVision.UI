using OneVision.UI.Iconography;

namespace OneVision.UI.Components;

public sealed record OVOverflowMenuItem(
    string Key,
    string Label,
    OneVisionIcon? Icon = null,
    bool Disabled = false,
    bool Destructive = false);
