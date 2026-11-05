using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class ConnectionManagementBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Real_connection_management_composes_list_overlay_and_governed_editor()
    {
        await OpenAsync();
        Assert.True(await Page.GetByRole(AriaRole.Table, new() { Name = "Managed connection records" }).IsVisibleAsync());
        Assert.True(await Page.GetByText("Core data warehouse", new() { Exact = true }).IsVisibleAsync());

        await Page.GetByRole(AriaRole.Button, new() { Name = "Add connection", Exact = true }).ClickAsync();
        var editor = Page.GetByRole(AriaRole.Dialog, new() { Name = "Add connection" });
        Assert.True(await editor.IsVisibleAsync());
        Assert.Equal(1, await editor.GetByRole(AriaRole.Button, new() { Name = "Save connection" }).CountAsync());
        Assert.True(await editor.GetByRole(AriaRole.Button, new() { Name = "Test connection" }).IsVisibleAsync());
        Assert.True(await editor.GetByRole(AriaRole.Button, new() { Name = "Save connection" }).IsDisabledAsync());

        await editor.GetByRole(AriaRole.Textbox, new() { Name = "Name", Exact = true }).FillAsync("Sample managed SQL");
        await editor.GetByRole(AriaRole.Textbox, new() { Name = "Server", Exact = true }).FillAsync("sql.example.invalid");
        await editor.GetByRole(AriaRole.Textbox, new() { Name = "Database", Exact = true }).FillAsync("Example");
        await editor.GetByRole(AriaRole.Button, new() { Name = "Save connection" }).ClickAsync();
        Assert.Equal(0, await Page.GetByRole(AriaRole.Dialog, new() { Name = "Add connection" }).CountAsync());
        Assert.True(await Page.GetByText("Sample managed SQL", new() { Exact = true }).IsVisibleAsync());
    }

    [Fact]
    public async Task Failed_load_can_retry_without_recreating_page_and_optional_capabilities_are_real()
    {
        await OpenAsync();
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("error");
        Assert.True(await Page.GetByText("Connections could not be loaded.", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal(0, await Page.GetByRole(AriaRole.Table, new() { Name = "Managed connection records" }).CountAsync());
        await Page.GetByRole(AriaRole.Button, new() { Name = "Retry", Exact = true }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Table, new() { Name = "Managed connection records" }).IsVisibleAsync());

        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Search connections" }).UncheckAsync();
        Assert.Equal(0, await Page.GetByPlaceholder("Search connections").CountAsync());
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Add action" }).UncheckAsync();
        Assert.Equal(0, await Page.GetByRole(AriaRole.Button, new() { Name = "Add connection", Exact = true }).CountAsync());
    }

    [Fact]
    public async Task Test_failure_is_non_destructive_and_can_recover()
    {
        await OpenAsync();
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("test-error");
        var editor = Page.GetByRole(AriaRole.Dialog, new() { Name = "Edit connection" });
        Assert.True(await editor.IsVisibleAsync());
        Assert.True(await editor.GetByText("Connection could not be established.", new() { Exact = false }).IsVisibleAsync());
        await editor.GetByRole(AriaRole.Button, new() { Name = "Test connection" }).ClickAsync();
        Assert.True(await editor.GetByText("Synthetic connection test failed.", new() { Exact = false }).IsVisibleAsync());
        await editor.GetByRole(AriaRole.Button, new() { Name = "Test connection" }).ClickAsync();
        Assert.True(await editor.GetByText("Synthetic connection test succeeded", new() { Exact = false }).IsVisibleAsync());
        Assert.True(await editor.GetByRole(AriaRole.Textbox, new() { Name = "Server", Exact = true }).IsVisibleAsync());
    }

    private async Task OpenAsync()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/connection-management");
        await Page.Locator("#ov-pattern-scenario").WaitForAsync();
    }
}
