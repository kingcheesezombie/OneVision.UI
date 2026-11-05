using System.Reflection;
using Microsoft.AspNetCore.Components;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class GovernedQualityLevelsContractTests
{
    [Fact]
    public void Hierarchy_and_mapping_support_non_destructive_refresh_states()
    {
        Require<OVTree>("Loading", "ErrorMessage", "Refreshing", "RefreshErrorMessage", "SelectedKeys", "SelectedKeysChanged", "ReadOnly");
        Require<OVMappingPair>("Loading", "Refreshing", "RefreshErrorMessage", "Mappings", "ReadOnly");
        Require<OVMappingJoin>("Loading", "Refreshing", "RefreshErrorMessage", "Joins", "ReadOnly");
        Assert.False(new OVTree().Refreshing);
        Assert.False(new OVMappingJoin().Refreshing);
    }

    [Fact]
    public void Schedule_and_connection_editors_keep_business_state_app_owned()
    {
        Require<OVScheduleBuilder>("Busy", "SaveErrorMessage", "SavedMessage",
            "EmbeddedInOverlay", "ShowItems", "ShowActions", "ReadOnly", "ValueChanged", "SaveRequested");
        Require<OVConnectionEditor>("Busy", "SaveErrorMessage", "SavedMessage",
            "EmbeddedInOverlay", "ShowTestAction", "ShowActions", "TestRequested", "SaveRequested", "ReadOnly");
        var schedule = new OVScheduleBuilder();
        var connection = new OVConnectionEditor();
        Assert.True(schedule.ShowItems);
        Assert.True(schedule.ShowActions);
        Assert.True(connection.ShowTestAction);
        Assert.True(connection.ShowActions);
        Assert.False(schedule.EmbeddedInOverlay);
        Assert.False(connection.EmbeddedInOverlay);
        Assert.Null(connection.SaveErrorMessage);
    }

    [Fact]
    public void Workspace_and_operational_detail_retain_content_during_refresh()
    {
        Require<OVDetailPage>("Refreshing", "RefreshErrorMessage", "Busy", "Loading", "ChildContent");
        Require<OVBuilderPage>("Busy", "Loading", "Refreshing", "RefreshErrorMessage",
            "Navigation", "Inspector", "ChildContent");
        Require<OVOperationalDetail>("Busy", "Refreshing", "RefreshErrorMessage",
            "ActionErrorMessage", "LatestEvent", "RecentActivity");
        Assert.False(new OVDetailPage().Refreshing);
        Assert.Null(new OVOperationalDetail().ActionErrorMessage);
    }

    [Fact]
    public void Data_grids_and_pagination_support_server_request_lifecycle()
    {
        Require<OVDataGrid>("Refreshing", "RefreshErrorMessage", "Loading", "Empty");
        Require<OVPagination>("Busy", "Page", "TotalPages", "PageChanged", "PageSize");
        Assert.False(new OVPagination().Busy);
        Assert.False(new OVDataGrid().Refreshing);
    }

    [Fact]
    public void Previously_reviewed_patterns_preserve_public_composition_contracts()
    {
        Require<OVAllocation>("EmbeddedInOverlay", "SourceCriteria", "SourceSelected", "SearchLoading", "SubmissionErrorMessage");
        Require<OVSourceConfiguration>("DisplayValueBuilderEnabled", "EmbeddedInOverlay", "MappingRequiresCurrentProfile", "SaveErrorMessage");
        Require<OVProgressiveConfigurationBuilder>("Busy", "EmbeddedInOverlay", "Completed", "ErrorMessage");
        Require<OVCatalogueManager>("Refreshing", "RetryRequested", "RefreshErrorMessage");
        Require<OVApiCallBuilder>("EmbeddedInOverlay", "SaveErrorMessage", "TestRequested");
        Require<OVEntityInspector>("Busy", "ActionErrorMessage", "Refreshing");
        Require<OVListPage>("Refreshing", "RefreshErrorMessage", "RetryRequested");
    }

    private static void Require<T>(params string[] names)
    {
        var found = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(p => p.Name).ToHashSet(StringComparer.Ordinal);
        Assert.All(names, key => Assert.Contains(key, found));
    }
}
