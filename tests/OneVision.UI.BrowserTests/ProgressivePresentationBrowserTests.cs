using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class ProgressivePresentationBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    private async Task OpenAllocationReferenceAsync(string scenario = "default")
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/progressive-configuration-builder");
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync(scenario);
        await Page.Locator(".ov-progressive-builder").WaitForAsync();
    }

    [Theory]
    [InlineData(1280, 900)]
    [InlineData(760, 900)]
    [InlineData(390, 844)]
    public async Task All_builder_regions_fill_the_same_parent_width_without_overflow(int width, int height)
    {
        await Page.SetViewportSizeAsync(width, height);
        await OpenAllocationReferenceAsync();

        var builder = Page.Locator(".ov-progressive-builder");
        Assert.Equal("fullwidth", await builder.GetAttributeAsync("data-layout"));
        var widths = await builder.EvaluateAsync<double[]>(
            @"root => {
                const parentWidth = root.getBoundingClientRect().width;
                const selectors = [
                    '.ov-progressive-builder__header', '.ov-progressive-builder__body',
                    '.ov-progressive-builder__preview', '.ov-progressive-builder__validation',
                    '.ov-progressive-builder__footer', '.ov-configuration-section',
                    '.ov-configuration-relationship'
                ];
                return [parentWidth, ...selectors.map(selector => root.querySelector(selector)?.getBoundingClientRect().width ?? -1)];
            }");
        Assert.All(widths.Skip(1), w => Assert.InRange(Math.Abs(w - widths[0]), 0, 2.5));
        Assert.True(await builder.EvaluateAsync<bool>("root => root.scrollWidth <= root.clientWidth + 2"));
        Assert.True(await Page.EvaluateAsync<bool>("() => document.documentElement.scrollWidth <= window.innerWidth + 2"));
    }

    [Fact]
    public async Task Constrained_mode_is_explicit_and_can_be_released_to_full_width()
    {
        await Page.SetViewportSizeAsync(1440, 900);
        await OpenAllocationReferenceAsync();
        var builder = Page.Locator(".ov-progressive-builder");

        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Constrained builder width" }).CheckAsync();
        Assert.Equal("constrained", await builder.GetAttributeAsync("data-layout"));
        Assert.True(await builder.EvaluateAsync<bool>("root => root.getBoundingClientRect().width <= 770"));
        Assert.True(await builder.EvaluateAsync<bool>("root => root.querySelector('.ov-progressive-builder__body').getBoundingClientRect().width >= root.getBoundingClientRect().width - 2"));

        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Constrained builder width" }).UncheckAsync();
        Assert.Equal("fullwidth", await builder.GetAttributeAsync("data-layout"));
    }

    [Fact]
    public async Task Preview_presentation_toggle_preserves_the_underlying_rule_content()
    {
        await OpenAllocationReferenceAsync("completed");
        var preview = Page.Locator(".ov-configuration-preview");
        Assert.True(await preview.EvaluateAsync<bool>("element => element.classList.contains('ov-configuration-preview--emphasized')"));
        Assert.Equal("readytosave", await preview.GetAttributeAsync("data-preview-state"));
        Assert.Contains("Ready to save", await preview.Locator(".ov-configuration-preview__state").InnerTextAsync());

        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Emphasised summary preview" }).UncheckAsync();
        Assert.True(await preview.EvaluateAsync<bool>("element => element.classList.contains('ov-configuration-preview--standard')"));
        Assert.Equal(0, await preview.GetByText("Ready to save", new() { Exact = true }).CountAsync());
        Assert.True(await preview.Locator(".ov-progressive-demo__preview-region").First.GetByText("ESR", new() { Exact = true }).IsVisibleAsync());
    }

    [Fact]
    public async Task Incomplete_selection_shows_unresolved_values_without_implying_wildcards()
    {
        await OpenAllocationReferenceAsync("initial");
        var preview = Page.Locator(".ov-configuration-preview");
        Assert.Equal("incomplete", await preview.GetAttributeAsync("data-preview-state"));
        Assert.Contains("Incomplete", await preview.Locator(".ov-configuration-preview__state").InnerTextAsync());
        Assert.True(await preview.Locator(".ov-progressive-demo__preview-unresolved").CountAsync() >= 4);
        Assert.Equal(0, await preview.GetByText("Any source", new() { Exact = true }).CountAsync());
        Assert.Equal(0, await preview.GetByText("Any type", new() { Exact = true }).CountAsync());
        Assert.Equal(0, await preview.GetByText("Ready to save", new() { Exact = true }).CountAsync());

        await Page.GetByRole(AriaRole.Combobox, new() { Name = "Demo state", Exact = true }).ClickAsync();
        await Page.GetByText("Any / wildcard selections", new() { Exact = true }).Last.ClickAsync();
        Assert.Equal("readytosave", await preview.GetAttributeAsync("data-preview-state"));
        Assert.True(await preview.GetByText("Any type", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await preview.GetByText("Any source", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await preview.GetByText("Any target type", new() { Exact = true }).IsVisibleAsync());
    }

    [Theory]
    [InlineData(1280, 900)]
    [InlineData(390, 844)]
    public async Task Source_to_target_direction_and_accessibility_survive_responsive_layout(int width, int height)
    {
        await Page.SetViewportSizeAsync(width, height);
        await OpenAllocationReferenceAsync("completed");

        var flow = Page.Locator(".ov-progressive-demo__preview-flow");
        var region = flow.Locator(".ov-progressive-demo__preview-region");
        var metrics = await flow.EvaluateAsync<double[]>(
            @"element => {
                const source = element.querySelectorAll('.ov-progressive-demo__preview-region')[0].getBoundingClientRect();
                const direction = element.querySelector('.ov-progressive-demo__preview-relationship').getBoundingClientRect();
                const target = element.querySelectorAll('.ov-progressive-demo__preview-region')[1].getBoundingClientRect();
                return [source.right, direction.left, target.left, source.bottom, direction.top, target.top];
            }");
        if (width > 760)
            Assert.True(metrics[0] <= metrics[1] + 2 && metrics[1] <= metrics[2], "Desktop preview must be source → relationship → target.");
        else
            Assert.True(metrics[3] <= metrics[4] + 2 && metrics[4] <= metrics[5], "Mobile preview must stack source → relationship → target.");

        Assert.Equal("Source population", await region.First.GetAttributeAsync("aria-label"));
        Assert.Equal("Target endpoint", await region.Last.GetAttributeAsync("aria-label"));
        Assert.True(await flow.EvaluateAsync<bool>("element => element.scrollWidth <= element.clientWidth + 2"));
        var violations = (await Page.Locator(".ov-configuration-preview").RunAxe()).Violations?.ToArray() ?? [];
        Assert.True(violations.Length == 0, string.Join(Environment.NewLine, violations.Select(v => v.Id + ": " + v.Description)));
    }
}
