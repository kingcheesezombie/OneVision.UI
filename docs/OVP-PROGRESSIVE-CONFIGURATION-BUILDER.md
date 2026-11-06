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

Do not use this pattern for independent form fields with no progressive relationship, genuinely multi-page long-running processes, navigation across unrelated configuration areas, domain-specific workflow state machines, or where another approved OVP describes the task better.

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

Applications may compose existing governed OneVision controls inside the active step editor slot.

## Builder and section labelling

When `Title` is supplied, the builder or section labels its region from that rendered title. When no title is supplied, it does not emit an `aria-labelledby` reference to a missing element.

Untitled compositions may supply `AriaLabel` when the region needs an explicit accessible name. If neither `Title` nor `AriaLabel` is supplied, no invalid or empty accessible-name relationship is emitted.

Builder and section visual titles expose accessible heading semantics. `HeadingLevel` is configurable so consumers can place the pattern correctly within the surrounding page hierarchy. The builder defaults to level 2 and sections to level 3; consumers should override those values where their page structure requires different levels. Values are constrained to heading levels 1–6.

The heading treatment remains visually compact and does not require applications to introduce custom heading CSS.

## Step contract

`OVConfigurationStep` exposes semantic state rather than styling switches.

Important parameters include `StepNumber`, `Label`, `Required`, `IsConditional`, `AllowAny`, `IsAny`, `AnyLabel`, `SelectedValue`, `HelpText`, `State`, `CanChange`, `IsVisible`, `ValidationState`, `ValidationMessage`, `Editor` and `ChangeRequested`.

`OVConfigurationStepState` is:

- `Pending` — dependencies are not yet satisfied;
- `Current` — the step is active and its application-supplied editor is shown;
- `Completed` — the selected semantic value is shown and may expose the governed Change action;
- `Disabled` — the step exists but is not available for the current configuration;
- `Error` — the active step requires correction.

Completed steps use a consistent label/value/action grid: a muted label, visually stronger selected value, and a right-aligned governed subtle Change action. The current step retains a pale primary-tinted surface and a stronger left accent. The relationship divider presents application-provided text beneath a restrained downward indicator in vertical compositions; the preview likewise places the source expression above the relationship and target expression. Valid wildcard values show a small asterisk indicator with a subtle primary tint, not a warning. These are visual-only refinements; no application-owned dependency or API contracts change.\n\nEvery rendered state retains accessible textual state information. Completed and pending states visually rely on their compact row composition and completion/sequence markers, while the screen-reader text preserves the explicit state. The active editor has a restrained primary accent and more space; error state has a governed error accent and associated inline validation. Requirement/applicability metadata is secondary and does not compete with the completed value.

## Required, optional and conditional semantics

Required/optional and conditional are independent meanings.

Valid combinations include:

- required;
- optional;
- required + conditional;
- optional + conditional.

A required step uses `Required="true"`. A conditional step uses `IsConditional="true"`; the application controls `IsVisible` and the current state. Conditional does not imply required.

The combined accessible text reflects both meanings, for example `Required · Conditional` or `Optional · Conditional`. The text is visually subordinate or screen-reader-only in compact completed and pending rows. Applications must not recreate these meanings with app-specific badges.

## Progressive dependencies

The application owns dependency rules. It decides which values are available, when each state applies, whether a conditional step is visible, which later selections become invalid when an earlier selection changes, and whether dependent values are cleared, recalculated or preserved.

OneVision.UI owns the presentation of those states and the standard Change interaction. It does not infer dependency graphs or silently mutate domain data.

When changing an earlier step invalidates later selections, the application should update those later values and step states immediately. Stale values must not continue to look completed.

## Wildcard / Any semantics

Set `AllowAny="true"`, `IsAny="true"` and provide an application-appropriate `AnyLabel` such as `Any type`, `Any source` or `Any target type`.

An Any selection is a legitimate selected value, not an error or incomplete state. It may be completed, including on an optional + conditional step. The application owns the underlying wildcard value and matching semantics.

If multiple OneVision applications later recreate the same specific-value-or-Any editor interaction, that may justify a separate semantic helper. It is not part of this pattern contract today.

## Change behaviour

A completed step may expose `Change` when `CanChange="true"` and `ChangeRequested` is supplied.

The application responds by making the chosen step current and updating dependent steps as required. OneVision.UI does not infer dependency graphs or mutate later values automatically.

## Preview

`OVConfigurationPreview` is part of the governed composition. The application supplies human-readable content; OneVision.UI owns the preview surface and incomplete-state presentation.

The preview should describe meaning rather than implementation syntax. Avoid exposing raw IDs, JSON, expression syntax or internal rule DSL unless the consuming application explicitly needs a technical view elsewhere.

The default preview is an integrated, concise summary separated from the steps by a restrained divider, without a decorative tinted card. The application supplies a human-readable statement that explains the configured meaning (for example, **ESR Organisation Unit assets may be allocated to EAM Location targets**), rather than repeating every technical token. For compact builders such as EAM Allocation Rules, the normal bottom-of-flow preview remains the governed default. Longer builders should still keep preview access useful through their composition. Sticky/anchored preview behaviour is not governed today; do not add app-local sticky CSS as a new convention. If repeated consumers demonstrate that need, add a future semantic OneVision.UI option deliberately.

## Validation

Step-specific validation belongs on the affected `OVConfigurationStep` using `ValidationState` and `ValidationMessage`. Cross-step or save-level validation belongs in the builder `Validation` region using existing OneVision messages such as `OVAlert`.

A broad Any value must not be styled as incomplete merely because it is less specific.

## Actions

Use the builder `Actions` region for the standard OneVision footer treatment. Normal editable configurations should use a secondary Cancel action and a primary Save action. Applications own save eligibility, persistence, authorization and dirty-state behaviour.

## Accessibility

The pattern provides labelled step regions; valid optional builder/section labelling; configurable heading semantics; `aria-current="step"` for active/current or error steps; disabled and invalid semantics; keyboard-operable Change actions; textual state communication; and live preview/validation announcement regions.

Applications remain responsible for accessible labels on controls supplied inside editor slots and for choosing heading levels appropriate to the surrounding page hierarchy.

## Ownership boundary

OneVision.UI owns the vertical single-workspace composition, section and step density, sequence indicators, required/optional/conditional presentation, current/completed/pending/disabled/error presentation, Any/wildcard presentation, completed-step Change placement, relationship-divider presentation, preview/incomplete-preview surfaces, validation placement, footer layout, and responsive/accessibility behaviour.

Applications own domain labels and meanings, available options, dependency rules, conditional visibility, wildcard business semantics, dependent-value invalidation/reset rules, preview content, validation rules, permissions and persistence.

## Reference composition

The gallery includes an allocation-rule-style example using generic demo data:

`Class → Entity Type → Source → may allocate to → Target Provider → conditional Target Entity Type`

The reference demo also offers initial, partially completed, completed, long selected-value and validation states. Completed values appear in compact single-line summary rows when space permits; the current step has a distinct editor surface; pending steps are subdued with dependency explanations. A responsive narrow viewport preserves the same semantic order.

The reference demonstrates:

- Class — required;
- Entity Type — optional, including `Any type`;
- Source — optional, including `Any source`;
- Target Provider — required;
- Target Entity Type — conditional + optional when applicable, including `Any target type`;
- Change behaviour and dependent reset;
- current/completed/pending/error states;
- live source-to-target preview;
- completed and validation examples.

Example human-readable results include:

`ESR Organisation Unit assets may be allocated to EAM Location targets.`

and:

`Any source assets of any type may be allocated to EAM targets of any type.`

These labels demonstrate composition only. No EAM business logic is encoded into the reusable components.

## Visual polish acceptance (0.1.21)\n\nRegression checks cover completed label/value hierarchy and action alignment, vertical relationship and preview placement, visible wildcard semantics, active-editor prominence and keyboard focus. Existing dependency, responsiveness, long-value wrapping and accessibility checks remain in effect.\n\n## OVP-000 impact

This refinement does not alter the application composition-root contract. `OVP-000 – OneVision Application Foundation` is unchanged.

## Operational-quality refinements (0.1.24)

The builder adds opt-in `Busy`/`BusyMessage`, `ErrorMessage`, `Completed`/`CompletedMessage`, `ReadOnly`, and `EmbeddedInOverlay` parameters. These state messages are displayed while the ordered step values and preview remain present. A read-only or completed builder hides its mutation footer. An embedded builder delegates heading and footer to the owning `OVOverlay`.

Applications own the step editor state, disabling editors while busy/read-only, clearing downstream dependent choices, and supplying/validating save/cancel actions. The wrapper does not invent dependency rules or modify selected values. The gallery demonstrates real Change/dependency-reset, read-only, submitting, recoverable submission failure and completed configurations, rather than unrelated static mocks.

## Level 2 workflow recheck — 0.1.25

The synthetic administration reference places the real Progressive Configuration Builder between Catalogue Management and Schedule Builder, followed by Operational Detail. Validate step dependency clearing, wildcard semantics, save errors and read-only behaviour in its own component tests; no application allocation-rule engine has been introduced.

## Definition-driven composition (0.1.37)

The preferred composition is now **Items**, an ordered `IReadOnlyList<OVConfigurationItemDefinition>`. Applications can supply any number of items; they do **not** need to construct separate `OVConfigurationStep` elements, assign ordinal numbers, choose the active editor, render Change buttons, or add CSS for separators.

- `OVConfigurationStepDefinition(Key, Label)`: an application-owned semantic step. Supply `Value`, `ValueChanged`, and `Options` for a governed OneVision `OVSelect`, or `Editor` for a specialised OneVision input. Optional `DisplayValue`, `Complete`, `DependsOn`, `Required`, `IsConditional`, `IsVisible`, `Enabled`, `CanChange`, wildcard, and validation fields control semantics. `Key` must be stable and unique.
- `OVConfigurationSectionDefinition(Key, Title, Items)`: optional semantic grouping, containing any number of steps or separators, with optional description. Sections can be omitted entirely or combined with ungrouped steps.
- `OVConfigurationSeparatorDefinition(Key, Style, Label)`: an optional boundary **between individual steps or whole sections**, not an application-specific markup fragment. `OVConfigurationSeparatorStyle` supports `None`, `Divider`, `LabelledDivider`, `Arrow` and `LabelledArrow`.

The library derives visible step numbering and exactly one active editor, places completed selections in compact rows with governed Change actions, displays pending/disabled/error states, respects explicit `DependsOn` prerequisites, advances when a built-in select changes, and keeps the existing preview, validation, loading, read-only and save footer semantics. `ActiveStepKey`/`ActiveStepKeyChanged` are available when a consumer needs to control which eligible editor is open. Rendering uses stable item keys; duplicate or empty keys are rejected.

**Ownership:** Applications still define data and option catalogues, dependency keys and eligibility, conditional visibility, selection invalidation, business validation, access rights and persistence. The library does not infer domain-specific downstream value resets. A custom editor updates the app's value and the library derives progression from the resulting definition; consumers may explicitly control the active step when needed.

Example (application code, synthetic values):

```razor
<OVProgressiveConfigurationBuilder Title="Policy" Items="@_definition" />

@code {
    private IReadOnlyList<OVConfigurationItemDefinition> _definition = [
        new OVConfigurationSectionDefinition("source", "Source", [
            new OVConfigurationStepDefinition("category", "Category") {
                Options = [new("primary", "Primary"), new("secondary", "Secondary")]
            }
        ]),
        new OVConfigurationSeparatorDefinition(
            "transition", OVConfigurationSeparatorStyle.LabelledArrow, "then apply"),
        new OVConfigurationSectionDefinition("destination", "Destination", [
            new OVConfigurationStepDefinition("route", "Route") {
                DependsOn = ["category"],
                Options = [new("a", "Route A"), new("b", "Route B")]
            }
        ])
    ];
}
```

For real interactive usage, bind each step's `Value` and `ValueChanged` to application-owned state. The gallery demonstrates both three-step and twelve-step definitions, section separators and inline separators, plus the retained legacy allocation example.

**Compatibility:** `ChildContent` is still supported when `Items` is omitted, preserving existing hand-composed consumers. When `Items` is supplied it is authoritative, including an empty list. This is an additive public API and does not change OVP-000.

## Visual hierarchy refinement (0.1.38)

This visual refinement applies to both the legacy `ChildContent` composition and the definition-driven `Items` composition introduced in 0.1.37. It does not change application-owned rule logic, dependencies, selected values or save callbacks.

- **Sections:** use stronger headings and secondary descriptions. An optional semantic `Completed` state displays one unobtrusive completion indicator beside the section heading. In definition-driven mode the builder derives completion from all visible, enabled child steps; applications using direct section composition supply `Completed` themselves.
- **Steps:** completed rows use balanced label/value/action columns at administrative density. The selected value leads visually, and the existing keyboard-operable `Change` action remains aligned to the right. Step numbers stay present and state information remains accessible, without a large success tick on every row. Current, pending, error, disabled and wildcard states retain their established semantic behaviour.
- **Relationship:** a labelled arrow or divider conveys the semantic transition between sections clearly, without additional cards. The established `OVConfigurationSeparatorStyle` variants remain supported.
- **Summary:** `OVConfigurationPreview` uses a compact, lightly separated surface; consumers should pass a single readable rule statement where practical, not a repetition of technical fields. It remains a generic `RenderFragment`, so specialised applications may provide other concise summaries.
- **Save feedback:** the completed builder uses a small, accessible, non-blocking success status instead of a large full-width success alert. Error and validation feedback retain their distinct alert treatment.

Desktop and narrow-screen regression checks cover step-to-step alignment, responsive wrapping, the active editor, accessible Change actions, section completion, save feedback and a human-readable summary. No `OVP-000` version change is required.
