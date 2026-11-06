using Microsoft.AspNetCore.Components;

namespace OneVision.UI.Components;

/// <summary>
/// An ordered item in a definition-driven progressive configuration. Keys must be unique
/// throughout the definition; they are not database identifiers.
/// </summary>
public abstract record OVConfigurationItemDefinition(string Key);

/// <summary>
/// A consumer-defined step. OneVision.UI owns the editor placement, numbered progression,
/// completion display and Change interaction; consumers own values, eligibility and validation.
/// Supply Options for a governed select editor or Editor for any other OneVision input.
/// </summary>
public sealed record OVConfigurationStepDefinition(string Key, string Label)
    : OVConfigurationItemDefinition(Key)
{
    public string? Value { get; init; }
    public string? DisplayValue { get; init; }
    public EventCallback<string?> ValueChanged { get; init; }
    public IReadOnlyList<OVOption<string>> Options { get; init; } = Array.Empty<OVOption<string>>();
    public RenderFragment? Editor { get; init; }
    public IReadOnlyList<string> DependsOn { get; init; } = Array.Empty<string>();
    public bool? Complete { get; init; }
    public bool Required { get; init; }
    public bool IsConditional { get; init; }
    public bool IsVisible { get; init; } = true;
    public bool Enabled { get; init; } = true;
    public bool CanChange { get; init; } = true;
    public bool AllowAny { get; init; }
    public string AnyValue { get; init; } = "*";
    public string AnyLabel { get; init; } = "Any";
    public string? HelpText { get; init; }
    public string? PendingText { get; init; }
    public string? DisabledText { get; init; }
    public OVConfigurationValidationState ValidationState { get; init; }
    public string? ValidationMessage { get; init; }
}

/// <summary>A named, optional group with any number of steps and separators.</summary>
public sealed record OVConfigurationSectionDefinition(
    string Key,
    string Title,
    IReadOnlyList<OVConfigurationItemDefinition> Items)
    : OVConfigurationItemDefinition(Key)
{
    public string? Description { get; init; }
}

/// <summary>
/// A presentation boundary between any two steps or sections.
/// Only the application supplies its optional relationship wording.
/// </summary>
public sealed record OVConfigurationSeparatorDefinition(
    string Key,
    OVConfigurationSeparatorStyle Style = OVConfigurationSeparatorStyle.LabelledArrow,
    string? Label = null)
    : OVConfigurationItemDefinition(Key);

public enum OVConfigurationSeparatorStyle
{
    None,
    Divider,
    LabelledDivider,
    Arrow,
    LabelledArrow
}
