# OneVision.UI — all-runtime-component remediation tracker

**Source baseline:** `main` 0.1.33, commit `6c33ae16bf59dff76a30203fdbe785335d07fa7c`, 5 November 2026.
**Scope:** **all 92 `OV*.razor` runtime components**, including internal supporting controls.
**Target:** every applicable L0/L1/L2/L4/L5 gate demonstrably strong enough for **≥4.5/5**; L3 consuming-app adoption verified separately when access is available.

The earlier scorecard is a source-and-test baseline, **not a certificate**. A green build or demo render is insufficient for 4.5/5. Component rows marked "implemented" only identify a proven improvement plus CI; full cross-level verification, 200%/400% zoom, large-data/stress and actual consumer adoption may still be open.

**Review workflow:** source contract → confirmed gap or evidence gap → smallest compatible fix → bUnit and browser/a11y/keyboard → responsive/async/stress → release package/demo commit → consuming-app checks where applicable. Do not mark a row complete without its evidence.

| Runtime component | Classification | Applicable levels | Current implementation evidence | Certification |
|---|---|---|---|---|
| `OVAccordion` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | 0.1.30 · stable controlled expansion; CI evidence | **Not yet 4.5-certified** |
| `OVActionBar` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | 0.1.33 · typed busy actions; PR CI passed | **Not yet 4.5-certified** |
| `OVAlert` | PRIMITIVE | 0, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVAllocation` | COMPOSITE / PATTERN | 0–5 (as relevant) | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVApiCallBuilder` | COMPOSITE / PATTERN | 0–5 (as relevant) | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVApiDecisionStep` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVApiDecisionTrail` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVApiParameterControl` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVApiResponsePreview` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVApplicationShell` | SHELL / PAGE | 0–5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVAutocomplete` | PRIMITIVE | 0, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVBadge` | PRIMITIVE | 0, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVBreadcrumb` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVBuilderPage` | COMPOSITE / PATTERN | 0–5 (as relevant) | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVButton` | PRIMITIVE | 0, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVCard` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVCardCollection` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVCatalogueEditor` | COMPOSITE / PATTERN | 0–5 (as relevant) | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVCatalogueManager` | COMPOSITE / PATTERN | 0–5 (as relevant) | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVCheckbox` | PRIMITIVE | 0, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVConfigurationPreview` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVConfigurationRelationship` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVConfigurationSection` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVConfigurationStep` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVConfirmDialog` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVConnectionEditor` | COMPOSITE / PATTERN | 0–5 (as relevant) | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVContextSummary` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVCopyableLink` | PRIMITIVE | 0, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVDataGrid` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVDatePicker` | PRIMITIVE | 0, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVDateRangePicker` | PRIMITIVE | 0, 2, 4, 5 | 0.1.32 · inclusive date validation; CI evidence | **Not yet 4.5-certified** |
| `OVDateTime` | PRIMITIVE | 0, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVDetailPage` | COMPOSITE / PATTERN | 0–5 (as relevant) | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVDialog` | COMPOSITE / PATTERN | 0–5 (as relevant) | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVDisplayValueBuilder` | COMPOSITE / PATTERN | 0–5 (as relevant) | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVDivider` | PRIMITIVE | 0, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVEmptyState` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVEntityInspector` | COMPOSITE / PATTERN | 0–5 (as relevant) | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVFileUpload` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | 0.1.32 · exact-file removal/read-only; CI evidence | **Not yet 4.5-certified** |
| `OVGuidedLandingPage` | SHELL / PAGE | 0–5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVIcon` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVIconButton` | PRIMITIVE | 0, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVKeyValue` | PRIMITIVE | 0, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVList` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVListPage` | COMPOSITE / PATTERN | 0–5 (as relevant) | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVMappingJoin` | COMPOSITE / PATTERN | 0–5 (as relevant) | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVMappingPair` | COMPOSITE / PATTERN | 0–5 (as relevant) | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVMappingPairSourceTree` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVMappingRule` | COMPOSITE / PATTERN | 0–5 (as relevant) | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVMappingRuleGroupEditor` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVMappingStructureSelector` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVMetadataList` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVMetric` | PRIMITIVE | 0, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVMultiSelect` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVNotification` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVNotificationHost` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | 0.1.31 · remaining-time/cancel-safe expiry; CI evidence | **Not yet 4.5-certified** |
| `OVNumberField` | PRIMITIVE | 0, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVOperationalDetail` | COMPOSITE / PATTERN | 0–5 (as relevant) | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVOverflowMenu` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVOverlay` | COMPOSITE / PATTERN | 0–5 (as relevant) | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVPagination` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVPanel` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | 0.1.33 · typed read-only slots; PR CI passed | **Not yet 4.5-certified** |
| `OVPill` | PRIMITIVE | 0, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVProcessStages` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVProcessStatus` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVProgress` | PRIMITIVE | 0, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVProgressiveConfigurationBuilder` | COMPOSITE / PATTERN | 0–5 (as relevant) | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVRadioGroup` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVScheduleBuilder` | COMPOSITE / PATTERN | 0–5 (as relevant) | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVSearchBox` | PRIMITIVE | 0, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVSelect` | PRIMITIVE | 0, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVSelectionList` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVSelectionSummary` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVSideNav` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVSideNavNode` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVSkeleton` | PRIMITIVE | 0, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVSourceConfiguration` | COMPOSITE / PATTERN | 0–5 (as relevant) | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVSourceDiscovery` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | 0.1.30 · listbox keyboard navigation; CI evidence | **Not yet 4.5-certified** |
| `OVSourceProfile` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVStandardPage` | COMPOSITE / PATTERN | 0–5 (as relevant) | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVStepper` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVStructuredFieldTree` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVTabs` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVTextArea` | PRIMITIVE | 0, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVTextField` | PRIMITIVE | 0, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVTimePicker` | PRIMITIVE | 0, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVTimeline` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVToggle` | PRIMITIVE | 0, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVToolbar` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | 0.1.33 · typed read-only slots + workbench; PR CI passed | **Not yet 4.5-certified** |
| `OVTooltip` | PRIMITIVE | 0, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVTree` | COMPOSITE / PATTERN | 0–5 (as relevant) | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |
| `OVValidationSummary` | COMPOSITE / SUPPORT | 0, 1, 2, 4, 5 | Source-screened; item-specific acceptance evidence pending | **Not yet 4.5-certified** |

## Cross-library blockers

- **L3:** the gallery does not certify real EAM/HDM/KPI/IDM adoption, permissions, API behaviour or OVP-000 application roots.
- **L4:** large hierarchy/data-set, asynchronous cancellation and 200%/400% zoom checks must be added selectively to applicable components.
- **L5:** the implemented gallery's 58 independent reference pages receive a catalogue-wide smoke gate. Embedded runtime children require direct parent composition or dedicated tests.
- **Scoring:** only adjust the baseline after recording actual pass/fail evidence against each applicable level; do not award 4.5 merely for registration, a new parameter, or a published package.

## Implementation waves

- **0.1.30:** Accordion and Source Discovery — published.
- **0.1.31:** Notification Host — published.
- **0.1.32:** Date Range Picker and File Upload — published.
- **0.1.33:** Panel, Toolbar and Action Bar — merged after successful PR testing.
- **Gallery coverage:** restore implemented Pagination/Dialog/Confirm Dialog to the workbench and test every active component route; this is a demo-only change without a library contract-version bump.
