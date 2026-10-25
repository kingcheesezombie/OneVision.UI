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
        var left = Field("left");
        var right = Field("right");
        var request = new OVMappingJoinRequest(left, right, "exact");
        Assert.Same(left, request.Left);
        Assert.Same(right, request.Right);
    }

    [Fact]
    public void One_to_one_is_enforced_only_when_application_cardinality_rejects_endpoint_reuse()
    {
        var joins = new[] { Join("j1", "A", "X") };
        var candidate = OVMappingJoinSemantics.ResolveExistingRelationship(joins, "A", "Y");
        Assert.Null(candidate);

        bool OneToOne(OVStructuredFieldNode left, OVStructuredFieldNode right, OVMappingJoinDefinition? existing)
            => existing is not null || !joins.Any(join => join.LeftKey == left.Key || join.RightKey == right.Key);

        Assert.False(OneToOne(Field("A"), Field("Y"), candidate));
    }

    [Fact]
    public void One_to_many_left_endpoint_does_not_imply_replacement()
    {
        var joins = new[] { Join("j1", "A", "X") };
        var candidate = OVMappingJoinSemantics.ResolveExistingRelationship(joins, "A", "Y");
        Assert.Null(candidate);

        bool OneToManyLeft(OVStructuredFieldNode left, OVStructuredFieldNode right, OVMappingJoinDefinition? existing)
            => existing is not null || !joins.Any(join => join.RightKey == right.Key);

        Assert.True(OneToManyLeft(Field("A"), Field("Y"), candidate));
    }

    [Fact]
    public void One_to_many_right_endpoint_does_not_imply_replacement()
    {
        var joins = new[] { Join("j1", "A", "X") };
        var candidate = OVMappingJoinSemantics.ResolveExistingRelationship(joins, "B", "X");
        Assert.Null(candidate);

        bool OneToManyRight(OVStructuredFieldNode left, OVStructuredFieldNode right, OVMappingJoinDefinition? existing)
            => existing is not null || !joins.Any(join => join.LeftKey == left.Key);

        Assert.True(OneToManyRight(Field("B"), Field("X"), candidate));
    }

    [Fact]
    public void Many_to_many_endpoint_reuse_remains_add_when_application_permits_it()
    {
        var joins = new[] { Join("j1", "A", "X"), Join("j2", "B", "Y") };
        Assert.Null(OVMappingJoinSemantics.ResolveExistingRelationship(joins, "A", "Y"));
        Assert.Null(OVMappingJoinSemantics.ResolveExistingRelationship(joins, "B", "X"));
    }

    [Fact]
    public void Explicit_replacement_targets_the_selected_join_even_when_endpoints_change()
    {
        var joins = new[] { Join("j1", "A", "X"), Join("j2", "B", "Y") };
        var existing = OVMappingJoinSemantics.ResolveExistingRelationship(joins, "A", "Y", "j2");
        Assert.NotNull(existing);
        Assert.Equal("j2", existing!.Key);
    }

    [Fact]
    public void Exact_duplicate_pair_resolves_as_existing_relationship()
    {
        var joins = new[] { Join("j1", "A", "X"), Join("j2", "A", "Y") };
        var existing = OVMappingJoinSemantics.ResolveExistingRelationship(joins, "A", "X");
        Assert.NotNull(existing);
        Assert.Equal("j1", existing!.Key);
    }

    [Fact]
    public void Application_supplied_cardinality_can_reject_an_otherwise_new_pair()
    {
        var joins = new[] { Join("j1", "A", "X") };
        var existing = OVMappingJoinSemantics.ResolveExistingRelationship(joins, "A", "Y");
        Assert.Null(existing);

        Func<OVStructuredFieldNode, OVStructuredFieldNode, OVMappingJoinDefinition?, bool> cardinality = (_, _, _) => false;
        Assert.False(cardinality(Field("A"), Field("Y"), existing));
    }

    private static OVStructuredFieldNode Field(string key)
        => new(key, key, $"$.{key}", OVStructuredNodeKind.Primitive, OVStructuredDataType.String);

    private static OVMappingJoinDefinition Join(string key, string left, string right)
        => new(key, left, right, "exact");
}
