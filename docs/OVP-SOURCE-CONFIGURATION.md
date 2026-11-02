# OVP-SOURCE-CONFIGURATION

`OVP-SOURCE-CONFIGURATION` governs inbound source configuration through four peer sections: **Overview**, **Connection**, **Profile** and **Mapping**.

The pattern supports both reusable managed connections and direct sources whose location and interpretation are part of the source definition itself.

## Connection mode

The application supplies a semantic `OVSourceConnectionMode` for the current source type:

- `ManagedConnection` — a separately managed reusable connection is required.
- `NoConnectionRequired` — no reusable connection object is required; the source is defined directly in Profile.

The application determines the mode from source type. Users should not normally choose it manually.

Typical examples:

- SQL → `ManagedConnection`
- LDAP → `ManagedConnection`
- FILE → `ManagedConnection`
- JSON/API → `ManagedConnection`
- Web Table → `NoConnectionRequired`

This contract belongs to `OVP-SOURCE-CONFIGURATION`. It does **not** add a connection type to `OVC-CONNECTION-EDITOR` and it does not change that component's contract.

## Contract

### Overview

Owns source identity, description, enabled/status context and summary metadata. A direct source must not be shown as unhealthy or incomplete merely because it has no managed connection.

### Connection

#### Managed connection

For `ManagedConnection`, Connection remains intentionally narrow because it references a separately managed reusable connection object. That connection owns everything required to reach and authenticate to its endpoint.

Examples:

- SQL: server/database/authentication.
- JSON/API: base URL/authentication.
- FILE: resolved file/location/access.
- LDAP: host/port/authentication and reusable directory/base context required by the connector.

Source Configuration must not repeat connection-owned location, transport or access details merely because a transport-specific Profile is being configured. Reusable connection-object creation/editing remains governed by `OVC-CONNECTION-EDITOR`.

#### No connection required

For `NoConnectionRequired`, the Connection section remains one of the four governed peer sections but renders the intentional satisfied state:

**No managed connection required**

`This source is configured directly in Profile.`

It must not render an empty managed-connection selector, disabled connection controls, an incomplete state or an error solely because no connection object exists.

`NoConnectionRequired` satisfies the Connection stage by design. Applications must not implement a parallel no-connection placeholder or alternate terminology.

### Profile

Profile owns the source-specific definition and the profiling lifecycle.

For a managed source, this means the definition that remains after the reusable connection has resolved endpoint/access concerns. For a direct source, Profile also owns the location and interpretation values that define the source itself.

#### Managed-source examples

- **SQL:** Profile contains T-SQL only. The managed connection already resolves server/database/authentication.
- **JSON/API:** Profile may contain an optional endpoint extension/relative route when the managed connection base path is not the complete request URL.
- **FILE:** Profile contains only file-interpretation details that remain source-specific, such as header row and worksheet where applicable.
- **LDAP:** Profile provides the requested field/attribute list; host, port, authentication and reusable base context remain connection-owned.

#### Direct-source composition

A direct source may require definition fields and an optional discovery step before profiling. The reusable pattern is intentionally domain-neutral:

`direct definition → optional discovery/inspection/selection → Run & Profile → current profile result`

OneVision.UI owns the placement and lifecycle of this experience. The consuming application owns the actual domain fields, execution, parsing and returned data.

`OVSourceDiscovery` provides a governed discovery/selection/preview surface for sources where users need to inspect multiple discovered candidates before choosing one. It supports:

- a governed discovery action and busy state;
- success, empty and failure composition;
- human-readable candidate metadata;
- explicit accessible single selection;
- representative column headings and sample rows;
- unusable candidate states;
- a bounded, horizontally scrollable preview that does not widen the page;
- responsive stacking at narrower widths;
- read-only presentation.

The component is not a browser or scraper. Applications perform network calls, parsing and candidate generation.

### Web Table reference composition

Web Table is the primary reference implementation of `NoConnectionRequired`; Web Table behavior is not embedded into `OVSourceConfiguration`.

The Profile flow is:

`Page URL → Discover tables → inspect available tables → preview actual representative data → select table → interpretation → Run & Profile`

The page URL belongs to Profile because it defines the direct source; it is not a managed Connection endpoint.

Discovery should return every relevant table with useful human-readable context where available, such as:

- table order/index;
- caption/title;
- column count;
- approximate row count;
- representative headings.

A user must be able to inspect representative **actual rows and columns** from a discovered table before committing to it as the source. The interface must not force the user to choose an opaque technical table index and hope it is correct.

Where multiple tables are found, the governed reference layout presents the candidate list alongside the selected table's preview on wider screens and stacks them at narrower widths.

Interpretation such as **First row contains column headings** remains Profile-owned because it changes how the selected table is understood.

The consuming application owns HTTP execution, HTML retrieval/parsing, timeout/cancellation, discovered table metadata, preview data, profile execution and schema generation.

### Profile state and invalidation

`OVSourceProfileState` expresses the governed lifecycle:

- `Initial`
- `Running`
- `Current`
- `Stale`
- `Failed`

Any source-definition value that can change the returned schema must invalidate a current profile result. For Web Table this includes at least:

- page URL;
- selected table;
- header-row interpretation.

The governed stale behavior is explicit: the previous result may remain visible for context and is clearly marked **Stale**, but it is not authoritative.

**A stale profile result must not be treated as the current source structure by Mapping.**

When `OVSourceConfiguration.ProfileState` is `Stale`, the Mapping section suppresses the application-supplied mapping content and presents a governed instruction to rerun profiling first. This prevents an old schema from appearing usable after the source definition changes.

Applications own the domain rules that decide whether a definition change affects schema; when it does, they set the profile state accordingly.

Whenever the current result is a discovered structured schema, render it with `OVC-STRUCTURED-FIELD-TREE`.

A managed structured profile may optionally allow the consuming application to designate exactly one array node as its logical **record collection**. Enable this with `RecordCollectionSelectionMode="SingleArray"` and bind `SelectedRecordCollectionPath` / `SelectedRecordCollectionPathChanged`. Record collection semantics remain independent of direct-source support.

### Run & Profile

`OVSourceProfile` owns source-definition/result layout, connection context when applicable, `Run & Profile` action placement, profiling progress, message/status placement, stale presentation, result placement and read-only action suppression.

The consuming application owns actual execution, validation, cancellation/timeouts, secure request material, persistence and conversion of returned data into the discovered field model.

### Mapping

Mapping semantics are unchanged. It consumes only the **current** profiled output and maps that into the declared target contract.

Use `OVC-MAPPING-PAIR` for directional source-to-target mapping and optionally `OVC-MAPPING-RULE` where source-specific rules are required. Direct sources must not introduce a separate mapping convention.

#### Required mappings

When the consuming feed contract supplies required targets, they remain permanently visible and cannot be removed as target definitions. Missing or invalid required mappings make the configuration invalid. OneVision.UI does not define which targets are required; the consuming application/feed supplies them.

#### Optional Standard Details

`OVC-MAPPING-PAIR` supports the existing opt-in Standard Details capability through `StandardDetailsEnabled`. This behavior is unchanged by direct-source support.

The application supplies `OVStandardDetailDefinition` values. Only details selected for the current source are rendered as mapping rows and selected enabled details must be mapped validly before save.

`OVMappingPairValidation.IsValid(...)` remains the generic contract check: every required target and every selected enabled Standard Detail must have a valid mapping.

#### Optional Display Value Builder

`OVP-SOURCE-CONFIGURATION` may opt into a governed Display Value Builder within Mapping. The builder is not a fifth Source Configuration section and does not select directly from profiled source fields.

Its input model is the **current Mapping target set**:

- core/default/required mapping targets already present in Mapping; and
- Standard Details that have actually been added to the current Mapping composition.

This establishes the semantic flow:

`profiled source fields → Mapping establishes target meaning → Display Value composes mapped meaning`

The builder must not expose the raw source field tree as its selectable catalogue. Source paths remain a Mapping concern.

Applications supply `OVDisplayValueTargetOption` values keyed by the same stable target identity used by Mapping. `OVDisplayValueDefinition` stores ordered target/literal segments by `TargetKey`; it does not require applications to persist raw tokenised template expressions.

The governed `OVDisplayValueBuilder` supports:

- grouped **Core fields** and **Standard Details** catalogues;
- adding mapped-target segments;
- adding short literal/text segments;
- visually distinct field and text segments;
- remove actions;
- deterministic move-left / move-right reordering that is keyboard accessible;
- live preview using application-supplied target sample values;
- semantic bracketed placeholders such as `[Display name]` when sample values are unavailable, never raw template tokens;
- clear empty, unresolved and read-only states;
- responsive wrapping without page-level horizontal overflow.

The collection order of `OVDisplayValueDefinition.Segments` is authoritative. Each segment has its own stable `Id` for UI identity. Target segments reference `TargetKey`; literal segments carry only their short literal value.

If a Standard Detail is removed from Mapping while an existing display definition still references it, the segment remains visible as **Mapped field removed** and the definition becomes invalid. The reference must not silently retarget or disappear. The user must repair or remove the unresolved segment before the definition is considered valid.

Literal-only definitions are disallowed by default. Applications can deliberately enable that semantic policy with `DisplayValueAllowLiteralOnly` / `AllowLiteralOnly`; OneVision.UI owns the resulting validation presentation.

When `DisplayValueBuilderEnabled` is false, existing Source Configuration consumers are unchanged.

The Source Configuration integration contract is:

- `DisplayValueBuilderEnabled`
- `DisplayValueTargets`
- `DisplayValueDefinition`
- `DisplayValueDefinitionChanged`
- `DisplayValueReadOnly`
- `DisplayValueAllowLiteralOnly`

OneVision.UI owns the builder interaction and presentation. Applications own which Mapping targets are currently present, sample values, default definition, persistence and runtime interpretation of the semantic definition.

## Validation and readiness

Validation is connection-mode aware:

- `ManagedConnection` — existing managed connection validation remains applicable.
- `NoConnectionRequired` — a managed connection is not required. Readiness is determined by the direct Profile definition supplied by the application.

The Connection stage must distinguish **Connection configured** from **No connection required** without treating the latter as incomplete or erroneous.

For Web Table, a normal ready-to-profile definition requires a valid URL and a usable selected table, plus any source-specific interpretation required by the application.

When the Display Value Builder is enabled and a display definition is present, applications should include `OVDisplayValueValidation.IsValid(...)` in their Mapping/save readiness decision. An unresolved target reference must not be saved as a valid current display definition.

## Read-only behavior

Read-only direct sources preserve the same four sections:

- Connection still communicates **No managed connection required**;
- direct definition values and selected candidate remain understandable;
- discovery and `Run & Profile` mutation actions are suppressed;
- preview/profile results remain visible;
- Mapping follows its existing read-only contract.

When the Display Value Builder is read-only, its segment composition and preview remain visible, target/literal distinction remains explicit and all add/remove/reorder/edit controls are suppressed. An unresolved target remains visibly unresolved rather than falling back to raw syntax.

## Responsive and accessibility contract

Direct-source composition follows existing OneVision administrative responsive rules. Candidate selection and preview may sit side-by-side at wider widths but must stack cleanly at narrower widths. Wide preview tables use bounded internal overflow; they must not introduce page-level horizontal scrolling.

The Display Value Builder may use a two-column target/composition workspace at wider widths and stacks it at narrower widths. Segment order remains explicit after wrapping, and reordering must not depend on drag-and-drop.

The governed experience must preserve labelled controls, announced busy/status states, keyboard-accessible selection, semantic table preview markup, non-colour-only state meaning and logical focus progression.

## Ownership boundary

OneVision.UI owns:

- Overview / Connection / Profile / Mapping section structure;
- managed vs no-connection-required presentation;
- intentional satisfied Connection state;
- Profile source-definition/result composition and profile-state presentation;
- discovery action placement, loading/error/empty/result composition;
- generic discovered-candidate selection and preview presentation;
- stale-profile presentation and protection against stale Mapping;
- discovered-structure presentation through governed structured-data controls;
- existing mapping composition;
- optional Display Value Builder composition, ordering, preview, read-only and unresolved-target presentation;
- reusable density, responsive layout, read-only behavior and accessibility.

Applications own:

- which source types require managed connections;
- available/eligible managed connections;
- direct-source field definitions;
- network/request execution and parsing;
- discovered candidate metadata and preview data;
- actual SQL/API/file/LDAP/Web Table execution;
- source-specific validation, permissions and persistence;
- profile/schema materialisation;
- timeout/cancellation and secure credential handling;
- required mapping definitions and domain semantics;
- which mapping targets are exposed to the optional Display Value Builder;
- display-value sample data, default definition, persistence and runtime interpretation.

## Demo/reference states

The gallery demonstrates both managed SQL and direct Web Table sources.

Web Table includes reference states for:

- initial;
- discovering;
- discovery failure;
- no tables found;
- multiple tables found;
- selected table with real data preview;
- profile running;
- profile success;
- profile failure;
- stale result after definition change;
- read-only configuration.

The focused Display Value Builder reference demonstrates:

- core Mapping targets;
- added Standard Details;
- field + literal composition;
- live sample preview;
- empty state;
- removed Standard Detail / unresolved target state;
- read-only presentation.

## Out of scope

This pattern does not:

- create a Web Table managed connection type;
- add Web Table settings to `OVC-CONNECTION-EDITOR`;
- perform HTTP requests or HTML parsing;
- become a generic scraping/browser tool;
- change existing SQL/LDAP/FILE/JSON connection ownership;
- replace required mapping or Standard Details;
- expose raw token/template editing;
- introduce expression scripting, conditional logic or arbitrary executable expressions;
- introduce application-specific or EAM-specific behavior.

## Foundation impact

This capability extends `OVP-SOURCE-CONFIGURATION` only. It does not alter `OVP-000 – OneVision Application Foundation`, so no OVP-000 version increment is required.
