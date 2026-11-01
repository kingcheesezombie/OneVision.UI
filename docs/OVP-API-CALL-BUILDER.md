# OVP-API-CALL-BUILDER — Guided API Call Builder

Status: Draft

## Purpose

`OVP-API-CALL-BUILDER` is the governed OneVision configuration experience for composing application-owned API requests through semantic choices rather than HTTP syntax.

The default question is **What are you trying to do?**. Users should not need to understand methods, route templates, query strings, JSON structure or token syntax to configure a valid request.

The pattern is not Postman, Swagger UI, a generic REST client, an arbitrary URL editor or a cross-application API catalogue.

## Ownership boundary

OneVision.UI owns:

- recursive decision-tree rendering;
- progressive question flow and decision trail;
- metadata-driven parameter controls;
- dynamic/dependent parameter states;
- required/optional presentation and generic validation;
- compilation into `OVApiCallConfiguration`;
- friendly request summary and secondary technical request details;
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

The governed flow is:

```text
Application API catalogue / OpenAPI metadata
        ↓
Recursive guided decisions
        ↓
Structured selections and parameter values
        ↓
OVApiCallConfiguration
        ↓
Request summary / technical preview
        ↓
Test / Save
```

`OVApiCallConfiguration` remains the compiled request output so existing execution and persistence integrations can continue to consume the established request contract.

The public guided-state contract is `OVApiGuidedState`. It records decision selections, parameter selections and the resolved operation so an application can reopen the same guided state.

## Reusable components

The pattern is decomposed into:

- `OVApiCallBuilder` — orchestration, traversal, validation and compilation;
- `OVApiDecisionTrail` — compact previous-choice navigation;
- `OVApiDecisionStep` — current semantic question and choices;
- `OVApiParameterControl` — metadata-driven parameter rendering, including nested object and collection editing;
- `OVApiResponsePreview` — governed test/result presentation.

The interaction design must remain data-driven and must not assume a fixed number of questions.

## Decision catalogue

Applications provide an `OVApiCatalogue` containing:

- `InitialNodeId`;
- recursive `OVApiDecisionNode` definitions;
- `OVApiDecisionChoice` branches;
- terminal `OVApiOperationDefinition` records.

A decision choice either points to another node or resolves a terminal operation. The component follows references until a terminal operation is reached.

Completed decisions remain visible in `OVApiDecisionTrail`. Selecting an earlier decision returns to that stage and discards incompatible downstream decisions and parameter values.

Only the current relevant branch is presented; the complete API tree is never rendered at once.

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

Required parameters appear in the primary configuration flow. Optional parameters are collapsed under **Additional options** unless the semantic definition intentionally promotes them.

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

## Request summary and advanced details

Once an operation resolves, a human-readable summary appears with:

- operation name;
- friendly parameter values;
- safe authentication context;
- request readiness.

Technical information is secondary under:

- **View request details**;
- **View raw request**;
- **Advanced**.

The default surface never opens as Method / Resource / Parameters / Headers / Body.

## Testing and response

`Test Request` is enabled only for a valid resolved request and when its managed connection authentication is configured.

The application receives the compiled `OVApiCallConfiguration` through `TestRequested` and supplies an `OVApiCallTestResult`.

Supported states are `NotTested`, `Testing`, `Success`, `Empty` and `Failure`.

Failures lead with friendly guidance. Technical diagnostics remain under secondary disclosure.

Successful structured responses expose:

- **Structure** — default, using `OVStructuredFieldTree`;
- **Data** — small readable sample;
- **Raw** — troubleshooting view.

Raw response is never the default.

## Authentication

Authentication belongs to the selected managed connection. The builder may display safe context such as **Windows Integrated** or **Service credential configured**, but credentials, passwords and tokens are never rendered as editable values.

## Accessibility

The governed pattern must retain:

- keyboard-operable decisions and controls;
- logical focus/document order;
- labelled current questions and controls;
- an accessible decision trail;
- accessible disclosure states;
- validation/error association;
- loading/result announcements;
- no colour-only status meaning;
- accessible response tabs.

## Responsive behaviour

Wide layouts may show the active guided configuration beside a sticky request summary. At narrower widths the summary moves beneath the active flow. The component must not create feature-level horizontal page overflow.

## Acceptance contract

A conforming implementation must ensure:

1. the default experience begins with an application-supplied semantic action choice;
2. the decision tree supports arbitrary recursive depth;
3. each selection narrows the next valid choices;
4. earlier choices can be changed without restarting the flow;
5. incompatible downstream selections are removed;
6. HTTP route construction remains secondary;
7. parameter controls follow datatype/semantic metadata;
8. dynamic/dependent sources are supported;
9. required and optional parameters are deliberately separated;
10. structured bodies are control-driven rather than raw-JSON-first;
11. collections use repeatable governed editors;
12. the compiled request remains available as secondary detail;
13. Test Request requires a valid request;
14. Structure is the default structured response view;
15. friendly labels dominate technical names;
16. advanced HTTP details do not clutter the default experience;
17. the guided state can be persisted and reconstructed;
18. each application exposes only its own catalogue;
19. credentials/secrets are never exposed;
20. the experience remains understandable without REST route knowledge.

## Foundation impact

This pattern does not change the OVP-000 Application Foundation contract. OVP-000 is not advanced by this redesign.
