using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class ApiCallBuilderBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Guided_choices_resolve_request_and_test_returns_structured_response()
    {
        await OpenAsync();
        await ResolveVersionRequestAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Test Request", Exact = true }).ClickAsync();
        await Page.GetByText("Request successful", new() { Exact = true }).WaitForAsync();
        await Page.GetByText("1,248 records", new() { Exact = true }).WaitForAsync();

        var activeTab = Page.GetByRole(AriaRole.Tab).Filter(new() { HasText = "Structure" });
        Assert.Equal("true", await activeTab.GetAttributeAsync("aria-selected"));
        await Page.Locator(".ov-api-response-preview__panel").WaitForAsync();
        Assert.Equal(1, await Page.Locator(".ov-api-response-preview__panel .ov-tree").CountAsync());
    }

    [Fact]
    public async Task Recursive_entity_branch_reveals_only_relevant_parameter_controls()
    {
        await OpenAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Get information", Exact = false }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Entities", Exact = false }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "By entity type", Exact = false }).ClickAsync();

        await Page.Locator("#ov-api-configure-heading").WaitForAsync();
        Assert.Equal("Get entities", (await Page.Locator("#ov-api-configure-heading").TextContentAsync())?.Trim());
        await Page.GetByLabel("Entity type", new() { Exact = true }).WaitForAsync();
        Assert.Equal(0, await Page.GetByText("Resource path", new() { Exact = true }).CountAsync());
        Assert.True(await Page.GetByRole(AriaRole.Button, new() { Name = "Test Request", Exact = true }).IsDisabledAsync());
    }

    [Fact]
    public async Task Request_details_and_raw_http_are_secondary_disclosures()
    {
        await OpenAsync();
        await ResolveVersionRequestAsync();

        var details = Page.Locator("details.ov-api-call-builder__request-details");
        Assert.Null(await details.GetAttributeAsync("open"));
        await details.Locator("summary").ClickAsync();
        await Page.GetByText("Endpoint", new() { Exact = true }).WaitForAsync();

        var rawDetails = Page.Locator("details.ov-api-call-builder__raw-request");
        Assert.Null(await rawDetails.GetAttributeAsync("open"));
        await rawDetails.Locator("summary").ClickAsync();
        var raw = rawDetails.Locator("pre");
        await raw.WaitForAsync();
        Assert.Contains("GET /api/v1/version", await raw.TextContentAsync(), StringComparison.Ordinal);

        var advanced = Page.Locator("details.ov-api-call-builder__advanced");
        Assert.Null(await advanced.GetAttributeAsync("open"));
        await advanced.Locator("summary").ClickAsync();
        await Page.GetByText("Request body", new() { Exact = true }).WaitForAsync();
    }

    [Fact]
    public async Task Earlier_decision_can_be_changed_without_restarting_flow()
    {
        await OpenAsync();
        await ResolveVersionRequestAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Change What do you want to retrieve?: Version", Exact = true }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "What do you want to retrieve?", Exact = true }).WaitForAsync();
        Assert.Equal(0, await Page.Locator(".ov-api-call-builder__raw-request").CountAsync());
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
    [InlineData(1280, 900)]
    [InlineData(760, 900)]
    [InlineData(390, 844)]
    public async Task Api_call_builder_reflows_without_feature_horizontal_overflow(int width, int height)
    {
        await Page.SetViewportSizeAsync(width, height);
        await OpenAsync();
        var builder = Page.Locator(".ov-api-call-builder");
        Assert.True(await builder.EvaluateAsync<bool>("element => element.scrollWidth <= element.clientWidth + 1"));
    }

    [Fact]
    public async Task Api_call_builder_is_axe_clean()
    {
        await OpenAsync();
        var result = await Page.Locator(".ov-api-call-builder").RunAxe();
        var violations = result.Violations?.ToArray() ?? [];
        Assert.True(violations.Length == 0, string.Join(Environment.NewLine, violations.Select(v => $"{v.Id}: {v.Description}")));
    }

    private async Task OpenAsync()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/api-call-builder");
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Build EAM API Request", Exact = true }).WaitForAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "What action do you want to take?", Exact = true }).WaitForAsync();
    }

    private async Task ResolveVersionRequestAsync()
    {
        await Page.GetByRole(AriaRole.Button, new() { Name = "Get information", Exact = false }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Version", Exact = false }).ClickAsync();
        await Page.GetByText("Request ready", new() { Exact = true }).WaitForAsync();
        await Page.GetByText("Windows Integrated", new() { Exact = true }).WaitForAsync();
    }
}
