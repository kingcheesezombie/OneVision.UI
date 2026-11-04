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
        var required = new[]
        {
            nameof(OVAllocation.Target),
            nameof(OVAllocation.SourceSystem),
            nameof(OVAllocation.FixedSourceSystem),
            nameof(OVAllocation.SourceType),
            nameof(OVAllocation.FixedSourceType),
            nameof(OVAllocation.SourceSearch),
            nameof(OVAllocation.SourceResults),
            nameof(OVAllocation.SourceSelected),
            nameof(OVAllocation.SelectedSource),
            nameof(OVAllocation.ChangeSourceRequested),
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

        Assert.All(required, parameter => Assert.Contains(parameter, Parameters()));
    }

    [Fact]
    public void Allocation_pattern_exposes_constrained_target_finder_state()
    {
        var required = new[]
        {
            nameof(OVAllocation.FindTarget),
            nameof(OVAllocation.TargetSelected),
            nameof(OVAllocation.TargetPlatform),
            nameof(OVAllocation.AvailableTargetPlatforms),
            nameof(OVAllocation.TargetPlatformChanged),
            nameof(OVAllocation.ChangeTargetRequested),
            nameof(OVAllocation.TargetType),
            nameof(OVAllocation.TargetSearch),
            nameof(OVAllocation.TargetResults),
            nameof(OVAllocation.SelectedTarget)
        };

        Assert.All(required, parameter => Assert.Contains(parameter, Parameters()));
    }

    [Fact]
    public void Allocation_pattern_defaults_preserve_known_target_always_belonged_and_platform_choices()
    {
        var pattern = new OVAllocation();

        Assert.False(pattern.FindTarget);
        Assert.False(pattern.TargetSelected);
        Assert.False(pattern.SourceSelected);
        Assert.Equal(OVAllocationTargetPlatform.Eam, pattern.TargetPlatform);
        Assert.Null(pattern.AvailableTargetPlatforms);
        Assert.True(pattern.AlwaysBelonged);
        Assert.Null(pattern.FromDate);
        Assert.Null(pattern.ToDate);
        Assert.Null(pattern.FixedSourceSystem);
        Assert.Null(pattern.FixedSourceType);
    }

    [Fact]
    public void Source_type_is_optional_for_constrained_app_allocations()
    {
        var parameter = typeof(OVAllocation).GetProperty(nameof(OVAllocation.SourceType))!;
        Assert.NotNull(parameter.GetCustomAttribute<ParameterAttribute>());
        Assert.Null(parameter.GetCustomAttribute<EditorRequiredAttribute>());
    }

    [Fact]
    public void Allocation_target_platforms_remain_eam_odm_and_idm()
    {
        Assert.Equal(new[]
        {
            OVAllocationTargetPlatform.Eam,
            OVAllocationTargetPlatform.Odm,
            OVAllocationTargetPlatform.Idm
        }, Enum.GetValues<OVAllocationTargetPlatform>());
    }

    [Fact]
    public void Allocation_pattern_remains_single_vertical_flow_without_tabs_or_steps()
    {
        var parameters = Parameters();
        Assert.DoesNotContain("Tabs", parameters);
        Assert.DoesNotContain("ActiveTab", parameters);
        Assert.DoesNotContain("SelectedTab", parameters);
        Assert.DoesNotContain("Steps", parameters);
    }

    [Fact]
    public void Allocation_pattern_does_not_expose_app_specific_resolution_contract()
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
