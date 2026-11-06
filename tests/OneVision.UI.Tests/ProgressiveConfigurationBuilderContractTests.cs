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
            .Add<RenderFragment>(p => p.Editor!, builder => builder.AddContent(0, "Editor content")));

        Assert.Contains("Required", cut.Markup);
        Assert.Contains("Current", cut.Markup);
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
    public void Conditional_required_step_exposes_combined_semantics_and_disabled_state()
    {
        using var context = new BunitContext();
        var cut = context.Render<OVConfigurationStep>(parameters => parameters
            .Add(p => p.StepNumber, 5)
            .Add(p => p.Label, "Target type")
            .Add(p => p.Required, true)
            .Add(p => p.IsConditional, true)
            .Add(p => p.State, OVConfigurationStepState.Disabled));

        Assert.Contains("Required · Conditional", cut.Markup);
        Assert.Contains("Unavailable", cut.Markup);
        Assert.Contains("aria-disabled=\"true\"", cut.Markup);
    }

    [Fact]
    public void Conditional_optional_step_supports_completed_any_selection()
    {
        using var context = new BunitContext();
        var cut = context.Render<OVConfigurationStep>(parameters => parameters
            .Add(p => p.StepNumber, 5)
            .Add(p => p.Label, "Target type")
            .Add(p => p.IsConditional, true)
            .Add(p => p.AllowAny, true)
            .Add(p => p.IsAny, true)
            .Add(p => p.AnyLabel, "Any target type")
            .Add(p => p.State, OVConfigurationStepState.Completed));

        Assert.Contains("Optional · Conditional", cut.Markup);
        Assert.Contains("Any target type", cut.Markup);
        Assert.Contains("Completed", cut.Markup);
        Assert.DoesNotContain("aria-invalid=\"true\"", cut.Markup);
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

        Assert.Contains("Needs attention", cut.Markup);
        Assert.Contains("aria-invalid=\"true\"", cut.Markup);
        Assert.Contains("role=\"alert\"", cut.Markup);
        Assert.Contains("Choose a source.", cut.Markup);
    }

    [Fact]
    public void Untitled_builder_does_not_emit_invalid_labelledby_reference()
    {
        using var context = new BunitContext();
        var cut = context.Render<OVProgressiveConfigurationBuilder>(parameters => parameters
            .Add<RenderFragment>(p => p.ChildContent, builder => builder.AddContent(0, "Builder content")));

        var root = cut.Find(".ov-progressive-builder");
        Assert.False(root.HasAttribute("aria-labelledby"));
        Assert.False(root.HasAttribute("aria-label"));
    }

    [Fact]
    public void Untitled_builder_can_use_alternative_accessible_name()
    {
        using var context = new BunitContext();
        var cut = context.Render<OVProgressiveConfigurationBuilder>(parameters => parameters
            .Add(p => p.AriaLabel, "Allocation rule configuration")
            .Add<RenderFragment>(p => p.ChildContent, builder => builder.AddContent(0, "Builder content")));

        var root = cut.Find(".ov-progressive-builder");
        Assert.Equal("Allocation rule configuration", root.GetAttribute("aria-label"));
        Assert.False(root.HasAttribute("aria-labelledby"));
    }

    [Fact]
    public void Builder_and_section_titles_expose_configurable_heading_semantics()
    {
        using var context = new BunitContext();
        var builder = context.Render<OVProgressiveConfigurationBuilder>(parameters => parameters
            .Add(p => p.Title, "Rule builder")
            .Add(p => p.HeadingLevel, 3)
            .Add<RenderFragment>(p => p.ChildContent, fragment => fragment.AddContent(0, "Builder content")));
        var section = context.Render<OVConfigurationSection>(parameters => parameters
            .Add(p => p.Title, "Source population")
            .Add(p => p.HeadingLevel, 4)
            .Add<RenderFragment>(p => p.ChildContent, fragment => fragment.AddContent(0, "Section content")));

        var builderHeading = builder.Find(".ov-progressive-builder__title");
        Assert.Equal("heading", builderHeading.GetAttribute("role"));
        Assert.Equal("3", builderHeading.GetAttribute("aria-level"));
        var sectionHeading = section.Find(".ov-configuration-section__title");
        Assert.Equal("heading", sectionHeading.GetAttribute("role"));
        Assert.Equal("4", sectionHeading.GetAttribute("aria-level"));
    }

    [Fact]
    public void Completed_section_has_one_group_status_without_changing_step_semantics()
    {
        using var context = new BunitContext();
        var cut = context.Render<OVConfigurationSection>(parameters => parameters
            .Add(p => p.Title, "Source population")
            .Add(p => p.Completed, true)
            .Add<RenderFragment>(p => p.ChildContent, builder => builder.AddContent(0, "Source steps")));

        Assert.Single(cut.FindAll(".ov-configuration-section__complete"));
        Assert.Equal("Section complete", cut.Find(".ov-configuration-section__complete").GetAttribute("aria-label"));
        Assert.Contains("Source population", cut.Markup);
    }

    [Fact]
    public void Completed_builder_reports_success_as_compact_status()
    {
        using var context = new BunitContext();
        var cut = context.Render<OVProgressiveConfigurationBuilder>(parameters => parameters
            .Add(p => p.Completed, true)
            .Add(p => p.CompletedMessage, "Configuration saved.")
            .Add<RenderFragment>(p => p.ChildContent!, builder => builder.AddContent(0, "Retained selection")));

        Assert.Single(cut.FindAll(".ov-progressive-builder__saved"));
        Assert.Equal("status", cut.Find(".ov-progressive-builder__saved").GetAttribute("role"));
        Assert.Contains("Configuration saved.", cut.Markup);
        Assert.Contains("Retained selection", cut.Markup);
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
