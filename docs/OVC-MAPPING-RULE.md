## Resolution-aware matching refinement (0.1.52)

The governed three-tab rule editor remains Overview / Rule / Conditions. The Rule tab accepts an authoritative, application-supplied `OVMappingGoverningContract` and optional application terminology. The previous EAM-named Allocation Rule parameters remain compatibility adapters without separate UI state.

Conditions may use `OVMappingResolutionFields` to show governed, application-supplied source/target field trees in Direct mode, or source/associated-field trees in Associated mode. The user activates an existing condition using **Edit pair** before choosing fields. Tree selections update the existing condition through `ConditionChanged`, without creating new conditions or inventing another condition/group model. Types, operators, field identities and candidate associations remain application owned.

Results support generic evidence origins, relationship references and processing eligibility while preserving EAM-named result adapters. Consuming applications own all matching, eligibility and persistence decisions. EAM displays its approved Allocation Rule / Associated Standard Details terminology through `Terminology`, not through hard-coded library behaviour.

# OVC-MAPPING-RULE — Conditional mapping rule contract

**Governance assessment:** 5 November 2026. Existing `OVC-MAPPING-RULE` is extended, not replaced. **No OVP-000 contract change.** Application integrations are not implicitly certified by library tests.

## Governing presentation redesign — 0.1.50

The authoritative composite is `OVP-MAPPING-RULE` (see
`docs/OVP-MAPPING-RULE.md`): governed Overview and Rule tabs plus optional
Target Namespace. Within Rule, Source, Outputs and Conditions appear in that
order, with Source/Outputs independently optional. Outcome is configured
in Overview, not duplicated as an output/action field. All five capability
states and container-aware nested groups are governed presentation states.

Existing callbacks and immutable condition/group model remain compatible.
Consumer applications are responsible for any data migration, persistence,
rule evaluation and configured target namespace option dependencies.
The `OVMappingJoin` rich two-sided join manager was reviewed; field-to-field
condition comparison reuses governed field selects rather than embedding the
entire multi-pair join workspace into each condition row.

## Allocation-contract-driven EAM composition (0.1.51)

The authoritative `OVP-MAPPING-RULE` is now an EAM allocation-contract-driven composition:
**Overview → Rule → Conditions**. The Rule tab references an Allocation Rule,
displays the inherited source and target relationship read-only, and captures
Auto/Potential Outcome and Direct/Associated resolution method. Conditions retain
the existing governed condition groups and optional Test Rule. The prior
independently configured Target Namespace is not part of this EAM composition;
the allocation contract already supplies the target context. See
`docs/OVP-MAPPING-RULE.md` for component contracts, presentation ownership,
migration and test-evidence extension. `Composition=Legacy` is a deliberate
temporary consumer-migration aid, not a second supported EAM editing pattern.
No OVP-000 foundation change is required.

## Purpose, ownership and reuse

This component edits conditional decisions over application-supplied source fields, optionally combining conditions with AND, OR and nested groups. It is reusable for source-to-target classification, mapping eligibility and other governed rule-building workflows. It does not execute a rule, query an operational source, persist an outcome or own an allocation.

OneVision.UI owns the rule editor, input/validation grammar, optional group interactions, accessible state presentation and optional evaluation-result display. The consuming application owns field metadata, any further operator restrictions, rule execution, source scoping, target catalogues, dependent lookups, conflict policies, approval and persistence. EAM is authoritative for allocations resulting from IDM decisions.

## Ten-point contract and gap review

| Review dimension | Current source evidence | Required action |
| --- | --- | --- |
| Purpose and boundaries | `OVMappingRule` already exposes rule, group and outcome callbacks | Retain the generic contract; no Episode/competency semantics in the library |
| Reusability | Application-supplied operands/operators and `OutcomeContent` exist | Add optional typed metadata and a reusable test-result contract |
| Density | Condition editor is compact but value field always appears | Hide value for unary operators; keep row compact and responsive |
| Interaction | Add/remove conditions and groups, AND/OR join exist | Expose condition enablement, ordering and discoverable rule feedback |
| Flexibility | Opt-in nested groups and outcome fragment | Keep these; add independent test capability and dependent target composition via existing outcome slot |
| Validation | Applications currently supply `IsValid` / `ValidationMessage` | Add optional operator-aware local validation, preserving app errors |
| Accessibility | Labelled governed inputs and icon actions | Check dynamic field/value alternatives, test feedback, keyboard grouping and focus |
| Edge cases | Empty/loading/error/read-only present | Explicit null/empty, wildcard escape, disabled, stale dependent options, ambiguous preview |
| Developer API | Existing group/condition callback models in use | Add opt-in metadata and callbacks without breaking existing parameters |
| Tests/demo/docs | No dedicated live Mapping Rule demo or canonical rule contract document | Add unit/browser tests and real capability-combination demo |

## Initial stable operator keys

| Key | Label | Source types | Comparison value |
| --- | --- | --- | --- |
| `equals` | Equals | Text, Number, Date, Boolean | Required |
| `not-equals` | Not equals | Text, Number, Date, Boolean | Required |
| `contains` | Contains | Text | Required |
| `not-contains` | Does not contain | Text | Required |
| `starts-with` | Starts with | Text | Required |
| `ends-with` | Ends with | Text | Required |
| `wildcard` | Wildcard pattern | Text | Required |
| `is-empty` | Is empty | All types | None |
| `is-not-empty` | Is not empty | All types | None |

`OVMappingRuleFieldDefinition` supplies a stable field key, label, type and optional eligible operator keys and comparison choices. `OVMappingRuleOperatorContract` supplies the reusable operator catalogue and validation. Existing `Operands` / `Operators` parameters and `OVMappingRuleCondition` positional constructor remain compatible.

Dates use ISO `yyyy-MM-dd` comparison input and numbers use invariant decimal notation for this initial contract. Applications can restrict the available operators; the editor must never offer a disallowed operation. Field transitions must clear or invalidate incompatible operators/values.

### Wildcard and null semantics

The `wildcard` operator uses SQL-LIKE-style rules: `%` matches zero or more characters, `_` matches exactly one character; `\\%`, `\\_` and `\\\` represent literal percent, underscore and backslash. A trailing `\` or an unsupported escape is invalid. The library validates the expression but **does not run or translate it to SQL**.

`is-empty` is true for null or the zero-length string, but not whitespace-only text; `is-not-empty` is its complement. A null source value cannot satisfy the other seven operators, including `not-equals` and `not-contains`. An empty comparison value is rejected for those operators; use the unary empty checks. Case-sensitivity policy must be explicitly selected by the application (`ApplicationDefault`, `CaseSensitive`, `CaseInsensitive`) and implemented by its runtime. The default is not a promise of case-insensitive behaviour.

Disabled conditions are excluded from evaluation and do not invalidate editing; an enabled condition must refer to an available field, allowed operator and valid required comparison. Validation cannot infer rule matches or approve candidates.

### Compatible component enhancement — 0.1.29

The original `OVMappingRule` API (`Operands`, `Operators`, `RootGroup`, `OutcomeOptions`, `OutcomeContent`, named callbacks and group/condition records) is preserved. Typed metadata is **opt-in** using `FieldDefinitions` and optional `OperatorDefinitions`. Without typed fields, existing consumers continue using their supplied field/operator options and application-provided validation unchanged.

With typed fields enabled:

- the field list comes from `OVMappingRuleFieldDefinition`;
- the operator list is filtered by type and field-specific allowed keys;
- the comparison value is hidden for `is-empty` and `is-not-empty`;
- optional `ComparisonValues` replace free-text comparison with a governed select;
- incompatible field changes invalidate/clear the selected operator and dependent value;
- typed local validation appears inline, while an explicit application `IsValid=false` remains authoritative.

The `AllowConditionDisabling` and `AllowReordering` capabilities are independent and disabled by default. Reordering emits `ConditionMoveRequested` with a group key, condition key and relative offset; consuming applications update order and persist as needed. Disabled conditions remain visible, are excluded from evaluation, and do not block validation.

`ShowCasePolicy` exposes a governed application-selectable case policy; it does not implement a matching engine.

`SourceScopeContent`, `ClassificationContent` and `TargetResolutionContent` are optional presentation slots. `ShowClassification` and `ShowTargetResolution` explicitly suppress optional sections without breaking the named Blazor content contract. Source qualification, classification and dependent target selections can be shown vertically in one builder without a new OVP. The consuming application owns source options, dependent option retrieval, clearing stale target values, rule persistence and outcome semantics. The live gallery demonstrates these dependencies with synthetic data.

`TestInputContent`, `TestRuleAsync` and `TestContextKey` enable a read-only Test Rule interaction. The library owns pending, explanation and error displays, and invalidates displayed results when the rule group or test context key changes. The result includes input, match outcome, condition evidence, optional proposed target, ambiguity and errors. Exceptions render a non-sensitive generic error. The callback executes in the consuming application; the component **never** creates candidates, approves a relationship, submits an allocation or modifies operational data.

### Gallery and consumer boundaries

The component gallery renders `OVMappingRule` itself with states for single/AND/OR/nested/wildcard/invalid/ambiguous/read-only/empty/loading/error/responsive configurations. Independent modifiers expose nested groups, condition disabling, ordering, case policy, Test Rule, classification and target resolution. Synthetic Episode descriptions and competency choices illustrate realistic use without disclosing operational records.

**IDM is a separate consuming-application stage**: source-scoped Episode processing, candidate/approval workflows, Worker recovery, conflict handling, EAM bulk allocation and reconciliation are not implemented by this UI contract. The IDM repository, runtime and deployment must be independently verified before those work items are claimed complete.

## Acceptance and release control

- Contract/unit tests cover the operator catalogue, type restrictions, wildcard and escaping, empty semantics, disabled conditions and backward-compatible models.
- Editor integration then needs rendering, keyboard, read-only, responsive and browser coverage including genuine callback-driven Test Rule.
- Library build/test and actual package/gallery publication must be verified independently. Only a merged, tested commit should be published. IDM adoption remains unverified until its repository is inspected and tested.

## UI delivery trace

0.1.29 adds the optional editor enhancements, callback-driven read-only Test Rule, real component gallery capability combinations and automated contract/browser test coverage. IDM adoption and EAM operational integration must be verified separately.
