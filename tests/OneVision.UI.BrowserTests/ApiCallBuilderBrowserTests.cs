using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class ApiCallBuilderBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Friendly_preset_builds_request_and_test_call_returns_structured_response()
    {
        await OpenAsync();
        await ApplyEntitiesPresetAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Test Call", Exact = true }).ClickAsync();
        await Page.GetByText("Request successful", new() { Exact = true }).WaitForAsync();
        await Page.GetByText("1,248 records returned", new() { Exact = false }).WaitForAsync();

        var activeTab = Page.GetByRole(AriaRole.Tab).Filter(new() { HasText = "Structure" });
        Assert.Equal("true", await activeTab.GetAttributeAsync("aria-selected"));

        var structurePanel = Page.Locator(".ov-api-call-builder__response-panel");
        await structurePanel.WaitForAsync();
        Assert.Contains("entityId", await structurePanel.TextContentAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Raw_request_and_advanced_options_are_secondary_disclosures()
    {
        await OpenAsync();
        await ApplyEntitiesPresetAsync();

        Assert.Equal(0, await Page.GetByText("Request body", new() { Exact = true }).CountAsync());
        var advanced = Page.GetByRole(AriaRole.Button, new() { Name = "Advanced", Exact = false });
        Assert.Equal("false", await advanced.GetAttributeAsync("aria-expanded"));
        await advanced.ClickAsync();
        Assert.Equal("true", await advanced.GetAttributeAsync("aria-expanded"));
        await Page.GetByLabel("Request body", new() { Exact = true }).WaitForAsync();

        var raw = Page.Locator(".ov-api-call-builder__raw-toggle").First;
        await raw.WaitForAsync();
        Assert.Equal("View raw request", (await raw.TextContentAsync())?.Trim());
        Assert.Equal("false", await raw.GetAttributeAsync("aria-expanded"));
        await raw.ClickAsync();
        Assert.Equal("true", await raw.GetAttributeAsync("aria-expanded"));

        var rawRequest = Page.Locator(".ov-api-call-builder__raw-request");
        await rawRequest.WaitForAsync();
        Assert.Contains("type=%7B%7BEntityType%7D%7D", await rawRequest.TextContentAsync(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Authentication_is_presented_as_safe_context_not_editable_secret()
    {
        await OpenAsync();
        await ApplyEntitiesPresetAsync();

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
        await Page.GetByRole(AriaRole.Heading, new() { Name = "API Call", Exact = true }).WaitForAsync();
    }

    private async Task ApplyEntitiesPresetAsync()
    {
        await Page.GetByRole(AriaRole.Button, new() { Name = "Get EAM entities", Exact = false }).ClickAsync();
        await Page.Locator(".ov-api-call-builder__connection-summary").WaitForAsync();
        await Page.GetByText("Windows Integrated", new() { Exact = true }).WaitForAsync();
    }
}
