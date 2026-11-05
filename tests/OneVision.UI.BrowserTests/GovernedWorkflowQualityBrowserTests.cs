using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class GovernedWorkflowQualityBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Hierarchy_editor_preserves_selected_node_through_overlay_and_return()
    {
        await OpenAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Open Northern Services hierarchy" }).ClickAsync();
        await Page.GetByText("Selected node", new() { Exact = true }).WaitForAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Edit selected node" }).ClickAsync();
        var dialog = Page.GetByRole(AriaRole.Dialog, new() { Name = "Edit node" });
        await dialog.GetByLabel("Node name").FillAsync("Northern Services Updated");
        await dialog.GetByRole(AriaRole.Button, new() { Name = "Save node" }).ClickAsync();
        Assert.True(await Page.GetByText("Northern Services Updated", new() { Exact = true }).First.IsVisibleAsync());

        await Page.GetByRole(AriaRole.Button, new() { Name = "Simulate refresh failure" }).ClickAsync();
        Assert.True(await Page.GetByText("The current selection is retained.", new() { Exact = false }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Button, new() { Name = "Back to catalogue" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Open Northern Services hierarchy" }).ClickAsync();
        Assert.True(await Page.GetByText("Northern Services Updated", new() { Exact = true }).First.IsVisibleAsync());
    }

    [Fact]
    public async Task Allocation_keeps_target_and_source_through_selection_create_and_inspection()
    {
        await OpenAsync();
        await Page.Locator("#ov-quality-workflow").SelectOptionAsync("allocation");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Allocate to Community Services" }).ClickAsync();
        var dialog = Page.GetByRole(AriaRole.Dialog, new() { Name = "Create allocation" });
        Assert.True(await dialog.GetByText("Community Services", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await dialog.GetByRole(AriaRole.Button, new() { Name = "Create Allocation" }).IsDisabledAsync());
        await dialog.Locator(".ov-selection-list__item input[type=radio]").First.ClickAsync();
        Assert.Equal(0, await dialog.Locator(".ov-allocation__results").CountAsync());
        await dialog.GetByRole(AriaRole.Button, new() { Name = "Create Allocation" }).ClickAsync();
        Assert.True(await Page.GetByText("Community Nurse · ESR", new() { Exact = true }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Button, new() { Name = "Inspect" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Dialog, new() { Name = "Community Services" }).IsVisibleAsync());
    }

    [Fact]
    public async Task Data_source_reference_composes_connection_profile_mapping_and_api_builder()
    {
        await OpenAsync();
        await Page.Locator("#ov-quality-workflow").SelectOptionAsync("source");
        Assert.True(await Page.GetByText("SQL settings", new() { Exact = true }).IsVisibleAsync());
        await NextAsync();
        Assert.True(await Page.GetByRole(AriaRole.Tab, new() { Name = "Display Value", Exact = true }).IsVisibleAsync());
        await NextAsync();
        Assert.True(await Page.Locator(".ov-mapping-pair").First.IsVisibleAsync());
        await NextAsync();
        Assert.True(await Page.Locator(".ov-api-call-builder").First.IsVisibleAsync());
        await Page.GetByRole(AriaRole.Button, new() { Name = "Previous stage" }).ClickAsync();
        Assert.True(await Page.Locator(".ov-mapping-pair").First.IsVisibleAsync());
    }

    [Fact]
    public async Task Administration_reference_composes_catalogue_progressive_schedule_and_operational_detail()
    {
        await OpenAsync();
        await Page.Locator("#ov-quality-workflow").SelectOptionAsync("operations");
        Assert.True(await Page.Locator(".ov-catalogue-manager").First.IsVisibleAsync());
        await NextAsync();
        Assert.True(await Page.Locator(".ov-progressive-builder").First.IsVisibleAsync());
        await NextAsync();
        Assert.True(await Page.Locator(".ov-schedule-builder").First.IsVisibleAsync());
        await NextAsync();
        Assert.True(await Page.Locator(".ov-operational-detail").First.IsVisibleAsync());
    }

    private async Task OpenAsync()
    {
        await Page.GotoAsync($"{BaseUrl}/quality/workflows");
        await Page.Locator("#ov-quality-workflow").WaitForAsync();
    }

    private async Task NextAsync() => await Page.GetByRole(AriaRole.Button, new() { Name = "Next stage" }).ClickAsync();
}
