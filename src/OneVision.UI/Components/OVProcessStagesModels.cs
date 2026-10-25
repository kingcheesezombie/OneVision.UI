namespace OneVision.UI.Components;

public enum OVProcessStageState
{
    Pending,
    Running,
    Complete,
    Failed,
    Skipped,
    Warning
}

public sealed record OVProcessStage(
    string Key,
    string Name,
    OVProcessStageState State,
    string? Description = null,
    string? CountSummary = null,
    TimeSpan? Duration = null,
    DateTimeOffset? StartedAt = null,
    DateTimeOffset? CompletedAt = null,
    string? Message = null,
    bool ActionAvailable = false,
    string? ActionLabel = null);
