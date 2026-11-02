using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class SourceConfigurationDirectSourceContractTests
{
    [Fact]
    public async Task Source_configuration_defaults_to_managed_connection_mode()
    {
        await using var context = CreateContext();
        var cut = context.Render<OVSourceConfiguration>(parameters => parameters
            .Add(component => component.Title, "Managed source")
            .Add(component => component.ActiveSection, "connection")
            .Add(component => component.Connection, builder => builder.AddMarkupContent(0, "<div data-testid='managed-connection'>Managed selector</div>")));

        Assert.Equal(OVSourceConnectionMode.ManagedConnection, cut.Instance.ConnectionMode);
        Assert.NotNull(cut.Find("[data-testid='managed-connection']"));
        Assert.DoesNotContain("No managed connection required", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public async Task No_connection_required_renders_intentional_satisfied_state_instead_of_managed_content()
    {
        await using var context = CreateContext();
        var cut = context.Render<OVSourceConfiguration>(parameters => parameters
            .Add(component => component.Title, "Direct source")
            .Add(component => component.ActiveSection, "connection")
            .Add(component => component.ConnectionMode, OVSourceConnectionMode.NoConnectionRequired)
            .Add(component => component.Connection, builder => builder.AddMarkupContent(0, "<div data-testid='managed-connection'>Managed selector</div>")));

        Assert.Contains("No managed connection required", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("This source is configured directly in Profile.", cut.Markup, StringComparison.Ordinal);
        Assert.Empty(cut.FindAll("[data-testid='managed-connection']"));
    }

    [Fact]
    public async Task Stale_profile_blocks_previous_mapping_content()
    {
        await using var context = CreateContext();
        var cut = context.Render<OVSourceConfiguration>(parameters => parameters
            .Add(component => component.Title, "Direct source")
            .Add(component => component.ActiveSection, "mapping")
            .Add(component => component.ProfileState, OVSourceProfileState.Stale)
            .Add(component => component.Mapping, builder => builder.AddMarkupContent(0, "<div data-testid='mapping'>Old mapping</div>")));

        Assert.Empty(cut.FindAll("[data-testid='mapping']"));
        Assert.Contains("Run &amp; Profile again", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("before using the discovered structure for Mapping", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Discovery_presents_multiple_candidates_and_actual_selected_preview_data()
    {
        await using var context = CreateContext();
        var candidates = new[]
        {
            new OVSourceDiscoveryCandidate(
                "population",
                "Population by neighbourhood",
                "Population estimates",
                1,
                3,
                120,
                ["Area", "Year", "Population"],
                [["North Ward", "2025", "8,412"], ["South Ward", "2025", "7,934"]]),
            new OVSourceDiscoveryCandidate(
                "activity",
                "Service activity",
                "Monthly activity",
                2,
                2,
                24,
                ["Month", "Completed"],
                [["2026-09", "1,876"]])
        };

        var cut = context.Render<OVSourceDiscovery>(parameters => parameters
            .Add(component => component.State, OVSourceDiscoveryState.Success)
            .Add(component => component.Candidates, candidates)
            .Add(component => component.SelectedId, "population"));

        Assert.Contains("Population by neighbourhood", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("Service activity", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("North Ward", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("8,412", cut.Markup, StringComparison.Ordinal);
        Assert.Equal(2, cut.FindAll("[role='option']").Count);
        Assert.Equal("true", cut.Find("[role='option'][aria-selected='true']").GetAttribute("aria-selected"));
    }

    [Fact]
    public async Task Source_profile_retains_stale_result_for_context_and_suppresses_mutation_actions_when_read_only()
    {
        await using var context = CreateContext();
        var cut = context.Render<OVSourceProfile>(parameters => parameters
            .Add(component => component.ProfileState, OVSourceProfileState.Stale)
            .Add(component => component.ReadOnly, true)
            .Add(component => component.Definition, builder => builder.AddContent(0, "Definition value"))
            .Add(component => component.DefinitionActions, builder => builder.AddMarkupContent(0, "<button data-testid='definition-action'>Change</button>"))
            .Add(component => component.Profile, builder => builder.AddContent(0, "Previously discovered field"))
            .Add(component => component.ResultActions, builder => builder.AddMarkupContent(0, "<button data-testid='result-action'>Change</button>")));

        Assert.Contains("Previously discovered field", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("Stale", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("retained for context only", cut.Markup, StringComparison.Ordinal);
        Assert.Empty(cut.FindAll("[data-testid='definition-action']"));
        Assert.Empty(cut.FindAll("[data-testid='result-action']"));
        Assert.DoesNotContain(cut.FindAll("button"), button => button.TextContent.Contains("Run & Profile", StringComparison.Ordinal));
    }

    [Fact]
    public void Direct_source_contracts_are_domain_neutral()
    {
        var candidate = new OVSourceDiscoveryCandidate("candidate-1", "Candidate source");

        Assert.Equal("candidate-1", candidate.Id);
        Assert.Equal("Candidate source", candidate.Title);
        Assert.True(candidate.Usable);
        Assert.Empty(candidate.ResolvedColumnHeadings);
        Assert.Empty(candidate.ResolvedPreviewRows);
    }

    private static BunitContext CreateContext()
    {
        var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }
}
