using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class StateAwareCompositionBrowserTests : PageTest
{
    private static string BaseUrl =>
        Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Toolbar_workbench_read_only_modifier_reaches_the_real_runtime_inputs()
    {
        await Page.GotoAsync($"{BaseUrl}/components/toolbar");
        var control = Page.GetByRole(AriaRole.Checkbox, new() { Name = "Read-only", Exact = true });
        await control.WaitForAsync();

        var search = Page.Locator(".ov-toolbar input");
        Assert.True(await search.IsEditableAsync());

        await control.CheckAsync();
        Assert.False(await search.IsEditableAsync());
        Assert.True(await Page.Locator(".ov-toolbar").GetByRole(AriaRole.Button, new() { Name = "Add entity" }).IsDisabledAsync());
        // Refresh is an allowed non-mutating action even while other edits are locked.
        Assert.True(await Page.Locator(".ov-toolbar").GetByRole(AriaRole.Button, new() { Name = "Refresh" }).IsEnabledAsync());

        await control.UncheckAsync();
        Assert.True(await search.IsEditableAsync());
    }

    [Fact]
    public async Task Panel_read_only_state_propagates_to_its_supplied_controls()
    {
        await Page.GotoAsync($"{BaseUrl}/components/panel");
        var control = Page.GetByRole(AriaRole.Checkbox, new() { Name = "Read-only", Exact = true });
        await control.WaitForAsync();

        var notes = Page.Locator(".ov-panel input");
        Assert.True(await notes.IsEditableAsync());
        await control.CheckAsync();
        Assert.False(await notes.IsEditableAsync());
        Assert.True(await Page.Locator(".ov-panel").GetByRole(AriaRole.Button, new() { Name = "Edit" }).IsDisabledAsync());
        Assert.True(await Page.Locator(".ov-panel").GetByText("SQL Server", new() { Exact = true }).IsVisibleAsync());

        var result = await Page.Locator(".ov-panel").RunAxe();
        var violations = result.Violations?.ToArray() ?? [];
        Assert.True(violations.Length == 0, string.Join(Environment.NewLine,
            violations.Select(v => $"{v.Id}: {v.Description}")));
    }

    [Fact]
    public async Task Action_bar_busy_modifier_disables_only_its_governed_mutating_actions()
    {
        await Page.GotoAsync($"{BaseUrl}/components/action-bar");
        var control = Page.GetByRole(AriaRole.Checkbox, new() { Name = "Busy", Exact = true });
        await control.WaitForAsync();

        var bar = Page.Locator(".ov-action-bar");
        Assert.Equal("false", await bar.GetAttributeAsync("aria-busy"));

        await control.CheckAsync();
        Assert.Equal("true", await bar.GetAttributeAsync("aria-busy"));
        Assert.True(await bar.GetByRole(AriaRole.Button, new() { Name = "Export" }).IsDisabledAsync());
        Assert.True(await bar.GetByRole(AriaRole.Button, new() { Name = "Delete selected" }).IsDisabledAsync());
    }
}
