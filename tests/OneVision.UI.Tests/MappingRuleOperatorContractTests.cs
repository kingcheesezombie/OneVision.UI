using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class MappingRuleOperatorContractTests
{
    private static OVMappingRuleCondition Condition(string op, string? value = "value", bool disabled = false)
        => new("condition-1", "description", op, value, Disabled: disabled);

    private static readonly OVMappingRuleFieldDefinition TextField = new("description", "Description");

    [Fact]
    public void Text_field_exposes_all_nine_standard_operators()
    {
        var operations = OVMappingRuleOperatorContract.ForField(TextField);
        Assert.Equal(9, operations.Count);
        Assert.Contains(operations, operation => operation.Key == OVMappingRuleOperatorContract.Wildcard);
    }

    [Fact]
    public void Non_text_fields_do_not_expose_text_matching_operators()
    {
        foreach (var type in new[] { OVMappingRuleFieldType.Number, OVMappingRuleFieldType.Date, OVMappingRuleFieldType.Boolean })
        {
            var operations = OVMappingRuleOperatorContract.ForField(new("description", "Other", type));
            Assert.Equal(4, operations.Count);
            Assert.DoesNotContain(operations, operation => operation.Key == OVMappingRuleOperatorContract.Contains);
        }
    }

    [Fact]
    public void Application_can_restrict_field_operators()
    {
        var field = new OVMappingRuleFieldDefinition("description", "Description",
            AllowedOperatorKeys: [OVMappingRuleOperatorContract.Equals]);
        var available = OVMappingRuleOperatorContract.ForField(field);
        Assert.Single(available);
        Assert.Equal(OVMappingRuleOperatorContract.Equals, available[0].Key);
        Assert.False(OVMappingRuleOperatorContract.Validate(Condition(OVMappingRuleOperatorContract.Contains), field).IsValid);
    }

    [Theory]
    [InlineData("Safeguarding%Level%1")]
    [InlineData("820%INDUCTION")]
    [InlineData("%Safeguarding%")]
    [InlineData(@"Literal\%\_\\")]
    [InlineData("one_two")]
    public void Valid_wildcards_follow_documented_like_escaping(string value)
    {
        Assert.True(OVMappingRuleOperatorContract.TryValidateWildcard(value, out var error));
        Assert.Null(error);
        Assert.True(OVMappingRuleOperatorContract.Validate(Condition(OVMappingRuleOperatorContract.Wildcard, value), TextField).IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("trailing\\")]
    [InlineData(@"invalid\q")]
    public void Invalid_wildcard_escapes_return_feedback(string value)
    {
        Assert.False(OVMappingRuleOperatorContract.TryValidateWildcard(value, out var error));
        Assert.False(string.IsNullOrWhiteSpace(error));
    }

    [Fact]
    public void Empty_operators_accept_no_comparison_value()
    {
        Assert.True(OVMappingRuleOperatorContract.Validate(Condition(OVMappingRuleOperatorContract.IsEmpty, null), TextField).IsValid);
        Assert.True(OVMappingRuleOperatorContract.Validate(Condition(OVMappingRuleOperatorContract.IsNotEmpty, ""), TextField).IsValid);
        Assert.False(OVMappingRuleOperatorContract.Validate(Condition(OVMappingRuleOperatorContract.IsEmpty), TextField).IsValid);
    }

    [Fact]
    public void Disabled_conditions_do_not_block_rule_editing()
    {
        Assert.True(OVMappingRuleOperatorContract.Validate(Condition("invalid", null, disabled: true), null).IsValid);
    }

    [Fact]
    public void Typed_values_are_checked_without_performing_match_evaluation()
    {
        var number = new OVMappingRuleFieldDefinition("description", "Number", OVMappingRuleFieldType.Number);
        var date = new OVMappingRuleFieldDefinition("description", "Date", OVMappingRuleFieldType.Date);
        var boolean = new OVMappingRuleFieldDefinition("description", "Flag", OVMappingRuleFieldType.Boolean);

        Assert.True(OVMappingRuleOperatorContract.Validate(Condition(OVMappingRuleOperatorContract.Equals, "1.25"), number).IsValid);
        Assert.False(OVMappingRuleOperatorContract.Validate(Condition(OVMappingRuleOperatorContract.Equals, "one"), number).IsValid);
        Assert.True(OVMappingRuleOperatorContract.Validate(Condition(OVMappingRuleOperatorContract.Equals, "2026-11-05"), date).IsValid);
        Assert.False(OVMappingRuleOperatorContract.Validate(Condition(OVMappingRuleOperatorContract.Equals, "05/11/26"), date).IsValid);
        Assert.True(OVMappingRuleOperatorContract.Validate(Condition(OVMappingRuleOperatorContract.Equals, "true"), boolean).IsValid);
        Assert.False(OVMappingRuleOperatorContract.Validate(Condition(OVMappingRuleOperatorContract.Equals, "yes"), boolean).IsValid);
    }

    [Fact]
    public void Application_supplied_comparison_values_must_be_eligible()
    {
        var field = new OVMappingRuleFieldDefinition("description", "Description",
            ComparisonValues: [new OVOption<string>("training", "Training"), new OVOption<string>("legacy", "Legacy", Disabled: true)]);
        Assert.True(OVMappingRuleOperatorContract.Validate(Condition(OVMappingRuleOperatorContract.Equals, "training"), field).IsValid);
        Assert.False(OVMappingRuleOperatorContract.Validate(Condition(OVMappingRuleOperatorContract.Equals, "legacy"), field).IsValid);
    }

    [Fact]
    public void Existing_group_and_condition_models_remain_unchanged()
    {
        var condition = new OVMappingRuleCondition("c1", "description", OVMappingRuleOperatorContract.Equals, "Training");
        var group = new OVMappingRuleGroup("root", OVMappingRuleJoin.Or, [condition]);
        Assert.Same(condition, Assert.Single(group.Conditions));
        Assert.Equal(OVMappingRuleJoin.Or, group.Join);
    }
}
