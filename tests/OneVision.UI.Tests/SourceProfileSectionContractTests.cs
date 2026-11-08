using Bunit;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class SourceProfileSectionContractTests
{
    private static BunitContext Context()
    {
        var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }

    [Theory]
    [InlineData(OVSourceProfileState.Initial)]
    [InlineData(OVSourceProfileState.Current)]
    [InlineData(OVSourceProfileState.Stale)]
    [InlineData(OVSourceProfileState.Failed)]
    public async Task Profile_has_two_contained_sections_with_run_action_inside_configuration(OVSourceProfileState state)
    {
        await using var context = Context();
        var cut = context.Render<OVSourceProfile>(parameters => parameters
            .Add(p => p.ProfileState, state)
            .Add<RenderFragment>(p => p.Definition, builder => builder.AddContent(0, "Source-specific settings"))
            .Add<RenderFragment>(p => p.Profile, builder => builder.AddContent(0, "Discovered fields")));

        var source = cut.Find("section.ov-source-profile__definition");
        var results = cut.Find("section.ov-source-profile__result");
        Assert.Equal("Source Configuration", source.QuerySelector("h3")?.TextContent);
        Assert.Equal("Profile Results", results.QuerySelector("h3")?.TextContent);
        Assert.Contains("Source-specific settings", source.TextContent);
        Assert.Contains("Run & Profile", source.TextContent);
        Assert.DoesNotContain("Run & Profile", results.QuerySelector(".ov-source-profile__section-heading")?.TextContent);
        Assert.Contains("Discovered fields", results.TextContent);
        Assert.Equal(1, cut.FindAll(".ov-source-profile__runbar").Count);
        Assert.NotNull(source.QuerySelector(".ov-source-profile__runbar"));
        Assert.Null(results.QuerySelector(".ov-source-profile__runbar"));
    }

    [Fact]
    public async Task Readonly_profile_keeps_results_and_suppresses_run_action()
    {
        await using var context = Context();
        var cut = context.Render<OVSourceProfile>(parameters => parameters
            .Add(p => p.ReadOnly, true)
            .Add<RenderFragment>(p => p.Profile, builder => builder.AddContent(0, "Saved profile")));

        Assert.Contains("Saved profile", cut.Find("section.ov-source-profile__result").TextContent);
        Assert.Empty(cut.FindComponents<OVButton>());
    }
}
