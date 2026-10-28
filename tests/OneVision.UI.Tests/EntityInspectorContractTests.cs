using System.Reflection;
using Microsoft.AspNetCore.Components;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class EntityInspectorContractTests
{
    [Fact]
    public void Inspector_exposes_both_governed_overlay_placements_and_controlled_close_refresh_contracts()
    {
        var parameters = ParametersOf<OVEntityInspector>();
        foreach (var required in new[]
        {
            nameof(OVEntityInspector.Open), nameof(OVEntityInspector.OpenChanged), nameof(OVEntityInspector.Title),
            nameof(OVEntityInspector.Subtitle), nameof(OVEntityInspector.Placement), nameof(OVEntityInspector.RefreshRequested),
            nameof(OVEntityInspector.StatusContent), nameof(OVEntityInspector.SummaryContent), nameof(OVEntityInspector.Sections),
            nameof(OVEntityInspector.Actions)
        })
            Assert.Contains(required, parameters);

        Assert.Equal(OVOverlayPlacement.RightSide, new OVEntityInspector().Placement);
        Assert.Contains(OVOverlayPlacement.Workspace, Enum.GetValues<OVOverlayPlacement>());
    }

    [Fact]
    public void Inspector_exposes_loading_error_empty_and_refreshing_states_without_domain_dependencies()
    {
        var parameters = ParametersOf<OVEntityInspector>();
        foreach (var required in new[]
        {
            nameof(OVEntityInspector.Loading), nameof(OVEntityInspector.Refreshing), nameof(OVEntityInspector.ErrorMessage),
            nameof(OVEntityInspector.Empty), nameof(OVEntityInspector.EmptyTitle), nameof(OVEntityInspector.EmptyDescription)
        })
            Assert.Contains(required, parameters);

        Assert.DoesNotContain(typeof(OVEntityInspector).Assembly.GetReferencedAssemblies(), reference =>
            reference.Name?.Contains("Identity", StringComparison.OrdinalIgnoreCase) == true ||
            reference.Name?.Contains("Allocation", StringComparison.OrdinalIgnoreCase) == true);
    }

    [Fact]
    public void Metadata_list_supports_grouping_empty_copy_monospace_and_structured_value_kinds()
    {
        var parameters = ParametersOf<OVMetadataList>();
        Assert.Contains(nameof(OVMetadataList.Items), parameters);
        Assert.Contains(nameof(OVMetadataList.EmptyText), parameters);

        Assert.Equal(new[] { "Text", "Number", "DateTime", "Boolean", "Structured" }, Enum.GetNames<OVMetadataValueKind>());

        var item = new OVMetadataItem("Reference", "ABC-123", "Identity", OVMetadataValueKind.Text, Copyable: true, Monospace: true);
        Assert.Equal("Identity", item.Group);
        Assert.True(item.Copyable);
        Assert.True(item.Monospace);

        var structured = new OVMetadataItem("Payload", "{\"state\":\"ready\"}", Kind: OVMetadataValueKind.Structured);
        Assert.Equal(OVMetadataValueKind.Structured, structured.Kind);
    }

    [Fact]
    public void Inspector_remains_read_only_by_default_and_application_actions_are_optional()
    {
        var inspector = new OVEntityInspector();
        Assert.False(inspector.Loading);
        Assert.False(inspector.Refreshing);
        Assert.False(inspector.Empty);
        Assert.Null(inspector.Actions);
        Assert.Equal("Close", inspector.CloseText);
    }

    private static HashSet<string> ParametersOf<T>()
        => typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);
}
