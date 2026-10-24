using Microsoft.AspNetCore.Components;

namespace OneVision.UI.Components;

public sealed record OVAccordionItem(
    string Title,
    RenderFragment Content,
    bool Expanded = false,
    bool Disabled = false);
