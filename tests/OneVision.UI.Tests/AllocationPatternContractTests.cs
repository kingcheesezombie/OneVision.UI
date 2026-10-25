using System.Reflection;
using Microsoft.AspNetCore.Components;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class AllocationPatternContractTests
{
    [Fact]
    public void Allocation_pattern_exposes_effective_odm_assignment_and_technical_detail_regions()
    {
        var parameters = typeof(OVAllocation)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);

        var required = new[]
        {
            nameof(OVAllocation.EffectiveOdm),
            nameof(OVAllocation.AssignmentMode),
            nameof(OVAllocation.SourceAncestor),
            nameof(OVAllocation.UnderlyingEamAllocation),
            nameof(OVAllocation.OdmHierarchyMemberships),
            nameof(OVAllocation.Actions),
            nameof(OVAllocation.UnderlyingAllocationExpanded),
            nameof(OVAllocation.Loading),
            nameof(OVAllocation.ErrorMessage)
        };

        Assert.All(required, parameter => Assert.Contains(parameter, parameters));
    }

    [Fact]
    public void Allocation_pattern_defaults_to_no_effective_assignment()
    {
        var pattern = new OVAllocation();
        Assert.Equal(OVAllocationAssignmentMode.None, pattern.AssignmentMode);
        Assert.False(pattern.UnderlyingAllocationExpanded);
    }

    [Fact]
    public void Allocation_pattern_uses_semantic_direct_inherited_and_none_modes()
    {
        Assert.Equal(new[]
        {
            OVAllocationAssignmentMode.None,
            OVAllocationAssignmentMode.Direct,
            OVAllocationAssignmentMode.Inherited
        }, Enum.GetValues<OVAllocationAssignmentMode>());
    }

    [Fact]
    public void Allocation_pattern_does_not_expose_the_retired_source_search_form_contract()
    {
        var parameters = typeof(OVAllocation)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(property => property.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        Assert.DoesNotContain("Target", parameters);
        Assert.DoesNotContain("SourceType", parameters);
        Assert.DoesNotContain("SourceSearch", parameters);
        Assert.DoesNotContain("SourceResults", parameters);
        Assert.DoesNotContain("SelectedSource", parameters);
        Assert.DoesNotContain("AlwaysBelonged", parameters);
        Assert.DoesNotContain("FromDate", parameters);
        Assert.DoesNotContain("ToDate", parameters);
    }
}
