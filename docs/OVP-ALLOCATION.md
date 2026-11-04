# OVP-ALLOCATION — effective-dated allocation

**Implementation:** `OVAllocation`  
**Status:** Draft — governed reusable composition  
**Foundation impact:** None; the OVP-000 contract remains unchanged.

## Purpose

Create a source-to-target allocation in a compact, single-column, target-first workflow. The pattern governs composition and state presentation; consuming applications own entity data, eligibility, validation, permissions, and persistence.

## Composition

1. **Target entity:** a compact, read-only identity row for a known target. In finder mode, show the available target platforms only when more than one is permitted, the target type where relevant, search and results. After selection, collapse to the same read-only identity row and optionally offer Change.
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
| `Target`, `TargetType`, `TargetSearch`, `TargetResults` | App-owned identity/type/search/results fragments |
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
- Other consumers may enable target finder, multi-platform selection, additional source types, and explicit dates.
- The library does **not** enforce target/source eligibility, cardinality, overlapping dates, rule permissions, lifecycle semantics, or call an allocation API.
- Consuming apps remain responsible for disabling app-supplied fragments in read-only mode and for clearing/updating the controlled selection state after Change.
- No inferred EAM/ODM resolution or hierarchy behaviour belongs in this pattern.

## Gallery validation

The gallery's **Allocation** workbench exposes known-target and find-target scenarios plus combinations of editable/fixed criteria, selected states, target-platform constraints and explicit dates. Verify both scenarios at desktop and compact widths, with the source result list and selected summary, and with/without dates.

## List presentation

Use `OVP-LIST-PAGE` with a governed grid for target-first allocation lists. `OVAllocation` owns the editor, not list management. For a single known target, display its identity once above its allocated source rows.

## Overlay composition and operational states

When rendering within `OVOverlay`, set `EmbeddedInOverlay=true`, supplying `Title`, `Description`, `SaveText`, `SaveDisabled`, `Busy`, `SaveRequested` and `OpenChanged` on the overlay. Do not render a second action footer inside the allocation pattern. The overlay owns scrolling, escape/backdrop handling, focus management and save/cancel placement. The allocation component still renders validation feedback in its body.

`Loading` is intended for the first fetch of the entire form. Use `SearchLoading`, `NoSourceResults`, `SearchErrorMessage` and `EligibilityUnavailable` for localised source-search states, retaining the known target. Use `Submitting`, `SubmissionErrorMessage` and `Completed` for submit states; selected identities persist. Errors must not destroy the in-progress allocation.

Consuming applications own server-side filtering/pagination, rule-constrained choices, invalidating downstream source selections, read-only/disabled behaviour within supplied fragments, permissions and save validity. No EAM-specific classifications, rule identifiers or API calls exist in `OVAllocation`. The generic `SourceCriteria` region allows any number of criteria; it does not implement a rule engine.

The gallery supplies a rule-constrained reference example and operational-state scenarios using synthetic entities. The app should demonstrate actual server-side search and submission separately.

See `docs/CONSUMER-USAGE-ALLOCATION.md` for the maintained EAM consumer-usage baseline and acceptance scenarios.
