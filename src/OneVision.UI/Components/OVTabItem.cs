using Microsoft.AspNetCore.Components;

namespace OneVision.UI.Components;

public sealed record OVTabItem(
    string Key,
    string Label,
    RenderFragment Content,
    bool Disabled = false);
