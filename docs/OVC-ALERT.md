# OVC-ALERT — Inline contextual feedback

OneVision.UI owns a compact, governed alert appearance for informational, success,
warning and error messages. The consuming application supplies the meaning and
content through `ChildContent`, selects `Tone`, and decides whether dismissal is allowed.

## Visibility and dismissal

`Dismissible=true` exposes the governed icon-only Dismiss message action.
Activating it hides the current alert and invokes the existing `Dismissed` callback.
Ordinary rerenders do not show the same dismissed message again.

For sources that reuse one Blazor component instance to show distinct successive
messages, specify the **optional** semantic `MessageKey`:

```razor
<OVAlert Tone="OVAlertTone.Warning"
         Dismissible="true"
         MessageKey="@_currentWarningId"
         Dismissed="OnDismissed">
    @_currentWarningText
</OVAlert>
```

A different `MessageKey` makes the replacement message visible, even if the previous
message was dismissed. Using the same key preserves the user's dismissal. Keys are
consumer-owned identity tokens, not display text; do not derive them from a rapidly
changing render timestamp or attempt to reset on any parent render.

Consumers that omit `MessageKey` preserve the original dismissal behaviour.
`MessageKey` does not modify the alert severity, callbacks or application error state.

## Quality gates

Library tests cover old/new/same-key visibility, opt-in semantics and the existing
dismiss callback. The actual gallery demonstrates a dismissible alert followed by
a replacement message. Browser tests cover pointer and keyboard interactions,
accessible naming, axe, and 640px/320px CSS viewports equivalent to 200%/400% zoom on a 1280px window; CI results and publication must be recorded
before this item is considered fully certified.

This is an additive OVC contract, not an OVP-000 change. No EAM, HDM, KPI or IDM
application behaviour is included in this component.
