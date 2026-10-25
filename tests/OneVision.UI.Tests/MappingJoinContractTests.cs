using System.Reflection;
using Microsoft.AspNetCore.Components;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class MappingJoinContractTests
{
    [Fact]
    public void Mapping_join_exposes_structured_data_inputs_and_callbacks()
    {
        var parameters = typeof(OVMappingJoin)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);

        var required = new[]
        {
            nameof(OVMappingJoin.LeftItems), nameof(OVMappingJoin.RightItems), nameof(OVMappingJoin.LeftFieldStates), nameof(OVMappingJoin.RightFieldStates),
            nameof(OVMappingJoin.Joins), nameof(OVMappingJoin.Methods), nameof(OVMappingJoin.JoinEligibility), nameof(OVMappingJoin.CardinalityRule),
            nameof(OVMappingJoin.ValidationMessages), nameof(OVMappingJoin.ReadOnly), nameof(OVMappingJoin.Loading), nameof(OVMappingJoin.ErrorMessage),
            nameof(OVMappingJoin.AddRequested), nameof(OVMappingJoin.ReplaceRequested), nameof(OVMappingJoin.RemoveRequested), nameof(OVMappingJoin.MethodChanged)
        };

        Assert.All(required, name => Assert.Contains(name, parameters));
        Assert.DoesNotContain("LeftCatalogueContent", parameters);
        Assert.DoesNotContain("RightCatalogueContent", parameters);
        Assert.Equal(typeof(IReadOnlyList<OVStructuredFieldNode>), typeof(OVMappingJoin).GetProperty(nameof(OVMappingJoin.LeftItems))!.PropertyType);
        Assert.Equal(typeof(IReadOnlyList<OVStructuredFieldNode>), typeof(OVMappingJoin).GetProperty(nameof(OVMappingJoin.RightItems))!.PropertyType);
    }

    [Fact]
    public void Structured_field_tree_exposes_governed_drag_and_drop_lifecycle()
    {
        var parameters = typeof(OVStructuredFieldTree)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);

        Assert.Contains(nameof(OVStructuredFieldTree.Draggable), parameters);
        Assert.Contains(nameof(OVStructuredFieldTree.DragStarted), parameters);
        Assert.Contains(nameof(OVStructuredFieldTree.DragEnded), parameters);
        Assert.Contains(nameof(OVStructuredFieldTree.DropMode), parameters);
        Assert.Contains(nameof(OVStructuredFieldTree.CanDrop), parameters);
        Assert.Contains(nameof(OVStructuredFieldTree.Dropped), parameters);
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
    public void Join_field_state_carries_required_and_readonly_without_domain_semantics()
    {
        var state = new OVMappingJoinFieldState("key", Required: true, ReadOnly: true);
        Assert.True(state.Required);
        Assert.True(state.ReadOnly);
    }

    [Fact]
    public void Join_request_uses_structured_fields_on_both_sides()
    {
        var left = new OVStructuredFieldNode("left", "left", "$.left", OVStructuredNodeKind.Primitive, OVStructuredDataType.String);
        var right = new OVStructuredFieldNode("right", "right", "$.right", OVStructuredNodeKind.Primitive, OVStructuredDataType.String);
        var request = new OVMappingJoinRequest(left, right, "exact");
        Assert.Same(left, request.Left);
        Assert.Same(right, request.Right);
    }
}
