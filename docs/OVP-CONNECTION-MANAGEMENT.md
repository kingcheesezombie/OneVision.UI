# OVP-CONNECTION-MANAGEMENT — Governed Connection Management

**Classification:** PATTERN; a composition of existing OneVision.UI primitives and the `OVC-CONNECTION-EDITOR` composite. It is **not** a standalone `OVConnectionManager` library class and must not be presented as one.

## Purpose

An application that manages reusable connection definitions composes:

`OVListPage → OVDataGrid/search/status filter → OVOverlay → OVConnectionEditor`

This governs a compact connection catalogue, initial loading/error/empty/filtered states, Add/Edit/Read-only entry, conditional SQL/FILE/LDAP/JSON fields, connection testing, save errors, dirty confirmation and return to the previous list state.

The list owns its title and Add action. The overlay owns its title, focus/dirty close and Save/Cancel footer. The embedded connection editor owns transport-specific fields, conditional authentication, safe test-result display and a **Test Connection** action; it must not render a second Save/Cancel footer. The catalogue retains its search and filter context after closing the overlay.

## Security and application boundary

Applications own connection identities, retrieval, server-side filtering/pagination where required, access permissions, endpoint test execution, test cancellation, network errors, persistence, version/conflict handling and credential storage. OneVision.UI **never logs, stores or publishes credentials**. Published demonstration records use synthetic `.invalid` endpoints and no secrets.

The governed editor does not construct connection strings or imply that enabling `TrustServerCertificate` removes transport encryption. The current SQL TLS default remains encrypted with server certificate validation.

The application must prevent stale connection-test responses from updating a different record's editor, disallow unsafe saves during tests, and avoid returning stored passwords to the browser for editing. Those are consumer integration acceptance criteria.

## Gallery quality scenarios

`/patterns/connection-management` renders the **actual** `OVListPage`, `OVDataGrid`, `OVOverlay` and `OVConnectionEditor`. It offers list load/empty/error/retry, read-only, Add, Edit, test error/retry, save error/retry and saving, plus independently controlled search, status filter, Add, row actions and Test Connection.

The gallery is a synthetic working reference composition, not an application service, end-user OneVision hub, general screen composer, or credential-storage solution. `OVConnectionEditor` remains the reusable implementation component; do not duplicate editor fields in applications or create an overlapping library control without demonstrated reusable behaviour.

## Level 0–5 review outcome

- Level 0: reuse governed fields, status pills, search, table, buttons and overlays; no copy of the previous Mud-specific mock.
- Level 1: reusable component `OVConnectionEditor` exposes Test Connection even when embedded, and suppresses duplicate Save/Cancel.
- Level 2: list → Add/Edit → Test/Save or cancel → return keeps list context and uses a single action owner.
- Level 3: EAM/IDM/HDM/KPI integrations remain independently verifiable; no consumer repository was modified here.
- Level 4: test and save failure retain draft values; stress/async cancellation and real network behaviour remain application-owned.
- Level 5: preserve OVP-000 foundation version, add browser and contract tests, release with exact source/package/demo SHA verification.
