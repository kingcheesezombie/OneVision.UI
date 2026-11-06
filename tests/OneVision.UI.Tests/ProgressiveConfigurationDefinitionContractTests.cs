using Bunit;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class ProgressiveConfigurationDefinitionContractTests
{
    private static BunitContext Context()
    {
        var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }

    [Fact]
    public async Task Definition_mode_supports_arbitrarily_many_steps_and_generated_sequence_numbers()
    {
        await using var context = Context();
        var steps = Enumerable.Range(1, 12)
            .Select(i => (OVConfigurationItemDefinition)new OVConfigurationStepDefinition($"step-{i}", $"Policy step {i}")
            {
                Value = i < 12 ? "Standard" : null,
                Editor = builder => builder.AddContent(0, $"Editor {i}")
            }).ToArray();
        var cut = context.Render<OVProgressiveConfigurationBuilder>(parameters => parameters
            .Add(p => p.Items, steps));

        Assert.Equal(12, cut.FindAll(".ov-configuration-step").Count);
        Assert.Equal("definitions", cut.Find(".ov-progressive-builder__body").GetAttribute("data-composition"));
        Assert.Contains("Editor 12", cut.Markup);
        Assert.Equal("current", cut.FindAll(".ov-configuration-step")[11].GetAttribute("data-state"));
        Assert.Contains("12", cut.FindAll(".ov-configuration-step")[11].QuerySelector(".ov-configuration-step__sequence")!.TextContent);
    }

    [Fact]
    public async Task Groups_and_boundaries_can_be_interleaved_with_steps()
    {
        await using var context = Context();
        IReadOnlyList<OVConfigurationItemDefinition> definition =
        [
            new OVConfigurationSectionDefinition("a", "A", [
                new OVConfigurationStepDefinition("first", "First") { Value = "Ready" },
                new OVConfigurationSeparatorDefinition("divider", OVConfigurationSeparatorStyle.LabelledDivider, "Next condition"),
                new OVConfigurationStepDefinition("second", "Second")
            ]),
            new OVConfigurationSeparatorDefinition("arrow", OVConfigurationSeparatorStyle.LabelledArrow, "then route to"),
            new OVConfigurationSectionDefinition("b", "B", [
                new OVConfigurationStepDefinition("third", "Third") { DependsOn = ["second"] }
            ])
        ];
        var cut = context.Render<OVProgressiveConfigurationBuilder>(parameters => parameters
            .Add(p => p.Items, definition));

        Assert.Equal(3, cut.FindAll(".ov-configuration-step").Count);
        Assert.Equal(2, cut.FindAll(".ov-configuration-section").Count);
        Assert.Equal(2, cut.FindAll(".ov-configuration-relationship").Count);
        Assert.Contains("Next condition", cut.Markup);
        Assert.Contains("then route to", cut.Markup);
        Assert.Equal("pending", cut.FindAll(".ov-configuration-step")[2].GetAttribute("data-state"));
    }

    [Fact]
    public async Task Different_separator_presentation_modes_are_governed()
    {
        await using var context = Context();
        var definition = new OVConfigurationItemDefinition[]
        {
            new OVConfigurationSeparatorDefinition("none", OVConfigurationSeparatorStyle.None),
            new OVConfigurationSeparatorDefinition("arrow", OVConfigurationSeparatorStyle.Arrow),
            new OVConfigurationSeparatorDefinition("divider", OVConfigurationSeparatorStyle.Divider),
            new OVConfigurationSeparatorDefinition("label", OVConfigurationSeparatorStyle.LabelledDivider, "After validation")
        };
        var cut = context.Render<OVProgressiveConfigurationBuilder>(p => p.Add(x => x.Items, definition));
        Assert.Equal(3, cut.FindAll(".ov-configuration-relationship").Count);
        Assert.Single(cut.FindAll(".ov-configuration-relationship__direction"));
        Assert.Equal(2, cut.FindAll(".ov-configuration-relationship__line").Count);
        Assert.Contains("After validation", cut.Markup);
    }

    [Fact]
    public async Task Explicit_dependencies_block_later_editor_until_parent_is_complete()
    {
        await using var context = Context();
        IReadOnlyList<OVConfigurationItemDefinition> before =
        [
            new OVConfigurationStepDefinition("first", "First") { Editor = b => b.AddContent(0, "Choose first") },
            new OVConfigurationStepDefinition("second", "Second") { DependsOn = ["first"], Editor = b => b.AddContent(0, "Choose second") }
        ];
        var cut = context.Render<OVProgressiveConfigurationBuilder>(p => p.Add(x => x.Items, before));
        Assert.Equal("current", cut.FindAll(".ov-configuration-step")[0].GetAttribute("data-state"));
        Assert.Equal("pending", cut.FindAll(".ov-configuration-step")[1].GetAttribute("data-state"));

        IReadOnlyList<OVConfigurationItemDefinition> after =
        [
            new OVConfigurationStepDefinition("first", "First") { Value = "Selected" },
            new OVConfigurationStepDefinition("second", "Second") { DependsOn = ["first"], Editor = b => b.AddContent(0, "Choose second") }
        ];
        cut.Render(p => p.Add(x => x.Items, after));
        Assert.Equal("completed", cut.FindAll(".ov-configuration-step")[0].GetAttribute("data-state"));
        Assert.Equal("current", cut.FindAll(".ov-configuration-step")[1].GetAttribute("data-state"));
        Assert.Contains("Choose second", cut.Markup);
    }

    [Fact]
    public async Task Definition_sections_derive_completion_from_actual_child_steps()
    {
        await using var context = Context();
        IReadOnlyList<OVConfigurationItemDefinition> before =
        [
            new OVConfigurationSectionDefinition("source", "Source", [
                new OVConfigurationStepDefinition("source-type", "Type") { Value = "Organisation Unit" }
            ]),
            new OVConfigurationSectionDefinition("target", "Target", [
                new OVConfigurationStepDefinition("target-type", "Type") { DependsOn = ["source-type"] }
            ])
        ];

        var cut = context.Render<OVProgressiveConfigurationBuilder>(p => p.Add(x => x.Items, before));
        Assert.Single(cut.FindAll(".ov-configuration-section__complete"));

        IReadOnlyList<OVConfigurationItemDefinition> after =
        [
            new OVConfigurationSectionDefinition("source", "Source", [
                new OVConfigurationStepDefinition("source-type", "Type") { Value = "Organisation Unit" }
            ]),
            new OVConfigurationSectionDefinition("target", "Target", [
                new OVConfigurationStepDefinition("target-type", "Type") { DependsOn = ["source-type"], Value = "Location" }
            ])
        ];
        cut.Render(p => p.Add(x => x.Items, after));
        Assert.Equal(2, cut.FindAll(".ov-configuration-section__complete").Count);
        Assert.All(cut.FindAll(".ov-configuration-step"), step => Assert.Equal("completed", step.GetAttribute("data-state")));
    }

    [Fact]
    public async Task Existing_child_content_composition_is_still_supported()
    {
        await using var context = Context();
        var cut = context.Render<OVProgressiveConfigurationBuilder>(p => p
            .Add<RenderFragment>(x => x.ChildContent!, builder => builder.AddContent(0, "Existing application builder")));
        Assert.Equal("legacy", cut.Find(".ov-progressive-builder__body").GetAttribute("data-composition"));
        Assert.Contains("Existing application builder", cut.Markup);
    }

    [Fact]
    public async Task Duplicate_keys_fail_explicitly_rather_than_corrupting_progression()
    {
        await using var context = Context();
        IReadOnlyList<OVConfigurationItemDefinition> items =
        [
            new OVConfigurationStepDefinition("same", "First"),
            new OVConfigurationSeparatorDefinition("same", OVConfigurationSeparatorStyle.Divider)
        ];
        Assert.ThrowsAny<Exception>(() => context.Render<OVProgressiveConfigurationBuilder>(p => p.Add(x => x.Items, items)));
    }
}
