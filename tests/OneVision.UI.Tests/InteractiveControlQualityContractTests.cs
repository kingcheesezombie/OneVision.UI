using System.Reflection;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class InteractiveControlQualityContractTests
{
    [Fact]
    public void Accordion_retains_legacy_constructor_and_adds_optional_stable_key()
    {
        RenderFragment fragment = builder => builder.AddContent(0, "Details");
        var legacy = new OVAccordionItem("Advanced", fragment, Expanded: true);
        var stable = new OVAccordionItem("Advanced", fragment, Expanded: true, Key: "advanced");

        Assert.True(legacy.Expanded);
        Assert.Null(legacy.Key);
        Assert.Equal("advanced", stable.Key);
    }

    [Fact]
    public void Accordion_exposes_explicit_controlled_and_uncontrolled_semantics()
    {
        var parameters = typeof(OVAccordion)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(p => p.Name)
            .ToHashSet(StringComparer.Ordinal);

        Assert.Contains(nameof(OVAccordion.Items), parameters);
        Assert.Contains(nameof(OVAccordion.Disabled), parameters);
        Assert.Contains(nameof(OVAccordion.ExpandedKeys), parameters);
        Assert.Contains(nameof(OVAccordion.ExpandedKeysChanged), parameters);
        Assert.Contains(nameof(OVAccordion.AllowMultiple), parameters);
        Assert.Equal(typeof(EventCallback<IReadOnlyCollection<string>>),
            typeof(OVAccordion).GetProperty(nameof(OVAccordion.ExpandedKeysChanged))!.PropertyType);
    }

    [Fact]
    public async Task Source_discovery_exposes_one_tabbable_candidate_and_preserves_preview_selection_contract()
    {
        await using var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;

        var items = new[]
        {
            new OVSourceDiscoveryCandidate("a", "First source"),
            new OVSourceDiscoveryCandidate("b", "Second source")
        };

        var cut = context.Render<OVSourceDiscovery>(parameters => parameters
            .Add(c => c.State, OVSourceDiscoveryState.Success)
            .Add(c => c.Candidates, items));

        var candidates = cut.FindAll(".ov-source-discovery__candidate");
        Assert.Equal(2, candidates.Count);
        Assert.Equal("0", candidates[0].GetAttribute("tabindex"));
        Assert.Equal("-1", candidates[1].GetAttribute("tabindex"));
        Assert.Equal("option", candidates[0].GetAttribute("role"));
        Assert.Equal("false", candidates[0].GetAttribute("aria-selected"));
        Assert.Contains("Up/Down arrows", cut.Markup);

        var readOnly = context.Render<OVSourceDiscovery>(parameters => parameters
            .Add(c => c.State, OVSourceDiscoveryState.Success)
            .Add(c => c.Candidates, items)
            .Add(c => c.ReadOnly, true));
        Assert.All(readOnly.FindAll(".ov-source-discovery__candidate"),
            candidate => Assert.NotNull(candidate.GetAttribute("disabled")));
    }
}
