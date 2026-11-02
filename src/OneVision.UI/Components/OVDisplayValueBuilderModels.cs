namespace OneVision.UI.Components;

public enum OVDisplayValueSegmentKind
{
    Target,
    Literal
}

public sealed record OVDisplayValueTargetOption(
    string Key,
    string Label,
    OVMappingTargetKind Kind = OVMappingTargetKind.Contract,
    string? Description = null,
    string? SampleValue = null,
    bool Enabled = true);

public sealed record OVDisplayValueSegment(
    string Id,
    OVDisplayValueSegmentKind Kind,
    string? TargetKey = null,
    string? Literal = null);

public sealed record OVDisplayValueDefinition(
    IReadOnlyList<OVDisplayValueSegment> Segments)
{
    public static OVDisplayValueDefinition Empty { get; } = new(Array.Empty<OVDisplayValueSegment>());
}

public static class OVDisplayValueValidation
{
    public static bool IsValid(
        OVDisplayValueDefinition? definition,
        IEnumerable<OVDisplayValueTargetOption> availableTargets,
        bool allowLiteralOnly = false)
    {
        if (definition is null || definition.Segments.Count == 0)
            return false;

        var targets = availableTargets
            .Where(target => target.Enabled)
            .Select(target => target.Key)
            .ToHashSet(StringComparer.Ordinal);

        var hasMeaningfulSegment = false;
        var hasTarget = false;

        foreach (var segment in definition.Segments)
        {
            switch (segment.Kind)
            {
                case OVDisplayValueSegmentKind.Target:
                    if (string.IsNullOrWhiteSpace(segment.TargetKey) || !targets.Contains(segment.TargetKey))
                        return false;
                    hasTarget = true;
                    hasMeaningfulSegment = true;
                    break;

                case OVDisplayValueSegmentKind.Literal:
                    if (!string.IsNullOrWhiteSpace(segment.Literal))
                        hasMeaningfulSegment = true;
                    break;
            }
        }

        return hasMeaningfulSegment && (allowLiteralOnly || hasTarget);
    }
}
