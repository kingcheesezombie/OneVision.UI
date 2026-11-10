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
    public async Task Rule_has_overview_and_rule_tabs_but_no_namespace_by_default()
    {
        await using var context = Context();
        var cut = context.Render<OVMappingRule>();
        var tabs = cut.FindComponent<OVTabs>().Instance.Items;
        Assert.Equal(new[] { "overview", "rule" }, tabs.Select(tab => tab.Key));
        Assert.DoesNotContain(tabs, tab => tab.Key == "target-namespace");
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
