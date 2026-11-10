namespace OneVision.UI.Components;

/// <summary>
/// Governs optional rule-editor capability presentation without changing its stored data.
/// Unavailable and Hidden do not render; Disabled and ReadOnly remain inspectable.
/// </summary>
public enum OVMappingRuleCapabilityState
{
    Unavailable,
    Hidden,
    Disabled,
    ReadOnly,
    Enabled
}
