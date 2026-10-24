using OneVision.UI.Iconography;

namespace OneVision.UI.Components;

public sealed record OVSideNavItem(
    string Key,
    string Label,
    IReadOnlyList<OVSideNavItem>? Children = null,
    bool Disabled = false,
    OneVisionIcon? Icon = null)
{
    public IReadOnlyList<OVSideNavItem> Items { get; } = Children ?? Array.Empty<OVSideNavItem>();
}
