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
- FILE: resolved file/location/access.
- LDAP: host/port/authentication and reusable directory/base context required by the connector.

Source Configuration must not repeat connector-owned location, transport or access details merely because a transport-specific Profile is being configured. Reusable connection-object creation/editing remains governed by `OVC-CONNECTION-EDITOR`.

### Profile

Profile owns only the **source-specific definition or interpretation that is not already resolved by the managed connection**, plus the **profiling lifecycle**. The user supplies the minimum remaining source-specific information, chooses **Run & Profile**, and inspects the returned/discovered structure.

Transport-specific contract:

- **SQL:** Profile contains T-SQL only. The managed connection already resolves server/database/authentication. The T-SQL may query tables/views, execute stored procedures, declare parameters, use CTEs or otherwise define the returned dataset. Do not add separate table/view/stored-procedure selectors that duplicate what can be expressed in T-SQL.
- **JSON/API:** Profile may contain an optional endpoint extension/relative route when the managed connection base path is not the complete request URL. Do not add a response-root selector: the returned JSON is profiled as-is and `OVC-STRUCTURED-FIELD-TREE` resolves its hierarchy. Do not duplicate connector-owned base URL/authentication/request transport configuration.
- **FILE:** The managed connection already resolves the file/location. Profile contains only file-interpretation details that remain source-specific, such as header row and worksheet where applicable. Do not repeat file path, file pattern, format selection or other connection-owned file-location controls unless a consuming application has a genuinely separate source-specific requirement.
- **LDAP:** The managed connection is fully resolved apart from the fields exposed to the source. Profile does not repeat host, port, authentication, base/search context or field-selection controls. Run Profile to discover the available fields. Which discovered fields are used, and what they mean to the target model, belongs to Mapping.

`OVSourceProfile` is the governed reusable Profile-region composition. It owns source-definition/result layout, selected-connection context, `Run & Profile` action placement, profiling progress, message/status placement and result placement. The consuming application owns the actual execution, validation, cancellation/timeouts, secure request material, persistence and conversion of returned data into the discovered field model.

Whenever the result is a discovered structured schema, render it with `OVC-STRUCTURED-FIELD-TREE`.

Changing the selected connection or any remaining source-specific Profile definition invalidates the previous profile result according to application rules; applications must not silently present an old schema as though it came from the new definition.

### Mapping

Mapping consumes the current profiled output and maps it into the declared target contract. It does not define or repeat connection details.

This distinction is particularly important for reusable FILE and LDAP connections:

- FILE Mapping maps discovered file columns/fields; it does not restate the file location.
- LDAP Mapping decides which discovered directory fields are used and which target fields they populate; it does not restate LDAP connection/base details.

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
- the actual SQL/API/file/LDAP execution implementation.
- source-specific validation and permissions.
- secret/credential handling.
- profile/schema materialisation and persistence.
- target/domain mapping semantics.

## Interaction flow

`managed connection → minimum source-specific Profile definition (when needed) → Run & Profile → discovered structure → mapping`

There is no separate top-level Source tab. Connector-owned details remain in the reusable managed connection and must not be duplicated into Source Configuration.
