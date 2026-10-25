namespace OneVision.UI.Demo;

public static class GovernedPatternCatalog
{
    private static readonly PatternNode SourceConfiguration = new(
        "Source Configuration",
        "source-configuration",
        PatternNodeKind.Container,
        "OVP-SOURCE-CONFIGURATION",
        PatternNodeStatus.Draft,
        "Managed inbound source configuration with Overview, Connection, Profile and Mapping sections.",
        new PatternNode("Overview", "overview", PatternNodeKind.Tab, "OVP-SOURCE-CONFIGURATION-OVERVIEW", PatternNodeStatus.Placeholder, "Source identity, type, enabled state and summary."),
        new PatternNode("Connection", "connection", PatternNodeKind.Tab, "OVP-SOURCE-CONFIGURATION-CONNECTION", PatternNodeStatus.Placeholder, "Connection/source settings supplied by the application."),
        new PatternNode("Profile", "profile", PatternNodeKind.Tab, "OVP-SOURCE-CONFIGURATION-PROFILE", PatternNodeStatus.Placeholder, "Profile, schema discovery and preview of the incoming structure."),
        new PatternNode("Mapping", "mapping", PatternNodeKind.Tab, "OVP-SOURCE-CONFIGURATION-MAPPING", PatternNodeStatus.Placeholder, "Compose OVC-MAPPING-PAIR and optional OVC-MAPPING-RULE without source-specific mapping conventions."));

    private static readonly PatternNode OperationalDetail = new(
        "Operational Detail",
        "operational-detail",
        PatternNodeKind.Container,
        "OVP-OPERATIONAL-DETAIL",
        PatternNodeStatus.Draft,
        "Predominantly read-only status/detail surface for first-class operational or configuration objects, with governed action hierarchy and overlay-based persisted configuration editing.",
        new PatternNode("Identity and status", "identity-status", PatternNodeKind.Section, "OVP-OPERATIONAL-DETAIL-STATUS", PatternNodeStatus.Placeholder, "Title, description, status and optional Enabled toggle."),
        new PatternNode("Health summary", "health-summary", PatternNodeKind.Section, "OVP-OPERATIONAL-DETAIL-HEALTH", PatternNodeStatus.Placeholder, "Compact current health or operational summary."),
        new PatternNode("Latest event", "latest-event", PatternNodeKind.Section, "OVP-OPERATIONAL-DETAIL-LATEST", PatternNodeStatus.Placeholder, "Optional latest run or event summary."),
        new PatternNode("Process stages", "process-stages", PatternNodeKind.Section, "OVP-OPERATIONAL-DETAIL-STAGES", PatternNodeStatus.Placeholder, "Optional OVC-PROCESS-STAGES composition."),
        new PatternNode("Recent activity", "recent-activity", PatternNodeKind.Section, "OVP-OPERATIONAL-DETAIL-ACTIVITY", PatternNodeStatus.Placeholder, "Optional recent activity or history data grid."));

    private static readonly PatternNode Allocation = new(
        "Allocation",
        "allocation",
        PatternNodeKind.Container,
        "OVP-ALLOCATION",
        PatternNodeStatus.Draft,
        "Present the effective ODM allocation for an object, including direct/inherited/none state, allocation source and EAM-owned technical detail.",
        new PatternNode("Effective ODM", "effective-odm", PatternNodeKind.Section, "OVP-ALLOCATION-EFFECTIVE", PatternNodeStatus.Placeholder, "Effective ODM identity and navigation target, or no effective ODM when unallocated."),
        new PatternNode("Assignment mode", "assignment-mode", PatternNodeKind.Section, "OVP-ALLOCATION-MODE", PatternNodeStatus.Placeholder, "Semantic DIRECT, INHERITED or NONE state. Inherited means nearest allocated ancestor wins."),
        new PatternNode("Source / ancestor", "source-ancestor", PatternNodeKind.Section, "OVP-ALLOCATION-SOURCE", PatternNodeStatus.Placeholder, "Nearest allocated ancestor when inherited; direct and none states show the plain No ancestor treatment."),
        new PatternNode("Underlying EAM allocation", "underlying-eam-allocation", PatternNodeKind.Section, "OVP-ALLOCATION-EAM", PatternNodeStatus.Placeholder, "Collapsible technical details for the explicit allocation record owned by EAM."),
        new PatternNode("ODM hierarchy memberships", "odm-hierarchy-memberships", PatternNodeKind.Section, "OVP-ALLOCATION-MEMBERSHIPS", PatternNodeStatus.Placeholder, "Hierarchy memberships for the effective ODM with app-owned viewer deep links."));

    private static readonly HashSet<string> RetiredPatternSlugs = new(StringComparer.OrdinalIgnoreCase)
    {
        "mapping-editor",
        "field-mapping-table",
        "allocation-manager",
        "assignment-picker"
    };

    public static IReadOnlyList<PatternNode> Patterns { get; } = PatternCatalog.Patterns
        .Where(pattern => !RetiredPatternSlugs.Contains(pattern.Slug))
        .Concat([SourceConfiguration, OperationalDetail, Allocation])
        .ToArray();

    public static PatternNode? FindPattern(string? slug)
        => Patterns.FirstOrDefault(pattern => string.Equals(pattern.Slug, slug, StringComparison.OrdinalIgnoreCase));

    public static PatternNode Resolve(PatternNode root, params string?[] path)
        => PatternCatalog.Resolve(root, path);
}
