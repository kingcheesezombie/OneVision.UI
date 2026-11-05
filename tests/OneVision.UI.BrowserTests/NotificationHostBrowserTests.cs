using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class NotificationHostBrowserTests : PageTest
{
    private static string BaseUrl =>
        Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Hover_pauses_a_timed_notification_then_resumes_with_remaining_time()
    {
        await Page.GotoAsync($"{BaseUrl}/components/notification");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Timed notice", Exact = true }).ClickAsync();

        var notice = Page.Locator(".ov-notification-host__item");
        await notice.WaitForAsync();
        await notice.HoverAsync();
        await Page.WaitForTimeoutAsync(1050);

        Assert.True(await notice.IsVisibleAsync());

        await Page.Mouse.MoveAsync(0, 0);
        await notice.WaitForAsync(new() { State = WaitForSelectorState.Detached, Timeout = 3000 });
    }

    [Fact]
    public async Task Replacement_keeps_one_accessible_notification_and_no_stale_content()
    {
        await Page.GotoAsync($"{BaseUrl}/components/notification");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Keyed notice", Exact = true }).ClickAsync();

        var notice = Page.Locator(".ov-notification-host__item");
        await notice.GetByText("Original keyed notification", new() { Exact = true }).WaitForAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Replace keyed notice", Exact = true }).ClickAsync();
        await notice.GetByText("Updated keyed notification", new() { Exact = true }).WaitForAsync();

        Assert.Equal(1, await notice.CountAsync());
        Assert.Equal(0, await Page.GetByText("Original keyed notification", new() { Exact = true }).CountAsync());

        var result = await Page.Locator(".ov-notification-host").RunAxe();
        var violations = result.Violations?.ToArray() ?? [];
        Assert.True(violations.Length == 0, string.Join(Environment.NewLine, violations.Select(v => $"{v.Id}: {v.Description}")));
    }
}
