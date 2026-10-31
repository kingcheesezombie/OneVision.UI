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
        Assert.Single(cut.FindAll("button"), button => button.TextContent.Contains("Add standard detail", StringComparison.Ordinal));
        Assert.Empty(cut.FindComponents<OVSelect<string>>());
        Assert.DoesNotContain("Preferred name", cut.Markup, StringComparison.Ordinal);
        Assert.DoesNotContain("Email address", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("Required label", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Add_creates_exactly_one_draft_selector_with_pending_mapping_surface()
    {
        await using var context = CreateContext();
        var cut = Render(context, selected: ["detail-email"]);

        await AddButton(cut).ClickAsync();

        var draft = Assert.Single(cut.FindAll(".ov-mapping-pair__target-row--draft"));
        var selector = Assert.Single(cut.FindComponents<OVSelect<string>>());
        Assert.Null(selector.Instance.Value);
        Assert.Equal("Standard Detail", selector.Instance.Label);
        Assert.Contains(selector.Instance.Options, option => option.Value == "detail-name");
        Assert.DoesNotContain(selector.Instance.Options, option => option.Value == "detail-email");
        Assert.DoesNotContain(selector.Instance.Options, option => option.Value == "detail-disabled");
        Assert.Contains("Choose a Standard Detail to enable mapping", draft.TextContent, StringComparison.Ordinal);
        Assert.Equal("true", draft.QuerySelector(".ov-mapping-pair__drop-zone")?.GetAttribute("aria-disabled"));
    }

    [Fact]
    public async Task Selecting_draft_converts_to_compact_committed_row_without_permanent_selector()
    {
        await using var context = CreateContext();
        IReadOnlyCollection<string>? changed = null;
        var cut = Render(context, changed: ids => changed = ids);

        await AddButton(cut).ClickAsync();
        var draft = Assert.Single(cut.FindComponents<OVSelect<string>>());
        await cut.InvokeAsync(() => draft.Instance.ValueChanged.InvokeAsync("detail-name"));

        Assert.Equal(["detail-name"], changed);

        cut.Render(parameters => AddParameters(parameters, selected: ["detail-name"], changed: ids => changed = ids));

        Assert.Empty(cut.FindComponents<OVSelect<string>>());
        Assert.Contains("Preferred name", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("Optional display name", cut.Markup, StringComparison.Ordinal);
        Assert.NotNull(cut.Find("button[aria-label='Change Preferred name standard detail']"));
        Assert.NotNull(cut.Find("button[aria-label='Remove Preferred name standard detail']"));
        Assert.Contains("Drop source field here", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("Mapping required", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Change_exposes_current_and_eligible_choices_but_excludes_duplicates_and_disabled_details()
    {
        await using var context = CreateContext();
        var cut = Render(context, selected: ["detail-name", "detail-email"]);

        await cut.Find("button[aria-label='Change Preferred name standard detail']").ClickAsync();

        var selector = Assert.Single(cut.FindComponents<OVSelect<string>>());
        Assert.Equal("detail-name", selector.Instance.Value);
        Assert.Contains(selector.Instance.Options, option => option.Value == "detail-name");
        Assert.DoesNotContain(selector.Instance.Options, option => option.Value == "detail-email");
        Assert.DoesNotContain(selector.Instance.Options, option => option.Value == "detail-disabled");
    }

    [Fact]
    public async Task Changing_selected_detail_clears_previous_mapping_and_emits_replacement_selection()
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

        await cut.Find("button[aria-label='Change Preferred name standard detail']").ClickAsync();
        var selector = Assert.Single(cut.FindComponents<OVSelect<string>>());
        await cut.InvokeAsync(() => selector.Instance.ValueChanged.InvokeAsync("detail-email"));

        Assert.Equal("detail-name", removedMapping);
        Assert.Equal(["detail-email"], changed);
    }

    [Fact]
    public async Task Mapped_standard_detail_keeps_source_inside_bounded_mapping_surface()
    {
        await using var context = CreateContext();
        var cut = Render(
            context,
            selected: ["detail-name"],
            mappings: [new("detail-name", "source-name", "$.name")]);

        var row = cut.Find(".ov-mapping-pair__target-row--standard-detail");
        var surface = row.QuerySelector(".ov-mapping-pair__drop-zone--mapped");

        Assert.NotNull(surface);
        Assert.Contains("$.name", surface.TextContent, StringComparison.Ordinal);
        Assert.DoesNotContain("Mapping required", row.TextContent, StringComparison.Ordinal);
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
        var draft = Assert.Single(cut.FindComponents<OVSelect<string>>());
        Assert.Contains(draft.Instance.Options, option => option.Value == "detail-name");
    }

    [Fact]
    public async Task Read_only_preserves_compact_rows_and_suppresses_add_change_remove_and_selectors()
    {
        await using var context = CreateContext();
        var cut = Render(
            context,
            selected: ["detail-name"],
            mappings: [new("detail-name", "source-name", "$.name")],
            readOnly: true);

        Assert.DoesNotContain(cut.FindAll("button"), button => button.TextContent.Contains("Add standard detail", StringComparison.Ordinal));
        Assert.Empty(cut.FindAll("button[aria-label^='Change ']") );
        Assert.Empty(cut.FindAll("button[aria-label^='Remove ']").Where(button => button.GetAttribute("aria-label")?.Contains("standard detail", StringComparison.Ordinal) == true));
        Assert.Empty(cut.FindComponents<OVSelect<string>>());
        Assert.Contains("Preferred name", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("Optional display name", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("$.name", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("Required label", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Exhausted_standard_details_hides_add_action_while_committed_rows_remain_compact()
    {
        await using var context = CreateContext();
        var cut = Render(context, selected: ["detail-name", "detail-email"]);

        Assert.DoesNotContain(cut.FindAll("button"), button => button.TextContent.Contains("Add standard detail", StringComparison.Ordinal));
        Assert.Empty(cut.FindComponents<OVSelect<string>>());
        Assert.Equal(2, cut.FindAll(".ov-mapping-pair__target-row--standard-detail").Count);
    }

    [Fact]
    public async Task Search_filters_only_composed_target_rows_and_never_exposes_unselected_catalogue()
    {
        await using var context = CreateContext();
        var cut = Render(context, selected: ["detail-name"]);

        Assert.Contains("Preferred name", cut.Markup, StringComparison.Ordinal);
        Assert.DoesNotContain("Email address", cut.Markup, StringComparison.Ordinal);

        var search = cut.FindComponent<OVSearchBox>();
        await cut.InvokeAsync(() => search.Instance.ValueChanged.InvokeAsync("Required"));

        Assert.Contains("Required label", cut.Markup, StringComparison.Ordinal);
        Assert.DoesNotContain("Preferred name", cut.Markup, StringComparison.Ordinal);
        Assert.DoesNotContain("Email address", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Required_mapping_rows_keep_existing_renderer_and_are_not_standard_detail_rows()
    {
        await using var context = CreateContext();
        var cut = Render(context, selected: ["detail-name"]);

        var requiredRow = cut.FindAll(".ov-mapping-pair__target-row")
            .Single(row => row.TextContent.Contains("Required label", StringComparison.Ordinal));

        Assert.DoesNotContain("ov-mapping-pair__target-row--standard-detail", requiredRow.ClassList);
        Assert.Contains("Required", requiredRow.TextContent, StringComparison.Ordinal);
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
