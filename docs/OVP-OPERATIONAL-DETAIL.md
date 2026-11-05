# OVP-OPERATIONAL-DETAIL — operational resource overview

`OVOperationalDetail` composes `OVDetailPage` with application-owned identity, status, toggle, health summary, metadata, latest event, process stages, recent activity, alerts and command slots. It does not own scheduling, job execution, persistence, permissions or polling.

## Level 1 contract

The existing `Loading` and fatal `ErrorMessage` states remain unchanged. Opt-in `Refreshing` and `RefreshErrorMessage` preserve previously loaded health/events and selected context; opt-in `Busy` sets the workspace busy semantic and `ActionErrorMessage` shows a non-destructive action failure without clearing status or metadata. The consumer must disable unsafe application-supplied action fragments when busy and decide when retry/run is allowed.

## Level 2 reference

`/quality/workflows` demonstrates a hierarchy edit feeding an operational detail page, and an administration journey from catalogue to progressive rule, schedule and operational overview. Synthetic events are not live job monitoring. Application teams own refresh lifetime, log provenance, security and status transitions. OVP-000 remains unchanged.
