# OVP-MAPPING-RULE — governed rule editor

**Change:** Two mandatory tabs (Overview and Rule) and optional Target Namespace.
**Scope:** Presentation and reusable interaction; no application rule-engine, allocation or database changes.
**Foundation:** Does not change OVP-000.

## Governing composition

- **Overview**: Name, Description, optional Rule Outcome (application option set, e.g. AUTO / POTENTIAL), Priority and Enabled. Outcome is configured here, not duplicated in Rule outputs.
- **Rule**: Source (optional), Outputs (optional), Conditions (required), and optional supplementary Test Rule.
- **Target Namespace**: Optional independent third tab, visible only for enabled/disabled/read-only configuration. The application can supply target provider/source/type/identifier options and callbacks, and additional custom content; no target resolution occurs.

The application owns persistence and rule-engine semantics. The library owns the governed tabs and section containment. Save/Cancel remain with the owning application or overlay.

### Outputs versus Target Namespace

Outputs produces application-defined classifications, types, groups, items and mapped results. Target Namespace configures where *subsequent* target identification may take place. Neither implicitly modifies the other. A rule can have both, either or neither. The previous `ClassificationContent`, `TargetResolutionContent` and `OutcomeContent` continue rendering in the unified Outputs section, with corresponding legacy show flags. `OutcomeOptions` / `OutcomeKey` now render as **Rule Outcome** in Overview; there is **no** predefined Action field.

### Capability state

`OVMappingRuleCapabilityState` is `Unavailable`, `Hidden`, `Disabled`, `ReadOnly` or `Enabled`.

Unavailable/Hidden remove presentation without deleting consumer values. Disabled remains visible but non-editable. ReadOnly retains inspectable values. Enabled allows mutation unless the entire rule is read-only. Semantic state is selected by the consumer; applications determine validation and persistence of inactive configurations. `SourceState`, `OutputsState` and `TargetNamespaceState` are independent.

### Conditions

Each group owns its AND/OR join and joins its immediate conditions and child groups. No competing rule-level operator is shown. Empty root Conditions displays a compact empty state with Add condition when editable; it does not define runtime matching semantics. Entirely disabled groups remain represented. The existing immutable `OVMappingRuleGroup` and `OVMappingRuleCondition` identifiers, callbacks and operator validation are retained.

Typed field/value comparison uses the existing governed field, operator and value controls. For optional field-to-field comparison, applications supply `AllowFieldComparison`, `FieldComparisonOperatorKeys` and optional `ComparisonFields`; the selected right-hand field is stored using the existing condition value contract, with operator-specific eligibility and validation. This does not execute a join. The large `OVMappingJoin` editor is designed for two full data structures and persisted multi-pair join management and should not be embedded wholesale in a single condition row.

### Responsive and overlay behaviour

The component and each nested group establish container-query width contexts. At wider widths condition rows align Field / Operator / Value / Actions. At smaller component widths controls reflow into two columns and then one. Nested groups use bounded hierarchy markers rather than compounding margins. Existing governed dropdowns, validation and row controls remain responsible for their own accessible overlays. The host owns vertical scrolling and fixed footer clearance; this pattern does not install a second scrollport.

### Migration and compatibility

- Existing `RootGroup`, conditions, operator definitions, IDs and callbacks are unchanged.
- `RuleName`, `Enabled`, `SourceScopeContent`, `ClassificationContent`, `TargetResolutionContent`, `OutcomeOptions`, `OutcomeKey`, `OutcomeContent`, `TestRuleAsync` and related flags remain available.
- Source scope is now in the optional Source section of Rule.
- Classification, target resolution and additional `OutputsContent` are combined in one Outputs section.
- Rule Outcome moves from the previous Then area into Overview.
- New Overview fields are `Description` and optional `Priority`, both application-controlled.
- The host should use `ActiveTab` and `ActiveTabChanged` to preserve the selected tab with its draft; changing tabs does not persist rule data.
- The optional `TargetNamespaceContent` is generic. Provider, Source, Type and Identifier selectors can be supplied separately; their option dependencies and lookup requests remain app-owned.
- No application needs a competing local tab system or duplicated pattern borders.

## Required verification

Solution restore, build, contract/unit tests, browser accessibility and interaction tests; width tests for 320/480/720/1024px including nested conditions; optional namespace, output, scope, read-only, hidden and error-state demonstrations. Verify successful source PR, package, demo publish, public repo build hash and live gallery rendering separately. Do not certify IDM/EAM adoption from a library-only release.
