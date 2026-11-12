using Bunit;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class MappingRuleCompositionContractTests
{
    private static BunitContext Context()
    {
        var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }

    [Fact]
    public async Task Allocation_matching_rule_has_three_mandatory_tabs_without_namespace()
    {
        await using var context = Context();
        var cut = context.Render<OVMappingRule>();
        var tabs = cut.FindComponent<OVTabs>().Instance.Items;
        Assert.Equal(new[] { "overview", "rule", "conditions" }, tabs.Select(tab => tab.Key));
        Assert.DoesNotContain(tabs, tab => tab.Key == "target-namespace");
    }

    [Fact]
    public async Task Allocation_contract_is_displayed_read_only_and_is_not_an_independent_target_selector()
    {
        await using var context = Context();
        var contract = new OVMappingAllocationContract("rule-1", "Synthetic rule", "Asset",
            "ESR Number", "ESR", "IDM", "Person", Revision: "v3");
        var cut = context.Render<OVMappingRule>(p => p
            .Add(x => x.ActiveTab, "rule")
            .Add(x => x.AllocationRuleKey, "rule-1")
            .Add(x => x.AllocationRules, new[] { new OVOption<string>("rule-1", "Synthetic rule") })
            .Add(x => x.AllocationContract, contract));
        Assert.Contains("Source Entity", cut.Markup);
        Assert.Contains("ESR Number", cut.Markup);
        Assert.Contains("IDM", cut.Markup);
        Assert.Contains("Contract revision: v3", cut.Markup);
        Assert.Empty(cut.FindAll(".ov-mapping-rule__section[aria-label='Outputs']"));
        Assert.Empty(cut.FindAll(".ov-mapping-rule__section[aria-label='Source']"));
        Assert.Empty(cut.FindAll(".ov-mapping-rule__section[aria-label='Target Namespace']"));
    }

    [Fact]
    public async Task Disabled_allocation_rule_preserves_inherited_relationship_with_warning()
    {
        await using var context = Context();
        var contract = new OVMappingAllocationContract("legacy", "Disabled", "Attribute",
            "Old email", "EAM", "EAM", "Attribute", Enabled: false, IsValid: false,
            ValidationMessage: "Allocation Rule is disabled");
        var cut = context.Render<OVMappingRule>(p => p
            .Add(x => x.ActiveTab, "rule")
            .Add(x => x.AllocationContract, contract));
        Assert.Contains("Allocation Rule is disabled", cut.Markup);
        Assert.Contains("Old email", cut.Markup);
    }

    [Theory]
    [InlineData(OVMappingRuleCapabilityState.Unavailable, false)]
    [InlineData(OVMappingRuleCapabilityState.Hidden, false)]
    [InlineData(OVMappingRuleCapabilityState.Disabled, true)]
    [InlineData(OVMappingRuleCapabilityState.ReadOnly, true)]
    [InlineData(OVMappingRuleCapabilityState.Enabled, true)]
    public async Task Target_namespace_capability_state_controls_tab_visibility_without_modifying_values(
        OVMappingRuleCapabilityState state, bool visible)
    {
        await using var context = Context();
        var cut = context.Render<OVMappingRule>(p => p
            .Add(x => x.Composition, OVMappingRuleComposition.Legacy)
            .Add(x => x.TargetNamespaceState, state)
            .Add(x => x.TargetProvider, "existing-provider")
            .Add(x => x.OutcomeKey, "AUTO"));
        Assert.Equal(visible, cut.FindComponent<OVTabs>().Instance.Items.Any(tab => tab.Key == "target-namespace"));
        Assert.Equal("existing-provider", cut.Instance.TargetProvider);
        Assert.Equal("AUTO", cut.Instance.OutcomeKey);
    }

    [Fact]
    public async Task Source_and_outputs_omit_without_removing_conditions_or_changing_root_group()
    {
        await using var context = Context();
        var original = new OVMappingRuleGroup("root", OVMappingRuleJoin.Or,
            [new OVMappingRuleCondition("one", "description", "contains", "hello")]);
        var cut = context.Render<OVMappingRule>(p => p
            .Add(x => x.Composition, OVMappingRuleComposition.Legacy)
            .Add(x => x.ActiveTab, "rule")
            .Add(x => x.RootGroup, original)
            .Add(x => x.SourceState, OVMappingRuleCapabilityState.Hidden)
            .Add(x => x.OutputsState, OVMappingRuleCapabilityState.Unavailable));
        Assert.Same(original, cut.Instance.RootGroup);
        Assert.NotNull(cut.Find(".ov-mapping-rule__section[aria-label='Conditions']"));
        Assert.Empty(cut.FindAll(".ov-mapping-rule__section[aria-label='Source']"));
        Assert.Empty(cut.FindAll(".ov-mapping-rule__section[aria-label='Outputs']"));
    }

    [Fact]
    public async Task Field_comparison_uses_governed_second_field_selector_without_join_workspace()
    {
        await using var context = Context();
        var group = new OVMappingRuleGroup("root", OVMappingRuleJoin.And,
            [new OVMappingRuleCondition("one", "source", "field-equals", "target")]);
        var fields = new[]
        {
            new OVMappingRuleFieldDefinition("source", "Source"),
            new OVMappingRuleFieldDefinition("target", "Target")
        };
        var operators = OVMappingRuleOperatorContract.StandardOperators.Append(
            new OVMappingRuleOperatorDefinition("field-equals", "Equals another field",
                [OVMappingRuleFieldType.Text])).ToArray();
        var cut = context.Render<OVMappingRuleGroupEditor>(p => p
            .Add(x => x.Group, group)
            .Add(x => x.FieldDefinitions, fields)
            .Add(x => x.OperatorDefinitions, operators)
            .Add(x => x.AllowFieldComparison, true)
            .Add(x => x.FieldComparisonOperatorKeys, new[] { "field-equals" }));
        Assert.Contains("Compare with field", cut.Markup);
        Assert.Empty(cut.FindAll(".ov-mapping-rule-condition--invalid"));
        Assert.Empty(cut.FindAll(".ov-mapping-join"));
    }

    [Fact]
    public async Task Disabled_conditions_do_not_raise_field_validation_errors()
    {
        await using var context = Context();
        var group = new OVMappingRuleGroup("root", OVMappingRuleJoin.And,
            [new OVMappingRuleCondition("one", "missing", "bad-op", null, Disabled: true, IsValid: false)]);
        var cut = context.Render<OVMappingRuleGroupEditor>(p => p
            .Add(x => x.Group, group)
            .Add(x => x.FieldDefinitions, new[] { new OVMappingRuleFieldDefinition("present", "Present") }));
        Assert.Empty(cut.FindAll(".ov-mapping-rule-condition--invalid"));
    }
}
