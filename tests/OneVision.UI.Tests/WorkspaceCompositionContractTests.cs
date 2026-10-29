using System.Reflection;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class WorkspaceCompositionContractTests
{
    [Fact]
    public void List_page_exposes_context_summary_region_for_collection_state()
    {
        var parameters = ParametersOf<OVListPage>();
        foreach (var required in new[]
        {
            nameof(OVListPage.Actions), nameof(OVListPage.Summary), nameof(OVListPage.Toolbar),
            nameof(OVListPage.ToolbarActions), nameof(OVListPage.Pagination), nameof(OVListPage.ChildContent)
        })
            Assert.Contains(required, parameters);
    }

    [Fact]
    public void Detail_page_exposes_full_width_workspace_regions_states_and_sticky_footer()
    {
        var parameters = ParametersOf<OVDetailPage>();
        foreach (var required in new[]
        {
            nameof(OVDetailPage.BackAction), nameof(OVDetailPage.Status), nameof(OVDetailPage.Summary), nameof(OVDetailPage.Actions),
            nameof(OVDetailPage.Loading), nameof(OVDetailPage.LoadingContent), nameof(OVDetailPage.ErrorMessage), nameof(OVDetailPage.Empty),
            nameof(OVDetailPage.Busy), nameof(OVDetailPage.StickyFooter), nameof(OVDetailPage.FooterContent), nameof(OVDetailPage.FooterActions),
            nameof(OVDetailPage.ChildContent)
        })
            Assert.Contains(required, parameters);
    }

    [Fact]
    public void Card_exposes_selectable_disabled_busy_and_governed_action_regions()
    {
        var parameters = ParametersOf<OVCard>();
        foreach (var required in new[]
        {
            nameof(OVCard.Selected), nameof(OVCard.Disabled), nameof(OVCard.Busy), nameof(OVCard.Actionable), nameof(OVCard.SelectedAction),
            nameof(OVCard.AriaLabel), nameof(OVCard.Header), nameof(OVCard.Status), nameof(OVCard.Metadata), nameof(OVCard.PrimaryAction),
            nameof(OVCard.SecondaryActions), nameof(OVCard.OverflowActions), nameof(OVCard.Actions), nameof(OVCard.ChildContent)
        })
            Assert.Contains(required, parameters);
    }

    [Fact]
    public async Task Card_keyboard_activation_works_and_disabled_or_busy_suppresses_selection()
    {
        await using var context = new BunitContext();
        var activations = 0;
        var cut = context.Render<OVCard>(parameters => parameters
            .Add(component => component.AriaLabel, "Select record")
            .Add(component => component.SelectedAction, () => activations++)
            .Add(component => component.ChildContent, TextFragment("Record")));

        await cut.Find(".ov-card__selection-target").KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });
        await cut.Find(".ov-card__selection-target").KeyDownAsync(new KeyboardEventArgs { Key = " " });
        Assert.Equal(2, activations);

        cut.Render(parameters => parameters
            .Add(component => component.Disabled, true)
            .Add(component => component.AriaLabel, "Select record")
            .Add(component => component.SelectedAction, () => activations++)
            .Add(component => component.ChildContent, TextFragment("Record")));
        await cut.Find(".ov-card__selection-target").ClickAsync();
        Assert.Equal(2, activations);

        cut.Render(parameters => parameters
            .Add(component => component.Busy, true)
            .Add(component => component.AriaLabel, "Select record")
            .Add(component => component.SelectedAction, () => activations++)
            .Add(component => component.ChildContent, TextFragment("Record")));
        await cut.Find(".ov-card__selection-target").ClickAsync();
        Assert.Equal(2, activations);
    }

    [Fact]
    public async Task Card_child_action_is_separate_from_selection_target_and_disabled_actions_are_inert()
    {
        await using var context = new BunitContext();
        var selections = 0;
        var actions = 0;
        var secondary = ButtonFragment("Inspect", () => actions++);
        var cut = context.Render<OVCard>(parameters => parameters
            .Add(component => component.SelectedAction, () => selections++)
            .Add(component => component.ChildContent, TextFragment("Record"))
            .Add(component => component.SecondaryActions, secondary));

        Assert.Empty(cut.FindAll(".ov-card__selection-target button"));
        await cut.Find(".ov-card__secondary-actions button").ClickAsync();
        Assert.Equal(0, selections);
        Assert.Equal(1, actions);

        cut.Render(parameters => parameters
            .Add(component => component.Disabled, true)
            .Add(component => component.SelectedAction, () => selections++)
            .Add(component => component.ChildContent, TextFragment("Record"))
            .Add(component => component.SecondaryActions, secondary));
        Assert.NotNull(cut.Find(".ov-card__actions").GetAttribute("inert"));
    }

    [Fact]
    public void Context_summary_exposes_compact_items_process_and_governed_content_states()
    {
        var parameters = ParametersOf<OVContextSummary>();
        foreach (var required in new[]
        {
            nameof(OVContextSummary.Items), nameof(OVContextSummary.Process), nameof(OVContextSummary.Loading),
            nameof(OVContextSummary.ErrorMessage), nameof(OVContextSummary.EmptyTitle), nameof(OVContextSummary.EmptyDescription)
        })
            Assert.Contains(required, parameters);

        var item = new OVContextSummaryItem("Status", "Active", Tone: OVPillTone.Success);
        Assert.Equal(OVPillTone.Success, item.Tone);
    }

    [Fact]
    public void Process_status_uses_semantic_state_display_modes_and_bounded_progress()
    {
        var parameters = ParametersOf<OVProcessStatus>();
        foreach (var required in new[]
        {
            nameof(OVProcessStatus.State), nameof(OVProcessStatus.Display), nameof(OVProcessStatus.Label),
            nameof(OVProcessStatus.ProgressCurrent), nameof(OVProcessStatus.ProgressTotal), nameof(OVProcessStatus.StartedAt),
            nameof(OVProcessStatus.CompletedAt), nameof(OVProcessStatus.LastRunAt), nameof(OVProcessStatus.Phase),
            nameof(OVProcessStatus.Message), nameof(OVProcessStatus.Loading), nameof(OVProcessStatus.ErrorMessage)
        })
            Assert.Contains(required, parameters);

        Assert.Equal(new[] { "Queued", "Running", "Completed", "Failed", "Stale" }, Enum.GetNames<OVProcessState>());
        Assert.Equal(new[] { "Compact", "Detailed" }, Enum.GetNames<OVProcessStatusDisplay>());
        Assert.Equal(0d, OVProcessStatusSemantics.CalculateProgressPercent(-5, 100));
        Assert.Equal(47d, OVProcessStatusSemantics.CalculateProgressPercent(47, 100));
        Assert.Equal(100d, OVProcessStatusSemantics.CalculateProgressPercent(125, 100));
        Assert.Equal(0d, OVProcessStatusSemantics.CalculateProgressPercent(10, 0));
    }

    [Fact]
    public void Card_collection_exposes_loading_error_empty_and_content_contract()
    {
        var parameters = ParametersOf<OVCardCollection>();
        foreach (var required in new[]
        {
            nameof(OVCardCollection.Loading), nameof(OVCardCollection.ErrorMessage), nameof(OVCardCollection.Empty),
            nameof(OVCardCollection.EmptyTitle), nameof(OVCardCollection.EmptyDescription), nameof(OVCardCollection.ChildContent)
        })
            Assert.Contains(required, parameters);
    }

    private static RenderFragment TextFragment(string text) => builder => builder.AddContent(0, text);

    private static RenderFragment ButtonFragment(string text, Action action) => builder =>
    {
        builder.OpenElement(0, "button");
        builder.AddAttribute(1, "type", "button");
        builder.AddAttribute(2, "onclick", EventCallback.Factory.Create<MouseEventArgs>(new object(), _ => action()));
        builder.AddContent(3, text);
        builder.CloseElement();
    };

    private static HashSet<string> ParametersOf<T>()
        => typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);
}
