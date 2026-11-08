# OVP-SOURCE-CONFIGURATION

`OVP-SOURCE-CONFIGURATION` governs inbound source configuration. Its four core peer sections are **Overview**, **Connection**, **Profile** and **Mapping**.

Applications may opt into a fifth peer section, **Display Value**, after Mapping. Display Value is an optional capability; consumers that do not enable it retain the existing four-tab composition unchanged.

The governed semantic flow is:

`Profile → Mapping → Display Value`

Profile discovers source structure, Mapping assigns target meaning, and Display Value optionally composes completed mapped meanings into a human-readable string.

## Governed visual containment and responsive composition (0.1.49)

All `OVP-SOURCE-CONFIGURATION` workspaces use one section-token convention:
`--ov-source-section-border`, `--ov-source-section-radius`,
`--ov-source-section-header`, `--ov-source-section-surface`,
`--ov-source-section-heading-size`, `--ov-source-section-heading-weight`,
`--ov-source-section-heading-padding` and `--ov-source-section-content-padding`.
These are sourced from theme palette variables by the shared pattern.
Profile's Source Configuration and Profile Results and Mapping's Field Mapping
share a neutral, subtly tinted header and consistent border/padding.
The **Identifier Configuration** is the intentionally emphatic variant with
the established primary-blue header; it retains the same section boundary and
heading metrics. No Current identity policy or inline Identity preview returns.

Container query names `ov-source-config`, `ov-source-profile`,
`ov-source-mapping` and `ov-identifier` make composition responsive to
the width of the containing editor or component, not only the browser viewport.
At narrow widths the identifier and fallback grid becomes a single column,
headings wrap, and the Run & Profile action remains beneath its controls.
Safe viewport media fallbacks are retained for browsers lacking support.
No section may cause page-level overflow.

The library owns its **outer** governed workspace border and heading.
Application-supplied content retains its own rendering; consuming apps should
avoid wrapping their entire fragment in a second decorative `OVPanel` when
the governed section already provides that boundary. The library must not
blindly strip borders from arbitrary nested panels, as those may represent
meaningful subgroups. Test both flat and nested application content.

Applications consume the compiled/versioned `OneVision.UI` package, without
requiring the UI source repository or a separately hosted runtime service.
They must not copy the library's CSS into app-specific overrides. The feature
does not introduce EAM dependencies and does not affect OVP-000.

The interactive gallery includes a constrained editor toggle (520px) so
reviewers can verify narrow-container reflow within a wide browser. This is
a layout refinement only: Web Table's separate Use this source interaction and
EAM's Validate Distinct Entities button remain outside this release.

## Connection mode

The application supplies `OVSourceConnectionMode`:

- `ManagedConnection` — a separately managed reusable connection is required.
- `NoConnectionRequired` — the source is defined directly in Profile.

For `NoConnectionRequired`, Connection remains visible and renders the governed satisfied state **No managed connection required** / **This source is configured directly in Profile.** It must not look incomplete or unhealthy merely because no managed connection object exists.

## Overview

Overview owns source identity, description, enabled/status context and summary metadata.

## Connection

Managed Connection content references reusable connection configuration. Direct sources use the governed no-connection-required state. `OVP-SOURCE-CONFIGURATION` does not create transport-specific connection conventions or extend `OVC-CONNECTION-EDITOR` merely to represent direct sources.

## Profile

Profile owns source-specific definition and the profiling lifecycle. Direct sources may include source location, optional discovery/inspection/selection and interpretation before `Run & Profile`.

### Contained configuration and results layout (0.1.48)

The governed `OVSourceProfile` composition displays exactly two distinct
sections, using the same compact section hierarchy as Mapping:

- **Source Configuration** holds the app-supplied source-specific controls,
  intermediate actions and selection, then an aligned `Run & Profile`
  action at its bottom (never in the Profile tab header). The existing
  disabled, running, read-only and callback contracts are unchanged.
- **Profile Results** holds the existing execution feedback, profile
  state, stale/failed notices, summary, discovered fields and app-supplied
  result actions.

This applies to managed SQL/API/FILE/LDAP, direct Web Table, and future
source modes without defining application-specific input fields or execution.
The profile action sits below all definition controls and above results; it
is not a separate third card. State and validation remain application-owned.
The existing discovery/selection behaviour is unchanged by this layout-only
refinement. No application composition-root or OVP-000 change is required.

`OVSourceProfileState` remains:

- `Initial`
- `Running`
- `Current`
- `Stale`
- `Failed`

A stale profile may remain visible for context but is not authoritative. **A stale profile result must not be treated as the current source structure by Mapping or as authoritative input to Display Value.**

When Profile is stale, Mapping is suppressed and the user is instructed to profile again. If an existing Display Value definition exists, Display Value may be retained read-only for context with a stale warning until profiling and Mapping are current again.

`OVSourceDiscovery` remains the governed domain-neutral discovery/selection/preview surface for source types that require inspection before profiling. Applications own execution, parsing and returned candidates.

## Mapping

Mapping consumes only current profiled output and maps source structure into the application-supplied target contract.

### Optional Identifier Configuration inside Mapping (0.1.46)

`IdentifierConfigurationEnabled=true` opts into the shared
`OVIdentifierConfiguration` policy panel **before** the application's
existing Mapping content. There is no additional top-level tab and no change
to `OVMappingPair` or the optional Display Value peer tab.

The application supplies `IdentifierDefinition` /
`IdentifierDefinitionChanged`, `IdentifierFields`,
`IdentifierStrategies`, `IdentifierCapabilities`,
`IdentifierEditState`, `IdentifierLockReason`,
`IdentifierValidationIssues`, `IdentifierSafetyInformation`,
`IdentifierPreviewConfigurationKey` and `IdentifierPreview`.
The definition is persistent semantic policy; permission, lock, saving,
profiling, validation and preview are independent runtime state. The default
edit permission is conservative **Unknown**, not Editable.

The entire identity policy is controlled as one capability: source-supplied
identifier toggle, source field, explicit missing-value fallback, optional
authorised deterministic derivation and app-produced sample preview.
Invalid, missing and stale field selections remain inspectable rather than
being silently replaced. The user may configure only app-permitted modes and
strategies. The application is responsible for identity continuity,
collision handling, canonicalisation, persistence and backend lock
enforcement.

When Profile is stale or unavailable, the saved identity configuration
**remains visible read-only**, even though the ordinary application-owned
Mapping content retains the established stale/blocked treatment. When the
capability is disabled (the default), legacy Mapping presentation is
unchanged. An app must not expose a second independently editable
identifier-target mapping alongside this policy.

See `docs/OVC-IDENTIFIER-CONFIGURATION.md` for the full semantic contract,
fallback terminality, inactive draft preservation, identity safety,
lock-state precedence and correlated preview requirements.


Use `OVC-MAPPING-PAIR` for directional source-to-target mapping and its optional Standard Details capability where applicable.

Mapping owns:

`profiled source fields → target meaning`

Required/default target definitions and Standard Detail definitions remain application-owned. Mapping validity remains governed by the relevant mapping contract; Display Value does not change Mapping drag/drop semantics or target requirements.

## Optional Display Value peer tab

Set `DisplayValueBuilderEnabled="true"` to add **Display Value** as a fifth peer tab after Mapping. When false, the tab does not exist and existing consumers remain unchanged.

Display Value must never be rendered inside Mapping when the capability is enabled. It owns:

`completed mapped target meaning → human-readable display string`

The Source Configuration integration contract is:

- `DisplayValueBuilderEnabled`
- `DisplayValueTargets`
- `DisplayValueDefinition`
- `DisplayValueDefinitionChanged`
- `DisplayValueReadOnly`
- `DisplayValueAllowLiteralOnly`

### Available fields

The application supplies only successfully completed mappings as `OVDisplayValueTargetOption` values. Eligible inputs are:

- completed mapped Core/default targets;
- completed mapped Standard Details.

Do not supply or expose:

- unmapped targets;
- invalid mappings;
- raw profiled source fields;
- source paths;
- source implementation identifiers.

The builder groups Core fields and Standard Details visually but both reference the same stable Mapping target identity through `TargetKey`.

### Pill interaction

Available mapped fields render as compact clickable pills. Activating a pill appends that mapped target to the composition. The same mapped field may be used more than once.

The visual sequence is fixed: **completed mapped-field pills → compact separator button group → prominent wrapping composition bar → live preview immediately below the bar**. Field tokens and separator tokens are visually distinguishable without introducing separate card rows. The preview should not be separated from the composer by a second editing toolbar or validation block.

Editable tokens expose a small remove action. There are no persistent visible move-left/move-right buttons in the composition bar.

### Separators

Separators are quick-add buttons rather than one global separator setting. The governed set is:

- `|`
- `-`
- `/`
- `,`
- `·`
- `(`
- `)`

Activating a separator button appends that separator as its own ordered literal segment. Different separator types may therefore coexist in the same display value, for example:

`Description (Locality) | Identifier`

The existing semantic persistence model is retained deliberately: `OVDisplayValueDefinition` stores ordered `OVDisplayValueSegment` values; mapped fields use stable `TargetKey` references and separators/literals store their semantic literal value. Applications do not persist or reverse-engineer a raw token template.

### Reordering

Composition tokens are draggable to reorder. Dragging is an enhancement, not the only interaction.

Keyboard users can focus an editable token and use **Alt + Left Arrow** or **Alt + Right Arrow** to move it deterministically. Reordering updates the ordered definition immediately. The UI must not require visible move-button clutter in the normal composition bar.

### Preview

The live preview updates immediately when:

- a field is added;
- a separator is added;
- a token is removed;
- tokens are reordered.

Applications may supply sample values on mapped targets. If a sample is unavailable, OneVision.UI uses a human-readable placeholder such as `[Description]`; it never exposes raw template syntax or source paths.

### Unresolved references

If a mapping already referenced by Display Value is subsequently removed, its segment remains in the definition and is shown as **Mapped field removed**. It becomes unresolved and invalid.

OneVision.UI must never silently substitute another mapped target, silently delete the segment, or expose a raw source field as fallback. The user must remove or repair the unresolved reference before the Display Value definition is valid.

### Empty and unavailable states

If the capability is enabled but there are no completed mappings, Display Value shows a governed empty state explaining that fields become available after Mapping is completed. It must not fall back to raw profile fields.

Display Value editing requires a current profile/mapping state. Before that state exists, the tab may remain visible but its content is governed as unavailable rather than presenting an editable empty builder.

### Validation

`OVDisplayValueValidation.IsValid(...)` validates the semantic definition against the current completed target set.

A definition is invalid when it contains an unresolved target reference or otherwise fails the configured meaningful-content policy. Separators/literals do not satisfy the default requirement for at least one mapped field. Literal-only definitions remain disabled by default and require the explicit `DisplayValueAllowLiteralOnly` policy.

Applications own final save/readiness composition and should include Display Value validity when a definition is configured.

### Read-only

Read-only mode preserves:

- the ordered composition;
- field/separator distinction;
- preview;
- unresolved-state presentation.

It suppresses mapped-field add controls, separator add controls, remove controls and reorder interaction.

## Responsive and accessibility contract

The Display Value experience uses compact OneVision administrative spacing and avoids unnecessary card nesting.

At narrower widths:

- available mapped-field pills wrap;
- composition tokens wrap;
- separator buttons wrap;
- preview remains readable;
- no page-level horizontal scrolling is introduced.

Accessibility requires:

- keyboard-focusable mapped-field and separator actions;
- keyboard activation for add/remove actions;
- deterministic non-drag reordering;
- meaningful unresolved/disabled states;
- logical focus order;
- live preview and validation announcements where appropriate;
- no reliance on colour alone.

## Ownership boundary

OneVision.UI owns:

- four core Source Configuration tabs and optional fifth Display Value tab placement;
- Connection satisfied-state presentation;
- Profile lifecycle presentation and stale protection;
- Mapping composition presentation;
- Display Value mapped-field pill presentation;
- separator quick actions;
- composition token presentation, drag and keyboard reorder;
- preview, empty, unresolved and read-only states;
- responsive and accessibility behaviour.

Applications own:

- source-specific fields and execution;
- connection/domain behavior;
- profiling and schema materialisation;
- mapping target definitions and business rules;
- which mappings are successfully completed and therefore exposed to Display Value;
- sample values and default Display Value definition;
- persistence and runtime interpretation;
- save eligibility, authorization and domain validation.

## Reference states

The gallery demonstrates managed and direct sources plus a focused Display Value reference containing:

- optional fifth peer tab;
- completed Core and Standard Detail pills;
- mixed separator tokens including parentheses;
- click-to-add fields and separators;
- drag reorder;
- keyboard reorder;
- live preview;
- no-mappings empty state;
- removed mapping/unresolved state;
- read-only state;
- responsive wrapping.

## Out of scope

This change does not alter source profiling behavior, Mapping drag/drop semantics, application-specific mapping requirements, connector behavior, Standard Detail catalogue ownership, or introduce application-specific/EAM-specific behavior.

## Foundation impact

This changes the externally observable `OVP-SOURCE-CONFIGURATION` contract when the capability is enabled, but it does not alter the application composition-root contract. `OVP-000 – OneVision Application Foundation` is unchanged.

## Operational-quality refinements (0.1.24)

- `EmbeddedInOverlay=true` omits the pattern-owned title and footer; the owning `OVOverlay` supplies its accessible heading, save/cancel controls, scroll and focus lifecycle. Status and header actions remain visible in the pattern body.
- `Saving` adds a busy semantic without clearing the configured profile, mapping or Display Value. `SaveErrorMessage` displays a non-destructive save failure while preserving the active tab and unsaved data.
- `MappingRequiresCurrentProfile=true` opts into a governed unavailable Mapping state whenever `ProfileState` is not Current. The existing Stale warning remains authoritative. The default is `false` to preserve existing consumers during migration; application owners should enable it when Mapping requires current profiling.
- If the optional Display Value capability is disabled while its tab is active, the rendered tab selection falls back to Mapping without mutating the application's stored section choice. Re-enabling Display Value restores the caller's controlled choice.
- The gallery exercises managed/direct connection modes, core versus Standard Detail mapping combinations, optional Display Value, blocked/stale states, saving and save-error states on the actual component.

The consumer continues to own readiness for saving, rule validation, connection/profile execution and persistence. Any mapped-field changes must be reflected in `DisplayValueTargets`; no stale source field may be treated as mapped data.

## Level 2 workflow recheck — 0.1.25

The new synthetic source-workflow reference composes a real Connection Editor, Source Configuration with Display Value and Standard Details enabled, Mapping Pair, then API Call Builder. It tests the human journey across controls, not only Source Configuration's own capability workbench. Direct-source, stale profile, read-only and mapping-blocked scenarios remain separately testable; this release does not change application profile or mapping rules.

## Source save-phase edit lock (0.1.43)

When `Saving=true`, the existing library-owned Display Value editor is automatically
rendered read-only while preserving the user's current definition and selected tab.
This applies even when the application has not separately enabled
`DisplayValueReadOnly`. Editing becomes available again when the save phase ends
unless the application still explicitly requests read-only mode.

A failed save (`Saving=false` and `SaveErrorMessage` populated) keeps the
definition visible and editable so the user can retry. Stale profiles continue to
display their existing read-only contextual treatment. Source execution, mapping,
validation, save eligibility, authorization and persistence remain application-owned.
The pattern does not attempt to lock arbitrary application-supplied child fragments
through hidden CSS or JS. No OVP-000 contract change.

Evidence: bUnit tests for save lock, unlock, retained error and explicit read-only
precedence; Playwright test for current/saving/error workbench states with optional
Display Value enabled. Publication and L3 consuming-app evidence are tracked separately.

## Identifier Configuration gallery and publication (0.1.46)

The Source Configuration workbench has an optional **Identifier Configuration**
capability and consumer-permission toggles for alternative and deterministic
fallback. Reference scenarios cover explicit/reject, alternative, no-ID
derivation, composite fields, authorised and prohibited derived fallback,
incomplete state, locked/unknown/read-only permissions, stale and unavailable
profiles, removed fields, saving, failed saving and stale asynchronous preview.
All scenarios use the exported `OVIdentifierConfiguration` through
`OVSourceConfiguration`. The sample SHA-256 routine is in the **demo
application**, not the library. The authorising application still owns
validation, uniqueness, save and ingestion.

This change is opt-in and does not alter the externally observable OVP-000
foundation. New consumers explicitly adopt the expanded
`OVP-SOURCE-CONFIGURATION` contract in OneVision.UI 0.1.46.

## Identifier-first visual composition (0.1.47)

When Identifier Configuration is enabled, it is the first, primary-blue governed
panel in Mapping. Editable primary identifier and missing-value fallback controls
sit alongside one another where the viewport permits, with the existing app-owned
preview, policy explanation and safety states retained. A separately contained
Field Mapping workspace follows below. Without the opt-in capability, the
legacy Mapping content and layout are not changed.

The current-policy summary and inline identity preview are intentionally omitted
from the governed identifier headline. The editable source/fallback controls,
locked-state explanation, deterministic warnings, validation and application-owned
Mapping content remain visible and unchanged in behaviour.
