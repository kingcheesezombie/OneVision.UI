# OVP-LIST-PAGE — governed administrative list

**Implementation:** `OVListPage` with application-supplied governed filters, results and actions.
**Foundation impact:** None; OVP-000 remains unchanged.

## Purpose and composition

One consistent target-first list/page grammar across OneVision applications: page title and optional description/actions, optional summary, search/filters and toolbar actions, results region, and pagination. The consuming application owns its entity columns, filter semantics, query/pagination API and row operations. Use `OVDataGrid`, `OVPagination`, `OVOverflowMenu` and existing OVC controls as appropriate. Standard row actions are right-aligned and secondary/destructive actions go in overflow.

## Controlled operational states

The default preserves the caller-supplied results unchanged. The optional parameters `Loading`, `LoadingLabel`, `ErrorMessage`, `RetryRequested`, `Empty`, `EmptyTitle`, `EmptyDescription`, `Refreshing`, `RefreshingLabel`, `RefreshErrorMessage` and `ResultsLabel` govern state presentation.

- Initial `Loading` replaces only the results region and suppresses pagination; the page title, search and filters remain available.
- Fatal `ErrorMessage` replaces results with a concise error and optional Retry action if `RetryRequested` is provided.
- `Empty` renders a governed `OVEmptyState`, distinct from a filtered-results list where a consumer can choose more specific content. Pagination is suppressed while empty.
- `Refreshing` does not hide previously loaded records; it shows compact progress above the results and retains pagination.
- `RefreshErrorMessage` presents recoverable feedback above retained results, not a blank result set.
- `ResultsLabel` provides an accessible name for the results region. Feedback has appropriate status/alert semantics.

The application is responsible for clearing or retaining cached results, for search/filter state and pagination, for permissions and for enabling mutation actions. Do not silently invent disabled or read-only policies for app-supplied fragments.

## Layout and quality

Use compact administrative density and a single hierarchy: header → toolbar → results → pagination. Hide optional regions entirely when their fragment is null; do not emit inert decorative controls. Search and filter controls wrap at narrow widths, result tables scroll internally if needed, identifiers wrap when practical and action placement remains predictable. Application compositions should avoid a redundant second pagination control or duplicate Add buttons.

The gallery renders `OVListPage` itself with working search, status filtering, Add, row overflow and governed `OVPagination` (25 rows/page), plus initial loading, fatal error/Retry, empty, refreshing, recoverable refresh error and read-only states. These are synthetic fixtures; they do not implement application persistence.

## Regression scenarios

Confirm search resets pagination, page navigation changes visible records, filtering changes count, action affordances disappear in a read-only consumer, loading/error/empty suppress results correctly and refreshing/refresh failure preserve the existing list. Test responsive widths of approximately 1280px, 760px and 390px. Verify that filter capability combinations render real behaviour and do not create empty action/summary containers.
## Level 2 workflow recheck — 0.1.25

The workflow-quality gallery uses the list page as the starting point of both hierarchy selection and known-target allocation. It verifies selection/identity continuity after leaving and returning to a workflow, while the existing List Page workbench still tests search, pagination, filters, error/Retry and read-only actions. Applications remain responsible for server-side pagination and navigation state.
