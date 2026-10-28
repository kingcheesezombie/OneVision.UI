using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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
            nameof(OVEntityInspector.Actions), nameof(OVEntityInspector.SecondaryActions), nameof(OVEntityInspector.PrimaryAction),
            nameof(OVEntityInspector.OverflowActions), nameof(OVEntityInspector.OverflowActionSelected)
        })
            Assert.Contains(required, parameters);

        Assert.Equal(OVOverlayPlacement.RightSide, new OVEntityInspector().Placement);
        Assert.Contains(OVOverlayPlacement.Workspace, Enum.GetValues<OVOverlayPlacement>());
    }

    [Fact]
    public async Task Close_is_allowed_during_refresh_by_default_and_can_be_deliberately_disabled()
    {
        bool? emitted = null;
        var inspector = new OVEntityInspector
        {
            Open = true,
            Refreshing = true,
            OpenChanged = EventCallback.Factory.Create<bool>(this, value => emitted = value)
        };

        await InvokePrivateTask(inspector, "CloseClicked", new MouseEventArgs());
        Assert.False(emitted);

        emitted = null;
        inspector.CloseDisabledWhileRefreshing = true;
        await InvokePrivateTask(inspector, "CloseClicked", new MouseEventArgs());
        Assert.Null(emitted);
    }

    [Fact]
    public async Task Refresh_invokes_the_application_callback_once_and_ignores_repeat_while_refreshing()
    {
        var count = 0;
        var inspector = new OVEntityInspector
        {
            RefreshRequested = EventCallback.Factory.Create(this, () => count++)
        };

        await InvokePrivateTask(inspector, "RefreshClicked", new MouseEventArgs());
        inspector.Refreshing = true;
        await InvokePrivateTask(inspector, "RefreshClicked", new MouseEventArgs());

        Assert.Equal(1, count);
    }

    [Fact]
    public void Inspector_exposes_initial_and_recoverable_error_states_without_domain_dependencies()
    {
        var parameters = ParametersOf<OVEntityInspector>();
        foreach (var required in new[]
        {
            nameof(OVEntityInspector.Loading), nameof(OVEntityInspector.Refreshing), nameof(OVEntityInspector.CloseDisabledWhileRefreshing),
            nameof(OVEntityInspector.ErrorMessage), nameof(OVEntityInspector.ErrorDetail), nameof(OVEntityInspector.RefreshErrorMessage),
            nameof(OVEntityInspector.Empty), nameof(OVEntityInspector.EmptyTitle), nameof(OVEntityInspector.EmptyDescription)
        })
            Assert.Contains(required, parameters);

        Assert.DoesNotContain(typeof(OVEntityInspector).Assembly.GetReferencedAssemblies(), reference =>
            reference.Name?.Contains("Identity", StringComparison.OrdinalIgnoreCase) == true ||
            reference.Name?.Contains("Allocation", StringComparison.OrdinalIgnoreCase) == true);
    }

    [Fact]
    public void Metadata_list_supports_stable_identity_formatting_empty_treatment_and_structured_threshold()
    {
        var parameters = ParametersOf<OVMetadataList>();
        Assert.Contains(nameof(OVMetadataList.Items), parameters);
        Assert.Contains(nameof(OVMetadataList.EmptyText), parameters);
        Assert.Contains(nameof(OVMetadataList.StructuredCollapseLineCount), parameters);
        Assert.Contains(nameof(OVMetadataList.ShowEmptyState), parameters);
        Assert.Contains(nameof(OVMetadataList.EmptyMessage), parameters);

        Assert.Equal(new[] { "Text", "Number", "DateTime", "Boolean", "Structured" }, Enum.GetNames<OVMetadataValueKind>());

        var item = new OVMetadataItem("Reference", "ABC-123", "Identity", OVMetadataValueKind.Text,
            Copyable: true, Monospace: true, SecondaryText: "Source reference", Format: "N0", Key: "reference");
        Assert.Equal("Identity", item.Group);
        Assert.True(item.Copyable);
        Assert.True(item.Monospace);
        Assert.Equal("Source reference", item.SecondaryText);
        Assert.Equal("N0", item.Format);
        Assert.Equal("reference", item.Key);

        var boolean = new OVMetadataItem("Enabled", null, Kind: OVMetadataValueKind.Boolean, RawValue: true);
        Assert.True((bool)boolean.RawValue!);

        var structured = new OVMetadataItem("Payload", "{\"state\":\"ready\"}", Kind: OVMetadataValueKind.Structured);
        Assert.Equal(OVMetadataValueKind.Structured, structured.Kind);
    }

    [Fact]
    public void Inspector_remains_read_only_by_default_and_close_text_is_formally_obsolete()
    {
        var inspector = new OVEntityInspector();
        Assert.False(inspector.Loading);
        Assert.False(inspector.Refreshing);
        Assert.False(inspector.CloseDisabledWhileRefreshing);
        Assert.False(inspector.Empty);
        Assert.Null(inspector.Actions);
        Assert.Null(inspector.SecondaryActions);
        Assert.Null(inspector.PrimaryAction);
        Assert.Empty(inspector.OverflowActions);

        var closeText = typeof(OVEntityInspector).GetProperty(nameof(OVEntityInspector.CloseText));
        Assert.NotNull(closeText);
        var obsolete = closeText!.GetCustomAttribute<ObsoleteAttribute>();
        Assert.NotNull(obsolete);
        Assert.Contains("overlay header", obsolete!.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static async Task InvokePrivateTask(object target, string methodName, params object?[] arguments)
    {
        var method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(method);
        var result = method!.Invoke(target, arguments);
        Assert.IsAssignableFrom<Task>(result);
        await (Task)result!;
    }

    private static HashSet<string> ParametersOf<T>()
        => typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);
}
