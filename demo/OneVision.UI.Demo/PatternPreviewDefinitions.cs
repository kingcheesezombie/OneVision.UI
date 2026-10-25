namespace OneVision.UI.Demo;

public enum PatternPreviewKind
{
    Form,
    List,
    Workspace,
    Wizard,
    Tree,
    Overlay,
    Builder,
    Review,
    Status,
    Detail,
    Picker,
    Api,
    Empty
}

public sealed record PatternPreviewDefinition(
    string Title,
    PatternPreviewKind Kind,
    string Summary,
    string[] Fields,
    string[] Items,
    string PrimaryAction,
    string SecondaryAction = "Cancel",
    string AsideTitle = "Details");

public static class PatternPreviewDefinitions
{
    public static PatternPreviewDefinition For(string slug) => slug switch
    {
        "schedule-editor" => D("Schedule Editor", PatternPreviewKind.Form, "Configure schedule identity, timing and execution behaviour.", ["Schedule name", "Timing mode", "Start date", "Time"], ["Overview", "Timing", "Advanced"], "Save schedule"),
        "list-page" => D("List Page", PatternPreviewKind.List, "Standard searchable, filterable and paginated OneVision list.", ["Search", "Status", "Type"], ["Community Nursing", "Vaccination Service", "Virtual Ward"], "Add new", "Export"),
        "edit-overlay" => D("Edit Overlay", PatternPreviewKind.Overlay, "Standard create/edit overlay with validation and governed actions.", ["Name", "Type", "Description", "Enabled"], ["General", "Configuration"], "Save"),
        "settings-page" => D("Settings Page", PatternPreviewKind.Workspace, "Local settings navigation with a focused configuration surface.", ["Display name", "Endpoint", "Enabled"], ["General", "Integrations", "API", "Security"], "Save changes"),
        "activity-list" => D("Activity List", PatternPreviewKind.List, "Operational history with status, timing and inspectable job details.", ["Search activity", "Status", "Date range"], ["EAM cache sync", "KPI recalculate", "Hierarchy publish"], "Refresh", "Export"),
        "builder-workspace" => D("Builder Workspace", PatternPreviewKind.Builder, "Shared shell for complex visual editors with toolbar, work area and inspector.", ["Name", "Mode"], ["Source", "Transform", "Output"], "Save builder"),
        "tree-workspace" => D("Tree Workspace", PatternPreviewKind.Tree, "Tree browsing/editing with search and selected-node inspector.", ["Search tree"], ["Birmingham", "East", "Central", "South"], "Add node", "Refresh"),
        "integration-card" => D("Integration Card", PatternPreviewKind.Status, "Compact integration summary combining configuration, health and actions.", ["Endpoint", "Authentication"], ["Configured", "Healthy", "Last checked 2 mins ago"], "Configure", "Test"),
        "run-overlay" => D("Run Overlay", PatternPreviewKind.Overlay, "Execution overlay covering options, progress and final result.", ["Run scope", "Reprocess"], ["Overview", "Options", "Progress", "Result"], "Run now"),
        "picker-overlay" => D("Picker Overlay", PatternPreviewKind.Picker, "Searchable and filterable selection overlay with selection summary.", ["Search", "Type", "Status"], ["Business Area A", "Business Area B", "Business Area C"], "Select"),
        "tabbed-editor" => D("Tabbed Editor", PatternPreviewKind.Form, "Reusable editor container with tabs, dirty state, validation and Save/Cancel.", ["Name", "Description", "Enabled"], ["Overview", "Configuration", "Advanced"], "Save"),
        "detail-workspace" => D("Detail Workspace", PatternPreviewKind.Detail, "Header, status/actions and tabbed detail content for a selected object.", ["Owner", "Created", "Updated"], ["Overview", "Details", "History"], "Edit", "Close"),
        "list-detail-workspace" => D("List + Detail Workspace", PatternPreviewKind.Workspace, "Selectable master list with the selected record shown in a detail pane.", ["Search", "Status"], ["Record 001", "Record 002", "Record 003"], "Add record"),
        "master-child-editor" => D("Master / Child Editor", PatternPreviewKind.Workspace, "Edit a parent object alongside its governed child collection.", ["Definition name", "Enabled"], ["Item 1", "Item 2", "Item 3"], "Save", "Add item", "Items"),
        "rule-builder" => D("Rule Builder", PatternPreviewKind.Builder, "Compose readable conditions from fields, operators and values.", ["Field", "Operator", "Value"], ["AND", "Condition 1", "Condition 2"], "Save rule", "Add condition", "Rule summary"),
        "mapping-editor" => D("Mapping Editor", PatternPreviewKind.Builder, "Map source fields or constants onto governed target values.", ["Source field", "Target field", "Constant"], ["Person ID → Identifier", "DOB → Date of birth", "Team → Business area"], "Save mappings", "Add mapping", "Preview"),
        "field-mapping-table" => D("Field Mapping Table", PatternPreviewKind.List, "Compact row-based source-to-target mapping editor.", ["Search mappings"], ["Source ID", "Description", "Start Date"], "Add mapping", "Validate"),
        "ingest-import-wizard" => D("Ingest / Import Wizard", PatternPreviewKind.Wizard, "Guided source, preview, mapping, validation and apply workflow.", ["Connection", "Source", "Profile rows"], ["Source", "Preview", "Mapping", "Validation", "Apply"], "Continue", "Back"),
        "import-result-review" => D("Import Result / Rejection Review", PatternPreviewKind.Review, "Accepted/rejected counts with reasons, samples and retry/export actions.", ["Run", "Result filter"], ["1,248 accepted", "14 rejected", "3 warning types"], "Reprocess", "Export rejected"),
        "allocation" => D("Allocation", PatternPreviewKind.Form, "Allocate a selected source object to the initiated target, with source-type search/selection and effective dating.", ["Target", "Source type", "Source search", "Selected source", "Always belonged", "From date", "To date"], ["Target", "Source", "Effective dates"], "Allocate"),
        "allocation-manager" => D("Allocation Manager", PatternPreviewKind.Workspace, "Browse current allocations and manage add, reallocate and end lifecycle actions.", ["Search allocations", "Target", "Status"], ["ODM: Community", "ODM: Specialist", "IDM: Workforce"], "Add allocation", "Reallocate", "Allocations"),
        "assignment-picker" => D("Assignment Picker", PatternPreviewKind.Picker, "Find and select an eligible target with optional effective dating.", ["Search target", "Type", "From date"], ["North Birmingham", "Central Birmingham", "South Birmingham"], "Assign"),
        "hierarchy-picker" => D("Hierarchy Picker", PatternPreviewKind.Tree, "Searchable hierarchy selection with current selection summary.", ["Search hierarchy"], ["Birmingham", "Locality A", "Team 1", "Team 2"], "Use selection", "Clear"),
        "hierarchy-builder" => D("Hierarchy Builder", PatternPreviewKind.Tree, "Editable hierarchy with node actions, drag/reparent affordance and inspector.", ["Search nodes", "Node name"], ["Root", "Division", "Service", "Team"], "Add node", "Publish", "Node inspector"),
        "dual-pane-builder" => D("Dual-Pane Builder", PatternPreviewKind.Builder, "Source/options on the left with a live result or canvas on the right.", ["Source", "Mode", "Filter"], ["Input", "Configuration", "Live preview"], "Save", "Run preview", "Preview"),
        "canvas-builder" => D("Canvas Builder", PatternPreviewKind.Builder, "Toolbar, design canvas, selected object and properties inspector.", ["Canvas name", "Layout"], ["KPI card", "Trend", "Breakdown"], "Save canvas", "Export", "Inspector"),
        "filter-builder" => D("Filter Builder", PatternPreviewKind.Builder, "Choose analytical dimensions and members while summarising active filters.", ["Dimension", "Members", "Operator"], ["Ethnicity", "Age band", "Gender"], "Apply filters", "Clear", "Active filters"),
        "metric-measure-builder" => D("Metric / Measure Builder", PatternPreviewKind.Builder, "Formula editor with available inputs, validation, formatting and preview.", ["Name", "Formula", "Format"], ["KPI Score", "Population", "Area"], "Save measure", "Validate", "Preview"),
        "run-configuration" => D("Run Configuration", PatternPreviewKind.Form, "Pre-run scope and reprocessing options before execution begins.", ["Scope", "Period", "Reprocess existing", "Notify on completion"], ["Configuration", "Impact"], "Start run"),
        "job-progress-result" => D("Job Progress / Result", PatternPreviewKind.Status, "Queued/running/completed/failed states with counts and elapsed time.", ["Job", "Started"], ["2,400 read", "2,386 accepted", "14 rejected"], "View details", "Close"),
        "activity-detail" => D("Activity Detail", PatternPreviewKind.Detail, "Inspect one operational activity with timings, counts, messages and linked object.", ["Started", "Completed", "Duration", "Triggered by"], ["Summary", "Messages", "Rejected samples"], "Open object", "Close"),
        "schedule-assignment" => D("Schedule Assignment", PatternPreviewKind.Workspace, "Attach one or more objects to an existing or newly created schedule.", ["Schedule", "Enabled", "Next run"], ["Feed A", "Feed B", "Feed C"], "Assign", "Create schedule", "Assigned objects"),
        "health-connectivity-check" => D("Health / Connectivity Check", PatternPreviewKind.Status, "Consistent configured, checking, healthy, degraded and failed presentation.", ["Endpoint", "Last checked"], ["Configuration valid", "Endpoint reachable", "Authentication successful"], "Test now", "Configure"),
        "integration-configuration" => D("Integration Configuration", PatternPreviewKind.Form, "Endpoint, authentication, validation and health configuration for an external integration.", ["Provider", "Endpoint", "Authentication", "Enabled"], ["Configuration", "Health"], "Save integration", "Test"),
        "permission-capability-editor" => D("Permission / Capability Editor", PatternPreviewKind.List, "Govern visible/read-only/edit capabilities by role or group.", ["Search capability", "Role"], ["View", "Edit", "Delete", "Publish"], "Save permissions", "Reset"),
        "bulk-action-bar" => D("Bulk Action Bar", PatternPreviewKind.List, "Contextual actions shown when one or more list/tree rows are selected.", ["3 selected"], ["Record A", "Record B", "Record C"], "Apply action", "Clear selection"),
        "export-overlay" => D("Export Overlay", PatternPreviewKind.Overlay, "Choose export format, scope and options before generating an output.", ["Format", "Scope", "Include metadata"], ["Image", "HTML", "Excel"], "Export"),
        "purge-review" => D("Purge / Destructive Action Review", PatternPreviewKind.Review, "Impact summary and explicit confirmation for destructive operations.", ["Object type", "Scope", "Confirmation"], ["248 entities", "312 allocations", "12 history groups"], "Purge", "Cancel"),
        "compare-drift-review" => D("Compare / Drift Review", PatternPreviewKind.Review, "Baseline/current comparison with variance, presets and detailed drift.", ["Baseline", "Current", "Preset"], ["Average drift 4.8%", "12 increased", "7 decreased"], "Export", "Refresh"),
        "search-select-overlay" => D("Search + Select Overlay", PatternPreviewKind.Picker, "Rich grouped, filterable multi-selection with confirmation.", ["Search", "Group", "Status"], ["Team A", "Team B", "Team C"], "Add selected"),
        "create-or-select-existing" => D("Create-or-Select Existing", PatternPreviewKind.Overlay, "Two-mode flow for reusing an existing object or creating a new one.", ["Search existing", "New name", "Type"], ["Existing", "Create new"], "Continue"),
        "inline-collection-editor" => D("Inline Collection Editor", PatternPreviewKind.List, "Compact embedded collection with add, edit, delete and reorder.", ["Search items"], ["Item A", "Item B", "Item C"], "Add item", "Reorder"),
        "publish-workflow" => D("Publish Workflow", PatternPreviewKind.Wizard, "Draft, validate and publish lifecycle with blockers and warnings.", ["Version", "Publish note"], ["Draft", "Validate", "Review", "Publish"], "Publish", "Back"),
        "version-history-viewer" => D("Version / History Viewer", PatternPreviewKind.Detail, "Browse current and historical versions with effective dates and comparison context.", ["Version", "Effective date", "Changed by"], ["Current", "v3", "v2", "v1"], "Compare", "Close"),
        "api-explorer" => D("API Explorer / Endpoint Reference", PatternPreviewKind.Api, "Browsable endpoint contract with request/response examples and copy/test actions.", ["Method", "Endpoint", "Parameters"], ["GET", "/api/v1/entities/{id}", "200 OK"], "Copy request", "Try example"),
        "empty-first-use" => D("Empty / First-use Setup", PatternPreviewKind.Empty, "Consistent first-use and no-data state with a clear primary setup action.", [], ["No configuration exists yet"], "Set up now", "Learn more"),
        _ => D("Pattern preview", PatternPreviewKind.Form, "First-pass pattern preview.", ["Name", "Status"], ["Overview"], "Save")
    };

    private static PatternPreviewDefinition D(
        string title,
        PatternPreviewKind kind,
        string summary,
        string[] fields,
        string[] items,
        string primaryAction,
        string secondaryAction = "Cancel",
        string asideTitle = "Details")
        => new(title, kind, summary, fields, items, primaryAction, secondaryAction, asideTitle);
}
