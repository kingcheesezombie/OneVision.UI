namespace OneVision.UI.Components;

public sealed record OVListItem(string Key, string Label, string? Description = null, string? EndText = null, bool Disabled = false);
