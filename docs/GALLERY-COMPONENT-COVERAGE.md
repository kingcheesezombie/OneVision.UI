# OneVision.UI gallery — complete implemented-component coverage gate

## Scope and ownership

The authoritative source catalogue comprises **92 `OV*.razor` runtime components**. The public gallery exposes user-meaningful OVC controls individually and embeds internal/helper controls in their owning parent OVC/OVP examples; a library child component is not automatically a standalone user-facing pattern.

The source catalogue and public gallery are different inventories. Component quality reviews must account for all runtime source files. Standalone gallery coverage must not be misrepresented as exhaustive source-level coverage, and a green gallery smoke test is not complete L0–L5 certification.

## Corrected active manifest

`ImplementedComponentCatalog` must exclude only references without a real implementation or representative demo. `OVC-SORTABLE-HEADER` remains concept-only.

`OVC-PAGINATION`, `OVC-DIALOG` and `OVC-CONFIRM-DIALOG` are implemented `OVPagination`, `OVDialog` and `OVConfirmDialog` runtime components with real demos, and must be visible in the active gallery. Hiding them under the concept-only filter previously prevented component-state inspection.

The active manifest has 58 independently displayed reference definitions (41 base + 17 governed additions + 1 connection editor – 1 unsupported concept). Internal supporting components continue to be exercised in the actual parent demonstrations.

## Regression guard

`AllComponentGalleryBrowserTests` obtains the actual catalogue entries from `/components` and opens each resulting route, checking that it renders a real component preview rather than a missing-implementation/placeholder message. It also verifies restored Pagination, Dialog and Confirm Dialog entries and uniqueness of their slugs.

This is a breadth **L0/L5** check, not a substitute for each component's deep interaction tests, 200%/400% zoom, large-record stress, keyboard/a11y scenarios, or L3 application adoption. Those remain separately recorded against the 92-item quality register. New independently demoable OVCs must appear automatically in the catalogue sweep once added to the manifest.

## Foundation and privacy

No OVP-000 change. Gallery fixtures remain synthetic; internal/application data, secrets and domain decisions are never introduced into the public demo.
