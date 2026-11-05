using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

/// <summary>
/// L0/L1/L4 keyboard and state continuity checks for governed interactive controls.
/// </summary>
public sealed class InteractionSemanticsBrowserTests : PageTest
{
    private static string BaseUrl =>
        Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Accordion_preserves_uncontrolled_expansion_when_parent_recreates_items()
    {
        await Page.GotoAsync($"{BaseUrl}/components/accordion");
        await Page.Locator(".ov-accordion").WaitForAsync();

        var headers = Page.Locator(".ov-accordion .mud-expand-panel-header");
        Assert.Equal(2, await headers.CountAsync());
        await headers.First.ClickAsync();
        await Page.GetByText("Expanded keys: advanced", new() { Exact = true }).WaitForAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Refresh items" }).ClickAsync();
        Assert.True(await Page.GetByText("Expanded keys: advanced", new() { Exact = true }).IsVisibleAsync());

        await headers.Nth(1).ClickAsync();
        await Page.GetByText("Expanded keys: additional", new() { Exact = true }).WaitForAsync();
    }

    [Fact]
    public async Task Accordion_supports_parent_controlled_and_multiple_expansion()
    {
        await Page.GotoAsync($"{BaseUrl}/components/accordion");
        await SelectStateAsync("Controlled");
        await Page.GetByText("Expanded keys: advanced", new() { Exact = true }).WaitForAsync();

        var headers = Page.Locator(".ov-accordion .mud-expand-panel-header");
        await headers.Nth(1).ClickAsync();
        await Page.GetByText("Expanded keys: additional", new() { Exact = true }).WaitForAsync();

        await SelectStateAsync("Multiple");
        await Page.GetByText("Expanded keys: advanced, additional", new() { Exact = true }).WaitForAsync();
        await headers.First.ClickAsync();
        await Page.GetByText("Expanded keys: additional", new() { Exact = true }).WaitForAsync();
    }

    [Fact]
    public async Task Source_discovery_supports_arrow_home_end_and_keyboard_preview()
    {
        await OpenDirectSourceAsync();
        var candidates = Page.Locator(".ov-source-discovery__candidate");
        Assert.True(await candidates.CountAsync() >= 2);

        var first = candidates.First;
        var second = candidates.Nth(1);
        var last = candidates.Last;
        Assert.Equal("0", await first.GetAttributeAsync("tabindex"));
        Assert.Equal("-1", await second.GetAttributeAsync("tabindex"));

        await first.FocusAsync();
        await first.PressAsync("End");
        await Page.Locator(".ov-source-discovery__candidate:last-child[aria-selected='true']").WaitForAsync();

        Assert.Equal("0", await last.GetAttributeAsync("tabindex"));
        Assert.True(await last.EvaluateAsync<bool>("element => document.activeElement === element"));

        await last.PressAsync("Home");
        await Page.Locator(".ov-source-discovery__candidate:first-child[aria-selected='true']").WaitForAsync();
        Assert.True(await first.EvaluateAsync<bool>("element => document.activeElement === element"));

        await first.PressAsync("ArrowDown");
        await Page.Locator(".ov-source-discovery__candidate:nth-child(2)[aria-selected='true']").WaitForAsync();
        await second.PressAsync("ArrowUp");
        await Page.Locator(".ov-source-discovery__candidate:first-child[aria-selected='true']").WaitForAsync();

        // Enter is native button activation; previewing must not commit source selection.
        await first.PressAsync("Enter");
        Assert.True(await Page.GetByText("North Ward", new() { Exact = true }).IsVisibleAsync());
        Assert.False(await Page.GetByRole(AriaRole.Button, new() { Name = "Run & Profile", Exact = true }).IsEnabledAsync());

        await Page.GetByRole(AriaRole.Button, new() { Name = "Use this source", Exact = true }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Button, new() { Name = "Run & Profile", Exact = true }).IsEnabledAsync());
    }

    private async Task OpenDirectSourceAsync()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/source-configuration");
        var source = Page.GetByRole(AriaRole.Combobox, new() { Name = "Reference source", Exact = true });
        await source.ClickAsync();
        await Page.GetByText("Direct Web Table source", new() { Exact = true }).ClickAsync();
        await Page.GetByRole(AriaRole.Tab, new() { Name = "Profile", Exact = true }).ClickAsync();
        await Page.Locator(".ov-source-discovery__candidate").First.WaitForAsync();
    }

    private async Task SelectStateAsync(string state)
    {
        await Page.Locator(".ov-state-choice", new() { HasText = state }).ClickAsync();
        await Page.Locator(".ov-state-choice--active", new() { HasText = state }).WaitForAsync();
    }
}
