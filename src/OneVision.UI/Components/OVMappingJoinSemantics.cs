namespace OneVision.UI.Components;

public static class OVMappingJoinSemantics
{
    public static OVMappingJoinDefinition? FindExactPair(
        IReadOnlyList<OVMappingJoinDefinition> joins,
        string leftKey,
        string rightKey)
        => joins.FirstOrDefault(join =>
            string.Equals(join.LeftKey, leftKey, StringComparison.Ordinal) &&
            string.Equals(join.RightKey, rightKey, StringComparison.Ordinal));

    public static OVMappingJoinDefinition? ResolveExistingRelationship(
        IReadOnlyList<OVMappingJoinDefinition> joins,
        string leftKey,
        string rightKey,
        string? explicitReplacementKey = null)
    {
        if (!string.IsNullOrWhiteSpace(explicitReplacementKey))
            return joins.FirstOrDefault(join => string.Equals(join.Key, explicitReplacementKey, StringComparison.Ordinal));

        return FindExactPair(joins, leftKey, rightKey);
    }
}
