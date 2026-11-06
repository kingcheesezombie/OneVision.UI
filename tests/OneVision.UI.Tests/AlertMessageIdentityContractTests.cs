using Bunit;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class AlertMessageIdentityContractTests
{
    private static BunitContext Context()
    {
        var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }

    [Fact]
    public async Task A_dismissed_message_stays_dismissed_on_ordinary_parent_rerenders()
    {
        await using var context = Context();
        var dismissedCount = 0;
        var cut = context.Render<OVAlert>(p => p
            .Add(x => x.Dismissible, true)
            .Add(x => x.MessageKey, "warning-1")
            .Add(x => x.Dismissed, EventCallback.Factory.Create(this, () => dismissedCount++))
            .Add<RenderFragment>(x => x.ChildContent, b => b.AddContent(0, "Check these items.")));

        Assert.Contains("Check these items.", cut.Markup);
        cut.Find("button[aria-label='Dismiss message']").Click();
        Assert.Equal(1, dismissedCount);
        Assert.DoesNotContain("Check these items.", cut.Markup);

        cut.Render(p => p
            .Add(x => x.Dismissible, true)
            .Add(x => x.MessageKey, "warning-1")
            .Add<RenderFragment>(x => x.ChildContent, b => b.AddContent(0, "Check these items.")));
        Assert.DoesNotContain("Check these items.", cut.Markup);
    }

    [Fact]
    public async Task A_new_message_identity_reopens_the_same_component()
    {
        await using var context = Context();
        var cut = context.Render<OVAlert>(p => p
            .Add(x => x.Dismissible, true)
            .Add(x => x.MessageKey, "error-1")
            .Add<RenderFragment>(x => x.ChildContent, b => b.AddContent(0, "First error.")));

        cut.Find("button[aria-label='Dismiss message']").Click();
        Assert.DoesNotContain("First error.", cut.Markup);

        cut.Render(p => p
            .Add(x => x.Dismissible, true)
            .Add(x => x.MessageKey, "error-2")
            .Add<RenderFragment>(x => x.ChildContent, b => b.AddContent(0, "Second error.")));

        Assert.Contains("Second error.", cut.Markup);
        Assert.Single(cut.FindAll("button[aria-label='Dismiss message']"));
    }

    [Fact]
    public async Task The_opt_in_identity_does_not_change_legacy_dismissal_behaviour()
    {
        await using var context = Context();
        var cut = context.Render<OVAlert>(p => p
            .Add(x => x.Dismissible, true)
            .Add<RenderFragment>(x => x.ChildContent, b => b.AddContent(0, "Same alert.")));

        cut.Find("button[aria-label='Dismiss message']").Click();
        Assert.DoesNotContain("Same alert.", cut.Markup);

        cut.Render(p => p
            .Add(x => x.Dismissible, true)
            .Add<RenderFragment>(x => x.ChildContent, b => b.AddContent(0, "Same alert.")));
        Assert.DoesNotContain("Same alert.", cut.Markup);
        Assert.Null(cut.Instance.MessageKey);
    }

    [Fact]
    public async Task Changing_key_does_not_change_alert_tone_or_dismissibility()
    {
        await using var context = Context();
        var cut = context.Render<OVAlert>(p => p
            .Add(x => x.Tone, OVAlertTone.Warning)
            .Add(x => x.Dismissible, false)
            .Add(x => x.MessageKey, "warning-2")
            .Add<RenderFragment>(x => x.ChildContent, b => b.AddContent(0, "Warning content.")));

        Assert.Contains("Warning content.", cut.Markup);
        Assert.Empty(cut.FindAll("button[aria-label='Dismiss message']"));
    }
}
