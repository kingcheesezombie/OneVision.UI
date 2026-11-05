using System.Reflection;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class Level0FollowUpContractTests
{
    [Fact]
    public async Task Data_grid_initial_error_replaces_rows_and_retries_through_application_callback()
    {
        await using var context = Context();
        var retried = false;
        var cut = context.Render<OVDataGrid>(parameters => parameters
            .Add(x => x.ChildContent, (RenderFragment)(b => b.AddMarkupContent(0, "<tbody><tr><td>Existing row</td></tr></tbody>")))
            .Add(x => x.ErrorMessage, "Could not load records.")
            .Add(x => x.RetryRequested, () => { retried = true; }));

        Assert.Contains("Could not load records.", cut.Markup, StringComparison.Ordinal);
        Assert.Empty(cut.FindAll("table"));
        await cut.Find("button").ClickAsync();
        Assert.True(retried);
    }

    [Fact]
    public async Task Multi_select_preserves_hidden_choices_when_visible_selection_changes()
    {
        var select = new OVMultiSelect
        {
            Options =
            [
                new("north", "North"),
                new("south", "South"),
                new("east", "East", Disabled: true)
            ],
            SelectedKeys = ["north", "south", "east"]
        };
        IReadOnlyCollection<string>? changed = null;
        select.SelectedKeysChanged = EventCallback.Factory.Create<IReadOnlyCollection<string>>(
            new object(), (IReadOnlyCollection<string> values) => changed = values);

        var search = typeof(OVMultiSelect).GetField("_search", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(search);
        search!.SetValue(select, "north");

        var selectChanged = typeof(OVMultiSelect).GetMethod("OnSelectedValuesChanged", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(selectChanged);
        var task = Assert.IsAssignableFrom<Task>(selectChanged!.Invoke(select, [new string[] { "north" }]));
        await task;

        Assert.NotNull(changed);
        Assert.Contains("north", changed!);
        Assert.Contains("south", changed!);
        Assert.Contains("east", changed!);
    }

    [Fact]
    public async Task Multi_select_read_only_rejects_changes_and_has_group_name()
    {
        var select = new OVMultiSelect { ReadOnly = true, Options = [new("north", "North")], SelectedKeys = ["north"] };
        var invoked = false;
        select.SelectedKeysChanged = EventCallback.Factory.Create<IReadOnlyCollection<string>>(
            new object(), (IReadOnlyCollection<string> _) => invoked = true);

        var method = typeof(OVMultiSelect).GetMethod("OnSelectedValuesChanged", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(method);
        await Assert.IsAssignableFrom<Task>(method!.Invoke(select, [new string[] { }]));
        Assert.False(invoked);
        Assert.Equal("Select values", select.AriaLabel);
    }

    [Fact]
    public async Task Selection_summary_announces_current_count_without_announcing_action_buttons()
    {
        await using var context = Context();
        var cut = context.Render<OVSelectionSummary>(parameters => parameters
            .Add(x => x.Items, new[] { "North", "South" }));
        var count = cut.Find("[role=status]");
        Assert.Equal("2 selected", count.TextContent.Trim());
        Assert.Equal("polite", count.GetAttribute("aria-live"));
    }

    [Theory]
    [InlineData(-20, 0)]
    [InlineData(150, 100)]
    [InlineData(double.PositiveInfinity, 0)]
    public void Progress_normalises_invalid_percentages(double value, double expected)
    {
        var progress = new OVProgress { Value = value };
        var safeValue = typeof(OVProgress).GetProperty("SafeValue", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(safeValue);
        Assert.Equal(expected, Assert.IsType<double>(safeValue!.GetValue(progress)));
    }

    private static BunitContext Context()
    {
        var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }
}
