# OneVision.UI — Twenty-component quality review, 6 November 2026

**Scope:** `kingcheesezombie/OneVision.UI` source and governed demo only.  
**Baseline:** `main` at `378a8e5` / OneVision.UI 0.1.44.  
**Release candidate:** 0.1.45.  
**Excluded:** API Call Builder family and all consuming application repositories.

This is the next targeted review wave within the original 92-item register. Source,
semantic state and pre-existing component tests were inspected for **all 20** named
components. Changed controls receive explicit new regression tests. Components with
no demonstrated reusable defect are preserved rather than subjected to arbitrary
visual redesign. This report distinguishes code/test evidence from browser and
production integration evidence that cannot safely be assumed.

## Item-by-item assessment

| # | Governed component | Finding and disposition | Acceptance evidence / remaining gate |
|---|---|---|---|
| 01 | `OVMappingPair` | Repeatedly rebuilt mapped-source sets and scanned all mappings per target, which scales poorly; reused section IDs could collide between instances. **Remediated:** indexed source/target mapping lookups; first-match contract retained; instance-stable heading IDs. | 8k mapped-target test and existing Standard Details, read-only, target-search and drag/drop tests. Browser multi-instance ID/accessibility and L3 still need sign-off. |
| 02 | `OVMappingJoin` | Nested fields scanned the entire supplied state list for every node. **Remediated:** construct a field-state lookup once per tree calculation, preserve original first-state wins. | 5k-node field-state stress regression plus existing cardinality/definition-change tests. Browser large joins and L3 pending. |
| 03 | `OVMappingRuleGroupEditor` | A newly selected field could retain an incompatible literal from the preceding field. **Remediated:** clear comparison literal on field identity change without erasing same-field values. | Dependent field invalidation test, existing typed-operator validation; app-owned evaluation unchanged. |
| 04 | `OVMappingRule` | While an old test was outstanding, changing the rule/context prevented a new test until the old finished. **Remediated:** generation-isolated concurrent outstanding tests; stale completion may not unset new running state. | In-flight context-change regression plus existing rule test and result contracts. |
| 05 | `OVMappingStructureSelector` | Dependent type/definition, loading, disabled and no-definition states already governed. **No new source change:** preserve current contract. | Existing `MappingStructureSelectorContractTests` cover clear-on-type-change and loading/invalidated structures; application integration not inferred. |
| 06 | `OVMappingPairSourceTree` | Read-only appearance did not prevent selection/focus events and search relied on placeholder naming. **Remediated:** disabled editing affordances and guarded callbacks; provided explicit accessible search name. | Direct read-only callback tests plus existing source-tree/Pair coverage. Keyboard and zoom follow-up required. |
| 07 | `OVTree` | Selection callbacks did not guard read-only and large multi-selections used repeated linear membership scans. **Remediated** callback guard and selected-key index. | Read-only event and 5k nodes / 2.5k selection tests; existing 10k-depth traversal and tree selection/move tests. Full deep-tree browser stress remains open. |
| 08 | `OVStructuredFieldTree` | Pre-order indexed lookup and state/record-collection invalidation already implemented in 0.1.35. **No source change** in this wave. | `StructuredTreeLookupQualityTests` and `StructuredRecordCollectionContractTests`; large-tree visual keyboard verification outstanding. |
| 09 | `OVSelectionList` | Disabled item already suppresses callbacks, but selected-member lookup was linear per rendered row. **Remediated:** index controlled selected identities per parameter update, preserving fieldset semantics. | Direct disabled-item regression plus pre-existing selection/primitive contracts. |
| 10 | `OVSelectionSummary` | `MaxVisibleItems=0` unexpectedly displayed one item, preventing a compact count-only view. **Remediated:** zero now means no preview labels and displays the omitted count. | Explicit zero-visible case plus default existing summary contract. |
| 11 | `OVMultiSelect` | Disabled selections remained removable through selected chips, and every selected label rescanned the full option catalogue. **Remediated:** protect disabled keys and index option labels once per update. | Rendered chip/callback plus 8k selected-option stress regression; search/filter and read-only retained. |
| 12 | `OVAutocomplete` | Already delegates async search, cancellation token, loading and read-only to the governed MudAutocomplete wrapper. **No source change.** | Public contract matrix and existing component browser smoke; async provider concurrency remains consumer-specific. |
| 13 | `OVSearchBox` | Accessible label, clear, immediate update, disabled/read-only states already present. **No source change.** | Accessible-name/read-only regression and existing `PrimitiveQualityContractTests`. |
| 14 | `OVDataGrid` | Loading/error/empty precedence and conditional Retry (only with handler) already correct. **No source change.** | Error vs loading/empty and inert-retry regression; pagination remains composition-owned. |
| 15 | `OVList` | Disabled rows already suppress activation, yet preserve labels; actions not invented without consumer callback. **No source change.** | Disabled-row regression plus existing list composition tests. |
| 16 | `OVListPage` | Fatal error coexisted with refresh feedback, creating contradictory progress/alert states. **Remediated:** fatal error takes precedence while preserving the existing page content and toolbar. | Fatal/refresh precedence test plus existing list-page gallery and browser coverage. |
| 17 | `OVPagination` | Button disabled state used raw out-of-range page numbers while rendering a clamped page; negative count was displayed verbatim. **Remediated:** use effective displayed bounds; clamp negative item totals for presentation only. | Clamped-page and count regression plus `PaginationContractTests`. Application owns page changes. |
| 18 | `OVFileUpload` | Per-file duplicate-name checks rescanned all selected files; identity-safe callback already existed. **Remediated:** index filename counts per parameters update, retaining exact-file removal. | Duplicate file test plus existing `DateRangeAndUploadContractTests`. Browser upload permissions/storage remain app-owned. |
| 19 | `OVConnectionEditor` | SQL/FILE/LDAP/JSON source fields, transport test state, read-only and password masking already governed. **No source change.** | Existing `ConnectionEditorTlsContractTests`, `ConnectionAndTreeContractTests`, gallery/browser states; security testing of actual backends is outside scope. |
| 20 | `OVSourceDiscovery` | Candidate keyboard listbox, explicit preview selection, loading/empty/disabled states already implemented in earlier release. **No source change.** | Existing `InteractiveControlQualityContractTests` and gallery/browser states; production-scale candidate lists remain an L4 gate. |

## Governed behaviour / scope control

- Existing OVC/OVP contracts and app-owned eligibility, mapping, persistence,
  validation, connections and authorization are unchanged, except for the
  explicitly refined presentation/selection behaviours listed above.
- Input and output semantics were preserved; no blanket UI redesign, conversion
  to new controls or invented conventions.
- Changes are local to components with observed issues. Ten thousand-row/depth
  tests from earlier releases remain part of the full suite.
- No published demo source SHA may be claimed until the merged commit is
  observed in `kingcheesezombie/OneVision.UI.Demo`.

## Quality gates and scoring rule

**Assessment:** all 20 source implementations, existing tests and public parameter
contracts inspected. The `TwentyComponentReviewContractTests` class enumerates
all 20 explicitly; further focused tests cover actual changed behaviour. This is
stronger than filename-only screening but not equivalent to production UX proof.

**Quality certification:** no row should be labelled 4.5/5 certified solely from
these source tests. Per-component L0–L5 certification requires relevant rendered
state combinations, keyboard/assistive-technology accessibility, responsive and
zoom behaviour, stress testing where material, and evidence for any real
consuming-application integration if within authorised scope. Unavailable L3 and
visual evidence must be recorded as an open gate rather than fabricated.

**Release checks (to complete):** PR CI build/unit tests; browser interaction
and axe suite; merge to `main`; verify demo repo commit references the exact
merged source SHA; separately verify GitHub Pages runtime if available. Any
introduced test failure must be resolved before merging.

**Next review:** continue the remaining 72 tracked items only after this wave
is validated. The API Call Builder family stays deferred by explicit instruction.
