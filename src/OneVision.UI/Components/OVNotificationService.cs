namespace OneVision.UI.Components;

public sealed class OVNotificationService
{
    public event Action<OVNotificationEntry>? NotificationRaised;

    public Guid Show(OVNotificationRequest request)
    {
        var entry = new OVNotificationEntry(Guid.NewGuid(), request);
        NotificationRaised?.Invoke(entry);
        return entry.Id;
    }

    public Guid Success(string message, string? title = null, TimeSpan? timeout = null)
        => Show(new(OVNotificationTone.Success, message, title, timeout));

    public Guid Information(string message, string? title = null, TimeSpan? timeout = null)
        => Show(new(OVNotificationTone.Information, message, title, timeout));

    public Guid Warning(string message, string? title = null, TimeSpan? timeout = null)
        => Show(new(OVNotificationTone.Warning, message, title, timeout));

    public Guid Error(string message, string? title = null, TimeSpan? timeout = null, bool persistent = false)
        => Show(new(OVNotificationTone.Error, message, title, timeout, persistent));
}
