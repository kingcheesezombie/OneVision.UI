# OneVision.UI — Complete component quality review register

**Baseline:** OneVision.UI 0.1.25, `main` commit `2f7a480a7e97c54ec236b47786f0013ed3d644b4` (5 November 2026).
**Scope:** all 92 actual `OV*.razor` components in authoritative library source, from the repository Git tree.
**This document is a work register, not a claim that every component has passed review.** Previous L1/L2 work covered selected high-level patterns. This first L0 tranche implements confirmed gaps in four foundational components and replaces the Connection Manager mock. Detailed visual, accessibility, robustness and consuming-application sign-offs remain open for the remaining components.

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
| `OVApiResponsePreview` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
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
| `OVDataGrid` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
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
| `OVMultiSelect` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVNotification` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVNotificationHost` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVNumberField` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVOperationalDetail` | COMPOSITE / PATTERN | 0–5 (as relevant) | Registered; detailed L0–5 sign-off pending |
| `OVOverflowMenu` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVOverlay` | COMPOSITE / PATTERN | 0–5 (as relevant) | Registered; detailed L0–5 sign-off pending |
| `OVPagination` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVPanel` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVPill` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVProcessStages` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVProcessStatus` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVProgress` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVProgressiveConfigurationBuilder` | COMPOSITE / PATTERN | 0–5 (as relevant) | Registered; detailed L0–5 sign-off pending |
| `OVRadioGroup` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Changed; regression tests in PR #39 |
| `OVScheduleBuilder` | COMPOSITE / PATTERN | 0–5 (as relevant) | Registered; detailed L0–5 sign-off pending |
| `OVSearchBox` | PRIMITIVE | 0, 2, 4, 5 | Changed; regression tests in PR #39 |
| `OVSelect` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVSelectionList` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVSelectionSummary` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVSideNav` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVSideNavNode` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVSkeleton` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVSourceConfiguration` | COMPOSITE / PATTERN | 0–5 (as relevant) | Registered; detailed L0–5 sign-off pending |
| `OVSourceDiscovery` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVSourceProfile` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVStandardPage` | COMPOSITE / PATTERN | 0–5 (as relevant) | Registered; detailed L0–5 sign-off pending |
| `OVStepper` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVStructuredFieldTree` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVTabs` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVTextArea` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVTextField` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVTimePicker` | PRIMITIVE | 0, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
| `OVTimeline` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Registered; detailed L0–5 sign-off pending |
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
