using Bunit;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

/// <summary>
/// Source Configuration governs the Display Value editor during its save phase.
/// This deliberately does not alter application-owned Mapping or save eligibility.
/// </summary>
public sealed class SourceConfigurationSaveStateContractTests
{
    [Fact]
    public async Task Saving_source_makes_current_display_value_read_only_without_losing_its_tab()
    {
        await using var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;

        var cut = context.Render<OVSourceConfiguration>(p => p
            .Add(x => x.Title, "Source")
            .Add(x => x.ActiveSection, "display-value")
            .Add(x => x.ProfileState, OVSourceProfileState.Current)
            .Add(x => x.DisplayValueBuilderEnabled, true)
            .Add(x => x.Saving, true));

        var editor = cut.FindComponent<OVDisplayValueBuilder>();
        Assert.True(editor.Instance.ReadOnly);
        Assert.Contains("Read only", editor.Markup);
        Assert.Contains("Display Value", cut.Markup);
        Assert.Equal("true", cut.Find(".ov-source-configuration").GetAttribute("aria-busy"));
    }

    [Fact]
    public async Task Completing_save_restores_editability_under_the_same_user_policy()
    {
        await using var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;

        var cut = context.Render<OVSourceConfiguration>(p => p
            .Add(x => x.Title, "Source")
            .Add(x => x.ActiveSection, "display-value")
            .Add(x => x.ProfileState, OVSourceProfileState.Current)
            .Add(x => x.DisplayValueBuilderEnabled, true)
            .Add(x => x.Saving, true));

        Assert.True(cut.FindComponent<OVDisplayValueBuilder>().Instance.ReadOnly);
        cut.Render(p => p
            .Add(x => x.Title, "Source")
            .Add(x => x.ActiveSection, "display-value")
            .Add(x => x.ProfileState, OVSourceProfileState.Current)
            .Add(x => x.DisplayValueBuilderEnabled, true)
            .Add(x => x.Saving, false));
        Assert.False(cut.FindComponent<OVDisplayValueBuilder>().Instance.ReadOnly);
    }

    [Fact]
    public async Task Save_failure_retains_unsaved_configuration_and_does_not_force_read_only_mode()
    {
        await using var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;

        var cut = context.Render<OVSourceConfiguration>(p => p
            .Add(x => x.Title, "Source")
            .Add(x => x.ActiveSection, "display-value")
            .Add(x => x.ProfileState, OVSourceProfileState.Current)
            .Add(x => x.DisplayValueBuilderEnabled, true)
            .Add(x => x.Saving, false)
            .Add(x => x.SaveErrorMessage, "Save failed. Your edits were retained."));

        Assert.False(cut.FindComponent<OVDisplayValueBuilder>().Instance.ReadOnly);
        Assert.Contains("Save failed. Your edits were retained.", cut.Markup);
    }

    [Fact]
    public async Task Explicit_read_only_remains_authoritative_without_a_save_in_progress()
    {
        await using var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;

        var cut = context.Render<OVSourceConfiguration>(p => p
            .Add(x => x.Title, "Source")
            .Add(x => x.ActiveSection, "display-value")
            .Add(x => x.ProfileState, OVSourceProfileState.Current)
            .Add(x => x.DisplayValueBuilderEnabled, true)
            .Add(x => x.DisplayValueReadOnly, true));

        Assert.True(cut.FindComponent<OVDisplayValueBuilder>().Instance.ReadOnly);
    }
}
