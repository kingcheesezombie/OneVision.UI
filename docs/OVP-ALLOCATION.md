# OVP-ALLOCATION — effective-dated allocation

**Implementation:** `OVAllocation`  
**Status:** Draft — governed reusable composition  
**Foundation impact:** None; the OVP-000 contract remains unchanged.

## Purpose

Create a source-to-target allocation in a compact, single-column, target-first workflow. The pattern governs composition and state presentation; consuming applications own entity data, eligibility, validation, permissions, and persistence.

## Composition

1. **Target entity:** a compact, read-only identity row for a known target. In finder mode, show the available target platforms only when more than one is permitted, the application-supplied Target Type when present, search and results. After selection, collapse to the same read-only identity row and optionally offer Change.
2. **Source entity:** optional source system/type criteria, a prominent search input, and results. Once selected, hide the source criteria, search and result rows and show the selected-source identity with an optional Change action. The consumer retains and invalidates controlled values.
3. **Effective dates:** Always belonged defaults on and removes date controls from the layout. Turning it off reveals required From and optional To fields. Consuming apps own validity of actual date ranges and date semantics.
4. **Validation and actions:** application-supplied validation summary and action footer. Keep operational UI free of technical implementation notices.

The layout is bounded (640px maximum), responsive and uses existing foundation tokens and governed OVC primitives. Do not wrap each region in a separate decorative card or introduce a wizard/tab interface.

## Semantic parameters

| Parameter | Meaning |
| --- | --- |
| `FindTarget` | Target search mode instead of already-known target |
| `TargetSelected`, `SelectedTarget` | Finder has an application-selected target identity |
| `AvailableTargetPlatforms` | Application-permitted subset of EAM, ODM, IDM; null preserves the default list |
| `ChangeTargetRequested` | Optional consumer callback to resume target selection |
| `Target` | App-supplied read-only target identity for known-target mode |
| `TargetType` | Optional app-owned Target Type fragment for **any** Provider, shown only in finder mode before target selection; omit entirely when not supplied |
| `TargetSearch`, `TargetResults` | Optional app-owned target finder and search results |
| `TargetPlatform`, `TargetPlatformChanged` | Controlled Provider selection and existing change callback; no library-owned target-type catalogue |
| `SourceSystem`, `SourceType` | Optional editable selector regions retained for existing consumers |
| `SourceCriteria` | Ordered application-owned criteria fragment, overriding the legacy two-field presentation |
| `FixedSourceSystem`, `FixedSourceType` | Read-only values when the application already knows an immutable choice; override their corresponding editor fragments |
| `SourceSearch`, `SourceResults` | Source search and eligible application-supplied results |
| `SourceSelected`, `SelectedSource` | App signals a completed choice; show selected identity instead of the result list |
| `ChangeSourceRequested` | Optional callback to resume source selection |
| `AlwaysBelonged`, `FromDate`, `ToDate` and callbacks | Governed conditional date editor |
| `SearchLoading`, `NoSourceResults`, `SearchErrorMessage` | Search-result state without losing target context |
| `EligibilityUnavailable`, `EligibilityUnavailableMessage` | Nonfatal eligibility warning |
| `Submitting`, `SubmissionErrorMessage`, `Completed`, `CompletedMessage` | Submission lifecycle; identity remains visible |
| `EmbeddedInOverlay` | Omit pattern heading and footer; `OVOverlay` owns them |
| `ReadOnly`, `Loading`, `ErrorMessage`, `ValidationSummary`, `Actions` | Governed interaction/feedback slots and app-supplied operations |

Source classification and source entity type are distinct. EAM's ATTRIBUTE classification is application-owned and may be fixed while the entity type remains selectable from rule-constrained values. `FixedSourceType` is for a genuinely fixed entity type, not a classification. `SourceCriteria` supports an arbitrary application-supplied set of ordered dependent criteria.

## Application ownership

- EAM may supply a fixed target platform, a known target entity, Attribute source classification, and source/type lists constrained by its allocation rules.
- Any consumer may enable target finder, one fixed or multiple selectable Target Providers, optional Target Type selection, additional source types, and explicit dates.
- The library does **not** enforce target/source eligibility, cardinality, overlapping dates, rule permissions, lifecycle semantics, or call an allocation API.
- Consuming apps remain responsible for disabled, loading and read-only states inside app-supplied Target Type, finder and source fragments; they must clear/update the controlled selection state after Change and invalidate dependent Target Type and target identity selections when Provider changes.
- No inferred EAM/ODM resolution or hierarchy behaviour belongs in this pattern.

## Gallery validation

The gallery's **Allocation** workbench exposes known target, finder, EAM Target Type, IDM PERSON, IDM COMPETENCY_ITEM, ODM without Target Type and a generic multi-provider finder; independent Target Type, loading/empty/disabled/read-only, selected states and explicit dates can be combined. Verify both scenarios at desktop and compact widths, with the source result list and selected summary, and with/without dates.


## Provider-independent Target Type — 0.1.36

`OVAllocation` exposes the existing optional `TargetType` `RenderFragment` in the **target-finder, not-yet-selected** state, regardless of whether `TargetPlatform` is `Eam`, `Idm` or `Odm`. There is no provider-specific visibility condition, no provider-specific target-type list and no new target-selection pattern.

- **Known target** (`FindTarget=false`): render the application-supplied `Target` identity only; the consumer may include Provider, Type, identifier and display name. Do not render editable Provider or Type selectors.
- **Find target** (`FindTarget=true`, `TargetSelected=false`): render the Provider choice only when `AvailableTargetPlatforms` has more than one permitted Provider (or when null defaults to the existing EAM/ODM/IDM list). Render the `TargetType` fragment when supplied, followed by optional `TargetSearch` and `TargetResults`. If the consumer passes `TargetType=null`, omit its complete field, label and spacing.
- **Selected finder target** (`FindTarget=true`, `TargetSelected=true`): collapse selectors and results into the supplied `SelectedTarget` identity, which may include Provider and Type. When `ChangeTargetRequested` is supplied and the pattern is interactive, show Change to resume selection.
- **Fixed Provider**: `AvailableTargetPlatforms=[selectedProvider]` hides the Provider choice. This is independent of whether `TargetType` is supplied.
- **Dynamic types**: the application resolves its type options according to the selected Provider and renders an updated `TargetType` fragment. Changing Provider must invalidate the application's selected Target Type, target identifier, search and any other dependent values **before** showing new options. The library only raises `TargetPlatformChanged` and re-renders supplied parameters; it does not silently apply domain eligibility, Type or identity rules.
- **Optional states**: applications may supply `OVSelect` with their own options, `Disabled`, `ReadOnly`, `Error`/`ErrorText`, `OVProgress` for loading or `OVEmptyState` for no options. When `OVAllocation.ReadOnly`, `Submitting` or `Completed` applies, its built-in Provider and Change actions are already blocked; consuming apps must apply the same state to their supplied selectors/results.
- **Responsiveness**: Target Type uses the existing `ov-allocation__field` grid, fluid nested controls and the established single-column/max-width layout, without a provider-specific wrapper.

### Example consumer-owned dependency flow

A consuming application supplies `TargetPlatform`, `TargetPlatformChanged`, `AvailableTargetPlatforms` and, when relevant, `TargetType`. In its Provider change callback it stores the new Provider, clears Target Type and selected target identity, resolves permitted types from its own catalogue, and passes the resulting selector/options into `TargetType`. For a Provider without a type requirement it supplies no fragment. This is **an application implementation responsibility**, not library behaviour.

The gallery uses synthetic examples of `EAM: Organisation Unit`, `IDM: PERSON` and `IDM: COMPETENCY_ITEM`, and optional `ODM` types solely to exercise this reusable presentation contract. Those labels are not added as platform identifiers or business rules to OneVision.UI.

### Compatibility and foundation impact

The public `OVAllocation` parameters, `OVAllocationTargetPlatform` enum, target-platform change callback, known-target presentation, search/results, source selection, effective-date controls, `OVOverlay` composition and submission lifecycle are unchanged. Existing consumers that omit `TargetType` still show no Target Type; EAM consumers supplying it retain the same presentation. **The only intentional behaviour change is that a supplied Target Type is no longer suppressed for IDM or ODM.** This is a refinement of `OVP-ALLOCATION`, not a new OVC/OVP, and **OVP-000 remains v1.5**.

## List presentation

Use `OVP-LIST-PAGE` with a governed grid for target-first allocation lists. `OVAllocation` owns the editor, not list management. For a single known target, display its identity once above its allocated source rows.

## Overlay composition and operational states

When rendering within `OVOverlay`, set `EmbeddedInOverlay=true`, supplying `Title`, `Description`, `SaveText`, `SaveDisabled`, `Busy`, `SaveRequested` and `OpenChanged` on the overlay. Do not render a second action footer inside the allocation pattern. The overlay owns scrolling, escape/backdrop handling, focus management and save/cancel placement. The allocation component still renders validation feedback in its body.

`Loading` is intended for the first fetch of the entire form. Use `SearchLoading`, `NoSourceResults`, `SearchErrorMessage` and `EligibilityUnavailable` for localised source-search states, retaining the known target. Use `Submitting`, `SubmissionErrorMessage` and `Completed` for submit states; selected identities persist. Errors must not destroy the in-progress allocation.

Consuming applications own server-side filtering/pagination, rule-constrained choices, invalidating downstream source selections, read-only/disabled behaviour within supplied fragments, permissions and save validity. No EAM-specific classifications, rule identifiers or API calls exist in `OVAllocation`. The generic `SourceCriteria` region allows any number of criteria; it does not implement a rule engine.

The gallery supplies a rule-constrained reference example and operational-state scenarios using synthetic entities. The app should demonstrate actual server-side search and submission separately.

See `docs/CONSUMER-USAGE-ALLOCATION.md` for the maintained EAM consumer-usage baseline and acceptance scenarios.

## Level 2 workflow recheck — 0.1.25

`/quality/workflows` demonstrates target-first entity browse → `OVOverlay + OVAllocation` (known-target, application-rule-constrained source, selected-source collapse) → create → target-first `OVDataGrid` → `OVEntityInspector`, using synthetic identity objects. This is a real-component interaction reference, not EAM API validation. EAM allocation direction, eligibility, source classification, persistence and DEV acceptance remain exclusively EAM-owned. `OVAllocation` itself is unchanged in this pass.
