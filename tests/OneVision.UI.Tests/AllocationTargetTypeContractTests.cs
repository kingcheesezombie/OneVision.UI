using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class AllocationTargetTypeContractTests
{
    [Theory]
    [InlineData(OVAllocationTargetPlatform.Eam)]
    [InlineData(OVAllocationTargetPlatform.Idm)]
    [InlineData(OVAllocationTargetPlatform.Odm)]
    public async Task Target_type_fragment_is_provider_independent(OVAllocationTargetPlatform platform)
    {
        await using var context = Context();
        var cut = context.Render<OVAllocation>(p => p
            .Add(x => x.FindTarget, true)
            .Add(x => x.TargetPlatform, platform)
            .Add(x => x.AvailableTargetPlatforms, new[] { platform })
            .Add(x => x.TargetType, Text("Provider-owned Target Type"))
            .Add(x => x.TargetSearch, Text("Find target"))
            .Add(x => x.SourceSearch, Text("Find source"))
            .Add(x => x.SourceResults, Text("Source results")));

        Assert.Single(cut.FindAll(".ov-allocation__target-type"));
        Assert.Contains("Provider-owned Target Type", cut.Markup);
        Assert.Empty(cut.FindAll(".ov-allocation__platform"));
    }

    [Fact]
    public async Task Target_type_is_omitted_when_unsupplied_even_with_multiple_providers()
    {
        await using var context = Context();
        var cut = context.Render<OVAllocation>(p => p
            .Add(x => x.FindTarget, true)
            .Add(x => x.TargetSearch, Text("Find target"))
            .Add(x => x.SourceSearch, Text("Find source"))
            .Add(x => x.SourceResults, Text("Source results")));

        Assert.Empty(cut.FindAll(".ov-allocation__target-type"));
        Assert.Equal(3, cut.FindAll(".ov-allocation__platform").Count);
    }

    [Fact]
    public async Task Controlled_provider_change_allows_consumers_to_supply_a_different_type_fragment()
    {
        await using var context = Context();
        OVAllocationTargetPlatform? requested = null;
        var cut = context.Render<OVAllocation>(p => p
            .Add(x => x.FindTarget, true)
            .Add(x => x.TargetPlatform, OVAllocationTargetPlatform.Eam)
            .Add(x => x.TargetPlatformChanged, platform => requested = platform)
            .Add(x => x.TargetType, Text("Organisation Unit"))
            .Add(x => x.SourceSearch, Text("Find source"))
            .Add(x => x.SourceResults, Text("Source results")));

        cut.FindAll(".ov-allocation__platform").Single(button => button.TextContent.Trim() == "IDM").Click();
        Assert.Equal(OVAllocationTargetPlatform.Idm, requested);

        // OVAllocation is controlled. The caller changes platform and the
        // dependent fragment together after clearing its type/target state.
        cut.Render(p => p
            .Add(x => x.FindTarget, true)
            .Add(x => x.TargetPlatform, OVAllocationTargetPlatform.Idm)
            .Add(x => x.TargetType, Text("PERSON or COMPETENCY_ITEM"))
            .Add(x => x.SourceSearch, Text("Find source"))
            .Add(x => x.SourceResults, Text("Source results")));

        Assert.Contains("PERSON or COMPETENCY_ITEM", cut.Markup);
        Assert.DoesNotContain("Organisation Unit", cut.Markup);
        Assert.Single(cut.FindAll(".ov-allocation__target-type"));
    }

    [Fact]
    public async Task Known_and_selected_targets_only_show_supplied_identity_not_selectors()
    {
        await using var context = Context();

        var known = context.Render<OVAllocation>(p => p
            .Add(x => x.FindTarget, false)
            .Add(x => x.Target, Text("EAM · Organisation Unit · ENT-001"))
            .Add(x => x.TargetType, Text("Hidden type editor"))
            .Add(x => x.SourceSearch, Text("Find source"))
            .Add(x => x.SourceResults, Text("Source results")));

        Assert.Contains("EAM · Organisation Unit · ENT-001", known.Markup);
        Assert.Empty(known.FindAll(".ov-allocation__target-type"));
        Assert.Empty(known.FindAll(".ov-allocation__platform"));

        var selected = context.Render<OVAllocation>(p => p
            .Add(x => x.FindTarget, true)
            .Add(x => x.TargetSelected, true)
            .Add(x => x.SelectedTarget, Text("IDM · PERSON · P-001"))
            .Add(x => x.TargetType, Text("Hidden type editor"))
            .Add(x => x.ChangeTargetRequested, () => { })
            .Add(x => x.SourceSearch, Text("Find source"))
            .Add(x => x.SourceResults, Text("Source results")));

        Assert.Contains("IDM · PERSON · P-001", selected.Markup);
        Assert.Empty(selected.FindAll(".ov-allocation__target-type"));
        Assert.Empty(selected.FindAll(".ov-allocation__platform"));
        Assert.Single(selected.FindAll(".ov-allocation__change"));
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task Read_only_or_submitting_blocks_provider_change_and_change_target(bool readOnly, bool submitting)
    {
        await using var context = Context();
        var requested = false;
        var cut = context.Render<OVAllocation>(p => p
            .Add(x => x.FindTarget, true)
            .Add(x => x.ReadOnly, readOnly)
            .Add(x => x.Submitting, submitting)
            .Add(x => x.TargetType, Text("Type remains visible"))
            .Add(x => x.TargetPlatformChanged, _ => requested = true)
            .Add(x => x.SourceSearch, Text("Find source"))
            .Add(x => x.SourceResults, Text("Source results")));

        Assert.All(cut.FindAll(".ov-allocation__platform"),
            button => Assert.NotNull(button.GetAttribute("disabled")));
        Assert.Contains("Type remains visible", cut.Markup);
        Assert.False(requested);
    }

    [Fact]
    public async Task Finder_preserves_source_selection_date_and_submission_regions()
    {
        await using var context = Context();
        var cut = context.Render<OVAllocation>(p => p
            .Add(x => x.FindTarget, true)
            .Add(x => x.TargetPlatform, OVAllocationTargetPlatform.Odm)
            .Add(x => x.TargetType, Text("Application types"))
            .Add(x => x.SourceSearch, Text("Source search"))
            .Add(x => x.SourceResults, Text("Source result"))
            .Add(x => x.AlwaysBelonged, false)
            .Add(x => x.FromDate, new DateTime(2026, 11, 1))
            .Add(x => x.SubmissionErrorMessage, "Try allocation again."));

        Assert.Contains("Source search", cut.Markup);
        Assert.Contains("Source result", cut.Markup);
        Assert.Equal(2, cut.FindAll(".ov-allocation__dates .mud-input-control").Count);
        Assert.Contains("Try allocation again.", cut.Markup);
    }

    private static RenderFragment Text(string value) => builder => builder.AddContent(0, value);

    private static BunitContext Context()
    {
        var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }
}
