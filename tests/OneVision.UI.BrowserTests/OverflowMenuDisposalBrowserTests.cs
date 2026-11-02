using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class OverflowMenuDisposalBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Changing_gallery_state_disposes_open_portalled_menu_without_orphaning_surface()
    {
        await Page.GotoAsync($"{BaseUrl}/components/overflow-menu");
        await Page.Locator("h1.ov-gallery-page-title", new() { HasText = "Overflow Menu" }).WaitForAsync();
        await Page.Locator(".ov-state-choice", new() { HasText = "Multiple menus" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "First row actions" }).ClickAsync();
        await Page.Locator("[role='menu'][data-ov-popover-portalled='true']").WaitForAsync();

        await Page.Locator(".ov-state-choice", new() { HasText = "Standard row menu" }).ClickAsync();
        await Page.Locator(".ov-state-choice--active", new() { HasText = "Standard row menu" }).WaitForAsync();

        Assert.Equal(0, await Page.Locator("body > [role='menu'][data-ov-popover-portalled='true']").CountAsync());
    }
}
