using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class WorkspaceCompositionBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Canonical_review_flow_moves_from_list_to_full_width_workspace_and_preserves_list_state()
    {
        await OpenLayoutsAsync();
        var pattern = Page.Locator("#list-to-workspace-cookbook");
        var search = pattern.GetByRole(AriaRole.Textbox, new() { Name = "Search", Exact = true });
        await search.FillAsync("Review");

        Assert.Equal(0, await pattern.GetByRole(AriaRole.Button, new() { Name = "Open review", Exact = true }).CountAsync());
        var item = pattern.GetByRole(AriaRole.Button, new() { Name = "Open Review Item A", Exact = false });
        await item.FocusAsync();
        await Page.Keyboard.PressAsync("Enter");
        await pattern.GetByRole(AriaRole.Heading, new() { Name = "Review Item A", Exact = true }).WaitForAsync();

        Assert.Equal(0, await pattern.Locator(".ov-builder-page__navigation").CountAsync());
        Assert.True(await pattern.GetByRole(AriaRole.Button, new() { Name = "Back to review items", Exact = false }).IsVisibleAsync());
        Assert.True(await pattern.GetByText("Validation", new() { Exact = true }).IsVisibleAsync());

        await pattern.GetByRole(AriaRole.Button, new() { Name = "Back to review items", Exact = false }).ClickAsync();
        await pattern.GetByRole(AriaRole.Heading, new() { Name = "Review items", Exact = true }).WaitForAsync();
        Assert.Equal("Review", await pattern.GetByRole(AriaRole.Textbox, new() { Name = "Search", Exact = true }).InputValueAsync());
        Assert.True(await pattern.Locator(".ov-card--selected").GetByText("Review Item A", new() { Exact = true }).IsVisibleAsync());
    }

    [Fact]
    public async Task Card_child_actions_do_not_select_the_card_and_disabled_actions_are_inert()
    {
        await OpenLayoutsAsync();
        var pattern = Page.Locator("#list-to-workspace-cookbook");
        await pattern.GetByRole(AriaRole.Button, new() { Name = "Open Review Item A", Exact = false }).ClickAsync();

        var selectedBefore = pattern.Locator(".ov-card--selected");
        Assert.True(await selectedBefore.GetByText("Option Alpha", new() { Exact = true }).IsVisibleAsync());

        await pattern.GetByRole(AriaRole.Button, new() { Name = "More", Exact = true }).ClickAsync();
        var selectedAfter = pattern.Locator(".ov-card--selected");
        Assert.True(await selectedAfter.GetByText("Option Alpha", new() { Exact = true }).IsVisibleAsync());
        Assert.False(await selectedAfter.GetByText("Option Beta", new() { Exact = true }).IsVisibleAsync());

        var unavailable = pattern.GetByRole(AriaRole.Button, new() { Name = "Unavailable action", Exact = true });
        Assert.True(await unavailable.EvaluateAsync<bool>("element => element.closest('[inert]') !== null"));

        await pattern.GetByRole(AriaRole.Button, new() { Name = "Back to review items", Exact = false }).ClickAsync();
        Assert.True(await pattern.GetByRole(AriaRole.Alertdialog).IsVisibleAsync());
        await pattern.GetByRole(AriaRole.Button, new() { Name = "Keep reviewing", Exact = true }).ClickAsync();
        Assert.True(await pattern.GetByRole(AriaRole.Heading, new() { Name = "Review Item A", Exact = true }).IsVisibleAsync());
    }

    [Fact]
    public async Task Capability_assignment_flow_edits_alias_assigns_job_saves_and_returns()
    {
        await OpenLayoutsAsync();
        var pattern = Page.Locator("#capability-assignment-workspace");

        var worker = pattern.GetByRole(AriaRole.Button, new() { Name = "Open Allocation and Merge Worker worker settings", Exact = false });
        await worker.FocusAsync();
        await Page.Keyboard.PressAsync("Enter");
        await pattern.Locator(".ov-page--detail h1").GetByText("Capability Assignment Workspace", new() { Exact = true }).WaitForAsync();

        var alias = pattern.GetByRole(AriaRole.Textbox, new() { Name = "Alias", Exact = true });
        await alias.FillAsync("Allocation, Merge and Validation Worker");

        var capability = pattern.GetByRole(AriaRole.Button, new() { Name = "Assign Validate data capability", Exact = false });
        await capability.FocusAsync();
        await Page.Keyboard.PressAsync("Space");
        await pattern.GetByRole(AriaRole.Button, new() { Name = "Remove Validate data capability", Exact = false }).WaitForAsync();

        var selectedCard = pattern.GetByRole(AriaRole.Button, new() { Name = "Remove Validate data capability", Exact = false }).Locator("xpath=..");
        Assert.True(await selectedCard.Locator(".ov-card__selected-indicator").IsVisibleAsync());
        Assert.Equal("true", await pattern.GetByRole(AriaRole.Button, new() { Name = "Remove Validate data capability", Exact = false }).GetAttributeAsync("aria-pressed"));

        await pattern.GetByRole(AriaRole.Button, new() { Name = "Save changes", Exact = true }).ClickAsync();
        await pattern.GetByRole(AriaRole.Heading, new() { Name = "Workers", Exact = true }).WaitForAsync();
        Assert.True(await pattern.GetByText("Allocation, Merge and Validation Worker", new() { Exact = true }).IsVisibleAsync());
    }

    [Fact]
    public async Task Capability_assignment_read_only_and_unavailable_states_keep_the_same_composition()
    {
        await OpenLayoutsAsync();
        var pattern = Page.Locator("#capability-assignment-workspace");

        await pattern.GetByRole(AriaRole.Button, new() { Name = "Open Allocation and Merge Worker worker settings", Exact = false }).ClickAsync();
        await pattern.GetByRole(AriaRole.Button, new() { Name = "View read-only", Exact = true }).ClickAsync();

        Assert.False(await pattern.GetByRole(AriaRole.Textbox, new() { Name = "Alias", Exact = true }).IsEditableAsync());
        Assert.Equal(0, await pattern.GetByRole(AriaRole.Button, new() { Name = "Save changes", Exact = true }).CountAsync());
        var readOnlyCapability = pattern.GetByRole(AriaRole.Button, new() { Name = "Validate data, not assigned, read-only", Exact = false });
        Assert.Equal("true", await readOnlyCapability.GetAttributeAsync("aria-disabled"));

        await pattern.GetByRole(AriaRole.Button, new() { Name = "Back to workers", Exact = false }).ClickAsync();
        await pattern.GetByRole(AriaRole.Button, new() { Name = "Open Publishing Worker worker settings", Exact = false }).ClickAsync();
        Assert.True(await pattern.GetByText("This worker is unavailable.", new() { Exact = false }).IsVisibleAsync());
        Assert.Equal(0, await pattern.GetByRole(AriaRole.Button, new() { Name = "Save changes", Exact = true }).CountAsync());
    }

    [Fact]
    public async Task Person_merge_consumes_governed_card_selection_and_back_navigation()
    {
        await OpenLayoutsAsync();
        var example = Page.Locator("#merge-review-workspace");

        Assert.Equal(0, await example.GetByRole(AriaRole.Button, new() { Name = "Open review", Exact = true }).CountAsync());
        await example.GetByRole(AriaRole.Button, new() { Name = "Open Group 001 review workspace", Exact = false }).ClickAsync();
        await example.GetByRole(AriaRole.Heading, new() { Name = "Group 001", Exact = true }).WaitForAsync();
        Assert.Equal(0, await example.Locator(".ov-builder-page__navigation").CountAsync());

        var candidate = example.GetByRole(AriaRole.Button, new() { Name = "Select Amelia Harte as survivor", Exact = false });
        await candidate.FocusAsync();
        await Page.Keyboard.PressAsync("Enter");
        await example.GetByRole(AriaRole.Button, new() { Name = "Amelia Harte, selected survivor", Exact = false }).WaitForAsync();

        var selectedCard = example.Locator(".ov-card--selected");
        Assert.True(await selectedCard.Locator(".ov-card__selected-indicator").IsVisibleAsync());
        Assert.Equal(0, await selectedCard.Locator(".ov-card__selection-target button").CountAsync());

        await example.GetByRole(AriaRole.Button, new() { Name = "Back to review groups", Exact = false }).ClickAsync();
        await example.GetByRole(AriaRole.Heading, new() { Name = "Merge Candidates", Exact = true }).WaitForAsync();
        Assert.True(await example.GetByRole(AriaRole.Button, new() { Name = "Open Group 001 review workspace", Exact = false }).IsVisibleAsync());
    }

    [Theory]
    [InlineData(1280, 900)]
    [InlineData(760, 900)]
    [InlineData(390, 844)]
    public async Task Focused_workspace_reflows_without_reserving_a_navigation_column(int width, int height)
    {
        await Page.SetViewportSizeAsync(width, height);
        await OpenLayoutsAsync();
        var pattern = Page.Locator("#list-to-workspace-cookbook");
        await pattern.GetByRole(AriaRole.Button, new() { Name = "Open Review Item A", Exact = false }).ClickAsync();

        var workspace = pattern.Locator(".ov-page--detail");
        await workspace.WaitForAsync();
        Assert.Equal(0, await pattern.Locator(".ov-builder-page__navigation").CountAsync());
        Assert.True(await workspace.EvaluateAsync<bool>("element => element.scrollWidth <= element.clientWidth + 1"));
        Assert.True(await Page.EvaluateAsync<bool>("document.documentElement.scrollWidth <= window.innerWidth + 1"));
        Assert.True(await pattern.GetByRole(AriaRole.Button, new() { Name = "Back to review items", Exact = false }).IsVisibleAsync());
    }

    [Theory]
    [InlineData(1280, 900)]
    [InlineData(760, 900)]
    [InlineData(390, 844)]
    public async Task Capability_assignment_workspace_reflows_without_horizontal_overflow(int width, int height)
    {
        await Page.SetViewportSizeAsync(width, height);
        await OpenLayoutsAsync();
        var pattern = Page.Locator("#capability-assignment-workspace");
        await pattern.GetByRole(AriaRole.Button, new() { Name = "Open Allocation and Merge Worker worker settings", Exact = false }).ClickAsync();

        var workspace = pattern.Locator(".ov-page--detail");
        await workspace.WaitForAsync();
        Assert.True(await workspace.EvaluateAsync<bool>("element => element.scrollWidth <= element.clientWidth + 1"));
        Assert.True(await Page.EvaluateAsync<bool>("document.documentElement.scrollWidth <= window.innerWidth + 1"));
        Assert.True(await pattern.GetByRole(AriaRole.Heading, new() { Name = "Job capabilities", Exact = true }).IsVisibleAsync());
    }

    [Fact]
    public async Task Canonical_workspace_example_is_axe_clean()
    {
        await OpenLayoutsAsync();
        var pattern = Page.Locator("#list-to-workspace-cookbook");
        await pattern.GetByRole(AriaRole.Button, new() { Name = "Open Review Item A", Exact = false }).ClickAsync();
        var workspace = pattern.Locator(".ov-page--detail");
        var result = await workspace.RunAxe();
        var violations = result.Violations?.ToArray() ?? [];
        Assert.True(violations.Length == 0, string.Join(Environment.NewLine, violations.Select(v => $"{v.Id}: {v.Description}")));
    }

    [Fact]
    public async Task Capability_assignment_workspace_is_axe_clean()
    {
        await OpenLayoutsAsync();
        var pattern = Page.Locator("#capability-assignment-workspace");
        await pattern.GetByRole(AriaRole.Button, new() { Name = "Open Allocation and Merge Worker worker settings", Exact = false }).ClickAsync();
        var workspace = pattern.Locator(".ov-page--detail");
        var result = await workspace.RunAxe();
        var violations = result.Violations?.ToArray() ?? [];
        Assert.True(violations.Length == 0, string.Join(Environment.NewLine, violations.Select(v => $"{v.Id}: {v.Description}")));
    }

    private async Task OpenLayoutsAsync()
    {
        await Page.GotoAsync($"{BaseUrl}/layouts", new() { WaitUntil = WaitUntilState.NetworkIdle });
        await Page.GetByText("Application shell & page layouts", new() { Exact = true }).WaitForAsync();
    }
}
