# OneVision.UI Component Catalogue

This document is the stable vocabulary for shared OneVision controls. Applications should prefer these controls over direct MudBlazor usage.

## Current

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

## Planned higher-level patterns

- `OVListPage`
- `OVEditOverlay`
- `OVConnectionManager`
- `OVScheduleEditor`
- `OVActivityList`
- `OVPickerDialog`
- `OVIntegrationCard`
- `OVBuilderWorkspace`
- `OVTreeWorkspace`

## Rule

MudBlazor is an implementation dependency, not the OneVision application contract. New application features should use an `OV*` component where one exists. If a reusable interaction is missing, establish the OneVision component/pattern here rather than duplicating application-specific styling.
