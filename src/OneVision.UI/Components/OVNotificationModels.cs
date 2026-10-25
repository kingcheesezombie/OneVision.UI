namespace OneVision.UI.Components;

public enum OVNotificationTone
{
    Success,
    Information,
    Warning,
    Error
}

public sealed record OVNotificationRequest(
    OVNotificationTone Tone,
    string Message,
    string? Title = null,
    TimeSpan? Timeout = null,
    bool Persistent = false,
    bool Dismissible = true,
    string? ActionLabel = null,
    Func<Task>? Action = null,
    string? ReplacementKey = null);

public sealed record OVNotificationEntry(Guid Id, OVNotificationRequest Request);
