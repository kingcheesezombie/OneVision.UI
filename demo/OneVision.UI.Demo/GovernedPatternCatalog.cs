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
        "Create an effective-dated allocation as one continuous vertical page. The pattern is never split across tabs; it supports either a known target or an optional target-finder state.",
        new PatternNode("Target platform", "target-platform", PatternNodeKind.Section, "OVP-ALLOCATION-TARGET-PLATFORM", PatternNodeStatus.Placeholder, "Finder state only: horizontal EAM, ODM and IDM platform options. Target type is shown for EAM only."),
        new PatternNode("Target", "target", PatternNodeKind.Section, "OVP-ALLOCATION-TARGET", PatternNodeStatus.Placeholder, "Known-target state shows read-only target context. Finder state shows target search, results and selected target."),
        new PatternNode("Source", "source", PatternNodeKind.Section, "OVP-ALLOCATION-SOURCE", PatternNodeStatus.Placeholder, "Source type, search and single-select source results, directly beneath Target on the same page."),
        new PatternNode("Effective dates", "effective-dates", PatternNodeKind.Section, "OVP-ALLOCATION-DATES", PatternNodeStatus.Placeholder, "Always belonged is on by default and disables From/To dates; when off, From is required and To is optional."),
        new PatternNode("Actions", "actions", PatternNodeKind.Section, "OVP-ALLOCATION-ACTIONS", PatternNodeStatus.Placeholder, "Secondary Cancel and primary Allocate actions at the bottom of the same vertical form."));

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
