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
            "Use when inspected source data must map to a declared/templated target contract.",
            ComponentPreviewKind.StructuredTree,
            ComponentKind.CompositeComponent,
            "OVMappingPair using OVC-STRUCTURED-FIELD-TREE for actual/discovered source data and a governed target-contract list",
            ["Default", "Required unmapped", "Invalid mapping", "Disabled", "Read-only", "Constants", "Configurable", "Loading", "Empty", "Error", "Responsive"],
            ["Actual/discovered source data is always rendered through OVC-STRUCTURED-FIELD-TREE.", "The target pane remains a declared/templated contract list rather than pretending target definitions are inspected data.", "Drag a source primitive onto an eligible target; eligible targets highlight and invalid targets reject the drop.", "Keyboard users select/focus a source item and activate an eligible target drop zone with Enter or Space.", "Applications supply cardinality/eligibility, required metadata, constants/configuration capabilities and add/remove/replace/configure callbacks."],
            ["mapping", "pair", "drag drop", "structured", "source", "target", "ingestion", "api"]),
        new(
            "Mapping Join",
            "mapping-join",
            "Mapping",
            "OVC-MAPPING-JOIN",
            "Symmetric structured-data pairing interaction with a configurable comparison method on every join pair.",
            "Use when actual/discovered fields on two data structures must be matched and compared.",
            ComponentPreviewKind.StructuredTree,
            ComponentKind.CompositeComponent,
            "OVMappingJoin composed from OVC-STRUCTURED-FIELD-TREE on both sides plus governed select, pill, toolbar, icon action, validation, alert, progress and empty-state primitives",
            ["Default", "Invalid / conflict", "Required unpaired", "Disabled items", "Read-only", "Multiple joins", "Loading", "Empty", "Error", "Responsive"],
            ["Both sides use OVC-STRUCTURED-FIELD-TREE and therefore share the same hierarchy, search, field metadata and selection grammar.", "Users may drag a primitive field from either tree onto an eligible primitive in the opposite tree; eligible/ineligible/active drop states are governed by OVC-TREE/OVC-STRUCTURED-FIELD-TREE.", "Keyboard users select one primitive on each side and create or replace a join with the same method selector.", "Existing join pairs remain visible and each pair owns its comparison method selector.", "Paired/unpaired, required, invalid/conflict, disabled and read-only states are explicit.", "Applications supply both structures, joins, methods, eligibility/cardinality rules, validation and callbacks; OneVision.UI owns layout, pairing grammar, drag lifecycle, accessibility and responsive density."],
            ["mapping", "join", "match", "compare", "structured field tree", "drag drop", "relationship"]),
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
