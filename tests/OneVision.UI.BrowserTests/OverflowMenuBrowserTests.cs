using System.Text.Json;
using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class OverflowMenuBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Menu_escapes_overflow_hidden_list_surface()
    {
        await OpenStateAsync("Clipped list surface");
        var container = Page.GetByTestId("overflow-clipped-surface");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Clipped row actions" }).ClickAsync();

        var menu = Page.Locator("[role='menu'][data-ov-popover-portalled='true']");
        await menu.WaitForAsync();
        var containerBox = await container.BoundingBoxAsync();
        var menuBox = await menu.BoundingBoxAsync();

        Assert.NotNull(containerBox);
        Assert.NotNull(menuBox);
        Assert.True(menuBox!.Y + menuBox.Height > containerBox!.Y + containerBox.Height);
        Assert.True(await menu.EvaluateAsync<bool>("element => element.parentElement === document.body"));
    }

    [Fact]
    public async Task Escape_outside_click_and_single_active_menu_are_governed()
    {
        await OpenStateAsync("Multiple menus");
        var first = Page.GetByRole(AriaRole.Button, new() { Name = "First row actions" });
        var second = Page.GetByRole(AriaRole.Button, new() { Name = "Second row actions" });

        await first.ClickAsync();
        Assert.Equal("true", await first.GetAttributeAsync("aria-expanded"));

        await second.ClickAsync();
        await Page.Locator("[role='menu']").WaitForAsync();
        Assert.Equal(1, await Page.Locator("[role='menu']").CountAsync());
        Assert.Equal("false", await first.GetAttributeAsync("aria-expanded"));
        Assert.Equal("true", await second.GetAttributeAsync("aria-expanded"));

        await Page.Keyboard.PressAsync("Escape");
        await Page.Locator("[role='menu']").WaitForAsync(new() { State = WaitForSelectorState.Detached });
        Assert.Equal("false", await second.GetAttributeAsync("aria-expanded"));

        await first.ClickAsync();
        await Page.Locator("h1.ov-gallery-page-title").ClickAsync();
        await Page.Locator("[role='menu']").WaitForAsync(new() { State = WaitForSelectorState.Detached });
        Assert.Equal("false", await first.GetAttributeAsync("aria-expanded"));
    }

    [Fact]
    public async Task Bottom_collision_flips_menu_above_trigger()
    {
        await OpenStateAsync("Viewport bottom");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Bottom row actions" }).ClickAsync();

        var menu = Page.Locator("[role='menu']");
        await menu.WaitForAsync();
        Assert.Equal("above", await menu.GetAttributeAsync("data-ov-popover-placement"));

        var triggerBox = await Page.GetByRole(AriaRole.Button, new() { Name = "Bottom row actions" }).BoundingBoxAsync();
        var menuBox = await menu.BoundingBoxAsync();
        Assert.NotNull(triggerBox);
        Assert.NotNull(menuBox);
        Assert.True(menuBox!.Y + menuBox.Height <= triggerBox!.Y);
    }

    [Theory]
    [InlineData(1280, 900)]
    [InlineData(760, 900)]
    [InlineData(390, 844)]
    public async Task Right_edge_collision_stays_inside_viewport(int width, int height)
    {
        await Page.SetViewportSizeAsync(width, height);
        await OpenStateAsync("Viewport right edge");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Right edge actions" }).ClickAsync();

        var menu = Page.Locator("[role='menu']");
        await menu.WaitForAsync();
        var box = await menu.BoundingBoxAsync();
        Assert.NotNull(box);
        Assert.True(box!.X >= 7);
        Assert.True(box.X + box.Width <= width - 7);
        Assert.True(await Page.EvaluateAsync<bool>("() => document.documentElement.scrollWidth <= document.documentElement.clientWidth + 1"));
    }

    [Fact]
    public async Task Scroll_container_repositions_and_open_menu_is_axe_clean()
    {
        await OpenStateAsync("Scroll container");
        var scroll = Page.GetByTestId("overflow-scroll-surface");
        var trigger = Page.GetByRole(AriaRole.Button, new() { Name = "Scroll row actions" });
        await trigger.ClickAsync();

        var menu = Page.Locator("[role='menu']");
        await menu.WaitForAsync();
        var before = await menu.BoundingBoxAsync();
        Assert.NotNull(before);

        await scroll.EvaluateAsync("element => { element.scrollLeft = 60; element.dispatchEvent(new Event('scroll', { bubbles: true })); }");
        await Page.WaitForTimeoutAsync(100);
        var after = await menu.BoundingBoxAsync();
        Assert.NotNull(after);
        Assert.NotEqual(before!.X, after!.X);

        var result = await Page.Locator("body").RunAxe();
        var violations = result.Violations?.ToArray() ?? [];
        Assert.True(violations.Length == 0, JsonSerializer.Serialize(violations));
    }

    private async Task OpenStateAsync(string state)
    {
        await Page.GotoAsync($"{BaseUrl}/components/overflow-menu");
        await Page.Locator("h1.ov-gallery-page-title", new() { HasText = "Overflow Menu" }).WaitForAsync();
        await Page.Locator(".ov-state-choice", new() { HasText = state }).ClickAsync();
        await Page.Locator(".ov-state-choice--active", new() { HasText = state }).WaitForAsync();
    }
}
