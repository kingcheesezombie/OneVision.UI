namespace OneVision.UI.Components;

internal static class OVStructuredRecordCollectionSemantics
{
    internal static string? ResolveSelectedPath(
        IReadOnlyList<OVStructuredFieldNode> nodes,
        OVStructuredRecordCollectionSelectionMode mode,
        string? selectedPath)
    {
        if (mode != OVStructuredRecordCollectionSelectionMode.SingleArray || string.IsNullOrWhiteSpace(selectedPath))
            return null;

        var node = FindByPath(nodes, selectedPath);
        return node is { Kind: OVStructuredNodeKind.Array } ? node.Path : null;
    }

    internal static bool IsSelected(
        OVStructuredFieldNode node,
        OVStructuredRecordCollectionSelectionMode mode,
        string? selectedPath)
        => mode == OVStructuredRecordCollectionSelectionMode.SingleArray
           && node.Kind == OVStructuredNodeKind.Array
           && !string.IsNullOrWhiteSpace(selectedPath)
           && string.Equals(node.Path, selectedPath, StringComparison.Ordinal);

    internal static string? SelectPath(
        IReadOnlyList<OVStructuredFieldNode> nodes,
        OVStructuredRecordCollectionSelectionMode mode,
        string? currentPath,
        string? candidatePath,
        bool readOnly)
    {
        var current = ResolveSelectedPath(nodes, mode, currentPath);
        if (readOnly || mode != OVStructuredRecordCollectionSelectionMode.SingleArray || string.IsNullOrWhiteSpace(candidatePath))
            return current;

        var candidate = FindByPath(nodes, candidatePath);
        return candidate is { Kind: OVStructuredNodeKind.Array, Disabled: false }
            ? candidate.Path
            : current;
    }

    private static OVStructuredFieldNode? FindByPath(IEnumerable<OVStructuredFieldNode> nodes, string path)
    {
        foreach (var node in nodes)
        {
            if (string.Equals(node.Path, path, StringComparison.Ordinal))
                return node;

            var child = FindByPath(node.Children, path);
            if (child is not null)
                return child;
        }

        return null;
    }
}
