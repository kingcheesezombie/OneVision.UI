namespace OneVision.UI.Components;

public enum OVTimelineTone
{
    Neutral,
    Information,
    Success,
    Warning,
    Error
}

public sealed record OVTimelineItem(
    string Key,
    DateTimeOffset Timestamp,
    string Title,
    string? Description = null,
    OVTimelineTone Tone = OVTimelineTone.Neutral,
    string? Metadata = null,
    bool ActionAvailable = false,
    string? ActionLabel = null);
