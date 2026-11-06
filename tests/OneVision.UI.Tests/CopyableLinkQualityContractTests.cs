using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class CopyableLinkQualityContractTests
{
    private static BunitContext CreateContext()
    {
        var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }

    [Fact]
    public async Task Successful_copy_calls_success_callback_without_failure_callback()
    {
        await using var context = CreateContext();
        // Verify that the copy API receives the value supplied for the
        // clipboard, not merely the shortened visible text.
        var clipboard = context.JSInterop.SetupVoid("navigator.clipboard.writeText", "Copied reference");
        clipboard.SetVoidResult();

        var copied = 0;
        var failures = 0;
        var cut = context.Render<OVCopyableLink>(p => p
            .Add(x => x.Text, "Visible reference")
            .Add(x => x.CopyValue, "Copied reference")
            .Add(x => x.FeedbackMilliseconds, 400)
            .Add(x => x.Copied, EventCallback.Factory.Create(this, () => copied++))
            .Add(x => x.CopyFailed, EventCallback.Factory.Create(this, () => failures++)));

        cut.Find("button").Click();

        Assert.Equal(1, copied);
        Assert.Equal(0, failures);
        Assert.Single(clipboard.Invocations);
    }

    [Fact]
    public async Task Denied_clipboard_does_not_report_success_and_notifies_failure()
    {
        await using var context = CreateContext();
        context.JSInterop.SetupVoid("navigator.clipboard.writeText", "Secure reference")
            .SetException(new JSException("NotAllowedError: Clipboard permission denied"));

        var copied = 0;
        var failures = 0;
        var cut = context.Render<OVCopyableLink>(p => p
            .Add(x => x.Text, "Secure reference")
            .Add(x => x.FeedbackMilliseconds, 400)
            .Add(x => x.Copied, EventCallback.Factory.Create(this, () => copied++))
            .Add(x => x.CopyFailed, EventCallback.Factory.Create(this, () => failures++)));

        cut.Find("button").Click();

        Assert.Equal(0, copied);
        Assert.Equal(1, failures);
    }

    [Fact]
    public async Task Disabled_copy_action_never_invokes_clipboard_or_callbacks()
    {
        await using var context = CreateContext();
        var copied = 0;
        var failures = 0;
        var cut = context.Render<OVCopyableLink>(p => p
            .Add(x => x.Text, "Reference")
            .Add(x => x.Disabled, true)
            .Add(x => x.Copied, EventCallback.Factory.Create(this, () => copied++))
            .Add(x => x.CopyFailed, EventCallback.Factory.Create(this, () => failures++)));

        Assert.True(cut.Find("button").HasAttribute("disabled"));
        Assert.Equal(0, copied);
        Assert.Equal(0, failures);
    }

    [Fact]
    public async Task New_payload_does_not_retain_stale_copy_success_or_failure_state()
    {
        await using var context = CreateContext();
        var cut = context.Render<OVCopyableLink>(p => p
            .Add(x => x.Text, "Old reference")
            .Add(x => x.FeedbackMilliseconds, 400));

        cut.Find("button").Click();
        cut.Render(p => p
            .Add(x => x.Text, "New reference")
            .Add(x => x.FeedbackMilliseconds, 400));

        Assert.Contains("New reference", cut.Markup);
        Assert.DoesNotContain("ov-copyable-link--copied", cut.Find("button").ClassName);
        Assert.DoesNotContain("ov-copyable-link--copy-failed", cut.Find("button").ClassName);
    }
}
