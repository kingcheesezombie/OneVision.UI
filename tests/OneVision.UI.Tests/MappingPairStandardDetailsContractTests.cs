using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class MappingPairStandardDetailsContractTests
{
    [Fact]
    public async Task Empty_standard_details_has_one_header_add_action_and_no_catalogue_rows()
    {
        await using var context = CreateContext();
        var cut = Render(context);

        Assert.Contains("No Standard Details added to this source.", cut.Markup, StringComparison.Ordinal);
        Assert.Single(cut.FindAll("button").Where(button => button.TextContent.Contains("Add standard detail", StringComparison.Ordinal)));
        Assert.DoesNotContain("Preferred name", cut.Markup, StringComparison.Ordinal);
        Assert.DoesNotContain("Email address", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("Required label", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Add_creates_one_selector_row_with_only_eligible_unselected_details()
    {
        await using var context = CreateContext();
        var cut = Render(context, selected: ["detail-email"]);

        await AddButton(cut).ClickAsync();

        var selectors = cut.FindComponents<OVSelect<string>>();
        Assert.Equal(2, selectors.Count);
        var draft = selectors.Single(selector => selector.Instance.Value is null);
        Assert.Equal("Standard Detail", draft.Instance.Label);
        Assert.Contains(draft.Instance.Options, option => option.Value == "detail-name");
        Assert.DoesNotContain(draft.Instance.Options, option => option.Value == "detail-email");
        Assert.DoesNotContain(draft.Instance.Options, option => option.Value == "detail-disabled");
        Assert.DoesNotContain("Legacy reference", cut.Markup, StringComparison.Ordinal);
        Assert.Single(cut.FindAll(".ov-mapping-pair__target-row--draft"));
    }

    [Fact]
    public async Task Selecting_draft_adds_one_detail_and_prevents_duplicate_selection()
    {
        await using var context = CreateContext();
        IReadOnlyCollection<string>? changed = null;
        var cut = Render(context, changed: ids => changed = ids);

        await AddButton(cut).ClickAsync();
        var draft = cut.FindComponents<OVSelect<string>>().Single(selector => selector.Instance.Value is null);
        await cut.InvokeAsync(() => draft.Instance.ValueChanged.InvokeAsync("detail-name"));

        Assert.Equal(["detail-name"], changed);

        cut.Render(parameters => AddParameters(parameters, selected: ["detail-name"], changed: ids => changed = ids));
        await AddButton(cut).ClickAsync();
        draft = cut.FindComponents<OVSelect<string>>().Single(selector => selector.Instance.Value is null);
        Assert.DoesNotContain(draft.Instance.Options, option => option.Value == "detail-name");
        Assert.Contains(draft.Instance.Options, option => option.Value == "detail-email");
    }

    [Fact]
    public async Task Removing_selected_detail_clears_mapping_and_makes_it_eligible_again()
    {
        await using var context = CreateContext();
        string? removedMapping = null;
        IReadOnlyCollection<string>? changed = null;
        var cut = Render(
            context,
            selected: ["detail-name"],
            mappings: [new("detail-name", "source-name", "$.name")],
            changed: ids => changed = ids,
            removed: key => removedMapping = key);

        await cut.Find("button[aria-label='Remove Preferred name standard detail']").ClickAsync();

        Assert.Equal("detail-name", removedMapping);
        Assert.Empty(changed ?? []);

        cut.Render(parameters => AddParameters(parameters, changed: ids => changed = ids));
        await AddButton(cut).ClickAsync();
        var draft = cut.FindComponents<OVSelect<string>>().Single(selector => selector.Instance.Value is null);
        Assert.Contains(draft.Instance.Options, option => option.Value == "detail-name");
    }

    [Fact]
    public async Task Read_only_preserves_selected_detail_without_add_remove_or_edit()
    {
        await using var context = CreateContext();
        var cut = Render(context, selected: ["detail-name"], readOnly: true);

        Assert.DoesNotContain(cut.FindAll("button"), button => button.TextContent.Contains("Add standard detail", StringComparison.Ordinal));
        Assert.Empty(cut.FindAll("button[aria-label^='Remove ']").Where(button => button.GetAttribute("aria-label")?.Contains("standard detail", StringComparison.Ordinal) == true));
        var selector = Assert.Single(cut.FindComponents<OVSelect<string>>());
        Assert.True(selector.Instance.ReadOnly);
        Assert.Equal("detail-name", selector.Instance.Value);
        Assert.Contains("Required label", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Exhausted_standard_details_hides_add_action()
    {
        await using var context = CreateContext();
        var cut = Render(context, selected: ["detail-name", "detail-email"]);

        Assert.DoesNotContain(cut.FindAll("button"), button => button.TextContent.Contains("Add standard detail", StringComparison.Ordinal));
        Assert.Equal(2, cut.FindComponents<OVSelect<string>>().Count);
    }

    private static IRenderedComponent<OVMappingPair> Render(
        BunitContext context,
        IReadOnlyCollection<string>? selected = null,
        IReadOnlyList<OVMappingPairEntry>? mappings = null,
        bool readOnly = false,
        Action<IReadOnlyCollection<string>>? changed = null,
        Action<string>? removed = null)
        => context.Render<OVMappingPair>(parameters => AddParameters(parameters, selected, mappings, readOnly, changed, removed));

    private static void AddParameters(
        ComponentParameterCollectionBuilder<OVMappingPair> parameters,
        IReadOnlyCollection<string>? selected = null,
        IReadOnlyList<OVMappingPairEntry>? mappings = null,
        bool readOnly = false,
        Action<IReadOnlyCollection<string>>? changed = null,
        Action<string>? removed = null)
    {
        parameters
            .Add(component => component.SourceItems, SourceItems)
            .Add(component => component.TargetItems, RequiredTargets)
            .Add(component => component.Mappings, mappings ?? [])
            .Add(component => component.StandardDetailsEnabled, true)
            .Add(component => component.StandardDetails, StandardDetails)
            .Add(component => component.SelectedStandardDetailIds, selected ?? [])
            .Add(component => component.ReadOnly, readOnly);

        if (changed is not null)
            parameters.Add(component => component.SelectedStandardDetailIdsChanged, ids => changed(ids));
        if (removed is not null)
            parameters.Add(component => component.RemoveRequested, key => removed(key));
    }

    private static AngleSharp.Dom.IElement AddButton(IRenderedComponent<OVMappingPair> cut)
        => cut.FindAll("button").Single(button => button.TextContent.Contains("Add standard detail", StringComparison.Ordinal));

    private static BunitContext CreateContext()
    {
        var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }

    private static readonly IReadOnlyList<OVStructuredFieldNode> SourceItems =
    [
        new("source-name", "name", "$.name", OVStructuredNodeKind.Primitive, OVStructuredDataType.String)
    ];

    private static readonly IReadOnlyList<OVMappingPairTarget> RequiredTargets =
    [
        new("required-label", "Required label", "Required contract value", Required: true)
    ];

    private static readonly IReadOnlyList<OVStandardDetailDefinition> StandardDetails =
    [
        new("detail-name", "Preferred name", "Optional display name", "String"),
        new("detail-email", "Email address", "Optional email", "String"),
        new("detail-disabled", "Legacy reference", "Retired detail", "String", Enabled: false)
    ];
}
