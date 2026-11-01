using System.Reflection;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class MappingStructureSelectorContractTests
{
    [Fact]
    public void Mapping_join_exposes_side_neutral_dependent_structure_contract()
    {
        var parameters = typeof(OVMappingJoin)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);

        Assert.Contains(nameof(OVMappingJoin.LeftStructure), parameters);
        Assert.Contains(nameof(OVMappingJoin.RightStructure), parameters);
        Assert.Contains(nameof(OVMappingJoin.LeftStructureChanged), parameters);
        Assert.Contains(nameof(OVMappingJoin.RightStructureChanged), parameters);
    }

    [Fact]
    public void Structure_type_semantically_declares_whether_a_definition_is_required()
    {
        var simple = new OVMappingStructureType("simple", "Simple");
        var dependent = new OVMappingStructureType("asset", "Asset", RequiresDefinition: true, DefinitionLabel: "Asset");

        Assert.False(simple.RequiresDefinition);
        Assert.True(dependent.RequiresDefinition);
        Assert.Equal("Asset", dependent.DefinitionLabel);
    }

    [Fact]
    public async Task Definition_selector_is_omitted_for_direct_structure_types()
    {
        await using var context = CreateContext();
        var state = State("simple", requiresDefinition: false, fields: Fields("candidate"));

        var cut = context.Render<OVMappingStructureSelector>(parameters => parameters
            .Add(component => component.State, state)
            .Add(component => component.SelectedType, "simple"));

        Assert.Single(cut.FindComponents<OVSelect<string>>());
    }

    [Fact]
    public async Task Definition_selector_uses_application_supplied_label_when_required()
    {
        await using var context = CreateContext();
        var state = State("asset", requiresDefinition: true, fields: [], definitionLabel: "Asset") with
        {
            Definitions = [new("esr", "ESR Number")],
            SelectedDefinition = "esr"
        };

        var cut = context.Render<OVMappingStructureSelector>(parameters => parameters
            .Add(component => component.State, state)
            .Add(component => component.SelectedType, "asset")
            .Add(component => component.SelectedDefinition, "esr"));

        var selects = cut.FindComponents<OVSelect<string>>();
        Assert.Equal(2, selects.Count);
        Assert.Equal("Asset", selects[1].Instance.Label);
    }

    [Fact]
    public async Task Changing_definition_immediately_hides_stale_structure_without_declaring_all_joins_invalid()
    {
        await using var context = CreateContext();
        OVMappingStructureSelectionChange? change = null;
        var oldFields = Fields("old");
        var right = State("asset", requiresDefinition: true, fields: oldFields, definitionLabel: "Asset") with
        {
            Definitions = [new("one", "One"), new("two", "Two")],
            SelectedDefinition = "one"
        };

        var cut = context.Render<OVMappingJoin>(parameters => parameters
            .Add(component => component.LeftItems, Fields("left"))
            .Add(component => component.RightStructure, right)
            .Add(component => component.Joins, [new OVMappingJoinDefinition("join-1", "left-leaf", "old-leaf", "exact")])
            .Add(component => component.Methods, [new OVOption<string>("exact", "Exact")])
            .Add(component => component.RightStructureChanged, EventCallback.Factory.Create<OVMappingStructureSelectionChange>(this, value => change = value)));

        Assert.Contains("old", cut.Markup, StringComparison.Ordinal);

        var selector = cut.FindComponent<OVMappingStructureSelector>();
        await cut.InvokeAsync(() => selector.Instance.DefinitionChanged.InvokeAsync("two"));

        Assert.DoesNotContain("old-leaf", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("Loading fields", cut.Markup, StringComparison.Ordinal);
        Assert.NotNull(change);
        Assert.Equal("asset", change!.TypeKey);
        Assert.Equal("two", change.DefinitionKey);
        Assert.Empty(change.InvalidJoinKeys);
    }

    [Fact]
    public async Task Replacement_structure_retains_shared_right_key_and_excludes_removed_right_key()
    {
        await using var context = CreateContext();
        OVMappingStructureSelectionChange? change = null;
        var source = Structure("source", "Source", ("source-id", "sourceId", "$.source.id"), ("source-surname", "surname", "$.source.surname"));
        var oldFields = Structure("candidate", "Candidate", ("person-code", "personCode", "$.candidate.personCode"), ("surname", "surname", "$.candidate.surname"));
        var newFields = Structure("replacement", "Replacement", ("person-code", "employeeCode", "$.replacement.employeeCode"), ("dob", "dateOfBirth", "$.replacement.dateOfBirth"));
        var oldState = State("candidate", requiresDefinition: true, fields: oldFields, definitionLabel: "Candidate") with
        {
            Definitions = [new("a", "Definition A"), new("b", "Definition B")],
            SelectedDefinition = "a"
        };
        var joins = new[]
        {
            new OVMappingJoinDefinition("keep", "source-id", "person-code", "exact"),
            new OVMappingJoinDefinition("remove", "source-surname", "surname", "exact")
        };

        var cut = context.Render<OVMappingJoin>(parameters => parameters
            .Add(component => component.LeftItems, source)
            .Add(component => component.RightStructure, oldState)
            .Add(component => component.Joins, joins)
            .Add(component => component.Methods, [new OVOption<string>("exact", "Exact")])
            .Add(component => component.RightStructureChanged, EventCallback.Factory.Create<OVMappingStructureSelectionChange>(this, value => change = value)));

        Assert.Contains("2 configured", cut.Markup, StringComparison.Ordinal);

        var selector = cut.FindComponent<OVMappingStructureSelector>();
        await cut.InvokeAsync(() => selector.Instance.DefinitionChanged.InvokeAsync("b"));
        Assert.Empty(change!.InvalidJoinKeys);
        Assert.Contains("Loading fields", cut.Markup, StringComparison.Ordinal);
        Assert.DoesNotContain("$.candidate.surname", cut.Markup, StringComparison.Ordinal);

        var replacement = oldState with { SelectedDefinition = "b", Fields = newFields, StructureLoading = false };
        cut.SetParametersAndRender(parameters => parameters.Add(component => component.RightStructure, replacement));

        Assert.Contains("1 configured", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("$.replacement.employeeCode", cut.Markup, StringComparison.Ordinal);
        Assert.DoesNotContain("$.candidate.surname", cut.Markup, StringComparison.Ordinal);
        Assert.DoesNotContain("$.replacement.dateOfBirth ↔", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Left_side_transition_uses_left_keys_symmetrically()
    {
        await using var context = CreateContext();
        var right = Structure("right", "Right", ("right-code", "code", "$.right.code"), ("right-name", "name", "$.right.name"));
        var oldLeft = Structure("old-left", "Old", ("shared-left", "legacyCode", "$.old.code"), ("removed-left", "legacyName", "$.old.name"));
        var newLeft = Structure("new-left", "New", ("shared-left", "personCode", "$.new.personCode"), ("new-left", "dateOfBirth", "$.new.dateOfBirth"));
        var oldState = State("source", requiresDefinition: true, fields: oldLeft, definitionLabel: "Source") with
        {
            Definitions = [new("a", "Definition A"), new("b", "Definition B")],
            SelectedDefinition = "a"
        };
        var joins = new[]
        {
            new OVMappingJoinDefinition("keep", "shared-left", "right-code", "exact"),
            new OVMappingJoinDefinition("remove", "removed-left", "right-name", "exact")
        };

        var cut = context.Render<OVMappingJoin>(parameters => parameters
            .Add(component => component.LeftStructure, oldState)
            .Add(component => component.RightItems, right)
            .Add(component => component.Joins, joins)
            .Add(component => component.Methods, [new OVOption<string>("exact", "Exact")]));

        var selector = cut.FindComponents<OVMappingStructureSelector>().Single();
        await cut.InvokeAsync(() => selector.Instance.DefinitionChanged.InvokeAsync("b"));
        Assert.Contains("Loading fields", cut.Markup, StringComparison.Ordinal);

        cut.SetParametersAndRender(parameters => parameters.Add(component => component.LeftStructure, oldState with { SelectedDefinition = "b", Fields = newLeft }));

        Assert.Contains("1 configured", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("$.new.personCode", cut.Markup, StringComparison.Ordinal);
        Assert.DoesNotContain("$.old.name", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Temporary_loading_does_not_permanently_remove_a_surviving_join()
    {
        await using var context = CreateContext();
        var source = Structure("source", "Source", ("source-id", "id", "$.source.id"));
        var oldFields = Structure("old", "Old", ("stable", "oldCode", "$.old.code"));
        var newFields = Structure("new", "New", ("stable", "newCode", "$.new.code"));
        var state = State("candidate", requiresDefinition: true, fields: oldFields, definitionLabel: "Candidate") with
        {
            Definitions = [new("a", "Definition A"), new("b", "Definition B")],
            SelectedDefinition = "a"
        };

        var cut = context.Render<OVMappingJoin>(parameters => parameters
            .Add(component => component.LeftItems, source)
            .Add(component => component.RightStructure, state)
            .Add(component => component.Joins, [new OVMappingJoinDefinition("keep", "source-id", "stable", "exact")])
            .Add(component => component.Methods, [new OVOption<string>("exact", "Exact")]));

        var selector = cut.FindComponent<OVMappingStructureSelector>();
        await cut.InvokeAsync(() => selector.Instance.DefinitionChanged.InvokeAsync("b"));

        cut.SetParametersAndRender(parameters => parameters.Add(component => component.RightStructure, state with
        {
            SelectedDefinition = "b",
            StructureLoading = true,
            Fields = []
        }));
        Assert.Contains("Loading fields", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("0 configured", cut.Markup, StringComparison.Ordinal);

        cut.SetParametersAndRender(parameters => parameters.Add(component => component.RightStructure, state with
        {
            SelectedDefinition = "b",
            StructureLoading = false,
            Fields = newFields
        }));
        Assert.Contains("1 configured", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("$.new.code", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Changing_type_clears_previous_definition()
    {
        await using var context = CreateContext();
        OVMappingStructureSelectionChange? change = null;
        var state = new OVMappingStructureState
        {
            TypeLabel = "Candidate type",
            Types =
            [
                new("asset", "Asset", RequiresDefinition: true, DefinitionLabel: "Asset"),
                new("simple", "Simple")
            ],
            SelectedType = "asset",
            Definitions = [new("one", "One")],
            SelectedDefinition = "one",
            Fields = Fields("asset")
        };

        var cut = context.Render<OVMappingJoin>(parameters => parameters
            .Add(component => component.LeftItems, Fields("left"))
            .Add(component => component.RightStructure, state)
            .Add(component => component.Methods, [new OVOption<string>("exact", "Exact")])
            .Add(component => component.RightStructureChanged, EventCallback.Factory.Create<OVMappingStructureSelectionChange>(this, value => change = value)));

        var selector = cut.FindComponent<OVMappingStructureSelector>();
        await cut.InvokeAsync(() => selector.Instance.TypeChanged.InvokeAsync("simple"));

        Assert.NotNull(change);
        Assert.Equal("simple", change!.TypeKey);
        Assert.Null(change.DefinitionKey);
        Assert.Empty(change.InvalidJoinKeys);
    }

    [Fact]
    public async Task Loading_no_definition_and_no_field_states_are_inline_and_explicit()
    {
        await using var context = CreateContext();
        var loading = State("asset", requiresDefinition: true, fields: [], definitionLabel: "Asset") with
        {
            DefinitionsLoading = true
        };

        var loadingCut = RenderJoin(context, loading);
        Assert.Contains("Loading available definitions", loadingCut.Markup, StringComparison.Ordinal);

        var none = State("asset", requiresDefinition: true, fields: [], definitionLabel: "Asset") with
        {
            NoDefinitionsText = "No Asset definitions are available."
        };
        var noDefinitionsCut = RenderJoin(context, none);
        Assert.Contains("No Asset definitions are available.", noDefinitionsCut.Markup, StringComparison.Ordinal);

        var empty = State("simple", requiresDefinition: false, fields: []) with
        {
            NoFieldsText = "No matchable fields are available for this selection."
        };
        var emptyCut = RenderJoin(context, empty);
        Assert.Contains("No matchable fields are available for this selection.", emptyCut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Nested_structure_is_preserved_and_readonly_disables_dependent_selector()
    {
        await using var context = CreateContext();
        var nested = new OVStructuredFieldNode("root", "root", "$.root", OVStructuredNodeKind.Object, OVStructuredDataType.Unknown,
        [
            new("child", "child", "$.root.child", OVStructuredNodeKind.Object, OVStructuredDataType.Unknown,
            [
                new("leaf", "leaf", "$.root.child.leaf", OVStructuredNodeKind.Primitive, OVStructuredDataType.String)
            ])
        ]);
        var state = State("asset", requiresDefinition: true, fields: [nested], definitionLabel: "Person Episode") with
        {
            Definitions = [new("employment", "Employment Episode")],
            SelectedDefinition = "employment"
        };

        var cut = context.Render<OVMappingJoin>(parameters => parameters
            .Add(component => component.LeftItems, Fields("left"))
            .Add(component => component.RightStructure, state)
            .Add(component => component.Methods, [new OVOption<string>("exact", "Exact")])
            .Add(component => component.ReadOnly, true));

        Assert.Contains("leaf", cut.Markup, StringComparison.Ordinal);
        Assert.All(cut.FindComponents<OVSelect<string>>().Where(select => select.Instance.Label is "Candidate type" or "Person Episode"), select => Assert.True(select.Instance.Disabled));
    }

    private static IRenderedComponent<OVMappingJoin> RenderJoin(BunitContext context, OVMappingStructureState state)
        => context.Render<OVMappingJoin>(parameters => parameters
            .Add(component => component.LeftItems, Fields("left"))
            .Add(component => component.RightStructure, state)
            .Add(component => component.Methods, [new OVOption<string>("exact", "Exact")]));

    private static OVMappingStructureState State(string type, bool requiresDefinition, IReadOnlyList<OVStructuredFieldNode> fields, string? definitionLabel = null)
        => new()
        {
            TypeLabel = "Candidate type",
            Types = [new(type, type, requiresDefinition, definitionLabel)],
            SelectedType = type,
            Fields = fields
        };

    private static IReadOnlyList<OVStructuredFieldNode> Fields(string prefix)
        => Structure(prefix, prefix, ($"{prefix}-leaf", $"{prefix}Leaf", $"$.{prefix}.leaf"));

    private static IReadOnlyList<OVStructuredFieldNode> Structure(string rootKey, string rootLabel, params (string Key, string Label, string Path)[] leaves)
        =>
        [
            new(rootKey, rootLabel, $"$.{rootLabel}", OVStructuredNodeKind.Object, OVStructuredDataType.Unknown,
                leaves.Select(leaf => new OVStructuredFieldNode(leaf.Key, leaf.Label, leaf.Path, OVStructuredNodeKind.Primitive, OVStructuredDataType.String)).ToArray())
        ];

    private static BunitContext CreateContext()
    {
        var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }
}
