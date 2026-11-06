using System.Diagnostics;
using System.Reflection;
using Bunit;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class ScheduleBuilderLookupQualityTests
{
    private static BunitContext Context()
    {
        var c = new BunitContext();
        c.Services.AddMudServices();
        c.JSInterop.Mode = JSRuntimeMode.Loose;
        return c;
    }

    [Fact]
    public async Task Committed_items_keep_supplied_order_and_first_matching_label()
    {
        await using var context = Context();
        var value = new OVScheduleBuilderValue
        {
            CommittedItemKeys = ["second", "first", "missing"]
        };
        var items = new[]
        {
            new OVScheduleItem("first", "First label"),
            new OVScheduleItem("second", "Second label"),
            new OVScheduleItem("first", "Conflicting duplicate")
        };
        var cut = context.Render<OVScheduleBuilder>(p => p
            .Add(x => x.ShowActions, false)
            .Add(x => x.Value, value)
            .Add(x => x.AvailableItems, items));

        var committed = cut.FindAll(".ov-schedule-builder__committed-row")
            .Select(x => x.TextContent).ToArray();
        Assert.Equal(3, committed.Length);
        Assert.Contains("Second label", committed[0]);
        Assert.Contains("First label", committed[1]);
        Assert.DoesNotContain("Conflicting duplicate", string.Join(" ", committed));
        Assert.Contains("missing", committed[2]);
        Assert.DoesNotContain("First label", cut.Find(".ov-schedule-builder__pane").TextContent);
    }

    [Fact]
    public async Task Changed_committed_and_available_parameters_update_both_lists()
    {
        await using var context = Context();
        var items = new[]
        {
            new OVScheduleItem("one", "One"), new OVScheduleItem("two", "Two"),
            new OVScheduleItem("three", "Three")
        };
        var cut = context.Render<OVScheduleBuilder>(p => p
            .Add(x => x.Value, new OVScheduleBuilderValue { CommittedItemKeys = ["one"] })
            .Add(x => x.AvailableItems, items));

        Assert.Single(cut.FindAll(".ov-schedule-builder__committed-row"));
        cut.Render(p => p
            .Add(x => x.Value, new OVScheduleBuilderValue { CommittedItemKeys = ["two", "three"] })
            .Add(x => x.AvailableItems, items));
        Assert.Equal(2, cut.FindAll(".ov-schedule-builder__committed-row").Count);
        Assert.Contains("Search list (1)", cut.Markup);
    }


    [Fact]
    public async Task Removing_an_available_item_prunes_an_obsolete_pending_checkbox_selection()
    {
        await using var context = Context();
        var originalItems = new[]
        {
            new OVScheduleItem("available-a", "Available A"),
            new OVScheduleItem("available-b", "Available B")
        };
        var cut = context.Render<OVScheduleBuilder>(p => p
            .Add(x => x.Value, new OVScheduleBuilderValue())
            .Add(x => x.AvailableItems, originalItems));

        cut.Find(".ov-schedule-builder__pane input[type='checkbox']").Change(true);
        Assert.False(cut.Find(".ov-schedule-builder__transfer-actions button").HasAttribute("disabled"));

        cut.Render(p => p
            .Add(x => x.Value, new OVScheduleBuilderValue())
            .Add(x => x.AvailableItems, new[] { originalItems[1] }));

        Assert.True(cut.Find(".ov-schedule-builder__transfer-actions button").HasAttribute("disabled"));
        Assert.Contains("Search list (1)", cut.Markup);
    }

    [Fact]
    public async Task Large_external_item_catalogue_and_committed_keys_resolve_in_linear_time()
    {
        await using var context = Context();
        var items = Enumerable.Range(0, 20_000)
            .Select(i => new OVScheduleItem($"item-{i}", $"Item {i}")).ToArray();
        var keys = Enumerable.Range(0, 10_000).Select(i => $"item-{i}").ToArray();
        var stopwatch = Stopwatch.StartNew();
        var cut = context.Render<OVScheduleBuilder>(p => p
            .Add(x => x.Value, new OVScheduleBuilderValue { CommittedItemKeys = keys })
            .Add(x => x.AvailableItems, items)
            .Add(x => x.ShowItems, false));
        var committed = (IReadOnlyList<OVScheduleItem>)typeof(OVScheduleBuilder)
            .GetProperty("CommittedItems", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(cut.Instance)!;
        var available = (IReadOnlyList<OVScheduleItem>)typeof(OVScheduleBuilder)
            .GetProperty("FilteredAvailableItems", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(cut.Instance)!;
        stopwatch.Stop();

        Assert.Equal(10_000, committed.Count);
        Assert.Equal(10_000, available.Count);
        Assert.Equal("Item 0", committed[0].Label);
        Assert.Equal("Item 10000", available[0].Label);
        // A generously bounded regression guard for O(n*m) accidental reintroduction.
        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(4),
            $"Schedule lookup took {stopwatch.Elapsed} for a 20k/10k catalogue.");
    }
}
