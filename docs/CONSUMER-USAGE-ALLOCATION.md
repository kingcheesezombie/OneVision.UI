# OVAllocation — consumer usage and acceptance

**Pattern:** `OVP-ALLOCATION` / `OVAllocation`
**Library baseline reviewed:** OneVision.UI 0.1.22 (`a2d5af44`)
**Reference consumer:** OneVision.EAM v0.99.15 (user-established baseline; live repository verification pending).

## Real EAM workflow (source of requirements)

From a selected Asset or Attribute in the EAM Entities list, a permitted operator selects Allocate. The editor opens immediately with that entity as its read-only target. Target provider is EAM, target identity/type/key are fixed and submitted unchanged. Source classification is ATTRIBUTE (an internal EAM constraint, not a UI input). EAM resolves eligible source systems and entity types from enabled allocation rules for the chosen target. Changing system/type invalidates an incompatible selected source. The operator searches eligible source entities by name, value or identifier via filtered paginated server requests; the entire catalogue is never loaded into the browser.

After choosing a source, search/results collapse to a named identity with Change action. Always belonged defaults on; disabling it reveals a required From and optional To. Cancel/ Create Allocation and valid/ submitting/ failed/ completed states are controlled by the consuming application and its existing allocation API. On successful save, EAM closes the overlay and refreshes the relevant view.

**Target-first allocation list:** present each relationship as Target entity ← Source entity, using the resolved source and target identity fields already returned in the verified v0.99.15 EAM-to-EAM API. Use the governed list/grid and overflow patterns, with type, identifier, effective dates and relationship status. Do not add N+1 identity calls or invent a generic allocation-list control.

## Reusable OneVision.UI behaviour

- Compact read-only target summary, optional finder and completed-source summary with Change.
- Any ordered, app-supplied `SourceCriteria` (or legacy SourceSystem/SourceType slots).
- In-overlay body composition with `EmbeddedInOverlay`, leaving overlay header/footer/focus to `OVOverlay`.
- Conditional date controls and responsive density.
- Search loading, no-results, eligibility-unavailable, selection invalidity feedback, submitting, submission-error, completed and read-only states without losing selected identities.

## EAM-only ownership and constraints

- Eligibility and dependent selection from enabled EAM allocation rules.
- Target provider, target entity key and type, source ATTRIBUTE classification, permissions, effective-date validation, allocation direction and persistence.
- Source search query, SQL filtering, pagination, result identities, refresh after save, and disabled/valid action logic.
- App-owned `OVOverlay` binding (`SaveRequested`, `SaveDisabled`, `Busy`, close).

## Known usability issues to guard against

- Redundant target provider/type/classification inputs in known-target mode.
- Duplicate pattern/overlay headings and footers.
- Search/results remaining visible after completed selection.
- Initial/full-screen loading or submission error erasing target/source context.
- Full-catalogue client-side entity search or uncontrolled source/type choices.
- Source and target identity conflated in allocation-list rows.

## Acceptance scenarios

1. Allocate action on EAM Entities row launches editor with that target without initial full-catalogue load.
2. Source system/type permitted by enabled rules only; classification fixed internally.
3. Source search performs server-side filtered/paginated queries and supports value/name/identifier.
4. Changing parent choice clears invalid dependent selections and previous source.
5. Selected source hides search/results; Change restores the picker.
6. Always belonged hides From/To, otherwise requires From, optionally accepts To.
7. Invalid and failed submission retain target/source; Create is disabled until valid and during submission.
8. Successful allocation uses existing EAM direction/API and refreshes the originating view.
9. Allocations list is target-first and uses resolved API identity objects.
10. Generic known-target and target-finder scenarios remain functional; page and overlay variants have accessible focus/scroll.

## Implementation references and verification

- UI source: `src/OneVision.UI/Components/OVAllocation.razor` and `.razor.css`.
- UI contract: `docs/OVP-ALLOCATION.md`.
- Gallery: `demo/OneVision.UI.Demo/Components/AllocationPatternDemo.razor`, `PatternWorkbenchCatalog.cs` and `PatternPreviewHost.razor`.
- Tests: `tests/OneVision.UI.Tests/AllocationPatternContractTests.cs` and `tests/OneVision.UI.BrowserTests/AllocationPatternBrowserTests.cs`.
- EAM repository: `kingcheesezombie/OneVision.EntityAllocationManager`; actual v0.99.15 editor, Entities list, Allocations list and API source paths **not verified with the current GitHub connection**. Do not use an older checkout or infer exact code paths.
- IDM and HDM are not documented as confirmed consumers until their requirements and implementations are inspected.

**Completion condition:** Library build, tests, published gallery SHA, and EAM DEV acceptance are independently verified; a library-only release does not close the EAM acceptance criteria.
