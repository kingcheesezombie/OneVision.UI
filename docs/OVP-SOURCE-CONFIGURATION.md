# OVP-SOURCE-CONFIGURATION

`OVP-SOURCE-CONFIGURATION` governs configuration of an inbound source through four peer sections: **Overview**, **Connection**, **Profile** and **Mapping**.

## Contract

### Overview

Owns source identity, description, enabled/status context and summary metadata.

### Connection

Connection is intentionally narrow in the Source Configuration pattern because it references a separately managed reusable connection object. That managed connection owns everything required to reach and authenticate to the endpoint.

Examples:

- SQL: server/database/authentication.
- JSON/API: base URL/authentication.
- FILE: location/path/access.
- LDAP: host/port/authentication and reusable directory/base context required by the connector.

Source Configuration must not repeat connector-owned location or connection details merely because a transport-specific Profile is being configured. Reusable connection-object creation/editing remains governed by `OVC-CONNECTION-EDITOR`.

### Profile

Profile owns the **source-specific definition** that is not already reusable connection state, plus the **profiling lifecycle**. The user defines any source-specific read/execution behaviour, chooses **Run & Profile**, and inspects the returned/discovered structure.

Transport-specific examples:

- SQL: table/view/stored procedure/query selection, query text where permitted, procedure/query parameters, execution and returned-column profiling.
- JSON/API: endpoint extension/relative route, method, parameters/body, response-root selection, execution and response-structure profiling.
- FILE: file-selection rules where required, format/parser settings such as header row, sample/read and field/type inference. The connection path/location is never repeated here.
- LDAP: no repetition of connector-owned host, port, authentication or directory/base connection context. Profile discovers the attributes exposed by the managed LDAP connection. Which discovered attributes are actually used, and what they mean to the target model, belongs to Mapping.

`OVSourceProfile` is the governed reusable Profile-region composition. It owns source-definition/result layout, selected-connection context, `Run & Profile` action placement, profiling progress, message/status placement and result placement. The consuming application owns transport-specific definition controls, validation, execution, cancellation/timeouts, secure request material, persistence and conversion of the returned data into the discovered field model.

Whenever the result is a discovered structured schema, render it with `OVC-STRUCTURED-FIELD-TREE`.

Changing the selected connection or source definition invalidates the previous profile result according to application rules; applications must not silently present an old schema as though it came from the new definition.

### Mapping

Mapping consumes the current profiled output and maps it into the declared target contract. It does not define or repeat connection details.

This distinction is particularly important for reusable FILE and LDAP connections:

- FILE Mapping maps discovered file columns/fields; it does not restate the file location.
- LDAP Mapping decides which discovered directory attributes are used and which target fields they populate; it does not restate LDAP connection/base details.

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

`managed connection → source-specific profile definition (when needed) → Run & Profile → discovered structure → mapping`

There is no separate top-level Source tab. Source-definition behaviour belongs to Profile because the definition and its discovered output form one profiling workflow. Connector-owned details remain in the reusable managed connection and must not be duplicated into Source Configuration.
