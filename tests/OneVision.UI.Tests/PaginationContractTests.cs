using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class PaginationContractTests
{
    [Fact]
    public async Task Pagination_renders_supplied_accessibility_label_and_preserves_page_callbacks()
    {
        await using var context = CreateContext();
        int? changedPage = null;

        var cut = context.Render<OVPagination>(parameters => parameters
            .Add(component => component.Page, 2)
            .Add(component => component.TotalPages, 4)
            .Add(component => component.TotalCount, 92)
            .Add(component => component.PageSize, 25)
            .Add(component => component.AriaLabel, "Standard details pagination")
            .Add(component => component.PageChanged, page => changedPage = page));

        Assert.NotNull(cut.Find("nav[aria-label='Standard details pagination']"));
        Assert.Contains("92 items", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("Page 2 of 4", cut.Markup, StringComparison.Ordinal);

        await cut.Find("button[aria-label='Next page']").ClickAsync();
        Assert.Equal(3, changedPage);

        await cut.Find("button[aria-label='Previous page']").ClickAsync();
        Assert.Equal(1, changedPage);
    }

    [Fact]
    public async Task Pagination_uses_governed_default_accessibility_label_when_omitted()
    {
        await using var context = CreateContext();
        var cut = context.Render<OVPagination>(parameters => parameters
            .Add(component => component.Page, 1)
            .Add(component => component.TotalPages, 2)
            .Add(component => component.TotalCount, 30));

        Assert.NotNull(cut.Find("nav[aria-label='Pagination']"));
    }

    [Fact]
    public async Task Pagination_boundary_states_keep_navigation_disabled()
    {
        await using var context = CreateContext();
        var cut = context.Render<OVPagination>(parameters => parameters
            .Add(component => component.Page, 1)
            .Add(component => component.TotalPages, 1)
            .Add(component => component.TotalCount, 8));

        Assert.True(cut.Find("button[aria-label='Previous page']").HasAttribute("disabled"));
        Assert.True(cut.Find("button[aria-label='Next page']").HasAttribute("disabled"));
        Assert.Contains("Page 1 of 1", cut.Markup, StringComparison.Ordinal);
    }

    private static BunitContext CreateContext()
    {
        var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }
}
