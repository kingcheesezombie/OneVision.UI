using System.Reflection;
using Microsoft.AspNetCore.Components;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class AllocationPatternContractTests
{
    [Fact]
    public void Allocation_pattern_exposes_app_owned_target_source_and_action_regions()
    {
        var parameters = typeof(OVAllocation)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);

        var required = new[]
        {
            nameof(OVAllocation.Target),
            nameof(OVAllocation.SourceType),
            nameof(OVAllocation.SourceSearch),
            nameof(OVAllocation.SourceResults),
            nameof(OVAllocation.SelectedSource),
            nameof(OVAllocation.AlwaysBelonged),
            nameof(OVAllocation.AlwaysBelongedChanged),
            nameof(OVAllocation.FromDate),
            nameof(OVAllocation.FromDateChanged),
            nameof(OVAllocation.ToDate),
            nameof(OVAllocation.ToDateChanged),
            nameof(OVAllocation.ValidationSummary),
            nameof(OVAllocation.Actions),
            nameof(OVAllocation.ReadOnly),
            nameof(OVAllocation.Loading),
            nameof(OVAllocation.ErrorMessage)
        };

        Assert.All(required, parameter => Assert.Contains(parameter, parameters));
    }

    [Fact]
    public void Allocation_pattern_defaults_to_always_belonged()
    {
        var pattern = new OVAllocation();
        Assert.True(pattern.AlwaysBelonged);
        Assert.Null(pattern.FromDate);
        Assert.Null(pattern.ToDate);
    }

    [Fact]
    public void Allocation_pattern_does_not_define_domain_cardinality_or_eligibility_rules()
    {
        var parameters = typeof(OVAllocation)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(property => property.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        Assert.DoesNotContain("CardinalityRule", parameters);
        Assert.DoesNotContain("EligibilityRule", parameters);
        Assert.DoesNotContain("SourceTypes", parameters);
        Assert.DoesNotContain("TargetType", parameters);
    }
}
