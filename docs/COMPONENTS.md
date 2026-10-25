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

### Selection
- `OVC-SELECT`
- `OVC-MULTI-SELECT`
- `OVC-AUTOCOMPLETE`
- `OVC-CHECKBOX`
- `OVC-RADIO-GROUP`
- `OVC-TOGGLE`

### Dates and times
- `OVC-DATE-PICKER`
- `OVC-DATE-RANGE`
- `OVC-TIME-PICKER`

### Status and labels
- `OVC-PILL` — unified Status / Type / Source / Filter / Count pill.

### Data and lists
- `OVC-DATA-GRID`
- `OVC-PAGINATION` — supporting list/grid paging behaviour.
- `OVC-SORTABLE-HEADER` — supporting grid-column sorting behaviour.
- `OVC-EMPTY-STATE`

### Navigation
- `OVC-TABS`
- `OVC-BREADCRUMB`
- `OVC-SIDE-NAV`
- `OVC-TREE`

### Feedback
- `OVC-ALERT`
- `OVC-VALIDATION-SUMMARY`
- `OVC-TOOLTIP`
- `OVC-PROGRESS` — determinate or indeterminate; there is no separate Spinner contract.

### Containers
- `OVC-CARD`
- `OVC-PANEL`
- `OVC-ACCORDION`

### Overlays
- `OVC-DIALOG`
- `OVC-OVERLAY`
- `OVC-CONFIRM-DIALOG`

### Display
- `OVC-KEY-VALUE`
- `OVC-METRIC`
- `OVC-SELECTION-SUMMARY`
- `OVC-COPYABLE-LINK` — visually distinct selectable text with `OneVisionIcon.Copy` at the trailing edge; clicking or keyboard activation copies the complete supplied value.

### App shell
- `OVC-TOOLBAR`
- `OVC-ACTION-BAR`

### Mapping
- `OVC-MAPPING-PAIR` — explicit directional source-to-target mapping interaction. Structured/hierarchical source items use `OVC-STRUCTURED-FIELD-TREE`; targets are rendered as governed mapping rows with mapped/unmapped/required/invalid/read-only states. Pointer users may drag an eligible source onto a target and keyboard users can select/focus a source then activate a target with Enter/Space. Applications supply eligibility/cardinality, constants/configuration capability, validation and callbacks.
- `OVC-MAPPING-JOIN` — symmetric/relational two-sided pairing interaction. It answers “which item on the left should be matched to which item on the right, and how should that pair be compared?”. OneVision.UI owns the two catalogue panes, independent search placement, paired/unpaired and required states, create/replace/remove interaction, visible join-pair rows, per-pair method selector placement, validation presentation, keyboard accessibility and responsive Manager density. Applications supply left/right catalogues, current joins, available methods, eligibility/cardinality rules, required/disabled/read-only metadata, validation and persistence callbacks. Flat catalogues are built in; `LeftCatalogueContent` / `RightCatalogueContent` may compose governed `OVC-TREE` or `OVC-STRUCTURED-FIELD-TREE` while retaining the join shell/search/pair-row contract.
- `OVC-MAPPING-RULE` — generic condition builder for deciding when a mapping or outcome occurs. Applications supply operands, operators, grouping rules, outcome options, validation and callbacks. OneVision.UI owns condition/group layout, AND/OR presentation, add/remove behaviour, nested groups when enabled, accessibility and responsive treatment.

Mapping controls standardise the interaction grammar, not domain semantics. Do not create application-specific Person/KPI/Entity/Allocation mapping components when the interaction can be expressed by Pair, Join or Rule. Do not overload `OVC-MAPPING-PAIR` with symmetric join semantics merely to avoid a separate relational contract.

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
- `OVP-012` / `OVDetailPage` — detail/edit page composition.
- `OVP-013` / `OVBuilderPage` — builder-page composition.

The page header is owned by these page-layout contracts; it is not a separate `OVC-PAGE-HEADER` component.

## Pattern references

Only top-level reusable patterns are first-class `OVP-*` catalogue contracts. Tabs, sections, panes and other internal regions belong to their owning pattern and are not independent searchable contracts.

- `OVP-LIST-PAGE` — standard searchable/filterable/paginated list composed from the actual governed list-page regions.
- `OVP-SOURCE-CONFIGURATION` — managed inbound source configuration with standard Overview, Connection, Profile and Mapping sections. The Mapping section composes `OVC-MAPPING-PAIR` and may also compose `OVC-MAPPING-RULE` when source-specific rules are required. The application owns source type, connection fields, profile/schema logic, domain labels, validation rules and persistence; OneVision.UI owns the reusable section structure, action placement, validation placement, density and responsive treatment.

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
