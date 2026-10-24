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
    private static PatternNode P(string name, string slug, string reference, string description, params PatternNode[] children)
        => new(name, slug, PatternNodeKind.Container, reference, PatternNodeStatus.Draft, description, children);

    private static PatternNode T(string name, string slug, string reference, string description, params PatternNode[] children)
        => new(name, slug, PatternNodeKind.Tab, reference, PatternNodeStatus.Placeholder, description, children);

    private static PatternNode S(string name, string slug, string reference, string description, params PatternNode[] children)
        => new(name, slug, PatternNodeKind.Section, reference, PatternNodeStatus.Placeholder, description, children);

    private static PatternNode N(string name, string slug, string reference, string description)
        => new(name, slug, PatternNodeKind.Pane, reference, PatternNodeStatus.Placeholder, description);

    public static IReadOnlyList<PatternNode> Patterns { get; } =
    [
        P("Connection Manager", "connection-manager", "OVP-CONNECTION-MANAGER", "Shared connection-management container.",
            T("Overview", "overview", "OVP-CONNECTION-MANAGER-OVERVIEW", "General connection identity and status."),
            T("Connection", "connection", "OVP-CONNECTION-MANAGER-CONNECTION", "Shared selector plus type-specific pane.",
                N("SQL", "sql", "OVP-CONNECTION-MANAGER-CONNECTION-SQL", "SQL Server connection controls."),
                N("LDAP", "ldap", "OVP-CONNECTION-MANAGER-CONNECTION-LDAP", "LDAP connection controls."),
                N("JSON", "json", "OVP-CONNECTION-MANAGER-CONNECTION-JSON", "JSON source controls."))),

        P("Schedule Editor", "schedule-editor", "OVP-SCHEDULE-EDITOR", "Shared schedule configuration for one-off and recurring execution.",
            T("Overview", "overview", "OVP-SCHEDULE-EDITOR-OVERVIEW", "Schedule identity and enabled state."),
            T("Timing", "timing", "OVP-SCHEDULE-EDITOR-TIMING", "Timing mode and cadence.",
                N("One-off", "one-off", "OVP-SCHEDULE-EDITOR-TIMING-ONE-OFF", "Single-run date/time controls."),
                N("Recurring", "recurring", "OVP-SCHEDULE-EDITOR-TIMING-RECURRING", "Recurring cadence controls.")),
            T("Advanced", "advanced", "OVP-SCHEDULE-EDITOR-ADVANCED", "Advanced execution options.")),

        P("List Page", "list-page", "OVP-LIST-PAGE", "Standard OneVision searchable, filterable and paginated list.",
            S("Toolbar", "toolbar", "OVP-LIST-PAGE-TOOLBAR", "Search, filters and actions.",
                N("Search", "search", "OVP-LIST-PAGE-TOOLBAR-SEARCH", "Dedicated list search."),
                N("Filters", "filters", "OVP-LIST-PAGE-TOOLBAR-FILTERS", "Context filters."),
                N("Actions", "actions", "OVP-LIST-PAGE-TOOLBAR-ACTIONS", "Primary list actions.")),
            S("Results", "results", "OVP-LIST-PAGE-RESULTS", "Result list and navigation.",
                N("Table", "table", "OVP-LIST-PAGE-RESULTS-TABLE", "Compact data table."),
                N("Pagination", "pagination", "OVP-LIST-PAGE-RESULTS-PAGINATION", "Top-right pagination."),
                N("Empty state", "empty-state", "OVP-LIST-PAGE-RESULTS-EMPTY-STATE", "No-results state."))),

        P("Edit Overlay", "edit-overlay", "OVP-EDIT-OVERLAY", "Standard create/edit overlay.",
            S("Header", "header", "OVP-EDIT-OVERLAY-HEADER", "Title, context and close behaviour."),
            S("Form", "form", "OVP-EDIT-OVERLAY-FORM", "Editable content."),
            S("Validation", "validation", "OVP-EDIT-OVERLAY-VALIDATION", "Validation behaviour."),
            S("Footer", "footer", "OVP-EDIT-OVERLAY-FOOTER", "Cancel/save/destructive actions.")),

        P("Settings Page", "settings-page", "OVP-SETTINGS-PAGE", "Settings workspace with local navigation.",
            S("Navigation", "navigation", "OVP-SETTINGS-PAGE-NAVIGATION", "Settings navigation."),
            S("Content", "content", "OVP-SETTINGS-PAGE-CONTENT", "Selected settings content."),
            S("Save state", "save-state", "OVP-SETTINGS-PAGE-SAVE-STATE", "Dirty/saving/saved states.")),

        P("Activity List", "activity-list", "OVP-ACTIVITY-LIST", "Operational activity and job history.",
            S("Filters", "filters", "OVP-ACTIVITY-LIST-FILTERS", "Activity filters."),
            S("List", "list", "OVP-ACTIVITY-LIST-LIST", "Activity rows and status."),
            S("Details", "details", "OVP-ACTIVITY-LIST-DETAILS", "Selected job detail.")),

        P("Builder Workspace", "builder-workspace", "OVP-BUILDER-WORKSPACE", "Shared shell for complex visual editors.",
            S("Toolbar", "toolbar", "OVP-BUILDER-WORKSPACE-TOOLBAR", "Builder actions."),
            S("Workspace", "workspace", "OVP-BUILDER-WORKSPACE-WORKSPACE", "Primary canvas/work area."),
            S("Inspector", "inspector", "OVP-BUILDER-WORKSPACE-INSPECTOR", "Selected-object properties.")),

        P("Tree Workspace", "tree-workspace", "OVP-TREE-WORKSPACE", "Shared tree browsing/editing workspace.",
            S("Toolbar", "toolbar", "OVP-TREE-WORKSPACE-TOOLBAR", "Tree actions."),
            S("Tree", "tree", "OVP-TREE-WORKSPACE-TREE", "Hierarchy presentation."),
            S("Search", "search", "OVP-TREE-WORKSPACE-SEARCH", "Tree search/reveal."),
            S("Inspector", "inspector", "OVP-TREE-WORKSPACE-INSPECTOR", "Selected-node detail.")),

        P("Integration Card", "integration-card", "OVP-INTEGRATION-CARD", "Compact integration summary.",
            S("Summary", "summary", "OVP-INTEGRATION-CARD-SUMMARY", "Identity and state."),
            S("Configuration", "configuration", "OVP-INTEGRATION-CARD-CONFIGURATION", "Configured endpoint/provider."),
            S("Health", "health", "OVP-INTEGRATION-CARD-HEALTH", "Health status."),
            S("Actions", "actions", "OVP-INTEGRATION-CARD-ACTIONS", "Configure/test actions.")),

        P("Run Overlay", "run-overlay", "OVP-RUN-OVERLAY", "Shared execution overlay.",
            T("Overview", "overview", "OVP-RUN-OVERLAY-OVERVIEW", "Run target summary."),
            T("Options", "options", "OVP-RUN-OVERLAY-OPTIONS", "Execution options."),
            T("Progress", "progress", "OVP-RUN-OVERLAY-PROGRESS", "Queued/running progress."),
            T("Result", "result", "OVP-RUN-OVERLAY-RESULT", "Completion/failure result.")),

        P("Picker Overlay", "picker-overlay", "OVP-PICKER-OVERLAY", "Shared searchable/filterable picker.",
            S("Search", "search", "OVP-PICKER-OVERLAY-SEARCH", "Picker search."),
            S("Filters", "filters", "OVP-PICKER-OVERLAY-FILTERS", "Picker filters."),
            S("Results", "results", "OVP-PICKER-OVERLAY-RESULTS", "Selectable results."),
            S("Selection summary", "selection-summary", "OVP-PICKER-OVERLAY-SELECTION-SUMMARY", "Current selection.")),

        P("Tabbed Editor", "tabbed-editor", "OVP-TABBED-EDITOR", "Reusable tabbed editor shell with dirty state, validation and Save/Cancel.",
            T("Overview", "overview", "OVP-TABBED-EDITOR-OVERVIEW", "Core fields."),
            T("Configuration", "configuration", "OVP-TABBED-EDITOR-CONFIGURATION", "Configuration fields."),
            T("Advanced", "advanced", "OVP-TABBED-EDITOR-ADVANCED", "Advanced settings."),
            S("Footer", "footer", "OVP-TABBED-EDITOR-FOOTER", "Save/cancel state.")),

        P("Detail Workspace", "detail-workspace", "OVP-DETAIL-WORKSPACE", "Header, status/actions and tabbed detail content.",
            S("Header", "header", "OVP-DETAIL-WORKSPACE-HEADER", "Identity/status/actions."),
            T("Overview", "overview", "OVP-DETAIL-WORKSPACE-OVERVIEW", "Overview tab."),
            T("Details", "details", "OVP-DETAIL-WORKSPACE-DETAILS", "Detail tab."),
            T("History", "history", "OVP-DETAIL-WORKSPACE-HISTORY", "History tab.")),

        P("List + Detail Workspace", "list-detail-workspace", "OVP-LIST-DETAIL-WORKSPACE", "Selectable list alongside selected-item detail.",
            S("List", "list", "OVP-LIST-DETAIL-WORKSPACE-LIST", "Master list."),
            S("Detail", "detail", "OVP-LIST-DETAIL-WORKSPACE-DETAIL", "Selected item detail."),
            S("Actions", "actions", "OVP-LIST-DETAIL-WORKSPACE-ACTIONS", "Context actions.")),

        P("Master / Child Editor", "master-child-editor", "OVP-MASTER-CHILD-EDITOR", "Parent editor with a governed child collection.",
            S("Master", "master", "OVP-MASTER-CHILD-EDITOR-MASTER", "Parent record."),
            S("Children", "children", "OVP-MASTER-CHILD-EDITOR-CHILDREN", "Managed child collection."),
            S("Child editor", "child-editor", "OVP-MASTER-CHILD-EDITOR-CHILD-EDITOR", "Create/edit child.")),

        P("Rule Builder", "rule-builder", "OVP-RULE-BUILDER", "Condition rows/groups with readable rule summary.",
            S("Conditions", "conditions", "OVP-RULE-BUILDER-CONDITIONS", "Condition rows."),
            S("Groups", "groups", "OVP-RULE-BUILDER-GROUPS", "AND/OR grouping."),
            S("Summary", "summary", "OVP-RULE-BUILDER-SUMMARY", "Readable rule summary.")),

        P("Mapping Editor", "mapping-editor", "OVP-MAPPING-EDITOR", "Map source fields/constants to governed targets.",
            S("Source", "source", "OVP-MAPPING-EDITOR-SOURCE", "Source fields/constants."),
            S("Mappings", "mappings", "OVP-MAPPING-EDITOR-MAPPINGS", "Mapping rows."),
            S("Preview", "preview", "OVP-MAPPING-EDITOR-PREVIEW", "Resolved preview.")),

        P("Field Mapping Table", "field-mapping-table", "OVP-FIELD-MAPPING-TABLE", "Compact row-based mapping editor.",
            S("Toolbar", "toolbar", "OVP-FIELD-MAPPING-TABLE-TOOLBAR", "Search/add/validate."),
            S("Rows", "rows", "OVP-FIELD-MAPPING-TABLE-ROWS", "Mapping rows."),
            S("Validation", "validation", "OVP-FIELD-MAPPING-TABLE-VALIDATION", "Mapping validation.")),

        P("Ingest / Import Wizard", "ingest-import-wizard", "OVP-INGEST-IMPORT-WIZARD", "Guided source-to-apply import flow.",
            T("Source", "source", "OVP-INGEST-IMPORT-WIZARD-SOURCE", "Choose source."),
            T("Preview", "preview", "OVP-INGEST-IMPORT-WIZARD-PREVIEW", "Profile/preview data."),
            T("Mapping", "mapping", "OVP-INGEST-IMPORT-WIZARD-MAPPING", "Configure mappings."),
            T("Validation", "validation", "OVP-INGEST-IMPORT-WIZARD-VALIDATION", "Validate input."),
            T("Apply", "apply", "OVP-INGEST-IMPORT-WIZARD-APPLY", "Apply/import.")),

        P("Import Result / Rejection Review", "import-result-review", "OVP-IMPORT-RESULT-REVIEW", "Accepted/rejected result review.",
            S("Summary", "summary", "OVP-IMPORT-RESULT-REVIEW-SUMMARY", "Counts and headline result."),
            S("Rejected", "rejected", "OVP-IMPORT-RESULT-REVIEW-REJECTED", "Rejected rows."),
            S("Reasons", "reasons", "OVP-IMPORT-RESULT-REVIEW-REASONS", "Grouped reasons."),
            S("Actions", "actions", "OVP-IMPORT-RESULT-REVIEW-ACTIONS", "Retry/export actions.")),

        P("Allocation Manager", "allocation-manager", "OVP-ALLOCATION-MANAGER", "Browse/manage allocation lifecycle.",
            S("Current allocations", "current", "OVP-ALLOCATION-MANAGER-CURRENT", "Current allocations."),
            S("Add", "add", "OVP-ALLOCATION-MANAGER-ADD", "New allocation."),
            S("Reallocate", "reallocate", "OVP-ALLOCATION-MANAGER-REALLOCATE", "Reallocate flow."),
            S("History", "history", "OVP-ALLOCATION-MANAGER-HISTORY", "Ended/history allocations.")),

        P("Assignment Picker", "assignment-picker", "OVP-ASSIGNMENT-PICKER", "Find/select an eligible target with effective dating.",
            S("Search", "search", "OVP-ASSIGNMENT-PICKER-SEARCH", "Target search."),
            S("Results", "results", "OVP-ASSIGNMENT-PICKER-RESULTS", "Eligible targets."),
            S("Effective dates", "effective-dates", "OVP-ASSIGNMENT-PICKER-EFFECTIVE-DATES", "Effective date options."),
            S("Selection", "selection", "OVP-ASSIGNMENT-PICKER-SELECTION", "Selected target.")),

        P("Hierarchy Picker", "hierarchy-picker", "OVP-HIERARCHY-PICKER", "Searchable hierarchy selection.",
            S("Search", "search", "OVP-HIERARCHY-PICKER-SEARCH", "Hierarchy search."),
            S("Tree", "tree", "OVP-HIERARCHY-PICKER-TREE", "Selectable tree."),
            S("Selection", "selection", "OVP-HIERARCHY-PICKER-SELECTION", "Selection summary.")),

        P("Hierarchy Builder", "hierarchy-builder", "OVP-HIERARCHY-BUILDER", "Editable hierarchy with node actions and inspector.",
            S("Toolbar", "toolbar", "OVP-HIERARCHY-BUILDER-TOOLBAR", "Builder actions."),
            S("Tree", "tree", "OVP-HIERARCHY-BUILDER-TREE", "Editable hierarchy."),
            S("Node inspector", "node-inspector", "OVP-HIERARCHY-BUILDER-NODE-INSPECTOR", "Node properties/actions."),
            S("Reparent", "reparent", "OVP-HIERARCHY-BUILDER-REPARENT", "Drag/drop/reparent state.")),

        P("Dual-Pane Builder", "dual-pane-builder", "OVP-DUAL-PANE-BUILDER", "Source/options pane with live result pane.",
            S("Source", "source", "OVP-DUAL-PANE-BUILDER-SOURCE", "Source/options."),
            S("Preview", "preview", "OVP-DUAL-PANE-BUILDER-PREVIEW", "Live result."),
            S("Actions", "actions", "OVP-DUAL-PANE-BUILDER-ACTIONS", "Save/preview actions.")),

        P("Canvas Builder", "canvas-builder", "OVP-CANVAS-BUILDER", "Toolbar, design canvas and properties inspector.",
            S("Toolbar", "toolbar", "OVP-CANVAS-BUILDER-TOOLBAR", "Canvas actions."),
            S("Palette", "palette", "OVP-CANVAS-BUILDER-PALETTE", "Available objects."),
            S("Canvas", "canvas", "OVP-CANVAS-BUILDER-CANVAS", "Design surface."),
            S("Inspector", "inspector", "OVP-CANVAS-BUILDER-INSPECTOR", "Selected object properties.")),

        P("Filter Builder", "filter-builder", "OVP-FILTER-BUILDER", "Choose dimensions/members and summarise active filters.",
            S("Dimension", "dimension", "OVP-FILTER-BUILDER-DIMENSION", "Dimension selector."),
            S("Members", "members", "OVP-FILTER-BUILDER-MEMBERS", "Member selector."),
            S("Active filters", "active-filters", "OVP-FILTER-BUILDER-ACTIVE-FILTERS", "Filter summary.")),

        P("Metric / Measure Builder", "metric-measure-builder", "OVP-METRIC-MEASURE-BUILDER", "Formula editor with validation and preview.",
            S("Inputs", "inputs", "OVP-METRIC-MEASURE-BUILDER-INPUTS", "Available fields/measures."),
            S("Formula", "formula", "OVP-METRIC-MEASURE-BUILDER-FORMULA", "Expression editor."),
            S("Formatting", "formatting", "OVP-METRIC-MEASURE-BUILDER-FORMATTING", "Display formatting."),
            S("Preview", "preview", "OVP-METRIC-MEASURE-BUILDER-PREVIEW", "Calculated preview.")),

        P("Run Configuration", "run-configuration", "OVP-RUN-CONFIGURATION", "Pre-run scope/options configuration.",
            S("Scope", "scope", "OVP-RUN-CONFIGURATION-SCOPE", "Run scope."),
            S("Options", "options", "OVP-RUN-CONFIGURATION-OPTIONS", "Reprocess/execution options."),
            S("Impact", "impact", "OVP-RUN-CONFIGURATION-IMPACT", "Expected impact summary.")),

        P("Job Progress / Result", "job-progress-result", "OVP-JOB-PROGRESS-RESULT", "Queued/running/completed/failed job state.",
            S("Progress", "progress", "OVP-JOB-PROGRESS-RESULT-PROGRESS", "Progress/counts."),
            S("Result", "result", "OVP-JOB-PROGRESS-RESULT-RESULT", "Completion result."),
            S("Errors", "errors", "OVP-JOB-PROGRESS-RESULT-ERRORS", "Error samples.")),

        P("Activity Detail", "activity-detail", "OVP-ACTIVITY-DETAIL", "Inspect one operational activity.",
            T("Summary", "summary", "OVP-ACTIVITY-DETAIL-SUMMARY", "Status/timing/counts."),
            T("Messages", "messages", "OVP-ACTIVITY-DETAIL-MESSAGES", "Messages/log summary."),
            T("Samples", "samples", "OVP-ACTIVITY-DETAIL-SAMPLES", "Rejected/error samples.")),

        P("Schedule Assignment", "schedule-assignment", "OVP-SCHEDULE-ASSIGNMENT", "Assign objects to an existing/new schedule.",
            S("Schedule", "schedule", "OVP-SCHEDULE-ASSIGNMENT-SCHEDULE", "Schedule selector/create action."),
            S("Available objects", "available", "OVP-SCHEDULE-ASSIGNMENT-AVAILABLE", "Assignable objects."),
            S("Assigned", "assigned", "OVP-SCHEDULE-ASSIGNMENT-ASSIGNED", "Assigned objects.")),

        P("Health / Connectivity Check", "health-connectivity-check", "OVP-HEALTH-CONNECTIVITY-CHECK", "Consistent connection/integration health presentation.",
            S("Configured", "configured", "OVP-HEALTH-CONNECTIVITY-CHECK-CONFIGURED", "Configuration state."),
            S("Checking", "checking", "OVP-HEALTH-CONNECTIVITY-CHECK-CHECKING", "Active test state."),
            S("Healthy", "healthy", "OVP-HEALTH-CONNECTIVITY-CHECK-HEALTHY", "Success state."),
            S("Failed", "failed", "OVP-HEALTH-CONNECTIVITY-CHECK-FAILED", "Failure state.")),

        P("Integration Configuration", "integration-configuration", "OVP-INTEGRATION-CONFIGURATION", "Endpoint/authentication/health configuration.",
            T("Configuration", "configuration", "OVP-INTEGRATION-CONFIGURATION-CONFIGURATION", "Provider and endpoint."),
            T("Authentication", "authentication", "OVP-INTEGRATION-CONFIGURATION-AUTHENTICATION", "Authentication settings."),
            T("Health", "health", "OVP-INTEGRATION-CONFIGURATION-HEALTH", "Validation/health.")),

        P("Permission / Capability Editor", "permission-capability-editor", "OVP-PERMISSION-CAPABILITY-EDITOR", "Govern visible/read-only/edit capabilities.",
            S("Role", "role", "OVP-PERMISSION-CAPABILITY-EDITOR-ROLE", "Role/group selector."),
            S("Capabilities", "capabilities", "OVP-PERMISSION-CAPABILITY-EDITOR-CAPABILITIES", "Capability matrix."),
            S("Effective access", "effective", "OVP-PERMISSION-CAPABILITY-EDITOR-EFFECTIVE", "Effective access summary.")),

        P("Bulk Action Bar", "bulk-action-bar", "OVP-BULK-ACTION-BAR", "Context actions for selected rows/nodes.",
            S("Selection", "selection", "OVP-BULK-ACTION-BAR-SELECTION", "Selection count/summary."),
            S("Actions", "actions", "OVP-BULK-ACTION-BAR-ACTIONS", "Bulk actions."),
            S("Confirmation", "confirmation", "OVP-BULK-ACTION-BAR-CONFIRMATION", "Confirmation state.")),

        P("Export Overlay", "export-overlay", "OVP-EXPORT-OVERLAY", "Export format/options/scope workflow.",
            S("Format", "format", "OVP-EXPORT-OVERLAY-FORMAT", "Export format."),
            S("Scope", "scope", "OVP-EXPORT-OVERLAY-SCOPE", "Export scope."),
            S("Options", "options", "OVP-EXPORT-OVERLAY-OPTIONS", "Format-specific options."),
            S("Result", "result", "OVP-EXPORT-OVERLAY-RESULT", "Generated output.")),

        P("Purge / Destructive Action Review", "purge-review", "OVP-PURGE-REVIEW", "Impact summary and explicit destructive confirmation.",
            S("Impact", "impact", "OVP-PURGE-REVIEW-IMPACT", "Affected data summary."),
            S("Options", "options", "OVP-PURGE-REVIEW-OPTIONS", "Dependent-data options."),
            S("Confirmation", "confirmation", "OVP-PURGE-REVIEW-CONFIRMATION", "Explicit confirmation."),
            S("Result", "result", "OVP-PURGE-REVIEW-RESULT", "Purge result.")),

        P("Compare / Drift Review", "compare-drift-review", "OVP-COMPARE-DRIFT-REVIEW", "Baseline/current comparison with variance/drift.",
            S("Controls", "controls", "OVP-COMPARE-DRIFT-REVIEW-CONTROLS", "Baseline/current/preset controls."),
            S("Summary", "summary", "OVP-COMPARE-DRIFT-REVIEW-SUMMARY", "Headline drift."),
            S("Detail", "detail", "OVP-COMPARE-DRIFT-REVIEW-DETAIL", "Detailed comparison.")),

        P("Search + Select Overlay", "search-select-overlay", "OVP-SEARCH-SELECT-OVERLAY", "Grouped, filterable multi-selection overlay.",
            S("Search", "search", "OVP-SEARCH-SELECT-OVERLAY-SEARCH", "Search."),
            S("Groups", "groups", "OVP-SEARCH-SELECT-OVERLAY-GROUPS", "Result groups."),
            S("Results", "results", "OVP-SEARCH-SELECT-OVERLAY-RESULTS", "Selectable results."),
            S("Selection", "selection", "OVP-SEARCH-SELECT-OVERLAY-SELECTION", "Selection summary.")),

        P("Create-or-Select Existing", "create-or-select-existing", "OVP-CREATE-OR-SELECT-EXISTING", "Choose existing or create new in one flow.",
            T("Existing", "existing", "OVP-CREATE-OR-SELECT-EXISTING-EXISTING", "Search/select existing."),
            T("Create new", "create-new", "OVP-CREATE-OR-SELECT-EXISTING-CREATE-NEW", "Create a new object."),
            S("Confirmation", "confirmation", "OVP-CREATE-OR-SELECT-EXISTING-CONFIRMATION", "Selected/created summary.")),

        P("Inline Collection Editor", "inline-collection-editor", "OVP-INLINE-COLLECTION-EDITOR", "Embedded collection add/edit/delete/reorder.",
            S("Toolbar", "toolbar", "OVP-INLINE-COLLECTION-EDITOR-TOOLBAR", "Add/search controls."),
            S("Rows", "rows", "OVP-INLINE-COLLECTION-EDITOR-ROWS", "Collection rows."),
            S("Inline edit", "inline-edit", "OVP-INLINE-COLLECTION-EDITOR-INLINE-EDIT", "Row editing.")),

        P("Publish Workflow", "publish-workflow", "OVP-PUBLISH-WORKFLOW", "Draft → validate → review → publish lifecycle.",
            T("Draft", "draft", "OVP-PUBLISH-WORKFLOW-DRAFT", "Draft content."),
            T("Validate", "validate", "OVP-PUBLISH-WORKFLOW-VALIDATE", "Blockers/warnings."),
            T("Review", "review", "OVP-PUBLISH-WORKFLOW-REVIEW", "Publish summary."),
            T("Publish", "publish", "OVP-PUBLISH-WORKFLOW-PUBLISH", "Publish/result state.")),

        P("Version / History Viewer", "version-history-viewer", "OVP-VERSION-HISTORY-VIEWER", "Current/historical versions with effective dates.",
            S("Versions", "versions", "OVP-VERSION-HISTORY-VIEWER-VERSIONS", "Version selector/timeline."),
            S("Snapshot", "snapshot", "OVP-VERSION-HISTORY-VIEWER-SNAPSHOT", "Selected version detail."),
            S("Compare", "compare", "OVP-VERSION-HISTORY-VIEWER-COMPARE", "Version comparison.")),

        P("API Explorer / Endpoint Reference", "api-explorer", "OVP-API-EXPLORER", "Endpoint contract and request/response examples.",
            S("Endpoints", "endpoints", "OVP-API-EXPLORER-ENDPOINTS", "Endpoint list."),
            S("Request", "request", "OVP-API-EXPLORER-REQUEST", "Method/path/parameters."),
            S("Response", "response", "OVP-API-EXPLORER-RESPONSE", "Response example."),
            S("Actions", "actions", "OVP-API-EXPLORER-ACTIONS", "Copy/test actions.")),

        P("Empty / First-use Setup", "empty-first-use", "OVP-EMPTY-FIRST-USE", "Consistent no-data/first-use setup state.",
            S("Message", "message", "OVP-EMPTY-FIRST-USE-MESSAGE", "Primary empty-state message."),
            S("Primary action", "primary-action", "OVP-EMPTY-FIRST-USE-PRIMARY-ACTION", "Setup action."),
            S("Help", "help", "OVP-EMPTY-FIRST-USE-HELP", "Secondary guidance/help."))
    ];

    public static PatternNode? FindPattern(string? slug)
        => Patterns.FirstOrDefault(pattern => string.Equals(pattern.Slug, slug, StringComparison.OrdinalIgnoreCase));

    public static PatternNode Resolve(PatternNode root, params string?[] path)
    {
        var current = root;
        foreach (var segment in path.Where(segment => !string.IsNullOrWhiteSpace(segment)))
        {
            var next = current.Children.FirstOrDefault(child => string.Equals(child.Slug, segment, StringComparison.OrdinalIgnoreCase));
            if (next is null) break;
            current = next;
        }
        return current;
    }
}
