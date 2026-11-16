using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class AccessManagementBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Bootstrap_is_reachable_without_a_prior_administration_session()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/access-management");
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("bootstrap-eligible");

        Assert.True(await Page.GetByText("Initial administrator setup", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await Page.GetByText("synthetic:operator-001", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal(0, await Page.GetByRole(AriaRole.Table, new() { Name = "Assigned application access" }).CountAsync());

        await Page.GetByRole(AriaRole.Button, new() { Name = "Set up initial administrator" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Confirm operation" }).ClickAsync();
        Assert.True(await Page.GetByText("Operation confirmed by the application.", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal(0, await Page.GetByRole(AriaRole.Button, new() { Name = "Set up initial administrator" }).CountAsync());
    }

    [Fact]
    public async Task Ineligible_bootstrap_never_exposes_a_grant_button()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/access-management");
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("bootstrap-ineligible");
        Assert.True(await Page.GetByText("Not eligible for this operation.", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal(0, await Page.GetByRole(AriaRole.Button, new() { Name = "Set up initial administrator" }).CountAsync());
    }

    [Fact]
    public async Task Inspect_only_mode_preserves_role_information_without_write_actions()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/access-management");
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("read-only");
        await Page.GetByRole(AriaRole.Table, new() { Name = "Assigned application access" }).WaitForAsync();
        Assert.True(await Page.GetByText("Avery Example", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal(0, await Page.GetByRole(AriaRole.Button, new() { Name = "Assign role" }).CountAsync());
    }
}
