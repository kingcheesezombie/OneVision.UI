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

    public static IReadOnlyList<PatternNode> Patterns { get; } = PatternCatalog.Patterns.Concat([SourceConfiguration]).ToArray();

    public static PatternNode? FindPattern(string? slug)
        => Patterns.FirstOrDefault(pattern => string.Equals(pattern.Slug, slug, StringComparison.OrdinalIgnoreCase));

    public static PatternNode Resolve(PatternNode root, params string?[] path)
        => PatternCatalog.Resolve(root, path);
}
