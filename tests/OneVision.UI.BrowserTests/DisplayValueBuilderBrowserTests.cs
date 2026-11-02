using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class DisplayValueBuilderBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Builder_uses_mapping_targets_and_groups_core_and_standard_details()
    {
        await OpenAsync();

        var builder = Page.Locator(".ov-display-value-builder");
        Assert.True(await builder.GetByText("Core fields", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await builder.GetByText("Standard Details", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await builder.GetByText("Display name", new() { Exact = true }).First.IsVisibleAsync());
        Assert.True(await builder.GetByText("Reference code", new() { Exact = true }).First.IsVisibleAsync());
        Assert.True(await builder.GetByText("Email Address", new() { Exact = true }).First.IsVisibleAsync());
        Assert.True(await builder.GetByText("Position", new() { Exact = true }).First.IsVisibleAsync());
        Assert.Equal(0, await builder.GetByText("forename", new() { Exact = true }).CountAsync());
    }

    [Fact]
    public async Task Builder_shows_readable_live_preview_without_raw_template_syntax()
    {
        await OpenAsync();

        var preview = Page.Locator(".ov-display-value-builder__preview");
        Assert.True(await preview.GetByText("John Smith (12345678)", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal(0, await Page.GetByText("{display-name}", new() { Exact = true }).CountAsync());
        Assert.Equal(0, await Page.GetByText("{reference-code}", new() { Exact = true }).CountAsync());
    }

    [Fact]
    public async Task Removed_standard_detail_becomes_visibly_unresolved_and_invalid()
    {
        await OpenAsync();
        await ChooseDemoStateAsync("Removed Standard Detail");

        Assert.True(await Page.GetByText("Mapped field removed", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await Page.GetByText("One or more fields used by this display value are no longer present in Mapping. Replace or remove them before saving.", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await Page.GetByRole(AriaRole.Button, new() { Name = "Save source", Exact = true }).IsDisabledAsync());
    }

    [Fact]
    public async Task Read_only_state_keeps_composition_and_preview_but_hides_mutation_controls()
    {
        await OpenAsync();
        await ChooseDemoStateAsync("Read-only");

        var builder = Page.Locator(".ov-display-value-builder");
        Assert.True(await builder.GetByText("Read only", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await builder.GetByText("John Smith (12345678)", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal(0, await builder.GetByRole(AriaRole.Button, new() { NameRegex = new("^Add ") }).CountAsync());
        Assert.Equal(0, await builder.GetByRole(AriaRole.Button, new() { NameRegex = new("^Move segment") }).CountAsync());
        Assert.Equal(0, await builder.GetByRole(AriaRole.Button, new() { NameRegex = new("^Remove segment") }).CountAsync());
    }

    [Theory]
    [InlineData(1280, 900)]
    [InlineData(760, 900)]
    [InlineData(390, 844)]
    public async Task Builder_reflows_without_adding_page_horizontal_overflow(int width, int height)
    {
        await Page.SetViewportSizeAsync(width, height);

        await Page.GotoAsync($"{BaseUrl}/patterns/source-configuration");
        await Page.GetByRole(AriaRole.Tab, new() { Name = "Mapping", Exact = true }).ClickAsync();
        await Page.GetByText("Pair mapping", new() { Exact = true }).WaitForAsync();
        var baselineWidth = await Page.EvaluateAsync<int>("document.documentElement.scrollWidth");

        await Page.GotoAsync($"{BaseUrl}/patterns/source-configuration/display-value-builder");
        var builder = Page.Locator(".ov-display-value-builder");
        await builder.WaitForAsync();

        Assert.True(await builder.EvaluateAsync<bool>("element => element.scrollWidth <= element.clientWidth + 1"));

        var finalWidth = await Page.EvaluateAsync<int>("document.documentElement.scrollWidth");
        var allowedWidth = Math.Max(width + 1, baselineWidth + 1);
        Assert.True(finalWidth <= allowedWidth, $"Display Value Builder widened the existing Mapping shell at {width}px: baseline {baselineWidth}px, final {finalWidth}px.");
    }

    [Fact]
    public async Task Builder_is_axe_clean()
    {
        await OpenAsync();
        var builder = Page.Locator(".ov-display-value-builder");
        var result = await builder.RunAxe();
        var violations = result.Violations?.ToArray() ?? [];
        Assert.True(violations.Length == 0, string.Join(Environment.NewLine, violations.Select(v => $"{v.Id}: {v.Description}")));
    }

    private async Task OpenAsync()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/source-configuration/display-value-builder");
        await Page.Locator(".ov-display-value-builder").WaitForAsync();
    }

    private async Task ChooseDemoStateAsync(string label)
    {
        var select = Page.GetByRole(AriaRole.Combobox, new() { Name = "Display builder demo state", Exact = true });
        await select.ClickAsync();
        await Page.GetByText(label, new() { Exact = true }).ClickAsync();
    }
}
