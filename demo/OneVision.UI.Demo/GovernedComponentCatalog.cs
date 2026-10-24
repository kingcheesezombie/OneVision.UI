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
            "Mapping Pair",
            "mapping-pair",
            "Mapping",
            "OVC-MAPPING-PAIR",
            "Explicit source-to-target mapping interaction with governed drag/drop and keyboard alternatives.",
            "Use when the user must answer which item on the left maps to which item on the right.",
            ComponentPreviewKind.StructuredTree,
            ComponentKind.CompositeComponent,
            "OVMappingPair using OVC-STRUCTURED-FIELD-TREE as the source and a governed target mapping list",
            ["Default", "Required unmapped", "Invalid mapping", "Disabled", "Read-only", "Constants", "Configurable", "Loading", "Empty", "Error", "Responsive"],
            ["The source pane uses OVC-STRUCTURED-FIELD-TREE for structured/hierarchical sources.", "The target pane is a defined list with one governed drop container per target item.", "Drag a source primitive onto an eligible target; eligible targets highlight and invalid targets reject the drop.", "Keyboard users select/focus a source item and activate an eligible target drop zone with Enter or Space.", "Applications supply cardinality/eligibility, required metadata, constants/configuration capabilities and add/remove/replace/configure callbacks.", "OneVision.UI owns mapped/unmapped/required/invalid/read-only presentation, accessibility, loading/empty/error treatment and responsive layout."],
            ["mapping", "pair", "drag drop", "structured", "source", "target", "ingestion", "api"]),
        new(
            "Mapping Rule",
            "mapping-rule",
            "Mapping",
            "OVC-MAPPING-RULE",
            "Generic condition-builder interaction for deciding when a mapping or outcome should occur.",
            "Use when the user must define conditions, operators, grouping and an outcome rather than directly connect two items.",
            ComponentPreviewKind.StructuredTree,
            ComponentKind.CompositeComponent,
            "OVMappingRule composed from governed Select, Text Field, Toggle, Empty State, Alert and Progress controls",
            ["Default", "Invalid", "Read-only", "Nested groups", "Empty", "Loading", "Error"],
            ["Applications supply operands, operators, grouping rules, outcome options, validation and callbacks.", "AND/OR grouping is explicit and nested groups are available only when enabled by the application.", "Incomplete or invalid conditions are shown at the affected row and may also be summarised by the owning pattern.", "Read-only mode preserves the full readable rule without mutation affordances.", "OneVision.UI owns condition/group layout, operator presentation, add/remove interaction, accessibility and responsive behaviour."],
            ["mapping", "rule", "conditions", "and or", "operator", "outcome"])
    ];

    public static IReadOnlyList<string> GroupOrder { get; } = ComponentCatalog.GroupOrder
        .Where(group => group != "Structured data")
        .Concat(["Mapping", "Structured data"])
        .ToArray();

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
