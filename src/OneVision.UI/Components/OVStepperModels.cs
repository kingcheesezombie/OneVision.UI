namespace OneVision.UI.Components;

public enum OVStepState
{
    Pending,
    Current,
    Complete,
    Error,
    Disabled
}

public sealed record OVStep(
    string Key,
    string Label,
    OVStepState State,
    string? Description = null,
    bool CanNavigate = false);
