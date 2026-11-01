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
- field-key comparison against replacement structures;
- suppression of join rows whose referenced field keys no longer resolve;
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
- stable field keys where semantic continuity is intended;
- field eligibility and domain validation;
- join cardinality and matching semantics beyond structural field existence;
- persistence/deletion of domain join records;
- API integration.

OneVision.UI never retrieves domain metadata itself.

## Compatibility

Existing applications may continue to provide `LeftItems` and `RightItems` directly. Dependent structure selection is opt-in by supplying `LeftStructure` and/or `RightStructure`.

This preserves the existing mapping/join contract while allowing either side to use dependent structure resolution.

`OVMappingStructureSelectionChange.InvalidJoinKeys` remains available for compatibility, but a selection-change notification does not populate it merely because the structure context changed. At that point the replacement fields are not yet known.

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

## State transitions and join survival

Structure selection and final structural join validity are separate stages.

When a structure type changes, OneVision.UI immediately hides the currently displayed structure on that side and clears transient field selection, focus, drag and replacement state. Changing type also clears the previous definition. The application is notified through the corresponding `LeftStructureChanged` or `RightStructureChanged` callback with the requested side, type key and definition key.

When a definition changes, the same stale-structure suppression occurs before the replacement metadata is available.

A selection change does **not** itself invalidate every configured join. While replacement metadata is loading, stale fields and stale join labels are suppressed, but the supplied join definitions are not permanently discarded simply because fields are temporarily unavailable.

Once the application supplies the replacement `Fields`, OneVision.UI determines structural join survival from `OVStructuredFieldNode.Key` only:

- on a left-side change, a join remains structurally valid when its `LeftKey` exists in the replacement left structure;
- on a right-side change, a join remains structurally valid when its `RightKey` exists in the replacement right structure;
- joins referencing missing field keys are suppressed from the active join presentation;
- joins whose stable field key still exists may reappear with the replacement field label/path.

Field continuity is **not** inferred from display label, path, ordering or data type. Applications must supply the same field key when two definitions represent the same semantic field.

Applications may apply additional domain validation to surviving joins and own any persistence/deletion required for domain join records.

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
- a structure transition where one stable-key join survives and one removed-field join disappears;
- definitions loading;
- structure loading;
- no definitions;
- no fields;
- existing invalid, read-only, error and responsive states.

The Asset and Person Episode labels are demo application data only; they are not library vocabulary.
