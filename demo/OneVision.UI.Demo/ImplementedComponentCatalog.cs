namespace OneVision.UI.Demo;

/// <summary>
/// Active gallery component manifest. Only concrete OneVision.UI components are exposed.
/// Concept-only compositions are deliberately excluded until they have an implementation.
/// </summary>
public static class ImplementedComponentCatalog
{
    private static readonly HashSet<string> ConceptOnlyReferences = new(StringComparer.OrdinalIgnoreCase)
    {
        "OVC-OVERFLOW-MENU",
        "OVC-PAGINATION",
        "OVC-SORTABLE-HEADER",
        "OVC-DIALOG",
        "OVC-CONFIRM-DIALOG"
    };

    public static IReadOnlyList<string> GroupOrder => GovernedComponentCatalog.GroupOrder;

    public static IEnumerable<ComponentDefinition> Components
        => GovernedComponentCatalog.Components
            .Where(component => !ConceptOnlyReferences.Contains(component.Reference))
            .Select(NormaliseImplementedContract);

    public static ComponentDefinition? Find(string slug)
        => Components.FirstOrDefault(component => string.Equals(component.Slug, slug, StringComparison.OrdinalIgnoreCase));

    public static IEnumerable<ComponentDefinition> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return Components;
        var q = query.Trim();
        return Components.Where(component =>
            component.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
            component.Reference.Contains(q, StringComparison.OrdinalIgnoreCase) ||
            component.Description.Contains(q, StringComparison.OrdinalIgnoreCase) ||
            component.UseWhen.Contains(q, StringComparison.OrdinalIgnoreCase) ||
            component.Group.Contains(q, StringComparison.OrdinalIgnoreCase) ||
            component.Kind.ToString().Contains(q, StringComparison.OrdinalIgnoreCase) ||
            component.ImplementationBasis.Contains(q, StringComparison.OrdinalIgnoreCase) ||
            component.States.Any(state => state.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
            component.Behaviours.Any(behaviour => behaviour.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
            component.Tags.Any(tag => tag.Contains(q, StringComparison.OrdinalIgnoreCase)));
    }

    public static string KindLabel(ComponentKind kind) => GovernedComponentCatalog.KindLabel(kind);

    private static ComponentDefinition NormaliseImplementedContract(ComponentDefinition component)
        => component.Reference == "OVC-OVERLAY"
            ? component with
            {
                Description = "Contained contextual working surface with governed right-side and centered placement states.",
                ImplementationBasis = "OVOverlay with semantic OVOverlayPlacement states",
                States = ["Right-side", "Centered 80×80", "Dirty", "Saving", "Error"],
                Behaviours =
                [
                    "Right-side preserves visible parent context for contextual work.",
                    "Centered uses an 80vw × 80vh work surface for larger focused workflows.",
                    "Close, Cancel and allowed backdrop dismissal update the owning Open state.",
                    "Dirty close requires an explicit leave decision."
                ],
                Tags = ["overlay", "drawer", "workspace", "right side", "centered", "80x80"]
            }
            : component;
}
