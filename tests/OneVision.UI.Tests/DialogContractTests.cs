using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class DialogContractTests
{
    [Fact]
    public async Task Dirty_dialog_uses_governed_discard_confirmation()
    {
        await using var context = CreateContext();

        var cut = context.Render<OVDialog>(parameters => parameters
            .Add(component => component.Open, true)
            .Add(component => component.Dirty, true)
            .Add(component => component.Title, "Edit source")
            .Add(component => component.ChildContent, (RenderFragment)(builder => builder.AddContent(0, "Body"))));

        await cut.Find("button[aria-label='Close dialog']").ClickAsync();

        var confirmation = cut.Find("section[role='alertdialog']");
        Assert.Contains("Discard changes?", confirmation.TextContent, StringComparison.Ordinal);
        Assert.Contains("Unsaved changes will be lost.", confirmation.TextContent, StringComparison.Ordinal);
        Assert.Contains("Cancel", confirmation.TextContent, StringComparison.Ordinal);
        Assert.Contains("Discard changes", confirmation.TextContent, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Dirty_confirmation_inherits_contained_dialog_scope()
    {
        await using var context = CreateContext();

        var cut = context.Render<OVDialog>(parameters => parameters
            .Add(component => component.Open, true)
            .Add(component => component.Dirty, true)
            .Add(component => component.Contained, true)
            .Add(component => component.Title, "Edit source")
            .Add(component => component.ChildContent, (RenderFragment)(builder => builder.AddContent(0, "Body"))));

        await cut.Find("button[aria-label='Close dialog']").ClickAsync();

        var confirmation = cut.Find("section[role='alertdialog']");
        var backdrop = confirmation.ParentElement;

        Assert.NotNull(backdrop);
        Assert.Contains("ov-confirm-dialog__backdrop--contained", backdrop!.ClassList);
    }

    private static BunitContext CreateContext()
    {
        var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }
}
