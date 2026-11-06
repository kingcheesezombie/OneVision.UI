namespace OneVision.UI.Components;

/// <summary>
/// Determines whether the progressive builder fills its owning container or
/// deliberately limits its own content width. The parent owns its actual size.
/// </summary>
public enum OVProgressiveBuilderLayout
{
    FullWidth,
    Constrained
}

/// <summary>
/// Standard preserves the established understated preview. Emphasized opts into
/// a bounded summary panel without changing the application's content.
/// </summary>
public enum OVConfigurationPreviewAppearance
{
    Standard,
    Emphasized
}

/// <summary>
/// An informational preview state provided by the consuming application.
/// ReadyToSave never means saved or enabled; validation and persistence remain app-owned.
/// </summary>
public enum OVConfigurationPreviewState
{
    Neutral,
    Incomplete,
    ReadyToSave
}
