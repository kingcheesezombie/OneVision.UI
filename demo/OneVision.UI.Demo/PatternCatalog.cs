namespace OneVision.UI.Demo;

public enum PatternNodeKind
{
    Container,
    Tab,
    Section,
    Pane
}

public enum PatternNodeStatus
{
    Placeholder,
    Draft,
    Approved
}

public sealed class PatternNode
{
    public PatternNode(
        string name,
        string slug,
        PatternNodeKind kind,
        string reference,
        PatternNodeStatus status,
        string description,
        params PatternNode[] children)
    {
        Name = name;
        Slug = slug;
        Kind = kind;
        Reference = reference;
        Status = status;
        Description = description;
        Children = children;
    }

    public string Name { get; }
    public string Slug { get; }
    public PatternNodeKind Kind { get; }
    public string Reference { get; }
    public PatternNodeStatus Status { get; }
    public string Description { get; }
    public IReadOnlyList<PatternNode> Children { get; }
}

public static class PatternCatalog
{
    public static IReadOnlyList<PatternNode> Patterns { get; } =
    [
        new(
            "Connection Manager",
            "connection-manager",
            PatternNodeKind.Container,
            "OVP-CONNECTION-MANAGER",
            PatternNodeStatus.Draft,
            "Shared connection-management container. Tabs and nested panes can be reviewed independently or in the full pattern context.",
            new(
                "Overview",
                "overview",
                PatternNodeKind.Tab,
                "OVP-CONNECTION-MANAGER-OVERVIEW",
                PatternNodeStatus.Draft,
                "General connection identity, status and top-level configuration."),
            new(
                "Connection",
                "connection",
                PatternNodeKind.Tab,
                "OVP-CONNECTION-MANAGER-CONNECTION",
                PatternNodeStatus.Draft,
                "Shared connection-type selector plus a type-specific lower pane.",
                new(
                    "SQL",
                    "sql",
                    PatternNodeKind.Pane,
                    "OVP-CONNECTION-MANAGER-CONNECTION-SQL",
                    PatternNodeStatus.Draft,
                    "SQL Server-specific connection controls."),
                new(
                    "LDAP",
                    "ldap",
                    PatternNodeKind.Pane,
                    "OVP-CONNECTION-MANAGER-CONNECTION-LDAP",
                    PatternNodeStatus.Draft,
                    "LDAP-specific connection controls."),
                new(
                    "JSON",
                    "json",
                    PatternNodeKind.Pane,
                    "OVP-CONNECTION-MANAGER-CONNECTION-JSON",
                    PatternNodeStatus.Draft,
                    "JSON source-specific connection controls."))),

        new(
            "Schedule Editor",
            "schedule-editor",
            PatternNodeKind.Container,
            "OVP-SCHEDULE-EDITOR",
            PatternNodeStatus.Placeholder,
            "Shared schedule configuration pattern for one-off and recurring execution.",
            new(
                "Overview",
                "overview",
                PatternNodeKind.Tab,
                "OVP-SCHEDULE-EDITOR-OVERVIEW",
                PatternNodeStatus.Placeholder,
                "Schedule identity, enabled state and summary."),
            new(
                "Timing",
                "timing",
                PatternNodeKind.Tab,
                "OVP-SCHEDULE-EDITOR-TIMING",
                PatternNodeStatus.Placeholder,
                "Timing mode and schedule-specific configuration.",
                new(
                    "One-off",
                    "one-off",
                    PatternNodeKind.Pane,
                    "OVP-SCHEDULE-EDITOR-TIMING-ONE-OFF",
                    PatternNodeStatus.Placeholder,
                    "Single-run date and time controls."),
                new(
                    "Recurring",
                    "recurring",
                    PatternNodeKind.Pane,
                    "OVP-SCHEDULE-EDITOR-TIMING-RECURRING",
                    PatternNodeStatus.Placeholder,
                    "Recurring cadence controls.")),
            new(
                "Advanced",
                "advanced",
                PatternNodeKind.Tab,
                "OVP-SCHEDULE-EDITOR-ADVANCED",
                PatternNodeStatus.Placeholder,
                "Advanced scheduling behaviour and execution options.")),

        new(
            "List Page",
            "list-page",
            PatternNodeKind.Container,
            "OVP-LIST-PAGE",
            PatternNodeStatus.Placeholder,
            "Standard OneVision list-page composition with local search, filters, actions and pagination.",
            new(
                "Toolbar",
                "toolbar",
                PatternNodeKind.Section,
                "OVP-LIST-PAGE-TOOLBAR",
                PatternNodeStatus.Placeholder,
                "Top-of-list controls.",
                new(
                    "Search",
                    "search",
                    PatternNodeKind.Pane,
                    "OVP-LIST-PAGE-TOOLBAR-SEARCH",
                    PatternNodeStatus.Placeholder,
                    "Dedicated list search control."),
                new(
                    "Filters",
                    "filters",
                    PatternNodeKind.Pane,
                    "OVP-LIST-PAGE-TOOLBAR-FILTERS",
                    PatternNodeStatus.Placeholder,
                    "Context-specific list filters."),
                new(
                    "Actions",
                    "actions",
                    PatternNodeKind.Pane,
                    "OVP-LIST-PAGE-TOOLBAR-ACTIONS",
                    PatternNodeStatus.Placeholder,
                    "Primary and secondary list actions.")),
            new(
                "Results",
                "results",
                PatternNodeKind.Section,
                "OVP-LIST-PAGE-RESULTS",
                PatternNodeStatus.Placeholder,
                "List content and navigation states.",
                new(
                    "Table",
                    "table",
                    PatternNodeKind.Pane,
                    "OVP-LIST-PAGE-RESULTS-TABLE",
                    PatternNodeStatus.Placeholder,
                    "Compact result table."),
                new(
                    "Pagination",
                    "pagination",
                    PatternNodeKind.Pane,
                    "OVP-LIST-PAGE-RESULTS-PAGINATION",
                    PatternNodeStatus.Placeholder,
                    "Top-right list pagination."),
                new(
                    "Empty state",
                    "empty-state",
                    PatternNodeKind.Pane,
                    "OVP-LIST-PAGE-RESULTS-EMPTY-STATE",
                    PatternNodeStatus.Placeholder,
                    "No-results and first-use empty states."))),

        new(
            "Edit Overlay",
            "edit-overlay",
            PatternNodeKind.Container,
            "OVP-EDIT-OVERLAY",
            PatternNodeStatus.Placeholder,
            "Standard edit/create overlay composition.",
            new("Header", "header", PatternNodeKind.Section, "OVP-EDIT-OVERLAY-HEADER", PatternNodeStatus.Placeholder, "Title, context and close behaviour."),
            new("Form", "form", PatternNodeKind.Section, "OVP-EDIT-OVERLAY-FORM", PatternNodeStatus.Placeholder, "Primary editable content region."),
            new("Validation", "validation", PatternNodeKind.Section, "OVP-EDIT-OVERLAY-VALIDATION", PatternNodeStatus.Placeholder, "Inline and summary validation behaviour."),
            new("Footer", "footer", PatternNodeKind.Section, "OVP-EDIT-OVERLAY-FOOTER", PatternNodeStatus.Placeholder, "Cancel, save and destructive actions.")),

        new(
            "Settings Page",
            "settings-page",
            PatternNodeKind.Container,
            "OVP-SETTINGS-PAGE",
            PatternNodeStatus.Placeholder,
            "Standard settings workspace with local navigation and governed save behaviour.",
            new("Navigation", "navigation", PatternNodeKind.Section, "OVP-SETTINGS-PAGE-NAVIGATION", PatternNodeStatus.Placeholder, "Settings section navigation."),
            new("Content", "content", PatternNodeKind.Section, "OVP-SETTINGS-PAGE-CONTENT", PatternNodeStatus.Placeholder, "Selected settings section content."),
            new("Save state", "save-state", PatternNodeKind.Section, "OVP-SETTINGS-PAGE-SAVE-STATE", PatternNodeStatus.Placeholder, "Dirty, saving, saved and validation states.")),

        new(
            "Activity List",
            "activity-list",
            PatternNodeKind.Container,
            "OVP-ACTIVITY-LIST",
            PatternNodeStatus.Placeholder,
            "Operational activity/job history list with filters and inspectable details.",
            new("Filters", "filters", PatternNodeKind.Section, "OVP-ACTIVITY-LIST-FILTERS", PatternNodeStatus.Placeholder, "Activity-specific filter controls."),
            new("List", "list", PatternNodeKind.Section, "OVP-ACTIVITY-LIST-LIST", PatternNodeStatus.Placeholder, "Activity rows, status and pagination."),
            new("Details", "details", PatternNodeKind.Section, "OVP-ACTIVITY-LIST-DETAILS", PatternNodeStatus.Placeholder, "Selected run/job detail presentation.")),

        new(
            "Builder Workspace",
            "builder-workspace",
            PatternNodeKind.Container,
            "OVP-BUILDER-WORKSPACE",
            PatternNodeStatus.Placeholder,
            "Shared builder-shell composition for more complex editors.",
            new("Toolbar", "toolbar", PatternNodeKind.Section, "OVP-BUILDER-WORKSPACE-TOOLBAR", PatternNodeStatus.Placeholder, "Builder actions and mode controls."),
            new("Workspace", "workspace", PatternNodeKind.Section, "OVP-BUILDER-WORKSPACE-WORKSPACE", PatternNodeStatus.Placeholder, "Primary builder canvas/work area."),
            new("Inspector", "inspector", PatternNodeKind.Section, "OVP-BUILDER-WORKSPACE-INSPECTOR", PatternNodeStatus.Placeholder, "Selected-object properties and context.")),

        new(
            "Tree Workspace",
            "tree-workspace",
            PatternNodeKind.Container,
            "OVP-TREE-WORKSPACE",
            PatternNodeStatus.Placeholder,
            "Shared tree-based browsing/editing workspace.",
            new("Toolbar", "toolbar", PatternNodeKind.Section, "OVP-TREE-WORKSPACE-TOOLBAR", PatternNodeStatus.Placeholder, "Tree actions and mode controls."),
            new("Tree", "tree", PatternNodeKind.Section, "OVP-TREE-WORKSPACE-TREE", PatternNodeStatus.Placeholder, "Hierarchy/tree presentation."),
            new("Search", "search", PatternNodeKind.Section, "OVP-TREE-WORKSPACE-SEARCH", PatternNodeStatus.Placeholder, "Tree search and reveal behaviour."),
            new("Inspector", "inspector", PatternNodeKind.Section, "OVP-TREE-WORKSPACE-INSPECTOR", PatternNodeStatus.Placeholder, "Selected-node detail and actions.")),

        new(
            "Integration Card",
            "integration-card",
            PatternNodeKind.Container,
            "OVP-INTEGRATION-CARD",
            PatternNodeStatus.Placeholder,
            "Compact integration/configuration summary pattern.",
            new("Summary", "summary", PatternNodeKind.Section, "OVP-INTEGRATION-CARD-SUMMARY", PatternNodeStatus.Placeholder, "Integration identity and state."),
            new("Configuration", "configuration", PatternNodeKind.Section, "OVP-INTEGRATION-CARD-CONFIGURATION", PatternNodeStatus.Placeholder, "Configured endpoint/provider summary."),
            new("Health", "health", PatternNodeKind.Section, "OVP-INTEGRATION-CARD-HEALTH", PatternNodeStatus.Placeholder, "Validation/health status."),
            new("Actions", "actions", PatternNodeKind.Section, "OVP-INTEGRATION-CARD-ACTIONS", PatternNodeStatus.Placeholder, "Configure, test and related actions.")),

        new(
            "Run Overlay",
            "run-overlay",
            PatternNodeKind.Container,
            "OVP-RUN-OVERLAY",
            PatternNodeStatus.Placeholder,
            "Shared run/reprocess execution overlay.",
            new("Overview", "overview", PatternNodeKind.Tab, "OVP-RUN-OVERLAY-OVERVIEW", PatternNodeStatus.Placeholder, "Run target and execution summary."),
            new("Options", "options", PatternNodeKind.Tab, "OVP-RUN-OVERLAY-OPTIONS", PatternNodeStatus.Placeholder, "Execution and reprocessing options."),
            new("Progress", "progress", PatternNodeKind.Tab, "OVP-RUN-OVERLAY-PROGRESS", PatternNodeStatus.Placeholder, "Queued/running progress presentation."),
            new("Result", "result", PatternNodeKind.Tab, "OVP-RUN-OVERLAY-RESULT", PatternNodeStatus.Placeholder, "Completed, warning and failed result states.")),

        new(
            "Picker Overlay",
            "picker-overlay",
            PatternNodeKind.Container,
            "OVP-PICKER-OVERLAY",
            PatternNodeStatus.Placeholder,
            "Shared searchable/filterable picker overlay.",
            new("Search", "search", PatternNodeKind.Section, "OVP-PICKER-OVERLAY-SEARCH", PatternNodeStatus.Placeholder, "Picker-local search control."),
            new("Filters", "filters", PatternNodeKind.Section, "OVP-PICKER-OVERLAY-FILTERS", PatternNodeStatus.Placeholder, "Picker-specific filters."),
            new("Results", "results", PatternNodeKind.Section, "OVP-PICKER-OVERLAY-RESULTS", PatternNodeStatus.Placeholder, "Selectable result list/tree/table."),
            new("Selection summary", "selection-summary", PatternNodeKind.Section, "OVP-PICKER-OVERLAY-SELECTION-SUMMARY", PatternNodeStatus.Placeholder, "Current selection, counts and confirmation actions."))
    ];

    public static PatternNode? FindPattern(string? slug)
        => Patterns.FirstOrDefault(pattern => string.Equals(pattern.Slug, slug, StringComparison.OrdinalIgnoreCase));

    public static PatternNode Resolve(PatternNode root, params string?[] path)
    {
        var current = root;

        foreach (var segment in path.Where(segment => !string.IsNullOrWhiteSpace(segment)))
        {
            var next = current.Children.FirstOrDefault(child =>
                string.Equals(child.Slug, segment, StringComparison.OrdinalIgnoreCase));

            if (next is null)
            {
                break;
            }

            current = next;
        }

        return current;
    }
}
