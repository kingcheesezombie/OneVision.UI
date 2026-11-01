# OVC-MAPPING-JOIN — Governed Mapping Join

## Purpose

`OVC-MAPPING-JOIN` is the governed OneVision.UI interaction for matching fields between two structured metadata sources and configuring a comparison method for each join.

The control supports both direct structures and an optional semantic dependency chain on either side:

`Structure Type → Structure Definition → Structure Metadata`

The capability is generic. OneVision.UI does not know application concepts such as Asset, Person Episode, feed names, EAM entity types, or domain APIs.

## Ownership

OneVision.UI owns:

- structure-type and dependent-definition composition;
- transition, loading, empty and unavailable states;
- stale-field suppression when structure context changes;
- structured field presentation and search;
- left-aligned field labels with hierarchy expressed through indentation;
- selected-field and drag/drop join composition;
- comparison-method selection;
- configured join presentation and row actions;
- accessibility and responsive behaviour.

The consuming application owns:

- available structure types;
- which types require definitions;
- definition labels and available definitions;
- retrieval/construction of metadata structures;
- field eligibility and domain validation;
- join cardinality and matching semantics;
- persistence and API integration.

OneVision.UI never retrieves domain metadata itself.

## Compatibility

Existing applications may continue to provide `LeftItems` and `RightItems` directly. Dependent structure selection is opt-in by supplying `LeftStructure` and/or `RightStructure`.

This preserves the existing mapping/join contract while allowing either side to use dependent structure resolution.

## Semantic structure model

Use `OVMappingStructureState` to describe one side of the join.

A state supplies:

- `TypeLabel`;
- `Types` (`OVMappingStructureType`);
- `SelectedType`;
- available `Definitions`;
- `SelectedDefinition`;
- `DefinitionsLoading`;
- `StructureLoading`;
- resolved `Fields`;
- optional application wording for no definitions, definition not selected, and no fields.

`OVMappingStructureType.RequiresDefinition` determines whether the secondary selector is rendered. Do not compose the behaviour with low-level `ShowSecondDropdown` flags.

The definition selector label comes from `OVMappingStructureType.DefinitionLabel`; applications may therefore use domain-appropriate labels without the library assuming the word “Candidate”.

## State transitions

When a structure type changes, OneVision.UI immediately invalidates the currently displayed structure on that side and clears transient field selection, focus, drag and replacement state. The application is notified through the corresponding `LeftStructureChanged` or `RightStructureChanged` callback.

When a definition changes, the same invalidation occurs before the replacement structure is available.

The callback receives `OVMappingStructureSelectionChange`, including the side, selected type/definition keys, and join keys affected by the structure transition. The application must persist/remove domain join state as appropriate.

While a replacement structure resolves, stale fields from the previous type/definition are never shown beneath the new selection. Join rows whose field keys are not present in the currently resolved structures are also suppressed.

## Loading and empty states

Loading is local to the affected selector/field pane. The full matching editor is not replaced by a page-level spinner when only definitions or structure metadata are resolving.

Governed states include:

- no structure type selected;
- definitions loading;
- no definitions available;
- definition required but not selected;
- structure loading;
- no matchable fields;
- resolved hierarchical structure.

The field area retains a minimum footprint while resolving to reduce layout movement.

## Field browser

Both sides use `OVC-STRUCTURED-FIELD-TREE`.

Field names and metadata-node labels are left aligned. Hierarchy remains visible through the tree’s existing indentation and expand/collapse grammar. Alignment of secondary metadata such as type or paired/unpaired status is independent from field-name alignment.

Primitive eligible fields remain selectable. Structural nodes remain hierarchical and non-selectable where required by the tree selection mode.

## Join interaction

The existing join behaviour remains authoritative:

- search/filter within each structured field tree;
- select one field on each side and activate **Create join**;
- drag from either structure onto an eligible field on the opposite structure;
- choose a governed match method;
- show paired/unpaired state;
- replace or remove existing joins;
- application-defined eligibility and cardinality rules;
- read-only behaviour.

Drag/drop is never the sole join-creation mechanism.

## Accessibility

Type and definition selectors use governed labelled `OVSelect` controls. Tree search and keyboard navigation remain governed by `OVC-STRUCTURED-FIELD-TREE` / `OVC-TREE`.

Loading regions use status semantics where practical. Structure changes clear stale focused/selected field state so an inaccessible focus target is not retained against a removed structure.

## Responsive behaviour

At normal manager widths, the two structures remain side-by-side. At narrower widths, the panes stack using the existing Mapping Join breakpoint. Dependent selectors stay with their owning pane and field labels remain readable and left aligned without horizontal feature overflow.

## Demo states

The gallery demonstrates:

- direct/simple structure;
- Asset-style dependent definition;
- Person Episode-style dependent definition;
- definitions loading;
- structure loading;
- no definitions;
- no fields;
- existing invalid, read-only, error and responsive states.

The Asset and Person Episode labels are demo application data only; they are not library vocabulary.
