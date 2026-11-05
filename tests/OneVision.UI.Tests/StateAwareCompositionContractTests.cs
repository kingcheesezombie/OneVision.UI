using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

/// <summary>
/// Reusable layout state is available to consuming content without globally
/// disabling arbitrary actions such as Help, Copy, Refresh or Cancel.
/// </summary>
public sealed class StateAwareCompositionContractTests
{
    [Fact]
    public async Task Panel_passes_read_only_to_named_header_and_content_fragments()
    {
        await using var context = Context();
        RenderFragment<bool> header = readOnly => builder =>
        {
            builder.OpenComponent<OVButton>(0);
            builder.AddAttribute(1, nameof(OVButton.Disabled), readOnly);
            builder.AddAttribute(2, nameof(OVButton.ChildContent), (RenderFragment)(b => b.AddContent(0, "Edit")));
            builder.CloseComponent();
        };
        RenderFragment<bool> content = readOnly => builder =>
        {
            builder.OpenComponent<OVTextField>(0);
            builder.AddAttribute(1, nameof(OVTextField.Label), "Notes");
            builder.AddAttribute(2, nameof(OVTextField.ReadOnly), readOnly);
            builder.CloseComponent();
        };

        var locked = context.Render<OVPanel>(p => p
            .Add(x => x.ReadOnly, true)
            .Add(x => x.ReadOnlyHeader, header)
            .Add(x => x.ReadOnlyContent, content));
        Assert.True(locked.FindComponent<OVButton>().Instance.Disabled);
        Assert.True(locked.FindComponent<OVTextField>().Instance.ReadOnly);

        var editable = context.Render<OVPanel>(p => p
            .Add(x => x.ReadOnly, false)
            .Add(x => x.ReadOnlyHeader, header)
            .Add(x => x.ReadOnlyContent, content));
        Assert.False(editable.FindComponent<OVButton>().Instance.Disabled);
        Assert.False(editable.FindComponent<OVTextField>().Instance.ReadOnly);
    }

    [Fact]
    public async Task Toolbar_passes_read_only_to_leading_and_trailing_actions()
    {
        await using var context = Context();
        RenderFragment<bool> input = readOnly => builder =>
        {
            builder.OpenComponent<OVSearchBox>(0);
            builder.AddAttribute(1, nameof(OVSearchBox.ReadOnly), readOnly);
            builder.CloseComponent();
        };
        RenderFragment<bool> action = readOnly => builder =>
        {
            builder.OpenComponent<OVButton>(0);
            builder.AddAttribute(1, nameof(OVButton.Disabled), readOnly);
            builder.CloseComponent();
        };
        var cut = context.Render<OVToolbar>(p => p
            .Add(x => x.ReadOnly, true)
            .Add(x => x.ReadOnlyLeading, input)
            .Add(x => x.ReadOnlyTrailing, action));

        Assert.True(cut.FindComponent<OVSearchBox>().Instance.ReadOnly);
        Assert.True(cut.FindComponent<OVButton>().Instance.Disabled);
        Assert.NotNull(cut.Find("[role=toolbar][aria-label='Workspace toolbar']"));
    }

    [Fact]
    public async Task Action_bar_passes_busy_to_actions_without_forcing_all_content_disabled()
    {
        await using var context = Context();
        RenderFragment<bool> actions = busy => builder =>
        {
            builder.OpenComponent<OVButton>(0);
            builder.AddAttribute(1, nameof(OVButton.Disabled), busy);
            builder.CloseComponent();
        };
        var cut = context.Render<OVActionBar>(p => p
            .Add(x => x.Busy, true)
            .Add(x => x.BusyActions, actions));

        Assert.True(cut.FindComponent<OVButton>().Instance.Disabled);
        Assert.Equal("true", cut.Find("[role=region]").GetAttribute("aria-busy"));
    }

    [Fact]
    public async Task Legacy_fragments_keep_rendering_without_state_aware_slots()
    {
        await using var context = Context();
        RenderFragment fragment = b => b.AddContent(0, "Legacy content");
        var panel = context.Render<OVPanel>(p => p
            .Add(x => x.ReadOnly, true)
            .Add(x => x.ChildContent, fragment));
        Assert.Contains("Legacy content", panel.Markup);

        var toolbar = context.Render<OVToolbar>(p => p
            .Add(x => x.ReadOnly, true)
            .Add(x => x.Leading, fragment));
        Assert.Contains("Legacy content", toolbar.Markup);

        var action = context.Render<OVActionBar>(p => p
            .Add(x => x.Busy, true)
            .Add(x => x.Actions, fragment));
        Assert.Contains("Legacy content", action.Markup);
    }

    private static BunitContext Context()
    {
        var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }
}
