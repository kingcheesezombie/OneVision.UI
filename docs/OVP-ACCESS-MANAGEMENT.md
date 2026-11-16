# OVP-ACCESS-MANAGEMENT — Governed Access Management

**Contract:** 1.0 (initial), **Foundation:** OVP-000 (unchanged)  
**Owner:** OneVision.UI for presentation and interaction; consuming application for authentication, authorisation, identity, grants, persistence and audit.

## Modes and scope

`OVAccessManagement` has one required semantic mode: `Manage`, `InspectOnly`, `Bootstrap` or `Recovery`. Bootstrap and recovery are **separate application-owned entry points**, not authenticated administration tabs. Consuming applications provide their routes, access policies, server-evaluated eligibility and operations. An application enabling bootstrap **must verify that an eligible first administrator can reach and complete setup before ordinary role administration is enforced**.

In `Manage` or `InspectOnly`, the pattern renders an assignment-backed principal catalogue using governed search, data grid, paging, pills, row overflow, role inspection, assignment overlay, optional effective access and optional audit. Identity discovery searches *only authorised server-provided results*, distinct from the assignment catalogue.

In `Bootstrap` or `Recovery`, the pattern renders the server-supplied lifecycle state, authenticated identity and confirmation. An accessible bootstrap route is not permission to initialise an administrator; server eligibility and one-time enforcement must happen atomically.

## Semantic API

`OVAccessFeatures` independently controls supported capabilities. `OVAccessAvailability` represents action-specific permission and reason, separately from support and lifecycle state. `OVAccessMode` is the presentation/lifecycle mode. The pattern never calculates access or AD membership.

All identity references (`PrincipalId`, `RoleId`, `AssignmentId`) are opaque stable strings; they are **not** display labels, inferred SID formats or composite client keys. Direct role grants are `OVAccessAssignment` entries, never inferred from implicit baseline or effective grants. Direct revocation sends `AssignmentId` and optional revision. Duplicate, conflict, forbidden, revoked and final-admin outcomes remain server-authored typed results. Grant/revoke callbacks are not invoked if the component has no server-provided permission/eligibility; the application must independently check every request.

The host supplies:
- `LoadPrincipals(OVAccessQuery, CancellationToken)` as a server-scoped, assignment-backed page.
- `Roles` with permissions, eligible/implicit state and stable identifiers.
- Optional `SearchAvailablePrincipals` and `CheckGrantEligibility` for progressive assignment.
- `GrantRole` and `RevokeRole` callbacks returning `OVAccessMutationResult`.
- Optional `LoadEffectiveAccess` returning read-only outcomes, provenance, evaluation quality/scope and timestamp.
- Optional `LoadAudit(OVAccessAuditQuery, CancellationToken)` returning a scoped, principal/action-filtered paged read-only audit with a completeness notice; the caller may supply `AuditActions` options.
- Separate `Lifecycle`, `BootstrapAdministrator` and `RequestRecovery` for dedicated modes.
- `DataChanged` to refresh app-owned authentication/session and any other affected views after server-confirmed success.

The library does not provide any HTTP endpoint, token, role policy, database, universal recovery credential, membership calculation or audit repository.

## Async and security boundaries

Catalogues use server search/paging. Search is debounced; obsolete query responses are ignored. Eligibility and effective-access requests are bound to their current selection and invalidated on close or change. Mutations prevent repeat submission and await server-confirmed results. Successful grant/revoke refreshes the assignment catalogue and invokes `DataChanged`. Unconfirmed failures retain selections, show errors and do not report success.

**Application responsibility:** every grant/revoke/bootstrap/recovery operation revalidates the caller, the target and protected-final-administrator constraints at write time. Final-administrator protection and one-time bootstrap are atomic database/security decisions. Exposing a UI route is never a security grant. Information returned to the component must already be authorised for the caller. The library cannot assert that a revoked assignment instantly changes an existing authenticated session or external group membership.

## Synthetic reference integration

The public gallery workbench's `access-management` preview is a provider-independent consuming application. It supplies synthetic principal records, group grants, role definitions, typed asynchronous query and mutation callbacks, effective-access provenance, audit records and lifecycle inputs. Its demonstration callbacks simulate a server; they are **not** security controls to copy into production.

## Delivery, tests, version

Test grant/revoke and lifecycle confirmation including failure and stale responses, no implicit assignment revocation, capability changes, denied actions, and no admin prerequisite for bootstrap presentation. Update the governed workbench and publish both NuGet and compiled public demo. Verify public `build.txt` equals the merged source commit. No EAM-specific roles, SIDs, internal endpoints or real NHS operational information may appear in the public demo.

OVP-000 remains unchanged. EAM adoption is an independent application activity.
