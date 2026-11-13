using Bunit;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

/// <summary>Reusable condition configuration, independent of EAM's matching engine.</summary>
public sealed class MappingRuleResolutionContractTests
{
    private static BunitContext Context()
    {
        var ctx = new BunitContext();
        ctx.Services.AddMudServices();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        return ctx;
    }

    private static OVStructuredFieldNode Tree(string name, params string[] keys) =>
        new(name, name, name, OVStructuredNodeKind.Object, OVStructuredDataType.Unknown,
            keys.Select(key => new OVStructuredFieldNode(key, key, key, OVStructuredNodeKind.Primitive)).ToArray());

    private static OVMappingResolutionFields Fields() =>
        new([Tree("Source", "src-email")],
            [Tree("Target", "target-email")],
            [Tree("Associated", "assoc-email")],
            [new("src-email", "Email")],
            [new("target-email", "Email")],
            [new("assoc-email", "Email")]);

    private static OVMappingRuleGroup Group(string right) =>
        new("root", OVMappingRuleJoin.And,
            [new OVMappingRuleCondition("first", "src-email", "field-equals", right)]);

    private static readonly OVMappingRuleOperatorDefinition FieldEquals =
        new("field-equals", "Equals field", [OVMappingRuleFieldType.Text]);

    [Fact]
    public async Task Generic_governing_contract_is_authoritative_over_legacy_allocation_adapter()
    {
        await using var ctx = Context();
        var generic = new OVMappingGoverningContract("customer", "Customer Contract",
            "External", "Customer", "Feed", "Directory", "Person");
        var legacy = new OVMappingAllocationContract("eam", "EAM Allocation", "Asset",
            "Episode", "EAM", "IDM", "Person");
        var cut = ctx.Render<OVMappingRule>(p => p
            .Add(x => x.ActiveTab, "rule")
            .Add(x => x.GoverningContract, generic)
            .Add(x => x.GoverningContractKey, "customer")
            .Add(x => x.GoverningContracts, new[] { new OVOption<string>("customer", "Customer Contract") })
            .Add(x => x.AllocationContract, legacy)
            .Add(x => x.AllocationRuleKey, "eam"));
        Assert.Contains("Governing Contract", cut.Markup);
        Assert.Contains("Directory", cut.Markup);
        Assert.DoesNotContain("EAM Allocation", cut.Markup);
        Assert.DoesNotContain("Target Namespace", cut.Markup);
    }

    [Fact]
    public async Task Direct_and_associated_expose_different_right_hand_structures_with_same_group()
    {
        await using var ctx = Context();
        var group = Group("target-email");
        var fields = Fields();
        var cut = ctx.Render<OVMappingRule>(p => p
            .Add(x => x.ActiveTab, "conditions")
            .Add(x => x.RootGroup, group)
            .Add(x => x.ResolutionFields, fields)
            .Add(x => x.FieldComparisonOperatorKeys, new[] { "field-equals" })
            .Add(x => x.OperatorDefinitions, new[] { FieldEquals }));
        Assert.Empty(cut.FindComponents<OVStructuredFieldTree>());
        Assert.Single(cut.FindAll(".ov-mapping-condition-list__row"));
        Assert.Empty(cut.FindAll(".ov-mapping-condition-list__row--invalid"));

        cut.Render(p => p.Add(x => x.ResolutionType, OVMappingRuleResolutionType.Associated));
        Assert.Single(cut.FindAll(".ov-mapping-condition-list__row"));
        Assert.Single(cut.FindAll(".ov-mapping-condition-list__row--invalid"));
        Assert.Same(group, cut.Instance.RootGroup);
    }

    [Fact]
    public async Task Editing_does_not_mutate_the_condition_before_the_dialog_commits()
    {
        await using var ctx = Context();
        OVMappingRuleConditionChange? change = null;
        var initial = Group("target-email");
        var cut = ctx.Render<OVMappingRule>(p => p
            .Add(x => x.ActiveTab, "conditions")
            .Add(x => x.RootGroup, initial)
            .Add(x => x.ResolutionFields, Fields())
            .Add(x => x.ConditionChanged, next => change = next)
            .Add(x => x.FieldComparisonOperatorKeys, new[] { "field-equals" })
            .Add(x => x.OperatorDefinitions, new[] { FieldEquals }));

        var edit = cut.FindComponents<OVMappingRuleConditionList>().First();
        await cut.InvokeAsync(() => edit.Instance.EditRequested.InvokeAsync(
            new OVMappingRuleConditionRequest("root", "first")));
        Assert.Null(change);
        var dialog = cut.FindComponent<OVMappingRuleConditionDialog>();
        Assert.True(dialog.Instance.Open);
        await cut.InvokeAsync(() => dialog.Instance.OpenChanged.InvokeAsync(false));
        Assert.Null(change);
        Assert.Equal("target-email", initial.Conditions[0].Value);
    }

    [Fact]
    public async Task Consuming_application_controls_cross_type_field_compatibility()
    {
        await using var ctx = Context();
        var group = new OVMappingRuleGroup("root", OVMappingRuleJoin.And,
            [new OVMappingRuleCondition("one", "source-date", "field-equals", "target-text")]);
        var op = new OVMappingRuleOperatorDefinition("field-equals", "Equals",
            [OVMappingRuleFieldType.Date, OVMappingRuleFieldType.Text]);
        var basic = ctx.Render<OVMappingRuleGroupEditor>(p => p
            .Add(x => x.Group, group)
            .Add(x => x.AllowFieldComparison, true)
            .Add(x => x.FieldComparisonOperatorKeys, new[] { "field-equals" })
            .Add(x => x.FieldDefinitions, new[] {
                new OVMappingRuleFieldDefinition("source-date", "Date", OVMappingRuleFieldType.Date) })
            .Add(x => x.ComparisonFieldDefinitions, new[] {
                new OVMappingComparisonField("target-text", "Text", OVMappingRuleFieldType.Text) })
            .Add(x => x.OperatorDefinitions, new[] { op })
            .Add(x => x.ComparisonFields, new[] { new OVOption<string>("target-text", "Text") }));
        Assert.Single(basic.FindAll(".ov-mapping-rule-condition--invalid"));

        basic.Render(p => p.Add(x => x.FieldTypesCompatible,
            (Func<OVMappingRuleFieldType, OVMappingRuleFieldType, bool>)((source, target) =>
                source == OVMappingRuleFieldType.Date && target == OVMappingRuleFieldType.Text)));
        Assert.Empty(basic.FindAll(".ov-mapping-rule-condition--invalid"));
    }

    [Fact]
    public async Task Configuring_a_field_without_a_selected_condition_does_not_create_one()
    {
        await using var ctx = Context();
        var cut = ctx.Render<OVMappingRule>(p => p
            .Add(x => x.ActiveTab, "conditions")
            .Add(x => x.RootGroup, Group("target-email"))
            .Add(x => x.ResolutionFields, Fields()));
        Assert.Contains("Field to field", cut.Markup);
        Assert.Equal(1, cut.Instance.RootGroup.Conditions.Count);
    }
}
