using System.Reflection;
using Microsoft.AspNetCore.Components;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class SourceConfigurationContractTests
{
    [Fact]
    public void Source_configuration_keeps_the_four_governed_peer_sections()
    {
        var parameters = ParametersOf<OVSourceConfiguration>();

        foreach (var required in new[]
        {
            nameof(OVSourceConfiguration.Overview),
            nameof(OVSourceConfiguration.Connection),
            nameof(OVSourceConfiguration.Profile),
            nameof(OVSourceConfiguration.Mapping),
            nameof(OVSourceConfiguration.ActiveSection),
            nameof(OVSourceConfiguration.ActiveSectionChanged)
        })
            Assert.Contains(required, parameters);
    }

    [Fact]
    public void Source_profile_exposes_definition_execution_and_profile_result_regions()
    {
        var parameters = ParametersOf<OVSourceProfile>();

        foreach (var required in new[]
        {
            nameof(OVSourceProfile.ConnectionLabel),
            nameof(OVSourceProfile.Definition),
            nameof(OVSourceProfile.DefinitionActions),
            nameof(OVSourceProfile.RunRequested),
            nameof(OVSourceProfile.Profiling),
            nameof(OVSourceProfile.Message),
            nameof(OVSourceProfile.Profile),
            nameof(OVSourceProfile.ResultSummary),
            nameof(OVSourceProfile.ResultActions)
        })
            Assert.Contains(required, parameters);
    }

    [Fact]
    public void Source_profile_owns_governed_run_and_profile_defaults()
    {
        var profile = new OVSourceProfile();

        Assert.Equal("Profile", profile.Title);
        Assert.Equal("Source definition", profile.DefinitionTitle);
        Assert.Equal("Profiled output", profile.ResultTitle);
        Assert.Equal("Run & Profile", profile.RunLabel);
    }

    private static HashSet<string> ParametersOf<T>()
        => typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);
}
