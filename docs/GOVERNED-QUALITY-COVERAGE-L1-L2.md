# OneVision.UI — governed quality coverage, Level 1 and Level 2

**Baseline:** OneVision.UI 0.1.24, merge `706a9dded984cd6212e054762bb55e79fe226d52` on `main`.
**Assessment date:** 5 November 2026.
**Scope:** all eight recommended next-wave areas and a second pass over the six 0.1.24 complex patterns plus OVAllocation.
**Status:** implementation assessment and library/gallery tests; consuming-application DEV validation remains independently required.

## Assessment methodology

**Level 1 — component/pattern quality:** public contract; visual density; required/optional information; empty/loading/read-only/busy/error/success states; keyboard and focus; responsive composition; mutation/validation boundaries; browser regression and accessible labels.

**Level 2 — workflow composition quality:** start at a real list or configured target; move through selection, edit, dependent fields, optional overlay and preview; complete, fail, retry and return; preserve identity/selection/search context; verify only one owner of titles and action footer; do not duplicate app business logic in the library. Reference workflow examples are synthetic **real-component compositions**, not a general screen composer or a substitute for application DEV tests.

Each row below is a review outcome, not an assertion that every consuming app has adopted the latest APIs.

| Quality area | Level 1 findings / bounded remedy | Level 2 reference / coverage | Ownership and outstanding app validation |
| --- | --- | --- | --- |
| **Tree and hierarchy** — `OVTree`, structured trees | Existing selection/edit/search/drags retained; add initial Loading/Error and retained-selection Refreshing/RefreshErrorMessage; expose search/read-only/refreshed variations | `/quality/workflows` Hierarchy: catalogue → selected tree → node editor overlay → return → operational detail | HDM owns hierarchy graph, permissions, mutations and bulk fetch. Real HDM tree-size and drag/drop validation outstanding |
| **Mapping** — Join, Pair, Rule, structured field tree | Preserve field eligibility and stale-join suppression; add non-destructive refresh and error feedback to MappingJoin/Pair. MappingRule, mapping field tree retain established semantics; investigate intermittent short-lived “Loading fields…” test | Source ingestion workflow: connection → current profile/display value → mapping pair → API; component join demo exercises type/definition transitions | IDM/EAM/HDM own metadata fetch, effective mapping cardinality, save and permissions; real transition timing and integrations need application tests |
| **Schedule** — `OVScheduleBuilder` | Existing day/week/month/interval states retained. Optional item-assignment section, busy/save/error/success, modal-footer ownership, guarded mutation during save; capability workbench covers independent switches | Operations workflow: catalogue → progressive rule → schedule → operation; independently verify date-mode and item transitions | Applications own eligibility, recurrence validity, runtime semantics and server execution |
| **Connection** — `OVConnectionEditor` | Existing SQL/LDAP/FILE/JSON and SQL TLS semantics unchanged. Optional Test action, busy and non-destructive save feedback, overlay/footer ownership, guarded test/save | Source workflow entry, then profile/mapping; component workbench exposes optional testing and busy states | Applications own secrets, credentials, network test and persistence; never include operational connection values in public demo |
| **Workspace composition** — `OVP-012`, Detail/Builder pages | Keep established browse→focus→return grammar; add non-destructive Refreshing/RefreshErrorMessage in detail and busy/initial/transient feedback in builder | Hierarchy list→builder→overlay→return preserves selected hierarchy identity. Existing List-to-Workspace example covers unsaved confirmation, sort/filter/paging and review-next | Routing, dirty edits, permissions and API remain consumer-owned; verify IDM/HDM actual navigation on adoption |
| **Operational detail** — `OVOperationalDetail`, `OVProcessStages` | Retain status/health/event/stages/activity; add Busy, Refreshing/RefreshErrorMessage and ActionErrorMessage without wiping prior detail | Hierarchy operational handoff and operations catalogue/rule/schedule→operational detail | Apps own current state, retry/run permissions, event provenance and live polling |
| **Overlay/dialog/action infrastructure** | Existing `OVOverlay`, `OVConfirmDialog`, `OVOverflowMenu` already govern focus, stacking, clipping and dirty confirmation; **no competing control introduced** | Hierarchy node edit and allocation create use real `OVOverlay`; inspection uses `OVEntityInspector`; existing nested-overlay and overflow tests remain regression contracts | Test actual host scroll contexts and browser/mobile viewport in consuming apps |
| **Selection/filter/data navigation** — Search, DataGrid, Pagination, SelectionList, MultiSelect | Existing search/selection semantics retained; add DataGrid retained-record refresh state, Pagination Busy guard, workbench switches for tree and selection. No generic server query engine invented | List/allocated relationships and hierarchy list; prior catalogue/list workbench searches, filters, 25/page and right-aligned actions | Consumers own server-side query/pagination, filtering rules and authoritative selected IDs |
| **OVAllocation** (previous) | Reverify known-target, rule-constrained criteria, selected-source collapse, date toggle, embedded overlay, save state without resetting identity | Dedicated allocation workflow: Entities target → governed selector/summary → Create → target-first grid → inspector | EAM direction, rules and persistence remain EAM-owned; DEV acceptance separately required |
| **OVSourceConfiguration** (previous) | Reverify mapping gate, managed/direct modes, profile-current vs stale, optional Display Value and Standard Details, SaveError | Source workflow: managed connection → current source profile and display value → mapping pair → API builder | EAM/HDM/IDM app mapping semantics not inferred |
| **OVProgressiveConfigurationBuilder** (previous) | Reverify dependent steps, wildcard/conditional, read-only, busy/failure/completion, optional preview/actions | Operations workflow: catalogue → progressive rule → schedule → operation | Applications own rule dependency and validation |
| **OVCatalogueManager/Editor** (previous) | Reverify compact list, search/filters, refresh vs fatal load, editor overlay and non-destructive save failure | Operations workflow first stage; existing catalogue component workbench retains state combinations | No new generic catalogue data engine |
| **OVApiCallBuilder** (previous) | Reverify guided/structured/bulk paths and metadata parameter conditions, generated request, retryable save, embedded mode | Source workflow final request stage after mapping | Application-managed connection/API test and credentials remain app-owned |
| **OVEntityInspector** (previous) | Reverify current identity through refresh/busy/error, optional regions and nested overlay focus | Allocated entity inspection after source selection/create | App owns metadata and action callbacks |
| **OVListPage** (previous) | Reverify initial/error/empty vs retained refresh, filters/search, row overflow, pagination | Hierarchy browse and allocation target-first list; previous List-to-Workspace cookbook | Application owns filter/query/paging API and row composition |

## Contract classification and safety

- `STANDARD`: reusable state presentation, focus, keyboard, consistent action hierarchy, overlays, tree/mapping selection display, loading and errors.
- `APP COMPOSITION`: ordering of list, editor, inspector and operational detail in a given consuming workflow.
- `APP-SPECIFIC`: permissions, source eligibility, entity identities, domain validation, API query/pagination, credentials, persistence and polling.
- `LIBRARY GAP` addressed: non-destructive refresh states, busy submission guards, optional Test/items and controlled workbench capability combinations. Remaining potential gaps require validated consumer evidence.

**OVP-000:** no change to externally observable foundation contract; this work does not advance OVP-000. New component capabilities default to their existing behaviour unless explicitly supplied. The public demo uses synthetic data only.

## Verification and exit criteria

1. Compile the updated OneVision.UI and demo from exact source commit.
2. Run all existing unit/contract and browser/accessibility regressions; include Level 2 workflow journeys (hierarchy edited/returned, allocation created/inspected, source and admin reference navigation).
3. Verify capability combinations at desktop, mid-width and narrow viewport; checks must exercise actual components, not HTML substitutes.
4. Publish NuGet and compiled gallery; verify build SHA matches source commit. Treat GitHub Pages delivery separately from compiled demo repository publication.
5. Keep consumer adoption open until app DEV scenarios demonstrate no broken rules, allocation direction, identity mapping, hierarchy mutation or sensitive configuration exposure.

The gallery workbench is deliberately not an end-user OneVision hub, screen composer or source of app data. Cross-application acceptance must be recorded in the consuming repository.
