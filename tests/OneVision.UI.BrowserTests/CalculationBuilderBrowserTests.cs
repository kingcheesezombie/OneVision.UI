using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class CalculationBuilderBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Three_tiers_expose_only_their_governed_controls()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/calculation-builder");
        await Page.Locator("#ov-pattern-scenario").WaitForAsync();
        Assert.True(await Page.GetByText("Core operands", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal(0, await Page.GetByText("GEO Measures", new() { Exact = true }).CountAsync());
        Assert.Equal(0, await Page.GetByRole(AriaRole.Button, new() { Name = "COUNT(" }).CountAsync());

        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("extended");
        Assert.True(await Page.GetByText("GEO Measures", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal(0, await Page.GetByRole(AriaRole.Button, new() { Name = "COUNT(" }).CountAsync());

        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("advanced");
        Assert.True(await Page.GetByRole(AriaRole.Button, new() { Name = "COUNT(" }).IsVisibleAsync());
        Assert.True(await Page.GetByText("GP Practice", new() { Exact = true }).First.IsVisibleAsync());
    }

    [Fact]
    public async Task Higher_tier_expression_is_preserved_in_lower_tier_and_format_is_independent()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/calculation-builder");
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("advanced-in-simple");
        Assert.True(await Page.GetByText("shown without modification", new() { Exact = false }).IsVisibleAsync());
        Assert.True(await Page.GetByRole(AriaRole.Group, new() { Name = "Ordered formula tokens" }).GetByText("COUNT").IsVisibleAsync());
        Assert.Equal(0, await Page.Locator(".ov-calculation-builder__remove").CountAsync());
        var decimalFormat = Page.GetByRole(AriaRole.Button, new() { Name = "Decimal Displays a calculated result as a decimal value." });
        Assert.True(await decimalFormat.IsVisibleAsync());
        await Page.GetByRole(AriaRole.Button, new() { Name = "Percentage Displays a calculated result as a percentage." }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Group, new() { Name = "Ordered formula tokens" }).GetByText("COUNT").IsVisibleAsync());
    }
}
