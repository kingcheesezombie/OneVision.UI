# OVC-SCHEDULE-BUILDER

`OVC-SCHEDULE-BUILDER` is the governed reusable schedule-definition control for OneVision applications.

## Scope

The component owns the reusable interaction grammar for:

- schedule name;
- schedule type states: `Day`, `Week`, `Month`, `Interval`;
- type-specific recurrence settings;
- start/end bounds;
- a human-readable schedule summary;
- item type selection;
- searchable available-items pane;
- committed-items pane;
- explicit Add / Remove movement between panes;
- responsive layout, disabled/read-only states and save/cancel action placement.

Applications own:

- what a schedule means;
- available item types and item data;
- item eligibility and permissions;
- validation beyond the component's basic input constraints;
- persistence, execution and job-processing semantics.

## Schedule states

### Day

Defines an every-N-days cadence, run time, start date and optional end date.

### Week

Defines an every-N-weeks cadence, run time, selected weekdays, start date and optional end date.

### Month

Supports either a numeric day of month or an ordinal weekday (for example first Monday / last Friday), plus run time, start date and optional end date.

### Interval

Defines an every-N-minutes cadence with an optional active time window plus start date and optional end date.

## Item commitment

The item flow is a deliberate dual-pane composition:

`Search list -> Committed items`

Search and selection do not commit items by themselves. The user explicitly adds or removes selected items. The application provides the available items and persists the resulting committed item keys.

## Contract

The canonical implementation is `OVScheduleBuilder` with `OVScheduleBuilderValue`, `OVScheduleType`, `OVScheduleMonthMode` and `OVScheduleItem`.

## Level 1 and Level 2 refinement — 0.1.25

`ShowItems` (default true) semantically hides optional item assignment for schedules that only define recurrence. `Busy`, `BusyLabel`, `SaveErrorMessage` and `SavedMessage` preserve the configured recurrence and committed items while showing save feedback. `EmbeddedInOverlay` delegates the footer to the existing `OVOverlay`; `ShowActions` remains supported. Mutation callbacks are guarded during Busy/Disabled/ReadOnly states.

The component workbench exposes item assignment, actions, read-only, busy and save-error independently of Day/Week/Month/Interval. The operations workflow composes Catalogue → Progressive Rule → Schedule → Operational Detail. Applications still own recurrence validity, enabled item types, runtime scheduling and all persistence.
