# Governed state-aware composition — OVC-PANEL, OVC-TOOLBAR, OVC-ACTION-BAR

## Principle

Containers own the **state signal and placement**, not arbitrary application actions. A generic read-only or busy mode cannot safely disable every nested control: users may still need to copy references, navigate, refresh, inspect, cancel an operation or request help.

The library now provides optional typed render-fragment slots to transmit that state to the code that owns the actual interaction. Existing untyped slots remain source-compatible and unchanged.

## OVC-PANEL

- `ReadOnly` continues to apply the governed visual modifier.
- `ReadOnlyHeader(RenderFragment<bool>)`, `ReadOnlyContent(RenderFragment<bool>)` and `ReadOnlyFooter(RenderFragment<bool>)` receive the effective read-only value.
- Supplied state-aware slots take precedence over legacy `Header`, `ChildContent` and `Footer`, respectively.
- Use the supplied bool to set `OVTextField.ReadOnly`, `OVSelect.ReadOnly`, `OVButton.Disabled` **only for mutations**, etc. Readable values, links, copy and permitted navigation must remain usable.
- The application chooses when the panel is read-only; no application data/permission model is embedded in the library.

## OVC-TOOLBAR

- `ReadOnlyLeading(RenderFragment<bool>)` and `ReadOnlyTrailing(RenderFragment<bool>)` are optional alternatives to `Leading` and `Trailing`.
- The mutable controls in a toolbar consume the propagated flag. Refresh and other non-mutating interactions can remain enabled.
- `Middle` remains pure supporting composition; no special state hook is required.

## OVC-ACTION-BAR

- `BusyActions(RenderFragment<bool>)` is the state-aware alternative to `Actions`.
- The caller disables unsafe/mutating actions while Busy while preserving meaningful cancellation, help or navigation where permitted.
- The existing `aria-busy` region, visibility and action hierarchy are unchanged.

## Workbench and regression evidence

The live component workbench now passes independent capabilities into the actual DisplayShellComponentDemo (including Toolbar, which previously received no modifiers). The workbench enables Read-only for Panel and Busy for Action Bar, with all stateful fragments wired to actual OneVision.UI controls.

The unit tests verify both state values, legacy fragments, semantic input/read-only and button/disabled wiring. Browser tests check actual workbench modifiers, preserving permitted Refresh and non-mutating context, and panel accessibility.

No OVP-000 contract change is introduced. Consuming applications must adopt typed slots where desired; their use of a legacy fragment is not evidence that child controls were automatically made read-only.
