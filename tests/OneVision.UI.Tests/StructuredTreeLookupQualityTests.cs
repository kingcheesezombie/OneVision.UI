using System.Reflection;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class StructuredTreeLookupQualityTests
{
    [Fact]
    public void Index_resolves_last_of_ten_thousand_sources_without_repeated_full_tree_walk()
    {
        var nodes = Enumerable.Range(0, 10_000)
            .Select(index => new OVStructuredFieldNode(
                $"field-{index}",
                $"Field {index}",
                $"$.field-{index}",
                OVStructuredNodeKind.Primitive,
                OVStructuredDataType.String))
            .ToArray();

        var lookup = Index(nodes);

        Assert.Equal(10_000, lookup.Count);
        Assert.Same(nodes[0], lookup["field-0"]);
        Assert.Same(nodes[9_999], lookup["field-9999"]);
        Assert.False(lookup.ContainsKey("field-10000"));
    }

    [Fact]
    public void Index_preserves_first_preorder_match_when_external_metadata_contains_duplicate_keys()
    {
        var nested = new OVStructuredFieldNode(
            "root", "Object", "$.root", OVStructuredNodeKind.Object, OVStructuredDataType.Unknown,
            [
                new("same", "Nested", "$.root.same", OVStructuredNodeKind.Primitive, OVStructuredDataType.String)
            ]);
        var sibling = new OVStructuredFieldNode(
            "same", "Root sibling", "$.same", OVStructuredNodeKind.Primitive, OVStructuredDataType.String);

        var lookup = Index([nested, sibling]);

        Assert.Equal(2, lookup.Count);
        Assert.Same(nested.Children[0], lookup["same"]);
    }

    [Fact]
    public void Empty_input_has_empty_lookup()
    {
        Assert.Empty(Index([]));
    }

    [Fact]
    public async Task Changing_source_metadata_rebuilds_lookup_and_discards_stale_nodes()
    {
        var first = new OVStructuredFieldNode("alpha", "Alpha", "$.alpha",
            OVStructuredNodeKind.Primitive, OVStructuredDataType.String);
        var second = new OVStructuredFieldNode("beta", "Beta", "$.beta",
            OVStructuredNodeKind.Primitive, OVStructuredDataType.String);
        var tree = new OVStructuredFieldTree { Nodes = [first] };

        var setParameters = typeof(OVStructuredFieldTree)
            .GetMethod("OnParametersSetAsync", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var field = typeof(OVStructuredFieldTree)
            .GetField("_nodesByKey", BindingFlags.Instance | BindingFlags.NonPublic)!;

        await (Task)setParameters.Invoke(tree, null)!;
        var before = (IReadOnlyDictionary<string, OVStructuredFieldNode>)field.GetValue(tree)!;
        Assert.Same(first, before["alpha"]);

        tree.Nodes = [second];
        await (Task)setParameters.Invoke(tree, null)!;
        var after = (IReadOnlyDictionary<string, OVStructuredFieldNode>)field.GetValue(tree)!;
        Assert.False(after.ContainsKey("alpha"));
        Assert.Same(second, after["beta"]);
    }

    private static IReadOnlyDictionary<string, OVStructuredFieldNode> Index(
        IReadOnlyList<OVStructuredFieldNode> nodes)
    {
        var method = typeof(OVStructuredFieldTree)
            .GetMethod("IndexByKey", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.NotNull(method);
        return (IReadOnlyDictionary<string, OVStructuredFieldNode>)method!.Invoke(null, [nodes])!;
    }
}
