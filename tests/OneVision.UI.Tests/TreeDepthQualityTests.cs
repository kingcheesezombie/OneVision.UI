using System.Reflection;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class TreeDepthQualityTests
{
    [Fact]
    public void Flatten_keeps_depth_first_preorder_without_recursing_through_ten_thousand_levels()
    {
        var root = CreateDeepChain(10_000);
        var method = typeof(OVTree).GetMethod("Flatten", BindingFlags.NonPublic | BindingFlags.Static)!;

        var nodes = (IEnumerable<OVTreeNode>)method.Invoke(null, [new[] { root }])!;
        var enumerated = nodes.ToArray();

        Assert.Equal(10_001, enumerated.Length);
        Assert.Equal("node-0", enumerated[0].Key);
        Assert.Equal("node-10000", enumerated[^1].Key);
    }

    [Fact]
    public void Move_cycle_prevention_works_for_deep_descendants_without_stack_overflow()
    {
        var root = CreateDeepChain(10_000);
        var method = typeof(OVTree).GetMethod("IsDescendant", BindingFlags.NonPublic | BindingFlags.Static)!;
        var last = root;
        while (last.Children.Count > 0)
            last = last.Children[0];

        Assert.True((bool)method.Invoke(null, [root, last])!);
        Assert.False((bool)method.Invoke(null, [last, root])!);
        Assert.False((bool)method.Invoke(null, [root, root])!);
    }

    private static OVTreeNode CreateDeepChain(int depth)
    {
        var next = new OVTreeNode($"node-{depth}", $"Node {depth}");
        for (var index = depth - 1; index >= 0; index--)
            next = new OVTreeNode($"node-{index}", $"Node {index}", [next]);
        return next;
    }
}
