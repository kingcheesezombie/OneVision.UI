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

public sealed record ComponentDefinition(
    string Name,
    string Slug,
    string Group,
    string Reference,
    string Description,
    string UseWhen,
    ComponentPreviewKind PreviewKind,
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
        string[] states,
        string[] behaviours,
        params string[] tags)
        => new(name, slug, group, reference, description, useWhen, previewKind, states, behaviours, tags);

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
        C("Button", "button", "Actions", "OVC-BUTTON", "Primary, secondary and low-emphasis actions.", "Use for an explicit user action with a clear verb.", ComponentPreviewKind.Action,
            ["Primary", "Outlined", "Text", "Hover", "Focus", "Disabled", "Loading"],
            ["Primary actions use the OneVision blue treatment.", "Disabled actions remain visible when their absence would remove context.", "Loading prevents repeat invocation and preserves the button width."],
            "action", "submit", "save"),
        C("Icon Button", "icon-button", "Actions", "OVC-ICON-BUTTON", "Compact icon-only action with accessible label.", "Use where a familiar action must fit in a compact toolbar or row.", ComponentPreviewKind.Action,
            ["Default", "Hover", "Focus", "Disabled", "Destructive"],
            ["Every icon-only action requires an accessible label and tooltip.", "Use only established icons; do not make users infer novel meaning."],
            "toolbar", "icon"),
        C("Overflow Menu", "overflow-menu", "Actions", "OVC-OVERFLOW-MENU", "Three-dot menu for secondary or destructive actions.", "Use for row/card actions that should not dominate the primary UI.", ComponentPreviewKind.Action,
            ["Closed", "Open", "Item hover", "Disabled item", "Destructive item"],
            ["Delete and other secondary actions should normally live here.", "Opening the menu must not change the underlying selection."],
            "menu", "three-dot", "delete"),
        C("Copy Button", "copy-button", "Actions", "OVC-COPY-BUTTON", "Compact copy-to-clipboard action with confirmation state.", "Use beside stable references, identifiers, paths and API values.", ComponentPreviewKind.Action,
            ["Default", "Copied", "Disabled"],
            ["Copy preserves the exact source text.", "Successful copy changes to a short-lived Copied state."],
            "clipboard", "reference"),

        C("Text Field", "text-field", "Text & input", "OVC-TEXT-FIELD", "Single-line text entry.", "Use for short free-text values.", ComponentPreviewKind.TextInput,
            ["Empty", "Populated", "Focused", "Required", "Disabled", "Read-only", "Validation error"],
            ["Labels remain visible when populated.", "Read-only values remain selectable/copyable.", "Validation appears adjacent to the field."],
            "input", "form"),
        C("Text Area", "text-area", "Text & input", "OVC-TEXT-AREA", "Multi-line text entry.", "Use for notes, descriptions, queries and other longer text.", ComponentPreviewKind.TextInput,
            ["Empty", "Populated", "Focused", "Disabled", "Read-only", "Validation error"],
            ["Grow vertically within sensible limits.", "Do not use for structured or coded values."],
            "notes", "description"),
        C("Number Field", "number-field", "Text & input", "OVC-NUMBER-FIELD", "Numeric entry with governed parsing and validation.", "Use where the domain value is numeric rather than text containing digits.", ComponentPreviewKind.TextInput,
            ["Empty", "Populated", "Minimum", "Maximum", "Disabled", "Validation error"],
            ["Reject invalid numeric input before submit.", "Display formatting is separate from stored numeric value."],
            "numeric", "measure"),
        C("Search Box", "search-box", "Text & input", "OVC-SEARCH-BOX", "Dedicated search control with clear behaviour.", "Use for every searchable list, picker or tree.", ComponentPreviewKind.TextInput,
            ["Empty", "Typing", "Results", "No results", "Cleared", "Disabled"],
            ["Search is scoped to the list/control it belongs to.", "A clear action appears when text is present.", "Search should normally update without a separate submit button."],
            "search", "filter"),

        C("Select", "select", "Selection", "OVC-SELECT", "Single-value dropdown selection.", "Use for a bounded set of choices when only one may be selected.", ComponentPreviewKind.Selection,
            ["Empty", "Populated", "Open", "Disabled", "Read-only", "Validation error"],
            ["Current selection remains visible when closed.", "Long option sets should use Autocomplete instead."],
            "dropdown", "single"),
        C("Multi-select", "multi-select", "Selection", "OVC-MULTI-SELECT", "Multiple-choice selection with compact selected-value treatment.", "Use when several members from a governed set may be chosen.", ComponentPreviewKind.Selection,
            ["Empty", "Populated", "Open", "Search", "Many selected", "Disabled"],
            ["Selections are removable without reopening the control.", "Large member sets should be searchable.", "Summary treatment must remain compact when many values are selected."],
            "multiple", "members"),
        C("Autocomplete", "autocomplete", "Selection", "OVC-AUTOCOMPLETE", "Search-first single selection.", "Use for large option sets where scanning a dropdown is impractical.", ComponentPreviewKind.Selection,
            ["Empty", "Typing", "Results", "Selected", "No results", "Disabled"],
            ["Do not accept arbitrary text unless explicitly configured.", "Keyboard navigation must work through results."],
            "lookup", "search"),
        C("Checkbox", "checkbox", "Selection", "OVC-CHECKBOX", "Independent yes/no or inclusion choice.", "Use for independent selections and multi-row picking.", ComponentPreviewKind.Toggle,
            ["Unchecked", "Checked", "Indeterminate", "Disabled"],
            ["Indeterminate is reserved for mixed child state.", "A checkbox should not be used where the choice immediately changes application mode."],
            "boolean", "multi"),
        C("Radio Group", "radio-group", "Selection", "OVC-RADIO-GROUP", "Visible mutually-exclusive choices.", "Use when a small set of exclusive choices benefits from being visible at once.", ComponentPreviewKind.Selection,
            ["Unselected", "Selected", "Disabled option", "Validation error"],
            ["Exactly one option may be active when selection is required.", "Prefer Select for larger sets."],
            "exclusive", "choice"),
        C("Toggle", "toggle", "Selection", "OVC-TOGGLE", "Immediate enabled/disabled setting.", "Use for settings that take effect as a binary capability state.", ComponentPreviewKind.Toggle,
            ["Off", "On", "Disabled", "Saving"],
            ["The visible label describes the setting, not the action.", "Use Enabled as the user-facing switch where lifecycle Active is internal."],
            "enabled", "switch"),

        C("Date Picker", "date-picker", "Dates & times", "OVC-DATE-PICKER", "Single date selection.", "Use for effective dates and other date-only values.", ComponentPreviewKind.DateTime,
            ["Empty", "Selected", "Open", "Disabled", "Read-only", "Validation error"],
            ["Display dates in DD/MM/YYYY for the OneVision UI.", "Do not imply time-zone semantics for date-only values."],
            "date", "effective"),
        C("Date Range", "date-range", "Dates & times", "OVC-DATE-RANGE", "From/to date selection.", "Use for filtering or editing bounded effective periods.", ComponentPreviewKind.DateTime,
            ["Empty", "From only", "Complete", "Invalid range", "Disabled"],
            ["To cannot precede From.", "Open-ended ranges must be explicitly supported rather than represented by a fake maximum date."],
            "range", "from", "to"),
        C("Time Picker", "time-picker", "Dates & times", "OVC-TIME-PICKER", "Time-of-day selection.", "Use for schedule times and other local clock values.", ComponentPreviewKind.DateTime,
            ["Empty", "Selected", "Open", "Disabled", "Validation error"],
            ["Display using 24-hour time.", "Time-zone context belongs to the surrounding pattern when relevant."],
            "time", "schedule"),

        C("Status Pill", "status-pill", "Status & labels", "OVC-STATUS-PILL", "Compact semantic status indicator.", "Use for lifecycle, health and job states.", ComponentPreviewKind.Pill,
            ["Neutral", "Info", "Success", "Warning", "Danger"],
            ["Colour reinforces text; it never replaces the text label.", "Use consistent semantic tones for equivalent states across applications."],
            "status", "chip"),
        C("Type Pill", "type-pill", "Status & labels", "OVC-TYPE-PILL", "Compact type/category label.", "Use in dense lists where object type needs quick scanning.", ComponentPreviewKind.Pill,
            ["Default", "Muted", "Selected"],
            ["Type pills are categorical, not success/error indicators."],
            "type", "category"),
        C("Source Pill", "source-pill", "Status & labels", "OVC-SOURCE-PILL", "Compact source-system label.", "Use to distinguish records originating from different governed sources.", ComponentPreviewKind.Pill,
            ["Default", "Muted", "Selected"],
            ["Source labels remain textually explicit even where colour is used."],
            "source", "system"),
        C("Filter Pill", "filter-pill", "Status & labels", "OVC-FILTER-PILL", "Active filter summary with optional remove action.", "Use to make applied analytical/list filters visible.", ComponentPreviewKind.Pill,
            ["Active", "Removable", "Locked", "Overflow summary"],
            ["Removing a pill removes only that filter.", "Locked/context filters remain visible but do not show a remove action."],
            "filter", "summary"),
        C("Count Badge", "count-badge", "Status & labels", "OVC-COUNT-BADGE", "Compact numeric count attached to a label or navigation item.", "Use where a small count adds useful context without becoming a metric card.", ComponentPreviewKind.Pill,
            ["Zero", "Positive", "Large count", "Muted"],
            ["Large values may compact visually but must retain accessible exact text."],
            "count", "badge"),

        C("Data Grid", "data-grid", "Data & lists", "OVC-DATA-GRID", "Standard compact OneVision data list.", "Use for structured tabular results with sorting, selection or row actions.", ComponentPreviewKind.Data,
            ["Default", "Loading", "Empty", "Filtered", "Sorted", "Row selected", "Row actions"],
            ["Default page size is 25 rows.", "Pagination belongs at the top-right of the list.", "Row delete belongs in the overflow menu unless it is the primary workflow.", "Every first-class list gets its own search and context filters."],
            "table", "list", "pagination"),
        C("Pagination", "pagination", "Data & lists", "OVC-PAGINATION", "Compact page navigation and result range.", "Use with paged lists and grids.", ComponentPreviewKind.Navigation,
            ["First page", "Middle page", "Last page", "Single page", "Disabled"],
            ["Place at the top-right of every first-class list.", "Changing page must preserve search/filter state."],
            "page", "25"),
        C("Sortable Header", "sortable-header", "Data & lists", "OVC-SORTABLE-HEADER", "Column header with sortable state.", "Use for columns where ordering helps users find or compare records.", ComponentPreviewKind.Data,
            ["Unsorted", "Ascending", "Descending", "Disabled"],
            ["Repeated activation cycles through supported sort states.", "Current sort direction is visually and accessibly explicit."],
            "sort", "column"),
        C("Empty State", "empty-state", "Data & lists", "OVC-EMPTY-STATE", "No-data/no-results presentation with next action.", "Use when a list, workspace or configuration has nothing to show.", ComponentPreviewKind.Feedback,
            ["First use", "No results", "Filtered out", "Permission constrained"],
            ["Explain why the area is empty and provide the most useful next action.", "No-results state should offer clear/reset filters where appropriate."],
            "empty", "no results"),

        C("Tabs", "tabs", "Navigation", "OVC-TABS", "Horizontal view/state navigation within one parent context.", "Use for a small number of peer views within the same object or workflow.", ComponentPreviewKind.Navigation,
            ["Default", "Active", "Disabled", "Overflow"],
            ["Changing tab changes the visible child state without losing the parent context.", "Tabs represent peer views, not sequential wizard steps."],
            "tab", "view"),
        C("Breadcrumb", "breadcrumb", "Navigation", "OVC-BREADCRUMB", "Hierarchy/location trail.", "Use where users can drill through nested scopes and need a quick route back.", ComponentPreviewKind.Navigation,
            ["Root", "Nested", "Long path"],
            ["Ancestors are navigable; the current location is not.", "Collapse excessively long paths without losing the root/current locations."],
            "path", "location"),
        C("Side Navigation", "side-navigation", "Navigation", "OVC-SIDE-NAV", "Local navigation for a workspace or settings area.", "Use when several persistent sections share the same surrounding workspace.", ComponentPreviewKind.Navigation,
            ["Default", "Active", "Hover", "Collapsed group"],
            ["Selected and hover backgrounds retain visible separation between adjacent items.", "Only show the hierarchy depth users need for the current task."],
            "menu", "settings"),
        C("Tree", "tree", "Navigation", "OVC-TREE", "Expandable hierarchical navigation/selection.", "Use for true parent/child structures.", ComponentPreviewKind.Navigation,
            ["Collapsed", "Expanded", "Selected", "Search match", "Disabled node", "Checked subtree"],
            ["Expand/collapse does not implicitly select a node.", "Search reveals matching descendants and their ancestor path.", "Selection capability is configured separately from structural expansion."],
            "hierarchy", "node"),

        C("Alert", "alert", "Feedback", "OVC-ALERT", "Inline contextual message.", "Use for information, warnings or errors that belong to the current page/section.", ComponentPreviewKind.Feedback,
            ["Info", "Success", "Warning", "Error", "Dismissible"],
            ["Messages explain both what happened and, where useful, what to do next."],
            "message", "warning"),
        C("Validation Message", "validation-message", "Feedback", "OVC-VALIDATION-MESSAGE", "Field or form validation feedback.", "Use when input cannot be accepted as entered.", ComponentPreviewKind.Feedback,
            ["Field error", "Form summary", "Warning"],
            ["Validation is specific and actionable.", "Do not rely on colour alone."],
            "error", "form"),
        C("Tooltip", "tooltip", "Feedback", "OVC-TOOLTIP", "Short supplementary explanation on hover/focus.", "Use to clarify compact controls or unfamiliar icons; never for essential instructions.", ComponentPreviewKind.Feedback,
            ["Hidden", "Visible", "Keyboard focus"],
            ["Essential information must remain available without the tooltip."],
            "help", "hover"),
        C("Progress", "progress", "Feedback", "OVC-PROGRESS", "Determinate or indeterminate operation progress.", "Use for operations long enough that users need confidence work is continuing.", ComponentPreviewKind.Feedback,
            ["Indeterminate", "Determinate", "Complete", "Failed"],
            ["Use determinate progress when reliable total work is known.", "Completion should resolve into a result state rather than leaving a full progress bar indefinitely."],
            "loading", "job"),
        C("Spinner", "spinner", "Feedback", "OVC-SPINNER", "Small indeterminate loading indicator.", "Use for short local waits where a full progress treatment would be excessive.", ComponentPreviewKind.Feedback,
            ["Inline", "Button", "Panel"],
            ["Do not replace substantial page content with a spinner when a skeleton or progress/result pattern is more informative."],
            "loading"),

        C("Card", "card", "Containers", "OVC-CARD", "Bounded content grouping.", "Use for a compact self-contained summary or action surface.", ComponentPreviewKind.Container,
            ["Default", "Selected", "Disabled", "Actionable"],
            ["Cards group related content; avoid nested card-on-card layouts without a clear hierarchy."],
            "panel", "summary"),
        C("Panel", "panel", "Containers", "OVC-PANEL", "Larger content region with optional title/actions.", "Use to structure a page into major working regions.", ComponentPreviewKind.Container,
            ["Default", "Loading", "Empty", "Read-only"],
            ["Panel boundaries should clarify working regions rather than decorate every block."],
            "section", "workspace"),
        C("Accordion", "accordion", "Containers", "OVC-ACCORDION", "Collapsible secondary content section.", "Use where optional detail should be available without dominating the page.", ComponentPreviewKind.Container,
            ["Collapsed", "Expanded", "Disabled"],
            ["Critical actions or validation should not be hidden in a collapsed section by default."],
            "expand", "collapse"),

        C("Dialog", "dialog", "Overlays", "OVC-DIALOG", "Modal decision or compact editing surface.", "Use for bounded work that must be resolved before returning to the parent context.", ComponentPreviewKind.Overlay,
            ["Open", "Validation error", "Saving", "Read-only"],
            ["Escape/close behaviour respects unsaved changes.", "Primary and secondary actions remain in a consistent footer."],
            "modal", "edit"),
        C("Overlay", "overlay", "Overlays", "OVC-OVERLAY", "Larger contextual working surface above the parent page.", "Use for rich create/edit/picker workflows that should preserve parent context.", ComponentPreviewKind.Overlay,
            ["Open", "Dirty", "Saving", "Error"],
            ["The underlying page context remains conceptually intact.", "Unsaved changes require an explicit leave decision."],
            "drawer", "workspace"),
        C("Confirm Dialog", "confirm-dialog", "Overlays", "OVC-CONFIRM-DIALOG", "Explicit confirmation for consequential actions.", "Use before destructive or difficult-to-reverse actions.", ComponentPreviewKind.Overlay,
            ["Confirm", "Destructive", "Busy"],
            ["State the consequence, not merely 'Are you sure?'.", "Destructive primary action uses explicit wording such as Delete or Purge."],
            "delete", "destructive"),

        C("Key / Value", "key-value", "Display", "OVC-KEY-VALUE", "Compact label/value presentation.", "Use for read-only metadata and object details.", ComponentPreviewKind.Display,
            ["Default", "Empty value", "Long value", "Copyable"],
            ["Labels and values remain visually distinguishable.", "Empty values render consistently rather than disappearing."],
            "details", "metadata"),
        C("Metric", "metric", "Display", "OVC-METRIC", "Headline numeric value with context.", "Use for a small number of important quantitative summaries.", ComponentPreviewKind.Display,
            ["Default", "Positive change", "Negative change", "Unavailable"],
            ["Formatting metadata governs decimals, units and suffixes.", "Do not imply positive/negative semantics from direction alone when the domain does not support it."],
            "kpi", "number"),
        C("Selection Summary", "selection-summary", "Display", "OVC-SELECTION-SUMMARY", "Compact summary of current multi-selection.", "Use at the edge of a picker or bulk-action workflow.", ComponentPreviewKind.Display,
            ["None", "One", "Several", "Many"],
            ["Summaries stay compact while retaining a way to inspect/remove individual selections."],
            "selected", "picker"),

        C("Page Header", "page-header", "App shell", "OVC-PAGE-HEADER", "Page title, context and primary actions.", "Use at the top of first-class OneVision pages.", ComponentPreviewKind.Shell,
            ["Title only", "With description", "With status", "With actions"],
            ["Identity/context stays left; primary contextual actions stay right.", "Avoid duplicating toolbar controls in the header."],
            "title", "page"),
        C("Toolbar", "toolbar", "App shell", "OVC-TOOLBAR", "Search/filter/action row for the current working area.", "Use above lists, trees, canvases and builders.", ComponentPreviewKind.Shell,
            ["Default", "Wrapping", "Selection mode", "Read-only"],
            ["Search belongs to the specific working area.", "Primary actions are right-aligned; context/search/filter controls stay grouped."],
            "actions", "search", "filter"),
        C("Action Bar", "action-bar", "App shell", "OVC-ACTION-BAR", "Contextual action strip, often triggered by selection.", "Use for bulk actions or persistent save/cancel controls.", ComponentPreviewKind.Shell,
            ["Hidden", "Visible", "Busy", "Disabled actions"],
            ["Only appears when its context is active.", "Selection count or edit state is explicit."],
            "bulk", "footer"),

        C("Structured Field Tree", "structured-field-tree", "Structured data", "OVC-STRUCTURED-FIELD-TREE", "Hierarchical representation of structured fields derived from incoming JSON/object constructs.", "Use when users must understand or select fields from nested JSON or equivalent structured payloads without flattening away their context.", ComponentPreviewKind.StructuredTree,
            ["Collapsed", "Expanded", "Leaf selected", "Object selected", "Array", "Search match", "Empty object", "Empty array", "Null/unknown type"],
            ["Object properties render as parent/child relationships.", "Arrays render as array nodes and expose the structure of an item rather than duplicating every data row.", "Leaf nodes display their inferred data type.", "Expanding/collapsing a node does not select it.", "Selecting a field exposes its complete structured path.", "Search matches field name and full path and automatically reveals matching ancestor branches.", "Selection capability is configurable, for example LeafOnly or AnyNode.", "Objects and arrays remain visible even when they are not selectable.", "The control must preserve construct hierarchy rather than presenting a flattened dotted-field list."],
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
            component.States.Any(state => state.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
            component.Behaviours.Any(behaviour => behaviour.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
            component.Tags.Any(tag => tag.Contains(q, StringComparison.OrdinalIgnoreCase)));
    }
}
