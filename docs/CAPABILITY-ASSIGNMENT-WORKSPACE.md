# Capability Assignment Workspace

## Purpose

Capability Assignment Workspace is a governed application composition for configuring a worker, service instance or other administrator-managed item that can perform one or more assignable capabilities.

It is not a domain-specific component. There is deliberately no `OVWorkerSettings`, `OVCapabilityCard` or `OVCapabilitySelector` contract at this stage.

The composition uses existing OneVision.UI contracts:

`OVListPage → OVDetailPage → OVContextSummary / OVProcessStatus → OVTextField / OVToggle → OVCardCollection / OVCard → governed footer actions`

The surrounding navigation follows OVP-012:

`item list → open item → focused configuration workspace → save/cancel → return`

The pattern name belongs to documentation/demo framing. A focused application workspace must use the current worker/item alias or human-readable object name as its `OVDetailPage` title rather than displaying `Capability Assignment Workspace` as the object heading.

## When to use capability cards

Use selectable capability cards when administrators must understand what an assignable job/function does before enabling it. Capability assignment is not ordinary boolean form input because each option carries meaningful identity, explanation, availability and operational context.

Use a normal checkbox/toggle when the input is simply a boolean setting and does not represent an independently described application capability.

## Identity and settings

The administrator-facing alias is primary. The technical worker identity is application-owned, read-only and visually secondary.

Recommended compact order:

1. Alias;
2. Enabled;
3. Worker name;
4. host / instance metadata.

The application owns:

- technical identity;
- alias validation and persistence;
- the consequences of enabled/disabled state;
- permissions and read-only decisions.

OneVision.UI owns the control presentation and states. Do not expand this region into a large general-purpose form when these compact identity/settings fields are sufficient.

## Operational state

Use `OVContextSummary` as a compact read-only status strip. Keep only enough information to understand current availability/heartbeat, and use `OVProcessStatus` for a live process when one exists.

A normal worker configuration page should read approximately as:

`Running · Processing queue · 18 / 42`

with heartbeat/time/detail secondary. Do not turn the configuration workspace into an operational dashboard.

Operational actions such as restart, drain or run-now are separate from configuration save. They must not compete visually with `Cancel` / `Save changes` in the configuration footer. If operational commands are introduced, place them in a distinct operational action region.

## Capability model

Capability definitions are application-supplied. A typical model contains:

```text
Code
DisplayName
Description
Category
Availability
OptionalNote
```

The human-readable display name and description are primary. Raw codes are secondary administrator metadata only. Keep raw codes available for diagnostics and implementation mapping, but render them with caption-sized muted treatment; optional monospace presentation is appropriate.

Applications own capability codes, categories, dependencies, availability and business rules. OneVision.UI must not infer or enforce dependency rules merely because supporting text mentions another capability or service.

## Selection semantics

`OVCard` is the selectable capability surface. Whole-card selection is preferred.

The card contract provides:

- pointer activation;
- Enter/Space activation;
- `aria-pressed`;
- explicit non-colour selected marker;
- selected border/background treatment;
- visible focus;
- disabled state;
- busy state;
- child-action isolation.

For capability cards, the intended hierarchy is:

1. capability name;
2. selected/assigned indication;
3. description;
4. supporting note and raw code.

The status pill must not be the only selected-state signal. Selected + busy must remain visibly selected while the save is in progress.

Do not add a duplicate checkbox purely to communicate assignment. A checkbox is appropriate only when the consuming domain or accessibility requirement genuinely calls for a separate control.

## Capability states

Recommended application-owned semantic states are:

- **Available** — can be assigned;
- **Assigned** — currently assigned;
- **Unavailable** — capability exists but cannot be used in the current environment/item;
- **Legacy / reserved** — retained intentionally for compatibility and not part of normal first-class processing.

Unavailable and Reserved are not interchangeable.

**Unavailable** should be subdued and disabled, with concise supporting text such as `Not supported by this worker environment.`

**Reserved / Legacy** should remain readable and informational rather than looking broken or unavailable, with supporting text such as `Retained for compatibility; not used by normal processing.`

## Grouping

Category membership remains application-owned, but grouping should be restrained. The purpose is faster scanning, not maximum classification.

For six or seven capabilities, prefer a small number of broad groups such as:

- Data & synchronisation;
- Processing;
- Operations.

Avoid five one- or two-card sections when three broader groups communicate the same structure more efficiently.

## Card density

Capability cards should remain compact administrative surfaces. A normal card contains:

- name;
- assignment/availability status;
- short description;
- raw code;
- optional one-line note.

Avoid excessive vertical padding, large empty areas or artificial equal-height treatment where one capability genuinely needs more explanation.

## Worker list cards

Worker list cards should lead with the administrator-facing alias, followed by the technical identity as secondary text, status, and a compact assignment/heartbeat line. For example:

```text
Allocation and Merge Worker
D-INFISDEVGEN01:IDM:02
Running
2 jobs assigned · heartbeat 18:12
```

Whole-card activation remains the default open action.

## Read-only and unavailable workers/items

Read-only mode uses the same composition. Alias and enabled state remain readable, assigned capabilities remain visible, selection is disabled and Save is absent or disabled.

An unavailable worker/item may show its last known configuration in the same read-only composition with an appropriate governed alert. Do not invent a second visual convention.

## Loading, error and empty states

Capability collections use the standard OneVision.UI precedence:

`loading → error → empty → content`

Examples include:

- no capabilities are available;
- the item is unavailable;
- permissions prevent editing;
- the capability catalogue failed to load.

A catalogue error is an error state, never an empty state.

## Save and busy behaviour

The OVP-012 footer remains the configuration action surface:

- optional explanatory/status text on the left;
- Cancel as secondary action;
- Save changes as the primary action.

When editable changes exist, the canonical feedback text is:

`Unsaved worker configuration changes`

During save:

- prevent duplicate submission;
- preserve all capability selections visually;
- keep selected cards selected even when busy;
- disable unsafe navigation;
- show Save changes as loading/busy;
- do not clear or rebuild the user's pending visual state.

After successful save the application may return to the worker list or remain on the workspace. If it remains, use governed success feedback rather than silently changing state.

## Responsive behaviour

The canonical composition is explicitly verified around 1280px, 760px and 390px.

At narrow/medium administrative widths:

- identity fields stack;
- capability cards become one column;
- raw codes wrap safely;
- status pills do not displace headings;
- descriptions remain visible;
- footer actions stack using the OVP-012 behavior;
- no horizontal scrolling is introduced.

## Canonical demo

The public gallery labels the surrounding example **Capability Assignment Workspace**, while the focused workspace itself uses the current worker alias as the page heading.

The neutral demo demonstrates:

- available + unassigned;
- assigned;
- unavailable;
- reserved;
- read-only;
- save/busy;
- contextual heading and Back-to-Workers flow;
- alias persistence after save.

An application such as IDM may compose its own Worker Settings screen from these contracts and supply its actual role codes, descriptions, permissions and persistence rules.

## Contract impact assessment

This refinement preserves the existing capability-assignment composition and ownership boundary. It adjusts the observable selected-content hierarchy of `OVC-CARD` so a card's primary heading is read before the generic selected marker, and refines the canonical OVP-012 composition/demo behavior.

It does not change the externally observable `OVP-000 – OneVision Application Foundation` contract.

**OVP-000 version impact: none. No version increase is required.**
