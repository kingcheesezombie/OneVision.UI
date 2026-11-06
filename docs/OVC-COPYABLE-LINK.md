# OVC-COPYABLE-LINK — Clipboard feedback and identity (0.1.43)

`OVCopyableLink` renders a compact, keyboard-accessible copy action with
visually selectable reference text. The component is responsible for invoking
the browser clipboard API; applications own the reference value, any optional
alternate `CopyValue`, and whether copying is permitted via `Disabled`.

## Copy success and failure

Successful clipboard writes show the existing temporary `Copied ✓` feedback
and invoke `Copied`. Denied or unavailable browser clipboard writes do **not**
report success. Instead, the component displays a restrained, transient
`Copy unavailable` state, announces the generic `CopyFailureText` through its
accessible status region and invokes the optional `CopyFailed` callback.

The default message is `Unable to copy to clipboard.`. Applications may supply
a more suitable safe generic message. No copied text, token, internal identifier
or exception details are included in error feedback by default.

`CopyFailed` is an optional `EventCallback`; existing `Copied` handlers and
the default click-to-copy contract remain backward-compatible. Browser
permission/secure-context policy remains outside the application/library and
cannot be overridden by the component.

## Lifecycle

- Changing `CopyValue ?? Text` clears any prior success/failure feedback,
  cancels its expiry timer and prevents an older pending clipboard result from
  showing success against the new value.
- Repeated copy clicks replace the previous transient feedback rather than
  allowing an old expiry timer to clear the new result.
- Disabled controls do not invoke the clipboard or callbacks.
- Keyboard activation, focus visibility, full-value copy and long-value
  truncation remain governed. `FeedbackMilliseconds` retains its minimum
  400ms presentation duration.

Quality verification includes controlled browser copy success and denial,
corresponding callbacks, updated copy-value identity, 200%/400% long-value zoom
and accessibility. Passing component-level tests does not confer L3
consuming-application certification. No OVP-000 contract change.
