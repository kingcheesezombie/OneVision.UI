# OVP-MAPPING-RULE — EAM allocation-contract-driven mapping rule

**Contract revision:** 0.1.51. **Owner:** OneVision.UI presentation contract; EAM owns rule data, matching, allocation policy and persistence. **Foundation impact:** none (OVP-000 unchanged).

## Governing model

An **Allocation Rule** is the authority for permitted source → target allocation relationships. A **Matching Rule** references that Allocation Rule and configures how its eligible source records are matched to targets. Matching rules never establish a competing allocation contract. EAM evaluates conditions, checks ambiguity and eligibility, approves allocations and persists provenance. Endpoint applications supply data/integration capabilities but do not host this matching-rule configuration.

The standard `OVMappingRule` composition is **AllocationContract**, with **exactly three tabs**:

- **Overview** — Name (required), Description (optional), Enabled (optional).
- **Rule** — Allocation Rule (required selector with inherited read-only Source Entity / Target summary), Outcome (Auto/Potential), Type (Direct/Associated).
- **Conditions** — existing field/operator/typed value comparisons, grouped AND/OR, optional nested groups and optional Test Rule.

No independent Source, Outputs or Target Namespace configuration is displayed in this composition. Target namespace is inherited from the Allocation Rule rather than configured here.

### Allocation Rule and inherited contract

The app supplies `AllocationRuleKey`, `AllocationRules`, `AllocationContract` and `AllocationRuleKeyChanged`. `OVMappingAllocationContract` is a view-only contract summary (rule ID/name, source class/type/system, target provider/type, enabled, validity, validation message and optional revision). When rule selection changes, EAM asynchronously supplies the new contract and eligible comparison fields; it owns invalidation of stale or incompatible condition references. Use `AllocationContractLoading` / `AllocationContractError` to present pending/unavailable states; the UI never clears values or interprets EAM's permission matrix.

Changing or disabling the underlying Allocation Rule must cause EAM to revalidate dependent matching rules. An unavailable inherited contract remains inspectable and is not presented as executable.

### Outcome and resolution method

`OutcomeKey` / `OutcomeKeyChanged` represent Auto/Potential; options may be app-supplied. Auto does not imply that OneVision.UI approves anything. `ResolutionType` / `ResolutionTypeChanged` select Direct or Associated.

- **Direct** compares source values and Standard Details against corresponding values held directly on the target.
- **Associated** compares against Standard Details available through eligible approved allocations associated with the target. The originating associated entity and allocation reference are app-owned evidence.

The application supplies the available condition field definitions and eligible operators for the selected contract and resolution method; stale conditions remain visible with validation. The app decides whether distinct associated entities can jointly satisfy a condition group. OneVision.UI does not infer associated evidence.

### Test Rule evidence

The existing `OVMappingRuleTestResult` remains compatible and optionally includes a target display key, resolution type, `OVMappingRuleEvidence` records, distinct target count and an `OVMappingRuleEligibility` snapshot. These are **read-only application-calculated facts**; the library displays them. Multiple pieces of evidence for one distinct target are not ambiguous; multiple distinct targets are ambiguous. Ambiguous results must never be represented by EAM as eligible for automatic processing.

`OVMappingRuleEligibility` separates **ConditionMatched**, **AllocationEligible** and **AutomaticProcessingEligible**, so a match is not confused with permission to allocate. Evidence shows the source entity and approved allocation lineage when supplied.

### Condition editor and visual behaviour

Reuse `OVMappingRuleGroupEditor`, `OVMappingRuleCondition`, typed `OVMappingRuleOperatorContract`, `OVMappingJoin`-governed field comparison concepts and existing section styling. Preserve identifiers, disabled conditions, group-level AND/OR, nested groups, read-only editing, validation, compact empty states and parent-container responsiveness (320px, 480px, 720px, 1024px). The host owns page/overlay scrolling, Save and Cancel.

### Migration from 0.1.50

The 0.1.50 Source / Outputs / Target Namespace arrangement is **not** part of the EAM allocation matching composition. Existing condition/group identities and callbacks remain unchanged. The previous presentation can be requested explicitly via `Composition="OVMappingRuleComposition.Legacy"` for a deliberate migration period, but it is not a parallel EAM matching-rule editor and must not be used for new EAM matching screens. Existing consuming screens do **not** automatically change their stored records or domain logic when the library is updated.

EAM adoption must supply Allocation Rule choices, contract snapshots, target comparison field catalogues, validity/revision status and application-executed test results. EAM must validate and enforce permissions independently of any UI state. Application migration, schema and API work, runtime matching and allocation persistence remain out of scope for this library change.

## Verification and publication

Verify solution restore/build/unit tests, browser tests for the three tabs, contract change and invalid states, evidence presentation, responsive nested groups, accessibility, the interactive demo, then package/public gallery publication. Never claim EAM deployment merely because OneVision.UI was released.
