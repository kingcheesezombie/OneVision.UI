using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class AllocationPatternBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Known_target_shows_compact_source_search_and_hides_implicit_dates()
    {
        await OpenAsync();
        Assert.True(await Page.GetByText("Children & Families", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal(1, await Page.Locator(".ov-allocation__results").CountAsync());
        Assert.Equal(0, await Page.Locator(".ov-allocation__dates").CountAsync());
        Assert.True(await Page.Locator(".ov-allocation__fixed-value").GetByText("Attribute").IsVisibleAsync());

        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Explicit effective dates" }).CheckAsync();
        Assert.Equal(1, await Page.Locator(".ov-allocation__dates").CountAsync());
    }

    [Fact]
    public async Task Selected_source_replaces_results_and_can_resume_search()
    {
        await OpenAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Selected source" }).CheckAsync();
        Assert.Equal(1, await Page.Locator(".ov-allocation__selected-source").CountAsync());
        Assert.Equal(0, await Page.Locator(".ov-allocation__results").CountAsync());

        await Page.Locator(".ov-allocation__selected-source")
            .GetByRole(AriaRole.Button, new() { Name = "Change" }).ClickAsync();
        Assert.Equal(0, await Page.Locator(".ov-allocation__selected-source").CountAsync());
        Assert.Equal(1, await Page.Locator(".ov-allocation__results").CountAsync());
    }

    [Fact]
    public async Task Finder_supports_constrained_platforms_and_selected_identity()
    {
        await OpenAsync();
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("finder");
        Assert.Equal(1, await Page.Locator(".ov-allocation__identity-row").CountAsync());

        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Constrain target to EAM" }).CheckAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Selected target" }).UncheckAsync();
        Assert.Equal(0, await Page.Locator(".ov-allocation__platform").CountAsync());
        Assert.Equal(1, await Page.Locator(".ov-allocation__results[aria-label='Target results']").CountAsync());
        Assert.Equal(1, await Page.Locator(".ov-allocation__results[aria-label='Source results']").CountAsync());
    }

    [Theory]
    [InlineData(1280, 900)]
    [InlineData(390, 844)]
    public async Task Allocation_stays_within_compact_width(int width, int height)
    {
        await Page.SetViewportSizeAsync(width, height);
        await OpenAsync();
        var elementWidth = await Page.Locator(".ov-allocation").EvaluateAsync<double>("element => element.getBoundingClientRect().width");
        Assert.True(elementWidth <= 641 && elementWidth <= width + 1, $"Allocation width {elementWidth}px exceeded the compact bounds.");
    }

    [Fact]
    public async Task Selected_source_hides_criteria_search_and_results_but_preserves_identity()
    {
        await OpenAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Selected source" }).CheckAsync();
        Assert.Equal(0, await Page.Locator(".ov-allocation__search").CountAsync());
        Assert.Equal(0, await Page.Locator(".ov-allocation__source-criteria").CountAsync());
        Assert.Equal(0, await Page.Locator(".ov-allocation__results").CountAsync());
        Assert.Equal(1, await Page.Locator(".ov-allocation__selected-source").CountAsync());
        Assert.Equal(1, await Page.Locator(".ov-allocation__identity-row").CountAsync());
    }

    [Fact]
    public async Task Rule_constrained_reference_supports_search_selection_change_and_pagination()
    {
        await OpenAsync();
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("rule-constrained");
        Assert.Equal(1, await Page.Locator(".ov-allocation__source-criteria").CountAsync());
        Assert.True(await Page.GetByText("Community Nurse", new() { Exact = true }).IsVisibleAsync());
        await Page.Locator(".ov-demo-allocation-pagination")
            .GetByRole(AriaRole.Button, new() { Name = "Next" }).ClickAsync();
        Assert.True(await Page.GetByText("Advanced Practitioner", new() { Exact = true }).IsVisibleAsync());
        await Page.Locator(".ov-allocation__results input[type=radio]").First.CheckAsync();
        Assert.Equal(1, await Page.Locator(".ov-allocation__selected-source").CountAsync());
        Assert.Equal(0, await Page.Locator(".ov-allocation__search").CountAsync());
        await Page.Locator(".ov-allocation__selected-source")
            .GetByRole(AriaRole.Button, new() { Name = "Change" }).ClickAsync();
        Assert.Equal(1, await Page.Locator(".ov-allocation__results").CountAsync());
    }

    [Theory]
    [InlineData("searching", "Searching eligible sources")]
    [InlineData("empty", "No eligible sources")]
    [InlineData("unavailable", "Allocation eligibility is unavailable")]
    [InlineData("submitting", "Creating allocation")]
    [InlineData("submission-error", "The allocation could not be created")]
    [InlineData("completed", "Allocation created.")]
    public async Task Operational_state_preserves_target(string scenario, string message)
    {
        await OpenAsync();
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync(scenario);
        Assert.Equal(1, await Page.Locator(".ov-allocation__identity-row").CountAsync());
        Assert.True(await Page.GetByText("Children & Families", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await Page.GetByText(message, new() { Exact = false }).IsVisibleAsync());
    }

    [Fact]
    public async Task Overlay_composition_has_single_heading_and_single_action_footer()
    {
        await OpenAsync();
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("overlay");
        Assert.Equal(1, await Page.Locator(".ov-overlay__header h2").CountAsync());
        Assert.Equal(1, await Page.Locator(".ov-overlay__footer").CountAsync());
        Assert.Equal(0, await Page.Locator(".ov-allocation__page-header").CountAsync());
        Assert.Equal(0, await Page.Locator(".ov-allocation__footer").CountAsync());
        Assert.True(await Page.GetByRole(AriaRole.Dialog).IsVisibleAsync());
    }

    private async Task OpenAsync()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/allocation");
        await Page.Locator(".ov-allocation__source").WaitForAsync();
    }
}
