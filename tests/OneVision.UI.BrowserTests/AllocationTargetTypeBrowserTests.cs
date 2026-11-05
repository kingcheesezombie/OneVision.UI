using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class AllocationTargetTypeBrowserTests : PageTest
{
    private static string BaseUrl =>
        Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Theory]
    [InlineData("eam-target-type", "EAM", "Organisation Unit")]
    [InlineData("idm-person", "IDM", "PERSON")]
    [InlineData("idm-competency", "IDM", "COMPETENCY_ITEM")]
    public async Task Provider_scenarios_render_supplied_target_types(string scenario, string provider, string type)
    {
        await OpenAsync();
        await ScenarioAsync(scenario);

        Assert.Equal(1, await Page.Locator(".ov-allocation__target-type").CountAsync());
        Assert.Equal(0, await Page.Locator(".ov-allocation__platform").CountAsync());
        Assert.True(await Page.Locator(".ov-allocation__target-type").GetByText(type, new() { Exact = true }).IsVisibleAsync());
        Assert.True(await Page.Locator(".ov-allocation__results[aria-label='Target results']").IsVisibleAsync());
        Assert.True(await Page.GetByText("Target entity", new() { Exact = true }).IsVisibleAsync());
    }

    [Fact]
    public async Task Odm_has_no_type_by_default_but_supports_an_opt_in_type_fragment()
    {
        await OpenAsync();
        await ScenarioAsync("odm-allocation");
        Assert.Equal(0, await Page.Locator(".ov-allocation__target-type").CountAsync());

        await CapabilityAsync("Target Type selector", true);
        Assert.Equal(1, await Page.Locator(".ov-allocation__target-type").CountAsync());
        Assert.Equal(0, await Page.Locator(".ov-allocation__platform").CountAsync());

        await CapabilityAsync("Target Type selector", false);
        Assert.Equal(0, await Page.Locator(".ov-allocation__target-type").CountAsync());
        Assert.Equal(1, await Page.Locator(".ov-allocation__results[aria-label='Target results']").CountAsync());
    }

    [Fact]
    public async Task Generic_finder_updates_provider_dependent_type_without_preserving_stale_value()
    {
        await OpenAsync();
        await ScenarioAsync("generic-allocation");
        Assert.Equal(3, await Page.Locator(".ov-allocation__platform").CountAsync());
        Assert.Equal(0, await Page.Locator(".ov-allocation__target-type").CountAsync());

        await CapabilityAsync("Target Type selector", true);
        Assert.True(await Page.Locator(".ov-allocation__target-type").GetByText("Organisation Unit", new() { Exact = true }).IsVisibleAsync());

        await Page.GetByRole(AriaRole.Button, new() { Name = "IDM", Exact = true }).ClickAsync();
        Assert.Equal(1, await Page.Locator(".ov-allocation__target-type").CountAsync());
        Assert.Equal(0, await Page.Locator(".ov-allocation__identity-row").CountAsync());
        Assert.Equal(0, await Page.Locator(".ov-allocation__target-type").GetByText("Organisation Unit", new() { Exact = true }).CountAsync());

        // The app-supplied IDM list must now contain the IDM fixture types,
        // rather than reusing the earlier EAM choice.
        await Page.Locator(".ov-allocation__target-type .mud-input-control").ClickAsync();
        var options = Page.Locator(".mud-popover-open .mud-list-item");
        await options.First.WaitForAsync();
        Assert.True(await options.Filter(new() { HasText = "PERSON" }).IsVisibleAsync());
        Assert.True(await options.Filter(new() { HasText = "COMPETENCY_ITEM" }).IsVisibleAsync());

        await options.Filter(new() { HasText = "PERSON" }).ClickAsync();
        await Page.Locator(".ov-allocation__results[aria-label='Target results'] input[type=radio]").First.ClickAsync();
        Assert.Equal(1, await Page.Locator(".ov-allocation__identity-row").CountAsync());
        Assert.Equal(0, await Page.Locator(".ov-allocation__target-type").CountAsync());

        await Page.Locator(".ov-allocation__identity-row").GetByRole(AriaRole.Button, new() { Name = "Change" }).ClickAsync();
        Assert.Equal(1, await Page.Locator(".ov-allocation__target-type").CountAsync());
        Assert.Equal(1, await Page.Locator(".ov-allocation__results[aria-label='Target results']").CountAsync());
    }

    [Fact]
    public async Task Target_type_loading_empty_disabled_and_read_only_use_governed_states()
    {
        await OpenAsync();
        await ScenarioAsync("generic-allocation");
        await CapabilityAsync("Target Type selector", true);

        await CapabilityAsync("Target Types loading", true);
        Assert.True(await Page.GetByText("Loading target types…", new() { Exact = false }).IsVisibleAsync());
        await CapabilityAsync("Target Types loading", false);

        await CapabilityAsync("No available Target Types", true);
        Assert.True(await Page.GetByText("No target types", new() { Exact = true }).IsVisibleAsync());
        await CapabilityAsync("No available Target Types", false);

        await CapabilityAsync("Target Type disabled", true);
        Assert.True(await Page.Locator(".ov-allocation__target-type .mud-disabled").First.IsVisibleAsync());
        await CapabilityAsync("Target Type disabled", false);

        await CapabilityAsync("Read-only allocation", true);
        Assert.Equal(3, await Page.Locator(".ov-allocation__platform:disabled").CountAsync());
        Assert.True(await Page.Locator(".ov-allocation__target-type .mud-disabled").First.IsVisibleAsync());
        await CapabilityAsync("Read-only allocation", false);
        Assert.Equal(0, await Page.Locator(".ov-allocation__platform:disabled").CountAsync());
    }

    [Fact]
    public async Task Known_target_keeps_its_fixed_provider_type_summary_without_selection_fields()
    {
        await OpenAsync();
        Assert.True(await Page.GetByText("Children & Families", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await Page.GetByText("EAM · Organisation Unit · 620496", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal(0, await Page.Locator(".ov-allocation__target-type").CountAsync());
        Assert.Equal(0, await Page.Locator(".ov-allocation__platform").CountAsync());
    }

    [Theory]
    [InlineData(390, 844)]
    [InlineData(1280, 900)]
    public async Task Optional_target_type_fits_compact_and_desktop_widths(int width, int height)
    {
        await Page.SetViewportSizeAsync(width, height);
        await OpenAsync();
        await ScenarioAsync("idm-person");

        var bounds = await Page.Locator(".ov-allocation__target-type")
            .EvaluateAsync<double[]>("node => { const outer = node.getBoundingClientRect(); const input = node.querySelector('.mud-input-control').getBoundingClientRect(); return [outer.width, input.width, input.right - outer.right]; }");
        Assert.True(bounds[0] <= width + 1, $"Target Type outer width {bounds[0]} exceeded viewport {width}.");
        Assert.True(bounds[1] <= bounds[0] + 1, $"Nested Target Type input width {bounds[1]} exceeded field width {bounds[0]}.");
        Assert.True(bounds[2] <= 1, $"Nested Target Type extends beyond field by {bounds[2]}px.");
    }

    private async Task OpenAsync()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/allocation");
        await Page.Locator(".ov-allocation__source").WaitForAsync();
    }

    private async Task ScenarioAsync(string value)
    {
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync(value);
        await Page.Locator(".ov-allocation__source").WaitForAsync();
    }

    private async Task CapabilityAsync(string name, bool enabled)
    {
        var toggle = Page.GetByRole(AriaRole.Checkbox, new() { Name = name, Exact = true });
        if (enabled) await toggle.CheckAsync();
        else await toggle.UncheckAsync();
    }
}
