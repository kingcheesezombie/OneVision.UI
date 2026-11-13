using Bunit;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class MappingRuleConditionEditorContractTests
{
    [Fact]
    public void Legacy_conditions_are_classified_using_operator_semantics_not_value_presence()
    {
        var operators = OVMappingRuleOperatorContract.StandardOperators.Append(
            new OVMappingRuleOperatorDefinition("field-equals", "Equals field",
                [OVMappingRuleFieldType.Text])).ToArray();
        var field = new OVMappingRuleCondition("f", "source", "field-equals", "target");
        var literal = new OVMappingRuleCondition("l", "source", "contains", "NURSE");
        var unary = new OVMappingRuleCondition("u", "source", "is-empty");
        var unknown = new OVMappingRuleCondition("x", "source", "retired-op", "target");

        Assert.Equal(OVMappingRuleOperandKind.Field,
            OVMappingRuleConditionSemantics.Resolve(field, operators, ["field-equals"]));
        Assert.Equal(OVMappingRuleOperandKind.Literal,
            OVMappingRuleConditionSemantics.Resolve(literal, operators, ["field-equals"]));
        Assert.Equal(OVMappingRuleOperandKind.None,
            OVMappingRuleConditionSemantics.Resolve(unary, operators, ["field-equals"]));
        Assert.Null(OVMappingRuleConditionSemantics.Resolve(unknown, operators, ["field-equals"]));
        Assert.Equal("target", field.Value);
    }

    [Fact]
    public void Optional_operand_metadata_does_not_rewrite_legacy_JSON_when_unset()
    {
        var legacy = new OVMappingRuleCondition("old", "source", "field-equals", "target");
        var serialized = System.Text.Json.JsonSerializer.Serialize(legacy);
        Assert.DoesNotContain("OperandKind", serialized);
        Assert.Contains("field-equals", serialized);
        Assert.Contains("target", serialized);

        var explicitCondition = legacy with { OperandKind = OVMappingRuleOperandKind.Field };
        Assert.Contains("OperandKind", System.Text.Json.JsonSerializer.Serialize(explicitCondition));
    }

    [Fact]
    public async Task Compact_list_preserves_mixed_conditions_and_boolean_group_identity()
    {
        await using var ctx = Context();
        var group = new OVMappingRuleGroup("root", OVMappingRuleJoin.Or,
            [new OVMappingRuleCondition("f", "source", "field-equals", "target"),
             new OVMappingRuleCondition("l", "source", "contains", "NURSE")],
            [new OVMappingRuleGroup("nested", OVMappingRuleJoin.And,
                [new OVMappingRuleCondition("u", "source", "is-empty")])]);
        var ops = OVMappingRuleOperatorContract.StandardOperators.Append(
            new OVMappingRuleOperatorDefinition("field-equals", "Equals field",
                [OVMappingRuleFieldType.Text])).ToArray();

        var cut = ctx.Render<OVMappingRuleConditionList>(p => p
            .Add(x => x.Group, group)
            .Add(x => x.SourceFields, new[] { new OVMappingRuleFieldDefinition("source", "Description") })
            .Add(x => x.RightFields, new[] { new OVMappingComparisonField("target", "Staff ID") })
            .Add(x => x.FieldComparisonOperatorKeys, new[] { "field-equals" })
            .Add(x => x.OperatorDefinitions, ops));
        Assert.Contains("Match any (OR)", cut.Markup);
        Assert.Contains("Match all (AND)", cut.Markup);
        Assert.Contains("Field to field", cut.Markup);
        Assert.Contains("Field to value", cut.Markup);
        Assert.Contains("NURSE", cut.Markup);
        Assert.Equal(3, cut.FindAll(".ov-mapping-condition-list__row").Count);
        Assert.Equal("nested", group.Groups[0].Key);
    }

    [Fact]
    public async Task Dialog_commits_valid_literal_through_one_callback_only_on_save()
    {
        await using var ctx = Context();
        OVMappingRuleConditionChange? saved = null;
        var condition = new OVMappingRuleCondition("draft", "source", "contains", "NURSE",
            OperandKind: OVMappingRuleOperandKind.Literal);
        var cut = ctx.Render<OVMappingRuleConditionDialog>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.IsNew, true)
            .Add(x => x.Condition, condition)
            .Add(x => x.GroupKey, "g1")
            .Add(x => x.SourceFields, new[] { new OVMappingRuleFieldDefinition("source", "Description") })
            .Add(x => x.Saved, change => saved = change));
        Assert.Null(saved);
        Assert.Contains("NURSE", cut.Markup);
        await cut.InvokeAsync(() => cut.FindComponent<OVDialog>().Instance.SaveRequested.InvokeAsync());
        Assert.NotNull(saved);
        Assert.Equal("g1", saved.GroupKey);
        Assert.Equal("draft", saved.Condition.Key);
        Assert.Equal("NURSE", saved.Condition.Value);
    }

    [Fact]
    public async Task Changing_source_selection_marks_dialog_dirty_without_mutating_original()
    {
        await using var ctx = Context();
        var original = new OVMappingRuleCondition("one", "source-email", "field-equals", "target-email",
            OperandKind: OVMappingRuleOperandKind.Field);
        var options = new[]
        {
            new OVMappingRuleFieldDefinition("source-email", "Email"),
            new OVMappingRuleFieldDefinition("source-reference", "ExternalReference")
        };
        var op = new OVMappingRuleOperatorDefinition("field-equals", "Equals field",
            [OVMappingRuleFieldType.Text], OperandKind: OVMappingRuleOperandKind.Field);
        var cut = ctx.Render<OVMappingRuleConditionDialog>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Condition, original)
            .Add(x => x.GroupKey, "root")
            .Add(x => x.SourceFields, options)
            .Add(x => x.RightFields, new[] { new OVMappingComparisonField("target-email", "Email") })
            .Add(x => x.FieldComparisonOperatorKeys, new[] { "field-equals" })
            .Add(x => x.OperatorDefinitions, new[] { op }));
        Assert.False(cut.FindComponent<OVDialog>().Instance.Dirty);
        var picker = cut.FindComponents<OVStructuredFieldPicker>().First();
        await cut.InvokeAsync(() => picker.Instance.ValueChanged.InvokeAsync("source-reference"));
        Assert.True(cut.FindComponent<OVDialog>().Instance.Dirty);
        Assert.Equal("source-email", original.OperandKey);
    }

    private static BunitContext Context()
    {
        var ctx = new BunitContext();
        ctx.Services.AddMudServices();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        return ctx;
    }
}
