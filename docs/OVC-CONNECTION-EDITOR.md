# OVC-CONNECTION-EDITOR — Connection Editor

## Purpose

`OVConnectionEditor` is the governed reusable editor for managed OneVision connection definitions. It owns connection-type selection, transport-specific control composition, editor state and Test/Save/Cancel interaction grammar. Applications own persistence, credentials, connection-string construction, connection execution and authorization.

## Supported connection types

- SQL
- FILE
- LDAP
- JSON

SQL-specific settings are rendered only while `OVConnectionType.Sql` is selected. SQL settings must not leak into FILE, LDAP, JSON or future non-SQL editor surfaces.

## SQL transport security

SQL connections expose two governed Boolean settings on `OVSqlConnectionSettings`:

- `Encrypt`
- `TrustServerCertificate`

The governed defaults are:

```text
Encrypt = true
TrustServerCertificate = false
```

Historical SQL connection payloads that omit these properties therefore retain encrypted transport with normal certificate-chain validation.

### Encrypt connection

`Encrypt` is presented as **Encrypt connection** using the governed toggle control. It determines whether the consuming application should request SQL transport encryption.

Changing the setting emits a new `OVConnectionEditorValue` through `ValueChanged`, so the owning application can use the same dirty-state mechanism as other editor fields.

### Trust server certificate

`TrustServerCertificate` is presented as **Trust server certificate** immediately beneath the encryption setting.

Supporting text is governed as:

> Accept the SQL Server certificate without validating its certificate chain.

The control is disabled when `Encrypt` is false. The value concerns certificate validation; enabling it does not mean encryption is disabled and it is not the recommended default configuration.

Changing the setting emits a new `OVConnectionEditorValue` through `ValueChanged`.

## Test connection boundary

`TestRequested` receives the current governed `OVConnectionEditorValue`, including the current SQL `Encrypt` and `TrustServerCertificate` values. OneVision.UI does not construct or execute a SQL connection string.

A consuming application is expected to map the current value to its SQL client configuration. For example:

```text
Encrypt = true
TrustServerCertificate = true
```

corresponds conceptually to:

```text
Encrypt=True;TrustServerCertificate=True;
```

When `Encrypt` is false, `TrustServerCertificate` must not influence the consuming application's generated SQL connection behaviour.

## Ownership

OneVision.UI owns:

- labels and terminology;
- defaults;
- SQL-only conditional visibility;
- enabled/disabled presentation;
- editor value propagation;
- Test Connection action presentation;
- responsive layout.

The consuming application owns:

- persistence;
- secure credential handling;
- SQL connection-string construction;
- connection execution;
- authorization;
- application/domain validation beyond the governed UI contract.

Applications must not create parallel SQL TLS controls outside `OVConnectionEditor`.

## Gallery states

The reference gallery demonstrates:

- `SQL` — encrypted transport with normal certificate validation;
- `SQL trusted certificate` — encryption enabled with certificate-chain validation bypass explicitly enabled;
- `SQL encryption disabled` — encryption off with Trust server certificate disabled;
- existing FILE, LDAP, JSON, testing, success, error and read-only states.
