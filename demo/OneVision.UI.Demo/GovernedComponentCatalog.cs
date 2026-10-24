namespace OneVision.UI.Demo;

/// <summary>
/// Gallery facade over the canonical component catalogue plus newly implemented governed components.
/// </summary>
public static class GovernedComponentCatalog
{
    private static readonly IReadOnlyList<ComponentDefinition> AdditionalComponents =
    [
        new(
            "Copyable Link",
            "copyable-link",
            "Display",
            "OVC-COPYABLE-LINK",
            "Highlighted selectable text with a semantic copy affordance at the trailing edge.",
            "Use for references, identifiers, paths or implementation snippets that users commonly copy.",
            ComponentPreviewKind.Display,
            ComponentKind.Primitive,
            "OVCopyableLink with OneVisionIcon.Copy",
            ["Default", "Disabled", "Long value"],
            ["The copyable text is visually distinct and remains selectable.", "The semantic copy icon is always at the trailing edge.", "Click, Enter or Space copies the full supplied value and announces completion accessibly."],
            ["copy", "reference", "link", "code"]),
        new(
            "Field Mapping",
            "field-mapping",
            "Structured data",
            "OVC-FIELD-MAPPING",
            "Domain-agnostic source-to-target field mapping workspace with governed drag/drop and keyboard interaction.",
            "Use when fields from a structured source must be mapped to a defined target contract.",
            ComponentPreviewKind.StructuredTree,
            ComponentKind.CompositeComponent,
            "OVFieldMapping using OVC-STRUCTURED-FIELD-TREE as the source and a governed target mapping list",
            ["Default", "Required unmapped", "Invalid mapping", "Disabled", "Read-only", "Constants", "Configurable", "Loading", "Empty", "Error", "Responsive"],
            ["The source pane always uses OVC-STRUCTURED-FIELD-TREE; hierarchical fields never introduce another tree convention.", "The target pane is a defined target list with a header and one drop container per target field.", "Drag a source primitive onto an eligible target; eligible targets highlight and invalid targets reject the drop.", "Keyboard users select/focus a source field and activate an eligible target drop zone with Enter or Space.", "Applications supply eligibility, required metadata, constants/configuration capabilities and add/remove/replace/configure callbacks.", "OneVision.UI owns mapped/unmapped/required/invalid/read-only presentation, accessibility, loading/empty/error treatment and responsive layout."],
            ["mapping", "drag drop", "structured", "source", "target", "ingestion", "api"])
    ];

    public static IReadOnlyList<string> GroupOrder => ComponentCatalog.GroupOrder;
    public static IEnumerable<ComponentDefinition> Components => ComponentCatalog.Components.Concat(AdditionalComponents);

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

    public static string KindLabel(ComponentKind kind) => ComponentCatalog.KindLabel(kind);
}
