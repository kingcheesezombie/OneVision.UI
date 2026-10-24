namespace OneVision.UI.Models;

public sealed record OVTreeNode(
    string Key,
    string Label,
    IReadOnlyList<OVTreeNode> Children,
    bool Disabled = false)
{
    public OVTreeNode(string key, string label, bool disabled = false)
        : this(key, label, Array.Empty<OVTreeNode>(), disabled)
    {
    }
}

public enum OVTreeSelectionMode
{
    None,
    Single,
    Multiple,
    HierarchicalMultiple,
    LeafOnly
}

public enum OVTreeSelectionState
{
    Unchecked,
    Mixed,
    Checked
}
