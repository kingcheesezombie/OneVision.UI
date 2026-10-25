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
