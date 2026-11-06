# OneVision.UI gallery workbench

The gallery is a **governed component and pattern explorer**, not a screen designer. It shows the runtime library's approved components in real reference compositions while letting implementation teams inspect supported states and independent optional capabilities together.

## Pattern exploration

Each implemented OVP page contains a common workbench with a reference state selector, independently supported optional capabilities (where the pattern exposes them), and a live render of the existing governed pattern. The choices are listed in `demo/OneVision.UI.Demo/PatternWorkbenchCatalog.cs`. Every listed choice must be connected to a real demo state or a public component parameter; the gallery must not infer fictional combinations from arbitrary property names.

For `OVP-SOURCE-CONFIGURATION`, inspect the four-tab baseline or optionally enable **Display Value** as a fifth peer tab. The source mode selects managed/direct connections, Standard Details determines Mapping's optional target catalogue, and the profile state governs whether Mapping and Display Value can be edited. Display Value receives completed target mappings, never raw source paths or unmapped fields. The example honours stale/unavailable/read-only state restrictions, and application data, persistence and domain rules remain app-owned.

Progressive Configuration Builder defaults to a generic three-step policy and also demonstrates a twelve-step definition, with independently toggleable section/step separators. The former allocation-rule-style reference remains available under its own scenario for compatibility and behavioural regression. The library, not the demo application, numbers steps and renders the active/completed/pending step states in definition mode.\n\nOther patterns expose their implemented scenarios; List Page and Operational Detail additionally demonstrate optional toolbar, action or section composition. Their real demo components remain interactive.

## Component exploration

Each OVC page retains its existing governed state choices. Where independent public parameters exist, the gallery additionally exposes modifiers from `ComponentCapabilityCatalog.cs`; these may be combined with the selected reference state. Unsupported modifiers are deliberately absent. Examples include disabled/read-only/validation/loading for appropriate input controls; independent read-only, constants and Standard Details for Mapping Pair; and dismissibility for Alert.

For `OVC-MAPPING-RULE`, inspect actual single/AND/OR/nested/wildcard/error/read-only conditions while independently toggling condition enablement, ordering, comparison case policy, a read-only Test Rule interaction, classification, and target resolution. The target selector demonstrates clear-on-parent-change using synthetic competency data. The actual governed `OVMappingRule` is rendered, not an independent screen composer.

The state panel and available independent modifiers are **gallery-only controls**. They are not new runtime APIs, validation rules or persistence schema.

## Implementation guidance

- The preview always renders the actual OneVision.UI component. Do not build a parallel HTML facsimile.
- Use the owning OVC/OVP contract to determine which modifiers exist and which combinations are meaningful.
- Represent dependencies through the owning component's existing semantics, not simulated success when inputs are invalid.
- The copyable selection summary records the current governed pattern, state, source mode where applicable, and enabled optional capabilities. Applications supply their own data, configuration, callbacks and permissions.
- Retain the established local reference-demo controls for detailed interaction/behaviour testing; the common explorer provides a consistent entry point.
- Do not introduce screen composition, arbitrary drag-and-drop component assemblies or a new runtime configuration engine.

## Verification

Browser regression coverage verifies each implemented OVP has a workbench; Source Configuration combines Display Value with managed/direct and profile states; list capabilities modify actual rendered actions; representative OVC modifiers combine with selected states; and the responsive workbench reflows at 1280/760/390px. Existing component/pattern behaviour and accessibility suites remain part of the build gate.

A gallery-only change has **no OVP-000 contract impact** and does not require a new OneVision.UI NuGet package version. Publish through the established compiled demo workflow and verify the public build marker matches the source merge commit.


## All-component coverage guard

The implemented component index must include Pagination, Dialog and Confirm Dialog because those components have real source implementations and representative interactive demos. Only Sortable Header remains concept-only. Browser regression iterates every active gallery route and rejects missing-demonstration fallbacks; the quality tracker has an entry for every runtime `OV*.razor` file, enforced by a repository contract test. See `docs/GALLERY-COMPONENT-COVERAGE.md` and `docs/QUALITY-REMEDIATION-TRACKER-20261105.md`.

Direct gallery coverage is not identical to source-component count: some runtime controls are intentionally internal children of composite OVCs. This breadth test does not certify deep behavior or 4.5/5 compliance.
