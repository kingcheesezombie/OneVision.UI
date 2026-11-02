namespace OneVision.UI.Components;

public enum OVSourceConnectionMode
{
    ManagedConnection,
    NoConnectionRequired
}

public enum OVSourceProfileState
{
    Initial,
    Running,
    Current,
    Stale,
    Failed
}

public enum OVSourceDiscoveryState
{
    Initial,
    Discovering,
    Success,
    Empty,
    Failed
}

public sealed record OVSourceDiscoveryCandidate(
    string Id,
    string Title,
    string? Description = null,
    int? Ordinal = null,
    int? ColumnCount = null,
    int? ApproximateRowCount = null,
    IReadOnlyList<string>? ColumnHeadings = null,
    IReadOnlyList<IReadOnlyList<string?>>? PreviewRows = null,
    bool Usable = true,
    string? UnusableReason = null)
{
    public IReadOnlyList<string> ResolvedColumnHeadings => ColumnHeadings ?? Array.Empty<string>();
    public IReadOnlyList<IReadOnlyList<string?>> ResolvedPreviewRows => PreviewRows ?? Array.Empty<IReadOnlyList<string?>>();
}
