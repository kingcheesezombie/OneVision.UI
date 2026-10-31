# OVC-MAPPING-PAIR — Mapping Pair

`OVC-MAPPING-PAIR` is the governed directional source-to-target mapping interaction. Actual/discovered source data is rendered through `OVC-STRUCTURED-FIELD-TREE`; the target side remains the declared or templated contract representation.

Applications supply source fields, target definitions, eligibility/cardinality rules, current mappings, optional Standard Detail catalogue data, selected Standard Details, validation and persistence callbacks. OneVision.UI owns the mapping interaction, target-row presentation, drag/drop and keyboard alternatives, optional Standard Detail row composition, responsive behaviour and accessibility.

## Required mappings

Required target mappings remain fixed in the workspace. They are always represented by their declared target row, cannot be converted into Standard Details, do not expose a Standard Detail selector and are not removable through the optional-detail interaction.

Existing mapping behaviour remains unchanged: an eligible source primitive may be dragged onto the target or selected/focused and mapped using the keyboard alternative. Existing add, replace, configure, constant and remove callbacks retain their established meanings.

## Optional Standard Details

When `StandardDetailsEnabled` is true, the target pane contains a separate **Standard Details** section.

The section header owns the single **Add standard detail** action. The action is available only when the workspace is editable, no uncommitted Standard Detail row is already present, and at least one enabled, unselected Standard Detail supplied by the application remains eligible.

Activating **Add standard detail** creates exactly one new optional row. It never expands the complete Standard Detail catalogue into the mapping workspace.

The new row contains a governed `OVC-SELECT` selector. Its choices are limited to enabled Standard Details supplied by the consuming application that are not already selected. Disabled and already-selected details cannot be chosen. Selecting a detail commits that row to the selected Standard Detail collection and enables the normal mapping/drop target for that detail.

A selected Standard Detail row continues to expose a governed selector containing its current value plus other currently eligible unselected choices. Changing the selected detail removes any UI mapping association for the previous detail before emitting the revised selected-detail collection.

Each selected Standard Detail appears at most once. Repeating **Add standard detail** adds another independent row only while another eligible detail remains.

## Empty and exhausted states

When no optional Standard Details are selected, the section displays:

`No Standard Details added to this source.`

The empty state does not contain an add action. The section header is the only add-action owner.

When all enabled supplied Standard Details are already selected, **Add standard detail** is not rendered.

## Remove behaviour

An editable Standard Detail row has a governed remove action whose accessible label identifies the detail being removed. Removing the row:

- requests removal of its current mapping association when one exists;
- removes the detail from the selected Standard Detail collection;
- makes the detail eligible for subsequent selection again.

The consuming application remains responsible for persistence and application-specific validation or confirmation requirements.

## Read-only behaviour

In read-only mode:

- **Add standard detail** is not rendered;
- Standard Detail selectors display the current selection without allowing edits;
- Standard Detail remove actions are not rendered;
- existing required and optional mappings remain visible.

## Search

Target search continues to filter only target rows that are actually part of the current mapping composition: required/declared targets and explicitly selected Standard Details. Unselected Standard Detail catalogue entries are never introduced as mapping rows by search.

## Accessibility and responsive behaviour

The governed selector supplies its visible accessible label through `OVC-SELECT`. Mapping/drop targets retain their existing accessible labels and keyboard mapping route. Standard Detail remove actions identify the detail being removed. The uncommitted row exposes its mapping target as unavailable until a detail is selected.

Rows follow the existing Mapping Pair responsive layout. At narrow widths the selector, mapping target and actions stack as one coherent row; the component introduces no fixed-width Standard Detail catalogue surface.

## Governance impact

This is an externally observable refinement of the existing `OVC-MAPPING-PAIR` contract. It does not create a new OVC or OVP and does not change `OVP-000`.
