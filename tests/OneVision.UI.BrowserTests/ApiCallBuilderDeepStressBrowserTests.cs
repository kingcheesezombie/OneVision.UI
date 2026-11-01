using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class ApiCallBuilderDeepStressBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Theory]
    [InlineData(1440, 900)]
    [InlineData(1024, 900)]
    [InlineData(760, 900)]
    [InlineData(390, 844)]
    public async Task Eleven_step_flow_remains_compact_oriented_and_overflow_safe(int width, int height)
    {
        await Page.SetViewportSizeAsync(width, height);
        await Page.GotoAsync($"{BaseUrl}/patterns/api-call-builder/deep-stress");
        await Page.GetByRole(AriaRole.Heading, new() { Name = "API Call Builder · 11-step stress test", Exact = true }).WaitForAsync();

        var items = Page.Locator(".ov-api-decision-trail__item");
        Assert.Equal(11, await items.CountAsync());
        Assert.True(await Page.Locator(".ov-api-call-builder__flow-context").IsVisibleAsync());
        Assert.True(await Page.Locator(".ov-api-call-builder__request").IsVisibleAsync());
        Assert.True(await Page.Locator("button[aria-label^='Copy resolved API path:']").IsVisibleAsync());

        var builder = Page.Locator(".ov-api-call-builder");
        Assert.True(await builder.EvaluateAsync<bool>("element => element.scrollWidth <= element.clientWidth + 1"));
    }

    [Fact]
    public async Task Eleven_step_flow_is_axe_clean()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/api-call-builder/deep-stress");
        await Page.GetByRole(AriaRole.Heading, new() { Name = "API Call Builder · 11-step stress test", Exact = true }).WaitForAsync();
        var result = await Page.Locator(".ov-api-call-builder").RunAxe();
        var violations = result.Violations?.ToArray() ?? [];
        Assert.True(violations.Length == 0, string.Join(Environment.NewLine, violations.Select(v => $"{v.Id}: {v.Description}")));
    }
}
