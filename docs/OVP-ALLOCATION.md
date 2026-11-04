# OVP-ALLOCATION — effective-dated allocation

**Implementation:** `OVAllocation`  
**Status:** Draft — governed reusable composition  
**Foundation impact:** None; the OVP-000 contract remains unchanged.

## Purpose

Create a source-to-target allocation in a compact, single-column, target-first workflow. The pattern governs composition and state presentation; consuming applications own entity data, eligibility, validation, permissions, and persistence.

## Composition

1. **Target entity:** a compact, read-only identity row for a known target. In finder mode, show the available target platforms only when more than one is permitted, the target type where relevant, search and results. After selection, collapse to the same read-only identity row and optionally offer Change.
2. **Source entity:** optional source system/type criteria, a prominent search input, and results. Once selection is complete, use the selected-source summary with an optional Change action in place of result rows.
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
| `SourceSystem`, `SourceType` | Optional editable selector regions |
| `FixedSourceSystem`, `FixedSourceType` | Read-only values when the application already knows an immutable choice; override their corresponding editor fragments |
| `SourceSearch`, `SourceResults` | Source search and eligible application-supplied results |
| `SourceSelected`, `SelectedSource` | App signals a completed choice; show selected identity instead of the result list |
| `ChangeSourceRequested` | Optional callback to resume source selection |
| `AlwaysBelonged`, `FromDate`, `ToDate` and callbacks | Governed conditional date editor |
| `ReadOnly`, `Loading`, `ErrorMessage`, `ValidationSummary`, `Actions` | Governed interaction/feedback slots and app-supplied operations |

`SourceType` is optional for fixed-classification workflows, such as EAM Attribute allocations. Do not expose a redundant editable type control when the app has supplied a single permissible type. `FixedSourceType` can show context without suggesting the value is editable.

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
