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

    private static readonly PatternNode ConnectionManagement = new(
        "Connection Management",
        "connection-management",
        PatternNodeKind.Container,
        "OVP-CONNECTION-MANAGEMENT",
        PatternNodeStatus.Draft,
        "Browse managed connections and use the governed Connection Editor in a contextual overlay. The app owns credentials, tests, persistence and permissions.",
        new PatternNode("Connections", "connections", PatternNodeKind.Section, "OVP-CONNECTION-MANAGEMENT-LIST", PatternNodeStatus.Draft, "Compact search/filter list, source type, enabled status and row actions."),
        new PatternNode("Connection editor", "editor", PatternNodeKind.Section, "OVP-CONNECTION-MANAGEMENT-EDITOR", PatternNodeStatus.Draft, "OVConnectionEditor in OVOverlay, with one owner of the Save/Cancel footer."),
        new PatternNode("Operational states", "states", PatternNodeKind.Section, "OVP-CONNECTION-MANAGEMENT-STATES", PatternNodeStatus.Draft, "Loading, failed loading/retry, read-only, testing, save failure and safe return to list."));

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

    private static readonly PatternNode ApiCallBuilder = new(
        "API Call Builder",
        "api-call-builder",
        PatternNodeKind.Container,
        "OVP-API-CALL-BUILDER",
        PatternNodeStatus.Draft,
        "Approachable, domain-neutral configuration of governed API requests without requiring users to construct URLs, query strings or token syntax manually.",
        new PatternNode("Connection", "connection", PatternNodeKind.Section, "OVP-API-CALL-BUILDER-CONNECTION", PatternNodeStatus.Draft, "Select an application-supplied managed connection and show safe authentication context without exposing credentials."),
        new PatternNode("Request", "request", PatternNodeKind.Section, "OVP-API-CALL-BUILDER-REQUEST", PatternNodeStatus.Draft, "Method, resource and metadata-driven request parameters with progressive disclosure for advanced HTTP options."),
        new PatternNode("Request preview", "request-preview", PatternNodeKind.Section, "OVP-API-CALL-BUILDER-PREVIEW", PatternNodeStatus.Draft, "Continuously resolved request summary with raw request available only as secondary technical detail."),
        new PatternNode("Test call", "test-call", PatternNodeKind.Section, "OVP-API-CALL-BUILDER-TEST", PatternNodeStatus.Draft, "Prominent application-owned execution callback with governed loading, success, empty and failure states."),
        new PatternNode("Response", "response", PatternNodeKind.Section, "OVP-API-CALL-BUILDER-RESPONSE", PatternNodeStatus.Draft, "Structured response discovery by default, readable sample data and raw response as a secondary troubleshooting view."));

    private static readonly PatternNode SourceConfiguration = new(
        "Source Configuration",
        "source-configuration",
        PatternNodeKind.Container,
        "OVP-SOURCE-CONFIGURATION",
        PatternNodeStatus.Draft,
        "Governed inbound source configuration for managed and direct sources, with optional Display Value Builder after Mapping and optional Standard Details.",
        new PatternNode("Overview", "overview", PatternNodeKind.Tab, "OVP-SOURCE-CONFIGURATION-OVERVIEW", PatternNodeStatus.Draft, "Source identity, type, enabled state and summary without treating direct sources as incomplete."),
        new PatternNode("Connection", "connection", PatternNodeKind.Tab, "OVP-SOURCE-CONFIGURATION-CONNECTION", PatternNodeStatus.Draft, "Select a managed connection where required, or present the governed No managed connection required satisfied state for direct sources."),
        new PatternNode("Profile", "profile", PatternNodeKind.Tab, "OVP-SOURCE-CONFIGURATION-PROFILE", PatternNodeStatus.Draft, "Define the source, optionally discover and inspect candidates, preview representative data, run profiling and manage current/stale profile state."),
        new PatternNode("Mapping", "mapping", PatternNodeKind.Tab, "OVP-SOURCE-CONFIGURATION-MAPPING", PatternNodeStatus.Draft, "Map only the current profiled output into the feed-defined required contract and, when enabled, explicitly selected application-supplied Standard Details."),
        new PatternNode("Display Value", "display-value", PatternNodeKind.Tab, "OVP-SOURCE-CONFIGURATION-DISPLAY-VALUE", PatternNodeStatus.Draft, "Optional fifth peer tab: compose successfully mapped target fields into a readable value."));

    private static readonly PatternNode ProgressiveConfigurationBuilder = new(
        "Progressive Configuration Builder",
        "progressive-configuration-builder",
        PatternNodeKind.Container,
        "OVP-PROGRESSIVE-CONFIGURATION-BUILDER",
        PatternNodeStatus.Draft,
        "Single-workspace vertical configuration of dependent semantic choices using governed step states, progressive disclosure, wildcard semantics and a live human-readable preview.",
        new PatternNode("Steps", "steps", PatternNodeKind.Section, "OVP-PROGRESSIVE-CONFIGURATION-BUILDER-STEPS", PatternNodeStatus.Draft, "Ordered required, optional and conditional steps with current, completed, pending, disabled and validation states."),
        new PatternNode("Relationships", "relationships", PatternNodeKind.Section, "OVP-PROGRESSIVE-CONFIGURATION-BUILDER-RELATIONSHIPS", PatternNodeStatus.Draft, "Optional semantic dividers between related groups without turning the flow into a wizard."),
        new PatternNode("Preview", "preview", PatternNodeKind.Section, "OVP-PROGRESSIVE-CONFIGURATION-BUILDER-PREVIEW", PatternNodeStatus.Draft, "Live application-supplied human-readable summary with incomplete-state presentation."),
        new PatternNode("Actions", "actions", PatternNodeKind.Section, "OVP-PROGRESSIVE-CONFIGURATION-BUILDER-ACTIONS", PatternNodeStatus.Draft, "Standard validation messaging plus Save and Cancel action treatment."));

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
        "Create an effective-dated allocation in a compact, target-first vertical editor with constrained selectors, completed selection and conditional dates, implemented by OVAllocation.",
        new PatternNode("Target platform", "target-platform", PatternNodeKind.Section, "OVP-ALLOCATION-TARGET-PLATFORM", PatternNodeStatus.Draft, "Finder state only: show the application-permitted Provider when multiple are available; show an optional app-supplied Target Type for any Provider."),
        new PatternNode("Target", "target", PatternNodeKind.Section, "OVP-ALLOCATION-TARGET", PatternNodeStatus.Draft, "Compact known-target identity or finder search/results and selected target with Change action."),
        new PatternNode("Source", "source", PatternNodeKind.Section, "OVP-ALLOCATION-SOURCE", PatternNodeStatus.Draft, "Optional editable/fixed source system and type, prominent search, eligible results or selected source summary."),
        new PatternNode("Effective dates", "effective-dates", PatternNodeKind.Section, "OVP-ALLOCATION-DATES", PatternNodeStatus.Draft, "Always belonged defaults on and collapses From/To dates; switching off reveals them."),
        new PatternNode("Actions", "actions", PatternNodeKind.Section, "OVP-ALLOCATION-ACTIONS", PatternNodeStatus.Draft, "Cancel and Allocate at the bottom of the same vertical form."));

    public static IReadOnlyList<PatternNode> Patterns { get; } =
    [
        ListPage,
        ConnectionManagement,
        CatalogueManagement,
        ApiCallBuilder,
        SourceConfiguration,
        ProgressiveConfigurationBuilder,
        OperationalDetail,
        EntityInspector,
        Allocation
    ];

    public static PatternNode? FindPattern(string? slug)
        => Patterns.FirstOrDefault(pattern => string.Equals(pattern.Slug, slug, StringComparison.OrdinalIgnoreCase));

    public static PatternNode Resolve(PatternNode root, params string?[] path)
        => PatternCatalog.Resolve(root, path);
}
