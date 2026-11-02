using System.Reflection;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using OneVision.UI.Components;
using OneVision.UI.Iconography;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class OverflowMenuContractTests
{
    [Fact]
    public void Public_api_remains_compatible()
    {
        var parameters = typeof(OVOverflowMenu)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);

        foreach (var expected in new[]
        {
            nameof(OVOverflowMenu.Items),
            nameof(OVOverflowMenu.ItemSelected),
            nameof(OVOverflowMenu.AriaLabel),
            nameof(OVOverflowMenu.Disabled),
            nameof(OVOverflowMenu.AlignRight),
            nameof(OVOverflowMenu.Class)
        })
            Assert.Contains(expected, parameters);

        Assert.True(new OVOverflowMenu().AlignRight);
    }

    [Fact]
    public async Task Opens_selects_and_closes_without_changing_item_semantics()
    {
        await using var context = CreateContext();
        string? selected = null;
        var items = new[]
        {
            new OVOverflowMenuItem("edit", "Edit", OneVisionIcon.Edit),
            new OVOverflowMenuItem("disabled", "Unavailable", Disabled: true),
            new OVOverflowMenuItem("delete", "Delete", OneVisionIcon.Delete, Destructive: true)
        };

        var cut = context.Render<OVOverflowMenu>(parameters => parameters
            .Add(component => component.Items, items)
            .Add(component => component.AriaLabel, "Row actions")
            .Add(component => component.ItemSelected, EventCallback.Factory.Create<string>(this, value => selected = value)));

        var trigger = cut.Find("button[aria-haspopup='menu']");
        Assert.Equal("false", trigger.GetAttribute("aria-expanded"));

        await trigger.ClickAsync();

        Assert.Equal("true", cut.Find("button[aria-haspopup='menu']").GetAttribute("aria-expanded"));
        Assert.Equal("Row actions", cut.Find("[role='menu']").GetAttribute("aria-label"));
        Assert.True(cut.Find("button[role='menuitem'][disabled]").HasAttribute("disabled"));
        Assert.Contains("ov-overflow-menu__item--destructive", cut.FindAll("button[role='menuitem']")[2].ClassList);

        await cut.FindAll("button[role='menuitem']")[0].ClickAsync();

        Assert.Equal("edit", selected);
        Assert.Empty(cut.FindAll("[role='menu']"));
        Assert.Equal("false", cut.Find("button[aria-haspopup='menu']").GetAttribute("aria-expanded"));
    }

    [Fact]
    public async Task Disabled_item_does_not_invoke_selection()
    {
        await using var context = CreateContext();
        var selected = false;
        var cut = context.Render<OVOverflowMenu>(parameters => parameters
            .Add(component => component.Items, [new OVOverflowMenuItem("disabled", "Unavailable", Disabled: true)])
            .Add(component => component.ItemSelected, EventCallback.Factory.Create<string>(this, _ => selected = true)));

        await cut.Find("button[aria-haspopup='menu']").ClickAsync();
        await cut.Find("button[role='menuitem']").ClickAsync();

        Assert.False(selected);
        Assert.Single(cut.FindAll("[role='menu']"));
    }

    [Fact]
    public void Align_left_does_not_change_the_public_trigger_contract()
    {
        using var context = CreateContext();
        var cut = context.Render<OVOverflowMenu>(parameters => parameters
            .Add(component => component.AlignRight, false)
            .Add(component => component.Items, [new OVOverflowMenuItem("edit", "Edit")]));

        Assert.DoesNotContain("ov-overflow-menu--right", cut.Find(".ov-overflow-menu").ClassList);
        Assert.Equal("menu", cut.Find("button").GetAttribute("aria-haspopup"));
    }

    private static BunitContext CreateContext()
    {
        var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }
}
