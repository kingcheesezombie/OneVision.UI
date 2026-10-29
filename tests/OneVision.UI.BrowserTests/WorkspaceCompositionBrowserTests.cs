using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class WorkspaceCompositionBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Canonical_review_flow_moves_from_list_to_full_width_workspace_and_back()
    {
        await OpenLayoutsAsync();
        var pattern = Page.Locator("#workspace-capabilities");

        await pattern.GetByRole(AriaRole.Button, new() { Name = "Open North region configuration", Exact = true }).ClickAsync();
        await pattern.GetByRole(AriaRole.Heading, new() { Name = "North region configuration", Exact = true }).WaitForAsync();

        Assert.Equal(0, await pattern.Locator(".ov-builder-page__navigation").CountAsync());
        Assert.True(await pattern.GetByRole(AriaRole.Button, new() { Name = "Back to reviews", Exact = false }).IsVisibleAsync());
        Assert.True(await pattern.GetByText("Validation", new() { Exact = true }).IsVisibleAsync());

        await pattern.GetByRole(AriaRole.Button, new() { Name = "Back to reviews", Exact = false }).ClickAsync();
        await pattern.GetByRole(AriaRole.Heading, new() { Name = "Configuration reviews", Exact = true }).WaitForAsync();
        Assert.True(await pattern.GetByRole(AriaRole.Button, new() { Name = "Open North region configuration", Exact = true }).IsVisibleAsync());
    }

    [Fact]
    public async Task Person_merge_consumes_governed_card_selection_and_back_navigation()
    {
        await OpenLayoutsAsync();
        var example = Page.Locator("#merge-review-workspace");

        await example.GetByRole(AriaRole.Button, new() { Name = "Open Group 001 review workspace", Exact = true }).ClickAsync();
        await example.GetByRole(AriaRole.Heading, new() { Name = "Group 001", Exact = true }).WaitForAsync();
        Assert.Equal(0, await example.Locator(".ov-builder-page__navigation").CountAsync());

        var candidate = example.GetByRole(AriaRole.Button, new() { Name = "Select Amelia Harte as survivor", Exact = true });
        await candidate.FocusAsync();
        await Page.Keyboard.PressAsync("Enter");
        await example.GetByRole(AriaRole.Button, new() { Name = "Amelia Harte, selected survivor", Exact = true }).WaitForAsync();

        var selectedCard = example.Locator(".ov-card--selected");
        Assert.True(await selectedCard.GetByText("Selected", new() { Exact = false }).IsVisibleAsync());
        Assert.Equal(0, await selectedCard.Locator(".ov-card__selection button").CountAsync());

        await example.GetByRole(AriaRole.Button, new() { Name = "Back to review groups", Exact = false }).ClickAsync();
        await example.GetByRole(AriaRole.Heading, new() { Name = "Merge Candidates", Exact = true }).WaitForAsync();
        Assert.True(await example.GetByRole(AriaRole.Button, new() { Name = "Open Group 001 review workspace", Exact = true }).IsVisibleAsync());
    }

    [Theory]
    [InlineData(1024, 768)]
    [InlineData(390, 844)]
    public async Task Focused_workspace_reflows_without_reserving_a_navigation_column(int width, int height)
    {
        await Page.SetViewportSizeAsync(width, height);
        await OpenLayoutsAsync();
        var pattern = Page.Locator("#workspace-capabilities");
        await pattern.GetByRole(AriaRole.Button, new() { Name = "Open North region configuration", Exact = true }).ClickAsync();

        var workspace = pattern.Locator(".ov-page--detail");
        await workspace.WaitForAsync();
        Assert.Equal(0, await pattern.Locator(".ov-builder-page__navigation").CountAsync());
        Assert.True(await workspace.EvaluateAsync<bool>("element => element.scrollWidth <= element.clientWidth + 1"));
        Assert.True(await Page.EvaluateAsync<bool>("document.documentElement.scrollWidth <= window.innerWidth + 1"));
    }

    [Fact]
    public async Task Canonical_workspace_example_is_axe_clean()
    {
        await OpenLayoutsAsync();
        var pattern = Page.Locator("#workspace-capabilities");
        await pattern.GetByRole(AriaRole.Button, new() { Name = "Open North region configuration", Exact = true }).ClickAsync();
        var result = await pattern.RunAxe();
        var violations = result.Violations?.ToArray() ?? [];
        Assert.True(violations.Length == 0, string.Join(Environment.NewLine, violations.Select(v => $"{v.Id}: {v.Description}")));
    }

    private async Task OpenLayoutsAsync()
    {
        await Page.GotoAsync($"{BaseUrl}/layouts", new() { WaitUntil = WaitUntilState.NetworkIdle });
        await Page.GetByText("Application shell & page layouts", new() { Exact = true }).WaitForAsync();
    }
}
