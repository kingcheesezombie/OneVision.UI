using System.Text.Json;
using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class MappingRuleBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Default_renders_actual_rule_builder_and_read_only_test_callback()
    {
        await OpenStateAsync("Default");
        await Page.Locator(".ov-mapping-rule__composition[aria-label='Classification']").WaitForAsync();
        await Page.Locator(".ov-mapping-rule__composition[aria-label='Target resolution']").WaitForAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Test rule", Exact = true }).ClickAsync();
        await Page.GetByText("Rule matched.", new() { Exact = true }).WaitForAsync();
        Assert.True(await Page.GetByText("Proposed outcome:", new() { Exact = false }).CountAsync() > 0);
        Assert.True(await Page.GetByText("Episode.Description contains Safeguarding", new() { Exact = true }).IsVisibleAsync());
    }

    [Fact]
    public async Task Workbench_modifiers_alter_the_real_component_without_screen_composition()
    {
        await OpenStateAsync("Default");
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Classification", Exact = true }).UncheckAsync();
        Assert.Equal(0, await Page.Locator(".ov-mapping-rule__composition[aria-label='Classification']").CountAsync());

        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Target resolution", Exact = true }).UncheckAsync();
        Assert.Equal(0, await Page.Locator(".ov-mapping-rule__composition[aria-label='Target resolution']").CountAsync());

        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Nested groups", Exact = true }).CheckAsync();
        Assert.True(await Page.GetByRole(AriaRole.Button, new() { Name = "Add group" }).IsVisibleAsync());
    }

    [Fact]
    public async Task Wildcard_state_can_be_tested_without_creating_an_allocation()
    {
        await OpenStateAsync("Wildcard");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Test rule", Exact = true }).ClickAsync();
        await Page.GetByText("Rule matched.", new() { Exact = true }).WaitForAsync();
        Assert.Equal(0, await Page.GetByText("Approved allocations").CountAsync());
    }

    [Fact]
    public async Task Invalid_rule_shows_local_validation_and_evaluation_error()
    {
        await OpenStateAsync("Invalid");
        Assert.True(await Page.Locator(".ov-mapping-rule-condition--invalid").IsVisibleAsync());
        await Page.GetByRole(AriaRole.Button, new() { Name = "Test rule", Exact = true }).ClickAsync();
        await Page.GetByText("Enter a comparison value.", new() { Exact = false }).First.WaitForAsync();
    }

    [Fact]
    public async Task Read_only_state_preserves_conditions_but_blocks_editing()
    {
        await OpenStateAsync("Read-only");
        Assert.True(await Page.GetByRole(AriaRole.Combobox, new() { Name = "Field", Exact = true }).IsDisabledAsync());
        Assert.Equal(0, await Page.GetByRole(AriaRole.Button, new() { Name = "Remove condition" }).CountAsync());
        Assert.True(await Page.GetByRole(AriaRole.Button, new() { Name = "Test rule", Exact = true }).IsVisibleAsync());
    }

    [Fact]
    public async Task Nested_groups_and_ambiguity_are_exposed_as_distinct_states()
    {
        await OpenStateAsync("Nested groups");
        Assert.Equal(2, await Page.Locator(".ov-mapping-rule-group").CountAsync());
        await SelectStateAsync("Ambiguous");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Test rule", Exact = true }).ClickAsync();
        await Page.GetByText("Ambiguous outcome — review required.", new() { Exact = true }).WaitForAsync();
    }

    [Fact]
    public async Task Default_rule_is_axe_clean()
    {
        await OpenStateAsync("Default");
        var result = await Page.Locator(".ov-mapping-rule").RunAxe();
        var violations = result.Violations?.ToArray() ?? [];
        Assert.True(violations.Length == 0, JsonSerializer.Serialize(violations));
    }

    private async Task OpenStateAsync(string state)
    {
        await Page.GotoAsync($"{BaseUrl}/components/mapping-rule");
        await Page.Locator(".ov-mapping-rule").WaitForAsync();
        await SelectStateAsync(state);
    }

    private async Task SelectStateAsync(string state)
    {
        await Page.Locator(".ov-state-choice", new() { HasText = state }).ClickAsync();
        await Page.Locator(".ov-state-choice--active", new() { HasText = state }).WaitForAsync();
    }
}
