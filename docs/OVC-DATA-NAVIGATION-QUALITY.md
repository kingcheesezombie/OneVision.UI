# Search, selection, grid and pagination — quality assessment

Scope: `OVSearchBox`, `OVSelect`, `OVMultiSelect`, `OVSelectionList`, `OVSelectionSummary`, `OVDataGrid` and `OVPagination`.

`OVDataGrid` supports new opt-in `Refreshing`/`RefreshingLabel` and `RefreshErrorMessage` that preserve already loaded table rows, unlike initial `Loading` or `Empty`. `OVPagination.Busy` disables previous/next navigation and guards direct callbacks while a consumer has an in-flight page request. Defaults preserve existing behaviour. No generic table/query engine has been added; applications own server-side paging/filtering, total-count accuracy, eligibility, stable keys and selected IDs.

The component workbench exposes independent retained-refresh states, Busy pagination and multiple/disabled SelectionList composition. `/quality/workflows` exercises a target-first list, governed source selection, and inspection of the result. Existing `OVP-LIST-PAGE` and `OVP-CATALOGUE-MANAGEMENT` contracts continue to govern action hierarchy, 25 rows/page where applicable, right-aligned secondary actions and list-level errors.

The underlying search, selection and multi-select controls were reviewed without introducing a new convention in the absence of a verified interaction gap. Confirm real 100+ row server queries, delayed page changes and input focus at application adoption. See `docs/GOVERNED-QUALITY-COVERAGE-L1-L2.md`.
