namespace OneVision.UI.Demo;

public enum PatternNodeKind
{
    Container,
    Tab,
    Section,
    Pane
}

public enum PatternNodeStatus
{
    Placeholder,
    Draft,
    Approved
}

public sealed class PatternNode
{
    public PatternNode(
        string name,
        string slug,
        PatternNodeKind kind,
        string reference,
        PatternNodeStatus status,
        string description,
        params PatternNode[] children)
    {
        Name = name;
        Slug = slug;
        Kind = kind;
        Reference = reference;
        Status = status;
        Description = description;
        Children = children;
    }

    public string Name { get; }
    public string Slug { get; }
    public PatternNodeKind Kind { get; }
    public string Reference { get; }
    public PatternNodeStatus Status { get; }
    public string Description { get; }
    public IReadOnlyList<PatternNode> Children { get; }
}

/// <summary>
/// Pattern node utilities only. The old speculative pattern manifest was removed.
/// The only gallery manifest is GovernedPatternCatalog, which contains implemented patterns only.
/// </summary>
public static class PatternCatalog
{
    public static PatternNode Resolve(PatternNode root, params string?[] path)
    {
        var current = root;
        foreach (var segment in path.Where(segment => !string.IsNullOrWhiteSpace(segment)))
        {
            var next = current.Children.FirstOrDefault(child =>
                string.Equals(child.Slug, segment, StringComparison.OrdinalIgnoreCase));
            if (next is null)
                return current;
            current = next;
        }
        return current;
    }
}
