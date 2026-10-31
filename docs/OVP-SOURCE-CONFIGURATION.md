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
- **LDAP:** The managed connection resolves host, port, authentication and reusable directory/base context, but it does not determine which attributes the source should request. Profile therefore provides a field/attribute list defining the LDAP fields to retrieve, then profiles the returned attributes. Do not repeat connector-owned connection/base details. Mapping decides how the returned fields map into the target model.

`OVSourceProfile` is the governed reusable Profile-region composition. It owns source-definition/result layout, selected-connection context, `Run & Profile` action placement, profiling progress, message/status placement and result placement. The consuming application owns the actual execution, validation, cancellation/timeouts, secure request material, persistence and conversion of returned data into the discovered field model.

Whenever the result is a discovered structured schema, render it with `OVC-STRUCTURED-FIELD-TREE`.

A structured profile may optionally allow the consuming application to designate exactly one array node as its logical **record collection**. Enable this with `RecordCollectionSelectionMode="SingleArray"` and bind `SelectedRecordCollectionPath` / `SelectedRecordCollectionPathChanged`. Only array nodes are eligible. OneVision.UI owns array-only selection, the single-selected-state interaction, the `Record collection` indicator, read-only presentation, retention across equivalent rerenders and clearing an invalid selection when the selected array disappears. Normal structured-tree selection remains independent.

The consuming application owns what a record collection means, whether the capability is enabled, persistence, domain validation, mapping eligibility and all extraction/iteration semantics. Record collection selection belongs to **Profile**; `OVC-MAPPING-PAIR` consumes the resulting profiled structure and does not acquire record-collection semantics.

Changing the selected connection or any remaining source-specific Profile definition invalidates the previous profile result according to application rules; applications must not silently present an old schema as though it came from the new definition.

### Mapping

Mapping consumes the current profiled output and maps it into the declared target contract. It does not define or repeat connection details, and it does not own record-collection selection.

This distinction is particularly important for reusable FILE and LDAP connections:

- FILE Mapping maps discovered file columns/fields; it does not restate the file location.
- LDAP Mapping maps the requested/profiled LDAP fields to target fields; it does not restate LDAP connection/base details.

Use `OVC-MAPPING-PAIR` for directional source-to-target mapping and optionally `OVC-MAPPING-RULE` where source-specific rules are required.

#### Required mappings

When the consuming feed contract supplies required targets, they remain permanently visible and cannot be removed as target definitions. Missing or invalid required mappings make the configuration invalid. OneVision.UI does not define which targets are required; the consuming application/feed supplies them.

#### Optional Standard Details

`OVC-MAPPING-PAIR` supports an opt-in Standard Details capability through `StandardDetailsEnabled`. It is disabled by default, so existing consumers retain the current mapping experience without supplying a catalogue.

When enabled, Mapping separates:

- **Required mappings** — application/feed supplied contract targets.
- **Standard Details** — optional, centrally governed semantic details supplied by the consuming application.

The application supplies `OVStandardDetailDefinition` values with system-owned `Id`, user-facing `Name`, optional `Description`, optional `DataType`, and `Enabled` state. OneVision.UI does not own or create this catalogue and never asks the user to enter technical target codes.

Only Standard Details explicitly selected for the current feed are rendered as mapping rows. **Add standard detail** opens the governed picker of enabled, not-yet-selected details. The picker displays user-facing names/descriptions, supports adding one or more details, and excludes already-selected details.

Once selected, a Standard Detail becomes a normal mapping target with semantic kind `StandardDetail`. It must be mapped validly before save. Removing it removes that feed-level selection and its mapping only; it does not mutate the application-owned catalogue definition.

`OVMappingPairValidation.IsValid(...)` provides the generic contract check used by the pattern: every required target must have a valid mapping, and every selected enabled Standard Detail must have a valid mapping. Unselected Standard Details do not participate in validation.

Read-only mode preserves both required and selected Standard Detail mappings while suppressing add/remove/edit interactions. Responsive stacking follows the existing mapping-pair behaviour.

## Ownership boundary

OneVision.UI owns:

- Overview / Connection / Profile / Mapping section structure.
- Profile source-definition/result composition.
- `Run & Profile` action placement and profiling-state presentation.
- discovered-structure presentation through governed structured-data controls.
- optional record-collection selection interaction and selected-state presentation for structured profiles.
- Required versus selected Standard Detail mapping composition when the optional capability is enabled.
- Add Standard Detail interaction, duplicate-selection prevention, read-only interaction state and generic validation presentation.
- reusable density, responsive layout, validation/status placement and accessibility.

Applications own:

- which managed connections are available and eligible.
- the actual SQL/API/file/LDAP execution implementation.
- source-specific validation and permissions.
- secret/credential handling.
- profile/schema materialisation and persistence.
- record-collection meaning, persistence, domain validation and extraction/iteration semantics.
- required mapping definitions for each feed/source type.
- the Standard Details catalogue and whether a feed enables the capability.
- persistence and ingest-time interpretation of required and Standard Detail mappings.
- target/domain mapping semantics beyond the generic mapping contract.

Applications may distinguish persisted mapping targets as `REQUIRED` and `STANDARD_DETAIL`; OneVision.UI does not prescribe a database schema.

## Interaction flow

`managed connection → minimum source-specific Profile definition (when needed) → Run & Profile → discovered structure → optional record collection → mapping`

Where Standard Details are enabled:

`required mappings → optional Add standard detail → selected detail mappings → validation/save`

There is no separate top-level Source tab. Connector-owned details remain in the reusable managed connection and must not be duplicated into Source Configuration.

## Foundation impact

This capability extends `OVP-SOURCE-CONFIGURATION` only. It does not alter `OVP-000 – OneVision Application Foundation`, so no OVP-000 version increment is required.
