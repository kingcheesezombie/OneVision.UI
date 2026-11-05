using System.Reflection;
using Microsoft.AspNetCore.Components;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class MappingRuleUiContractTests
{
    [Fact]
    public void Existing_mapping_rule_callback_and_source_options_remain_available()
    {
        var names = ParametersOf<OVMappingRule>();
        Assert.Contains(nameof(OVMappingRule.Operands), names);
        Assert.Contains(nameof(OVMappingRule.Operators), names);
        Assert.Contains(nameof(OVMappingRule.OutcomeOptions), names);
        Assert.Contains(nameof(OVMappingRule.OutcomeContent), names);
        Assert.Contains(nameof(OVMappingRule.AllowNestedGroups), names);
        Assert.Contains(nameof(OVMappingRule.ConditionChanged), names);
        Assert.Contains(nameof(OVMappingRule.AddConditionRequested), names);
        Assert.Contains(nameof(OVMappingRule.GroupJoinChanged), names);
    }

    [Fact]
    public void Typed_fields_and_optional_rule_capabilities_have_semantic_public_parameters()
    {
        var names = ParametersOf<OVMappingRule>();
        Assert.Contains(nameof(OVMappingRule.FieldDefinitions), names);
        Assert.Contains(nameof(OVMappingRule.OperatorDefinitions), names);
        Assert.Contains(nameof(OVMappingRule.AllowConditionDisabling), names);
        Assert.Contains(nameof(OVMappingRule.AllowReordering), names);
        Assert.Contains(nameof(OVMappingRule.ConditionMoveRequested), names);
        Assert.Contains(nameof(OVMappingRule.ShowCasePolicy), names);
        Assert.Contains(nameof(OVMappingRule.CasePolicyChanged), names);
        Assert.Contains(nameof(OVMappingRule.SourceScopeContent), names);
        Assert.Contains(nameof(OVMappingRule.ClassificationContent), names);
        Assert.Contains(nameof(OVMappingRule.TargetResolutionContent), names);
    }

    [Fact]
    public void Rule_test_uses_a_result_callback_and_never_exposes_a_persistence_contract()
    {
        var parameters = ParametersOf<OVMappingRule>();
        Assert.Contains(nameof(OVMappingRule.TestInputContent), parameters);
        Assert.Contains(nameof(OVMappingRule.TestRuleAsync), parameters);
        Assert.Equal(typeof(Func<Task<OVMappingRuleTestResult>>),
            typeof(OVMappingRule).GetProperty(nameof(OVMappingRule.TestRuleAsync))!.PropertyType);
        Assert.DoesNotContain("Approve", parameters);
        Assert.DoesNotContain("SaveAllocation", parameters);
        Assert.DoesNotContain("Persist", parameters);
    }

    [Fact]
    public void Test_result_retains_conditions_and_ambiguity_as_display_only_evidence()
    {
        var evidence = new[] { new OVMappingRuleTestConditionResult("c1", "Episode.Description contains Safeguarding", true) };
        var result = new OVMappingRuleTestResult("Safeguarding Level 1", true, evidence, "Training", Ambiguous: true);
        Assert.True(result.Matched);
        Assert.True(result.Ambiguous);
        Assert.Single(result.Conditions);
        Assert.Equal("Training", result.Outcome);
    }

    private static HashSet<string> ParametersOf<T>()
        => typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);
}
