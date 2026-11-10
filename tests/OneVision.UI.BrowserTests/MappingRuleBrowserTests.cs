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
        await Page.Locator(".ov-mapping-rule__section[aria-label='Outputs']").WaitForAsync();
        Assert.True(await Page.GetByRole(AriaRole.Tab, new() { Name = "Overview" }).IsVisibleAsync());
        Assert.True(await Page.GetByRole(AriaRole.Tab, new() { Name = "Rule" }).IsVisibleAsync());

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
        Assert.Equal(0, await Page.GetByRole(AriaRole.Combobox, new() { Name = "Episode type", Exact = true }).CountAsync());

        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Target resolution", Exact = true }).UncheckAsync();
        Assert.Equal(0, await Page.Locator(".ov-mapping-rule__section[aria-label='Outputs']").CountAsync());

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
    public async Task Two_mandatory_tabs_keep_unsaved_draft_and_namespace_is_optional()
    {
        await OpenStateAsync("Default");
        Assert.Equal(2, await Page.Locator(".ov-mapping-rule").GetByRole(AriaRole.Tab).CountAsync());
        await Page.GetByRole(AriaRole.Tab, new() { Name = "Overview" }).ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Textbox, new() { Name = "Description" }).IsVisibleAsync());
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Name", Exact = true }).FillAsync("Unsaved rule draft");
        await Page.GetByRole(AriaRole.Tab, new() { Name = "Rule", Exact = true }).ClickAsync();
        Assert.True(await Page.Locator(".ov-mapping-rule__section[aria-label='Conditions']").IsVisibleAsync());
        await Page.GetByRole(AriaRole.Tab, new() { Name = "Overview" }).ClickAsync();
        Assert.Equal("Unsaved rule draft", await Page.GetByRole(AriaRole.Textbox, new() { Name = "Name", Exact = true }).InputValueAsync());
    }

    [Fact]
    public async Task Empty_conditions_remain_editable_without_an_isolated_join_selector()
    {
        await OpenStateAsync("Empty");
        Assert.True(await Page.GetByText("No conditions configured", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal(0, await Page.GetByRole(AriaRole.Combobox, new() { Name = "Join", Exact = true }).CountAsync());
        Assert.True(await Page.GetByRole(AriaRole.Button, new() { Name = "Add condition" }).IsVisibleAsync());
    }

    [Theory]
    [InlineData(320)]
    [InlineData(480)]
    [InlineData(720)]
    [InlineData(1024)]
    public async Task Mapping_rule_remains_contained_at_constrained_viewport_widths(int width)
    {
        await Page.SetViewportSizeAsync(width, 850);
        await OpenStateAsync("Nested groups");
        var rule = Page.Locator(".ov-mapping-rule");
        Assert.True(await rule.EvaluateAsync<bool>("e => e.scrollWidth <= e.clientWidth + 2"));
        Assert.True(await Page.EvaluateAsync<bool>("() => document.documentElement.scrollWidth <= window.innerWidth + 2"));
    }

    [Theory]
    [InlineData("320", 322)]
    [InlineData("480", 482)]
    [InlineData("720", 722)]
    [InlineData("1024", 1026)]
    public async Task Narrow_editor_reflows_inside_wide_desktop_viewport(string width, int maximum)
    {
        await Page.SetViewportSizeAsync(1440, 900);
        await OpenStateAsync("Nested groups");
        var chooser = Page.GetByRole(AriaRole.Combobox, new() { Name = "Editor container width" });
        await chooser.ClickAsync();
        await Page.GetByText(width + "px", new() { Exact = true }).Last.ClickAsync();

        var rule = Page.Locator(".ov-mapping-rule");
        Assert.True(await rule.EvaluateAsync<bool>($"e => e.getBoundingClientRect().width <= {maximum}"));
        Assert.True(await rule.EvaluateAsync<bool>("e => e.scrollWidth <= e.clientWidth + 2"));
    }

    [Fact]
    public async Task Optional_target_namespace_tab_is_independent_of_rule_outputs()
    {
        await OpenStateAsync("Default");
        var chooser = Page.GetByRole(AriaRole.Combobox, new() { Name = "Target Namespace capability" });
        await chooser.ClickAsync();
        await Page.Keyboard.PressAsync("End");
        await Page.Keyboard.PressAsync("Enter");

        var namespaceTab = Page.Locator(".ov-mapping-rule").GetByRole(AriaRole.Tab, new() { Name = "Target Namespace" });
        await namespaceTab.ClickAsync();
        Assert.True(await Page.GetByRole(AriaRole.Combobox, new() { Name = "Target Provider" }).IsVisibleAsync());
        Assert.True(await Page.GetByRole(AriaRole.Combobox, new() { Name = "Target Identifier Mapping" }).IsVisibleAsync());
        Assert.Equal(0, await Page.GetByRole(AriaRole.Button, new() { Name = "Resolve target" }).CountAsync());

        await Page.Locator(".ov-mapping-rule").GetByRole(AriaRole.Tab, new() { Name = "Rule", Exact = true }).ClickAsync();
        Assert.True(await Page.Locator(".ov-mapping-rule__section[aria-label='Outputs']").IsVisibleAsync());
    }

    [Fact]
    public async Task Three_levels_of_nested_groups_fit_within_narrow_desktop_editor()
    {
        await Page.SetViewportSizeAsync(1440, 900);
        await OpenStateAsync("Default");
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Deep nesting (three levels)" }).CheckAsync();

        var editor = Page.GetByRole(AriaRole.Combobox, new() { Name = "Editor container width" });
        await editor.ClickAsync();
        await Page.GetByText("320px", new() { Exact = true }).Last.ClickAsync();

        var rule = Page.Locator(".ov-mapping-rule");
        Assert.Equal(4, await rule.Locator(".ov-mapping-rule-group").CountAsync());
        Assert.True(await rule.EvaluateAsync<bool>("e => e.scrollWidth <= e.clientWidth + 2"));
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
