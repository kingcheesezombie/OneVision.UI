using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class GovernedCapabilityQualityBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Schedule_builder_capabilities_hide_optional_items_and_guard_actions_during_save()
    {
        await OpenAsync("schedule-builder");
        Assert.True(await Page.GetByRole(AriaRole.Button, new() { Name = "Save schedule" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Item assignment" }).UncheckAsync();
        Assert.Equal(0, await Page.GetByText("Choose the items committed to this schedule.", new() { Exact = true }).CountAsync());
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Saving" }).CheckAsync();
        Assert.True(await Page.GetByText("Saving schedule…").IsVisibleAsync());
        Assert.True(await Page.GetByRole(AriaRole.Button, new() { Name = "Save schedule" }).IsDisabledAsync());
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Saving" }).UncheckAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Save error" }).CheckAsync();
        Assert.True(await Page.GetByText("Could not save schedule.", new() { Exact = false }).IsVisibleAsync());
    }

    [Fact]
    public async Task Connection_editor_capabilities_hide_test_and_preserve_settings_during_error()
    {
        await OpenAsync("connection-editor");
        Assert.True(await Page.GetByRole(AriaRole.Button, new() { Name = "Test connection" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Test connection action" }).UncheckAsync();
        Assert.Equal(0, await Page.GetByRole(AriaRole.Button, new() { Name = "Test connection" }).CountAsync());
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Save error" }).CheckAsync();
        Assert.True(await Page.GetByText("Could not save connection.", new() { Exact = false }).IsVisibleAsync());
        Assert.True(await Page.GetByLabel("Server", new() { Exact = true }).IsVisibleAsync());
    }

    [Fact]
    public async Task Tree_and_mapping_refresh_feedback_retain_the_existing_structure()
    {
        await OpenAsync("tree");
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Recoverable refresh failure" }).CheckAsync();
        Assert.True(await Page.GetByText("Existing nodes and selection remain available.", new() { Exact = false }).IsVisibleAsync());
        Assert.True(await Page.GetByText("Team North", new() { Exact = true }).First.IsVisibleAsync());
        await OpenAsync("mapping-pair");
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Refresh in progress" }).CheckAsync();
        Assert.True(await Page.GetByText("Refreshing mapping fields…").IsVisibleAsync());
        Assert.True(await Page.GetByText("Target contract", new() { Exact = true }).First.IsVisibleAsync());
    }

    private async Task OpenAsync(string slug)
    {
        await Page.GotoAsync($"{BaseUrl}/components/{slug}");
        await Page.GetByText("Independent capabilities", new() { Exact = true }).WaitForAsync();
    }
}
