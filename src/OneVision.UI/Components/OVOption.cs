namespace OneVision.UI.Components;

public sealed record OVOption<T>(
    T Value,
    string Label,
    bool Disabled = false,
    string? Group = null);
