using System.Reflection;
using Bunit;
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
            nameof(OVSourceConfiguration.DisplayValueBuilderEnabled), nameof(OVSourceConfiguration.DisplayValueTargets),
            nameof(OVSourceConfiguration.DisplayValueDefinition), nameof(OVSourceConfiguration.DisplayValueDefinitionChanged),
            nameof(OVSourceConfiguration.DisplayValueReadOnly), nameof(OVSourceConfiguration.DisplayValueAllowLiteralOnly)
        }) Assert.Contains(required, parameters);

        var source = new OVSourceConfiguration();
        Assert.False(source.DisplayValueBuilderEnabled);
        Assert.Empty(source.DisplayValueTargets);
        Assert.Null(source.DisplayValueDefinition);
    }

    [Fact]
    public void Display_value_targets_reference_mapping_identity_not_source_paths()
    {
        var core = new OVDisplayValueTargetOption("description", "Description", OVMappingTargetKind.Contract);
        var detail = new OVDisplayValueTargetOption("locality", "Locality", OVMappingTargetKind.StandardDetail);
        Assert.Equal("description", core.Key);
        Assert.Equal(OVMappingTargetKind.Contract, core.Kind);
        Assert.Equal("locality", detail.Key);
        Assert.Equal(OVMappingTargetKind.StandardDetail, detail.Kind);
    }

    [Fact]
    public void Mixed_separator_literals_remain_ordered_semantic_segments()
    {
        var definition = new OVDisplayValueDefinition(
        [
            new("description", OVDisplayValueSegmentKind.Target, TargetKey: "description"),
            new("open", OVDisplayValueSegmentKind.Literal, Literal: " ("),
            new("locality", OVDisplayValueSegmentKind.Target, TargetKey: "locality"),
            new("close", OVDisplayValueSegmentKind.Literal, Literal: ")"),
            new("bar", OVDisplayValueSegmentKind.Literal, Literal: " | "),
            new("identifier", OVDisplayValueSegmentKind.Target, TargetKey: "identifier")
        ]);

        Assert.Equal(new[] { "description", "open", "locality", "close", "bar", "identifier" }, definition.Segments.Select(x => x.Id));
        Assert.Equal(" (", definition.Segments[1].Literal);
        Assert.Equal(" | ", definition.Segments[4].Literal);
    }

    [Fact]
    public void Definition_is_valid_when_all_target_segments_exist_in_mapping_target_set()
    {
        var targets = new[]
        {
            new OVDisplayValueTargetOption("description", "Description"),
            new OVDisplayValueTargetOption("locality", "Locality"),
            new OVDisplayValueTargetOption("identifier", "Identifier")
        };
        var definition = new OVDisplayValueDefinition(
        [
            new("a", OVDisplayValueSegmentKind.Target, TargetKey: "description"),
            new("b", OVDisplayValueSegmentKind.Literal, Literal: " ("),
            new("c", OVDisplayValueSegmentKind.Target, TargetKey: "locality"),
            new("d", OVDisplayValueSegmentKind.Literal, Literal: ") | "),
            new("e", OVDisplayValueSegmentKind.Target, TargetKey: "identifier")
        ]);
        Assert.True(OVDisplayValueValidation.IsValid(definition, targets));
    }

    [Fact]
    public void Removing_a_mapped_target_makes_existing_reference_invalid_without_mutating_definition()
    {
        var definition = new OVDisplayValueDefinition(
        [
            new("a", OVDisplayValueSegmentKind.Target, TargetKey: "description"),
            new("b", OVDisplayValueSegmentKind.Literal, Literal: " | "),
            new("c", OVDisplayValueSegmentKind.Target, TargetKey: "locality")
        ]);
        var before = new[]
        {
            new OVDisplayValueTargetOption("description", "Description"),
            new OVDisplayValueTargetOption("locality", "Locality", OVMappingTargetKind.StandardDetail)
        };
        var after = before.Take(1).ToArray();

        Assert.True(OVDisplayValueValidation.IsValid(definition, before));
        Assert.False(OVDisplayValueValidation.IsValid(definition, after));
        Assert.Equal("locality", definition.Segments[2].TargetKey);
    }

    [Fact]
    public void Literal_only_definition_is_disallowed_by_default_but_can_be_explicitly_enabled()
    {
        var definition = new OVDisplayValueDefinition([new("a", OVDisplayValueSegmentKind.Literal, Literal: " | ")]);
        Assert.False(OVDisplayValueValidation.IsValid(definition, Array.Empty<OVDisplayValueTargetOption>()));
        Assert.True(OVDisplayValueValidation.IsValid(definition, Array.Empty<OVDisplayValueTargetOption>(), allowLiteralOnly: true));
    }

    [Fact]
    public void Empty_or_whitespace_only_definition_is_not_valid()
    {
        Assert.False(OVDisplayValueValidation.IsValid(null, Array.Empty<OVDisplayValueTargetOption>()));
        Assert.False(OVDisplayValueValidation.IsValid(OVDisplayValueDefinition.Empty, Array.Empty<OVDisplayValueTargetOption>()));
        var whitespace = new OVDisplayValueDefinition([new("a", OVDisplayValueSegmentKind.Literal, Literal: "   ")]);
        Assert.False(OVDisplayValueValidation.IsValid(whitespace, Array.Empty<OVDisplayValueTargetOption>(), allowLiteralOnly: true));
    }

    [Fact]
    public void Builder_renders_parenthesis_and_mixed_separator_quick_actions()
    {
        using var context = new BunitContext();
        var cut = context.Render<OVDisplayValueBuilder>(p => p.Add(x => x.Targets, new[] { new OVDisplayValueTargetOption("description", "Description") }));
        Assert.Contains("Add separator open parenthesis", cut.Markup);
        Assert.Contains("Add separator close parenthesis", cut.Markup);
        Assert.Contains("Add separator vertical bar", cut.Markup);
        Assert.DoesNotContain("Move segment", cut.Markup);
    }

    private static HashSet<string> ParametersOf<T>()
        => typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetCustomAttribute<ParameterAttribute>() is not null)
            .Select(property => property.Name).ToHashSet(StringComparer.Ordinal);
}
