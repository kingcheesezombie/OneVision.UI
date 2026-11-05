using System.Reflection;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class PrimitiveQualityContractTests
{
    [Fact]
    public void Search_and_radio_groups_expose_independent_accessible_names()
    {
        Assert.Equal("Search", new OVSearchBox().AriaLabel);
        Assert.Equal("Options", new OVRadioGroup<string>().AriaLabel);
        Assert.Contains("AriaLabel", ParameterNames<OVSearchBox>());
        Assert.Contains("AriaLabel", ParameterNames<OVRadioGroup<string>>());
    }

    [Fact]
    public async Task Search_box_renders_a_real_accessible_name_instead_of_relying_on_placeholder()
    {
        await using var context = NewContext();
        var cut = context.Render<OVSearchBox>(p => p
            .Add(x => x.Placeholder, "Find a record")
            .Add(x => x.AriaLabel, "Search records"));
        Assert.Equal("Search records", cut.Find("input").GetAttribute("aria-label"));
    }

    [Fact]
    public async Task File_upload_labels_its_native_input_and_exposes_a_governed_limit()
    {
        await using var context = NewContext();
        var cut = context.Render<OVFileUpload>(p => p
            .Add(x => x.Prompt, "Choose import data")
            .Add(x => x.MaximumFileCount, 2)
            .Add(x => x.AllowMultiple, true));
        Assert.Equal("Choose import data", cut.Find("input[type=file]").GetAttribute("aria-label"));
        Assert.Equal(2, cut.Instance.MaximumFileCount);
    }

    [Fact]
    public async Task Tabs_fall_back_to_first_enabled_panel_when_requested_key_is_unavailable()
    {
        await using var context = NewContext();
        var tabs = new[]
        {
            new OVTabItem("disabled", "Disabled tab", builder => builder.AddContent(0, "Unavailable"), Disabled: true),
            new OVTabItem("enabled", "Available tab", builder => builder.AddContent(0, "Content"))
        };
        var cut = context.Render<OVTabs>(p => p
            .Add(x => x.Items, tabs)
            .Add(x => x.ActiveKey, "missing"));

        Assert.Equal(1, cut.FindComponent<MudBlazor.MudTabs>().Instance.ActivePanelIndex);
    }

    private static BunitContext NewContext()
    {
        var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }

    private static HashSet<string> ParameterNames<T>()
        => typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(property => property.Name).ToHashSet(StringComparer.Ordinal);
}
