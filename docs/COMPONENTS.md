# OneVision.UI Component Catalogue

This document is the stable vocabulary for shared OneVision controls and structural patterns. Applications should prefer these contracts over direct MudBlazor composition where an approved OneVision equivalent exists.

## Current components

### `OVButton`
Standard OneVision action button.

Initial contract:
- `Variant`
- `Color`
- `Disabled`
- `OnClick`
- `StartIcon`
- `ChildContent`

### `OVStatusPill`
Compact status indicator.

Initial tones:
- Neutral
- Info
- Success
- Warning
- Danger

## Foundation patterns

### `OVP-001` / `OVApplicationShell`
Governed application shell containing:
- identity/logo region;
- app title/subtitle;
- header actions;
- application navigation;
- main content host;
- footer.

### `OVP-010` / `OVStandardPage`
Standard page title, optional description, actions and content.

### `OVP-011` / `OVListPage`
Standard list page with title/actions, toolbar, results and pagination regions.

### `OVP-012` / `OVDetailPage`
Detail/edit page with title, status, actions, content and footer actions.

### `OVP-013` / `OVBuilderPage`
Two/three-pane builder page with optional toolbar, navigation pane, main canvas and optional inspector.

These patterns govern structure and shared spacing only. Applications own their domain content and determine when actions/states apply.

## Planned shared primitives

- `OVSearchBox`
- `OVSelect`
- `OVMultiSelect`
- `OVAutocomplete`
- `OVToggle`
- `OVDataGrid`
- `OVPagination`
- `OVTabs`
- `OVDialog`
- `OVOverflowMenu`
- `OVTooltip`
- `OVTree`

## Other higher-level patterns

- `OVEditOverlay`
- `OVConnectionManager`
- `OVScheduleEditor`
- `OVActivityList`
- `OVPickerDialog`
- `OVIntegrationCard`
- `OVTreeWorkspace`

## Rule

MudBlazor is an implementation dependency, not the OneVision application contract. New application features should use an `OV*` component or approved `OVP-*` pattern where one exists. If a reusable interaction is missing, establish the OneVision component/pattern here rather than duplicating application-specific styling.
