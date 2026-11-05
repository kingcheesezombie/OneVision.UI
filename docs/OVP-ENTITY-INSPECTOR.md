# OVP-ENTITY-INSPECTOR

`OVP-ENTITY-INSPECTOR` governs read-only inspection of an application-owned business/domain entity without navigating away from the originating working context.

## Contract

The inspector composes the existing `OVC-OVERLAY` interaction and therefore supports the governed `RightSide` and `Workspace` placements. Opening an inspector must preserve the originating screen state; closing it returns the user to that unchanged context.

The pattern provides:

- title and optional subtitle/reference text;
- application-supplied status/type/source pill content;
- optional summary content;
- application-defined grouped detail sections, including governed tabs where richer records require them;
- deterministic loading, error, empty and loaded states;
- refresh action when supplied by the application;
- governed secondary, overflow and primary application-action positions;
- governed close behaviour inherited from `OVC-OVERLAY`.

The default use is read-only. The pattern does not imply edit, merge, allocation or persistence semantics.

## State precedence

Inspector content has one deterministic initial-content state:

`Loading > Error > Empty > Content`

- **Loading** means entity detail content is not yet available. The entity identity/header remains visible and the body uses governed skeleton rows.
- **Error** means initial detail retrieval failed. The entity identity/header remains visible; the application supplies a concise safe message and may provide additional governed detail. Retry uses the existing refresh action.
- **Empty** means the entity exists but currently has no detail content. It uses a compact `OVC-EMPTY-STATE`, not a page-level empty composition.
- **Content** renders summary and application-defined sections.

`Refreshing` is orthogonal to this precedence. Refresh does not replace already-loaded content with Loading. Existing content remains visible and a subtle refresh-progress treatment is shown. A recoverable `RefreshErrorMessage` may be displayed above retained content.

Closing during refresh is allowed by default because inspection is read-only. Applications with an unsafe refresh lifetime may set `CloseDisabledWhileRefreshing`; this is an explicit exception rather than an implied consequence of `Refreshing`.

## Focus and keyboard behaviour

The inspector inherits the complete `OVC-OVERLAY` focus contract:

- when opened, the element that triggered the overlay is captured;
- focus moves into the overlay, preferring the first meaningful interactive body control, then the close control, then the dialog surface;
- `Tab` and `Shift+Tab` are explicitly contained within the open overlay;
- `Escape` performs the same governed close attempt as the visible close action;
- when overlays are nested, only the topmost overlay owns Escape and focus containment;
- Busy, deliberate close-disable and Dirty confirmation rules apply identically to Escape, backdrop and explicit close;
- after a successful close, focus returns to the originating trigger when it still exists;
- nested close restores focus to the trigger in the underlying overlay, while the underlying overlay remains active;
- non-contained overlays use reference-counted document/body scroll locking, so closing a nested overlay does not unlock the page while an underlying overlay remains open;
- contained overlays do not apply global scroll locking.

The overlay exposes `role="dialog"`, `aria-modal="true"`, `aria-labelledby`, `aria-describedby` when a description exists, and `aria-busy="true"` when the overlay itself is Busy.

## Status presentation

Status content remains application-supplied but must use existing governed `OVC-PILL` presentation. The expected semantic order is:

1. Status
2. Type
3. Source

Pills wrap when space is constrained and remain visually subordinate to the inspector title. The inspector does not define additional badge styles.

## Action hierarchy

Inspection is the primary purpose. Application actions must not dominate the surface.

The governed footer order is:

1. inspector utility action such as Refresh;
2. application secondary actions;
3. application overflow for destructive or rare actions;
4. optional application primary action.

The header owns the normal Close action, so Close does not compete visually with meaningful application actions in the footer. `CloseText` is retained only as an obsolete compatibility parameter and has no visual effect; it should be removed at the next deliberate breaking-version boundary. Destructive application actions should continue to use the governed confirmation contract.

The legacy `Actions` slot remains for compatibility and is treated as secondary action content. New consumers should use `SecondaryActions`, `OverflowActions` and `PrimaryAction`.

## Metadata

Use `OVC-METADATA-LIST` for arbitrary property collections when a compact label/value treatment is appropriate. `OVC-KEY-VALUE` remains appropriate for isolated key/value presentation.

The list retains semantic `dl` / `dt` / `dd` markup and uses a compact bounded-label/two-column desktop layout that stacks labels above values on narrow widths. Groups are separated through spacing/subtle dividers rather than nested cards. Missing values use one governed default representation: `—`.

An empty item collection renders nothing by default. Consumers that need an explicit collection-level absence message may opt into `ShowEmptyState`; this renders one compact muted line using `EmptyMessage`, not a page-level `OVC-EMPTY-STATE`.

### Metadata identity

Structured expansion state uses `OVMetadataItem.Key` when a stable key is supplied. If no key is supplied, the component uses the item ordinal for that render. This means duplicate labels and groups are independent without requiring application keys. Applications should supply `Key` only when expansion identity must survive a data refresh or reordering. Expansion state for keys no longer present in the current dataset is discarded.

### Metadata value semantics

`OVMetadataValueKind` affects presentation rather than being descriptive metadata only:

- **Text** — normal administrative text; wraps naturally, preserves useful line breaks and never creates page-wide horizontal overflow.
- **Number** — a non-null display-ready `Value` always wins. When `Value` is null and `RawValue` implements `IFormattable`, optional `Format` is applied using the current culture. Invalid format hints fall back safely to default current-culture formatting. Currency meaning, units, percentage semantics, suffixes and domain-specific precision remain application-owned.
- **DateTime** — when supplied a `DateTime`/`DateTimeOffset` `RawValue`, renders through the existing governed `OVC-DATE-TIME` conventions. `DateTimeDisplay` selects Date, Time or DateTime semantics. A display-ready `Value` remains available for explicit application-owned formatting.
- **Boolean** — a boolean `RawValue` uses neutral `Yes` / `No` text. True/false is never automatically mapped to success/error colour.
- **Structured** — preserves indentation in a technical/monospace surface, wraps long lines, constrains large payload height with internal scrolling and supports copy. Values over the configured line threshold are collapsed by default with `Show more` / `Show less`; short values remain fully visible. The disclosure control exposes `aria-expanded` and `aria-controls` against the stable rendered content target.

`RawValue` exists only to provide safe reusable primitive formatting. The precedence is `Value > RawValue + Format > safe RawValue formatting`. `SecondaryText` provides a small reusable explanatory line such as source/freshness context without turning metadata into a generic layout DSL.

## Responsive and motion behaviour

Right-side inspectors use the governed compact desktop width and become near/full-width at narrow breakpoints rather than leaving an unusably small panel. Long titles, subtitles, metadata and action groups must wrap without introducing horizontal page scrolling. The close action remains available when header text wraps, and the footer wraps actions rather than shrinking controls below usable sizes.

Non-essential loading animation respects the platform `prefers-reduced-motion` setting. Skeleton animation is only enabled when reduced motion is not requested, and governed indeterminate progress suppresses its moving bars under reduced motion.

## Ownership boundary

OneVision.UI owns:

- overlay placement, focus containment, nested-overlay keyboard ownership, close and scroll-lock interaction;
- header/subtitle presentation;
- status, summary and section composition;
- loading/error/empty/refresh presentation;
- refresh and application-action placement;
- responsive density and accessibility inherited from governed overlay/actions;
- compact metadata-list presentation and primitive value rendering semantics.

Applications own:

- entity retrieval and refresh/cancellation lifetime;
- entity identity, fields and metadata values;
- stable metadata keys where identity must survive a dataset refresh;
- domain formatting beyond safe primitive defaults;
- permissions;
- which sections/tabs exist;
- domain-specific actions;
- all domain meaning, validation and persistence.

No consuming application name or business-domain type is part of the pattern contract.

## Record comparison

A separate `OVC-RECORD-COMPARISON` is not part of this contract. Merge/review implementations should first compose `OVC-DATA-GRID`, `OVC-METADATA-LIST`, `OVC-PILL` and `OVP-ENTITY-INSPECTOR`. A comparison component should only be introduced if real application implementation demonstrates a reusable interaction that those assets cannot express cleanly.

## OVP-000 impact

This refinement remains component/pattern-level behaviour. It does not change `OVP-000 – OneVision Application Foundation` and therefore does not advance the foundation contract version.

## Operational-quality refinements (0.1.24)

`OVEntityInspector.Busy` passes an application-action busy state through to the owning `OVOverlay` (including guarded close/focus behaviour) and shows `BusyMessage` while retaining the entity identity and previously loaded metadata. `ActionErrorMessage` presents a recoverable non-destructive failure without clearing the entity summary or sections. The inspector disables its governed Refresh and overflow controls while Busy; application-supplied action fragments must honour the same business state themselves.

This is distinct from the existing `Loading > Error > Empty > Content` precedence. A transient Refreshing state and `RefreshErrorMessage` continue to preserve content. The gallery demonstrates busy, action-error and refresh-error states with synthetic metadata and an explicit way to complete the simulated busy operation, avoiding an uncloseable demo.
