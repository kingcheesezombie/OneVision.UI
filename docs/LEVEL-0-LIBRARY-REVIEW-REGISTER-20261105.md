# OneVision.UI — Complete component quality review register

**Baseline:** OneVision.UI 0.1.25, `main` commit `2f7a480a7e97c54ec236b47786f0013ed3d644b4` (5 November 2026).
**Scope:** all 92 actual `OV*.razor` components in authoritative library source. Each of the 92 source files was inspected in the code-level screening pass; this is not a substitute for rendered interaction testing.
**This document is a work register, not a claim that every component has passed review.** Previous L1/L2 work covered selected high-level patterns. This first L0 tranche implements confirmed gaps in four foundational components and replaces the Connection Manager mock. Detailed visual, accessibility, robustness and consuming-application sign-offs remain open where not backed by specific tests.

## Uniform review contract

For **each item**, assess every applicable level with recorded evidence:

- **L0** primitive semantics, compact visual quality, accessibility, keyboard, focus, disabled/read-only, validation, long values, responsive and embedding.
- **L1** component/coherent composite interaction, public semantic API, loading/error/success, save and retry, state precedence and capabilities.
- **L2** real reusable workflow composition (selection → edit → save/error → return) without app business logic or double title/action ownership.
- **L3** actual consuming application integrations and OVP-000 foundation compliance where repository/DEV access permits.
- **L4** asynchronous cancellation, stale response protection, large data, keyboard/zoom/motion and non-destructive resilience.
- **L5** contract and browser regression evidence, version/API compatibility, package/gallery publishing and public SHA correlation.

A build passes the **release gate**, not the whole quality programme. App behaviours cannot be claimed validated from gallery fixtures. Small primitives need not be assigned fictitious API/load tests.

## Register — 92 source components

| Implementation | Classification | Relevant quality levels | Current evidence / sign-off status |
|---|---|---|---|
| `OVAccordion` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVActionBar` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVAlert` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVAllocation` | COMPOSITE / PATTERN | 0–5 (as relevant) | Registered; detailed L0–5 sign-off pending |
| `OVApiCallBuilder` | COMPOSITE / PATTERN | 0–5 (as relevant) | Registered; detailed L0–5 sign-off pending |
| `OVApiDecisionStep` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVApiDecisionTrail` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVApiParameterControl` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVApiResponsePreview` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | L0 accessibility remediation and regression evidence in 0.1.28; L3/L4 app/scale validation pending |
| `OVApplicationShell` | SHELL / PAGE | 0–5 | Registered; detailed L0–5 sign-off pending |
| `OVAutocomplete` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVBadge` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVBreadcrumb` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVBuilderPage` | COMPOSITE / PATTERN | 0–5 (as relevant) | Registered; detailed L0–5 sign-off pending |
| `OVButton` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVCard` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVCardCollection` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVCatalogueEditor` | COMPOSITE / PATTERN | 0–5 (as relevant) | Registered; detailed L0–5 sign-off pending |
| `OVCatalogueManager` | COMPOSITE / PATTERN | 0–5 (as relevant) | Registered; detailed L0–5 sign-off pending |
| `OVCheckbox` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVConfigurationPreview` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVConfigurationRelationship` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVConfigurationSection` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVConfigurationStep` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVConfirmDialog` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVConnectionEditor` | COMPOSITE / PATTERN | 0–5 (as relevant) | Changed; regression tests in PR #39 |
| `OVContextSummary` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVCopyableLink` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVDataGrid` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Level 0/1 remediation and targeted tests in 0.1.27; remaining L3/L4 visual and consumer validation pending |
| `OVDatePicker` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVDateRangePicker` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVDateTime` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVDetailPage` | COMPOSITE / PATTERN | 0–5 (as relevant) | Registered; detailed L0–5 sign-off pending |
| `OVDialog` | COMPOSITE / PATTERN | 0–5 (as relevant) | Registered; detailed L0–5 sign-off pending |
| `OVDisplayValueBuilder` | COMPOSITE / PATTERN | 0–5 (as relevant) | Registered; detailed L0–5 sign-off pending |
| `OVDivider` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVEmptyState` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVEntityInspector` | COMPOSITE / PATTERN | 0–5 (as relevant) | Registered; detailed L0–5 sign-off pending |
| `OVFileUpload` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Changed; regression tests in PR #39 |
| `OVGuidedLandingPage` | SHELL / PAGE | 0–5 | Registered; detailed L0–5 sign-off pending |
| `OVIcon` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVIconButton` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVKeyValue` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVList` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVListPage` | COMPOSITE / PATTERN | 0–5 (as relevant) | Registered; detailed L0–5 sign-off pending |
| `OVMappingJoin` | COMPOSITE / PATTERN | 0–5 (as relevant) | Registered; detailed L0–5 sign-off pending |
| `OVMappingPair` | COMPOSITE / PATTERN | 0–5 (as relevant) | Registered; detailed L0–5 sign-off pending |
| `OVMappingPairSourceTree` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVMappingRule` | COMPOSITE / PATTERN | 0–5 (as relevant) | Registered; detailed L0–5 sign-off pending |
| `OVMappingRuleGroupEditor` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVMappingStructureSelector` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVMetadataList` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVMetric` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVMultiSelect` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Level 0/1 remediation and targeted tests in 0.1.27; remaining L3/L4 visual and consumer validation pending |
| `OVNotification` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVNotificationHost` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | L0 accessibility remediation and regression evidence in 0.1.28; L3/L4 app/scale validation pending |
| `OVNumberField` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVOperationalDetail` | COMPOSITE / PATTERN | 0–5 (as relevant) | Registered; detailed L0–5 sign-off pending |
| `OVOverflowMenu` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVOverlay` | COMPOSITE / PATTERN | 0–5 (as relevant) | Registered; detailed L0–5 sign-off pending |
| `OVPagination` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVPanel` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVPill` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVProcessStages` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | L0 accessibility remediation and regression evidence in 0.1.28; L3/L4 app/scale validation pending |
| `OVProcessStatus` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVProgress` | PRIMITIVE | 0, 2, 4, 5 | Level 0/1 remediation and targeted tests in 0.1.27; remaining L3/L4 visual and consumer validation pending |
| `OVProgressiveConfigurationBuilder` | COMPOSITE / PATTERN | 0–5 (as relevant) | Registered; detailed L0–5 sign-off pending |
| `OVRadioGroup` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Changed; regression tests in PR #39 |
| `OVScheduleBuilder` | COMPOSITE / PATTERN | 0–5 (as relevant) | Registered; detailed L0–5 sign-off pending |
| `OVSearchBox` | PRIMITIVE | 0, 2, 4, 5 | Changed; regression tests in PR #39 |
| `OVSelect` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVSelectionList` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | L0 accessibility remediation and regression evidence in 0.1.28; L3/L4 app/scale validation pending |
| `OVSelectionSummary` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Level 0/1 remediation and targeted tests in 0.1.27; remaining L3/L4 visual and consumer validation pending |
| `OVSideNav` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVSideNavNode` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVSkeleton` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVSourceConfiguration` | COMPOSITE / PATTERN | 0–5 (as relevant) | Registered; detailed L0–5 sign-off pending |
| `OVSourceDiscovery` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVSourceProfile` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVStandardPage` | COMPOSITE / PATTERN | 0–5 (as relevant) | Registered; detailed L0–5 sign-off pending |
| `OVStepper` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVStructuredFieldTree` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVTabs` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Changed; disabled/missing active key fallback regression in PR #39 |
| `OVTextArea` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVTextField` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVTimePicker` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVTimeline` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | L0 accessibility remediation and regression evidence in 0.1.28; L3/L4 app/scale validation pending |
| `OVToggle` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVToolbar` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVTooltip` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVTree` | COMPOSITE / PATTERN | 0–5 (as relevant) | Registered; detailed L0–5 sign-off pending |
| `OVValidationSummary` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |

## Confirmed issues and remediation in 0.1.26 proposal

- `OVSearchBox`: a placeholder is not an accessible name. New optional `AriaLabel` with safe default.
- `OVRadioGroup`: group lacked an explicitly governed accessible name. New optional `AriaLabel` with safe default.
- `OVFileUpload`: selection beyond `MaximumFileCount` can throw; invalid files could disappear without explanation. Handle limits, report validation errors and label the native input while retaining previous selection on total rejection.
- `OVConnectionEditor`: embedded form lost Test Connection when delegating Save/Cancel to `OVOverlay`. The embedded test action now remains opt-in through `ShowTestAction`.
- **Connection Manager:** an obsolete bespoke demo implemented non-governed SQL/LDAP/JSON settings and inert buttons. `OVP-CONNECTION-MANAGEMENT` now composes the actual governed list, editor and overlay with synthetic Add/Edit/Test/Retry states; no new competing app-specific control.

## Remaining quality tasks

Detailed L0 review of each unmodified primitive, L1 review of composites, L2 workflow review, L3 application integration and L4 stress and concurrency review remain open until tested with evidence. High-value candidates are input error states; selection and focus retention; date-range validation; large tree/field mapping; notification queues; virtualised list performance; File Upload browser event edge cases; credentials handling, and app-specific connection test concurrency. These require dedicated review, not automatic changes without evidence.

**OVP-000:** unchanged; no new composition root conventions. **Security:** public fixtures use only synthetic endpoints and contain no secrets.


## 0.1.27 continued review — confirmed issues, scoped remediation and evidence

The prior 92-component source screen remains the coverage baseline, **not an all-level certification**. A targeted follow-up of selection, data navigation, progress, display, operation and workspace families identified the following reusable issues and demo accuracy gaps:

| Component/pattern | Review finding | Remediation | Evidence |
|---|---|---|---|
| `OVDataGrid` | Initial failures had no dedicated, application-owned error/retry state; refresh-error could conflict with initial error | Add `ErrorMessage` and `RetryRequested`, enforce initial-error precedence, retain rows for refresh errors | bUnit and rendered gallery browser scenario |
| `OVMultiSelect` | Selecting while searching could drop selections hidden by the current filter, especially with grouped/disabled values | Preserve filtered-out and disabled selections; prevent changes in disabled/read-only; reuse accessible `OVSearchBox` | Selection callback unit tests |
| `OVSelectionSummary` | Selection count was not announced as a live update | Announce count alone, not action buttons | bUnit count and ARIA test |
| `OVProgress` | Invalid/non-finite percentages could reach the rendering primitive | Clamp to 0–100; non-finite maps to zero | Unit range tests |
| Gallery `OVC-DATA-GRID` | Selected-row checkboxes and overflow actions were inert, obscuring combined states | Use working synthetic selection, Clear, inspect menu, load error/retry, retained refresh failure | Browser interaction tests |
| Gallery `OVC-TABS` | Overflow example contained only three tabs | Render twelve peer tabs in overflow scenario and describe their behaviour accurately | Visual review remains pending |
| Gallery data-grid documentation | Claimed that a MudDataGrid/MudTable implementation was selected automatically although `OVDataGrid` is a semantic table shell | Correct implementation basis and ownership boundary | Source contract inspection |

### Further L0–L5 review outcomes

- **Unchanged after source review:** alerts, text/number/select inputs, time/date inputs, pill/badge, shells, modal basics, schedules, mapping controls, process stages and existing source-configuration workflows remain subject to existing contract tests. No silent redesigns were introduced without a documented finding.
- **Level 2:** data-grid error/retry and selected-row actions can now be demonstrated in the gallery; the Data Grid still accepts application-defined `thead`/`tbody` rather than inventing a second selection architecture.
- **Level 3:** no claim is made about actual EAM/HDM/KPI/IDM screen adoption; consumer repositories and DEV workflow validation are separate quality gates.
- **Level 4:** selection persistence is tested; true large-data, accessibility-at-zoom and application async-race testing require consuming-app fixtures.
- **Level 5:** the 0.1.27 release must build the same committed sources, pass the full contract/browser suite, publish NuGet and compiled public gallery, and verify the build SHA. **OVP-000 remains unchanged.**

Remaining defects must be found by evidence-based visual, keyboard, scaling or integration testing. Do not treat a successful CI run as proof of comprehensive L0–L5 sign-off.


## 0.1.28 focused accessibility follow-up

A code-level audit of notifications, selection, process timelines and API result previews identified five unambiguous semantic gaps; the changes preserve the existing component arrangement, callbacks, visual contracts and app-owned business logic:

- `OVTimeline` and `OVProcessStages`: previously supplied `aria-label` on a generic `div` without an explicit role. These now expose named groups with their existing ordered-list semantics intact.
- `OVNotificationHost`: exposes one named Notifications region around individually announced status/alert entries, without changing transient-queue or timeout behaviour.
- `OVSelectionList`: its optional `Label` could leave a `fieldset` without an accessible name. A semantic `AriaLabel` fallback names only the unlabelled case; an explicit `legend` remains authoritative where supplied.
- `OVApiResponsePreview`: the visible response tab buttons and single dynamic panel now have stable, instance-local `id`, `aria-controls` and `aria-labelledby` relationships. The preview's rendering, tab choice and data/structure/raw content are unchanged.

**Evidence:** the added bUnit tests assert named groups, selection-list legend fallback, a named notification region, and correct response tab-panel associations. Further keyboard-arrow tab interaction, zoom/accessibility stress, notification-burst performance and consumer-app screen validation remain separate Level 3/4 gates; these claims are not implied by passing contract tests.

**Foundation:** OVP-000 unchanged. **Version:** OneVision.UI 0.1.28 is an additive accessibility refinement; source/package/demo publication and SHA verification are required before calling it released.


## 0.1.30 first interaction-quality remediation batch

This is an evidence-driven implementation batch toward the L0–L5 **4.5/5 target**, not a retrospective change to the provisional ratings. Existing component consumers are preserved.

### OVC-ACCORDION
- Existing `OVAccordionItem(Title, Content, Expanded, Disabled)` calls remain valid. Optional `Key` allows stable identity under reorder/insert/remove.
- `ExpandedKeys = null` uses uncontrolled initial `item.Expanded` state, retaining user changes across ordinary parent rerenders; stale keys are removed when items disappear.
- Non-null `ExpandedKeys` is fully parent-controlled; `ExpandedKeysChanged` reports explicit key changes without overwriting the parent's supplied state.
- `AllowMultiple` is opt-in; default remains a single expanded section. Disabled items/global disabled block edits.
- The gallery shows collapsed, initially expanded, controlled, multi-open and disabled states with real callbacks and rerender verification.

### OVC-SOURCE-DISCOVERY
- Retains the existing listbox/option semantics, source-preview versus committed-selection distinction and callback API.
- Exactly one candidate is tabbable; ArrowUp, ArrowDown, Home and End change preview and move focus. Enter/Space remain native button activation.
- A visible keyboard hint is associated with the listbox using `aria-describedby`; selection remains a separate deliberate action.
- Candidate identity is keyed through rerenders to avoid spurious focus loss; disabled and read-only states suppress interaction.

### Acceptance evidence and score policy
Dedicated bUnit contract checks and Playwright browser tests are required, in addition to existing source-profile and accessibility regressions. The actual main build, package and compiled gallery SHA must match. Item scores must be recalculated only after CI/browser evidence; L3 application adoption and 200/400% zoom remain separately unverified. No foundation version change is required.
