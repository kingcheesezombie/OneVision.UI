using System.Reflection;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class MappingRuleAndSelectionLifecycleQualityTests
{
    [Fact]
    public async Task Changing_mapping_field_clears_previous_field_comparison_value()
    {
        OVMappingRuleConditionChange? reported = null;
        var editor = new OVMappingRuleGroupEditor
        {
            FieldDefinitions = [
                new OVMappingRuleFieldDefinition("old", "Old field"),
                new OVMappingRuleFieldDefinition("new", "New field", OVMappingRuleFieldType.Number)
            ],
            ConditionChanged = EventCallback.Factory.Create<OVMappingRuleConditionChange>(this, value => reported = value)
        };
        var before = new OVMappingRuleCondition("condition", "old", OVMappingRuleOperatorContract.Equals, "previous literal");
        var method = typeof(OVMappingRuleGroupEditor).GetMethod("ChangeOperand", BindingFlags.Instance | BindingFlags.NonPublic)!;

        await (Task)method.Invoke(editor, [before, "new"])!;

        Assert.NotNull(reported);
        Assert.Equal("new", reported!.Condition.OperandKey);
        Assert.Null(reported.Condition.Value);
        Assert.Null(reported.Condition.ValidationMessage);
    }

    [Fact]
    public async Task Changed_rule_context_allows_new_test_while_old_test_is_in_flight_without_stale_completion()
    {
        var first = new TaskCompletionSource<OVMappingRuleTestResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        var second = new TaskCompletionSource<OVMappingRuleTestResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        var callCount = 0;
        var rule = new OVMappingRule
        {
            RootGroup = new OVMappingRuleGroup("root", OVMappingRuleJoin.And, []),
            TestContextKey = "first",
            TestRuleAsync = () => ++callCount == 1 ? first.Task : second.Task
        };
        var type = typeof(OVMappingRule);
        var set = type.GetMethod("OnParametersSet", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var run = type.GetMethod("RunTest", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var isRunning = type.GetField("_testRunning", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var testError = type.GetField("_testError", BindingFlags.Instance | BindingFlags.NonPublic)!;
        set.Invoke(rule, null);
        var oldTask = (Task)run.Invoke(rule, [new MouseEventArgs()])!;
        Assert.True((bool)isRunning.GetValue(rule)!);

        rule.TestContextKey = "second";
        set.Invoke(rule, null);
        var newTask = (Task)run.Invoke(rule, [new MouseEventArgs()])!;
        Assert.Equal(2, callCount);
        Assert.True((bool)isRunning.GetValue(rule)!);

        first.SetException(new InvalidOperationException("Old request failed"));
        await oldTask;
        Assert.True((bool)isRunning.GetValue(rule)!);
        Assert.Null(testError.GetValue(rule));

        second.SetException(new InvalidOperationException("New request failed"));
        await newTask;
        Assert.False((bool)isRunning.GetValue(rule)!);
        Assert.Equal("Rule evaluation could not be completed.", testError.GetValue(rule));
    }

    [Fact]
    public async Task Disabled_selection_list_item_does_not_emit_a_change()
    {
        var changes = 0;
        var control = new OVSelectionList
        {
            Items = [new OVListItem("disabled", "Disabled", Disabled: true)],
            SelectedKeysChanged = EventCallback.Factory.Create<IReadOnlyCollection<string>>(this, _ => changes++)
        };
        var method = typeof(OVSelectionList).GetMethod("Toggle", BindingFlags.Instance | BindingFlags.NonPublic)!;
        await (Task)method.Invoke(control, [control.Items[0]])!;
        Assert.Equal(0, changes);
    }

    [Fact]
    public async Task DataGrid_fatal_error_takes_precedence_over_loading_empty_and_does_not_offer_inert_retry()
    {
        await using var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        var cut = context.Render<OVDataGrid>(p => p
            .Add(x => x.ErrorMessage, "Unable to load rows")
            .Add(x => x.Loading, true)
            .Add(x => x.Empty, true)
            .Add<RenderFragment>(x => x.ChildContent, b => b.AddContent(0, "Rows")));

        Assert.Contains("Unable to load rows", cut.Markup);
        Assert.Empty(cut.FindAll(".ov-data-grid-scroll"));
        Assert.DoesNotContain("Rows", cut.Markup);
        Assert.DoesNotContain("Retry", cut.Markup);
    }

    [Fact]
    public async Task Disabled_list_row_preserves_label_but_is_not_activatable()
    {
        await using var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        var cut = context.Render<OVList>(p => p
            .Add(x => x.Items, new[] {
                new OVListItem("disabled", "Disabled row", Disabled: true),
                new OVListItem("enabled", "Enabled row")
            })
            .Add(x => x.ItemActivated, _ => { }));

        Assert.Contains("Disabled row", cut.Markup);
        Assert.Single(cut.FindAll(".ov-list__body[disabled]"));
        Assert.Single(cut.FindAll("button.ov-list__body:not([disabled])"));
    }

    [Fact]
    public async Task SearchBox_keeps_accessible_name_and_readonly_visibility()
    {
        await using var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        var cut = context.Render<OVSearchBox>(p => p
            .Add(x => x.Value, "existing search")
            .Add(x => x.AriaLabel, "Search mapped fields")
            .Add(x => x.ReadOnly, true));

        Assert.Contains("Search mapped fields", cut.Markup);
        Assert.Contains("existing search", cut.Markup);
        Assert.True(cut.Instance.ReadOnly);
    }
}
