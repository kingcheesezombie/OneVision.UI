# OneVision.UI Component Catalogue

This document describes the canonical shared OneVision component vocabulary. Applications should use these contracts rather than direct MudBlazor composition where an approved OneVision equivalent exists.

MudBlazor remains an implementation dependency; OneVision.UI owns the semantic application contract.

## Canonical components

### Actions
- `OVC-BUTTON` — semantic Primary / Secondary / Subtle action.
- `OVC-ICON-BUTTON` — compact semantic icon action with mandatory accessible label.
- `OVC-OVERFLOW-MENU` — vertical three-dot secondary/destructive action menu. Its trigger is always `OneVisionIcon.More` and is pinned to the far-right trailing edge of the row, card or owning control.

### Text and input
- `OVC-TEXT-FIELD`
- `OVC-TEXT-AREA`
- `OVC-NUMBER-FIELD`
- `OVC-SEARCH-BOX` — supporting search capability normally exposed by an owning component or pattern.
- `OVC-FILE-UPLOAD` — governed file browse/drag selection surface with selected-file presentation, application-supplied validation, optional removal and upload progress. Applications own what the file means and how it is processed.
- `OVC-CONNECTION-EDITOR` — reusable editor for a managed connection object. Governed connection types are SQL, FILE, LDAP and JSON, each with a dedicated settings surface. Shared editor behaviour includes Name, Description, Enabled, type selection, validation placement, Test Connection lifecycle, disabled/read-only treatment and Save/Cancel action placement. SQL and LDAP support conditional authentication credentials; applications own secure credential storage, validation, test execution and persistence and must not expose persisted secrets simply to repopulate the editor.

### Selection
- `OVC-SELECT`
- `OVC-MULTI-SELECT`
- `OVC-AUTOCOMPLETE`
- `OVC-CHECKBOX`
- `OVC-RADIO-GROUP`
- `OVC-TOGGLE`
- `OVC-SELECTION-LIST` — compact single- or multi-selection rows for bounded known options that benefit from descriptions. Do not use for hierarchical selection; use `OVC-TREE`.

### Dates and times
- `OVC-DATE-PICKER`
- `OVC-DATE-RANGE`
- `OVC-TIME-PICKER`
- `OVC-DATE-TIME` — read-only governed date/time display. Applications may supply a timezone; OneVision.UI owns consistent display formatting while preserving a machine-readable datetime value.

### Status and labels
- `OVC-PILL` — unified Status / Type / Source / Filter / Count pill.
- `OVC-BADGE` — terse attached count/text indicator for icon/navigation contexts. It is subordinate to `OVC-PILL` and is not the normal visible status vocabulary.

### Data and lists
- `OVC-DATA-GRID`
- `OVC-LIST` — compact non-tabular list for readable/actionable items when a grid is too heavy and the content is not hierarchical.
- `OVC-PAGINATION` — supporting list/grid paging behaviour.
- `OVC-SORTABLE-HEADER` — supporting grid-column sorting behaviour.
- `OVC-EMPTY-STATE`

### Navigation
- `OVC-TABS`
- `OVC-BREADCRUMB`
- `OVC-SIDE-NAV`
- `OVC-TREE` — generic searchable hierarchy for navigation/selection with `View` and `StructureEdit` modes. `StructureEdit` keeps normal hierarchy/search/focus behaviour while adding root/child/sibling creation, explicit rename/delete/properties actions and drag-and-drop movement. Drag movement covers sibling reorder (`Before` / `After`) and reparenting (`Child`). OneVision.UI owns the editing interaction, drag/drop state and visual affordances; applications own whether each operation is permitted, parent/child type rules, permissions, validation and persistence.
- `OVC-STEPPER` — user-driven ordered steps for a bounded multi-step task. This is not a background-process display; use `OVC-PROCESS-STAGES` for operational execution.

### Feedback
- `OVC-ALERT`
- `OVC-VALIDATION-SUMMARY`
- `OVC-TOOLTIP`
- `OVC-PROGRESS` — determinate or indeterminate; there is no separate Spinner contract.
- `OVC-SKELETON` — lightweight decorative loading placeholder for progressive hydration where full progress treatment would be excessive. Reduced-motion preferences suppress animation.
- `OVC-NOTIFICATION` — transient application feedback rendered by `OVNotificationHost` and raised through `OVNotificationService`. Tones are Success, Information, Warning and Error. The host owns bounded stacking/queueing, governed timeout clamping, optional persistence/manual dismissal, replacement keys, hover/focus pause behaviour, semantic ARIA live announcement and responsive placement. Notifications never steal keyboard focus. Do not use transient notifications for validation or information the user must retain; continue to use `OVC-ALERT` and `OVC-VALIDATION-SUMMARY` for persistent/important content.

### Containers
- `OVC-CARD`
- `OVC-PANEL`
- `OVC-ACCORDION`
- `OVC-DIVIDER` — semantic horizontal or vertical separator with optional label. Use sparingly; it separates adjacent regions without creating a new visual container.

### Overlays
- `OVC-DIALOG`
- `OVC-OVERLAY`
- `OVC-CONFIRM-DIALOG`

### Display
- `OVC-KEY-VALUE`
- `OVC-METADATA-LIST` — compact renderer for arbitrary metadata/property collections. It supports optional grouping, governed empty-value treatment, long-value wrapping, copyable values, optional monospace technical identifiers and structured values without card-per-property nesting. Applications own labels, values and domain meaning; OneVision.UI owns presentation.
- `OVC-METRIC`
- `OVC-SELECTION-SUMMARY`
- `OVC-COPYABLE-LINK` — visually distinct selectable text with `OneVisionIcon.Copy` at the trailing edge; clicking or keyboard activation copies the complete supplied value.
- `OVC-TIMELINE` — chronological event/history presentation for audit and operational history. It is event-oriented, not a process state machine.
- `OVC-PROCESS-STAGES` — ordered read-only operational stage sequence for runs/processes/jobs. Supported states are Pending, Running, Complete, Failed, Skipped and Warning. Applications own stage sequence and execution metadata; OneVision.UI owns progression layout, connectors, state appearance/icons, optional count/duration/timestamps/message placement, accessibility, loading/empty/error treatment and responsive density.

### App shell
- `OVC-TOOLBAR`
- `OVC-ACTION-BAR`

### Mapping
- `OVC-MAPPING-PAIR` — explicit directional source-to-target mapping interaction. Actual/discovered source data is rendered through `OVC-STRUCTURED-FIELD-TREE`; the target side remains the declared/templated contract representation. Pointer users may drag an eligible source primitive onto a target and keyboard users can select/focus a source then activate a target with Enter/Space. Applications supply eligibility/cardinality, constants/configuration capability, validation and callbacks.
- `OVC-MAPPING-JOIN` — symmetric/relational two-sided pairing interaction for actual/discovered data structures. Both sides are `OVC-STRUCTURED-FIELD-TREE`. Users may drag a primitive field from either tree onto an eligible primitive in the opposite tree, or select one field on each side and create/replace the join with the keyboard-accessible action. Every visible join pair owns a comparison/match method. OneVision.UI owns tree-to-tree drag lifecycle, eligible/ineligible/active drop states, paired/unpaired/required/read-only presentation, pair rows, method placement, validation, accessibility and responsive Manager density. Applications supply the two structured field trees, current joins, methods, eligibility/cardinality rules, field state metadata, validation and persistence callbacks.
- `OVC-MAPPING-RULE` — generic condition builder for deciding when a mapping or outcome occurs. Applications supply operands, operators, grouping rules, outcome options, validation and callbacks. OneVision.UI owns condition/group layout, AND/OR presentation, add/remove behaviour, nested groups when enabled, accessibility and responsive treatment.

`OVC-MAPPING-JOIN` must not infer cardinality. Endpoint reuse alone never means replacement. An exact existing left+right pair may be recognised as that existing relationship, and a join explicitly selected through the Replace interaction may target that specific relationship. Otherwise normal Create/drag-drop remains an add request subject to the consuming application's `CardinalityRule`. OneVision.UI owns the join interaction; the consuming application owns relationship eligibility and cardinality, including one-to-one, one-to-many, many-to-one or many-to-many policies.

Mapping controls standardise the interaction grammar, not domain semantics. Do not create application-specific Person/KPI/Entity/Allocation mapping components when the interaction can be expressed by Pair, Join or Rule. Do not overload `OVC-MAPPING-PAIR` with symmetric join semantics merely to avoid a separate relational contract.

### Structured data governance
- `OVC-STRUCTURED-FIELD-TREE` is the canonical representation whenever a OneVision control or pattern is inspecting actual/discovered data structure: profiled payloads, source fields, API/JSON/XML/object shapes, imported structures, or two-sided data joins.
- Do not replace inspected data with bespoke flat field lists merely because a particular screen is simpler today. Hierarchy, path, type, disabled state, search, focus, selection, drag and drop behaviours remain governed through the structured field tree.
- Structured profiles may optionally enable `RecordCollectionSelectionMode = SingleArray` and bind `SelectedRecordCollectionPath`. Only array nodes are eligible and exactly one path is designated at a time. OneVision.UI owns the array-only interaction, `Record collection` selected-state presentation, read-only behaviour, retention across equivalent rerenders and clearing an invalid selected path after genuine structure change. The application owns what the collection means, whether the capability is enabled, persistence, validation, mapping eligibility and extraction/iteration semantics. This capability is independent of normal structured-tree selection.
- Declared/templated mapping targets are different: they may use the target representation owned by their mapping contract rather than masquerading as discovered data. `OVC-MAPPING-PAIR` therefore remains Structured Field Tree → target contract; `OVC-MAPPING-JOIN` is Structured Field Tree ↔ Structured Field Tree. Record-collection selection does not change Mapping Pair semantics.
- `OVC-TREE` owns reusable drag-source and drop-target lifecycle primitives; `OVC-STRUCTURED-FIELD-TREE` constrains those behaviours to eligible primitive fields. `OVC-TREE` StructureEdit additionally owns the hierarchy-maintenance move grammar; consumers remain responsible for domain placement rules.

A typical matching-rule editor may therefore compose `OVC-OVERLAY` → app-owned rule metadata → `OVC-MAPPING-JOIN` → optional/additional `OVC-MAPPING-RULE` → `OVC-VALIDATION-SUMMARY` → `OVC-ACTION-BAR`.

### Structured data
- `OVC-STRUCTURED-FIELD-TREE`

## Semantic iconography

Applications use `OneVisionIcon` meanings rather than MudBlazor or Fluent implementation strings. `OneVisionIcon.More` is the vertical overflow ellipsis. `OneVisionIcon.Open` means open/navigate to a resource or view without implying edit.

## Foundation and page layouts

These are OVP contracts rather than OVC components:

- `OVP-000` — OneVision Application Foundation.
- `OVP-001` / `OVApplicationShell` — governed application shell.
- `OVP-010` / `OVStandardPage` — standard page composition.
- `OVP-011` / `OVListPage` — standard list-page composition. Persistent page actions live in the page-header action region; list-level actions have a dedicated trailing toolbar action region. Both are right-aligned. Row action columns are minimum-width and right-aligned, with overflow as the final trailing action.
- `OVP-012` / `OVDetailPage` — general detail/edit page composition.
- `OVP-013` / `OVBuilderPage` — builder-page composition.

The page header is owned by these page-layout contracts; it is not a separate `OVC-PAGE-HEADER` component.

## Pattern references

Only top-level reusable patterns are first-class `OVP-*` catalogue contracts. Tabs, sections, panes and other internal regions belong to their owning pattern and are not independent searchable contracts.

- `OVP-LIST-PAGE` — standard searchable/filterable/paginated list composed from the actual governed list-page regions.
- `OVP-CONNECTION-MANAGEMENT` — managed connection catalogue as a governed `OVListPage` + `OVDataGrid` + `OVOverlay` + `OVConnectionEditor` composition. The application owns credentials, testing, data persistence, permissions and server paging; the library owns reusable structure and interaction states. No separate `OVConnectionManager` control is required by default.
- `OVP-SOURCE-CONFIGURATION` — managed inbound source configuration with standard Overview, Connection, Profile and Mapping sections. **Connection references the reusable managed endpoint/access definition and owns everything required to reach it. Profile must never duplicate connection state.** Profile contains only the minimum remaining source-specific definition needed before `Run & Profile`: SQL uses T-SQL only (including stored-procedure execution when required); JSON/API may use an optional relative endpoint extension and profiles the returned JSON without a response-root selector; FILE uses only unresolved file interpretation such as header row and worksheet, never path/pattern/format controls already resolved by the connection; LDAP Profile supplies only the field/attribute names to retrieve, while host/port/authentication/base context remain connector-owned. Profiled structured sources may optionally designate one array as a logical record collection through the governed `OVC-STRUCTURED-FIELD-TREE` capability; this establishes profile context and does not alter Mapping Pair semantics. Mapping consumes the profile output through `OVC-MAPPING-PAIR` and optional `OVC-MAPPING-RULE`. See `docs/OVP-SOURCE-CONFIGURATION.md` for the full contract.
- `OVP-OPERATIONAL-DETAIL` — predominantly read-only status/detail composition for first-class operational/configuration objects. It composes the existing `OVP-012` page structure with identity/description, current status, optional `OVC-TOGGLE` for Enabled, one visually dominant primary operational action, secondary Edit action that opens `OVC-OVERLAY`, overflow for secondary/destructive actions, compact health/status summary, optional key/value metadata, optional latest event, optional `OVC-PROCESS-STAGES`, optional recent activity/history and alerts. Enabled belongs on the row/status surface and is not duplicated in the settings overlay. Persisted configuration editing belongs in an overlay, and read-only summary data must not be duplicated there unless it is genuinely editable. Destructive actions use governed confirmation. Applications own which fields/operations exist, permissions and health/business calculations; OneVision.UI owns composition, hierarchy, density, accessibility and responsive behaviour.
- `OVP-ENTITY-INSPECTOR` / `OVEntityInspector` — read-only in-context inspection of an application-owned business/domain entity using the existing governed overlay placements. It supports title/subtitle, application-supplied status/type/source pills, summary content, grouped sections or governed tabs, loading/empty/error states, refresh, close and optional application actions. `OVC-METADATA-LIST` is the standard compact arbitrary-property renderer inside the inspector when needed. OneVision.UI owns the interaction/layout/state/action grammar; applications own retrieval, values, permissions, refresh semantics and domain actions. See `docs/OVP-ENTITY-INSPECTOR.md` for the full contract.
- `OVP-ALLOCATION` / `OVAllocation` — governed compact target-first source-to-target allocation editor. The target is shown first as read-only context or through an application-constrained finder. Source system/type are optional selectors or fixed read-only context, source search is prominent, and completed selection replaces the result list with a selected-source summary and optional Change action. Effective dates appear only when `Always belonged` is off (From required, To optional). Apps own eligibility, cardinality, dates, permissions and persistence; OneVision.UI governs layout, selection presentation, validation/action placement and accessibility. See `docs/OVP-ALLOCATION.md`.

`OVC-RECORD-COMPARISON` is not currently a governed component. Continue to use existing grid/detail composition unless a consuming implementation demonstrates a genuinely reusable comparison interaction that cannot be expressed cleanly with current standards.

## Loading and transient feedback guidance

Use `OVC-SKELETON` for shape-preserving progressive loading, `OVC-PROGRESS` for explicit ongoing work, `OVC-NOTIFICATION` for transient feedback, `OVC-ALERT` for persistent important information, and `OVC-VALIDATION-SUMMARY` for validation the user must retain and resolve. These contracts are complementary rather than interchangeable.

## CSS isolation contract

OneVision.UI component `.razor.css` styles are part of the governed implementation. A consuming Blazor application must load its generated `{HostAssembly}.styles.css` bundle so Razor Class Library isolated styles are included. UI Studio loads `OneVision.UI.Demo.styles.css` explicitly. Critical components must not be considered visually verified if this generated stylesheet is absent.

## Removed compatibility names

The catalogue no longer retains retired aliases or wrapper components. Use the canonical contracts directly:

- Copy action → `OVC-ICON-BUTTON` with semantic Copy icon, or `OVC-COPYABLE-LINK` when the value itself is a copy target.
- Status / Type / Source / Filter / Count labels → `OVC-PILL`.
- Validation message → field validation or `OVC-VALIDATION-SUMMARY`.
- Spinner → `OVC-PROGRESS` in indeterminate mode.
- Field Mapping → `OVC-MAPPING-PAIR`; the older field-specific umbrella name is retired.

## Rule

If a reusable interaction is missing, identify a OneVision.UI gap and add a deliberate shared contract. Do not create duplicate aliases, parallel app conventions, or compatibility wrappers merely to preserve obsolete gallery behaviour.
