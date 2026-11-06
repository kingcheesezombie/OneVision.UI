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
    public async Task Definition_sections_have_numbered_headers_and_derived_progress_states()
    {
        await using var context = Context();
        IReadOnlyList<OVConfigurationItemDefinition> definition =
        [
            new OVConfigurationSectionDefinition("general", "General", [
                new OVConfigurationStepDefinition("audience", "Audience") { Value = "Standard" }
            ]),
            new OVConfigurationSectionDefinition("delivery", "Delivery", [
                new OVConfigurationStepDefinition("channel", "Channel") { Value = "Email" },
                new OVConfigurationStepDefinition("priority", "Priority") { Editor = b => b.AddContent(0, "Priority editor") }
            ]),
            new OVConfigurationSectionDefinition("control", "Control", [
                new OVConfigurationStepDefinition("approval", "Approval") { DependsOn = ["priority"] }
            ])
        ];
        var cut = context.Render<OVProgressiveConfigurationBuilder>(p => p.Add(x => x.Items, definition));
        var sections = cut.FindAll(".ov-configuration-section");

        Assert.Equal(3, sections.Count);
        Assert.Equal("completed", sections[0].GetAttribute("data-state"));
        Assert.Equal("current", sections[1].GetAttribute("data-state"));
        Assert.Equal("pending", sections[2].GetAttribute("data-state"));
        Assert.Contains("Complete", sections[0].QuerySelector(".ov-configuration-section__status")!.TextContent);
        Assert.Contains("In progress", sections[1].QuerySelector(".ov-configuration-section__status")!.TextContent);
        Assert.Contains("1 of 2 steps complete", sections[1].QuerySelector(".ov-configuration-section__progress")!.TextContent);
        Assert.Contains("02", sections[1].QuerySelector(".ov-configuration-section__number")!.TextContent);
        Assert.Contains("Not started", sections[2].QuerySelector(".ov-configuration-section__status")!.TextContent);
    }

    [Fact]
    public async Task Section_header_marks_step_validation_errors_as_needing_attention()
    {
        await using var context = Context();
        IReadOnlyList<OVConfigurationItemDefinition> definition =
        [
            new OVConfigurationSectionDefinition("settings", "Settings", [
                new OVConfigurationStepDefinition("invalid", "Invalid") {
                    ValidationState = OVConfigurationValidationState.Error,
                    Editor = b => b.AddContent(0, "Edit the selection")
                }
            ])
        ];
        var cut = context.Render<OVProgressiveConfigurationBuilder>(p => p.Add(x => x.Items, definition));
        Assert.Equal("attention", cut.Find(".ov-configuration-section").GetAttribute("data-state"));
        Assert.Contains("Needs attention", cut.Find(".ov-configuration-section__status").TextContent);
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
