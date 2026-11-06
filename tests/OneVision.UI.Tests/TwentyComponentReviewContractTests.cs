using System.Diagnostics;
using System.Reflection;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class TwentyComponentReviewContractTests
{
    private static BunitContext NewContext()
    {
        var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }

    public static TheoryData<Type, string[]> ReviewedContracts => new()
    {
        { typeof(OVMappingPair), [nameof(OVMappingPair.Mappings), nameof(OVMappingPair.ReadOnly), nameof(OVMappingPair.AddRequested)] },
        { typeof(OVMappingJoin), [nameof(OVMappingJoin.Joins), nameof(OVMappingJoin.JoinEligibility), nameof(OVMappingJoin.ReadOnly)] },
        { typeof(OVMappingRuleGroupEditor), [nameof(OVMappingRuleGroupEditor.Group), nameof(OVMappingRuleGroupEditor.ReadOnly)] },
        { typeof(OVMappingRule), [nameof(OVMappingRule.RootGroup), nameof(OVMappingRule.TestContextKey)] },
        { typeof(OVMappingStructureSelector), [nameof(OVMappingStructureSelector.State), nameof(OVMappingStructureSelector.DefinitionsLoading)] },
        { typeof(OVMappingPairSourceTree), [nameof(OVMappingPairSourceTree.Nodes), nameof(OVMappingPairSourceTree.ReadOnly)] },
        { typeof(OVTree), [nameof(OVTree.SelectionMode), nameof(OVTree.SelectedKeysChanged), nameof(OVTree.ReadOnly)] },
        { typeof(OVStructuredFieldTree), [nameof(OVStructuredFieldTree.RecordCollectionSelectionMode), nameof(OVStructuredFieldTree.ReadOnly)] },
        { typeof(OVSelectionList), [nameof(OVSelectionList.Multiple), nameof(OVSelectionList.Disabled)] },
        { typeof(OVSelectionSummary), [nameof(OVSelectionSummary.MaxVisibleItems), nameof(OVSelectionSummary.OnClear)] },
        { typeof(OVMultiSelect), [nameof(OVMultiSelect.Options), nameof(OVMultiSelect.ReadOnly)] },
        { typeof(OVAutocomplete<string>), [nameof(OVAutocomplete<string>.SearchFunc), nameof(OVAutocomplete<string>.ReadOnly)] },
        { typeof(OVSearchBox), [nameof(OVSearchBox.ValueChanged), nameof(OVSearchBox.Disabled)] },
        { typeof(OVDataGrid), [nameof(OVDataGrid.RefreshErrorMessage), nameof(OVDataGrid.RetryRequested)] },
        { typeof(OVList), [nameof(OVList.ItemActions), nameof(OVList.ItemActivated)] },
        { typeof(OVListPage), [nameof(OVListPage.RefreshErrorMessage), nameof(OVListPage.RetryRequested)] },
        { typeof(OVPagination), [nameof(OVPagination.PageChanged), nameof(OVPagination.Busy)] },
        { typeof(OVFileUpload), [nameof(OVFileUpload.RemoveFileRequested), nameof(OVFileUpload.ReadOnly)] },
        { typeof(OVConnectionEditor), [nameof(OVConnectionEditor.TestState), nameof(OVConnectionEditor.SaveRequested)] },
        { typeof(OVSourceDiscovery), [nameof(OVSourceDiscovery.Candidates), nameof(OVSourceDiscovery.SelectedIdChanged)] }
    };

    [Theory]
    [MemberData(nameof(ReviewedContracts))]
    public void All_twenty_governed_component_contracts_exist_without_application_ownership(Type component, string[] parameters)
    {
        var publicParameters = component.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(p => p.Name).ToHashSet(StringComparer.Ordinal);
        Assert.All(parameters, p => Assert.Contains(p, publicParameters));
    }

    [Fact]
    public void MappingPair_target_and_source_lookups_preserve_first_mapping_and_scale_to_large_catalogues()
    {
        var mappings = Enumerable.Range(0, 8_000)
            .Select(i => new OVMappingPairEntry($"target-{i}", $"source-{i}")).ToList();
        mappings.Add(new OVMappingPairEntry("target-0", "second-source"));
        var pair = new OVMappingPair { Mappings = mappings };
        var start = Stopwatch.StartNew();
        InvokePrivate(pair, "OnParametersSet");
        var index = (Dictionary<string, OVMappingPairEntry>)PrivateField(pair, "_mappingByTarget")!;
        var sourceKeys = (HashSet<string>)PrivateField(pair, "_mappedSourceKeys")!;
        start.Stop();

        Assert.Equal(8_000, index.Count);
        Assert.Equal(8_001, sourceKeys.Count);
        Assert.Equal("source-0", index["target-0"].SourceKey);
        Assert.True(start.Elapsed < TimeSpan.FromSeconds(4), start.Elapsed.ToString());

        pair.Mappings = [new OVMappingPairEntry("replacement", "new-source")];
        InvokePrivate(pair, "OnParametersSet");
        Assert.Single(index);
        Assert.False(index.ContainsKey("target-0"));
        Assert.Contains("new-source", sourceKeys);
    }

    [Fact]
    public void MappingJoin_nested_readonly_states_resolve_without_repeated_field_scans()
    {
        var nodes = Enumerable.Range(0, 5_000)
            .Select(i => new OVStructuredFieldNode($"source-{i}", $"Source {i}", $"$.source{i}",
                OVStructuredNodeKind.Primitive, OVStructuredDataType.String)).ToArray();
        var states = Enumerable.Range(0, 5_000).Select(i =>
            new OVMappingJoinFieldState($"source-{i}", ReadOnly: i % 2 == 0)).ToArray();
        var method = typeof(OVMappingJoin).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Single(m => m.Name == "ApplyFieldState" &&
                m.GetParameters()[0].ParameterType == typeof(IReadOnlyList<OVStructuredFieldNode>));
        var start = Stopwatch.StartNew();
        var result = (IReadOnlyList<OVStructuredFieldNode>)method.Invoke(null, [nodes, states])!;
        start.Stop();

        Assert.Equal(5_000, result.Count);
        Assert.True(result[0].Disabled);
        Assert.False(result[1].Disabled);
        Assert.True(result[4_998].Disabled);
        Assert.True(start.Elapsed < TimeSpan.FromSeconds(4), start.Elapsed.ToString());
    }

    [Fact]
    public async Task MappingPair_source_tree_readonly_blocks_focus_and_selection_callback_even_if_event_arrives()
    {
        var selected = 0;
        var focused = 0;
        var node = new OVStructuredFieldNode("field", "Field", "$.field",
            OVStructuredNodeKind.Primitive, OVStructuredDataType.String);
        var tree = new OVMappingPairSourceTree
        {
            ReadOnly = true,
            SelectedKeysChanged = EventCallback.Factory.Create<IReadOnlyCollection<string>>(this, _ => selected++),
            FocusedKeyChanged = EventCallback.Factory.Create<string?>(this, _ => focused++)
        };
        await (Task)InvokePrivate(tree, "OnSelectedNodeChanged", node)!;
        await (Task)InvokePrivate(tree, "FocusNode", node)!;
        Assert.Equal(0, selected);
        Assert.Equal(0, focused);
    }

    [Fact]
    public async Task Tree_readonly_blocks_selection_callback_from_a_stale_ui_event()
    {
        var selected = 0;
        var tree = new OVTree
        {
            ReadOnly = true,
            SelectedKeysChanged = EventCallback.Factory.Create<IReadOnlyCollection<string>>(this, _ => selected++)
        };
        await (Task)InvokePrivate(tree, "OnSelectedNodesChanged",
            (IReadOnlyCollection<OVTreeNode>)new[] { new OVTreeNode("one", "One") })!;
        Assert.Equal(0, selected);
    }

    [Fact]
    public async Task MultiSelect_disabled_selected_option_has_no_remove_button_or_removal_callback()
    {
        await using var context = NewContext();
        IReadOnlyCollection<string>? changed = null;
        var cut = context.Render<OVMultiSelect>(p => p
            .Add(x => x.Options, new[]
            {
                new OVSelectionOption("locked", "Locked", Disabled: true),
                new OVSelectionOption("free", "Free")
            })
            .Add(x => x.SelectedKeys, new[] { "locked", "free" })
            .Add(x => x.SelectedKeysChanged, keys => changed = keys));

        Assert.Empty(cut.FindAll("button[aria-label='Remove Locked']"));
        Assert.Single(cut.FindAll("button[aria-label='Remove Free']"));
        await (Task)InvokePrivate(cut.Instance, "Remove", "locked")!;
        Assert.Null(changed);
        await (Task)InvokePrivate(cut.Instance, "Remove", "free")!;
        Assert.Equal(new[] { "locked" }, changed);
    }

    [Fact]
    public async Task SelectionSummary_explicit_zero_visible_items_is_count_only()
    {
        await using var context = NewContext();
        var cut = context.Render<OVSelectionSummary>(p => p
            .Add(x => x.Items, new[] { "A", "B", "C" })
            .Add(x => x.MaxVisibleItems, 0));

        Assert.Contains("3 selected", cut.Markup);
        Assert.Contains("+3 more", cut.Markup);
        Assert.DoesNotContain(">A, B, C", cut.Markup);
    }

    [Fact]
    public async Task Pagination_action_disabled_state_matches_clamped_page_and_nonnegative_counts()
    {
        await using var context = NewContext();
        var cut = context.Render<OVPagination>(p => p
            .Add(x => x.Page, 100)
            .Add(x => x.TotalPages, 5)
            .Add(x => x.TotalCount, -1));
        Assert.Contains("Page 5 of 5", cut.Markup);
        Assert.Contains("0 items", cut.Markup);
        Assert.True(cut.Find("button[aria-label='Next page']").HasAttribute("disabled"));
        Assert.False(cut.Find("button[aria-label='Previous page']").HasAttribute("disabled"));
    }

    [Fact]
    public async Task ListPage_fatal_error_displaces_refreshing_and_refresh_failure_messages()
    {
        await using var context = NewContext();
        var cut = context.Render<OVListPage>(p => p
            .Add(x => x.Title, "Items")
            .Add(x => x.ErrorMessage, "Unable to load items")
            .Add(x => x.Refreshing, true)
            .Add(x => x.RefreshErrorMessage, "Refresh issue")
            .Add<RenderFragment>(x => x.ChildContent, b => b.AddContent(0, "Rows")));

        Assert.Contains("Unable to load items", cut.Markup);
        Assert.DoesNotContain("Refresh issue", cut.Markup);
        Assert.Empty(cut.FindAll(".ov-list-page__refreshing"));
        Assert.DoesNotContain("Rows", cut.Markup);
    }

    [Fact]
    public async Task FileUpload_duplicate_file_names_are_indexed_and_removed_by_identity()
    {
        await using var context = NewContext();
        var first = new StubFile("same.csv");
        var second = new StubFile("same.csv");
        var cut = context.Render<OVFileUpload>(p => p
            .Add(x => x.SelectedFiles, (IReadOnlyList<IBrowserFile>)new IBrowserFile[] { first, second }));
        var counts = (Dictionary<string, int>)PrivateField(cut.Instance, "_filenameCounts")!;
        Assert.Equal(2, counts["same.csv"]);
        Assert.Equal(2, cut.FindAll(".ov-file-upload__file").Count);
        Assert.NotEqual(
            (string)InvokePrivate(cut.Instance, "RemoveLabel", first, 0)!,
            (string)InvokePrivate(cut.Instance, "RemoveLabel", second, 1)!);
    }

    private static object? PrivateField(object target, string name) =>
        target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(target);

    private static object? InvokePrivate(object target, string name, params object?[] args) =>
        target.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(target, args);

    private sealed class StubFile(string name) : IBrowserFile
    {
        public string Name { get; } = name;
        public DateTimeOffset LastModified => DateTimeOffset.UtcNow;
        public long Size => 8;
        public string ContentType => "text/csv";
        public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
            => new MemoryStream(new byte[8]);
    }
}
