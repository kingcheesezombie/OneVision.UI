using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class CatalogueManagementBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Add_and_edit_use_governed_catalogue_overlay()
    {
        await OpenAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Add Standard Detail", Exact = true }).ClickAsync();
        await Page.GetByRole(AriaRole.Dialog, new() { Name = "Add Standard Detail", Exact = true }).WaitForAsync();
        Assert.True(await Page.GetByRole(AriaRole.Button, new() { Name = "Save", Exact = true }).IsDisabledAsync());

        await Page.GetByLabel("Name", new() { Exact = true }).FillAsync("Example Detail");
        Assert.False(await Page.GetByRole(AriaRole.Button, new() { Name = "Save", Exact = true }).IsDisabledAsync());
    }

    [Fact]
    public async Task Read_only_catalogue_retains_search_but_suppresses_mutation_actions()
    {
        await OpenAsync();
        await ChooseStateAsync("Read-only catalogue");
        Assert.Equal(0, await Page.GetByRole(AriaRole.Button, new() { Name = "Add Standard Detail", Exact = true }).CountAsync());
        Assert.Equal(0, await Page.GetByRole(AriaRole.Button, new() { Name = "Edit", Exact = true }).CountAsync());
        Assert.True(await Page.GetByPlaceholder("Search standard details...", new() { Exact = true }).IsVisibleAsync());
    }

    [Theory]
    [InlineData(1280, 900)]
    [InlineData(760, 900)]
    [InlineData(390, 844)]
    public async Task Catalogue_management_reflows_without_feature_horizontal_overflow(int width, int height)
    {
        await Page.SetViewportSizeAsync(width, height);
        await OpenAsync();
        var pattern = Page.Locator(".ov-catalogue-demo");
        Assert.True(await pattern.EvaluateAsync<bool>("element => element.scrollWidth <= element.clientWidth + 1"));
    }

    [Fact]
    public async Task Catalogue_management_is_axe_clean()
    {
        await OpenAsync();
        var result = await Page.Locator(".ov-catalogue-demo").RunAxe();
        var violations = result.Violations?.ToArray() ?? [];
        Assert.True(violations.Length == 0, string.Join(Environment.NewLine, violations.Select(v => $"{v.Id}: {v.Description}")));
    }

    private async Task OpenAsync()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/catalogue-management");
        await Page.GetByRole(AriaRole.Combobox, new() { Name = "Demo state", Exact = true }).WaitForAsync();
    }

    private async Task ChooseStateAsync(string label)
    {
        var options = new[] { "Populated catalogue", "Empty catalogue", "Filtered / no results", "Add overlay", "Edit overlay", "Read-only catalogue", "Loading", "Error" };
        var index = Array.IndexOf(options, label);
        var select = Page.GetByRole(AriaRole.Combobox, new() { Name = "Demo state", Exact = true });
        await select.ClickAsync();
        await Page.Keyboard.PressAsync("Home");
        for (var step = 0; step < index; step++) await Page.Keyboard.PressAsync("ArrowDown");
        await Page.Keyboard.PressAsync("Enter");
        await select.GetByText(label, new() { Exact = true }).WaitForAsync();
    }
}
