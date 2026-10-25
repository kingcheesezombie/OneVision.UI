using System.Reflection;
using Microsoft.AspNetCore.Components;
using OneVision.UI.Components;
using OneVision.UI.Iconography;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class SurfaceControlsContractTests
{
    [Fact]
    public void Overflow_menu_exposes_semantic_items_and_selection()
    {
        var parameters = ParametersOf<OVOverflowMenu>();
        Assert.Contains(nameof(OVOverflowMenu.Items), parameters);
        Assert.Contains(nameof(OVOverflowMenu.ItemSelected), parameters);
        Assert.Contains(nameof(OVOverflowMenu.AriaLabel), parameters);
        Assert.Contains(nameof(OVOverflowMenu.Disabled), parameters);

        var item = new OVOverflowMenuItem("delete", "Delete", OneVisionIcon.Delete, Destructive: true);
        Assert.Equal("delete", item.Key);
        Assert.True(item.Destructive);
    }

    [Fact]
    public void Pagination_exposes_page_total_and_default_page_size_contract()
    {
        var parameters = ParametersOf<OVPagination>();
        Assert.Contains(nameof(OVPagination.Page), parameters);
        Assert.Contains(nameof(OVPagination.TotalPages), parameters);
        Assert.Contains(nameof(OVPagination.TotalCount), parameters);
        Assert.Contains(nameof(OVPagination.PageSize), parameters);
        Assert.Contains(nameof(OVPagination.PageChanged), parameters);
        Assert.Equal(25, new OVPagination().PageSize);
    }

    [Fact]
    public void Dialog_and_overlay_intercept_dirty_close_and_expose_governed_actions()
    {
        foreach (var type in new[] { typeof(OVDialog), typeof(OVOverlay) })
        {
            var parameters = ParametersOf(type);
            foreach (var required in new[] { "Open", "OpenChanged", "Title", "Description", "Busy", "Dirty", "SaveDisabled", "SaveText", "SaveRequested", "Footer", "ChildContent" })
                Assert.Contains(required, parameters);
        }
    }

    [Fact]
    public void Confirm_dialog_exposes_consequence_and_busy_contract()
    {
        var parameters = ParametersOf<OVConfirmDialog>();
        foreach (var required in new[] { nameof(OVConfirmDialog.Open), nameof(OVConfirmDialog.OpenChanged), nameof(OVConfirmDialog.Title), nameof(OVConfirmDialog.Message), nameof(OVConfirmDialog.ConfirmText), nameof(OVConfirmDialog.Busy), nameof(OVConfirmDialog.Confirmed), nameof(OVConfirmDialog.Cancelled) })
            Assert.Contains(required, parameters);
    }

    private static HashSet<string> ParametersOf<T>() => ParametersOf(typeof(T));

    private static HashSet<string> ParametersOf(Type type)
        => type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);
}
