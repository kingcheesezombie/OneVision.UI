namespace OneVision.UI.Components;

public sealed record OVBreadcrumbItem(
    string Label,
    string? Href = null,
    bool Disabled = false);
