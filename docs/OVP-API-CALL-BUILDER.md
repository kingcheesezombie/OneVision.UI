# OVP-API-CALL-BUILDER — Guided API Call Builder

Status: Draft

## Purpose

`OVP-API-CALL-BUILDER` is the governed OneVision experience for composing application-owned API requests through semantic choices rather than HTTP syntax.

The pattern is deliberately not Postman, Swagger UI, a generic REST client, an arbitrary URL editor or a cross-application API catalogue. The application supplies its own API contract; OneVision.UI owns the interaction and presentation.

The premium UX contract prioritises calm hierarchy, low-friction progression, persistent decision context, technical capability without a technical-first surface, and graceful behaviour from simple two-step flows through 10+ decision levels.

## Ownership boundary

OneVision.UI owns:

- recursive decision rendering and arbitrary depth;
- completed/active decision presentation;
- governed choice presentation and smart thresholds;
- focus/orientation behaviour through long flows;
- metadata-driven parameter controls and grouping;
- dynamic/dependent parameter states;
- local validation timing and presentation;
- compilation into `OVApiCallConfiguration`;
- request readiness and copy surfaces;
- Test Request interaction;
- loading/success/empty/failure response states;
- Structure / Data / Raw response presentation;
- read-only presentation;
- responsive and accessible behaviour.

The consuming application owns:

- its own semantic decision catalogue;
- operation and parameter metadata;
- optional parameter grouping metadata;
- optional choice-presentation hints;
- managed connections and authentication;
- dynamic lookup/source resolution;
- domain validation;
- runtime request execution;
- persistence and authorization;
- returned response metadata, samples and raw content.

OneVision.UI does not aggregate EAM, IDM, HDM and KPI into a central API browser.

## Visual flow

The default composition is always top-to-bottom:

```text
Build request
    ↓
Request
    ↓
Response
```

Sections use spacing and restrained dividers rather than separate heavy cards. The active work remains visually dominant; technical output appears progressively as it becomes meaningful.

The data contract remains:

```text
Application API catalogue / OpenAPI metadata
        ↓
Recursive semantic decisions
        ↓
Structured parameter values
        ↓
OVApiCallConfiguration
        ↓
Request / Test
        ↓
Response
```

`OVApiCallConfiguration` remains the compiled request output. Premium UX refinements do not replace the existing execution/persistence contract.

## Decision flow

Applications provide `OVApiDecisionNode` and `OVApiDecisionChoice` definitions. A choice either points to another decision or resolves an `OVApiOperationDefinition`.

Completed decisions remain visible as compact vertical history rows:

```text
✓ Action        GET                         Change
✓ Resource      Allocations                 Change
✓ Selection     By target                   Change
```

Completed rows are deliberately quieter than the current question. They use muted question labels, stronger selected answers, a subtle completion marker and a lightweight Change action. They are not rendered as a stack of cards.

The active question uses stronger typography, whitespace and a restrained accent. Only the active branch is shown; future irrelevant questions are hidden.

Selecting **Change** removes only incompatible downstream decisions and values. The selected earlier decision becomes current again. Focus returns to the active flow position so keyboard and long-scroll users do not need to rediscover it.

### Choice presentation

`OVApiDecisionNode.Presentation` may suggest:

- `Buttons`
- `Cards`
- `Select`
- `SearchableSelect`
- `Radio`
- `Auto`

The hint selects a governed OneVision presentation; it does not allow application-owned styling.

When `Auto` is used, the governed thresholds are:

- 2–4 choices → cards;
- 5–10 choices → select;
- more than 10 choices → searchable select.

Choice descriptions should distinguish options rather than restating their labels.

## Deep-flow behaviour

The builder is designed and regression-tested at 10+ recursive decision levels.

Completed history remains compact. Once a flow reaches five completed decisions, a small sticky orientation cue summarises recent request context. It is orientation only; it never replaces the persistent completed-decision stack.

New/current decisions receive focus without aggressive viewport jumping. Section context, request output and the current task remain findable throughout long configurations.

The gallery includes Simple, Medium, Deep, Structured body, Bulk body and Read-only scenarios, plus a dedicated 11-step stress route at `/patterns/api-call-builder/deep-stress` for depth, accessibility and responsive regression.

## Request options and parameter grouping

Resolving an operation continues naturally into **Request options** rather than switching into a technical form.

`OVApiParameterDefinition` retains datatype, requirement, source, dependency, location and nested-field metadata, and additionally supports:

- `Group`
- `GroupDescription`
- `GroupOrder`

Grouping is optional and application-supplied. OneVision.UI owns group rendering.

Required parameters remain in the primary flow. Optional parameters remain collapsed under **Additional options**. When optional values are active, the disclosure reports configured state, for example:

```text
Additional options    2 configured
```

Otherwise it reports availability, for example `3 available`. Collapsing the section never clears selected values.

## Dynamic/dependent values

`ParameterSourceResolver`, `SourceKey` and `DependsOn` retain the existing source contract.

A dependent control whose prerequisite is incomplete keeps its field identity visible and explains what is needed, for example:

```text
Target type
Choose target provider first.
```

Loading also retains the field identity. Source failures remain local and provide Retry where available. When a controlling value changes, stale dependent values and stale source options are cleared before the new source resolves.

## Validation

Newly resolved operations must not appear broken before the user interacts with them.

Required-field validation is shown locally after:

- the field has been edited; or
- the user attempts Test Request; or
- the user attempts Save.

Dependency problems may be shown immediately because they explain why a control is unavailable. The builder does not render a duplicate global validation panel for ordinary field errors.

Request readiness may still identify the next missing value before validation is displayed, for example `Choose an entity type`.

## Request section

Request is an output of the guided flow, not a second configuration surface.

The default priority is:

1. **Resolved path**
2. **Full URL**
3. API path
4. generated body where present
5. generated/raw request under secondary disclosure

The section shows method + friendly operation label, a specific readiness state, safe authentication context and Test Request.

### Copy contract

`OVCopyableLink` provides local copy interaction and accessible feedback. A successful copy briefly shows **Copied ✓**, announces `Copied`, then returns to its normal state without a toast.

The Request section provides individual copy surfaces for resolved path, full URL and API path, plus **Copy request** for a useful method + full URL representation (and body where applicable).

Long URLs wrap without changing the copied value or pushing the copy action out of reach. Credentials, passwords, tokens and protected authentication values must never enter copy surfaces.

### Details and Advanced

Technical information remains collapsed by default.

**Details** may show method, content type, enabled header count and generated-body state. **Advanced** may show safe connection/timeout context and the generated request. Active non-default advanced configuration should remain visible through concise configured-state text even while collapsed.

## Test and Save semantics

`Test Request` belongs to Request and means **execute this request now**.

`Save` belongs to the configuration footer and means **save this configured request for later use**.

Their placement intentionally separates these scopes. Save remains the primary configuration-level action; Cancel is secondary. Test Request does not compete beside Save.

Attempting Test or Save with missing required values activates contextual field validation rather than sending an invalid request.

## Response

Before testing, Response stays minimal:

```text
Response
Test the request to preview its structure and returned data.
```

Testing uses contextual status text rather than an unexplained spinner.

Successful responses use a compact summary such as:

```text
✓ Request successful
200 OK · 1,248 records · 182 ms
```

Structured responses expose restrained tabs:

- **Structure** — default unless the result explicitly prefers a clearly tabular Data view;
- **Data** — compact governed sample table, up to 25 rows, with sampling context;
- **Raw** — secondary troubleshooting view with Copy and line-wrap control.

The Data view does not attempt to render the complete result set.

### Failure contract

`OVApiCallTestResult.FailureKind` can identify:

- `Configuration`
- `Authentication`
- `Connection`
- `Validation`
- `Server`
- `Unknown`

Friendly guidance is shown first. Technical detail remains collapsed by default and may contain status, endpoint, duration, response headers, correlation/request ID, diagnostic text and response body. Status codes provide a sensible fallback when no explicit failure kind/message is supplied.

## Read-only mode

Read-only mode is not a disabled form.

It renders configured labels and values as intentional static content while retaining useful copy surfaces, request details and response preview. Editing/Test/Save affordances are removed; the footer uses Close.

## Accessibility

The governed pattern must retain:

- logical top-to-bottom keyboard order;
- focus return to the active decision after changing earlier choices;
- completed decision announcements such as `Completed: Resource, Allocations`;
- current decision context such as `Current question: Entity type`;
- labelled controls and disclosures;
- local associated validation;
- accessible loading/result announcements;
- accessible Copy confirmation;
- no colour-only state meaning;
- accessible response tabs and tables;
- reduced-motion behaviour when requested by the operating system.

## Responsive behaviour

The builder remains single-column in normal use. It must not reintroduce the previous dense right-hand configuration panel.

Regression widths include large desktop, standard desktop, narrow desktop, tablet and mobile-like sizes. Parameter grids collapse, request labels stack, long paths wrap, data tables contain their own horizontal overflow, actions stay reachable, and the feature itself must not create horizontal page overflow.

## Initial state

Before the user begins, show the title and the initial semantic question only. Empty Request and Response panels are not rendered.

## Acceptance contract

A conforming implementation must ensure:

1. the active decision is immediately identifiable;
2. completed decisions remain visible but visually quiet;
3. recursive flows remain usable at 10+ decision levels;
4. changing an earlier decision removes only invalid downstream context and returns focus appropriately;
5. parameter entry feels continuous with the guided workflow;
6. optional settings remain progressively disclosed and report configured state;
7. grouping is application-supplied but OneVision.UI-rendered;
8. choice hints remain semantic and OneVision.UI-governed;
9. unresolved dependencies explain what is required;
10. validation is local and contextual rather than eager/noisy;
11. resolved path is the primary copyable technical output;
12. copy feedback is immediate, local and accessible;
13. long technical values do not damage layout;
14. Test and Save scopes are visually distinct;
15. response states are compact and informative;
16. structured responses provide governed Structure/Data/Raw views;
17. failures distinguish common failure categories and hide diagnostics by default;
18. read-only mode is intentional rather than disabled-form presentation;
19. credentials and secrets are never rendered or copied;
20. no EAM-specific behaviour is introduced into the shared component;
21. narrow layouts require no feature-level horizontal scrolling;
22. the experience remains understandable without REST route knowledge.

## Foundation impact

This premium refinement changes the API Call Builder pattern/component contract only. It does not change the OVP-000 Application Foundation composition-root contract, so OVP-000 is not advanced.
