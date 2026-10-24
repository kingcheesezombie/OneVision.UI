# OneVision.UI Component Catalogue

This document describes the canonical shared OneVision component vocabulary. Applications should use these contracts rather than direct MudBlazor composition where an approved OneVision equivalent exists.

MudBlazor remains an implementation dependency; OneVision.UI owns the semantic application contract.

## Canonical components

### Actions
- `OVC-BUTTON` — semantic Primary / Secondary / Subtle action.
- `OVC-ICON-BUTTON` — compact semantic icon action with mandatory accessible label.
- `OVC-OVERFLOW-MENU` — vertical three-dot secondary/destructive action menu.

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

### App shell
- `OVC-TOOLBAR`
- `OVC-ACTION-BAR`

### Structured data
- `OVC-STRUCTURED-FIELD-TREE`

## Foundation and page layouts

These are OVP contracts rather than OVC components:

- `OVP-000` — OneVision Application Foundation.
- `OVP-001` / `OVApplicationShell` — governed application shell.
- `OVP-010` / `OVStandardPage` — standard page composition.
- `OVP-011` / `OVListPage` — standard list-page composition.
- `OVP-012` / `OVDetailPage` — detail/edit page composition.
- `OVP-013` / `OVBuilderPage` — builder-page composition.

The page header is owned by these page-layout contracts; it is not a separate `OVC-PAGE-HEADER` component.

## Pattern references

Only top-level reusable patterns are first-class `OVP-*` catalogue contracts. Tabs, sections, panes and other internal regions belong to their owning pattern and are not independent searchable contracts.

For example, `OVP-LIST-PAGE` is a pattern contract. Internal implementation regions such as its Results/Table area are not separate library items.

## Removed compatibility names

The catalogue no longer retains retired aliases or wrapper components. Use the canonical contracts directly:

- Copy action → `OVC-ICON-BUTTON` with semantic Copy icon.
- Status / Type / Source / Filter / Count labels → `OVC-PILL`.
- Validation message → field validation or `OVC-VALIDATION-SUMMARY`.
- Spinner → `OVC-PROGRESS` in indeterminate mode.

## Rule

If a reusable interaction is missing, identify a OneVision.UI gap and add a deliberate shared contract. Do not create duplicate aliases, parallel app conventions, or compatibility wrappers merely to preserve obsolete gallery behaviour.
