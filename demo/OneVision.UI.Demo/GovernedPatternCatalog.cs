namespace OneVision.UI.Demo;

/// <summary>
/// Authoritative gallery manifest for implemented OneVision patterns.
/// A pattern is listed here only when the repository contains a real implementation
/// and an explicit representative demo. There are no metadata-generated previews.
/// </summary>
public static class GovernedPatternCatalog
{
    private static readonly PatternNode ListPage = new(
        "List Page",
        "list-page",
        PatternNodeKind.Container,
        "OVP-LIST-PAGE",
        PatternNodeStatus.Draft,
        "Standard OneVision searchable, filterable and paginated list implemented by OVListPage and its representative gallery demo.",
        new PatternNode("Toolbar", "toolbar", PatternNodeKind.Section, "OVP-LIST-PAGE-TOOLBAR", PatternNodeStatus.Draft, "Search, filters, pagination context and primary action."),
        new PatternNode("Results", "results", PatternNodeKind.Section, "OVP-LIST-PAGE-RESULTS", PatternNodeStatus.Draft, "Compact governed list/grid results."));

    private static readonly PatternNode SourceConfiguration = new(
        "Source Configuration",
        "source-configuration",
        PatternNodeKind.Container,
        "OVP-SOURCE-CONFIGURATION",
        PatternNodeStatus.Draft,
        "Managed inbound source configuration implemented by OVSourceConfiguration and its representative gallery demo.",
        new PatternNode("Overview", "overview", PatternNodeKind.Tab, "OVP-SOURCE-CONFIGURATION-OVERVIEW", PatternNodeStatus.Draft, "Source identity, type, enabled state and summary."),
        new PatternNode("Connection", "connection", PatternNodeKind.Tab, "OVP-SOURCE-CONFIGURATION-CONNECTION", PatternNodeStatus.Draft, "Select the managed endpoint and authentication context. Connection does not define queries, routes or source reads."),
        new PatternNode("Profile", "profile", PatternNodeKind.Tab, "OVP-SOURCE-CONFIGURATION-PROFILE", PatternNodeStatus.Draft, "Define what to execute/read through the selected connection, run it, and profile the returned structure."),
        new PatternNode("Mapping", "mapping", PatternNodeKind.Tab, "OVP-SOURCE-CONFIGURATION-MAPPING", PatternNodeStatus.Draft, "Map the profiled output into the declared target contract using governed mapping controls."));

    private static readonly PatternNode OperationalDetail = new(
        "Operational Detail",
        "operational-detail",
        PatternNodeKind.Container,
        "OVP-OPERATIONAL-DETAIL",
        PatternNodeStatus.Draft,
        "Read-only operational/configuration detail implemented by OVOperationalDetail and its representative gallery demo.",
        new PatternNode("Identity and status", "identity-status", PatternNodeKind.Section, "OVP-OPERATIONAL-DETAIL-STATUS", PatternNodeStatus.Draft, "Title, description, status and optional Enabled toggle."),
        new PatternNode("Health summary", "health-summary", PatternNodeKind.Section, "OVP-OPERATIONAL-DETAIL-HEALTH", PatternNodeStatus.Draft, "Compact current health or operational summary."),
        new PatternNode("Latest event", "latest-event", PatternNodeKind.Section, "OVP-OPERATIONAL-DETAIL-LATEST", PatternNodeStatus.Draft, "Optional latest run or event summary."),
        new PatternNode("Process stages", "process-stages", PatternNodeKind.Section, "OVP-OPERATIONAL-DETAIL-STAGES", PatternNodeStatus.Draft, "Optional OVC-PROCESS-STAGES composition."),
        new PatternNode("Recent activity", "recent-activity", PatternNodeKind.Section, "OVP-OPERATIONAL-DETAIL-ACTIVITY", PatternNodeStatus.Draft, "Optional recent activity/history data."));

    private static readonly PatternNode Allocation = new(
        "Allocation",
        "allocation",
        PatternNodeKind.Container,
        "OVP-ALLOCATION",
        PatternNodeStatus.Draft,
        "Create an effective-dated allocation as one continuous vertical page, implemented by OVAllocation and its representative gallery demo.",
        new PatternNode("Target platform", "target-platform", PatternNodeKind.Section, "OVP-ALLOCATION-TARGET-PLATFORM", PatternNodeStatus.Draft, "Finder state only: horizontal EAM, ODM and IDM options. Target type is shown for EAM only."),
        new PatternNode("Target", "target", PatternNodeKind.Section, "OVP-ALLOCATION-TARGET", PatternNodeStatus.Draft, "Known-target context or target search/results and selected target."),
        new PatternNode("Source", "source", PatternNodeKind.Section, "OVP-ALLOCATION-SOURCE", PatternNodeStatus.Draft, "Source type, search and single-select source results."),
        new PatternNode("Effective dates", "effective-dates", PatternNodeKind.Section, "OVP-ALLOCATION-DATES", PatternNodeStatus.Draft, "Always belonged defaults on and disables From/To dates."),
        new PatternNode("Actions", "actions", PatternNodeKind.Section, "OVP-ALLOCATION-ACTIONS", PatternNodeStatus.Draft, "Cancel and Allocate at the bottom of the same vertical form."));

    public static IReadOnlyList<PatternNode> Patterns { get; } =
    [
        ListPage,
        SourceConfiguration,
        OperationalDetail,
        Allocation
    ];

    public static PatternNode? FindPattern(string? slug)
        => Patterns.FirstOrDefault(pattern => string.Equals(pattern.Slug, slug, StringComparison.OrdinalIgnoreCase));

    public static PatternNode Resolve(PatternNode root, params string?[] path)
        => PatternCatalog.Resolve(root, path);
}
