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
        await OpenStateAsync("AND conditions");
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
        await OpenStateAsync("AND conditions");
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
        await OpenStateAsync("AND conditions");
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
        await OpenStateAsync("AND conditions");
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
        await OpenStateAsync("AND conditions");
        await Page.GetByText("Deep nesting (three levels)", new() { Exact = true }).ClickAsync();

        var editor = Page.GetByRole(AriaRole.Combobox, new() { Name = "Editor container width" });
        await editor.ClickAsync();
        await Page.GetByText("320px", new() { Exact = true }).Last.ClickAsync();

        var rule = Page.Locator(".ov-mapping-rule");
        Assert.Equal(4, await rule.Locator(".ov-mapping-rule-group").CountAsync());
        Assert.True(await rule.EvaluateAsync<bool>("e => e.scrollWidth <= e.clientWidth + 2"));
    }

    [Fact]
    public async Task Default_allocation_matching_uses_only_overview_rule_and_conditions()
    {
        await OpenStateAsync("Default");
        var editor = Page.Locator(".ov-mapping-rule");
        Assert.Equal(3, await editor.GetByRole(AriaRole.Tab).CountAsync());
        Assert.Equal(0, await editor.GetByRole(AriaRole.Tab, new() { Name = "Target Namespace" }).CountAsync());
        Assert.True(await Page.Locator(".ov-mapping-rule__section[aria-label='Allocation Rule']").IsVisibleAsync());
        Assert.True(await Page.Locator(".ov-mapping-rule__contract").IsVisibleAsync());
        Assert.True(await Page.Locator(".ov-mapping-rule__section[aria-label='Outcome']").IsVisibleAsync());
        Assert.True(await Page.Locator(".ov-mapping-rule__section[aria-label='Type']").IsVisibleAsync());
        Assert.Equal(0, await Page.Locator(".ov-mapping-rule__section[aria-label='Outputs']").CountAsync());
        await editor.GetByRole(AriaRole.Tab, new() { Name = "Conditions" }).ClickAsync();
        Assert.True(await Page.Locator(".ov-mapping-rule-group").IsVisibleAsync());
    }

    [Fact]
    public async Task Allocation_contract_direct_and_associated_test_results_show_provenance()
    {
        await OpenStateAsync("Default");
        var editor = Page.Locator(".ov-mapping-rule");
        await editor.GetByRole(AriaRole.Tab, new() { Name = "Conditions" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Test rule" }).ClickAsync();
        Assert.True(await Page.GetByText("Allocation eligible:", new() { Exact = false }).IsVisibleAsync());

        await editor.GetByRole(AriaRole.Tab, new() { Name = "Rule", Exact = true }).ClickAsync();
        await Page.GetByRole(AriaRole.Combobox, new() { Name = "Type", Exact = true }).ClickAsync();
        await Page.GetByText("Associated", new() { Exact = true }).Last.ClickAsync();
        await editor.GetByRole(AriaRole.Tab, new() { Name = "Conditions" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Test rule" }).ClickAsync();
        Assert.True(await Page.GetByText("Synthetic approved ESR entity").IsVisibleAsync());
        Assert.True(await Page.GetByText("Synthetic approved allocation A-001").IsVisibleAsync());
    }

    [Fact]
    public async Task Default_allocation_contract_editor_is_accessible()
    {
        await OpenStateAsync("Default");
        var result = await Page.Locator(".ov-mapping-rule").RunAxe();
        var violations = result.Violations?.ToArray() ?? [];
        Assert.True(violations.Length == 0, JsonSerializer.Serialize(violations));
    }

    [Fact]
    public async Task Generic_contract_shows_governed_trees_without_EAM_allocation_language()
    {
        await OpenStateAsync("Generic contract");
        var editor = Page.Locator(".ov-mapping-rule");
        Assert.Equal(3, await editor.GetByRole(AriaRole.Tab).CountAsync());
        Assert.True(await Page.GetByRole(AriaRole.Combobox, new() { Name = "Governing Contract" }).IsVisibleAsync());
        Assert.Equal(0, await editor.GetByRole(AriaRole.Combobox, new() { Name = "Allocation Rule" }).CountAsync());
        await editor.GetByRole(AriaRole.Tab, new() { Name = "Conditions" }).ClickAsync();
        Assert.True(await editor.GetByRole(AriaRole.Region, new() { Name = "Source" }).IsVisibleAsync());
        Assert.True(await editor.GetByRole(AriaRole.Region, new() { Name = "Target" }).IsVisibleAsync());
        Assert.True(await editor.GetByRole(AriaRole.Button, new() { Name = "Edit pair" }).IsVisibleAsync());
    }

    [Fact]
    public async Task Generic_associated_changes_right_field_catalogue_not_condition_storage()
    {
        await OpenStateAsync("Generic contract");
        var editor = Page.Locator(".ov-mapping-rule");
        await editor.GetByRole(AriaRole.Tab, new() { Name = "Rule", Exact = true }).ClickAsync();
        await Page.GetByRole(AriaRole.Combobox, new() { Name = "Type", Exact = true }).ClickAsync();
        await Page.GetByText("Associated", new() { Exact = true }).Last.ClickAsync();
        await editor.GetByRole(AriaRole.Tab, new() { Name = "Conditions" }).ClickAsync();
        Assert.True(await Page.Locator(".ov-mapping-rule__field-pane[aria-label='Associated Fields']").IsVisibleAsync());
        Assert.Equal(1, await Page.Locator(".ov-mapping-rule-condition").CountAsync());
        Assert.True(await Page.Locator(".ov-mapping-rule-condition--invalid").IsVisibleAsync());
        await Page.GetByRole(AriaRole.Button, new() { Name = "Test rule" }).ClickAsync();
        Assert.True(await Page.GetByText("Rule did not match.", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await Page.GetByText("Processing eligible:", new() { Exact = true }).IsVisibleAsync());
    }

    [Fact]
    public async Task Valid_generic_associated_rule_shows_application_owned_relationship_provenance()
    {
        await OpenStateAsync("Generic associated");
        var editor = Page.Locator(".ov-mapping-rule");
        await editor.GetByRole(AriaRole.Tab, new() { Name = "Conditions" }).ClickAsync();
        Assert.True(await Page.Locator(".ov-mapping-rule__field-pane[aria-label='Associated Fields']").IsVisibleAsync());
        Assert.Equal(0, await Page.Locator(".ov-mapping-rule-condition--invalid").CountAsync());
        await Page.GetByRole(AriaRole.Button, new() { Name = "Test rule" }).ClickAsync();
        Assert.True(await Page.GetByText("Customer relationship R-7").IsVisibleAsync());
        Assert.True(await Page.GetByText("Processing eligible:", new() { Exact = true }).IsVisibleAsync());
    }

    [Fact]
    public async Task Direct_pair_selection_from_both_trees_updates_the_condition_and_survives_tab_navigation()
    {
        await OpenStateAsync("Generic contract");
        var editor = Page.Locator(".ov-mapping-rule");
        await editor.GetByRole(AriaRole.Tab, new() { Name = "Conditions" }).ClickAsync();

        var condition = editor.Locator(".ov-mapping-rule-condition").First;
        await condition.GetByRole(AriaRole.Button, new() { Name = "Edit pair" }).ClickAsync();
        Assert.True(await condition.GetByRole(AriaRole.Button, new() { Name = "Editing" }).IsVisibleAsync());

        await SelectMatchingFieldAsync(editor, "Source", "Email");
        await SelectMatchingFieldAsync(editor, "Target", "Email");
        Assert.True(await condition.GetByText("Source.Email", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await condition.GetByText("Target.Email", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal(0, await editor.Locator(".ov-mapping-rule-condition--invalid").CountAsync());

        await editor.GetByRole(AriaRole.Tab, new() { Name = "Overview" }).ClickAsync();
        await editor.GetByRole(AriaRole.Tab, new() { Name = "Conditions" }).ClickAsync();
        Assert.True(await condition.GetByText("Source.Email", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await condition.GetByText("Target.Email", new() { Exact = true }).IsVisibleAsync());

        await editor.GetByRole(AriaRole.Button, new() { Name = "Test rule" }).ClickAsync();
        await Page.GetByText("Rule matched.", new() { Exact = true }).WaitForAsync();
    }

    [Fact]
    public async Task Associated_pair_selection_replaces_invalid_direct_target_and_keeps_the_edited_condition()
    {
        await OpenStateAsync("Generic contract");
        var editor = Page.Locator(".ov-mapping-rule");
        await editor.GetByRole(AriaRole.Combobox, new() { Name = "Type", Exact = true }).ClickAsync();
        await Page.GetByText("Associated", new() { Exact = true }).Last.ClickAsync();
        await editor.GetByRole(AriaRole.Tab, new() { Name = "Conditions" }).ClickAsync();

        Assert.True(await editor.Locator(".ov-mapping-rule-condition--invalid").IsVisibleAsync());
        var condition = editor.Locator(".ov-mapping-rule-condition").First;
        await condition.GetByRole(AriaRole.Button, new() { Name = "Edit pair" }).ClickAsync();

        await SelectMatchingFieldAsync(editor, "Source", "Email");
        await SelectMatchingFieldAsync(editor, "Associated Fields", "Email");
        Assert.True(await condition.GetByText("Source.Email", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await condition.GetByText("Associated.Email", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal(0, await editor.Locator(".ov-mapping-rule-condition--invalid").CountAsync());

        await editor.GetByRole(AriaRole.Tab, new() { Name = "Rule", Exact = true }).ClickAsync();
        await editor.GetByRole(AriaRole.Tab, new() { Name = "Conditions" }).ClickAsync();
        Assert.True(await condition.GetByText("Associated.Email", new() { Exact = true }).IsVisibleAsync());
        await editor.GetByRole(AriaRole.Button, new() { Name = "Test rule" }).ClickAsync();
        await Page.GetByText("Rule matched.", new() { Exact = true }).WaitForAsync();
        Assert.True(await Page.GetByText("Customer relationship R-7").IsVisibleAsync());
    }

    private static async Task SelectMatchingFieldAsync(ILocator editor, string paneName, string fieldName)
    {
        var pane = editor.Locator($".ov-mapping-rule__field-pane[aria-label='{paneName}']");
        var leaf = pane.Locator(".ov-tree__label", new() { HasText = fieldName }).Last;
        await leaf.ClickAsync();
    }

    [Theory]
    [InlineData(320)]
    [InlineData(480)]
    [InlineData(720)]
    [InlineData(1024)]
    public async Task Resolution_aware_two_tree_editor_stays_contained_at_supported_widths(int width)
    {
        await Page.SetViewportSizeAsync(1440, 900);
        await OpenStateAsync("Default");
        await Page.Locator(".ov-mapping-rule").GetByRole(AriaRole.Tab, new() { Name = "Conditions" }).ClickAsync();
        var editor = Page.Locator(".ov-mapping-rule").First;
        await editor.EvaluateAsync("(element, width) => { element.parentElement.style.width = width + 'px'; }", width);
        Assert.Equal(2, await Page.Locator(".ov-mapping-rule__field-pane").CountAsync());
        Assert.True(await editor.EvaluateAsync<bool>("e => e.scrollWidth <= e.clientWidth + 2"));
    }

    [Fact]
    public async Task Default_rule_is_axe_clean()
    {
        await OpenStateAsync("AND conditions");
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
