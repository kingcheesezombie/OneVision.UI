using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class PatternWorkbenchBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Theory]
    [InlineData("list-page")]
    [InlineData("catalogue-management")]
    [InlineData("api-call-builder")]
    [InlineData("source-configuration")]
    [InlineData("progressive-configuration-builder")]
    [InlineData("operational-detail")]
    [InlineData("entity-inspector")]
    [InlineData("allocation")]
    public async Task Each_governed_pattern_exposes_a_real_live_workbench(string slug)
    {
        var errors = new List<string>();
        Page.PageError += (_, error) => errors.Add(error);
        Page.Console += (_, msg) => { if (msg.Type == "error") errors.Add(msg.Text); };
        await Page.GotoAsync($"{BaseUrl}/patterns/{slug}");
        try
        {
            await Page.Locator("#ov-pattern-scenario").WaitForAsync(new() { Timeout = 30000 });
        }
        catch (TimeoutException)
        {
            var app = await Page.Locator("#app").InnerTextAsync();
            var htmlTail = await Page.EvaluateAsync<string>("() => document.querySelector('#app')?.innerHTML.slice(-6000) || ''");
            throw new InvalidOperationException($"Pattern {slug} never rendered its scenario selector. URL={Page.Url}; Errors={string.Join(" | ", errors)}; App text={app[..Math.Min(app.Length, 4000)]}; HTML tail={htmlTail}");
        }
        Assert.True(await Page.Locator("#ov-pattern-scenario").IsVisibleAsync());
        Assert.True(await Page.Locator(".ov-pattern-explorer__preview").IsVisibleAsync());
        Assert.True(await Page.GetByRole(AriaRole.Button, new() { Name = "Reset to reference defaults" }).IsVisibleAsync());
    }

    [Fact]
    public async Task Source_configuration_capabilities_combine_in_the_actual_pattern()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/source-configuration");
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("current");
        var display = Page.GetByRole(AriaRole.Checkbox, new() { Name = "Display Value Builder" });
        await display.CheckAsync();
        Assert.True(await display.IsCheckedAsync());
        var tab = Page.GetByRole(AriaRole.Tab, new() { Name = "Display Value", Exact = true });
        Assert.True(await tab.IsVisibleAsync());
        await tab.ClickAsync();
        Assert.True(await Page.Locator(".ov-display-value-builder").IsVisibleAsync());
        Assert.True(await Page.GetByText("Display name", new() { Exact = true }).Last.IsVisibleAsync());

        await display.UncheckAsync();
        Assert.Equal(0, await tab.CountAsync());
        await display.CheckAsync();
        await Page.GetByRole(AriaRole.Tab, new() { Name = "Display Value", Exact = true }).ClickAsync();
        Assert.True(await Page.Locator(".ov-display-value-builder").IsVisibleAsync());
    }

    [Fact]
    public async Task Direct_source_current_and_stale_states_obey_the_governed_dependencies()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/source-configuration");
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("current");
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Display Value Builder" }).CheckAsync();
        await Page.Locator("#ov-pattern-source-mode").SelectOptionAsync("direct");
        await Page.GetByRole(AriaRole.Tab, new() { Name = "Connection", Exact = true }).ClickAsync();
        Assert.True(await Page.GetByText("No managed connection required", new() { Exact = true }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Tab, new() { Name = "Display Value", Exact = true }).ClickAsync();
        Assert.True(await Page.Locator(".ov-display-value-builder").IsVisibleAsync());

        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("stale");
        await Page.GetByRole(AriaRole.Tab, new() { Name = "Display Value", Exact = true }).ClickAsync();
        Assert.True(await Page.GetByText("The source definition has changed since it was last profiled.", new() { Exact = false }).IsVisibleAsync());
        Assert.True(await Page.Locator(".ov-display-value-builder").IsVisibleAsync());
    }

    [Fact]
    public async Task List_capabilities_affect_its_real_toolbar_and_results()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/list-page");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Add new" }).WaitForAsync();
        Assert.True(await Page.GetByRole(AriaRole.Button, new() { Name = "Add new" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Primary Add action" }).UncheckAsync();
        Assert.Equal(0, await Page.GetByRole(AriaRole.Button, new() { Name = "Add new" }).CountAsync());
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("empty");
        Assert.True(await Page.GetByText("No matching records", new() { Exact = true }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Button, new() { Name = "Reset to reference defaults" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Button, new() { Name = "Add new" }).IsVisibleAsync());
    }

    [Theory]
    [InlineData(1280, 900)]
    [InlineData(760, 900)]
    [InlineData(390, 844)]
    public async Task Workbench_reflows_with_no_own_horizontal_overflow(int width, int height)
    {
        await Page.SetViewportSizeAsync(width, height);
        await Page.GotoAsync($"{BaseUrl}/patterns/source-configuration");
        var explorer = Page.Locator(".ov-pattern-explorer");
        await explorer.WaitForAsync();
        Assert.True(await explorer.EvaluateAsync<bool>("element => element.scrollWidth <= element.clientWidth + 1"));
    }

    [Fact]
    public async Task Workbench_controls_are_axe_clean()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/source-configuration");
        await Page.Locator("#ov-pattern-scenario").WaitForAsync();
        var result = await Page.Locator(".ov-pattern-explorer__controls").RunAxe();
        var violations = result.Violations?.ToArray() ?? [];
        Assert.True(violations.Length == 0, string.Join(Environment.NewLine, violations.Select(v => $"{v.Id}: {v.Description}")));
    }
}
