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

    private static readonly IReadOnlyList<ComponentDefinition> AdditionalImplementedComponents =
    [
        new("Connection Editor", "connection-editor", "Text & input", "OVC-CONNECTION-EDITOR",
            "Governed reusable connection editor with dedicated SQL, FILE, LDAP and JSON settings.",
            "Use when an application manages a reusable connection object. Source configuration references a connection rather than duplicating its transport settings.",
            ComponentPreviewKind.TextInput, ComponentKind.CompositeComponent,
            "OVConnectionEditor composed from governed OneVision fields, actions, status and validation surfaces",
            ["SQL", "FILE", "LDAP", "JSON", "Testing", "Success", "Error", "Read-only"],
            [
                "Connection type selects a dedicated governed settings surface.",
                "SQL and LDAP expose conditional authentication credentials; applications own secure storage and never return persisted secrets merely to repopulate the editor.",
                "Test connection is application-executed while OneVision.UI owns testing, success and error presentation.",
                "The application owns persistence and validation; the editor owns layout, conditional field grammar, action placement and responsive behaviour."
            ],
            ["connection", "sql", "file", "ldap", "json", "test connection"])
    ];

    public static IReadOnlyList<string> GroupOrder => GovernedComponentCatalog.GroupOrder;

    public static IEnumerable<ComponentDefinition> Components
        => GovernedComponentCatalog.Components
            .Where(component => !ConceptOnlyReferences.Contains(component.Reference))
            .Select(NormaliseImplementedContract)
            .Concat(AdditionalImplementedComponents);

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
    {
        if (component.Reference == "OVC-OVERLAY")
        {
            return component with
            {
                Description = "Governed working surface with distinct right-side and Workspace placement states.",
                ImplementationBasis = "OVOverlay with semantic OVOverlayPlacement states",
                States = ["Right-side", "Workspace", "Dirty", "Saving", "Error"],
                Behaviours =
                [
                    "Right-side preserves visible parent context for contextual work.",
                    "Workspace is a viewport-centred 80vw × 80vh modal work surface for larger focused workflows.",
                    "Workspace uses the viewport as its positioning context rather than a contained gallery or parent surface.",
                    "Close, Cancel and allowed backdrop dismissal update the owning Open state.",
                    "Dirty close requires an explicit leave decision."
                ],
                Tags = ["overlay", "drawer", "workspace", "right side", "viewport centred", "80x80"]
            };
        }

        if (component.Reference == "OVC-TREE")
        {
            return component with
            {
                Description = "Generic searchable hierarchy for navigation/selection with an optional governed structure-edit state.",
                States = ["Default", "Single", "Multiple", "LeafOnly", "Read-only", "StructureEdit"],
                Behaviours =
                [
                    "Default is MultipleHierarchical with search enabled.",
                    "Expansion and focus remain distinct from selection.",
                    "StructureEdit adds root/child/sibling creation, rename/delete/properties actions and drag-and-drop movement without creating a second hierarchy component.",
                    "Drag move covers sibling reorder and reparenting; applications own placement rules, permissions, validation and persistence."
                ],
                Tags = ["hierarchy", "tree", "structure edit", "drag drop", "reparent", "reorder"]
            };
        }

        if (component.Reference == "OVC-MAPPING-JOIN")
        {
            return component with
            {
                Description = "Symmetric structured-data pairing with optional governed Type → Definition → Structure selection on either side.",
                ImplementationBasis = "OVMappingJoin using OVC-STRUCTURED-FIELD-TREE and optional OVMappingStructureState / OVMappingStructureSelector composition",
                States = ["Default", "Asset candidate", "Person Episode candidate", "Structure transition", "Definitions loading", "Structure loading", "No definitions", "Structure empty", "Invalid / conflict", "Read-only", "Loading", "Error", "Responsive"],
                Behaviours =
                [
                    "Either side may optionally expose a semantic Structure Type → Structure Definition → Structure Metadata dependency without creating a second matching control.",
                    "Applications supply type/definition options and resolved metadata; OneVision.UI owns selectors, transition states, stale-field suppression and structure presentation.",
                    "Changing a type or definition hides stale fields immediately without declaring all existing joins invalid before replacement metadata is known.",
                    "When replacement metadata resolves, stable field keys determine which existing joins survive; joins referencing missing keys are suppressed.",
                    "Both sides use OVC-STRUCTURED-FIELD-TREE with left-aligned field names and hierarchy expressed through indentation.",
                    "Drag may start from either side and keyboard field selection plus Create join remains available.",
                    "Endpoint reuse never implies replacement; exact-pair recognition and explicit replacement remain distinct from application-owned cardinality."
                ],
                Tags = ["mapping", "join", "match", "dependent structure", "definition", "structured field tree", "cardinality"]
            };
        }

        return component;
    }
}
