using System.Reflection;
using Microsoft.AspNetCore.Components;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class AllocationPatternContractTests
{
    [Fact]
    public void Allocation_pattern_exposes_known_target_source_and_effective_date_regions()
    {
        var parameters = Parameters();

        var required = new[]
        {
            nameof(OVAllocation.Target),
            nameof(OVAllocation.SourceType),
            nameof(OVAllocation.SourceSearch),
            nameof(OVAllocation.SourceResults),
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
    public void Allocation_pattern_exposes_optional_target_finder_state()
    {
        var parameters = Parameters();

        var required = new[]
        {
            nameof(OVAllocation.FindTarget),
            nameof(OVAllocation.TargetPlatform),
            nameof(OVAllocation.TargetPlatformChanged),
            nameof(OVAllocation.TargetType),
            nameof(OVAllocation.TargetSearch),
            nameof(OVAllocation.TargetResults),
            nameof(OVAllocation.SelectedTarget)
        };

        Assert.All(required, parameter => Assert.Contains(parameter, parameters));
    }

    [Fact]
    public void Allocation_pattern_defaults_to_known_target_and_always_belonged()
    {
        var pattern = new OVAllocation();

        Assert.False(pattern.FindTarget);
        Assert.Equal(OVAllocationTargetPlatform.Eam, pattern.TargetPlatform);
        Assert.True(pattern.AlwaysBelonged);
        Assert.Null(pattern.FromDate);
        Assert.Null(pattern.ToDate);
    }

    [Fact]
    public void Allocation_target_platforms_are_governed_as_eam_odm_and_idm()
    {
        Assert.Equal(new[]
        {
            OVAllocationTargetPlatform.Eam,
            OVAllocationTargetPlatform.Odm,
            OVAllocationTargetPlatform.Idm
        }, Enum.GetValues<OVAllocationTargetPlatform>());
    }

    [Fact]
    public void Allocation_pattern_is_single_vertical_flow_and_has_no_tab_contract()
    {
        var parameters = Parameters();

        Assert.DoesNotContain("Tabs", parameters);
        Assert.DoesNotContain("ActiveTab", parameters);
        Assert.DoesNotContain("SelectedTab", parameters);
        Assert.DoesNotContain("Steps", parameters);
    }

    [Fact]
    public void Allocation_pattern_does_not_expose_resolution_status_contract()
    {
        var parameters = Parameters();

        Assert.DoesNotContain("EffectiveOdm", parameters);
        Assert.DoesNotContain("AssignmentMode", parameters);
        Assert.DoesNotContain("SourceAncestor", parameters);
        Assert.DoesNotContain("UnderlyingEamAllocation", parameters);
        Assert.DoesNotContain("OdmHierarchyMemberships", parameters);
    }

    private static HashSet<string> Parameters()
        => typeof(OVAllocation)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);
}
