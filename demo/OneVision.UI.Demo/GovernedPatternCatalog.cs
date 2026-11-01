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

    private static readonly PatternNode CatalogueManagement = new(
        "Catalogue Management",
        "catalogue-management",
        PatternNodeKind.Container,
        "OVP-CATALOGUE-MANAGEMENT",
        PatternNodeStatus.Draft,
        "Domain-neutral management of small governed or application-owned catalogues using OVListPage, compact results and governed Add/Edit overlays.",
        new PatternNode("Toolbar", "toolbar", PatternNodeKind.Section, "OVP-CATALOGUE-MANAGEMENT-TOOLBAR", PatternNodeStatus.Draft, "Primary Add action, search, optional filters and pagination context."),
        new PatternNode("Results", "results", PatternNodeKind.Section, "OVP-CATALOGUE-MANAGEMENT-RESULTS", PatternNodeStatus.Draft, "Compact application-defined columns with governed density, state and row-action placement."),
        new PatternNode("Editor", "editor", PatternNodeKind.Section, "OVP-CATALOGUE-MANAGEMENT-EDITOR", PatternNodeStatus.Draft, "Right-side Add/Edit overlay with application-supplied fields and governed dirty, busy, validity and save/cancel behaviour."),
        new PatternNode("States", "states", PatternNodeKind.Section, "OVP-CATALOGUE-MANAGEMENT-STATES", PatternNodeStatus.Draft, "Loading, error, empty, no-results and read-only catalogue states."));

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
        new PatternNode("Mapping", "mapping", PatternNodeKind.Tab, "OVP-SOURCE-CONFIGURATION-MAPPING", PatternNodeStatus.Draft, "Map the profiled output into the feed-defined required contract and, when enabled, explicitly selected application-supplied Standard Details."));

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

    private static readonly PatternNode EntityInspector = new(
        "Entity Inspector",
        "entity-inspector",
        PatternNodeKind.Container,
        "OVP-ENTITY-INSPECTOR",
        PatternNodeStatus.Draft,
        "Read-only in-context inspection of an application-owned business/domain entity without losing the originating screen state.",
        new PatternNode("Identity and status", "identity-status", PatternNodeKind.Section, "OVP-ENTITY-INSPECTOR-STATUS", PatternNodeStatus.Draft, "Title, optional reference/subtitle and application-supplied status/type/source pills."),
        new PatternNode("Summary", "summary", PatternNodeKind.Section, "OVP-ENTITY-INSPECTOR-SUMMARY", PatternNodeStatus.Draft, "Compact application-supplied summary content, commonly OVC-METADATA-LIST."),
        new PatternNode("Sections", "sections", PatternNodeKind.Section, "OVP-ENTITY-INSPECTOR-SECTIONS", PatternNodeStatus.Draft, "Application-defined grouped details or governed tabs/sections."),
        new PatternNode("States", "states", PatternNodeKind.Section, "OVP-ENTITY-INSPECTOR-STATES", PatternNodeStatus.Draft, "Governed loading, empty and error presentation."),
        new PatternNode("Actions", "actions", PatternNodeKind.Section, "OVP-ENTITY-INSPECTOR-ACTIONS", PatternNodeStatus.Draft, "Refresh, close and optional application-supplied actions."));

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
        CatalogueManagement,
        SourceConfiguration,
        OperationalDetail,
        EntityInspector,
        Allocation
    ];

    public static PatternNode? FindPattern(string? slug)
        => Patterns.FirstOrDefault(pattern => string.Equals(pattern.Slug, slug, StringComparison.OrdinalIgnoreCase));

    public static PatternNode Resolve(PatternNode root, params string?[] path)
        => PatternCatalog.Resolve(root, path);
}
