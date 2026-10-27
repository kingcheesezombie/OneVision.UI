# OVP-SOURCE-CONFIGURATION

`OVP-SOURCE-CONFIGURATION` governs configuration of an inbound source through four peer sections: **Overview**, **Connection**, **Profile** and **Mapping**.

## Contract

### Overview

Owns source identity, description, enabled/status context and summary metadata.

### Connection

Connection is intentionally narrow. It identifies the managed endpoint and authentication/access context used to reach the source.

Examples:

- SQL: server/database/authentication.
- JSON/API: base URL/authentication.
- FILE: location/path/access.
- LDAP: host/port/authentication.

Connection does **not** own SQL queries, stored procedure selection, API route extensions, request bodies, file-selection rules, LDAP searches or discovered schema. Reusable connection-object creation/editing remains governed by `OVC-CONNECTION-EDITOR`.

### Profile

Profile owns both the **source definition** and the **profiling lifecycle**. The user defines what should be executed/read through the selected connection, chooses **Run & Profile**, and inspects the returned/discovered structure.

Transport-specific examples:

- SQL: table/view/stored procedure/query selection, query text where permitted, procedure/query parameters, execution and returned-column profiling.
- JSON/API: endpoint extension/relative route, method, parameters/body, response-root selection, execution and response-structure profiling.
- FILE: file/pattern selection beneath the managed location, format/parser settings, sample/read and field/type inference.
- LDAP: search base/filter/object/attribute selection, execution and returned-attribute profiling.

`OVSourceProfile` is the governed reusable Profile-region composition. It owns source-definition/result layout, selected-connection context, `Run & Profile` action placement, profiling progress, message/status placement and result placement. The consuming application owns transport-specific definition controls, validation, execution, cancellation/timeouts, secure request material, persistence and conversion of the returned data into the discovered field model.

Whenever the result is a discovered structured schema, render it with `OVC-STRUCTURED-FIELD-TREE`.

Changing the selected connection or source definition invalidates the previous profile result according to application rules; applications must not silently present an old schema as though it came from the new definition.

### Mapping

Mapping consumes the current profiled output and maps it into the declared target contract. It does not define how source data is retrieved.

Use `OVC-MAPPING-PAIR` for directional source-to-target mapping and optionally `OVC-MAPPING-RULE` where source-specific rules are required.

## Ownership boundary

OneVision.UI owns:

- Overview / Connection / Profile / Mapping section structure.
- Profile source-definition/result composition.
- `Run & Profile` action placement and profiling-state presentation.
- discovered-structure presentation through governed structured-data controls.
- reusable density, responsive layout, validation/status placement and accessibility.

Applications own:

- which managed connections are available and eligible.
- the actual SQL/API/file/LDAP operations supported.
- execution and data-access implementation.
- parameters, domain validation and permissions.
- secret/credential handling.
- profile/schema materialisation and persistence.
- target/domain mapping semantics.

## Interaction flow

`managed connection → source definition → Run & Profile → discovered structure → mapping`

There is no separate top-level Source tab. Source-definition behaviour belongs to Profile because the definition and its discovered output form one profiling workflow.
