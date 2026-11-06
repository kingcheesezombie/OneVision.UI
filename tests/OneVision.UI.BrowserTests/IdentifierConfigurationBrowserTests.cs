using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class IdentifierConfigurationBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ??
        "http://127.0.0.1:5050";

    private async Task<IPage> OpenScenarioAsync(string scenario)
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/source-configuration");
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync(scenario);
        await Page.Locator(".ov-identifier-configuration").WaitForAsync();
        return Page;
    }

    [Fact]
    public async Task Legacy_source_pattern_does_not_render_identifier_without_opt_in()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/source-configuration");
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("current");
        Assert.Equal(0, await Page.Locator(".ov-identifier-configuration").CountAsync());
        var tabs = Page.GetByRole(AriaRole.Tab);
        Assert.Equal(4, await tabs.CountAsync());

        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Identifier Configuration", Exact = true }).CheckAsync();
        var policy = Page.Locator(".ov-identifier-configuration");
        await policy.WaitForAsync();
        Assert.Equal(4, await tabs.CountAsync());
        var mapping = Page.Locator(".ov-source-configuration__mapping-composition");
        var ids = await mapping.EvaluateAsync<double[]>(
            @"element => {
                const identity = element.querySelector('.ov-identifier-configuration').getBoundingClientRect();
                const mapping = element.querySelector('.ov-source-config-demo__mapping').getBoundingClientRect();
                return [identity.top, identity.bottom, mapping.top];
            }");
        Assert.True(ids[0] < ids[2] && ids[1] <= ids[2] + 1, "Identifier must precede general mapping.");
    }

    [Fact]
    public async Task Explicit_primary_and_alternative_fallback_are_colocated()
    {
        await OpenScenarioAsync("identifier-alternative");
        var policy = Page.Locator(".ov-identifier-configuration");
        Assert.True(await policy.GetByText("Identifier source field", new() { Exact = true }).CountAsync() > 0);
        Assert.True(await policy.GetByText("When the identifier is missing", new() { Exact = true }).CountAsync() > 0);
        Assert.True(await policy.GetByText("Alternative identifier field", new() { Exact = true }).CountAsync() > 0);
        Assert.Contains("If both primary and alternative identifiers are unavailable, reject the record.",
            await policy.InnerTextAsync());
        Assert.Equal(0, await policy.GetByText("Deterministic identifier", new() { Exact = true }).CountAsync());
        Assert.True(await Page.GetByText("Pair mapping", new() { Exact = true }).IsVisibleAsync());
    }

    [Fact]
    public async Task No_source_identifier_requires_derivation_and_shows_actual_demo_result()
    {
        await OpenScenarioAsync("identifier-derived");
        var policy = Page.Locator(".ov-identifier-configuration");
        Assert.Contains("The source has no explicit identifier", await policy.InnerTextAsync());
        Assert.True(await policy.GetByRole(AriaRole.Combobox, new() { Name = "Derive from source field", Exact = true }).IsVisibleAsync());
        Assert.Equal(0, await policy.GetByText("Identifier source field", new() { Exact = true }).CountAsync());
        Assert.Equal(0, await policy.GetByText("When the identifier is missing", new() { Exact = true }).CountAsync());
        Assert.Equal(0, await policy.GetByText("Current identity policy", new() { Exact = true }).CountAsync());
        Assert.Equal(0, await policy.GetByText("Identity preview", new() { Exact = true }).CountAsync());
        Assert.Contains("OneVision.UI does not generate identifiers.", await policy.InnerTextAsync());
    }

    [Fact]
    public async Task Deterministic_fallback_requires_explicit_continuity_capability()
    {
        await OpenScenarioAsync("identifier-fallback-prohibited");
        var policy = Page.Locator(".ov-identifier-configuration");
        Assert.Contains("Deterministic fallback is not authorised", await policy.InnerTextAsync());
        Assert.Contains("Identity continuity risk", await policy.InnerTextAsync());
        Assert.Equal(0, await policy.Locator(".ov-copyable-link").CountAsync());
        Assert.False(await Page.GetByRole(AriaRole.Button, new() { Name = "Save source" }).IsEnabledAsync());
    }

    [Theory]
    [InlineData("identifier-locked", "Identifier configuration locked")]
    [InlineData("identifier-read-only", "Identifier configuration read only")]
    [InlineData("identifier-permission-unknown", "Edit permission not confirmed")]
    [InlineData("identifier-saving", "Saving identifier configuration")]
    public async Task Governed_locked_or_readonly_state_preserves_values_and_disables_changes(
        string scenario, string explanation)
    {
        await OpenScenarioAsync(scenario);
        var policy = Page.Locator(".ov-identifier-configuration");
        Assert.Contains(explanation, await policy.InnerTextAsync());
        Assert.Contains("Entity identifier", await policy.InnerTextAsync());
        var toggle = policy.Locator("input[type='checkbox']").First;
        Assert.True(await toggle.IsDisabledAsync());
        Assert.Equal(0, await policy.GetByRole(AriaRole.Button, new() { Name = "Unlock" }).CountAsync());
        Assert.True(await Page.GetByText("Pair mapping", new() { Exact = true }).IsVisibleAsync());
    }

    [Fact]
    public async Task Stale_profile_keeps_saved_identifier_visible_but_suppresses_general_mapping()
    {
        await OpenScenarioAsync("identifier-stale");
        var policy = Page.Locator(".ov-identifier-configuration");
        Assert.Contains("Profile is stale", await policy.InnerTextAsync());
        Assert.True(await policy.Locator("input[type='checkbox']").First.IsDisabledAsync());
        Assert.Equal(0, await Page.GetByText("Pair mapping", new() { Exact = true }).CountAsync());
        Assert.Contains("Run & Profile again", await Page.Locator(".ov-source-configuration").InnerTextAsync());
    }

    [Fact]
    public async Task Removed_field_is_retained_for_inspection_without_silent_substitution()
    {
        await OpenScenarioAsync("identifier-missing-field");
        var policy = Page.Locator(".ov-identifier-configuration");
        Assert.Contains("Unresolved saved field: retired-source-id", await policy.InnerTextAsync());
        Assert.Contains("unavailable or ineligible", await policy.InnerTextAsync());
        Assert.Equal(0, await policy.Locator(".ov-copyable-link").CountAsync());
    }

    [Fact]
    public async Task Stale_asynchronous_preview_is_never_shown()
    {
        await OpenScenarioAsync("identifier-preview-pending");
        var policy = Page.Locator(".ov-identifier-configuration");
        Assert.Equal(0, await policy.GetByText("Identity preview", new() { Exact = true }).CountAsync());
        Assert.DoesNotContain("STALE-RESULT-SHOULD-NOT-SHOW", await policy.InnerTextAsync());
        Assert.Equal(0, await policy.Locator(".ov-copyable-link").CountAsync());
    }

    [Theory]
    [InlineData(1280, 900)]
    [InlineData(760, 900)]
    [InlineData(390, 844)]
    public async Task Identity_composition_is_responsive_without_horizontal_overflow(int width, int height)
    {
        await Page.SetViewportSizeAsync(width, height);
        await OpenScenarioAsync("identifier-multi-field");
        var policy = Page.Locator(".ov-identifier-configuration");
        Assert.True(await policy.EvaluateAsync<bool>(
            "element => element.scrollWidth <= element.clientWidth + 2"));
        Assert.True(await Page.EvaluateAsync<bool>(
            "() => document.documentElement.scrollWidth <= window.innerWidth + 2"));
        Assert.True(await policy.GetByText("Derive from source fields", new() { Exact = true }).IsVisibleAsync());
    }

    [Fact]
    public async Task Identifier_configuration_is_keyboard_accessible_and_axe_clean()
    {
        await OpenScenarioAsync("identifier-derived");
        var policy = Page.Locator(".ov-identifier-configuration");
        var toggle = policy.Locator("input[type='checkbox']").First;
        await toggle.FocusAsync();
        await Page.Keyboard.PressAsync("Space");
        Assert.Contains("Identifier source field", await policy.InnerTextAsync());

        var axe = await policy.RunAxe();
        var violations = axe.Violations?.ToArray() ?? [];
        Assert.True(violations.Length == 0,
            string.Join(Environment.NewLine, violations.Select(v => $"{v.Id}: {v.Description}")));
    }

    [Fact]
    public async Task Save_failure_keeps_existing_editable_policy_and_application_error()
    {
        await OpenScenarioAsync("identifier-save-failed");
        var policy = Page.Locator(".ov-identifier-configuration");
        Assert.Equal("editable", await policy.GetAttributeAsync("data-edit-state"));
        Assert.Contains("Could not save this source.", await Page.Locator(".ov-source-configuration").InnerTextAsync());
        Assert.False(await policy.Locator("input[type='checkbox']").First.IsDisabledAsync());
        var save = Page.GetByRole(AriaRole.Button, new() { Name = "Save source", Exact = true });
        Assert.True(await save.IsEnabledAsync());
        await save.ClickAsync();
        Assert.Contains("Example save failed; the current identifier policy remains editable and unchanged.",
            await Page.Locator(".ov-source-configuration").InnerTextAsync());
        Assert.Equal("editable", await policy.GetAttributeAsync("data-edit-state"));
    }
}
