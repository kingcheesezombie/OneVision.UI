# OVP-012 – Detail / Edit / Workspace Page

## Purpose

OVP-012 governs a full-width focused application surface beneath the OneVision application foundation. It supports traditional detail/edit pages and focused workspaces opened from a list or another parent context.

The pattern does not require or imply a left navigation pane. Applications own routing and domain data; OneVision.UI owns the reusable page regions, state presentation, selection grammar and action hierarchy.

## Governed composition

A focused OVP-012 workspace may contain:

1. optional Back action;
2. title and subtitle/description;
3. compact status/header actions;
4. optional contextual summary strip;
5. main content region;
6. optional footer context;
7. footer actions with one clear primary action and subordinate secondary/overflow actions.

The application decides when a list item opens a workspace and what Back means. OneVision.UI does not own application routing.

## Context summary

`OVC-CONTEXT-SUMMARY` (`OVContextSummary`) provides a compact horizontal strip for state that is useful at the top of a tab or focused workspace.

It is intentionally not a dashboard/card grid. Summary items contain a label, value, optional supporting text and optional semantic status tone. A summary may also embed a current process status.

On narrower screens summary items wrap and the optional process region moves beneath the metrics.

## Process status

`OVC-PROCESS-STATUS` (`OVProcessStatus`) represents one durable application-owned process state. It is distinct from `OVC-PROCESS-STAGES`, which represents ordered stages within a run.

Semantic states are:

- `Queued`
- `Running`
- `Completed`
- `Failed`
- `Stale`

The application supplies semantic data such as:

```text
State = Running
Label = "Refreshing catalogue"
ProgressCurrent = 1284
ProgressTotal = 5930
StartedAt = ...
Phase = "Evaluating records"
Message = "Applying the current validation rules"
```

OneVision.UI decides the status label, tone, progress treatment, timestamps and responsive presentation. The application decides when the process changes state.

## Selectable information cards

`OVC-CARD` remains the governed card component. It now supports dense selectable-record semantics without creating a parallel selectable-card component.

Semantic inputs/regions include:

- `Selected`
- `Disabled`
- `Busy`
- `SelectedAction`
- `Status`
- `Metadata`
- `PrimaryAction`
- `SecondaryActions`
- existing header/content/action regions for compatibility
- accessible label

Selectable cards support pointer, Enter and Space activation. Selected state is not conveyed by colour alone: a selected marker is rendered in addition to border emphasis. Disabled and busy cards cannot be activated.

The selectable card body is a separate interaction target from its action region, so primary/secondary/overflow controls are not nested inside the selection control. Secondary card actions therefore do not accidentally change selection.

## Card collection

`OVC-CARD-COLLECTION` (`OVCardCollection`) is a lightweight responsive layout for groups of information cards. It owns wrapping/reflow only; it does not introduce a nested visual surface around the cards.

It also governs the normal content-state precedence for a card group.

## Content-state precedence

Workspace surfaces, contextual summaries and card collections use the same precedence:

`loading → error → empty → content`

Loading should normally preserve approximate content shape with `OVC-SKELETON`. An error is never represented as an empty state.

## Action hierarchy and busy state

Focused workspaces should expose one visually clear primary action. Secondary actions remain subordinate and destructive/infrequent actions should use the established overflow conventions where appropriate.

The application owns whether an action is valid. Components expose busy/disabled state so duplicate submission can be prevented while work is underway.

## Tab summary convention

First-class administrative tabs/pages should use a compact contextual summary when persistent state materially helps the user understand the current view. The strip may include the relevant active process.

Do not add a summary merely to fill space, and do not convert a small number of contextual values into a dashboard of cards.

## Responsive behaviour

- context summary items wrap cleanly;
- embedded process status moves below summary metrics when required;
- card collections reflow from multiple columns to one column;
- workspace header and footer actions remain usable on narrow screens;
- selected, disabled and busy state remains explicit at all widths.

## Ownership boundary

OneVision.UI owns:

- workspace regions and responsive layout;
- summary presentation;
- process-state visual grammar;
- selectable-card interaction/accessibility;
- card wrapping;
- loading/error/empty precedence;
- action hierarchy presentation.

Applications own:

- routing and Back destination;
- domain labels/data;
- process truth and lifecycle;
- selection meaning;
- permissions and validation;
- persistence and API calls;
- consequences of primary, secondary or overflow actions.

There is deliberately no merge-specific component or `OVC-MERGE-WORKSPACE`.

## Contract impact assessment

This change extends OVP-012 and existing reusable component capabilities below the application composition root. It does **not** alter the externally observable `OVP-000 – OneVision Application Foundation` contract.

**OVP-000 version impact: none. No version increase is required.**

The public demo should show these behaviours using neutral Manager/administrative data and the existing Fluent-inspired OneVision visual language.
