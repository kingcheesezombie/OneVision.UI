namespace OneVision.UI.Components;

public sealed record OVStructuredFieldNode(
    string Key,
    string Name,
    string Path,
    OVStructuredNodeKind Kind,
    OVStructuredDataType DataType,
    IReadOnlyList<OVStructuredFieldNode> Children,
    bool Disabled = false)
{
    public OVStructuredFieldNode(
        string key,
        string name,
        string path,
        OVStructuredNodeKind kind,
        OVStructuredDataType dataType = OVStructuredDataType.Unknown,
        bool disabled = false)
        : this(key, name, path, kind, dataType, Array.Empty<OVStructuredFieldNode>(), disabled)
    {
    }

    public bool IsEmptyContainer => Kind != OVStructuredNodeKind.Primitive && Children.Count == 0;
}
