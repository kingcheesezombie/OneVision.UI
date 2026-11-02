using Bunit;
using Microsoft.AspNetCore.Components;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class ProgressiveConfigurationBuilderContractTests
{
    [Fact]
    public void Step_contract_exposes_semantic_configuration_parameters()
    {
        var parameters = typeof(OVConfigurationStep)
            .GetProperties()
            .Where(property => Attribute.IsDefined(property, typeof(ParameterAttribute)))
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);

        foreach (var expected in new[]
        {
            nameof(OVConfigurationStep.StepNumber),
            nameof(OVConfigurationStep.Label),
            nameof(OVConfigurationStep.Required),
            nameof(OVConfigurationStep.IsConditional),
            nameof(OVConfigurationStep.AllowAny),
            nameof(OVConfigurationStep.IsAny),
            nameof(OVConfigurationStep.AnyLabel),
            nameof(OVConfigurationStep.SelectedValue),
            nameof(OVConfigurationStep.HelpText),
            nameof(OVConfigurationStep.State),
            nameof(OVConfigurationStep.CanChange),
            nameof(OVConfigurationStep.IsVisible),
            nameof(OVConfigurationStep.ValidationState),
            nameof(OVConfigurationStep.ValidationMessage),
            nameof(OVConfigurationStep.Editor),
            nameof(OVConfigurationStep.ChangeRequested)
        })
            Assert.Contains(expected, parameters);
    }

    [Fact]
    public void Step_states_are_governed_and_domain_neutral()
    {
        Assert.Equal(new[]
        {
            OVConfigurationStepState.Pending,
            OVConfigurationStepState.Current,
            OVConfigurationStepState.Completed,
            OVConfigurationStepState.Disabled,
            OVConfigurationStepState.Error
        }, Enum.GetValues<OVConfigurationStepState>());
    }

    [Fact]
    public void Required_current_step_renders_current_semantics()
    {
        using var context = new BunitContext();
        var cut = context.Render<OVConfigurationStep>(parameters => parameters
            .Add(p => p.StepNumber, 1)
            .Add(p => p.Label, "Class")
            .Add(p => p.Required, true)
            .Add(p => p.State, OVConfigurationStepState.Current)
            .Add<RenderFragment>(p => p.Editor, builder => builder.AddContent(0, "Editor content")));

        Assert.Contains("Required", cut.Markup);
        Assert.Contains("aria-current=\"step\"", cut.Markup);
        Assert.Contains("Editor content", cut.Markup);
    }

    [Fact]
    public void Any_selection_is_a_completed_optional_value_not_an_error()
    {
        using var context = new BunitContext();
        var cut = context.Render<OVConfigurationStep>(parameters => parameters
            .Add(p => p.StepNumber, 2)
            .Add(p => p.Label, "Source")
            .Add(p => p.AllowAny, true)
            .Add(p => p.IsAny, true)
            .Add(p => p.AnyLabel, "Any source")
            .Add(p => p.State, OVConfigurationStepState.Completed)
            .Add(p => p.CanChange, false));

        Assert.Contains("Optional", cut.Markup);
        Assert.Contains("Any source", cut.Markup);
        Assert.Contains("Completed", cut.Markup);
        Assert.DoesNotContain("aria-invalid=\"true\"", cut.Markup);
    }

    [Fact]
    public void Conditional_disabled_step_exposes_required_conditional_and_disabled_semantics()
    {
        using var context = new BunitContext();
        var cut = context.Render<OVConfigurationStep>(parameters => parameters
            .Add(p => p.StepNumber, 5)
            .Add(p => p.Label, "Target type")
            .Add(p => p.Required, true)
            .Add(p => p.IsConditional, true)
            .Add(p => p.State, OVConfigurationStepState.Disabled));

        Assert.Contains("Required", cut.Markup);
        Assert.Contains("Conditional", cut.Markup);
        Assert.Contains("Unavailable", cut.Markup);
        Assert.Contains("aria-disabled=\"true\"", cut.Markup);
    }

    [Fact]
    public void Invisible_conditional_step_does_not_render()
    {
        using var context = new BunitContext();
        var cut = context.Render<OVConfigurationStep>(parameters => parameters
            .Add(p => p.StepNumber, 5)
            .Add(p => p.Label, "Conditional target type")
            .Add(p => p.IsConditional, true)
            .Add(p => p.IsVisible, false));

        Assert.True(string.IsNullOrWhiteSpace(cut.Markup));
    }

    [Fact]
    public void Error_step_exposes_invalid_semantics_and_message()
    {
        using var context = new BunitContext();
        var cut = context.Render<OVConfigurationStep>(parameters => parameters
            .Add(p => p.StepNumber, 3)
            .Add(p => p.Label, "Source")
            .Add(p => p.State, OVConfigurationStepState.Error)
            .Add(p => p.ValidationState, OVConfigurationValidationState.Error)
            .Add(p => p.ValidationMessage, "Choose a source."));

        Assert.Contains("aria-invalid=\"true\"", cut.Markup);
        Assert.Contains("role=\"alert\"", cut.Markup);
        Assert.Contains("Choose a source.", cut.Markup);
    }

    [Fact]
    public void Preview_supports_incomplete_state_and_live_region()
    {
        using var context = new BunitContext();
        var cut = context.Render<OVConfigurationPreview>(parameters => parameters
            .Add(p => p.Title, "Rule preview")
            .Add(p => p.Incomplete, true)
            .Add<RenderFragment>(p => p.ChildContent, builder => builder.AddContent(0, "ASSET → [TARGET]")));

        Assert.Contains("Rule preview", cut.Markup);
        Assert.Contains("Complete the remaining steps", cut.Markup);
        Assert.Contains("aria-live=\"polite\"", cut.Markup);
        Assert.Contains("ASSET → [TARGET]", cut.Markup);
    }
}
