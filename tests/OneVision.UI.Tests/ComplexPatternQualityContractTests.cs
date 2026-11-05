using System.Reflection;
using Microsoft.AspNetCore.Components;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class ComplexPatternQualityContractTests
{
    [Fact]
    public void List_page_exposes_non_destructive_loading_and_retry_states()
    {
        Require<OVListPage>("Loading", "LoadingLabel", "Refreshing", "RefreshingLabel",
            "ErrorMessage", "RefreshErrorMessage", "RetryRequested",
            "Empty", "EmptyTitle", "EmptyDescription", "ResultsLabel");
        var page = new OVListPage();
        Assert.False(page.Loading);
        Assert.False(page.Refreshing);
        Assert.False(page.Empty);
        Assert.Null(page.ErrorMessage);
    }

    [Fact]
    public void Source_configuration_exposes_optional_readiness_and_overlay_composition()
    {
        Require<OVSourceConfiguration>("EmbeddedInOverlay", "MappingRequiresCurrentProfile",
            "SaveErrorMessage", "DisplayValueBuilderEnabled", "Saving");
        var source = new OVSourceConfiguration();
        Assert.False(source.MappingRequiresCurrentProfile);
        Assert.False(source.EmbeddedInOverlay);
        Assert.Null(source.SaveErrorMessage);
    }

    [Fact]
    public void Progressive_configuration_exposes_retryable_submission_states_without_owning_dependencies()
    {
        Require<OVProgressiveConfigurationBuilder>("EmbeddedInOverlay", "Busy", "BusyMessage",
            "ErrorMessage", "Completed", "CompletedMessage", "ReadOnly");
        var builder = new OVProgressiveConfigurationBuilder();
        Assert.False(builder.Busy);
        Assert.False(builder.Completed);
        Assert.False(builder.EmbeddedInOverlay);
        Assert.False(builder.ReadOnly);
        Assert.Null(builder.ErrorMessage);
    }

    [Fact]
    public void Catalogue_manager_and_editor_retain_records_or_inputs_on_recoverable_failures()
    {
        Require<OVCatalogueManager>("Refreshing", "RefreshErrorMessage", "RetryRequested",
            "Loading", "ReadOnly", "GridContent", "PageChanged");
        Require<OVCatalogueEditor>("SaveErrorMessage", "IsBusy", "IsDirty",
            "SaveRequested", "ChildContent");
        Assert.False(new OVCatalogueManager().Refreshing);
        Assert.Null(new OVCatalogueEditor().SaveErrorMessage);
    }

    [Fact]
    public void Api_builder_preserves_existing_semantic_fields_with_overlay_and_save_feedback()
    {
        Require<OVApiCallBuilder>("EmbeddedInOverlay", "BusyLabel", "SaveErrorMessage",
            "SavedMessage", "GuidedState", "ParameterDefinitions", "SaveRequested",
            "ConfigurationChanged", "ReadOnly");
        Assert.False(new OVApiCallBuilder().EmbeddedInOverlay);
        Assert.Null(new OVApiCallBuilder().SaveErrorMessage);
    }

    [Fact]
    public void Entity_inspector_exposes_busy_action_and_refresh_error_as_separate_states()
    {
        Require<OVEntityInspector>("Busy", "BusyMessage", "ActionErrorMessage",
            "Refreshing", "RefreshErrorMessage", "SummaryContent", "Sections",
            "CloseDisabledWhileRefreshing");
        var inspector = new OVEntityInspector();
        Assert.False(inspector.Busy);
        Assert.Null(inspector.ActionErrorMessage);
    }

    private static void Require<T>(params string[] expected)
    {
        var actual = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);
        Assert.All(expected, value => Assert.Contains(value, actual));
    }
}
