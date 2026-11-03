using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class DisplayValueBuilderBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Display_value_is_an_optional_peer_tab_and_not_embedded_in_mapping()
    {
        await OpenAsync();

        Assert.True(await Page.GetByRole(AriaRole.Tab, new() { Name = "Display Value", Exact = true }).IsVisibleAsync());
        Assert.Equal("true", await Page.GetByRole(AriaRole.Tab, new() { Name = "Display Value", Exact = true }).GetAttributeAsync("aria-selected"));
        Assert.True(await Page.Locator(".ov-display-value-builder").IsVisibleAsync());

        await Page.GetByRole(AriaRole.Tab, new() { Name = "Mapping", Exact = true }).ClickAsync();
        Assert.Equal(0, await Page.Locator(".ov-display-value-builder").CountAsync());
        Assert.True(await Page.GetByText("Completed mappings", new() { Exact = true }).IsVisibleAsync());
    }

    [Fact]
    public async Task Builder_offers_only_completed_mapping_targets_as_grouped_pills()
    {
        await OpenAsync();
        var builder = Page.Locator(".ov-display-value-builder");

        Assert.True(await builder.GetByText("Core fields", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await builder.GetByText("Standard Details", new() { Exact = true }).IsVisibleAsync());
        foreach (var field in new[] { "Identifier", "Description", "Service", "Locality", "Organisation" })
            Assert.True(await builder.GetByRole(AriaRole.Button, new() { Name = $"Add mapped field {field}", Exact = true }).IsVisibleAsync());

        Assert.Equal(0, await builder.GetByText("source-description", new() { Exact = true }).CountAsync());
        Assert.Equal(0, await builder.GetByText("$.description", new() { Exact = true }).CountAsync());
    }

    [Fact]
    public async Task Field_and_separator_buttons_append_mixed_tokens_and_update_preview()
    {
        await OpenAsync();
        await ChooseDemoStateAsync("No mappings empty state");
        Assert.True(await Page.GetByText("No completed mappings yet", new() { Exact = true }).IsVisibleAsync());

        await ChooseDemoStateAsync("Configured mixed separators");
        var builder = Page.Locator(".ov-display-value-builder");
        Assert.True(await builder.GetByText("Children & Families (North Locality) | BU-1042", new() { Exact = true }).IsVisibleAsync());

        await builder.GetByRole(AriaRole.Button, new() { Name = "Add separator slash", Exact = true }).ClickAsync();
        await builder.GetByRole(AriaRole.Button, new() { Name = "Add mapped field Service", Exact = true }).ClickAsync();

        Assert.True(await builder.GetByText("Children & Families (North Locality) | BU-1042 / Community", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await builder.GetByRole(AriaRole.Button, new() { Name = "Add separator open parenthesis", Exact = true }).IsVisibleAsync());
        Assert.True(await builder.GetByRole(AriaRole.Button, new() { Name = "Add separator close parenthesis", Exact = true }).IsVisibleAsync());
    }

    [Fact]
    public async Task Composition_tokens_drag_to_reorder_without_visible_move_buttons()
    {
        var asset = await Page.APIRequest.GetAsync($"{BaseUrl}/_content/OneVision.UI/ov-display-value-dnd.js");
        Assert.True(asset.Ok, $"Display Value drag helper was not served: {asset.Status}.");

        await OpenAsync();
        var builder = Page.Locator(".ov-display-value-builder");
        await builder.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await Page.WaitForFunctionAsync("element => element.dataset.dragReady === 'true'", await builder.ElementHandleAsync());
        Assert.Equal("true", await builder.GetAttributeAsync("data-drag-ready"));
        Assert.Equal(0, await builder.GetByRole(AriaRole.Button, new() { NameRegex = new("^Move segment") }).CountAsync());

        var identifier = builder.Locator("[data-segment-id='identifier']");
        var description = builder.Locator("[data-segment-id='description']");
        await identifier.DragToAsync(description);

        var tokens = builder.Locator(".ov-display-value-builder__segment");
        Assert.Contains("Identifier", await tokens.First.InnerTextAsync());
    }

    [Fact]
    public async Task Composition_tokens_support_hidden_keyboard_reordering()
    {
        await OpenAsync();
        var builder = Page.Locator(".ov-display-value-builder");
        var identifier = builder.Locator("[data-segment-id='identifier']");
        await identifier.FocusAsync();
        await identifier.PressAsync("Alt+ArrowLeft");

        var tokens = builder.Locator(".ov-display-value-builder__segment");
        var penultimate = await tokens.Nth(await tokens.CountAsync() - 2).InnerTextAsync();
        Assert.Contains("Identifier", penultimate);
    }

    [Fact]
    public async Task Removed_mapping_reference_stays_visibly_unresolved_and_invalid()
    {
        await OpenAsync();
        await ChooseDemoStateAsync("Removed mapped field");

        Assert.True(await Page.GetByText("Mapped field removed", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await Page.GetByText("One or more fields used by this display value are no longer present in Mapping. Replace or remove them before saving.", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await Page.GetByRole(AriaRole.Button, new() { Name = "Save source", Exact = true }).IsDisabledAsync());
    }

    [Fact]
    public async Task Read_only_state_keeps_composition_and_preview_but_suppresses_mutation()
    {
        await OpenAsync();
        await ChooseDemoStateAsync("Read-only");
        var builder = Page.Locator(".ov-display-value-builder");

        Assert.True(await builder.GetByText("Read only", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await builder.GetByText("Children & Families (North Locality) | BU-1042", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal(0, await builder.GetByRole(AriaRole.Button, new() { NameRegex = new("^Add mapped field") }).CountAsync());
        Assert.Equal(0, await builder.GetByRole(AriaRole.Button, new() { NameRegex = new("^Add separator") }).CountAsync());
        Assert.Equal(0, await builder.GetByRole(AriaRole.Button, new() { NameRegex = new("^Remove ") }).CountAsync());
    }

    [Theory]
    [InlineData(1280, 900)]
    [InlineData(760, 900)]
    [InlineData(390, 844)]
    public async Task Builder_wraps_without_page_horizontal_overflow(int width, int height)
    {
        await Page.SetViewportSizeAsync(width, height);
        await OpenAsync();
        var builder = Page.Locator(".ov-display-value-builder");
        Assert.True(await builder.EvaluateAsync<bool>("element => element.scrollWidth <= element.clientWidth + 1"));
        var pageWidth = await Page.EvaluateAsync<int>("document.documentElement.scrollWidth");
        Assert.True(pageWidth <= width + 2, $"Display Value Builder overflowed at {width}px: {pageWidth}px.");
    }

    [Fact]
    public async Task Builder_is_axe_clean()
    {
        await OpenAsync();
        var result = await Page.Locator(".ov-display-value-builder").RunAxe();
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
        await Page.GetByText(label, new() { Exact = true }).Last.ClickAsync();
    }
}
