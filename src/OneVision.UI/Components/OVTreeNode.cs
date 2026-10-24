namespace OneVision.UI.Components;

public sealed record OVTreeNode(
    string Key,
    string Label,
    IReadOnlyList<OVTreeNode> Children,
    bool Disabled = false,
    string? EndText = null,
    string? SearchText = null)
{
    public OVTreeNode(string key, string label, bool disabled = false, string? endText = null, string? searchText = null)
        : this(key, label, Array.Empty<OVTreeNode>(), disabled, endText, searchText)
    {
    }

    public bool IsLeaf => Children.Count == 0;
}
