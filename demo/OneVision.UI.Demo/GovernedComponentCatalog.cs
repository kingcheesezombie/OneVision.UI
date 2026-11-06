namespace OneVision.UI.Demo;

/// <summary>
/// Gallery facade over the canonical component catalogue plus newly implemented governed components.
/// </summary>
public static class GovernedComponentCatalog
{
    private static readonly IReadOnlyList<ComponentDefinition> AdditionalComponents =
    [
        new("Metadata List", "metadata-list", "Display", "OVC-METADATA-LIST",
            "Compact renderer for arbitrary metadata/property collections with grouping and technical-value treatments.",
            "Use for entity metadata, diagnostics and other compact property collections where repeated standalone key/value composition becomes cumbersome.",
            ComponentPreviewKind.Display, ComponentKind.CompositeComponent, "OVMetadataList using semantic definition-list markup and OVC-COPYABLE-LINK for copyable values",
            ["Default", "Grouped", "Empty values", "Long values", "Structured values"],
            ["Supports optional groups without card-per-property nesting.", "Long and structured values wrap within compact administrative layouts.", "Applications own labels, values and domain meaning; OneVision.UI owns presentation."],
            ["metadata", "properties", "details", "diagnostics", "key value"]),
        new("Copyable Link", "copyable-link", "Display", "OVC-COPYABLE-LINK",
            "Highlighted selectable text with a semantic copy affordance at the trailing edge.",
            "Use for references, identifiers, paths or implementation snippets that users commonly copy.",
            ComponentPreviewKind.Display, ComponentKind.Primitive, "OVCopyableLink with OneVisionIcon.Copy",
            ["Default", "Disabled", "Long value"],
            ["The copyable text is visually distinct and remains selectable.", "The semantic copy icon is always at the trailing edge.", "Click, Enter or Space copies the full supplied value and announces completion accessibly.", "If clipboard access fails, the component displays an explicit unavailable state and calls CopyFailed rather than Copied.", "Changing the copy payload clears stale success/error feedback without remounting the component."],
            ["copy", "reference", "link", "code"]),
        new("Mapping Pair", "mapping-pair", "Mapping", "OVC-MAPPING-PAIR",
            "Explicit source-to-target mapping interaction with governed drag/drop, keyboard alternatives and optional governed Standard Details.",
            "Use when inspected source data must map to a declared/templated target contract.",
            ComponentPreviewKind.StructuredTree, ComponentKind.CompositeComponent,
            "OVMappingPair using OVC-STRUCTURED-FIELD-TREE for actual/discovered source data, governed target rows and OVC-SELECT for optional Standard Details",
            ["Default", "Standard Details empty", "Standard Details multiple", "Standard Details exhausted", "Required unmapped", "Invalid mapping", "Disabled", "Read-only", "Constants", "Configurable", "Loading", "Empty", "Error", "Responsive"],
            ["Actual/discovered source data is always rendered through OVC-STRUCTURED-FIELD-TREE.", "The target pane remains a declared/templated contract list.", "Optional Standard Details are added one row at a time through the single section-header action; selectors contain only eligible unselected details and unselected catalogue entries never become mapping rows.", "Removing an optional Standard Detail clears its UI mapping association and makes it eligible for selection again.", "Drag a source primitive onto an eligible target; keyboard users can select/focus a source and activate a target."],
            ["mapping", "pair", "drag drop", "structured", "source", "target", "standard details"]),
        new("Mapping Join", "mapping-join", "Mapping", "OVC-MAPPING-JOIN",
            "Symmetric structured-data pairing interaction with a configurable comparison method on every join pair.",
            "Use when actual/discovered fields on two data structures must be matched and compared.",
            ComponentPreviewKind.StructuredTree, ComponentKind.CompositeComponent,
            "OVMappingJoin composed from OVC-STRUCTURED-FIELD-TREE on both sides plus governed select, pill, toolbar, icon action, validation, alert, progress and empty-state primitives",
            ["Default", "Asset candidate", "Person Episode candidate", "Structure transition", "Definitions loading", "Structure loading", "No definitions", "Structure empty", "Invalid / conflict", "Required unpaired", "Disabled items", "Read-only", "Multiple joins", "Loading", "Empty", "Error", "Responsive"],
            ["Both sides use OVC-STRUCTURED-FIELD-TREE.", "Dependent Type → Definition → Structure selection is supported symmetrically.", "Structure changes hide stale fields immediately; surviving joins are determined from stable field keys in the replacement structure.", "Drag may start from either side and keyboard selection remains available.", "Endpoint reuse never implies replacement; exact-pair recognition and explicit replacement are distinct from application-owned cardinality."],
            ["mapping", "join", "match", "compare", "structured field tree", "cardinality"]),
        new("Mapping Rule", "mapping-rule", "Mapping", "OVC-MAPPING-RULE",
            "Generic condition-builder interaction for deciding when a mapping or outcome should occur.",
            "Use when the user must define conditions, operators, grouping and an outcome rather than directly connect two items.",
            ComponentPreviewKind.StructuredTree, ComponentKind.CompositeComponent,
            "OVMappingRule composed from governed Select, Text Field, Toggle, Empty State, Alert and Progress controls",
            ["Default", "AND conditions", "OR conditions", "Nested groups", "Wildcard", "Invalid", "Ambiguous", "Disabled", "Read-only", "Empty", "Loading", "Error", "Responsive"],
            ["Typed fields constrain the nine governed operators and validate comparison values, including escaped LIKE-style wildcard patterns.", "AND/OR joins, optional nested groups, condition disabling and ordering retain application-owned state.", "Test Rule invokes an application callback and displays an explanation without writing operational data.", "Classification and dynamic target-resolution slots allow the governed builder composition; applications own catalogue, dependencies, source scope and outcome semantics."],
            ["mapping", "rule", "conditions", "operator", "outcome"]),
        new("Process Stages", "process-stages", "Operations", "OVC-PROCESS-STAGES",
            "Ordered operational process-stage display with execution state and optional run metadata.",
            "Use for read-only status of a run, job or process with meaningful ordered stages.",
            ComponentPreviewKind.Display, ComponentKind.CompositeComponent, "OVProcessStages composed from OVC-PILL, OVC-ICON-BUTTON, OVC-PROGRESS, OVC-ALERT and OVC-EMPTY-STATE",
            ["Default", "Running", "Failed", "Warning", "Loading", "Empty", "Error", "Responsive"],
            ["Pending, Running, Complete, Failed, Skipped and Warning are non-colour-only states.", "Applications own stage sequence and execution metadata; OneVision.UI owns progression grammar."],
            ["process", "stages", "job", "run", "operations"]),
        new("Notification", "notification", "Feedback", "OVC-NOTIFICATION",
            "Transient application feedback delivered through a governed host and service.",
            "Use for short-lived completion or operational feedback that does not need to persist in page content.",
            ComponentPreviewKind.Feedback, ComponentKind.CompositeComponent, "OVNotification + OVNotificationHost + OVNotificationService",
            ["Success", "Information", "Warning", "Error", "Persistent", "Stacked", "Timed", "Replacement"],
            ["Notifications never steal focus.", "The host owns bounded stacking, queueing, remaining-time pause/resume for pointer and focus, replacement keys and disposal-safe dismissal.", "Persistent or validation content still uses OVC-ALERT / OVC-VALIDATION-SUMMARY."],
            ["notification", "toast", "snackbar", "feedback"]),
        new("File Upload", "file-upload", "Text & input", "OVC-FILE-UPLOAD",
            "Governed browse/drag file-selection surface with selected-file, validation and upload-progress states.",
            "Use when an application accepts one or more files for import, attachment or processing.",
            ComponentPreviewKind.TextInput, ComponentKind.CompositeComponent, "OVFileUpload using InputFile, OVC-ICON-BUTTON, OVC-VALIDATION-SUMMARY and OVC-PROGRESS",
            ["Default", "Multiple", "Selected", "Invalid", "Uploading", "Read-only", "Disabled"],
            ["Applications own file meaning and validation rules.", "OneVision.UI owns selection surface, selected-file presentation, progress and removal affordance.", "Strongly typed RemoveFileRequested allows exact-file removal when filenames collide; legacy name-based removal remains available."],
            ["file", "upload", "import", "browse", "drag drop"]),
        new("Timeline", "timeline", "Display", "OVC-TIMELINE",
            "Chronological event/history presentation distinct from process-stage progression.",
            "Use for audit, history and operational events ordered by time.",
            ComponentPreviewKind.Display, ComponentKind.CompositeComponent, "OVTimeline using OVC-DATE-TIME and semantic OneVision icons",
            ["Default", "Loading", "Empty", "Error", "Responsive"],
            ["Events are chronological and may expose an app-owned action.", "Timeline is not a workflow state machine; use OVC-PROCESS-STAGES for ordered execution stages."],
            ["timeline", "history", "audit", "events"]),
        new("Skeleton", "skeleton", "Feedback", "OVC-SKELETON",
            "Lightweight loading placeholder for progressive content hydration.",
            "Use where content shape should remain visible while data is loading and full OVC-PROGRESS treatment is too heavy.",
            ComponentPreviewKind.Feedback, ComponentKind.SupportingControl, "OVSkeleton with reduced-motion-aware optional animation",
            ["Text", "Rectangle", "Circle", "Static"],
            ["Skeleton is decorative and aria-hidden.", "Animation is suppressed automatically when reduced motion is requested."],
            ["skeleton", "loading", "placeholder"]),
        new("List", "list", "Data & lists", "OVC-LIST",
            "Compact non-tabular list for readable or actionable items.",
            "Use when a grid is too heavy and the content is not hierarchical.",
            ComponentPreviewKind.Data, ComponentKind.CompositeComponent, "OVList with optional item activation and trailing actions",
            ["Default", "Actionable", "Disabled", "Empty"],
            ["Lists remain non-hierarchical and compact.", "Use OVC-DATA-GRID for tabular data and OVC-TREE for hierarchy."],
            ["list", "items", "compact"]),
        new("Selection List", "selection-list", "Selection", "OVC-SELECTION-LIST",
            "Compact single- or multi-selection list for bounded known options.",
            "Use when options benefit from descriptive rows rather than a Select or Multi-select dropdown.",
            ComponentPreviewKind.Selection, ComponentKind.CompositeComponent, "OVSelectionList with native radio/checkbox semantics",
            ["Single", "Multiple", "Disabled", "Empty"],
            ["Uses native accessible selection semantics.", "Does not replace OVC-TREE for hierarchical selection."],
            ["selection", "list", "radio", "checkbox"]),
        new("Divider", "divider", "Containers", "OVC-DIVIDER",
            "Semantic horizontal or vertical separator with optional label.",
            "Use sparingly to separate adjacent regions when spacing alone is insufficient.",
            ComponentPreviewKind.Container, ComponentKind.Primitive, "OVDivider semantic separator",
            ["Horizontal", "Labelled", "Vertical"],
            ["Divider conveys separation without creating a new container surface."],
            ["divider", "separator"]),
        new("Badge", "badge", "Status & labels", "OVC-BADGE",
            "Small attached count/text indicator for compact icon or navigation contexts.",
            "Use for terse attached indicators; use OVC-PILL for normal visible status/type/source/filter/count labels.",
            ComponentPreviewKind.Pill, ComponentKind.Primitive, "OVBadge",
            ["Information", "Success", "Warning", "Error", "Overflow count"],
            ["Badge is subordinate to OVC-PILL and should not become the normal status vocabulary."],
            ["badge", "count", "indicator"]),
        new("Stepper", "stepper", "Navigation", "OVC-STEPPER",
            "User-driven ordered step navigation for multi-step tasks.",
            "Use when a user deliberately progresses through a bounded multi-step workflow.",
            ComponentPreviewKind.Navigation, ComponentKind.CompositeComponent, "OVStepper with accessible ordered-step semantics",
            ["Default", "Complete", "Error", "Disabled", "Responsive"],
            ["Stepper is for user-driven progress, not background execution; use OVC-PROCESS-STAGES for operational runs."],
            ["stepper", "wizard", "steps", "workflow"]),
        new("Schedule Builder", "schedule-builder", "Operations", "OVC-SCHEDULE-BUILDER",
            "Governed schedule-definition control with explicit Day, Week, Month and Interval states plus dual-pane item commitment.",
            "Use when users define a reusable recurrence and commit a set of application-supplied items to that schedule.",
            ComponentPreviewKind.DateTime, ComponentKind.CompositeComponent, "OVScheduleBuilder composed from governed inputs, selection, actions and dual-pane item commitment",
            ["Day", "Week", "Month", "Interval", "Disabled", "Read-only", "Responsive"],
            ["Schedule type is a first-class state, not a free-form cron expression.", "Week and Month expose state-specific recurrence controls.", "Items move explicitly from Search list to Committed items; selection alone never commits.", "Applications own item meaning, eligibility, validation, persistence and execution semantics."],
            ["schedule", "builder", "recurrence", "day", "week", "month", "interval", "items"]),
        new("Date / Time", "date-time-display", "Display", "OVC-DATE-TIME",
            "Governed display formatting for dates and timestamps using explicit timezone conversion.",
            "Use whenever operational dates/times need consistent OneVision presentation.",
            ComponentPreviewKind.DateTime, ComponentKind.SupportingControl, "OVDateTime semantic time element",
            ["Date", "Time", "DateTime", "Seconds", "Empty"],
            ["The application may provide a timezone; display formatting is consistent and machine-readable datetime is preserved."],
            ["date", "time", "timestamp", "timezone"])
    ];

    public static IReadOnlyList<string> GroupOrder { get; } = ComponentCatalog.GroupOrder
        .Where(group => group != "Structured data")
        .Concat(["Mapping", "Operations", "Structured data"])
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToArray();

    public static IEnumerable<ComponentDefinition> Components => ComponentCatalog.Components.Concat(AdditionalComponents);

    public static ComponentDefinition? Find(string slug)
        => Components.FirstOrDefault(component => string.Equals(component.Slug, slug, StringComparison.OrdinalIgnoreCase));

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

    public static string KindLabel(ComponentKind kind) => ComponentCatalog.KindLabel(kind);
}
