# APP COMPOSITION — Merge Review Workspace

## Status

Reference application composition. This is not a new OVC or OVP contract.

The composition is driven by the IDM merge-candidate use case but intentionally contains no IDM-specific reusable component or domain implementation inside OneVision.UI.

## Purpose

Demonstrate how an application can compose existing governed OneVision.UI assets into a dense review workspace where a reviewer can:

1. select a proposed merge group;
2. compare candidate people;
3. choose exactly one survivor;
4. remove a candidate from the current proposed merge set without deleting or durably excluding the Person;
5. review proposal evidence and conflicts;
6. review an application-supplied resulting-record preview;
7. perform a deliberate durable `Not same person` action separately from transient removal;
8. execute the application-owned merge operation.

## Existing governed assets used

- `OVP-000` application foundation at the consuming application's composition root;
- `OVP-013 / OVBuilderPage` for the split navigator/workspace layout;
- `OVTextField` for search;
- `OVSelect` for review-state filtering;
- `OVButton` for refresh, secondary actions and primary merge;
- `OVPill` for confidence, state, source and selected-primary semantics;
- `OVMetadataList` for proposal evidence and resulting-record metadata;
- `OVAlert` for blocking/review conditions;
- `OVEmptyState` for no-selection/no-result states;
- `OVSkeleton` for initial loading.

Candidate Person cards, evidence rows and resulting-record composition are application-owned arrangements of those primitives. They are not promoted into a new shared component.

## Interaction model

The normal review flow is:

`Select merge group → compare candidate cards → select survivor → remove non-participants → review evidence/result → Merge`

The left navigator remains in place while the selected group changes. Candidate selection and transient removal update the workspace in place without page navigation.

### Primary/survivor

Exactly one included candidate should be selected as the survivor before merge can proceed. The selected state must be explicit in content as well as visual styling, for example `PRIMARY` and `Selected survivor`.

Candidate cards must be keyboard selectable. A consuming application owns the current primary identifier and validation.

### Remove from merge set

`Remove from merge set` is transient review state. It means that Person is not included in the current proposed merge operation.

It must not be implemented as deletion or as the durable exclusion contract.

The selected primary should not normally be removable until another included Person is selected as primary.

### Not same person

`Not same person` is deliberately separate from transient removal. Its durable meaning, persistence and regeneration/exclusion semantics are owned by the consuming application.

The UI should explain its consequence and present it as a deliberate secondary review action rather than a casual per-card control.

## Evidence and conflicts

Applications supply readable evidence describing why candidates were proposed together. The composition should surface:

- matching fields;
- agreements and conflicts;
- matching rule/evidence source;
- confidence;
- ambiguity/candidate count;
- source/allocation evidence where relevant.

Do not treat multiple legitimate assets belonging to one Person as a conflict. Ambiguity such as one asset plausibly matching more than one Person remains application/domain logic.

## Resulting record preview

The application supplies the resulting Person/record preview using its actual merge rules. OneVision.UI does not infer conflict resolution.

When the selected primary changes, the application should recompute the preview immediately. If a result cannot be determined by the application's rules, the preview should label it unresolved instead of inventing a value.

## Action hierarchy

The application-owned `Merge` action is primary and should remain bottom/right in the review workspace.

Secondary actions can include refresh/re-evaluate and durable `Not same person`. Per-candidate transient removal is subordinate to the primary merge action.

The application owns eligibility and must disable merge when, at minimum:

- fewer than two candidates remain included;
- there is not exactly one primary;
- the proposal is no longer eligible;
- an unresolved condition blocks merge;
- merge execution is already running.

## State updates

Applications should preserve the current group across refresh where it still exists. After a successful merge or durable exclusion, remove/update the reviewed group, select the next sensible group and clear stale candidate content.

Use skeletons for initial loading. In-place refresh should preserve usable context where possible.

## Ownership

### OneVision.UI owns

- the reused governed components and their contracts;
- layout/density/focus/disabled/loading semantics supplied by those components;
- the public synthetic reference composition in the gallery.

### Application owns

- merge grouping and regeneration;
- candidate data and identity fields;
- current primary/survivor state;
- transient candidate inclusion/removal;
- durable exclusions;
- evidence/scoring semantics;
- ambiguity semantics;
- allocation interpretation;
- resulting-record computation;
- merge eligibility;
- execution, persistence, auditing and completion feedback.

## OneVision.UI gap assessment

No new reusable OneVision.UI capability is required for this composition at present.

Do **not** introduce `OVC-RECORD-COMPARISON` solely for merge review. Reconsider that component only if real consuming-app implementation demonstrates that `OVBuilderPage + OVMetadataList + OVPill + existing grids/detail primitives` cannot express reusable comparison behaviour cleanly.

## OVP-000 impact

None. This is a reference application composition using the existing foundation and components. `OVP-000` must not advance for this change.
