namespace OneVision.UI.Components;

public sealed record OVContextSummaryItem(
    string Label,
    string Value,
    string? SupportingText = null,
    OVPillTone? Tone = null);
