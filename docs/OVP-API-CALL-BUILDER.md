# OVP-API-CALL-BUILDER — Guided API Call Builder

Status: Draft

## Purpose

`OVP-API-CALL-BUILDER` is the governed OneVision experience for composing application-owned API requests through semantic choices rather than HTTP syntax.

The pattern is deliberately not Postman, Swagger UI, a generic REST client, an arbitrary URL editor or a cross-application API catalogue. The application supplies its own API contract; OneVision.UI owns the interaction and presentation.

The UX contract prioritises calm hierarchy, low-friction progression, persistent decision context, technical capability without a technical-first surface, and graceful behaviour from simple two-step flows through 10+ decision levels.

## Ownership boundary

OneVision.UI owns:

- recursive decision rendering and arbitrary depth;
- completed/active decision presentation;
- governed choice presentation and conservative automatic thresholds;
- focus/orientation behaviour through long flows;
- metadata-driven parameter controls, grouping and group layout;
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

- its semantic decision catalogue;
- operation and parameter metadata;
- optional parameter grouping/layout metadata;
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

`OVApiCallConfiguration` remains the compiled request output. UX refinements do not replace the existing execution/persistence contract.

## Decision flow

Applications provide `OVApiDecisionNode` and `OVApiDecisionChoice` definitions. A choice either points to another decision or resolves an `OVApiOperationDefinition`.

Completed decisions remain visible as compact vertical history rows. The question is visually quiet, the selected answer is stronger, and Change remains a lightweight text action. Completed decisions must not become a stack of cards.

Selecting **Change** removes only incompatible downstream decisions and values. The selected earlier decision becomes current again. Focus returns to the active flow position so keyboard and long-scroll users do not need to rediscover it.

### Choice presentation

`OVApiDecisionNode.Presentation` may specify:

- `Buttons`
- `Cards`
- `Select`
- `SearchableSelect`
- `Radio`
- `Auto`

The hint selects a governed OneVision presentation; it does not allow application-owned styling.

When `Auto` is used, the governed defaults are deliberately conservative:

- 2–4 short/simple choices with no descriptions → compact buttons;
- 2–4 choices containing explanatory descriptions → cards;
- 5–10 choices → select;
- more than 10 choices → searchable select.

Applications may use an explicit presentation hint where automatic inference would be ambiguous. No complicated semantic heuristics are applied.

## Deep-flow behaviour

The builder is designed and regression-tested at 10+ recursive decision levels.

Completed history remains compact. A sticky orientation cue appears only once a flow reaches five completed decisions. It summarises recent request context, remains visually secondary, and never replaces the persistent completed-decision stack.

New/current decisions receive focus without aggressive viewport jumping. Changing an earlier decision returns focus to the newly active step and must not expose a Request section until an operation is resolved again.

The gallery includes Simple, Medium, Deep, Structured body, Bulk body and Read-only scenarios, plus a dedicated deep-stress route where provided by the demo.

## Request options and parameter grouping

Resolving an operation continues naturally into **Request options** rather than switching into a technical form.

`OVApiParameterDefinition` supports datatype, requirement, source, dependency, location, nested-field and grouping metadata including:

- `Group`
- `GroupDescription`
- `GroupOrder`
- `GroupLayout`

`GroupLayout` is presentation metadata only and supports:

- `Auto`
- `SingleColumn`
- `TwoColumn`

`Auto` uses two columns only when the fields are reasonably independent. It resolves to a single column when the group has one field, contains a dependent field, or contains wide/structured control types such as lookup, multi-select, textarea, structured object or collection. Explicit `SingleColumn` or `TwoColumn` metadata overrides that automatic choice. Responsive layouts may still collapse two columns to one column.

Required parameters remain in the primary flow. Optional parameters remain collapsed under **Additional options**. The disclosure uses concise state text. Configured values take precedence over availability, for example:

```text
Additional options    2 configured
```

otherwise:

```text
Additional options    3 available
```

Collapsing the section never clears selected values.

## Dynamic/dependent values

`ParameterSourceResolver`, `SourceKey` and `DependsOn` retain the existing source contract.

A dependent control whose prerequisite is incomplete keeps its field identity visible and explains what is needed, for example:

```text
Target type
Choose target provider first.
```

Loading also retains the field identity. Source failures remain local and provide Retry where available. When a controlling value changes, stale dependent values and stale source options are cleared before the new source resolves.

## Validation and action enablement

Newly resolved operations must not appear broken before the user interacts with them. Required-field validation remains local and is shown when the field has been touched/cleared or equivalent local interaction establishes an error.

Incomplete resolved requests are **not** valid saved configurations. Test and Save are therefore disabled while the request is invalid.

Save is enabled only when:

```text
Dirty
AND
Operation resolved
AND
Request valid
AND
Not busy
AND
Not read-only
```

Test Request is enabled only when the request is valid, execution is not busy/testing, and the managed authentication context is configured.

Readiness remains visible before validation becomes noisy. For one missing required value it identifies that value, for example `Choose an entity type`. For multiple missing values it reports a concise count such as `3 required values remaining`. Field-level validation remains beside the relevant controls.

## Request section

Request is an output of the guided flow, not a second configuration surface.

The default priority is:

1. **Resolved path**
2. **Full URL**
3. API path
4. safe authentication context
5. collapsed Details / Advanced information
6. Test Request

The section shows method + friendly operation label and a specific readiness state. Redundant explanatory text around Test Request is omitted when the action is self-explanatory.

### Copy contract

`OVCopyableLink` provides local copy interaction and accessible feedback. A successful copy briefly shows **Copied ✓**, announces `Copied`, then returns to its normal state without a toast or layout shift.

The Request section provides individual copy surfaces for resolved path, full URL and API path, plus **Copy request** for a useful method + full URL representation (and body where applicable).

Resolved path remains the visually strongest reusable output. Full URL and API path are secondary, while Copy request remains a utility action. Body/raw request copy controls stay inside their expanded technical sections.

Long URLs and other technical values must wrap or contain their own overflow without widening the builder. Credentials, passwords, tokens and protected authentication values must never enter copy surfaces.

### Authentication

Authentication context is subordinate in the normal state, for example:

```text
Authentication
Windows Integrated
```

A problem state receives stronger emphasis and a useful message such as `Service credential not configured.` Secret inputs are never rendered by this pattern.

### Details and Advanced

Technical information remains collapsed by default.

**Details** may show method, content type, enabled header count and generated-body state. **Advanced** may show safe connection/timeout context and the generated request. Active non-default advanced configuration should remain visible through concise configured-state text even while collapsed.

## Test and Save semantics

`Test Request` belongs to Request and means **execute this request now**.

`Save` belongs to the configuration footer and means **save this valid configured request for later use**.

Their placement intentionally separates these scopes. Save remains the primary configuration-level action; Cancel is secondary. Test Request does not compete beside Save. Neither action is enabled for an invalid request.

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

No additional response tabs are introduced without a real use case.

### Failure contract

`OVApiCallTestResult.FailureKind` can identify:

- `Configuration`
- `Authentication`
- `Connection`
- `Validation`
- `Server`
- `Unknown`

Explicit friendly guidance/failure kind takes precedence. When only a status code is available, common failures use specific governed wording:

- 400 → `The API rejected one or more request values.`
- 401 → `Authentication required.`
- 403 → `You do not have permission to perform this request.`
- 404 → `The requested resource was not found.`
- 500+ → `The API could not complete the request.`

Technical detail remains collapsed by default and may contain status, endpoint, duration, response headers, correlation/request ID, diagnostic text and response body.

## Read-only mode

Read-only mode is not a disabled form.

It renders completed decisions and configured labels/values as intentional static content while retaining useful copy surfaces, request details and response preview. Editing, Test and Save affordances are removed; the footer uses Close.

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

## Responsive and long-value behaviour

The builder remains single-column in normal use. It must not reintroduce a dense right-hand configuration panel.

Regression coverage explicitly includes 1024px, 768px, 600px and 400px widths, in addition to larger desktop states. At these sizes:

- the feature must not create horizontal page overflow;
- long URLs remain contained;
- disclosures remain usable;
- copy controls remain visible;
- buttons do not overlap;
- footer actions stack where required;
- two-column parameter groups collapse appropriately;
- active choice controls fit cleanly.

Long-value containment applies to path/query values, lookup labels, full URLs, bodies, raw requests, response fields and configured read-only values. No individual value may widen the builder.

## Initial state

Before the user begins, show the title and the initial semantic question only. Empty Request and Response panels are not rendered.

## Acceptance contract

A conforming implementation must ensure:

1. Save is disabled until a dirty, resolved request is valid and the builder is not busy/read-only;
2. Test Request is disabled until the request is valid and executable;
3. active decisions are immediately identifiable and completed decisions remain compact/quiet;
4. sticky orientation context appears only for genuine deep flows and remains visually secondary;
5. recursive flows remain usable at 10+ decision levels;
6. changing an earlier decision removes only invalid downstream context and returns focus appropriately;
7. small/simple choices default to buttons while descriptive choices may use cards;
8. explicit choice-presentation hints remain supported;
9. parameter entry feels continuous with the guided workflow;
10. parameter group layout supports Auto, SingleColumn and TwoColumn presentation metadata;
11. Auto layout avoids two-column presentation for dependent, structured or wide controls;
12. optional settings remain progressively disclosed and report concise configured/available state;
13. unresolved dependencies explain what is required;
14. readiness messaging identifies one missing value or counts multiple missing values;
15. resolved path is the primary copyable technical output;
16. copy feedback is immediate, local and accessible without layout shift;
17. Test and Save scopes are visually distinct;
18. response summaries are compact and Structure/Data/Raw remains the governed view set;
19. common HTTP failures have meaningful user-facing messages while diagnostics stay collapsed;
20. read-only mode is intentional rather than disabled-form presentation;
21. credentials and secrets are never rendered or copied;
22. 1024/768/600/400 layouts require no feature-level horizontal scrolling;
23. long technical values do not widen the component;
24. no EAM-specific behaviour is introduced into the shared component;
25. the experience remains understandable without REST route knowledge.

## Foundation impact

This final refinement changes the API Call Builder pattern/component contract only. It does not change the OVP-000 Application Foundation composition-root contract, so OVP-000 is not advanced.

## Operational-quality refinements (0.1.24)

`OVApiCallBuilder` retains its existing guided decision tree, parameter dependencies and generated-request semantics while using a denser administrative hierarchy and less decorative whitespace. `EmbeddedInOverlay=true` suppresses its own heading and save/cancel footer so an `OVOverlay` may own them; the consumer must supply equivalent save validity, dirty confirmation and focus through that overlay. This is presentation composition, not a new API execution contract.

`BusyLabel`, `SaveErrorMessage` and `SavedMessage` provide non-destructive operational feedback above the flow. Requests, selected decisions, configured values and technical previews remain visible through save failures. Test, Save and Cancel event handlers guard against direct invocation while busy/read-only, and Save also guards against non-dirty submissions. Consumers still own execution, validation, authorization and persistence. The gallery exercises guided, deep, structured, bulk, read-only, saving, save failure and completed states on the actual builder.
