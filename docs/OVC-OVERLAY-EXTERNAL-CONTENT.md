# OVC-OVERLAY — External Content (Stage A)

Status: Stage A implementation candidate; Stage B (cooperative messaging) is not included in this release.
Baseline: OneVision.UI 0.1.54. OVP-000 is unchanged.

## Ownership

OneVision.UI owns the overlay shell, responsive iframe presentation, origin checking and close behaviour. The host application selects the destination and approved origins. The destination application owns its own authentication, authorisation, embedded layout, permissions and business operations.

## Opt-in API

```razor
<OVOverlay Open="@showViewer"
           OpenChanged="@((bool open) => showViewer = open)"
           Title="External viewer"
           Placement="OVOverlayPlacement.Workspace"
           ContentMode="OVOverlayContentMode.External"
           ExternalContent="@viewer" />
```

```csharp
private OVOverlayExternalContent viewer = new()
{
    Url = "https://viewer.example.org/items/123",
    PermittedOrigins = new[] { "https://viewer.example.org" },
    FrameTitle = "Item viewer",
    ClosePolicy = OVOverlayExternalClosePolicy.Normal
};
```

Native mode remains the default and existing `ChildContent` render paths are unchanged. External-only overlays do not require `ChildContent`. When external mode is selected, external configuration governs the body; native `ChildContent` is not rendered.

## Safety and browser limitations

- The host validates the configured initial iframe URL and any later URL supplied by the host against explicitly configured scheme/host/port origins. Only HTTP and HTTPS are accepted. URL user-info is rejected. HTTP may be used where host deployment policy permits it.
- The host **cannot** generally inspect or enforce navigation occurring inside a cross-origin iframe. A cooperating destination or browser policy must enforce any subsequent navigation constraints.
- A destination may refuse embedding through Content Security Policy `frame-ancestors` or `X-Frame-Options`. The library does not bypass these protections.
- An iframe `load` event is **not** proof that the desired application is ready. Stage A presents the browser-load completion as unconfirmed rather than reporting success or inferring HTTP failures.
- Authentication, cookie policies, third-party-cookie restrictions and Windows Authentication must be validated in the target environment.
- No sandbox attributes or permissions are applied implicitly; these need explicit destination-specific assessment rather than globally weakened browser policies.
- `StandaloneUrl` is optional and must share an approved origin. An approved configured iframe URL is used for the fallback action when a separate standalone URL is absent.
- `ClosePolicy=Confirm` uses the existing governed discard confirmation; `Normal` uses the existing close lifecycle. Stage A cannot automatically detect remote unsaved changes.

## Independent release stages

Stage A: iframe hosting, URL/origin validation, responsive presentation, loading/unconfirmed state, close policy and optional standalone navigation. Requires **no message protocol**.

Stage B (future optional addition): verified window/source/origin/version/session messaging; readiness, dirty state, changed, completed, cancelled and close-request events. Its absence must not interfere with Stage A viewers.

## Release verification

Build and run component/security/browser tests for both native and external modes, including framing refusal, close confirmation, 320–1024px sizes and focus restoration. Publish only after these pass, update the demo capability explorer and verify that the public Pages gallery reflects the shipped package. Do not advance OVP-000.
