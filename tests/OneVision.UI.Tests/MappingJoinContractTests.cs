using System.Reflection;
using Microsoft.AspNetCore.Components;
using OneVision.UI.Components;

namespace OneVision.UI.Tests;

public sealed class MappingJoinContractTests
{
    [Fact]
    public void Mapping_join_exposes_governed_application_inputs_and_callbacks()
    {
        var parameters = typeof(OVMappingJoin)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);

        var required = new[]
        {
            nameof(OVMappingJoin.LeftItems), nameof(OVMappingJoin.RightItems), nameof(OVMappingJoin.Joins), nameof(OVMappingJoin.Methods),
            nameof(OVMappingJoin.LeftCatalogueContent), nameof(OVMappingJoin.RightCatalogueContent), nameof(OVMappingJoin.JoinEligibility),
            nameof(OVMappingJoin.CardinalityRule), nameof(OVMappingJoin.ValidationMessages), nameof(OVMappingJoin.ReadOnly), nameof(OVMappingJoin.Loading),
            nameof(OVMappingJoin.ErrorMessage), nameof(OVMappingJoin.AddRequested), nameof(OVMappingJoin.ReplaceRequested), nameof(OVMappingJoin.RemoveRequested),
            nameof(OVMappingJoin.MethodChanged)
        };

        Assert.All(required, name => Assert.Contains(name, parameters));
    }

    [Fact]
    public void Join_definition_keeps_pair_and_method_as_first_class_state()
    {
        var join = new OVMappingJoinDefinition("join-1", "left-a", "right-b", "normalised", false, "Conflict", true);

        Assert.Equal("left-a", join.LeftKey);
        Assert.Equal("right-b", join.RightKey);
        Assert.Equal("normalised", join.MethodKey);
        Assert.False(join.IsValid);
        Assert.Equal("Conflict", join.ValidationMessage);
        Assert.True(join.ReadOnly);
    }

    [Fact]
    public void Join_item_metadata_carries_required_disabled_and_readonly_without_domain_semantics()
    {
        var item = new OVMappingJoinItem("key", "Label", "Description", Disabled: true, ReadOnly: true, Required: true, Group: "Group");

        Assert.True(item.Disabled);
        Assert.True(item.ReadOnly);
        Assert.True(item.Required);
        Assert.Equal("Group", item.Group);
    }
}
