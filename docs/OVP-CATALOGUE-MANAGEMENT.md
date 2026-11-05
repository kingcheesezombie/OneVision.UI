# OVP-CATALOGUE-MANAGEMENT — Catalogue Management

Status: Draft

## Purpose

`OVP-CATALOGUE-MANAGEMENT` governs management of small reusable catalogues and reference/configuration definitions. It is intended for compact application-owned or governed catalogues such as classifications, categories, semantic definitions and configuration definitions.

It is not intended for high-volume operational data grids, transactional work queues or domain-specific processing screens.

## Governed composition

The pattern is implemented by `OVCatalogueManager` and `OVCatalogueEditor` using existing OneVision.UI primitives:

- `OVListPage` for the page shell and primary action region;
- `OVSearchBox` for search placement;
- application-supplied governed filters such as `OVSelect`;
- `OVDataGrid` for compact results;
- `OVToggle` where a consumer supports enable/disable behaviour;
- `OVOverflowMenu` for secondary/destructive row actions;
- `OVPagination` with a default page size of 25;
- `OVOverlay` for Add/Edit;
- existing feedback, loading, empty and validation controls.

The pattern is domain-neutral. It does not define a catalogue data model or persistence mechanism.

## Catalogue manager contract

`OVCatalogueManager` owns:

- title and description placement;
- `Add {ItemName}` as the primary page action when the catalogue is editable;
- search placement and accessible toolbar grouping;
- optional application-supplied filters;
- compact grid presentation;
- loading, error, empty and no-results states;
- top-right/list-page pagination placement;
- read-only suppression of the Add action;
- responsive toolbar reflow;
- a governed `ov-catalogue-manager__secondary-column` marker for lower-priority columns that may collapse on narrow layouts.

Applications provide the grid header/body content so catalogue schemas remain application-owned.

## Row behaviour

The primary display field should be visually clear and may include secondary descriptive text. Enabled state is opt-in and should use `OVToggle`. Applications determine whether a state change is permitted and own the callback/persistence consequence.

Edit is normally the directly available row action. Secondary and destructive actions use `OVOverflowMenu`. Applications decide which actions exist. Destructive operations must use the existing governed confirmation behaviour when confirmation is required.

## Editor contract

`OVCatalogueEditor` wraps `OVOverlay` with `RightSide` placement. Its semantic state is:

- `Mode`: `Add` or `Edit`;
- `ItemName` and optional explicit `Title`/`Description`;
- application-supplied `ChildContent` fields;
- `IsDirty`;
- `IsBusy`;
- `IsValid`;
- `ReadOnly`;
- `SaveRequested`;
- optional application-supplied validation summary.

Save is disabled when the application reports the editor invalid or read-only. Dirty-close confirmation, busy behaviour, close behaviour, focus management and focus restoration remain owned by `OVOverlay`.

## Semantic states

The manager supports populated, loading, error, empty, no-results and read-only states. Empty catalogue and no-results states are deliberately distinct: an empty catalogue invites creation when permitted, while no-results explains that the current search/filter has no matches.

Search, filters and pagination remain usable in read-only mode. Mutation actions are suppressed or made read-only by the consuming composition.

## Responsive behaviour

The normal desktop composition is a compact table. At narrower widths the toolbar stacks naturally, lower-priority columns may use the governed secondary-column marker to collapse, and the primary identity/state/actions must remain available. The editor overlay fields stack through normal governed field behaviour. The pattern does not introduce a separate mobile interaction model.

Validate consuming compositions at approximately 1280px, 760px and 390px and ensure the pattern does not create page-level horizontal overflow.

## Ownership boundary

OneVision.UI owns the management experience: page/action placement, toolbar composition, compact results surface, standard states, pagination, Add/Edit overlay composition, dirty/busy/read-only presentation, responsive behaviour and accessibility.

Consuming applications own catalogue data, field definitions, columns, persistence, validation rules, permissions, searchable properties/matching logic, filters, whether Enabled is supported, row actions and the domain consequences of mutations.

## Example

The gallery demonstrates the pattern with non-operational Standard Details data (`Name`, `Description`, `Data Type`, `Enabled`). This is a demonstration consumer only; no Standard Details or IDM-specific model/persistence exists in the reusable pattern.

## OVP-000 impact

This pattern does not alter the application composition-root contract. OVP-000 remains unchanged.

## Operational-quality refinements (0.1.24)

`OVCatalogueManager` supports `Refreshing`, `RefreshErrorMessage` and `RetryRequested`, in addition to its existing initial Loading/Error/Empty/NoResults states. A transient refresh leaves the grid visible; a recoverable refresh failure shows feedback while retaining records and filters. A fatal load error may expose a governed Retry action when the consumer supplies a callback. Header actions are absent rather than empty when Add is unavailable, and an empty catalogue shows its Add affordance once within the empty state.

`OVCatalogueEditor.SaveErrorMessage` shows a recoverable error above application-supplied fields without clearing entered values or changing the overlay's dirty/focus rules. Consumers own save validity and persistence, must disable their own field fragments appropriately during busy/read-only states, and may use the existing `OVPagination` for server-side paging. The gallery distinguishes initial loading, refreshing, refresh failure, editor-save failure, and true read-only composition.

## Level 2 workflow recheck — 0.1.25

The synthetic administration reference starts in Catalogue Management and moves to progressive rule editing, scheduling and operational detail. Catalogue-specific search/filter, no-results vs empty, refresh-recovery, read-only and Add/Edit overlay capabilities remain governed by this pattern's workbench. Domain catalogue query and persistence remain application-owned.
