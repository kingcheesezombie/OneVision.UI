namespace OneVision.UI.Components;

/// <summary>Native Blazor content is the default; external iframe content is opt-in.</summary>
public enum OVOverlayContentMode { Native, External }

/// <summary>Policy for closing an external document without a cooperative dirty-state protocol.</summary>
public enum OVOverlayExternalClosePolicy { Normal, Confirm }

/// <summary>
/// Host-owned allowlisted URL configuration. Origin entries must be full scheme/host/port
/// origins (for example https://example.org or http://localhost:8081).
/// This configuration does not grant the destination any permissions.
/// </summary>
public sealed class OVOverlayExternalContent
{
    public string Url { get; set; } = string.Empty;
    public IReadOnlyCollection<string> PermittedOrigins { get; set; } = Array.Empty<string>();
    public string? StandaloneUrl { get; set; }
    public string FrameTitle { get; set; } = "External content";
    public OVOverlayExternalClosePolicy ClosePolicy { get; set; } = OVOverlayExternalClosePolicy.Normal;
}
