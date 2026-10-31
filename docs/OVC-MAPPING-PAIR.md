# OVC-MAPPING-PAIR — Mapping Pair

`OVC-MAPPING-PAIR` is the governed directional source-to-target mapping interaction. Actual/discovered source data is rendered through `OVC-STRUCTURED-FIELD-TREE`; the target side remains the declared or templated contract representation.

Applications supply source fields, target definitions, eligibility/cardinality rules, current mappings, optional Standard Detail catalogue data, selected Standard Details, validation and persistence callbacks. OneVision.UI owns the mapping interaction, target-row presentation, drag/drop and keyboard alternatives, optional Standard Detail row composition, responsive behaviour and accessibility.

## Required mappings

Required target mappings remain fixed in the workspace. They are always represented by their declared target row, cannot be converted into Standard Details, do not expose a Standard Detail selector and are not removable through the optional-detail interaction.

Existing mapping behaviour remains unchanged: an eligible source primitive may be dragged onto the target or selected/focused and mapped using the keyboard alternative. Existing add, replace, configure, constant and remove callbacks retain their established meanings.

## Optional Standard Details

When `StandardDetailsEnabled` is true, the target pane contains a separate **Standard Details** section with the supporting text:

`Optional governed details added to this source only when needed.`

The section header owns the single **Add standard detail** action. The action is available only when the workspace is editable, no uncommitted Standard Detail row is already present, and at least one enabled, unselected Standard Detail supplied by the application remains eligible.

Activating **Add standard detail** creates exactly one draft row. It never expands the complete Standard Detail catalogue into the mapping workspace.

The draft row contains a governed `OVC-SELECT` selector, a pending mapping surface and a governed remove/cancel action. The mapping surface remains unavailable until a detail is chosen. Selector choices are limited to enabled Standard Details supplied by the consuming application that are not already selected. Disabled and already-selected details cannot be chosen.

Selecting a detail commits the row. A committed Standard Detail row is data-like rather than form-like and follows the governed composition:

`Standard Detail identity → mapped source → action`

The left area displays the Standard Detail name and subordinate description. The central area is the existing bounded mapping/drop surface. The right area owns governed row actions, including removal where editable. An unmapped committed row retains the active drop target and may display the compact **Mapping required** status.

The full selector is not permanently visible in committed state. Editable committed rows expose a subtle **Change** affordance. Activating Change temporarily exposes the governed selector for that row. The selector contains the current detail plus eligible unselected details, excludes disabled details, and excludes details already selected by another row.

Changing the selected detail removes any current mapping association for the previous detail before emitting the revised selected-detail collection. Selecting the current value closes the temporary edit state without changing selection.

Each selected Standard Detail appears at most once. Repeating **Add standard detail** adds another independent row only while another eligible detail remains.

## Empty and exhausted states

When no optional Standard Details are selected and no draft row exists, the section displays:

`No Standard Details added to this source.`

The empty state does not contain an add action. The section header is the only add-action owner.

When all enabled supplied Standard Details are already selected, **Add standard detail** is not rendered.

## Mapping surface

Committed Standard Detail rows reuse the governed Mapping Pair mapping surface rather than rendering mapped source data as loose text.

Mapped rows display the mapped source path within the bounded mapped surface. Unmapped rows display the governed drop guidance. Existing drag/drop, keyboard mapping, replacement, configuration and constant behaviour remains unchanged where applicable.

## Remove behaviour

An editable Standard Detail row has a governed remove action at the far right whose accessible label identifies the detail being removed. Removing the row:

- requests removal of its current mapping association when one exists;
- removes the detail from the selected Standard Detail collection;
- makes the detail eligible for subsequent selection again.

The consuming application remains responsible for persistence and application-specific validation or confirmation requirements.

## Read-only behaviour

In read-only mode:

- **Add standard detail** is not rendered;
- **Change** affordances are not rendered;
- Standard Detail selectors are not rendered for committed rows;
- Standard Detail remove actions are not rendered;
- Standard Detail name and description remain visible;
- existing required and optional mapping state remains visible in the same compact row composition.

## Search

Target search continues to filter only target rows that are actually part of the current mapping composition: required/declared targets and explicitly selected Standard Details. Unselected Standard Detail catalogue entries are never introduced as mapping rows by search.

## Accessibility and responsive behaviour

The temporary selector supplies its visible accessible label through `OVC-SELECT`. The visible **Change** action exposes an accessible label identifying the selected Standard Detail. Mapping/drop targets retain their existing accessible labels and keyboard mapping route. Standard Detail remove actions identify the detail being removed. The draft row exposes its mapping target as unavailable until a detail is selected.

At normal administrative widths the committed row follows approximately:

`[ Standard Detail identity ] [ Mapping surface ] [ Actions ]`

The first two areas flex without fixed pixel widths. At narrow widths those logical areas stack in the same order while remaining one coherent row. The mapping surface stays visually associated with its Standard Detail and the remove action remains discoverable.

## Ownership

OneVision.UI owns committed and draft Standard Detail row composition, Change interaction, mapping-surface presentation, remove-action placement, responsive behaviour, empty-state behaviour, accessibility and selection interaction.

Consuming applications continue to own catalogue data, enabled state, selected detail IDs, mapping state, persistence, validation, domain-specific eligibility and API/database operations. Applications must not recreate a competing Standard Detail row convention locally.

## Governance impact

This is an externally observable refinement of the existing `OVC-MAPPING-PAIR` contract. It does not create a new OVC or OVP and does not change `OVP-000`.
