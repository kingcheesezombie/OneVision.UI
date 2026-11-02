using System.Reflection;
using Microsoft.AspNetCore.Components;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class DisplayValueBuilderContractTests
{
    [Fact]
    public void Source_configuration_exposes_opt_in_display_value_contract()
    {
        var parameters = ParametersOf<OVSourceConfiguration>();

        foreach (var required in new[]
        {
            nameof(OVSourceConfiguration.DisplayValueBuilderEnabled),
            nameof(OVSourceConfiguration.DisplayValueTargets),
            nameof(OVSourceConfiguration.DisplayValueDefinition),
            nameof(OVSourceConfiguration.DisplayValueDefinitionChanged),
            nameof(OVSourceConfiguration.DisplayValueReadOnly),
            nameof(OVSourceConfiguration.DisplayValueAllowLiteralOnly)
        })
            Assert.Contains(required, parameters);

        var source = new OVSourceConfiguration();
        Assert.False(source.DisplayValueBuilderEnabled);
        Assert.Empty(source.DisplayValueTargets);
        Assert.Null(source.DisplayValueDefinition);
    }

    [Fact]
    public void Display_value_targets_reference_mapping_target_identity_not_source_paths()
    {
        var core = new OVDisplayValueTargetOption("display-name", "Display name", OVMappingTargetKind.Contract);
        var detail = new OVDisplayValueTargetOption("email-detail", "Email Address", OVMappingTargetKind.StandardDetail);

        Assert.Equal("display-name", core.Key);
        Assert.Equal(OVMappingTargetKind.Contract, core.Kind);
        Assert.Equal("email-detail", detail.Key);
        Assert.Equal(OVMappingTargetKind.StandardDetail, detail.Kind);
    }

    [Fact]
    public void Definition_is_valid_when_all_target_segments_exist_in_mapping_target_set()
    {
        var targets = new[]
        {
            new OVDisplayValueTargetOption("display-name", "Display name"),
            new OVDisplayValueTargetOption("reference-code", "Reference code")
        };
        var definition = new OVDisplayValueDefinition(
        [
            new("a", OVDisplayValueSegmentKind.Target, TargetKey: "display-name"),
            new("b", OVDisplayValueSegmentKind.Literal, Literal: " ("),
            new("c", OVDisplayValueSegmentKind.Target, TargetKey: "reference-code"),
            new("d", OVDisplayValueSegmentKind.Literal, Literal: ")")
        ]);

        Assert.True(OVDisplayValueValidation.IsValid(definition, targets));
    }

    [Fact]
    public void Removing_a_standard_detail_makes_existing_reference_invalid()
    {
        var definition = new OVDisplayValueDefinition(
        [
            new("a", OVDisplayValueSegmentKind.Target, TargetKey: "display-name"),
            new("b", OVDisplayValueSegmentKind.Literal, Literal: " — "),
            new("c", OVDisplayValueSegmentKind.Target, TargetKey: "position-detail")
        ]);

        var beforeRemoval = new[]
        {
            new OVDisplayValueTargetOption("display-name", "Display name", OVMappingTargetKind.Contract),
            new OVDisplayValueTargetOption("position-detail", "Position", OVMappingTargetKind.StandardDetail)
        };
        var afterRemoval = beforeRemoval.Take(1).ToArray();

        Assert.True(OVDisplayValueValidation.IsValid(definition, beforeRemoval));
        Assert.False(OVDisplayValueValidation.IsValid(definition, afterRemoval));
    }

    [Fact]
    public void Literal_only_definition_is_disallowed_by_default_but_can_be_explicitly_enabled()
    {
        var definition = new OVDisplayValueDefinition(
        [
            new("a", OVDisplayValueSegmentKind.Literal, Literal: "Static label")
        ]);

        Assert.False(OVDisplayValueValidation.IsValid(definition, Array.Empty<OVDisplayValueTargetOption>()));
        Assert.True(OVDisplayValueValidation.IsValid(definition, Array.Empty<OVDisplayValueTargetOption>(), allowLiteralOnly: true));
    }

    [Fact]
    public void Empty_or_whitespace_only_definition_is_not_valid()
    {
        Assert.False(OVDisplayValueValidation.IsValid(null, Array.Empty<OVDisplayValueTargetOption>()));
        Assert.False(OVDisplayValueValidation.IsValid(OVDisplayValueDefinition.Empty, Array.Empty<OVDisplayValueTargetOption>()));

        var whitespace = new OVDisplayValueDefinition(
        [
            new("a", OVDisplayValueSegmentKind.Literal, Literal: "   ")
        ]);
        Assert.False(OVDisplayValueValidation.IsValid(whitespace, Array.Empty<OVDisplayValueTargetOption>(), allowLiteralOnly: true));
    }

    private static HashSet<string> ParametersOf<T>()
        => typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);
}
