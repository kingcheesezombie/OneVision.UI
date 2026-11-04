using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class ProgressiveConfigurationBuilderBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Default_state_shows_completed_current_and_pending_steps()
    {
        await OpenAsync();

        var steps = Page.Locator(".ov-configuration-step");
        Assert.Equal("completed", await steps.Nth(0).GetAttributeAsync("data-state"));
        Assert.Equal("absolute", await steps.Nth(0).Locator(".ov-configuration-step__state-label").EvaluateAsync<string>("element => getComputedStyle(element).position"));
        Assert.True(await steps.Nth(2).GetByRole(AriaRole.Combobox, new() { Name = "Source", Exact = true }).IsVisibleAsync());
        Assert.Equal("current", await steps.Nth(2).GetAttributeAsync("data-state"));
        Assert.Equal("pending", await steps.Nth(3).GetAttributeAsync("data-state"));
        Assert.Equal("step", await steps.Nth(2).GetAttributeAsync("aria-current"));
    }

    [Fact]
    public async Task Any_state_is_presented_as_valid_completed_wildcard_values()
    {
        await OpenAsync();
        await ChooseDemoStateAsync("Any / wildcard selections");

        var anyValues = Page.Locator(".ov-configuration-step__value--any");
        Assert.Equal(3, await anyValues.CountAsync());
        Assert.True(await anyValues.GetByText("Any type", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await anyValues.GetByText("Any source", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await anyValues.GetByText("Any target type", new() { Exact = true }).IsVisibleAsync());
        var targetTypeStep = Page.Locator(".ov-configuration-step").Nth(4);
        Assert.Equal("completed", await targetTypeStep.GetAttributeAsync("data-state"));
        Assert.Equal("absolute", await targetTypeStep.Locator(".ov-configuration-step__semantics").EvaluateAsync<string>("element => getComputedStyle(element).position"));
        Assert.True(await Page.GetByRole(AriaRole.Button, new() { Name = "Save rule", Exact = true }).IsEnabledAsync());
    }

    [Fact]
    public async Task Conditional_target_step_can_be_hidden_by_application_state()
    {
        await OpenAsync();
        await ChooseDemoStateAsync("Conditional target step hidden");

        Assert.Equal(0, await Page.GetByText("Target Entity Type", new() { Exact = true }).CountAsync());
        Assert.True(await Page.GetByText("ASSET · ORGANISATION_UNIT · ESR", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await Page.GetByText("ODM", new() { Exact = true }).Last.IsVisibleAsync());
    }

    [Fact]
    public async Task Change_then_selecting_an_earlier_value_resets_dependent_steps()
    {
        await OpenAsync();
        await ChooseDemoStateAsync("Completed rule");

        var firstStep = Page.Locator(".ov-configuration-step").Nth(0);
        await firstStep.GetByRole(AriaRole.Button, new() { Name = "Change Class", Exact = true }).ClickAsync();
        var classSelect = firstStep.GetByRole(AriaRole.Combobox, new() { Name = "Class", Exact = true });
        await classSelect.ClickAsync();
        await Page.GetByText("ATTRIBUTE", new() { Exact = true }).Last.ClickAsync();

        var steps = Page.Locator(".ov-configuration-step");
        Assert.Equal("current", await steps.Nth(1).GetAttributeAsync("data-state"));
        Assert.Equal("pending", await steps.Nth(2).GetAttributeAsync("data-state"));
        Assert.True(await Page.GetByRole(AriaRole.Button, new() { Name = "Save rule", Exact = true }).IsDisabledAsync());
    }

    [Fact]
    public async Task Validation_state_marks_step_invalid_and_surfaces_message()
    {
        await OpenAsync();
        await ChooseDemoStateAsync("Validation state");

        var sourceStep = Page.Locator(".ov-configuration-step").Nth(2);
        Assert.Equal("true", await sourceStep.GetAttributeAsync("aria-invalid"));
        Assert.Equal("error", await sourceStep.GetAttributeAsync("data-state"));
        Assert.True(await sourceStep.GetByText("Choose a source or explicitly use Any source.", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await Page.GetByText("The rule is incomplete. Resolve the highlighted step before saving.", new() { Exact = true }).IsVisibleAsync());
    }

    [Fact]
    public async Task Completed_state_renders_live_human_readable_preview()
    {
        await OpenAsync();
        await ChooseDemoStateAsync("Completed rule");

        var preview = Page.Locator(".ov-configuration-preview");
        Assert.True(await preview.GetByText("ASSET · ORGANISATION_UNIT · ESR", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await preview.GetByText("EAM · LOCATION", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await Page.GetByRole(AriaRole.Button, new() { Name = "Save rule", Exact = true }).IsEnabledAsync());
    }

    [Theory]
    [InlineData(1280, 900)]
    [InlineData(760, 900)]
    [InlineData(390, 844)]
    public async Task Pattern_does_not_add_horizontal_overflow(int width, int height)
    {
        await Page.SetViewportSizeAsync(width, height);
        await OpenAsync();

        var builder = Page.Locator(".ov-progressive-builder");
        Assert.True(await builder.EvaluateAsync<bool>("element => element.scrollWidth <= element.clientWidth + 1"));
        var pageWidth = await Page.EvaluateAsync<int>("document.documentElement.scrollWidth");
        Assert.True(pageWidth <= width + 2, $"Progressive configuration builder overflowed at {width}px: {pageWidth}px.");
    }

    [Fact]
    public async Task Completed_rows_are_compact_and_current_editor_is_visually_primary()
    {
        await OpenAsync();
        var steps = Page.Locator(".ov-configuration-step");
        var completed = steps.Nth(0);
        var current = steps.Nth(2);
        var heights = await Page.EvaluateAsync<double[]>(
            "() => { const elements = document.querySelectorAll('.ov-configuration-step'); return [elements[0].getBoundingClientRect().height, elements[2].getBoundingClientRect().height]; }");
        Assert.True(heights[0] < heights[1], $"Completed row {heights[0]}px should be shorter than current editor {heights[1]}px.");
        Assert.True(await completed.Locator(".ov-configuration-step__completed-value").IsVisibleAsync());
        Assert.True(await current.Locator(".ov-configuration-step__editor").IsVisibleAsync());
        Assert.False(await completed.Locator(".ov-configuration-step__heading p").IsVisibleAsync());
    }

    [Fact]
    public async Task Demo_exposes_initial_and_long_value_states_without_overflow()
    {
        await Page.SetViewportSizeAsync(390, 844);
        await OpenAsync();
        await ChooseDemoStateAsync("Initial configuration");
        Assert.Equal("current", await Page.Locator(".ov-configuration-step").First.GetAttributeAsync("data-state"));
        await ChooseDemoStateAsync("Long selected values");
        Assert.True(await Page.GetByText("Regional service delivery organisation unit — Northern locality", new() { Exact = true }).First.IsVisibleAsync());
        var pageWidth = await Page.EvaluateAsync<int>("document.documentElement.scrollWidth");
        var overflowing = await Page.EvaluateAsync<string[]>(
            "() => Array.from(document.querySelectorAll('body *')).filter(element => element.getBoundingClientRect().right > window.innerWidth + 2).slice(0,8).map(element => element.tagName + '.' + String(element.className).slice(0,90))");
        Assert.True(pageWidth <= 392, $"Long-value state overflowed: {pageWidth}px; elements: {string.Join(", ", overflowing)}");
    }

    [Fact]
    public async Task Semantic_state_text_remains_in_dom_for_assistive_technology()
    {
        await OpenAsync();
        var steps = Page.Locator(".ov-configuration-step");
        Assert.Equal("Completed", await steps.First.Locator(".ov-configuration-step__state-label").TextContentAsync());
        Assert.Equal("Current", await steps.Nth(2).Locator(".ov-configuration-step__state-label").TextContentAsync());
        Assert.Equal("Pending", await steps.Nth(3).Locator(".ov-configuration-step__state-label").TextContentAsync());
    }

    [Fact]
    public async Task Pattern_is_axe_clean()
    {
        await OpenAsync();
        var result = await Page.Locator(".ov-progressive-builder").RunAxe();
        var violations = result.Violations?.ToArray() ?? [];
        Assert.True(violations.Length == 0, string.Join(Environment.NewLine, violations.Select(v => $"{v.Id}: {v.Description}")));
    }

    private async Task OpenAsync()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/progressive-configuration-builder");
        await Page.Locator(".ov-progressive-builder").WaitForAsync();
    }

    private async Task ChooseDemoStateAsync(string label)
    {
        var select = Page.GetByRole(AriaRole.Combobox, new() { Name = "Demo state", Exact = true });
        await select.ClickAsync();
        await Page.GetByText(label, new() { Exact = true }).Last.ClickAsync();
    }
}
