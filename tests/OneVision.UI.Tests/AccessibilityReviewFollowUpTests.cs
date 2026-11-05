using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class AccessibilityReviewFollowUpTests
{
    [Fact]
    public async Task Displayed_timeline_and_process_stages_have_named_group_semantics()
    {
        await using var context = Context();
        var timeline = context.Render<OVTimeline>();
        var stages = context.Render<OVProcessStages>();

        Assert.NotNull(timeline.Find("div[role='group'][aria-label='Timeline']"));
        Assert.NotNull(stages.Find("div[role='group'][aria-label='Process stages']"));
    }

    [Fact]
    public async Task Selection_list_is_named_without_a_legend_and_uses_legend_when_provided()
    {
        await using var context = Context();
        var unnamed = context.Render<OVSelectionList>();
        Assert.Equal("Selection options", unnamed.Find("fieldset").GetAttribute("aria-label"));

        var named = context.Render<OVSelectionList>(p => p.Add(x => x.Label, "Available departments"));
        Assert.Null(named.Find("fieldset").GetAttribute("aria-label"));
        Assert.Equal("Available departments", named.Find("legend").TextContent);
    }

    [Fact]
    public async Task Notification_host_exposes_one_named_region_containing_notifications()
    {
        await using var context = Context();
        context.Services.AddSingleton(new OVNotificationService());
        var host = context.Render<OVNotificationHost>();
        Assert.NotNull(host.Find("div[role='region'][aria-label='Notifications']"));
    }

    [Fact]
    public async Task Api_response_tabs_reference_their_selected_panel()
    {
        await using var context = Context();
        var cut = context.Render<OVApiResponsePreview>(p =>
            p.Add(x => x.Result, new OVApiCallTestResult(OVApiTestState.Success)));
        var panel = cut.Find("[role='tabpanel']");
        var tabs = cut.FindAll("[role='tab']");
        Assert.Equal(3, tabs.Count);
        Assert.All(tabs, tab => Assert.Equal(panel.Id, tab.GetAttribute("aria-controls")));
        var selected = Assert.Single(tabs.Where(tab => tab.GetAttribute("aria-selected") == "true"));
        Assert.Equal(selected.Id, panel.GetAttribute("aria-labelledby"));
    }

    private static BunitContext Context()
    {
        var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }
}
