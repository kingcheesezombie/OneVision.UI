# OVC-NOTIFICATION — transient feedback

## Ownership

`OVNotificationService` publishes application-supplied `OVNotificationRequest` messages. The library governs semantic tone, accessibility announcement, bounded delivery, dismissal, replacement, transient duration and pause/resume. The consuming application owns what the message means, including actions and whether a message should be persistent. A notification is not a substitute for persistent validation or error content.

## Governed lifecycle

- The host shows at most `MaximumVisible` notifications (minimum effective value: 1), retaining excess messages in first-in-first-out order.
- A non-empty `ReplacementKey` replaces a matching visible notification or removes a matching queued message without disturbing unrelated queued messages.
- The incoming replacement has a fresh countdown. An expired/cancelled predecessor cannot dismiss it.
- `Persistent=true` disables automatic expiry; it does not disable explicit dismissal when `Dismissible=true`.
- Non-persistent timeouts are clamped to `MinimumTimeout` and `MaximumTimeout`. Invalid/non-positive bounds normalise to a minimum of 1 millisecond.
- Hover or keyboard focus *individually* pauses the countdown. It resumes only when neither is active, with the **remaining duration**; it must not restart the entire timeout.
- When an entry is removed/replaced or the host is disposed, its pending timer is cancelled and disposed. Queued render callbacks must not resurrect disposed hosts.
- `Clock` defaults to `TimeProvider.System` and is an optional test/integration clock for deterministic timeout verification. Normal applications need not supply it.

## Accessibility and presentation

- Individual feedback preserves the existing governed role and live-region semantics: success/information as status; warning/error as alert.
- Notifications must not steal keyboard focus. Any action or dismiss control remains keyboard-operable and labelled.
- Timeouts pause while the pointer is over the item or keyboard focus is within it.
- The host stays above app surfaces without capturing unrelated pointer events, with responsive placement and reduced-motion support.

## Reference gallery and tests

The gallery renders the actual `OVNotificationHost` and `OVNotification` with real service callbacks, including timed expiry and keyed replacement actions. The contract suite uses a deterministic monotonic clock for remaining-duration, hover/focus overlap, queue replacement and disposal. Browser tests verify pointer pause/resume, live replacement and accessible presentation.

These checks support L0/L1/L4/L5 review but do not, by themselves, certify L3 consuming-application adoption or 200/400% zoom behaviour. The scorecard should change only after the relevant evidence is green.

## Foundation impact

None. This is a compatible behavioural improvement to OVC-NOTIFICATION; OVP-000 v1.5 remains unchanged.
