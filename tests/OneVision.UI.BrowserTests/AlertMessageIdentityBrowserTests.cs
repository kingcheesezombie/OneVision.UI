using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class AlertMessageIdentityBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    private async Task OpenDismissibleAlertAsync()
    {
        await Page.GotoAsync($"{BaseUrl}/components/alert");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Dismissible", Exact = true }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Dismiss message", Exact = true }).WaitForAsync();
    }

    [Fact]
    public async Task A_dismissed_alert_remains_hidden_until_a_new_message_is_presented()
    {
        await OpenDismissibleAlertAsync();
        var dismiss = Page.GetByRole(AriaRole.Button, new() { Name = "Dismiss message", Exact = true });
        await dismiss.ClickAsync();
        Assert.Equal(0, await dismiss.CountAsync());

        // Toggling an independent capability causes a normal parent render,
        // but must not resurrect the same message.
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Dismissible", Exact = true }).CheckAsync();
        Assert.Equal(0, await dismiss.CountAsync());

        await Page.GetByRole(AriaRole.Button, new() { Name = "Show next message", Exact = true }).ClickAsync();
        Assert.True(await Page.GetByText("New message 1:", new() { Exact = false }).IsVisibleAsync());
        Assert.Equal(1, await dismiss.CountAsync());
    }

    [Fact]
    public async Task Dismiss_and_replacement_are_keyboard_operable_and_accessible()
    {
        await OpenDismissibleAlertAsync();
        var dismiss = Page.GetByRole(AriaRole.Button, new() { Name = "Dismiss message", Exact = true });
        await dismiss.FocusAsync();
        await Page.Keyboard.PressAsync("Enter");
        Assert.Equal(0, await dismiss.CountAsync());

        var replace = Page.GetByRole(AriaRole.Button, new() { Name = "Show next message", Exact = true });
        await replace.FocusAsync();
        await Page.Keyboard.PressAsync("Enter");
        Assert.Equal(1, await dismiss.CountAsync());
        var violations = (await Page.Locator(".ov-component-demo__body").RunAxe()).Violations?.ToArray() ?? [];
        Assert.True(violations.Length == 0, string.Join(Environment.NewLine, violations.Select(x => x.Id + ": " + x.Description)));
    }

    [Theory]
    [InlineData("200%")]
    [InlineData("400%")]
    public async Task Alert_message_and_actions_fit_without_component_horizontal_overflow_at_zoom(string zoom)
    {
        await Page.SetViewportSizeAsync(1280, 900);
        await OpenDismissibleAlertAsync();
        await Page.EvaluateAsync("(value) => { document.documentElement.style.zoom = value; }", zoom);

        var body = Page.Locator(".ov-component-demo__body");
        Assert.True(await body.EvaluateAsync<bool>("e => e.scrollWidth <= e.clientWidth + 2"));
        var button = Page.GetByRole(AriaRole.Button, new() { Name = "Dismiss message", Exact = true });
        Assert.True(await button.IsVisibleAsync());
    }
}
