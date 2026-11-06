namespace OneVision.UI.Components;

/// <summary>Behaviour when a source-supplied primary identifier has no value.</summary>
public enum OVIdentifierFallbackStrategy
{
    RejectRecord,
    AlternativeField,
    Deterministic
}

/// <summary>
/// Consumer-authoritative edit permission. Unknown is deliberately non-editable.
/// Transient Saving and profile state are supplied separately.
/// </summary>
public enum OVIdentifierEditState
{
    Unknown,
    Editable,
    ReadOnly,
    Locked
}

/// <summary>
/// Persistent identity definition. Inactive alternative/derivation fields are
/// retained as draft settings when the mode changes, never applied implicitly.
/// Lock, profile, validation and preview state are not persisted in this model.
/// </summary>
public sealed record OVIdentifierDefinition
{
    public bool SourceProvidesIdentifier { get; init; }
    public string? IdentifierFieldKey { get; init; }
    public OVIdentifierFallbackStrategy FallbackStrategy { get; init; } = OVIdentifierFallbackStrategy.RejectRecord;
    public string? AlternativeFieldKey { get; init; }
    public string? DerivationStrategyKey { get; init; }
    public IReadOnlyList<string> DerivationFieldKeys { get; init; } = Array.Empty<string>();
}

/// <summary>Source-profile field eligibility is supplied by the application.</summary>
public sealed record OVIdentifierFieldOption(
    string Key,
    string Label,
    bool PrimaryEligible = true,
    bool AlternativeEligible = true,
    bool DerivationEligible = true,
    bool Disabled = false);

/// <summary>Application-owned deterministic strategy, never executed in OneVision.UI.</summary>
public sealed record OVIdentifierDerivationStrategyOption(
    string Key,
    string Label,
    string? Description = null,
    bool AllowsMultipleFields = false,
    int MinimumFields = 1,
    int? MaximumFields = null,
    bool Disabled = false);

/// <summary>
/// The consuming application must only enable deterministic fallback when it
/// can enforce identity continuity between derived and primary identities.
/// </summary>
public sealed record OVIdentifierCapabilities(
    bool AllowAlternativeFieldFallback = false,
    bool AllowDeterministicFallback = false);

/// <summary>Optional app-provided identity-safety details. No claims are inferred.</summary>
public sealed record OVIdentifierSafetyInformation(
    string? InputStability = null,
    string? Uniqueness = null,
    string? CollisionHandling = null,
    string? IdentityContinuity = null);

/// <summary>An application-provided validation result, alongside structural checks.</summary>
public sealed record OVIdentifierValidationIssue(string Code, string Message);

public enum OVIdentifierPreviewState
{
    NotConfigured,
    AwaitingInputs,
    Calculating,
    Available,
    Unavailable,
    Invalid
}

/// <summary>
/// Application-produced preview from actual derivation logic. ConfigurationKey
/// must match the current application-supplied PreviewConfigurationKey to show
/// a result, preventing an old asynchronous response replacing a new preview.
/// </summary>
public sealed record OVIdentifierPreviewResult(
    string ConfigurationKey,
    OVIdentifierPreviewState State,
    string? SourceInput = null,
    string? Identifier = null,
    string? Message = null);

/// <summary>
/// Structural policy checks only: actual uniqueness, canonicalisation, identity
/// continuity, save eligibility and runtime identity generation are app-owned.
/// </summary>
public static class OVIdentifierPolicyValidation
{
    public static IReadOnlyList<OVIdentifierValidationIssue> Assess(
        OVIdentifierDefinition? definition,
        IReadOnlyList<OVIdentifierFieldOption> fields,
        IReadOnlyList<OVIdentifierDerivationStrategyOption> strategies,
        OVIdentifierCapabilities capabilities)
    {
        var issues = new List<OVIdentifierValidationIssue>();
        if (definition is null)
        {
            issues.Add(new("policy", "The consuming application has not supplied an identifier policy."));
            return issues;
        }

        bool FieldEligible(string? key, Func<OVIdentifierFieldOption, bool> isEligible) =>
            !string.IsNullOrWhiteSpace(key) &&
            fields.Any(field => string.Equals(field.Key, key, StringComparison.Ordinal) &&
                                 !field.Disabled && isEligible(field));

        var requiresDerivation = !definition.SourceProvidesIdentifier;
        if (definition.SourceProvidesIdentifier)
        {
            if (!FieldEligible(definition.IdentifierFieldKey, field => field.PrimaryEligible))
                issues.Add(new("primary", string.IsNullOrWhiteSpace(definition.IdentifierFieldKey)
                    ? "Select the primary identifier source field."
                    : "The configured primary identifier field is unavailable or ineligible."));

            switch (definition.FallbackStrategy)
            {
                case OVIdentifierFallbackStrategy.RejectRecord:
                    break;
                case OVIdentifierFallbackStrategy.AlternativeField:
                    if (!capabilities.AllowAlternativeFieldFallback)
                        issues.Add(new("fallback-policy", "Alternative-field fallback is not permitted by this source."));
                    if (!FieldEligible(definition.AlternativeFieldKey, field => field.AlternativeEligible))
                        issues.Add(new("alternative", string.IsNullOrWhiteSpace(definition.AlternativeFieldKey)
                            ? "Select an alternative identifier field."
                            : "The alternative identifier field is unavailable or ineligible."));
                    else if (string.Equals(definition.IdentifierFieldKey, definition.AlternativeFieldKey, StringComparison.Ordinal))
                        issues.Add(new("alternative", "The alternative identifier field must differ from the primary identifier field."));
                    break;
                case OVIdentifierFallbackStrategy.Deterministic:
                    requiresDerivation = true;
                    if (!capabilities.AllowDeterministicFallback)
                        issues.Add(new("fallback-policy", "Deterministic fallback is not authorised for this source."));
                    break;
                default:
                    issues.Add(new("fallback-policy", "The saved fallback strategy is unsupported."));
                    break;
            }
        }

        if (requiresDerivation)
        {
            var strategy = strategies.FirstOrDefault(item =>
                !item.Disabled && string.Equals(item.Key, definition.DerivationStrategyKey, StringComparison.Ordinal));
            if (strategy is null)
                issues.Add(new("strategy", string.IsNullOrWhiteSpace(definition.DerivationStrategyKey)
                    ? "Select a deterministic derivation strategy."
                    : "The configured derivation strategy is unavailable."));

            var keys = definition.DerivationFieldKeys ?? Array.Empty<string>();
            if (keys.Count == 0 || keys.Count < Math.Max(1, strategy?.MinimumFields ?? 1))
                issues.Add(new("derivation-fields", "Select the required source field or fields for derivation."));
            else
            {
                if (keys.Distinct(StringComparer.Ordinal).Count() != keys.Count)
                    issues.Add(new("derivation-fields", "Derivation inputs must not contain duplicate fields."));
                if (keys.Any(key => !FieldEligible(key, field => field.DerivationEligible)))
                    issues.Add(new("derivation-fields", "One or more configured derivation fields are unavailable or ineligible."));
            }

            if (strategy is not null)
            {
                if (!strategy.AllowsMultipleFields && keys.Count > 1)
                    issues.Add(new("derivation-fields", "The selected strategy supports only one derivation field."));
                if (strategy.MaximumFields is int max && keys.Count > max)
                    issues.Add(new("derivation-fields", "Too many derivation fields are selected for the strategy."));
            }
        }

        return issues;
    }
}
