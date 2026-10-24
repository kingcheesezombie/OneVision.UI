namespace OneVision.UI.Demo;

public enum ComponentPreviewKind
{
    Action,
    TextInput,
    Selection,
    Toggle,
    DateTime,
    Pill,
    Data,
    Navigation,
    Feedback,
    Container,
    Overlay,
    Display,
    Shell,
    StructuredTree
}

public enum ComponentKind
{
    Primitive,
    CompositeComponent,
    SupportingControl,
    OvpRegion,
    SpecialisedComponent
}

public sealed record ComponentDefinition(
    string Name,
    string Slug,
    string Group,
    string Reference,
    string Description,
    string UseWhen,
    ComponentPreviewKind PreviewKind,
    ComponentKind Kind,
    string ImplementationBasis,
    string[] States,
    string[] Behaviours,
    string[] Tags);

public static class ComponentCatalog
{
    private static ComponentDefinition C(
        string name,
        string slug,
        string group,
        string reference,
        string description,
        string useWhen,
        ComponentPreviewKind previewKind,
        ComponentKind kind,
        string implementationBasis,
        string[] states,
        string[] behaviours,
        params string[] tags)
        => new(name, slug, group, reference, description, useWhen, previewKind, kind, implementationBasis, states, behaviours, tags);

    public static IReadOnlyList<string> GroupOrder { get; } =
    [
        "Actions",
        "Text & input",
        "Selection",
        "Dates & times",
        "Status & labels",
        "Data & lists",
        "Navigation",
        "Feedback",
        "Containers",
        "Overlays",
        "Display",
        "App shell",
        "Structured data"
    ];

    public static IReadOnlyList<ComponentDefinition> Components { get; } =
    [
        C("Button", "button", "Actions", "OVC-BUTTON", "Explicit semantic action with Primary, Secondary and Subtle emphasis.", "Use for an action with a clear verb.", ComponentPreviewKind.Action, ComponentKind.Primitive, "OVButton over MudButton",
            ["Default", "Disabled", "Loading"],
            ["Applications choose semantic emphasis rather than MudBlazor Filled/Outlined/Text variants.", "Loading prevents repeat invocation and preserves the button footprint.", "Use semantic OneVision icons rather than implementation icon strings."],
            "action", "primary", "secondary", "subtle"),
        C("Icon Button", "icon-button", "Actions", "OVC-ICON-BUTTON", "Compact semantic icon-only action with mandatory accessible label.", "Use for established actions in compact toolbars and rows.", ComponentPreviewKind.Action, ComponentKind.Primitive, "OVIconButton with OneVision semantic icons",
            ["Default", "Disabled", "Loading", "Destructive"],
            ["Accessible label and tooltip are mandatory.", "Use only established semantic icons.", "Destructive tone is governed rather than app-coloured."],
            "icon", "toolbar", "row action"),
        C("Overflow Menu", "overflow-menu", "Actions", "OVC-OVERFLOW-MENU", "Vertical three-dot secondary action menu for rows and cards.", "Use for secondary, infrequent or destructive row/card actions.", ComponentPreviewKind.Action, ComponentKind.CompositeComponent, "MudMenu composition",
            ["Closed", "Open", "Disabled item", "Destructive item"],
            ["Use a vertical ellipsis.", "One visible primary row action at most; secondary and destructive actions normally live here.", "Opening the menu must not select the underlying row."],
            "menu", "vertical dots", "delete"),

        C("Text Field", "text-field", "Text & input", "OVC-TEXT-FIELD", "Single-line text entry.", "Use for short free-text values.", ComponentPreviewKind.TextInput, ComponentKind.Primitive, "Governed MudTextField wrapper",
            ["Default", "Disabled", "Read-only", "Validation error"],
            ["Labels remain visible when populated.", "Read-only values remain selectable/copyable.", "Field validation stays adjacent to the field."],
            "input", "form"),
        C("Text Area", "text-area", "Text & input", "OVC-TEXT-AREA", "Multi-line text entry.", "Use for notes, descriptions, queries and longer text.", ComponentPreviewKind.TextInput, ComponentKind.Primitive, "Governed MudTextField multiline wrapper",
            ["Default", "Disabled", "Read-only", "Validation error"],
            ["Grow vertically within sensible limits.", "Do not use for structured or coded values."],
            "notes", "description"),
        C("Number Field", "number-field", "Text & input", "OVC-NUMBER-FIELD", "Numeric entry with governed parsing and validation.", "Use when the domain value is numeric.", ComponentPreviewKind.TextInput, ComponentKind.Primitive, "Governed MudNumericField wrapper",
            ["Default", "Disabled", "Read-only", "Validation error"],
            ["Reject invalid numeric input before submit.", "Display formatting is separate from the stored numeric value."],
            "numeric", "measure"),
        C("Search Box", "search-box", "Text & input", "OVC-SEARCH-BOX", "Reusable local search behaviour normally consumed through an owning control.", "Use internally for searchable lists, trees, pickers and workspaces.", ComponentPreviewKind.TextInput, ComponentKind.SupportingControl, "Governed MudTextField search composition",
            ["Default", "Disabled"],
            ["Search is local to its owning component/workspace by default.", "Clear is available when text is present.", "Owning controls expose Searchable rather than requiring apps to compose a search box manually."],
            "search", "supporting"),

        C("Select", "select", "Selection", "OVC-SELECT", "Single selection from a bounded scannable option set.", "Use when all choices can reasonably be presented together.", ComponentPreviewKind.Selection, ComponentKind.Primitive, "Governed MudSelect wrapper",
            ["Default", "Disabled", "Read-only", "Validation error"],
            ["Current selection remains visible when closed.", "Use Autocomplete for large or dynamic sets."],
            "dropdown", "single"),
        C("Multi-select", "multi-select", "Selection", "OVC-MULTI-SELECT", "Flat multiple selection with search, removable selections and optional organisational groups.", "Use when several independent values may be selected from a governed set.", ComponentPreviewKind.Selection, ComponentKind.CompositeComponent, "OVMultiSelect using MudList multi-selection",
            ["Default", "Disabled", "Read-only"],
            ["Groups are organisational and never selectable/cascading.", "Selected values are removable without reopening the list.", "True hierarchy belongs in OVC-TREE."],
            "multiple", "grouped", "search"),
        C("Autocomplete", "autocomplete", "Selection", "OVC-AUTOCOMPLETE", "Search-first single selection for large or dynamic option sets.", "Use where scanning a full dropdown is impractical.", ComponentPreviewKind.Selection, ComponentKind.Primitive, "Governed MudAutocomplete wrapper",
            ["Default", "Loading", "No results", "Disabled"],
            ["Free text is off by default.", "Keyboard navigation is required.", "Remote loading and no-results states are explicit."],
            "lookup", "search"),
        C("Checkbox", "checkbox", "Selection", "OVC-CHECKBOX", "Independent boolean or inclusion choice.", "Use for independent selections and row picking.", ComponentPreviewKind.Toggle, ComponentKind.Primitive, "Governed MudCheckBox wrapper",
            ["Unchecked", "Checked", "Indeterminate", "Disabled"],
            ["Indeterminate is reserved for mixed child state.", "Do not use where the choice changes application mode immediately."],
            "boolean", "multi"),
        C("Radio Group", "radio-group", "Selection", "OVC-RADIO-GROUP", "Visible mutually-exclusive choices.", "Use for a small set of exclusive choices that benefits from being visible.", ComponentPreviewKind.Selection, ComponentKind.CompositeComponent, "Governed MudRadioGroup composition",
            ["Default", "Disabled option", "Validation error"],
            ["Exactly one option may be active when selection is required.", "Prefer Select for larger sets."],
            "exclusive", "choice"),
        C("Toggle", "toggle", "Selection", "OVC-TOGGLE", "Immediate binary enabled/disabled setting.", "Use for settings that take effect as a binary capability state.", ComponentPreviewKind.Toggle, ComponentKind.Primitive, "Governed MudSwitch wrapper",
            ["Off", "On", "Disabled", "Saving"],
            ["The label describes the setting, not the action.", "Enabled is the normal user-facing switch where lifecycle Active is internal."],
            "enabled", "switch"),

        C("Date Picker", "date-picker", "Dates & times", "OVC-DATE-PICKER", "Single date selection.", "Use for effective dates and other date-only values.", ComponentPreviewKind.DateTime, ComponentKind.Primitive, "Governed MudDatePicker wrapper",
            ["Default", "Disabled", "Read-only", "Validation error"],
            ["Display dates in DD/MM/YYYY.", "Do not imply time-zone semantics for date-only values."],
            "date", "effective"),
        C("Date Range", "date-range", "Dates & times", "OVC-DATE-RANGE", "From/to date selection.", "Use for bounded effective periods and date filtering.", ComponentPreviewKind.DateTime, ComponentKind.Primitive, "Governed MudDateRangePicker wrapper",
            ["Default", "Open-ended", "Invalid range", "Disabled"],
            ["To cannot precede From.", "Open-ended ranges are explicit rather than represented by a fake maximum date."],
            "range", "from", "to"),
        C("Time Picker", "time-picker", "Dates & times", "OVC-TIME-PICKER", "Time-of-day selection.", "Use for schedule times and other local clock values.", ComponentPreviewKind.DateTime, ComponentKind.Primitive, "Governed MudTimePicker wrapper",
            ["Default", "Disabled", "Validation error"],
            ["Display 24-hour time.", "Time-zone context belongs to the surrounding pattern when relevant."],
            "time", "schedule"),

        C("Pill", "pill", "Status & labels", "OVC-PILL", "Unified compact semantic label for status, type, source, filter or count roles.", "Use where dense categorical/status/filter/count context needs quick scanning.", ComponentPreviewKind.Pill, ComponentKind.Primitive, "OVPill",
            ["Status", "Type", "Source", "Filter", "Count"],
            ["Role controls permitted behaviour.", "Status uses semantic tone; Type and Source are categorical.", "Only Filter may be removable or locked.", "Count remains numeric/contextual rather than a metric."],
            "status", "type", "source", "filter", "count"),

        C("Data Grid", "data-grid", "Data & lists", "OVC-DATA-GRID", "Governed compact tabular results control.", "Use for structured tabular results with sorting, selection or row actions.", ComponentPreviewKind.Data, ComponentKind.CompositeComponent, "MudTable/MudDataGrid selected by OneVision capability",
            ["Default", "Loading", "Empty", "Read-only"],
            ["The grid owns rows, columns, sorting, selection and row-action slots.", "Search, filters, pagination placement and bulk actions belong to the surrounding list composition.", "Inline editing is opt-in rather than the default."],
            "table", "grid", "selection"),
        C("Pagination", "pagination", "Data & lists", "OVC-PAGINATION", "Internal paging behaviour for governed list/grid compositions.", "Use through a list/grid composition rather than placing it independently.", ComponentPreviewKind.Navigation, ComponentKind.SupportingControl, "Integrated list/grid paging",
            ["Default", "Single page"],
            ["Default page size is 25.", "First-class list placement is top-right.", "Changing page preserves search/filter state."],
            "page", "25", "supporting"),
        C("Sortable Header", "sortable-header", "Data & lists", "OVC-SORTABLE-HEADER", "Internal sortable column-header behaviour.", "Use through a governed grid column rather than as a free-standing control.", ComponentPreviewKind.Data, ComponentKind.SupportingControl, "MudTableSortLabel / DataGrid column sort",
            ["Unsorted", "Ascending", "Descending", "Disabled"],
            ["Current sort direction is visually and accessibly explicit.", "Sorting is configured on the governed column definition."],
            "sort", "column", "supporting"),
        C("Empty State", "empty-state", "Data & lists", "OVC-EMPTY-STATE", "Valid absence-of-content presentation with an optional next action.", "Use for first-use, no-data, no-results or permission-limited absence.", ComponentPreviewKind.Feedback, ComponentKind.Primitive, "OVEmptyState",
            ["First use", "No data", "No results", "Permission limited"],
            ["Errors are not empty states.", "The parent supplies domain wording and optional action.", "Manager remains restrained; Experience may use richer optional visuals."],
            "empty", "no results", "first use"),

        C("Tabs", "tabs", "Navigation", "OVC-TABS", "Peer-view navigation within one parent context.", "Use for a small number of peer views within the same object/workflow.", ComponentPreviewKind.Navigation, ComponentKind.CompositeComponent, "Governed MudTabs wrapper",
            ["Default", "Disabled", "Overflow"],
            ["Tabs are peer views, not wizard steps.", "Overflow uses a governed treatment instead of endlessly shrinking labels."],
            "tab", "view"),
        C("Breadcrumb", "breadcrumb", "Navigation", "OVC-BREADCRUMB", "Hierarchy/location trail.", "Use when users drill through nested scopes and need a route back.", ComponentPreviewKind.Navigation, ComponentKind.CompositeComponent, "Governed MudBreadcrumbs wrapper",
            ["Default", "Long path"],
            ["Root and current location always remain visible.", "Intermediate ancestors may collapse on long paths.", "Current location is not a navigation target."],
            "path", "location"),
        C("Side Navigation", "side-navigation", "Navigation", "OVC-SIDE-NAV", "Persistent local workspace navigation with shallow grouping.", "Use when several persistent sections share the same workspace.", ComponentPreviewKind.Navigation, ComponentKind.CompositeComponent, "Governed MudNavMenu composition",
            ["Default", "Collapsed group", "Disabled item"],
            ["All labels are left-aligned.", "Optional icons occupy a fixed leading column so labels align.", "Grouping is shallow; true hierarchy belongs in Tree.", "Selected/hover backgrounds retain visible separation."],
            "settings", "workspace", "left aligned"),
        C("Tree", "tree", "Navigation", "OVC-TREE", "Generic searchable hierarchical navigation/selection control.", "Use for true parent/child structures.", ComponentPreviewKind.Navigation, ComponentKind.CompositeComponent, "OVTree over MudTreeView",
            ["Default", "Single", "Multiple", "LeafOnly", "Read-only"],
            ["Default is MultipleHierarchical with search enabled.", "Parent selection cascades to selectable descendants and mixed children produce indeterminate parent state.", "Expansion and focus remain distinct from checkbox selection.", "Search reveals matching descendants and ancestors without changing selection."],
            "hierarchy", "multi-select", "search"),

        C("Alert", "alert", "Feedback", "OVC-ALERT", "Inline contextual information, warning, success or error.", "Use for messages belonging to the current page or section.", ComponentPreviewKind.Feedback, ComponentKind.Primitive, "Governed MudAlert wrapper",
            ["Info", "Success", "Warning", "Error", "Dismissible"],
            ["Messages explain what happened and what to do next where useful."],
            "message", "warning"),
        C("Validation Summary", "validation-summary", "Feedback", "OVC-VALIDATION-SUMMARY", "Form-level aggregation of validation issues.", "Use when multiple validation problems need a concise overview and route to correction.", ComponentPreviewKind.Feedback, ComponentKind.Primitive, "OVValidationSummary",
            ["Error", "Warning"],
            ["Field validation belongs to each input OVC rather than this component.", "Issues may focus the relevant field.", "Do not rely on colour alone."],
            "validation", "form", "summary"),
        C("Tooltip", "tooltip", "Feedback", "OVC-TOOLTIP", "Short non-interactive supplementary explanation on hover/focus.", "Use to clarify compact controls or unfamiliar icons; never for essential instructions.", ComponentPreviewKind.Feedback, ComponentKind.SupportingControl, "Governed MudTooltip wrapper",
            ["Default", "Disabled target"],
            ["Tooltip content is explanatory only and never interactive.", "Essential information remains available without the tooltip."],
            "help", "hover"),
        C("Progress", "progress", "Feedback", "OVC-PROGRESS", "Determinate or indeterminate operation progress.", "Use for waits long enough that users need confidence work is continuing.", ComponentPreviewKind.Feedback, ComponentKind.Primitive, "OVProgress over MudProgressLinear",
            ["Determinate", "Indeterminate", "Complete", "Failed"],
            ["Spinner is not a separate OVC; indeterminate progress covers that need.", "Use determinate mode only when reliable total work is known.", "Completion resolves into a result state rather than remaining a full bar indefinitely."],
            "loading", "spinner", "job"),

        C("Card", "card", "Containers", "OVC-CARD", "Compact self-contained object/summary surface.", "Use for one compact object, summary or clear single action.", ComponentPreviewKind.Container, ComponentKind.CompositeComponent, "Governed MudCard/MudPaper composition",
            ["Default", "Disabled", "Actionable"],
            ["An actionable card has one clear primary action.", "Avoid competing click targets and nested card-on-card layouts."],
            "summary", "object"),
        C("Panel", "panel", "Containers", "OVC-PANEL", "Larger structural working/content region.", "Use to divide a page into major working regions.", ComponentPreviewKind.Container, ComponentKind.CompositeComponent, "Governed MudPaper composition",
            ["Default"],
            ["Panel itself does not own loading, empty or read-only states; child content does.", "Boundaries clarify work regions rather than decorating every block.", "Avoid panel-on-panel nesting without clear hierarchy."],
            "section", "workspace"),
        C("Accordion", "accordion", "Containers", "OVC-ACCORDION", "Collapsible secondary content section.", "Use where optional detail should remain available without dominating the page.", ComponentPreviewKind.Container, ComponentKind.CompositeComponent, "Governed MudExpansionPanel wrapper",
            ["Collapsed", "Expanded", "Disabled"],
            ["Critical actions or validation are not hidden by default."],
            "expand", "collapse"),

        C("Dialog", "dialog", "Overlays", "OVC-DIALOG", "Bounded modal decision/editing surface.", "Use for compact work that must be resolved before returning to the parent context.", ComponentPreviewKind.Overlay, ComponentKind.CompositeComponent, "Governed MudDialog composition",
            ["Default", "Saving", "Read-only"],
            ["Focus is trapped and parent interaction blocked.", "Cancel/secondary action precedes the far-right primary action.", "Dirty close is intercepted rather than silently discarded."],
            "modal", "edit"),
        C("Overlay", "overlay", "Overlays", "OVC-OVERLAY", "Right-side contextual working surface preserving visible parent context.", "Use for substantial create/edit/picker workflows with multiple sections or richer content.", ComponentPreviewKind.Overlay, ComponentKind.CompositeComponent, "Governed right-side drawer/overlay composition",
            ["Default", "Dirty", "Saving", "Error"],
            ["Right-side drawer is the standard orientation.", "Parent context remains visible.", "Dirty close requires an explicit leave decision."],
            "drawer", "workspace", "right side"),
        C("Confirm Dialog", "confirm-dialog", "Overlays", "OVC-CONFIRM-DIALOG", "Specialised Dialog composition for consequential actions.", "Use before destructive or difficult-to-reverse actions.", ComponentPreviewKind.Overlay, ComponentKind.SpecialisedComponent, "OVC-DIALOG composition",
            ["Default", "Busy"],
            ["State the consequence rather than asking only 'Are you sure?'.", "Use explicit action wording such as Delete connection or Purge records.", "Cancel precedes the far-right destructive action."],
            "delete", "destructive"),

        C("Key / Value", "key-value", "Display", "OVC-KEY-VALUE", "Compact label/value presentation.", "Use for read-only metadata and object details.", ComponentPreviewKind.Display, ComponentKind.Primitive, "OneVision display primitive",
            ["Default", "Empty value", "Long value"],
            ["Labels and values remain visually distinguishable.", "Empty values render consistently.", "Copy is composed with the standard semantic copy action rather than embedded into every value."],
            "details", "metadata"),
        C("Metric", "metric", "Display", "OVC-METRIC", "Headline formatted value with optional comparison and semantic trend meaning.", "Use for a small number of important quantitative summaries.", ComponentPreviewKind.Display, ComponentKind.Primitive, "OVMetric",
            ["Default", "Unavailable"],
            ["Formatting metadata governs decimals, units and suffixes upstream of the component.", "Direction and meaning are separate: up/down never automatically means good/bad."],
            "kpi", "number", "format"),
        C("Selection Summary", "selection-summary", "Display", "OVC-SELECTION-SUMMARY", "Passive compact summary of a selection owned by another control.", "Use beside trees, pickers or bulk workflows to summarise current selection.", ComponentPreviewKind.Display, ComponentKind.Primitive, "OVSelectionSummary",
            ["Default", "Empty"],
            ["The component does not own selection mechanics.", "It may expose review and clear affordances supplied by the parent.", "Many selections collapse to a compact +N summary."],
            "selected", "picker"),

        C("Toolbar", "toolbar", "App shell", "OVC-TOOLBAR", "Persistent workspace controls arranged through named regions.", "Use above lists, trees, canvases and builders.", ComponentPreviewKind.Shell, ComponentKind.CompositeComponent, "OVToolbar",
            ["Default", "Read-only"],
            ["Leading holds search/filter/context controls.", "Middle is optional supporting context.", "Trailing holds refresh/export/primary actions.", "Normally expose only one primary action."],
            "actions", "search", "filter", "named regions"),
        C("Action Bar", "action-bar", "App shell", "OVC-ACTION-BAR", "Contextual action surface with named Summary, Supporting and Actions regions.", "Use when a parent context activates selection/edit/process actions.", ComponentPreviewKind.Shell, ComponentKind.CompositeComponent, "OVActionBar",
            ["Default", "Busy"],
            ["The component does not hard-code selection/edit/process workflow types.", "The parent owns visibility and meaning.", "Actions remain right-aligned and context remains explicit."],
            "bulk", "context", "named regions"),

        C("Structured Field Tree", "structured-field-tree", "Structured data", "OVC-STRUCTURED-FIELD-TREE", "Structured JSON/object field view built on OVC-TREE with explicit construct and data-type metadata.", "Use when users must understand or select nested incoming fields without flattening away context.", ComponentPreviewKind.StructuredTree, ComponentKind.SpecialisedComponent, "OVStructuredFieldTree over OVTree",
            ["LeafOnly", "AnyNode"],
            ["Node kind is Object, Array or Primitive and is not an interaction state.", "Primitive data types are String, Integer, Number, Boolean, Date, DateTime, Null or Unknown.", "Empty object/array are data conditions rather than interaction states.", "Default selection is LeafOnly.", "Search matches field name and full path and reveals ancestor branches."],
            "json", "schema", "field", "tree", "mapping")
    ];

    public static ComponentDefinition? Find(string slug)
        => Components.FirstOrDefault(component => string.Equals(component.Slug, slug, StringComparison.OrdinalIgnoreCase));

    public static IEnumerable<IGrouping<string, ComponentDefinition>> Grouped()
        => GroupOrder.Select(group => Components.Where(component => component.Group == group).ToArray())
            .Where(items => items.Length > 0)
            .SelectMany(items => items.GroupBy(component => component.Group));

    public static IEnumerable<ComponentDefinition> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return Components;
        var q = query.Trim();
        return Components.Where(component =>
            component.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
            component.Reference.Contains(q, StringComparison.OrdinalIgnoreCase) ||
            component.Description.Contains(q, StringComparison.OrdinalIgnoreCase) ||
            component.UseWhen.Contains(q, StringComparison.OrdinalIgnoreCase) ||
            component.Group.Contains(q, StringComparison.OrdinalIgnoreCase) ||
            component.Kind.ToString().Contains(q, StringComparison.OrdinalIgnoreCase) ||
            component.ImplementationBasis.Contains(q, StringComparison.OrdinalIgnoreCase) ||
            component.States.Any(state => state.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
            component.Behaviours.Any(behaviour => behaviour.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
            component.Tags.Any(tag => tag.Contains(q, StringComparison.OrdinalIgnoreCase)));
    }

    public static string KindLabel(ComponentKind kind) => kind switch
    {
        ComponentKind.CompositeComponent => "Composite component",
        ComponentKind.SupportingControl => "Supporting control",
        ComponentKind.OvpRegion => "OVP region",
        ComponentKind.SpecialisedComponent => "Specialised component",
        _ => "Primitive"
    };
}
