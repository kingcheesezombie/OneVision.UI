using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class GovernedGridWorkbenchBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Data_grid_gallery_selection_and_clear_actions_are_real()
    {
        await Page.GotoAsync($"{BaseUrl}/components/data-grid");
        var preview = Page.Locator(".ov-component-workbench__preview");
        await preview.GetByRole(AriaRole.Checkbox, new() { Name = "Select Example A" }).CheckAsync();
        Assert.True(await preview.GetByText("1 selected", new() { Exact = true }).IsVisibleAsync());
        await preview.GetByRole(AriaRole.Button, new() { Name = "Clear", Exact = true }).ClickAsync();
        Assert.Equal(0, await preview.GetByText("1 selected", new() { Exact = true }).CountAsync());
    }

    [Fact]
    public async Task Data_grid_error_state_has_real_retry_and_retains_table_after_refresh_failure()
    {
        await Page.GotoAsync($"{BaseUrl}/components/data-grid");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Error", Exact = true }).ClickAsync();
        var preview = Page.Locator(".ov-component-workbench__preview");
        Assert.True(await preview.GetByText("Could not load example records.", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal(0, await preview.GetByRole(AriaRole.Table, new() { Name = "Example governed records" }).CountAsync());
        await preview.GetByRole(AriaRole.Button, new() { Name = "Retry", Exact = true }).ClickAsync();
        Assert.True(await preview.GetByRole(AriaRole.Table, new() { Name = "Example governed records" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Recoverable refresh failure" }).CheckAsync();
        Assert.True(await preview.GetByText("Unable to refresh records; existing rows are retained.", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await preview.GetByRole(AriaRole.Table, new() { Name = "Example governed records" }).IsVisibleAsync());
    }
}
