# Capability Assignment Workspace

## Purpose

Capability Assignment Workspace is a governed application composition for configuring a worker, service instance or other administrator-managed item that can perform one or more assignable capabilities.

It is not a domain-specific component. There is deliberately no `OVWorkerSettings`, `OVCapabilityCard` or `OVCapabilitySelector` contract at this stage.

The composition uses existing OneVision.UI contracts:

`OVListPage → OVDetailPage → OVContextSummary / OVProcessStatus → OVTextField / OVToggle → OVCardCollection / OVCard → governed footer actions`

The surrounding navigation follows OVP-012:

`item list → open item → focused configuration workspace → save/cancel → return`

## When to use capability cards

Use selectable capability cards when administrators must understand what an assignable job/function does before enabling it. Capability assignment is not ordinary boolean form input because each option carries meaningful identity, explanation, availability and operational context.

Use a normal checkbox/toggle when the input is simply a boolean setting and does not represent an independently described application capability.

## Identity and settings

The technical identity is application-owned and read-only. A friendly alias may be editable with `OVTextField`; the enabled state uses `OVToggle`.

The application owns:

- technical identity;
- alias validation and persistence;
- the consequences of enabled/disabled state;
- permissions and read-only decisions.

OneVision.UI owns the control presentation and states.

## Operational state

Use `OVContextSummary` as a compact strip for worker/item status such as status, heartbeat, host, instance, assigned-job count and current activity. Do not turn this into a dashboard.

When a durable live process exists, embed `OVProcessStatus`. The application supplies process truth, phase, message, timestamps and progress; OneVision.UI supplies the governed process presentation.

Operational actions such as restart, drain or run-now are separate from configuration save. They must not compete visually with `Save changes` in the configuration footer.

## Capability model

Capability definitions are application-supplied. A typical model contains:

```text
Code
DisplayName
Description
Category
Enabled / Available
OptionalNote
```

The human-readable display name and description are primary. Raw codes are secondary administrator metadata only.

Applications own capability codes, categories, dependencies, availability and business rules. OneVision.UI must not infer or enforce dependency rules merely because supporting text mentions another capability or service.

## Selection semantics

`OVCard` is the selectable capability surface. Whole-card selection is preferred.

The card contract provides:

- pointer activation;
- Enter/Space activation;
- `aria-pressed`;
- explicit non-colour selected marker;
- visible focus;
- disabled state;
- busy state;
- child-action isolation.

Do not add a duplicate checkbox purely to communicate assignment. A checkbox is appropriate only when the consuming domain or accessibility requirement genuinely calls for a separate control.

## Capability states

Recommended application-owned semantic states are:

- **Available** — can be assigned;
- **Assigned** — currently assigned;
- **Disabled** — known capability that cannot currently be changed;
- **Unavailable** — not supported by the current item/application/environment;
- **Legacy / reserved** — retained for compatibility but not a normal first-class capability.

Unavailable and reserved capabilities remain visible when useful to administrators. Reserved capabilities should be visually distinguished without making them look broken or missing.

## Grouping

When many capabilities exist, applications may group them into categories such as Data ingestion, Synchronisation, Processing, Operational and Maintenance. OneVision.UI governs normal section/card presentation; the application owns category membership.

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
- Cancel or Reset as secondary action;
- Save changes as the primary action.

During save, prevent duplicate submission, preserve the selected cards visually, disable unsafe navigation and use the existing busy/process conventions. Do not clear or rebuild the user's current selections while the save is in progress.

## Responsive behaviour

The composition must remain compact and usable at desktop, medium administrative width and narrow/mobile width:

- identity fields wrap;
- capability cards reflow to one column;
- long descriptions wrap without distorting adjacent cards;
- summary/process context remains readable;
- footer actions stack using the existing OVP-012 behaviour;
- no horizontal overflow is introduced.

## Canonical demo

The public gallery demonstrates a neutral **Capability Assignment Workspace** using generic capabilities such as Import data, Synchronise records, Validate data, Process queue and Publish output. It also demonstrates unavailable/reserved capability states and the same layout in read-only mode.

An application such as IDM may compose its own Worker Settings screen from these contracts and supply its actual role codes, descriptions, permissions and persistence rules.

## Contract impact assessment

This work adds a governed reusable composition using existing OVP-012 and OVC contracts. It does not change the externally observable `OVP-000 – OneVision Application Foundation` contract.

**OVP-000 version impact: none. No version increase is required.**
