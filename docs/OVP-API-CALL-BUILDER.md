# OVP-API-CALL-BUILDER — Guided API Call Builder

Status: Draft

## Purpose

`OVP-API-CALL-BUILDER` is the governed OneVision configuration experience for composing application-owned API requests through semantic choices rather than HTTP syntax.

The default question is **What are you trying to do?**. Users should not need to understand methods, route templates, query strings, JSON structure or token syntax to configure a valid request.

The pattern is not Postman, Swagger UI, a generic REST client, an arbitrary URL editor or a cross-application API catalogue.

## Ownership boundary

OneVision.UI owns:

- recursive decision-tree rendering;
- persistent completed-decision presentation and in-place change actions;
- metadata-driven parameter controls;
- dynamic/dependent parameter states;
- required/optional presentation and generic validation;
- compilation into `OVApiCallConfiguration`;
- copyable request output and secondary technical request details;
- structured body and repeatable collection editing;
- safe authentication context;
- testing/loading/success/empty/failure states;
- Structure / Data / Raw response presentation;
- unsaved-change confirmation;
- responsive and accessible interaction behaviour.

The consuming application owns:

- its own API catalogue and semantic decision structure;
- OpenAPI-derived operation/parameter metadata;
- managed connections;
- lookup/source resolution;
- application/domain validation;
- runtime request execution;
- authentication and credentials;
- persistence;
- authorization;
- returned response metadata/sample/raw content.

A consuming application exposes only its own API definitions. OneVision.UI does not aggregate EAM, IDM, HDM and KPI into a central browser.

## Architecture

The default visual flow is intentionally top-to-bottom:

```text
Decision flow
    ↓
Request
    ↓
Response
```

The data flow remains:

```text
Application API catalogue / OpenAPI metadata
        ↓
Recursive guided decisions
        ↓
Structured selections and parameter values
        ↓
OVApiCallConfiguration
        ↓
Request output / Test
        ↓
Response
```

`OVApiCallConfiguration` remains the compiled request output so existing execution and persistence integrations can continue to consume the established request contract.

The public guided-state contract is `OVApiGuidedState`. It records decision selections, parameter selections and the resolved operation so an application can reopen the same guided state.

## Reusable components

The pattern is decomposed into:

- `OVApiCallBuilder` — orchestration, traversal, validation, compilation and Decision → Request → Response composition;
- `OVApiDecisionTrail` — persistent vertical stack of completed choices with low-emphasis Change actions;
- `OVApiDecisionStep` — current semantic question and choices;
- `OVApiParameterControl` — metadata-driven parameter rendering, including nested object and collection editing;
- `OVApiResponsePreview` — governed test/result presentation;
- `OVCopyableLink` — governed copy interaction for safe reusable request values.

The interaction design must remain data-driven and must not assume a fixed number of questions.

## Decision catalogue and flow

Applications provide an `OVApiCatalogue` containing:

- `InitialNodeId`;
- recursive `OVApiDecisionNode` definitions;
- `OVApiDecisionChoice` branches;
- terminal `OVApiOperationDefinition` records.

A decision choice either points to another node or resolves a terminal operation. The component follows references until a terminal operation is reached.

Completed decisions remain visible as compact vertical rows showing the question, selected answer, a subtle completion marker and a **Change** action. The completed stack is the primary representation of prior decisions; a breadcrumb is not required to reconstruct the selected path.

Selecting an earlier decision removes incompatible downstream decisions and parameter values, then presents that decision again in place. The user is not returned to the beginning of the builder.

Only the current relevant branch is presented. Future questions remain hidden until relevant; the complete API tree is never rendered at once.

## Operation metadata

`OVApiOperationDefinition` provides the compiled operation details that normally come from OpenAPI plus application semantic metadata:

- operation identifier and friendly label;
- HTTP method;
- route template;
- parameter definitions;
- managed connection identifier;
- headers/content type/timeout where required.

HTTP route details are not the primary editing surface.

## Parameter controls

`OVApiParameterDefinition` supports friendly labels/descriptions, required state, defaults, technical names, source/dependency metadata, parameter location and nested fields.

Governed mappings include:

- `Boolean` → Yes/No select;
- `Date` → date picker;
- `DateTime` → date/time controls;
- `Integer` → integer numeric control;
- `Number` → numeric control;
- `Select` → bounded select;
- `MultiSelect` → governed multi-select;
- `Text` → text field;
- `TextArea` → text area;
- `Lookup` → autocomplete/lookup;
- `StructuredObject` → recursive structured controls;
- `Collection` → repeatable governed item editor.

Friendly labels dominate. Technical API names are subordinate context.

Required parameters appear in the primary decision/configuration flow. Optional parameters are collapsed under **Additional options** unless the semantic definition intentionally promotes them.

## Dynamic and dependent sources

`ParameterSourceResolver` allows the application to supply values from static catalogues, application lists, other endpoints, runtime context or earlier decisions.

`SourceKey` identifies externally supplied values. `DependsOn` identifies a controlling parameter.

When a controlling value changes, the builder:

1. clears the dependent selection;
2. resolves the new allowed values;
3. displays loading/error states;
4. prevents stale values remaining silently selected.

The application owns the lookup implementation; OneVision.UI owns its interaction contract.

## Request compilation

The builder continuously compiles the resolved operation into `OVApiCallConfiguration`.

Parameter locations are respected:

- `Path` values replace route placeholders and are URI escaped;
- `Query` values populate the established parameter collection;
- `Header` values populate headers;
- `Body` values are serialized into generated JSON.

Structured object and collection request bodies are generated from controls. Raw JSON is not the default editing experience.

## Request output and copy contract

The Request section appears only after the decision flow resolves an operation. It is output from the decision process rather than a second configuration form.

The default request surface shows:

- request readiness;
- HTTP method and friendly operation label;
- **API path** — the operation route template;
- **Resolved path** — path substitutions and enabled query values applied;
- **Full URL** — the selected managed connection base address plus the resolved path;
- safe authentication context;
- `Test Request` adjacent to the request it will execute.

API path, resolved path and full URL use `OVCopyableLink`. Copy feedback is lightweight and announced through the governed control. Credentials, protected authentication values and secrets must never be included in copy surfaces.

Where present, generated request body and raw generated request may also be copied through `OVCopyableLink`.

## Request details and advanced information

Technical information is secondary and collapsed by default.

**Request details** exposes concise metadata such as:

- method;
- content type;
- header count;
- whether a request body was generated.

**Advanced** may expose safe connection context, timeout and the raw generated request.

Raw HTTP, generated JSON, header detail and technical parameter names must not compete with the decision flow or the primary request path on first render.

## Testing and response

`Test Request` is enabled only for a valid resolved request and when its managed connection authentication is configured. It remains visually associated with the Request section.

The application receives the compiled `OVApiCallConfiguration` through `TestRequested` and supplies an `OVApiCallTestResult`.

Supported states are `NotTested`, `Testing`, `Success`, `Empty` and `Failure`.

The Response section sits below Request. Before a test, it provides the governed not-tested state. During testing, the decision and request surfaces remain visible so the user can see exactly what is being executed.

Failures lead with friendly guidance. Technical diagnostics remain under secondary disclosure.

Successful structured responses expose compact tabs:

- **Structure** — default, using `OVStructuredFieldTree`;
- **Data** — small readable sample;
- **Raw** — troubleshooting view.

Raw response is never the default.

## Authentication

Authentication belongs to the selected managed connection. The builder may display safe context such as **Windows Integrated** or **Service credential configured**, but credentials, passwords and tokens are never rendered as editable or copyable values.

## Accessibility

The governed pattern must retain:

- keyboard-operable decisions and controls;
- logical top-to-bottom focus/document order;
- labelled current questions and controls;
- accessible completed decisions and Change actions;
- accessible copy controls with non-intrusive live confirmation;
- accessible disclosure states;
- validation/error association;
- loading/result announcements;
- no colour-only status meaning;
- accessible response tabs.

## Responsive behaviour

The default composition remains Decision flow → Request → Response rather than a permanent 50/50 configuration/preview split.

At wide widths the decision flow retains comfortable working width. Request output may use available width, but must remain visually subordinate to the active decision until the operation resolves.

At narrower widths the experience is single-column. Parameter grids collapse to one column, request labels stack above copyable values, long paths/URLs wrap safely, primary actions remain reachable, and the component must not create feature-level horizontal page overflow.

## Initial state

Before the user begins, the component shows the builder heading and the initial semantic question. Empty Request and Response panels are not rendered. Secondary output appears progressively only when there is meaningful request context.

## Acceptance contract

A conforming implementation must ensure:

1. the default experience begins with an application-supplied semantic action choice;
2. the decision tree supports arbitrary recursive depth;
3. each selection narrows the next valid choices;
4. completed decisions remain visible and editable without relying on a breadcrumb;
5. earlier choices can be changed without restarting the flow;
6. incompatible downstream selections are removed;
7. the active decision has stronger visual priority than completed decisions;
8. future irrelevant decisions remain hidden;
9. parameter controls follow datatype/semantic metadata;
10. dynamic/dependent sources are supported;
11. required and optional parameters are deliberately separated;
12. structured bodies are control-driven rather than raw-JSON-first;
13. collections use repeatable governed editors;
14. Request is visually separated from the decision process;
15. API path, resolved path and full URL are directly copyable through `OVCopyableLink`;
16. technical request details are collapsed by default;
17. Test Request is visually associated with the generated request;
18. Response is a dedicated lower section;
19. Structure is the default structured response view;
20. advanced HTTP details do not clutter the default experience;
21. the guided state can be persisted and reconstructed;
22. each application exposes only its own catalogue;
23. credentials/secrets are never exposed or copied;
24. narrow layouts require no feature-level horizontal scrolling;
25. the experience remains understandable without REST route knowledge.

## Foundation impact

This visual refinement does not change the OVP-000 Application Foundation contract. OVP-000 is not advanced.
