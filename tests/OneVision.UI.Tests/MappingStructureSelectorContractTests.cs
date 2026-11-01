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
    public void Definition_selector_is_omitted_for_direct_structure_types()
    {
        using var context = CreateContext();
        var state = State("simple", requiresDefinition: false, fields: Fields("candidate"));

        var cut = context.Render<OVMappingStructureSelector>(parameters => parameters
            .Add(component => component.State, state)
            .Add(component => component.SelectedType, "simple"));

        Assert.Single(cut.FindComponents<OVSelect<string>>());
    }

    [Fact]
    public void Definition_selector_uses_application_supplied_label_when_required()
    {
        using var context = CreateContext();
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
    public async Task Changing_definition_immediately_hides_stale_structure_and_reports_affected_joins()
    {
        using var context = CreateContext();
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
        Assert.Contains("join-1", change.InvalidJoinKeys);
    }

    [Fact]
    public void Loading_no_definition_and_no_field_states_are_inline_and_explicit()
    {
        using var context = CreateContext();
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
    public void Nested_structure_is_preserved_and_readonly_disables_dependent_selector()
    {
        using var context = CreateContext();
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
        =>
        [
            new($"{prefix}-root", prefix, $"$.{prefix}", OVStructuredNodeKind.Object, OVStructuredDataType.Unknown,
            [
                new($"{prefix}-leaf", $"{prefix}Leaf", $"$.{prefix}.leaf", OVStructuredNodeKind.Primitive, OVStructuredDataType.String)
            ])
        ];

    private static BunitContext CreateContext()
    {
        var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }
}
