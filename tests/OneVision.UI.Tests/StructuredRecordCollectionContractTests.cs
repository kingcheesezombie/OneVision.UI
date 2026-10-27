using System.Reflection;
using Microsoft.AspNetCore.Components;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class StructuredRecordCollectionContractTests
{
    private static readonly OVStructuredFieldNode Primitive =
        new("name", "name", "$.name", OVStructuredNodeKind.Primitive, OVStructuredDataType.String);

    private static readonly OVStructuredFieldNode FirstArray =
        new("items", "items", "$.items", OVStructuredNodeKind.Array, OVStructuredDataType.Unknown,
        [new("item-name", "name", "$.items[].name", OVStructuredNodeKind.Primitive, OVStructuredDataType.String)]);

    private static readonly OVStructuredFieldNode SecondArray =
        new("warnings", "warnings", "$.warnings", OVStructuredNodeKind.Array, OVStructuredDataType.Unknown,
        [new("warning-code", "code", "$.warnings[].code", OVStructuredNodeKind.Primitive, OVStructuredDataType.String)]);

    private static readonly IReadOnlyList<OVStructuredFieldNode> Nodes = [Primitive, FirstArray, SecondArray];

    [Fact]
    public void Structured_tree_exposes_record_collection_semantic_api_without_removing_existing_tree_api()
    {
        Assert.Equal(new[]
        {
            OVStructuredRecordCollectionSelectionMode.None,
            OVStructuredRecordCollectionSelectionMode.SingleArray
        }, Enum.GetValues<OVStructuredRecordCollectionSelectionMode>());

        var parameters = ParametersOf<OVStructuredFieldTree>();
        foreach (var required in new[]
        {
            nameof(OVStructuredFieldTree.RecordCollectionSelectionMode),
            nameof(OVStructuredFieldTree.SelectedRecordCollectionPath),
            nameof(OVStructuredFieldTree.SelectedRecordCollectionPathChanged),
            nameof(OVStructuredFieldTree.SelectionMode),
            nameof(OVStructuredFieldTree.SelectedKeys),
            nameof(OVStructuredFieldTree.SelectedKeysChanged),
            nameof(OVStructuredFieldTree.FocusedKey),
            nameof(OVStructuredFieldTree.FocusedKeyChanged),
            nameof(OVStructuredFieldTree.Searchable),
            nameof(OVStructuredFieldTree.ReadOnly),
            nameof(OVStructuredFieldTree.Draggable),
            nameof(OVStructuredFieldTree.DropMode)
        })
            Assert.Contains(required, parameters);
    }

    [Fact]
    public void Record_collection_mode_disabled_resolves_no_collection()
    {
        Assert.Null(Resolve(Nodes, OVStructuredRecordCollectionSelectionMode.None, FirstArray.Path));
    }

    [Fact]
    public void One_array_can_be_selected_and_selection_can_move_to_another_array()
    {
        Assert.Equal(FirstArray.Path, Select(Nodes, OVStructuredRecordCollectionSelectionMode.SingleArray, null, FirstArray.Path, false));
        Assert.Equal(SecondArray.Path, Select(Nodes, OVStructuredRecordCollectionSelectionMode.SingleArray, FirstArray.Path, SecondArray.Path, false));
    }

    [Fact]
    public void Non_array_and_disabled_array_cannot_become_record_collections()
    {
        Assert.Null(Select(Nodes, OVStructuredRecordCollectionSelectionMode.SingleArray, null, Primitive.Path, false));

        var disabledArray = FirstArray with { Disabled = true };
        Assert.Null(Select([disabledArray], OVStructuredRecordCollectionSelectionMode.SingleArray, null, disabledArray.Path, false));
    }

    [Fact]
    public void Read_only_state_retains_selected_collection_and_prevents_change()
    {
        Assert.Equal(FirstArray.Path, Select(Nodes, OVStructuredRecordCollectionSelectionMode.SingleArray, FirstArray.Path, SecondArray.Path, true));
        Assert.True(IsSelected(FirstArray, OVStructuredRecordCollectionSelectionMode.SingleArray, FirstArray.Path));
    }

    [Fact]
    public void Equivalent_rerender_data_retains_path_based_selection()
    {
        var equivalentNodes = new[]
        {
            new OVStructuredFieldNode("name", "name", "$.name", OVStructuredNodeKind.Primitive, OVStructuredDataType.String),
            new OVStructuredFieldNode("items", "items", "$.items", OVStructuredNodeKind.Array, OVStructuredDataType.Unknown,
            [new("item-name", "name", "$.items[].name", OVStructuredNodeKind.Primitive, OVStructuredDataType.String)]),
            new OVStructuredFieldNode("warnings", "warnings", "$.warnings", OVStructuredNodeKind.Array, OVStructuredDataType.Unknown,
            [new("warning-code", "code", "$.warnings[].code", OVStructuredNodeKind.Primitive, OVStructuredDataType.String)])
        };

        Assert.Equal(FirstArray.Path, Resolve(equivalentNodes, OVStructuredRecordCollectionSelectionMode.SingleArray, FirstArray.Path));
    }

    [Fact]
    public void Selected_state_clears_when_selected_array_disappears_or_ceases_to_be_an_array()
    {
        Assert.Null(Resolve([Primitive, SecondArray], OVStructuredRecordCollectionSelectionMode.SingleArray, FirstArray.Path));

        var changedKind = new OVStructuredFieldNode("items", "items", FirstArray.Path, OVStructuredNodeKind.Object, OVStructuredDataType.Unknown);
        Assert.Null(Resolve([changedKind], OVStructuredRecordCollectionSelectionMode.SingleArray, FirstArray.Path));
    }

    private static string? Resolve(IReadOnlyList<OVStructuredFieldNode> nodes, OVStructuredRecordCollectionSelectionMode mode, string? path)
        => (string?)InvokeSemantics("ResolveSelectedPath", nodes, mode, path);

    private static string? Select(IReadOnlyList<OVStructuredFieldNode> nodes, OVStructuredRecordCollectionSelectionMode mode, string? currentPath, string? candidatePath, bool readOnly)
        => (string?)InvokeSemantics("SelectPath", nodes, mode, currentPath, candidatePath, readOnly);

    private static bool IsSelected(OVStructuredFieldNode node, OVStructuredRecordCollectionSelectionMode mode, string? path)
        => (bool)InvokeSemantics("IsSelected", node, mode, path)!;

    private static object? InvokeSemantics(string methodName, params object?[] args)
    {
        var type = typeof(OVStructuredFieldTree).Assembly.GetType("OneVision.UI.Components.OVStructuredRecordCollectionSemantics", throwOnError: true)!;
        var method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic)!;
        return method.Invoke(null, args);
    }

    private static HashSet<string> ParametersOf<T>()
        => typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);
}
