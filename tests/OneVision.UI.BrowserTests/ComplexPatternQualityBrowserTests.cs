using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class ComplexPatternQualityBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task List_page_paginates_real_results_and_recovers_from_load_error()
    {
        await OpenAsync("list-page");
        var paging = Page.GetByRole(AriaRole.Navigation, new() { Name = "Example list pages" });
        Assert.True(await paging.GetByText("Page 1 of 2", new() { Exact = true }).IsVisibleAsync());
        await paging.GetByRole(AriaRole.Button, new() { Name = "Next page" }).ClickAsync();
        Assert.True(await Page.GetByText("Example service 26", new() { Exact = true }).IsVisibleAsync());
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("refreshing");
        Assert.True(await Page.GetByText("Community Nursing", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await Page.GetByText("Refreshing results…").IsVisibleAsync());
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("error");
        Assert.Equal(0, await Page.GetByRole(AriaRole.Table, new() { Name = "Example list records" }).CountAsync());
        await Page.GetByRole(AriaRole.Button, new() { Name = "Retry", Exact = true }).ClickAsync();
        Assert.True(await Page.GetByText("Community Nursing", new() { Exact = true }).IsVisibleAsync());
    }

    [Fact]
    public async Task Catalogue_refresh_error_retains_records_and_editor_error_retains_fields()
    {
        await OpenAsync("catalogue-management");
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("refresh-error");
        Assert.True(await Page.GetByText("Forename", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await Page.GetByText("Could not refresh the catalogue.", new() { Exact = false }).IsVisibleAsync());
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("save-error");
        Assert.True(await Page.GetByRole(AriaRole.Dialog, new() { Name = "Edit Standard Detail" }).IsVisibleAsync());
        Assert.True(await Page.GetByText("Could not save this item.", new() { Exact = false }).IsVisibleAsync());
        Assert.True(await Page.GetByLabel("Name", new() { Exact = true }).IsVisibleAsync());
    }

    [Fact]
    public async Task Source_configuration_restricts_mapping_until_current_and_keeps_save_error_visible()
    {
        await OpenAsync("source-configuration");
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("initial");
        await Page.GetByRole(AriaRole.Tab, new() { Name = "Mapping", Exact = true }).ClickAsync();
        Assert.True(await Page.GetByText("Mapping requires a current profile", new() { Exact = true }).IsVisibleAsync());
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("save-error");
        Assert.True(await Page.GetByText("Could not save this source.", new() { Exact = false }).IsVisibleAsync());
        Assert.True(await Page.GetByRole(AriaRole.Tab, new() { Name = "Mapping", Exact = true }).IsVisibleAsync());
    }

    [Fact]
    public async Task Progressive_builder_preserves_choices_on_error_and_can_complete()
    {
        await OpenAsync("progressive-configuration-builder");
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("submission-error");
        Assert.True(await Page.GetByText("The rule could not be saved.", new() { Exact = false }).IsVisibleAsync());
        Assert.True(await Page.GetByText("Rule preview", new() { Exact = true }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save rule", Exact = true }).ClickAsync();
        Assert.True(await Page.GetByText("Configuration saved.", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal(0, await Page.GetByRole(AriaRole.Button, new() { Name = "Save rule", Exact = true }).CountAsync());
    }

    [Fact]
    public async Task Api_builder_keeps_request_visible_through_failed_save_and_busy_state()
    {
        await OpenAsync("api-call-builder");
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("save-error");
        Assert.True(await Page.GetByText("The request could not be saved.", new() { Exact = false }).IsVisibleAsync());
        Assert.True(await Page.GetByText("Resolved path", new() { Exact = true }).IsVisibleAsync());
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("saving");
        Assert.True(await Page.GetByText("Saving API request…").IsVisibleAsync());
        Assert.True(await Page.GetByText("Resolved path", new() { Exact = true }).IsVisibleAsync());
    }

    [Fact]
    public async Task Inspector_busy_blocks_close_until_action_completes_but_preserves_details()
    {
        await OpenAsync("entity-inspector");
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("busy");
        var dialog = Page.GetByRole(AriaRole.Dialog);
        Assert.Equal("true", await dialog.GetAttributeAsync("aria-busy"));
        Assert.True(await Page.GetByText("Processing entity action…").IsVisibleAsync());
        await Page.GetByRole(AriaRole.Button, new() { Name = "Finish simulated action" }).ClickAsync();
        Assert.Equal("false", await dialog.GetAttributeAsync("aria-busy"));
        await dialog.GetByRole(AriaRole.Button, new() { Name = "Close overlay" }).ClickAsync();
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("action-error");
        Assert.True(await Page.GetByText("The requested example action failed.", new() { Exact = false }).IsVisibleAsync());
        Assert.True(await Page.GetByText("Region", new() { Exact = true }).IsVisibleAsync());
    }

    private async Task OpenAsync(string slug)
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/{slug}");
        await Page.Locator("#ov-pattern-scenario").WaitForAsync();
    }
}
