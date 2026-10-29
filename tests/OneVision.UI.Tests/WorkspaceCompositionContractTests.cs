using System.Reflection;
using Microsoft.AspNetCore.Components;
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
            nameof(OVListPage.Actions),
            nameof(OVListPage.Summary),
            nameof(OVListPage.Toolbar),
            nameof(OVListPage.ToolbarActions),
            nameof(OVListPage.Pagination),
            nameof(OVListPage.ChildContent)
        })
            Assert.Contains(required, parameters);
    }

    [Fact]
    public void Detail_page_exposes_full_width_workspace_regions_and_states()
    {
        var parameters = ParametersOf<OVDetailPage>();
        foreach (var required in new[]
        {
            nameof(OVDetailPage.BackAction),
            nameof(OVDetailPage.Status),
            nameof(OVDetailPage.Summary),
            nameof(OVDetailPage.Actions),
            nameof(OVDetailPage.Loading),
            nameof(OVDetailPage.ErrorMessage),
            nameof(OVDetailPage.Empty),
            nameof(OVDetailPage.Busy),
            nameof(OVDetailPage.FooterContent),
            nameof(OVDetailPage.FooterActions),
            nameof(OVDetailPage.ChildContent)
        })
            Assert.Contains(required, parameters);
    }

    [Fact]
    public void Card_exposes_selectable_disabled_busy_and_semantic_information_regions()
    {
        var parameters = ParametersOf<OVCard>();
        foreach (var required in new[]
        {
            nameof(OVCard.Selected),
            nameof(OVCard.Disabled),
            nameof(OVCard.Busy),
            nameof(OVCard.Actionable),
            nameof(OVCard.SelectedAction),
            nameof(OVCard.AriaLabel),
            nameof(OVCard.Header),
            nameof(OVCard.Status),
            nameof(OVCard.Metadata),
            nameof(OVCard.PrimaryAction),
            nameof(OVCard.SecondaryActions),
            nameof(OVCard.Actions),
            nameof(OVCard.ChildContent)
        })
            Assert.Contains(required, parameters);
    }

    [Fact]
    public void Context_summary_exposes_compact_items_process_and_governed_content_states()
    {
        var parameters = ParametersOf<OVContextSummary>();
        foreach (var required in new[]
        {
            nameof(OVContextSummary.Items),
            nameof(OVContextSummary.Process),
            nameof(OVContextSummary.Loading),
            nameof(OVContextSummary.ErrorMessage),
            nameof(OVContextSummary.EmptyTitle),
            nameof(OVContextSummary.EmptyDescription)
        })
            Assert.Contains(required, parameters);

        var item = new OVContextSummaryItem("Status", "Active", Tone: OVPillTone.Success);
        Assert.Equal(OVPillTone.Success, item.Tone);
    }

    [Fact]
    public void Process_status_uses_semantic_application_owned_state_and_progress_inputs()
    {
        var parameters = ParametersOf<OVProcessStatus>();
        foreach (var required in new[]
        {
            nameof(OVProcessStatus.State),
            nameof(OVProcessStatus.Label),
            nameof(OVProcessStatus.ProgressCurrent),
            nameof(OVProcessStatus.ProgressTotal),
            nameof(OVProcessStatus.StartedAt),
            nameof(OVProcessStatus.CompletedAt),
            nameof(OVProcessStatus.LastRunAt),
            nameof(OVProcessStatus.Phase),
            nameof(OVProcessStatus.Message),
            nameof(OVProcessStatus.Loading),
            nameof(OVProcessStatus.ErrorMessage)
        })
            Assert.Contains(required, parameters);

        Assert.Equal(new[] { "Queued", "Running", "Completed", "Failed", "Stale" }, Enum.GetNames<OVProcessState>());
    }

    [Fact]
    public void Card_collection_exposes_loading_error_empty_and_content_contract()
    {
        var parameters = ParametersOf<OVCardCollection>();
        foreach (var required in new[]
        {
            nameof(OVCardCollection.Loading),
            nameof(OVCardCollection.ErrorMessage),
            nameof(OVCardCollection.Empty),
            nameof(OVCardCollection.EmptyTitle),
            nameof(OVCardCollection.EmptyDescription),
            nameof(OVCardCollection.ChildContent)
        })
            Assert.Contains(required, parameters);
    }

    private static HashSet<string> ParametersOf<T>()
        => typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);
}
