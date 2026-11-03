using System.Reflection;
using Bunit;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class SourceConfigurationContractTests
{
    [Fact]
    public void Source_configuration_keeps_four_core_sections_and_opt_in_display_value_capability()
    {
        var parameters = ParametersOf<OVSourceConfiguration>();
        foreach (var required in new[]
        {
            nameof(OVSourceConfiguration.Overview), nameof(OVSourceConfiguration.Connection), nameof(OVSourceConfiguration.Profile),
            nameof(OVSourceConfiguration.Mapping), nameof(OVSourceConfiguration.ActiveSection), nameof(OVSourceConfiguration.ActiveSectionChanged),
            nameof(OVSourceConfiguration.DisplayValueBuilderEnabled)
        }) Assert.Contains(required, parameters);

        Assert.False(new OVSourceConfiguration().DisplayValueBuilderEnabled);
    }

    [Fact]
    public void Display_value_tab_is_absent_by_default_and_fifth_when_enabled()
    {
        using var context = new BunitContext();
        context.Services.AddMudServices();

        var without = context.Render<OVSourceConfiguration>(p => p.Add(x => x.Title, "Source"));
        Assert.DoesNotContain("Display Value", without.Markup);

        var with = context.Render<OVSourceConfiguration>(p => p
            .Add(x => x.Title, "Source")
            .Add(x => x.DisplayValueBuilderEnabled, true)
            .Add(x => x.ProfileState, OVSourceProfileState.Current));
        Assert.Contains("Display Value", with.Markup);
    }

    [Fact]
    public void Source_profile_exposes_definition_execution_and_profile_result_regions()
    {
        var parameters = ParametersOf<OVSourceProfile>();
        foreach (var required in new[]
        {
            nameof(OVSourceProfile.ConnectionLabel), nameof(OVSourceProfile.Definition), nameof(OVSourceProfile.DefinitionActions),
            nameof(OVSourceProfile.RunRequested), nameof(OVSourceProfile.Profiling), nameof(OVSourceProfile.Message),
            nameof(OVSourceProfile.Profile), nameof(OVSourceProfile.ResultSummary), nameof(OVSourceProfile.ResultActions)
        }) Assert.Contains(required, parameters);
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

    [Fact]
    public void Mapping_pair_exposes_opt_in_standard_details_contract()
    {
        var parameters = ParametersOf<OVMappingPair>();
        foreach (var required in new[]
        {
            nameof(OVMappingPair.StandardDetailsEnabled), nameof(OVMappingPair.StandardDetails),
            nameof(OVMappingPair.SelectedStandardDetailIds), nameof(OVMappingPair.SelectedStandardDetailIdsChanged)
        }) Assert.Contains(required, parameters);

        var mapping = new OVMappingPair();
        Assert.False(mapping.StandardDetailsEnabled);
        Assert.Empty(mapping.StandardDetails);
        Assert.Empty(mapping.SelectedStandardDetailIds);
    }

    [Fact]
    public void Standard_detail_definition_is_application_supplied_and_system_identified()
    {
        var detail = new OVStandardDetailDefinition("email", "Email Address", "Contact email", "Text");
        Assert.Equal("email", detail.Id);
        Assert.Equal("Email Address", detail.Name);
        Assert.Equal("Contact email", detail.Description);
        Assert.Equal("Text", detail.DataType);
        Assert.True(detail.Enabled);
    }

    [Fact]
    public void Mapping_validation_requires_required_targets_and_only_selected_standard_details()
    {
        var required = new[] { new OVMappingPairTarget("code", "Code", Required: true), new OVMappingPairTarget("description", "Description", Required: true) };
        var details = new[] { new OVStandardDetailDefinition("email", "Email Address"), new OVStandardDetailDefinition("position", "Position") };
        Assert.False(OVMappingPairValidation.IsValid(required, details, Array.Empty<string>(), Array.Empty<OVMappingPairEntry>()));

        var requiredMappings = new[] { new OVMappingPairEntry("code", "source-code"), new OVMappingPairEntry("description", "source-description") };
        Assert.True(OVMappingPairValidation.IsValid(required, details, Array.Empty<string>(), requiredMappings));
        Assert.False(OVMappingPairValidation.IsValid(required, details, new[] { "email" }, requiredMappings));
        var withEmail = requiredMappings.Append(new OVMappingPairEntry("email", "source-email")).ToArray();
        Assert.True(OVMappingPairValidation.IsValid(required, details, new[] { "email" }, withEmail));
        Assert.False(OVMappingPairValidation.IsValid(required, details, new[] { "email", "position" }, withEmail));
    }

    [Fact]
    public void Standard_detail_mapping_target_has_distinct_semantic_kind()
    {
        var target = new OVMappingPairTarget("email", "Email Address", Kind: OVMappingTargetKind.StandardDetail);
        Assert.Equal(OVMappingTargetKind.StandardDetail, target.Kind);
        Assert.False(target.Required);
    }

    private static HashSet<string> ParametersOf<T>()
        => typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(property => property.Name).ToHashSet(StringComparer.Ordinal);
}
