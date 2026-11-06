using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class ProgressiveConfigurationDefinitionsBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Default_gallery_is_a_generic_three_step_definition_not_an_allocation_specific_composition()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/progressive-configuration-builder");
        var builder = Page.Locator(".ov-progressive-builder");
        await builder.WaitForAsync();

        Assert.Equal("definitions", await builder.Locator(".ov-progressive-builder__body").GetAttributeAsync("data-composition"));
        Assert.Equal(3, await builder.Locator(".ov-configuration-step").CountAsync());
        Assert.True(await builder.GetByText("General settings", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await builder.GetByText("Delivery settings", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal("current", await builder.Locator(".ov-configuration-step").Nth(1).GetAttributeAsync("data-state"));
        Assert.Equal("pending", await builder.Locator(".ov-configuration-step").Nth(2).GetAttributeAsync("data-state"));
    }

    [Fact]
    public async Task Section_headers_remain_full_width_and_follow_progression()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/progressive-configuration-builder");
        var builder = Page.Locator(".ov-progressive-builder");
        var general = builder.Locator(".ov-configuration-section").Nth(0);
        var delivery = builder.Locator(".ov-configuration-section").Nth(1);

        Assert.Equal("current", await general.GetAttributeAsync("data-state"));
        Assert.Equal("pending", await delivery.GetAttributeAsync("data-state"));
        var headerWidth = await general.EvaluateAsync<double[]>(
            "el => [el.querySelector('.ov-configuration-section__header').getBoundingClientRect().width, el.getBoundingClientRect().width]");
        Assert.True(headerWidth[0] >= headerWidth[1] - 2);

        var channel = builder.GetByRole(AriaRole.Combobox, new() { Name = "Channel", Exact = true });
        await channel.ClickAsync();
        await Page.GetByText("Extended", new() { Exact = true }).Last.ClickAsync();

        Assert.Equal("completed", await general.GetAttributeAsync("data-state"));
        Assert.Equal("current", await delivery.GetAttributeAsync("data-state"));
        Assert.True(await general.GetByText("Complete", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await delivery.GetByText("In progress", new() { Exact = true }).IsVisibleAsync());
        var completedColor = await general.Locator(".ov-configuration-section__header").EvaluateAsync<string>(
            "el => getComputedStyle(el).backgroundColor");
        var activeColor = await delivery.Locator(".ov-configuration-section__header").EvaluateAsync<string>(
            "el => getComputedStyle(el).backgroundColor");
        Assert.NotEqual(completedColor, activeColor);
    }

    [Fact]
    public async Task Twelve_step_scenario_has_stable_order_and_governed_group_and_inline_separators()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/progressive-configuration-builder");
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("many-steps");
        var builder = Page.Locator(".ov-progressive-builder");
        Assert.Equal(12, await builder.Locator(".ov-configuration-step").CountAsync());
        Assert.Equal(3, await builder.Locator(".ov-configuration-section").CountAsync());
        Assert.Equal(3, await builder.Locator(".ov-configuration-relationship").CountAsync());
        Assert.True(await builder.GetByText("Refine policy", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await builder.GetByText("then configure delivery", new() { Exact = true }).IsVisibleAsync());

        var width = await builder.EvaluateAsync<bool>("el => el.scrollWidth <= el.clientWidth + 1");
        Assert.True(width);
    }

    [Fact]
    public async Task Separators_are_a_true_optional_pattern_capability()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/progressive-configuration-builder");
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("many-steps");
        var builder = Page.Locator(".ov-progressive-builder");
        Assert.Equal(3, await builder.Locator(".ov-configuration-relationship").CountAsync());

        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Section and step separators" }).UncheckAsync();
        Assert.Equal(0, await builder.Locator(".ov-configuration-relationship").CountAsync());
        Assert.Equal(12, await builder.Locator(".ov-configuration-step").CountAsync());
    }

    [Fact]
    public async Task Selecting_current_step_advances_to_next_explicitly_dependent_step()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/progressive-configuration-builder");
        var builder = Page.Locator(".ov-progressive-builder");
        var channel = builder.GetByRole(AriaRole.Combobox, new() { Name = "Channel", Exact = true });
        await channel.ClickAsync();
        await Page.GetByText("Extended", new() { Exact = true }).Last.ClickAsync();

        Assert.Equal("completed", await builder.Locator(".ov-configuration-step").Nth(1).GetAttributeAsync("data-state"));
        Assert.Equal("current", await builder.Locator(".ov-configuration-step").Nth(2).GetAttributeAsync("data-state"));
        Assert.True(await builder.GetByRole(AriaRole.Combobox, new() { Name = "Priority", Exact = true }).IsVisibleAsync());
    }
}
