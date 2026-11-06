using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class CopyableLinkFailureBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Denied_clipboard_permission_shows_an_informational_error_not_fake_success()
    {
        await Page.AddInitScriptAsync("""
            Object.defineProperty(navigator, 'clipboard', {
                configurable: true,
                value: { writeText: async () => { throw new DOMException('Denied', 'NotAllowedError'); } }
            });
            """);
        await Page.GotoAsync($"{BaseUrl}/components/copyable-link");

        var copy = Page.Locator(".ov-component-demo__body button.ov-copyable-link");
        await copy.ClickAsync();
        await Page.Locator(".ov-component-demo__body button.ov-copyable-link--copy-failed").WaitForAsync();

        Assert.True(await copy.GetByText("Copy unavailable", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal("Unable to copy to clipboard.", await copy.Locator(".ov-copyable-link__status").InnerTextAsync());
        Assert.Equal(0, await copy.GetByText("Copied ✓").CountAsync());
    }

    [Fact]
    public async Task Copy_feedback_is_cleared_if_the_consumer_changes_the_value()
    {
        await Page.AddInitScriptAsync("""
            Object.defineProperty(navigator, 'clipboard', {
                configurable: true,
                value: { writeText: async value => { window.__testCopiedValue = value; } }
            });
            """);
        await Page.GotoAsync($"{BaseUrl}/components/copyable-link");

        var copy = Page.Locator(".ov-component-demo__body button.ov-copyable-link");
        await copy.ClickAsync();
        await Page.Locator(".ov-component-demo__body button.ov-copyable-link--copied").WaitForAsync();
        Assert.Equal("OneVisionIcon.Copy", await Page.EvaluateAsync<string>("() => window.__testCopiedValue"));

        await Page.Locator(".ov-state-choice", new() { HasText = "Long value" }).ClickAsync();
        Assert.Equal(0, await Page.Locator(".ov-component-demo__body button.ov-copyable-link--copied").CountAsync());
        Assert.True(await copy.GetByText("OneVisionIcon.Example.With.A.Very.Long.Reference.Value.That.Truncates", new() { Exact = true }).CountAsync() > 0);
    }

    [Theory]
    [InlineData(640)]
    [InlineData(320)]
    public async Task Long_copyable_value_remains_inside_its_own_control_at_zoom_equivalent_viewport(int viewportWidth)
    {
        await Page.SetViewportSizeAsync(viewportWidth, 900);
        await Page.GotoAsync($"{BaseUrl}/components/copyable-link");
        await Page.Locator(".ov-state-choice", new() { HasText = "Long value" }).ClickAsync();

        var copy = Page.Locator(".ov-component-demo__body button.ov-copyable-link");
        Assert.True(await copy.EvaluateAsync<bool>(
            "e => e.scrollWidth <= e.clientWidth + 2 && e.getBoundingClientRect().width > 0"));
        Assert.True(await copy.IsVisibleAsync());
    }
}
