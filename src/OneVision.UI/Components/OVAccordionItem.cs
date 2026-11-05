using Microsoft.AspNetCore.Components;

namespace OneVision.UI.Components;

/// <summary>
/// <c>Key</c> is optional for legacy consumers. Supply a stable key
/// when items can be reordered, inserted or removed between renders.
/// </summary>
public sealed record OVAccordionItem(
    string Title,
    RenderFragment Content,
    bool Expanded = false,
    bool Disabled = false,
    string? Key = null);
