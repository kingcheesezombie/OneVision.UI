# OVP-012 – Detail / Edit / Workspace Page

## Purpose

OVP-012 governs a full-width focused application surface beneath the OneVision application foundation. It supports traditional detail/edit pages and focused workspaces opened from a list or another parent context.

The canonical review composition is:

`Browse → open → focus → review/edit → act → return or continue`

or, structurally:

`OVListPage → open item → OVDetailPage → Back or Review next`

The collection is the previous level of navigation. A focused review workspace must not retain the entire collection as a permanent left rail unless the application genuinely has a separate persistent hierarchical/navigation requirement.

Applications own routing and domain data; OneVision.UI owns reusable page regions, state presentation, selection grammar and action hierarchy.

## Canonical cookbook

The public demo's neutral **List to Workspace** example is the primary implementation cookbook. It uses neutral `Review Item A/B/C` records and demonstrates the reusable pattern independently of Person Merge/IDM semantics.

Person Merge remains a richer application-composition example that consumes the same governed components. There is deliberately no merge-specific `OVC`.

## List state – `OVListPage`

A collection/list state normally contains:

1. title and description;
2. optional page actions;
3. compact `OVContextSummary` and relevant process state;
4. search/filter/sort controls;
5. governed rows/cards representing available work items;
6. `OVPagination` where required.

`OVListPage.Summary` is the governed summary slot above the list toolbar/results.

Applications should preserve useful list context when returning from a focused workspace: search, filters, paging, sort, scroll position and last-opened/selected item where practical. OneVision.UI demonstrates the interaction; the application/router owns persistence and restoration of its actual state.

## Activation model

For actionable review cards the default is **whole-card activation**:

- pointer activation opens/selects;
- hover and focus make actionability obvious;
- Enter/Space activates the card;
- the card exposes semantic pressed/disabled state;
- a separate `Open` button is not routinely duplicated.

Use an explicit primary card action only when it performs a genuinely distinct action beyond the whole-card selection/open action, or where a specific application accessibility/discoverability requirement justifies it.

## Focused workspace – `OVDetailPage`

The visual reading order is governed and predictable:

1. Back action;
2. Title;
3. Description;
4. Status;
5. Context summary and relevant process state;
6. page-level actions;
7. full-width main content;
8. footer context and actions.

Applications should not rearrange these regions into competing workspace header conventions.

The footer supports informational context on the left and primary/secondary actions on the right. `StickyFooter` may be enabled for long operational workspaces so important actions remain reachable. On narrow widths the footer stacks cleanly.

## Back, busy and unsaved state

Navigation rules remain application-controlled but use governed semantics:

- disable Back while a destructive/server operation is actively running when leaving would be unsafe;
- use the existing OneVision confirmation-dialog convention before abandoning unsaved local changes;
- allow ordinary navigation when state is read-only or purely server-side and does not require a local discard decision.

These are generic workspace semantics, not merge rules.

## Optional Review next

Queue/review applications may expose **Review next** as a secondary workspace action. The application owns ordering and eligibility. OneVision.UI governs only its placement and hierarchy. `Review next` is separate from Back and is not baked into `OVDetailPage`.

## Context summary – `OVContextSummary`

`OVContextSummary` is one compact operational strip, not a dashboard/card collection.

- metrics align consistently;
- values are visually stronger than labels;
- supporting text remains secondary;
- long labels truncate/wrap without producing uneven blocks;
- embedded process state is related but subordinate;
- metrics and process wrap cleanly at narrower widths.

Content-state precedence is:

`loading → error → empty → content`

## Process status – `OVProcessStatus`

`OVProcessStatus` represents one durable application-owned process state. Its hierarchy is:

`State → label → phase/message → progress → timestamps`

Semantic states are `Queued`, `Running`, `Completed`, `Failed`, `Stale`.

Presentation modes are:

- `Display = Compact` – intended inside `OVContextSummary` and other dense operational strips;
- `Display = Detailed` – intended for a dedicated process/operational area.

The underlying process contract is identical in both modes. Applications must not manually hide individual fields to reproduce these layouts.

Visual semantics:

- `Running` is immediately identifiable but restrained;
- `Failed` is clear without becoming visually aggressive;
- `Completed` is intentionally quiet;
- `Stale` is noticeable but not styled as an application error;
- `Queued` is distinct from active execution.

Progress is clamped to the valid 0–100% presentation range.

## Selectable information cards – `OVCard`

`OVCard` is the reusable information/selectable/actionable record surface. Selection is explicit without relying on colour alone.

Distinct states are governed for normal, hover, focused, selected, disabled and busy. Selected state includes a visible marker, semantic pressed state, border/background emphasis and keyboard-focus compatibility. Selected + busy remains visibly selected.

Selection target and child action regions remain separate, preventing nested interactive controls. Disabled/busy cards suppress activation of both the card target and child action region.

### Card action hierarchy

The demo must not imply that every card needs every action region.

Typical administrative use is:

- whole card = select/open;
- `PrimaryAction` = a distinct high-value action beyond selection, used sparingly;
- `SecondaryActions` = occasional inline supporting actions;
- `OverflowActions` = destructive/infrequent/less common actions using the established overflow convention;
- `Actions` = legacy compatibility region, not preferred for new composition.

Keep cards information-dense and visually clean.

## Card collection – `OVCardCollection`

`OVCardCollection` owns governed responsive card layout and content-state precedence. It does not add a nested decorative surface.

Loading uses content-shaped card skeletons that resemble the final card structure rather than generic large rectangles.

## Empty/error/loading specificity

Applications supply context-specific text while OneVision.UI governs presentation. Distinguish, where relevant:

- no records exist;
- no search/filter matches;
- no item selected;
- item no longer exists;
- queue completed.

Errors are never represented as empty state.

Workspace loading should approximate the final header/summary/content/card structure using governed skeletons.

## Responsive behaviour

Verify desktop, medium-width administrative windows and narrow/mobile widths. Expected behavior:

- summary items wrap gracefully;
- cards reflow from multi-column to single-column;
- metadata stays readable;
- footer actions stack sensibly;
- process status does not overflow;
- no empty persistent navigation column appears;
- Back remains obvious;
- long titles/statuses do not push actions off-screen;
- useful information is not simply hidden to make the layout fit.

The OneVision administrative density remains compact: approximately 10pt body text, restrained whitespace/shadows/borders and compact pills/actions.

## Builder boundary

`OVBuilderPage` remains for genuine builder/editor experiences where a persistent structure/navigation pane is intrinsic to the task. Do not use it as the default container for review/detail workspaces merely because it exposes a left-hand region.

## Ownership

- `OVListPage` = list/queue surface.
- `OVDetailPage` = focused workspace.
- `OVContextSummary` = compact contextual metrics/state.
- `OVProcessStatus` = durable operation/process presentation.
- `OVCard` = reusable information/selectable/actionable record surface.
- `OVCardCollection` = governed collection layout.

Applications compose these and own domain rules, routes, state persistence, permissions, validation, process truth, APIs and the consequences of actions.

## Contract testing

Contract/browser tests should cover semantic regions and behavior rather than incidental CSS, including:

- `OVDetailPage` Back, Summary, Content and Footer regions;
- loading/error/empty/content precedence;
- `OVCard` Enter/Space activation and selected state;
- disabled/busy suppression and child-action isolation;
- `OVProcessStatus` state/display semantics and progress boundaries;
- `OVContextSummary` content-state contract;
- canonical list→workspace→Back flow without a persistent navigation rail;
- neutral cookbook use of governed components rather than recreated interaction markup;
- responsive desktop/medium/narrow behavior.

## Contract impact assessment

These refinements strengthen OVP-012 and its supporting reusable components. They do **not** alter the externally observable `OVP-000 – OneVision Application Foundation` contract.

**OVP-000 version impact: none. No version increase is required.**
