# Complex pattern quality review — November 2026

**Scope:** OVP-SOURCE-CONFIGURATION, OVP-PROGRESSIVE-CONFIGURATION-BUILDER, OVP-CATALOGUE-MANAGEMENT, OVP-API-CALL-BUILDER, OVP-ENTITY-INSPECTOR, OVP-LIST-PAGE.
**Baseline:** OneVision.UI 0.1.23, main commit `9c7f8a45`.
**Purpose:** Improve actual component composition, feedback and capability-workbench exercises without modifying application business rules. This record documents evidence from the current library source and its demos; EAM/IDM/HDM/KPI repository implementations were not freshly inspected in this change.

## Findings and changes

| Pattern | Observed quality weakness | Governed refinement | Consumer impact |
| --- | --- | --- | --- |
| Source Configuration | Save failures were not represented at the pattern level; initial profile could still expose app-supplied Mapping content; optional tab state could be mismatched after a capability toggle | Non-destructive SaveErrorMessage, optional current-profile Mapping gate, embedded overlay ownership, active-tab fallback | Opt-in adoption; app supplies profile readiness, save failure and overlay actions |
| Progressive Configuration Builder | No shared processing, save failure, read-only or completed visual state around an ordered rule; app compositions tended to invent their own feedback | Busy/Error/Completed/ReadOnly states that preserve preview and choices; optional embedded presentation | App still controls steps, dependent resets and save eligibility |
| Catalogue Management | Error had no governed Retry; refresh had no state that retained rows; Add could be duplicated in empty view; editor failure had no non-destructive feedback | RetryRequested, Refreshing, RefreshErrorMessage, one Add affordance, editor SaveErrorMessage | Optional callbacks; app controls data queries and persistence |
| API Call Builder | Generous spacing and large headings obscured workflow; no shared save feedback; builder title/footer could duplicate overlay | Denser hierarchy, embedded overlay ownership, BusyLabel/SaveErrorMessage/SavedMessage, guarded Test/Save/Cancel callbacks | App owns validity/dirty state and overlay footer; request contracts unchanged |
| Entity Inspector | Action-in-progress was not represented to the overlay; action failures had no state retaining loaded detail | Busy/BusyMessage, ActionErrorMessage, disabled governed Refresh/overflow while busy | Consumer action fragments must honour the same Busy and permission state |
| List Page | Results loading/error/empty were left entirely to consumers; no governed refresh-with-retained-results state; static gallery pagination | Initial and transient operational states, Retry action, accessible results label; gallery uses real pagination/filter/action callbacks | Existing result fragments work unchanged unless new states are opted into |

## Architectural decisions

- No new app-specific components or source/target allocation rules.
- OVP-000 is unchanged; these are opt-in OVC/OVP-level capabilities, not changes to application composition roots.
- Synthetic gallery fixtures are deliberately not authoritative data or persistence implementations.
- Keep original interaction and public parameters valid. New capabilities default to off/null, with explicit application ownership.
- A workflow-specific state is only a library contract when its presentation is reusable; domain validation and data retrieval remain with EAM, IDM, HDM and KPI.
- Do not infer real consumer adoption from gallery rendering. Verify app implementation, API semantics and permissions in their own repository threads.

## Release acceptance

- Restore/build the whole solution, execute contract tests and Playwright interaction/accessibility tests.
- Verify known default scenarios, newly controlled operational states and optional capability combinations at desktop and narrow widths.
- Publish the NuGet package and compiled gallery from the merged commit, then confirm its public build marker matches the source commit.
- Record any application adoption issues as reusable OneVision.UI gaps instead of introducing app-local visual conventions.

## Review after adoption

The priority for follow-up application validation is real Source Configuration mapping readiness and Display Value capability combinations, progressive allocation/rule editing, catalogue search/empty/error lifecycle, API request save/test readiness, inspector action lifetimes and large-list server-side pagination. This library release does **not** assert those application behaviours are validated.
