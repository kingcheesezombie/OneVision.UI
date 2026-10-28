# OVP-ENTITY-INSPECTOR

`OVP-ENTITY-INSPECTOR` governs read-only inspection of an application-owned business/domain entity without navigating away from the originating working context.

## Contract

The inspector composes the existing `OVC-OVERLAY` interaction and therefore supports the governed `RightSide` and `Workspace` placements. Opening an inspector must preserve the originating screen state; closing it returns the user to that unchanged context.

The pattern provides:

- title and optional subtitle/reference text;
- application-supplied status/type/source pill content;
- optional summary content;
- application-defined grouped detail sections, including governed tabs where richer records require them;
- loading, empty and error states;
- refresh action when supplied by the application;
- close action;
- optional application-supplied actions.

The default use is read-only. The pattern does not imply edit, merge, allocation or persistence semantics.

## Metadata

Use `OVC-METADATA-LIST` for arbitrary property collections when a compact label/value treatment is appropriate. It supports grouping, empty values, copyable values, long wrapping, monospace technical identifiers and structured values without introducing card-per-property nesting.

`OVC-KEY-VALUE` remains appropriate for isolated key/value presentation; `OVC-METADATA-LIST` is the governed collection renderer.

## Ownership boundary

OneVision.UI owns:

- overlay placement and interaction model;
- header/subtitle presentation;
- status, summary and section composition;
- loading/error/empty presentation;
- refresh/close/application-action placement;
- responsive density and accessibility inherited from governed overlay/actions;
- compact metadata-list presentation.

Applications own:

- entity retrieval and refresh semantics;
- entity identity, fields and metadata values;
- permissions;
- which sections/tabs exist;
- domain-specific actions;
- all domain meaning, validation and persistence.

No consuming application name or business-domain type is part of the pattern contract.

## Record comparison

A separate `OVC-RECORD-COMPARISON` is not part of this contract. Existing grids/detail composition should be used unless a consuming implementation demonstrates a genuinely reusable comparison interaction that cannot be expressed cleanly with current governed assets.

## OVP-000 impact

This is an additional governed pattern. It does not change `OVP-000 – OneVision Application Foundation` and therefore does not advance the foundation contract version.
