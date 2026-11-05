# OVC-MAPPING-RULE — Conditional mapping rule contract

**Governance assessment:** 5 November 2026. Existing \`OVC-MAPPING-RULE\` is extended, not replaced. **No OVP-000 contract change.** Application integrations are not implicitly certified by library tests.

## Purpose, ownership and reuse

This component edits conditional decisions over application-supplied source fields, optionally combining conditions with AND, OR and nested groups. It is reusable for source-to-target classification, mapping eligibility and other governed rule-building workflows. It does not execute a rule, query an operational source, persist an outcome or own an allocation.

OneVision.UI owns the rule editor, input/validation grammar, optional group interactions, accessible state presentation and optional evaluation-result display. The consuming application owns field metadata, any further operator restrictions, rule execution, source scoping, target catalogues, dependent lookups, conflict policies, approval and persistence. EAM is authoritative for allocations resulting from IDM decisions.

## Ten-point contract and gap review

| Review dimension | Current source evidence | Required action |
| --- | --- | --- |
| Purpose and boundaries | \`OVMappingRule\` already exposes rule, group and outcome callbacks | Retain the generic contract; no Episode/competency semantics in the library |
| Reusability | Application-supplied operands/operators and \`OutcomeContent\` exist | Add optional typed metadata and a reusable test-result contract |
| Density | Condition editor is compact but value field always appears | Hide value for unary operators; keep row compact and responsive |
| Interaction | Add/remove conditions and groups, AND/OR join exist | Expose condition enablement, ordering and discoverable rule feedback |
| Flexibility | Opt-in nested groups and outcome fragment | Keep these; add independent test capability and dependent target composition via existing outcome slot |
| Validation | Applications currently supply \`IsValid\` / \`ValidationMessage\` | Add optional operator-aware local validation, preserving app errors |
| Accessibility | Labelled governed inputs and icon actions | Check dynamic field/value alternatives, test feedback, keyboard grouping and focus |
| Edge cases | Empty/loading/error/read-only present | Explicit null/empty, wildcard escape, disabled, stale dependent options, ambiguous preview |
| Developer API | Existing group/condition callback models in use | Add opt-in metadata and callbacks without breaking existing parameters |
| Tests/demo/docs | No dedicated live Mapping Rule demo or canonical rule contract document | Add unit/browser tests and real capability-combination demo |

## Initial stable operator keys

| Key | Label | Source types | Comparison value |
| --- | --- | --- | --- |
| \`equals\` | Equals | Text, Number, Date, Boolean | Required |
| \`not-equals\` | Not equals | Text, Number, Date, Boolean | Required |
| \`contains\` | Contains | Text | Required |
| \`not-contains\` | Does not contain | Text | Required |
| \`starts-with\` | Starts with | Text | Required |
| \`ends-with\` | Ends with | Text | Required |
| \`wildcard\` | Wildcard pattern | Text | Required |
| \`is-empty\` | Is empty | All types | None |
| \`is-not-empty\` | Is not empty | All types | None |

\`OVMappingRuleFieldDefinition\` supplies a stable field key, label, type and optional eligible operator keys and comparison choices. \`OVMappingRuleOperatorContract\` supplies the reusable operator catalogue and validation. Existing \`Operands\` / \`Operators\` parameters and \`OVMappingRuleCondition\` positional constructor remain compatible.

Dates use ISO \`yyyy-MM-dd\` comparison input and numbers use invariant decimal notation for this initial contract. Applications can restrict the available operators; the editor must never offer a disallowed operation. Field transitions must clear or invalidate incompatible operators/values.

### Wildcard and null semantics

The \`wildcard\` operator uses SQL-LIKE-style rules: \`%\` matches zero or more characters, \`_\` matches exactly one character; \`\\%\`, \`\\_\` and \`\\\\\` represent literal percent, underscore and backslash. A trailing \`\\\` or an unsupported escape is invalid. The library validates the expression but **does not run or translate it to SQL**.

\`is-empty\` is true for null or the zero-length string, but not whitespace-only text; \`is-not-empty\` is its complement. A null source value cannot satisfy the other seven operators, including \`not-equals\` and \`not-contains\`. An empty comparison value is rejected for those operators; use the unary empty checks. Case-sensitivity policy must be explicitly selected by the application (\`ApplicationDefault\`, \`CaseSensitive\`, \`CaseInsensitive\`) and implemented by its runtime. The default is not a promise of case-insensitive behaviour.

Disabled conditions are excluded from evaluation and do not invalidate editing; an enabled condition must refer to an available field, allowed operator and valid required comparison. Validation cannot infer rule matches or approve candidates.

### Existing compatibility and future UI stages

This first implementation tranche adds **operator metadata, wildcard syntax validation and unit tests only**; the existing editor and existing app consumers remain visually and behaviourally unchanged. A subsequent tranche wires the metadata into \`OVMappingRule\` and \`OVMappingRuleGroupEditor\`, adds optional condition enablement/order and the read-only Test Rule callback/result view, and exposes real capability combinations in the gallery. No new first-class OVP is warranted while \`OutcomeContent\` supports the reusable composition.

The later IDM composition must apply source scope, Episode type and dependent Resolution Type → Competency Group → Item under an application-owned rule editor using this component. Matching creates candidates with evidence and ambiguity handling; approved allocations use EAM APIs. Removing the obsolete Episode Attribute Set processing stage does **not** delete EAM-owned attribute data.

## Acceptance and release control

- Contract/unit tests cover the operator catalogue, type restrictions, wildcard and escaping, empty semantics, disabled conditions and backward-compatible models.
- Editor integration then needs rendering, keyboard, read-only, responsive and browser coverage including genuine callback-driven Test Rule.
- Library build/test and actual package/gallery publication must be verified independently. Only a merged, tested commit should be published. IDM adoption remains unverified until its repository is inspected and tested.
