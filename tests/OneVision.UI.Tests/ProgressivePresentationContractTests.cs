using Bunit;
using Microsoft.AspNetCore.Components;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class ProgressivePresentationContractTests
{
    [Fact]
    public void Progressive_builder_defaults_to_full_width_and_allows_governed_constrained_mode()
    {
        using var context = new BunitContext();
        var cut = context.Render<OVProgressiveConfigurationBuilder>(p => p
            .Add<RenderFragment>(x => x.ChildContent!, b => b.AddContent(0, "Configuration")));

        Assert.Equal(OVProgressiveBuilderLayout.FullWidth, cut.Instance.Layout);
        Assert.Equal("fullwidth", cut.Find(".ov-progressive-builder").GetAttribute("data-layout"));
        Assert.Contains("ov-progressive-builder--fullwidth", cut.Markup);

        cut.Render(p => p
            .Add(x => x.Layout, OVProgressiveBuilderLayout.Constrained)
            .Add<RenderFragment>(x => x.ChildContent!, b => b.AddContent(0, "Configuration")));

        Assert.Equal("constrained", cut.Find(".ov-progressive-builder").GetAttribute("data-layout"));
        Assert.Contains("ov-progressive-builder--constrained", cut.Markup);
    }

    [Fact]
    public void Standard_preview_retains_existing_default_and_incomplete_behaviour()
    {
        using var context = new BunitContext();
        var cut = context.Render<OVConfigurationPreview>(p => p
            .Add(x => x.Title, "Existing summary")
            .Add(x => x.Incomplete, true)
            .Add<RenderFragment>(x => x.ChildContent, b => b.AddContent(0, "Existing application content")));

        Assert.Equal(OVConfigurationPreviewAppearance.Standard, cut.Instance.Appearance);
        Assert.Contains("ov-configuration-preview--standard", cut.Markup);
        Assert.Contains("Complete the remaining steps", cut.Markup);
        Assert.DoesNotContain("ov-configuration-preview__state ", cut.Markup);
        Assert.Contains("Existing application content", cut.Markup);
    }

    [Theory]
    [InlineData(OVConfigurationPreviewState.Incomplete, "incomplete", "Incomplete")]
    [InlineData(OVConfigurationPreviewState.ReadyToSave, "readytosave", "Ready to save")]
    [InlineData(OVConfigurationPreviewState.Neutral, "neutral", "")]
    public void Emphasized_preview_exposes_only_the_explicit_informational_state(
        OVConfigurationPreviewState state, string expectedState, string label)
    {
        using var context = new BunitContext();
        var cut = context.Render<OVConfigurationPreview>(p => p
            .Add(x => x.Appearance, OVConfigurationPreviewAppearance.Emphasized)
            .Add(x => x.State, state)
            .Add(x => x.Title, "Rule summary")
            .Add(x => x.Description, "Configured relationship")
            .Add<RenderFragment>(x => x.ChildContent, b => b.AddContent(0, "Source → Destination")));

        Assert.Equal(expectedState, cut.Find(".ov-configuration-preview").GetAttribute("data-preview-state"));
        Assert.Contains("ov-configuration-preview--emphasized", cut.Markup);
        Assert.Contains("Configured relationship", cut.Markup);
        Assert.Contains("Source → Destination", cut.Markup);
        if (state == OVConfigurationPreviewState.Neutral)
            Assert.Empty(cut.FindAll(".ov-configuration-preview__state"));
        else
            Assert.Contains(label, cut.Find(".ov-configuration-preview__state").TextContent);
        Assert.DoesNotContain("Configuration saved", cut.Markup);
    }

    [Fact]
    public void Existing_incomplete_flag_overrides_a_conflicting_ready_state()
    {
        using var context = new BunitContext();
        var cut = context.Render<OVConfigurationPreview>(p => p
            .Add(x => x.Appearance, OVConfigurationPreviewAppearance.Emphasized)
            .Add(x => x.Incomplete, true)
            .Add(x => x.State, OVConfigurationPreviewState.ReadyToSave)
            .Add<RenderFragment>(x => x.ChildContent, b => b.AddContent(0, "In progress")));

        Assert.Equal("incomplete", cut.Find(".ov-configuration-preview").GetAttribute("data-preview-state"));
        Assert.Contains("Incomplete", cut.Find(".ov-configuration-preview__state").TextContent);
        Assert.DoesNotContain("Ready to save", cut.Markup);
    }
}
