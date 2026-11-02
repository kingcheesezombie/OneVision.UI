# OVP-PROGRESSIVE-CONFIGURATION-BUILDER

`OVP-PROGRESSIVE-CONFIGURATION-BUILDER` governs compact, vertically presented configuration flows where later semantic choices depend on earlier choices but the work still belongs in one tab or workspace.

It is intentionally domain-neutral. Applications decide what each step means, which values are available, when dependencies are satisfied and which later values must be reset. OneVision.UI governs how those decisions are presented.

## When to use

Use this pattern when a user is constructing one semantic configuration through an ordered set of dependent choices, for example a rule, routing definition, matching definition or policy.

The pattern is appropriate when:

- the work is conceptually one configuration;
- choices have a meaningful order;
- later choices may depend on earlier choices;
- some choices may be optional, conditional or broad/wildcard selections;
- the resulting configuration benefits from a concise live human-readable summary.

## When not to use

Do not use this pattern for:

- independent form fields with no progressive relationship;
- long-running multi-stage processes that genuinely need separate pages;
- navigation across unrelated configuration areas;
- domain-specific workflow state machines;
- cases where the application already has an approved dedicated OVP that better describes the task.

A multi-page wizard is not the default. The governed composition is one vertical flow in one tab or workspace.

## Governed composition

The default order is:

1. optional title and description;
2. one or more `OVConfigurationSection` groups;
3. ordered `OVConfigurationStep` instances;
4. optional `OVConfigurationRelationship` dividers between semantic groups;
5. `OVConfigurationPreview`;
6. optional validation/information messaging;
7. standard footer actions.

Use spacing, typography and restrained dividers rather than wrapping every step in a separate card.

## Public components

The pattern is composed from:

- `OVProgressiveConfigurationBuilder` — outer one-workspace shell;
- `OVConfigurationSection` — optional semantic grouping of steps;
- `OVConfigurationStep` — governed step state and active-editor placement;
- `OVConfigurationRelationship` — optional semantic relationship between groups;
- `OVConfigurationPreview` — live human-readable result summary.

Applications may compose existing OneVision controls such as `OVSelect`, `OVTextField`, toggles and messages inside the active step editor slot.

## Step contract

`OVConfigurationStep` exposes semantic state rather than styling switches.

Important parameters include:

- `StepNumber`;
- `Label`;
- `Required`;
- `IsConditional`;
- `AllowAny`;
- `IsAny`;
- `AnyLabel`;
- `SelectedValue`;
- `HelpText`;
- `State`;
- `CanChange`;
- `IsVisible`;
- `ValidationState`;
- `ValidationMessage`;
- `Editor`;
- `ChangeRequested`.

`OVConfigurationStepState` is:

- `Pending` — dependencies are not yet satisfied;
- `Current` — the step is active and its application-supplied editor is shown;
- `Completed` — the selected semantic value is shown and may expose the governed Change action;
- `Disabled` — the step exists but is not available for the current configuration;
- `Error` — the active step requires correction.

State meaning is communicated through text and structure as well as colour.

## Progressive dependencies

The application owns dependency rules. It decides:

- which values are available;
- when a step is `Pending`, `Current`, `Completed`, `Disabled` or `Error`;
- whether a conditional step is visible;
- which later selections become invalid when an earlier selection changes;
- whether dependent values should be cleared, recalculated or preserved.

OneVision.UI owns the presentation of those states and the standard Change interaction for completed steps.

When changing an earlier step invalidates later selections, the application should update those later values and step states immediately. Stale values must not continue to look completed.

## Required, optional and conditional steps

A required step uses `Required="true"`.

A conditional step uses `IsConditional="true"` and the application controls `IsVisible`. A conditional step may also be required when it is visible.

A step that is neither required nor conditional is presented as optional.

Do not create application-specific badges for these meanings.

## Wildcard / Any semantics

Set `AllowAny="true"`, `IsAny="true"` and provide an application-appropriate `AnyLabel` such as `Any type` or `Any source`.

An Any selection is a legitimate selected value, not an error or incomplete state. When active, the step explains that the broad selection applies across matching values. When completed, the Any value is shown as the governed selected state.

The application owns the underlying wildcard value and matching semantics.

## Change behaviour

A completed step may expose `Change` when `CanChange="true"` and `ChangeRequested` is supplied.

The application responds by making the chosen step current and updating dependent steps as required. OneVision.UI does not infer dependency graphs or mutate later values automatically.

## Preview

`OVConfigurationPreview` is part of the governed composition and should remain visible while the configuration is being built.

The application supplies the human-readable preview content. It may contain labels, semantic tokens, separators and directional relationships. OneVision.UI owns the preview surface and incomplete-state presentation.

The preview should describe meaning rather than implementation syntax. Avoid exposing raw IDs, JSON, expression syntax or internal rule DSL unless the application explicitly needs a technical view elsewhere.

For source-to-target configurations, visually distinguishing the source/context side from the result/target side is encouraged, but the domain wording remains application-owned.

## Validation

Step-specific validation belongs on the affected `OVConfigurationStep` using `ValidationState` and `ValidationMessage`.

Cross-step or save-level validation belongs in the builder `Validation` region using existing OneVision messages such as `OVAlert`.

A broad Any value must not be styled as incomplete merely because it is less specific.

## Actions

Use the builder `Actions` region for the standard OneVision footer treatment. Normal editable configurations should use a secondary Cancel action and a primary Save action. Applications own save eligibility, persistence, authorization and dirty-state behaviour.

## Accessibility

The pattern provides:

- labelled step regions;
- `aria-current="step"` for the active/current or error step;
- disabled and invalid semantics where applicable;
- keyboard-operable standard Change buttons;
- text labels for completed, pending and unavailable states;
- live preview/validation announcement regions;
- visible focus behaviour inherited from governed OneVision controls.

Applications must supply accessible labels to the controls placed in each `Editor` slot.

## Ownership boundary

OneVision.UI owns:

- the vertical single-workspace composition;
- section and step spacing/density;
- sequence indicators;
- required/optional/conditional presentation;
- current/completed/pending/disabled/error presentation;
- Any/wildcard presentation;
- completed-step Change action placement;
- semantic relationship divider presentation;
- preview and incomplete-preview surfaces;
- validation placement;
- footer action layout;
- responsive and accessibility behaviour.

Applications own:

- domain labels and field meanings;
- available options;
- dependency rules;
- conditional visibility decisions;
- wildcard business semantics;
- dependent-value invalidation/reset rules;
- preview content;
- validation rules;
- permissions and persistence.

## Reference composition

The gallery includes an allocation-rule-style example using generic demo data. It demonstrates:

`Class → Entity Type → Source → may allocate to → Target Provider → conditional Target Entity Type`

The example includes:

- required and optional steps;
- `Any type` and `Any source`;
- a conditional target-type step;
- Change behaviour;
- dependent reset when an earlier step changes;
- current/completed/pending/error states;
- live source-to-target preview;
- completed and validation examples.

These labels demonstrate composition only. No EAM business logic is encoded into the reusable components.

## OVP-000 impact

This is a new reusable pattern and does not alter the application composition-root contract. `OVP-000 – OneVision Application Foundation` is unchanged.
