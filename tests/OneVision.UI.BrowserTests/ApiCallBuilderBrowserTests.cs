using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class ApiCallBuilderBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Guided_choices_resolve_request_and_test_returns_compact_structured_response()
    {
        await OpenAsync();
        await ResolveVersionRequestAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Test Request", Exact = true }).ClickAsync();
        var status = Page.Locator(".ov-api-response-preview__status strong");
        await status.WaitForAsync();
        Assert.Contains("Request successful", await status.TextContentAsync(), StringComparison.Ordinal);
        await Page.GetByText("1,248 records", new() { Exact = true }).WaitForAsync();

        var activeTab = Page.GetByRole(AriaRole.Tab).Filter(new() { HasText = "Structure" });
        Assert.Equal("true", await activeTab.GetAttributeAsync("aria-selected"));
        await Page.Locator(".ov-api-response-preview__panel").WaitForAsync();
        Assert.Equal(1, await Page.Locator(".ov-api-response-preview__panel .ov-tree").CountAsync());
    }

    [Fact]
    public async Task Completed_decisions_remain_visible_and_editable_as_quiet_vertical_history()
    {
        await OpenAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Get information", Exact = false }).ClickAsync();

        var completed = Page.Locator(".ov-api-decision-trail__item");
        await completed.First.WaitForAsync();
        Assert.Contains("Get information", await completed.First.TextContentAsync(), StringComparison.Ordinal);
        Assert.Equal("Completed: What action do you want to take?, Get information", await completed.First.GetAttributeAsync("aria-label"));
        Assert.True(await completed.First.GetByRole(AriaRole.Button, new() { Name = "Change What action do you want to take?: Get information", Exact = true }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Heading, new() { Name = "What do you want to retrieve?", Exact = true }).WaitForAsync();
        Assert.Equal(0, await Page.Locator(".ov-api-call-builder__flow-context").CountAsync());
    }

    [Fact]
    public async Task Recursive_entity_branch_disables_actions_until_required_values_are_valid()
    {
        await OpenAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Get information", Exact = false }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Entities", Exact = false }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "By entity type", Exact = false }).ClickAsync();

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Request options", Exact = true }).WaitForAsync();
        await Page.GetByLabel("Entity type", new() { Exact = true }).WaitForAsync();
        Assert.Equal(0, await Page.GetByText("Resource path", new() { Exact = true }).CountAsync());
        Assert.Contains("Choose an entity type", await Page.Locator(".ov-api-call-builder__ready").TextContentAsync(), StringComparison.OrdinalIgnoreCase);
        Assert.Equal(0, await Page.Locator(".mud-input-error").CountAsync());
        Assert.True(await Page.GetByRole(AriaRole.Button, new() { Name = "Test Request", Exact = true }).IsDisabledAsync());
        Assert.True(await Page.GetByRole(AriaRole.Button, new() { Name = "Save", Exact = true }).IsDisabledAsync());
    }

    [Fact]
    public async Task Request_output_prioritises_resolved_path_and_copy_controls_work_locally()
    {
        await OpenAsync();
        await ResolveVersionRequestAsync();

        var request = Page.Locator(".ov-api-call-builder__request");
        await request.WaitForAsync();
        Assert.Contains("GET · Get API version", await request.TextContentAsync(), StringComparison.Ordinal);
        Assert.True(await request.Locator(".ov-api-call-builder__copy-row--primary button[aria-label^='Copy resolved API path:']").IsVisibleAsync());
        Assert.True(await request.Locator("button[aria-label^='Copy full URL:']").IsVisibleAsync());
        Assert.True(await request.Locator("button[aria-label^='Copy API path:']").IsVisibleAsync());
        Assert.True(await request.Locator("button[aria-label^='Copy request:']").IsVisibleAsync());

        await Context.GrantPermissionsAsync(["clipboard-read", "clipboard-write"], new() { Origin = BaseUrl });
        var resolvedCopy = request.Locator("button[aria-label^='Copy resolved API path:']");
        await resolvedCopy.ClickAsync();
        await resolvedCopy.GetByText("Copied ✓", new() { Exact = true }).WaitForAsync();
        Assert.Equal("/api/v1/version", await Page.EvaluateAsync<string>("navigator.clipboard.readText()"));

        var details = Page.Locator("details.ov-api-call-builder__request-details");
        Assert.Null(await details.GetAttributeAsync("open"));
        await details.Locator("summary").ClickAsync();
        await Page.GetByText("Content type", new() { Exact = true }).WaitForAsync();

        var advanced = Page.Locator("details.ov-api-call-builder__advanced");
        Assert.Null(await advanced.GetAttributeAsync("open"));
        await advanced.Locator("summary").ClickAsync();
        await Page.GetByText("Generated request", new() { Exact = true }).WaitForAsync();
        Assert.Contains("GET /api/v1/version", await advanced.Locator("pre").TextContentAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Earlier_decision_can_be_changed_without_restarting_flow()
    {
        await OpenAsync();
        await ResolveVersionRequestAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Change What do you want to retrieve?: Version", Exact = true }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "What do you want to retrieve?", Exact = true }).WaitForAsync();
        Assert.Equal(0, await Page.Locator(".ov-api-call-builder__request").CountAsync());
        Assert.True(await Page.GetByText("Get information", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await Page.Locator(".ov-api-call-builder__active-host").EvaluateAsync<bool>("element => document.activeElement === element"));
    }

    [Fact]
    public async Task Medium_scenario_groups_parameters_and_reports_configured_optional_state()
    {
        await OpenAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Medium", Exact = true }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Request options", Exact = true }).WaitForAsync();

        var groupHeadings = Page.Locator(".ov-api-call-builder__parameter-group-heading strong");
        await groupHeadings.Filter(new() { HasText = "Selection" }).WaitForAsync();
        var additional = Page.Locator("details.ov-api-call-builder__additional");
        Assert.Null(await additional.GetAttributeAsync("open"));
        Assert.Contains("3 available", await additional.TextContentAsync(), StringComparison.Ordinal);

        await additional.Locator("summary").ClickAsync();
        await groupHeadings.Filter(new() { HasText = "Scope" }).WaitForAsync();
        var search = Page.GetByLabel("Search", new() { Exact = true });
        await search.FillAsync("620496");
        await Page.GetByText("1 configured", new() { Exact = true }).WaitForAsync();
    }

    [Fact]
    public async Task Deep_scenario_keeps_history_compact_and_orientation_visible()
    {
        await OpenAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Deep", Exact = true }).ClickAsync();

        var items = Page.Locator(".ov-api-decision-trail__item");
        await items.Nth(7).WaitForAsync();
        Assert.True(await items.CountAsync() >= 8);
        Assert.True(await Page.Locator(".ov-api-call-builder__flow-context").IsVisibleAsync());
        Assert.True(await Page.Locator(".ov-api-call-builder__request").IsVisibleAsync());
        Assert.True(await Page.Locator(".ov-api-call-builder__response").IsVisibleAsync());
    }

    [Fact]
    public async Task Long_resolved_url_wraps_without_hiding_copy_action_or_breaking_layout()
    {
        await Page.SetViewportSizeAsync(768, 900);
        await OpenAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Medium", Exact = true }).ClickAsync();
        var additional = Page.Locator("details.ov-api-call-builder__additional");
        await additional.Locator("summary").ClickAsync();
        await Page.GetByLabel("Search", new() { Exact = true }).FillAsync(new string('x', 420));

        var primary = Page.Locator(".ov-api-call-builder__copy-row--primary");
        Assert.True(await primary.Locator("button[aria-label^='Copy resolved API path:']").IsVisibleAsync());
        var builder = Page.Locator(".ov-api-call-builder");
        var overflow = await builder.EvaluateAsync<string>("element => Array.from(element.querySelectorAll('*')).filter(node => node.scrollWidth > node.clientWidth + 1 && getComputedStyle(node).overflowX === 'visible').slice(0,8).map(node => `${node.tagName}.${node.className}: ${node.scrollWidth}/${node.clientWidth}`).join(' | ')");
        Assert.True(await builder.EvaluateAsync<bool>("element => element.scrollWidth <= element.clientWidth + 1"), overflow);
    }

    [Fact]
    public async Task Read_only_scenario_is_intentional_and_keeps_copy_surfaces()
    {
        await OpenAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Read-only", Exact = true }).ClickAsync();
        await Page.Locator(".ov-api-call-builder--readonly").WaitForAsync();

        Assert.True(await Page.Locator(".ov-api-call-builder__readonly-values").IsVisibleAsync());
        Assert.Equal(0, await Page.GetByRole(AriaRole.Button, new() { Name = "Save", Exact = true }).CountAsync());
        Assert.Equal(0, await Page.GetByRole(AriaRole.Button, new() { Name = "Test Request", Exact = true }).CountAsync());
        Assert.True(await Page.GetByRole(AriaRole.Button, new() { Name = "Close", Exact = true }).IsVisibleAsync());
        Assert.True(await Page.Locator("button[aria-label^='Copy resolved API path:']").IsVisibleAsync());
    }

    [Fact]
    public async Task Authentication_is_presented_as_safe_context_not_editable_secret()
    {
        await OpenAsync();
        await ResolveVersionRequestAsync();

        await Page.GetByText("Windows Integrated", new() { Exact = true }).WaitForAsync();
        Assert.Equal(0, await Page.GetByLabel("Password", new() { Exact = false }).CountAsync());
        Assert.Equal(0, await Page.Locator("input[type='password']").CountAsync());
    }

    [Theory]
    [InlineData(1440, 900)]
    [InlineData(1280, 900)]
    [InlineData(1024, 900)]
    [InlineData(768, 900)]
    [InlineData(600, 900)]
    [InlineData(400, 844)]
    public async Task Api_call_builder_reflows_without_feature_horizontal_overflow(int width, int height)
    {
        await Page.SetViewportSizeAsync(width, height);
        await OpenAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Deep", Exact = true }).ClickAsync();
        var builder = Page.Locator(".ov-api-call-builder");
        Assert.True(await builder.EvaluateAsync<bool>("element => element.scrollWidth <= element.clientWidth + 1"));
    }

    [Fact]
    public async Task Api_call_builder_is_axe_clean_in_deep_state()
    {
        await OpenAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Deep", Exact = true }).ClickAsync();
        var result = await Page.Locator(".ov-api-call-builder").RunAxe();
        var violations = result.Violations?.ToArray() ?? [];
        Assert.True(violations.Length == 0, string.Join(Environment.NewLine, violations.Select(v => $"{v.Id}: {v.Description}")));
    }

    private async Task OpenAsync()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/api-call-builder");
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Build EAM API Request", Exact = true }).WaitForAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "What action do you want to take?", Exact = true }).WaitForAsync();
        Assert.Equal(0, await Page.Locator(".ov-api-call-builder__request").CountAsync());
        Assert.Equal(0, await Page.Locator(".ov-api-call-builder__response").CountAsync());
    }

    private async Task ResolveVersionRequestAsync()
    {
        await Page.GetByRole(AriaRole.Button, new() { Name = "Get information", Exact = false }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Version", Exact = false }).ClickAsync();
        await Page.GetByText("Ready", new() { Exact = true }).WaitForAsync();
        await Page.GetByText("Windows Integrated", new() { Exact = true }).WaitForAsync();
    }
}