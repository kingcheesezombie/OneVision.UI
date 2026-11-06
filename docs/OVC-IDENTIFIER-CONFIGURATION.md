# OVC-IDENTIFIER-CONFIGURATION — Governed identifier policy (0.1.47)

**Classification:** reusable semantic component, integrated as an opt-in
capability of `OVP-SOURCE-CONFIGURATION`.

**Ownership:** OneVision.UI presents and edits a configured identity *policy*.
The consuming application owns source profiling, persistence, canonicalisation,
identity generation, uniqueness, continuity, historical identity locking and
save eligibility. This component does not contain an identity algorithm or
application-specific ingestion decisions.

## Governed headline presentation (0.1.47)

When the optional identifier capability is enabled, the Mapping tab presents
Identifier Configuration as the prominent, visually contained policy headline.
Its primary-blue heading displays the edit state, while the editable toggle,
source identifier and missing-identifier action remain together in a compact,
responsive two-column layout. Application-generated preview values remain
compact and read-only. The subsequent Field Mapping section receives a separate
heading and contained workspace, but the application-owned mapping fragment,
mappings and behaviour are unchanged.

The header uses themed primary styling, wraps on narrow screens and preserves
read-only/locked/unknown/stale/saving states and explicit validation. This is a
presentation-only refinement and does not alter the identity policy or the
OVP-000 foundation contract.

## Application-controlled definition

`OVIdentifierDefinition` is the persisted semantic definition:

| Property | Active meaning |
|---|---|
| `SourceProvidesIdentifier` | Explicit application-supplied source identifier availability, never inferred from mappings |
| `IdentifierFieldKey` | Primary source field when a source identifier exists |
| `FallbackStrategy` | RejectRecord (default), AlternativeField, or Deterministic when permitted |
| `AlternativeFieldKey` | Alternative source field, used only in AlternativeField fallback |
| `DerivationStrategyKey` | Application-provided deterministic strategy key |
| `DerivationFieldKeys` | Application-provided source inputs, including multi-field composition where allowed |

The definition is controlled via `Definition` and
`EventCallback<OVIdentifierDefinition> DefinitionChanged`. The component does
not persist or mutate caller data behind the callback. The consumer must provide
the initial `SourceProvidesIdentifier` value explicitly.

### Inactive draft policy

Switching between source-supplied and derived identifiers, or changing the
fallback strategy, **retains** configured but inapplicable fields as inactive
draft values. They do not affect the currently active policy, validation or
preview. This allows users to explore options without losing work.

Do **not** interpret inactive draft values as runtime fallback rules. Consumers
must execute only the active policy, and the backend must validate it.

## Fallback behaviour

- `RejectRecord`: no valid primary identifier means the record is rejected.
- `AlternativeField`: if the primary value is missing, use the explicitly
  configured alternative. If the alternative is also missing, reject. The same
  field cannot be both primary and alternative.
- `Deterministic`: when the primary identifier is missing, derive from the
  configured strategy and inputs; if derivation fails, reject.

Invalid primary values are **not** automatically equivalent to missing values.
The consuming application's explicit validation policy determines whether they
trigger fallback or rejection. The UI does not chain hidden fallbacks.

`OVIdentifierCapabilities` gates alternative-field and deterministic
fallback independently. Deterministic fallback in a source with primary IDs
must be explicitly authorised **only when the consumer can guarantee
identity continuity**. The UI always displays a continuity warning for this
mode and rejects a saved unsupported fallback in structural validation.

## Deterministic derivation and safety

The application supplies `OVIdentifierDerivationStrategyOption` and
`OVIdentifierFieldOption` with stable keys and eligibility. A strategy
declares whether multiple fields are supported and its required input count.

The component validates *structural* completeness and references:

- required primary field when `SourceProvidesIdentifier=true`;
- allowed fallback and required/different alternative field;
- valid strategy and eligible derivation inputs when deriving;
- no duplicate, removed, disabled or ineligible derivation input;
- required field count, single-field and maximum-field constraints.

`OVIdentifierPolicyValidation.Assess(...)` produces structural issues.
`ValidationIssues` adds application-owned semantic or runtime errors.
Neither acts as the authoritative save decision.

A deterministic identifier is not necessarily stable or unique. Source input
edits can change the derived identifier, and distinct records can collide.
Applications can pass `OVIdentifierSafetyInformation` to describe their
input stability, uniqueness, collision handling and identity continuity.
OneVision.UI never claims assurance when the application has supplied none.

A source-record identifier must not silently be conflated with a canonical
entity identity. Applications retain ownership of any resolution process.

## Operational edit and lock state

`OVIdentifierEditState` is separate from the stored definition:

- `Unknown` (the default): **non-editable** until authority is confirmed.
- `Editable`: edit only when the profile is Current and the source is not Saving.
- `ReadOnly`: inspect the saved policy, but no identity edits.
- `Locked`: show all saved policy values, reason and preview without any
  edit action or Unlock control.

The consumer supplies `EditState`, `LockReason`, `Saving` and
`ProfileState`. Priority is: Locked > unknown permission > Saving > stale or
unavailable profile > explicit read-only > editable. Locking applies only to
the Identifier Configuration; other app-supplied Source Configuration
sections stay under their own app-specific permissions.

Handlers guard updates when not editable, in addition to disabling controls.
The authoritative backend must also reject forbidden identity mutations.
An identity migration/unlock workflow is separate and is never implied here.

Stale or removed fields remain visible as unresolved saved selections.
The component does not substitute new fields or discard identity policy
without an explicit application-controlled change.

## Application-generated identity preview

The application supplies `Preview` (an `OVIdentifierPreviewResult`) and
`PreviewConfigurationKey`. A preview is rendered only when its non-empty
`ConfigurationKey` exactly matches `PreviewConfigurationKey`. Results from
older asynchronous calculations are not shown after the input policy changes.
The consumer must increment or regenerate the configuration key whenever
effective input, strategy, or sample values change and must discard stale
runtime responses as appropriate.

Preview states are NotConfigured, AwaitingInputs, Calculating, Available,
Unavailable and Invalid. No preview or a mismatched key shows the explicit
unavailable/pending state. Available results display app-supplied source input
and identifier with the governed `OVCopyableLink`. A single sample preview
is informational; it does not prove uniqueness or imply saved/enabled state.

**Never** implement a parallel hash or canonicalisation algorithm in
OneVision.UI. The gallery's synthetic example uses a demonstrative
application-owned SHA-256 transformation for sample fields only.

## Composition and backward compatibility

Opt into the Source Configuration mapping composition with
`IdentifierConfigurationEnabled=true`. The pattern renders
`OVIdentifierConfiguration` **before** the existing application-owned
`Mapping` fragment, without adding a new top-level tab. No
`OVMappingPair` target is silently added or changed. The consumer must
ensure that an existing identifier field mapping does not become a second
independently editable identity policy.

When the profile is stale or unavailable, the saved identifier policy remains
visible read-only; the existing ordinary Mapping fragment retains the
preexisting stale/unavailable treatment. When the capability is omitted (the
default), the existing four/five tab layout and Mapping rendering remain
unchanged.

Example consumption (the application supplies its own states and callbacks):

```razor
<OVSourceConfiguration Title="Source"
    ProfileState="@ProfileState"
    IdentifierConfigurationEnabled="true"
    IdentifierDefinition="@Identity"
    IdentifierDefinitionChanged="UpdateIdentity"
    IdentifierFields="@EligibleFields"
    IdentifierStrategies="@DerivationStrategies"
    IdentifierCapabilities="@IdentityCapabilities"
    IdentifierEditState="@AuthoritativeIdentityEditState"
    IdentifierLockReason="@IdentityLockReason"
    IdentifierSafetyInformation="@SafetyInformation"
    IdentifierPreviewConfigurationKey="@CurrentPreviewKey"
    IdentifierPreview="@DerivedPreview">
    <Mapping>@ApplicationMapping</Mapping>
</OVSourceConfiguration>
```

The caller's `UpdateIdentity` must apply controlled changes and invalidate
the previous preview key. Only the caller performs derivation and persistence.

## Required release evidence

Unit tests: explicit and derived modes; capability gating and invalid saved
fallback; source-field changes and unresolved references; controlled
callbacks and draft retention; locked/read-only/unknown/saving/stale precedence;
app preview correlation; inactive legacy behaviour.

Browser tests: interaction and keyboard navigation; screen-reader labels,
validation and locked explanations; 1280/760/390px layouts and no horizontal
overflow; all gallery scenario combinations, stale profile and copyable result.
Full CI, source merge, NuGet and compiled demo publication are separate gates.
No EAM/IDM/HDM/KPI adoption is part of this release.
