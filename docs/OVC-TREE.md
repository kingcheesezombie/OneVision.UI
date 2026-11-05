# OVC-TREE — hierarchy view, selection and editing

`OVTree` is the existing governed hierarchy control. It supports View and StructureEdit, governed selection modes, node focus separate from selection, subtree search, expansion, node-specific permissions, drag/drop, root/child/sibling actions and supplied action slots. Applications own node IDs, hierarchy queries, edit permissions, move eligibility, persistence and business validation.

## Level 1 contract and state precedence

- Existing initial data and selections render without an extra wrapper.
- `Loading=true` uses an indeterminate status while the first hierarchy is fetched. If no nodes are available, an empty tree is not rendered prematurely. `ErrorMessage` is a fatal initial-fetch error.
- `Refreshing=true` **retains** the prior tree and selected keys. `RefreshErrorMessage` is recoverable and must not clear selection or collapse the tree. These states do not perform network calls.
- Searchability and selection-count visibility are independent from the semantic selection mode and edit/read-only state. The app supplies `SelectedKeys` and `FocusedKey`; selection must remain controlled.
- A disabled/read-only node cannot become a forbidden edit target. Node move, rename and delete are application-authorised through existing semantic callbacks; no generic hierarchy permission engine is introduced.

## Level 2 reference

The synthetic `/quality/workflows` hierarchy scenario demonstrates `OVListPage → OVBuilderPage + OVTree → OVOverlay → return with prior selection → OVOperationalDetail`. The component workbench also exposes search, selection count, read-only, retained-refresh and recoverable failure as **real** `OVTree` props.

Application validation should include large node counts, narrow viewport behaviour, async refresh, drag/drop permission denial, keyboard interaction and preserved ancestor expansion. Do not assume the synthetic tree represents an HDM implementation. See `docs/GOVERNED-QUALITY-COVERAGE-L1-L2.md`.
