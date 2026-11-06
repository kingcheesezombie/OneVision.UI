using System.Reflection;
using Bunit;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class IdentifierConfigurationContractTests
{
    private static readonly IReadOnlyList<OVIdentifierFieldOption> Fields =
    [
        new("record-id", "Record ID"),
        new("description", "Description"),
        new("secondary", "Secondary ID"),
        new("restricted", "Ineligible", DerivationEligible: false),
        new("disabled", "Disabled field", Disabled: true)
    ];

    private static readonly IReadOnlyList<OVIdentifierDerivationStrategyOption> Strategies =
    [
        new("description-hash", "Description-derived identifier"),
        new("composite-hash", "Composite derived identifier", AllowsMultipleFields: true,
            MinimumFields: 2, MaximumFields: 3)
    ];

    private static readonly OVIdentifierCapabilities AllCapabilities = new(true, true);

    private static OVIdentifierDefinition Explicit => new()
    {
        SourceProvidesIdentifier = true,
        IdentifierFieldKey = "record-id",
        FallbackStrategy = OVIdentifierFallbackStrategy.RejectRecord,
        AlternativeFieldKey = "secondary",
        DerivationStrategyKey = "description-hash",
        DerivationFieldKeys = ["description"]
    };

    private static OVIdentifierDefinition Derived => Explicit with { SourceProvidesIdentifier = false };

    private static BunitContext Context()
    {
        var ctx = new BunitContext();
        ctx.Services.AddMudServices();
        ctx.JSInterop.Mode = JSRuntimeMode.Loose;
        return ctx;
    }

    private static IReadOnlyList<OVIdentifierValidationIssue> Check(OVIdentifierDefinition definition,
        OVIdentifierCapabilities? capabilities = null, IReadOnlyList<OVIdentifierFieldOption>? fields = null)
        => OVIdentifierPolicyValidation.Assess(definition, fields ?? Fields, Strategies, capabilities ?? AllCapabilities);

    [Fact]
    public void Stored_identity_policy_has_no_lock_or_runtime_privileges_and_defaults_to_reject()
    {
        var definition = new OVIdentifierDefinition { SourceProvidesIdentifier = true };
        Assert.Equal(OVIdentifierFallbackStrategy.RejectRecord, definition.FallbackStrategy);
        Assert.DoesNotContain(typeof(OVIdentifierDefinition).GetProperties(),
            property => property.Name is "IsLocked" or "LockReason" or "Saving" or "Preview");
        Assert.Equal(OVIdentifierEditState.Unknown, new OVIdentifierConfiguration().EditState);
    }

    [Fact]
    public void Explicit_source_requires_primary_and_valid_fallback()
    {
        Assert.Empty(Check(Explicit));
        Assert.Contains(Check(Explicit with { IdentifierFieldKey = null }), issue => issue.Code == "primary");
        Assert.Contains(Check(Explicit with { IdentifierFieldKey = "removed" }), issue => issue.Code == "primary");
        Assert.Contains(Check(Explicit with { IdentifierFieldKey = "disabled" }), issue => issue.Code == "primary");
        Assert.Contains(Check(Explicit with
        {
            FallbackStrategy = OVIdentifierFallbackStrategy.AlternativeField,
            AlternativeFieldKey = null
        }), issue => issue.Code == "alternative");
        Assert.Contains(Check(Explicit with
        {
            FallbackStrategy = OVIdentifierFallbackStrategy.AlternativeField,
            AlternativeFieldKey = "record-id"
        }), issue => issue.Code == "alternative");
        Assert.Empty(Check(Explicit with { FallbackStrategy = OVIdentifierFallbackStrategy.AlternativeField }));
    }

    [Fact]
    public void Deterministic_source_identity_requires_eligible_strategy_and_inputs()
    {
        Assert.Empty(Check(Derived));
        Assert.Contains(Check(Derived with { DerivationStrategyKey = null }), issue => issue.Code == "strategy");
        Assert.Contains(Check(Derived with { DerivationFieldKeys = [] }), issue => issue.Code == "derivation-fields");
        Assert.Contains(Check(Derived with { DerivationFieldKeys = ["removed"] }), issue => issue.Code == "derivation-fields");
        Assert.Contains(Check(Derived with { DerivationFieldKeys = ["restricted"] }), issue => issue.Code == "derivation-fields");
        Assert.Contains(Check(Derived with { DerivationFieldKeys = ["description", "secondary"] }),
            issue => issue.Code == "derivation-fields");
        Assert.Empty(Check(Derived with
        {
            DerivationStrategyKey = "composite-hash",
            DerivationFieldKeys = ["description", "secondary"]
        }));
        Assert.Contains(Check(Derived with
        {
            DerivationStrategyKey = "composite-hash",
            DerivationFieldKeys = ["description"]
        }), issue => issue.Code == "derivation-fields");
    }

    [Fact]
    public void Deterministic_fallback_requires_explicit_consumer_continuity_permission()
    {
        var fallback = Explicit with { FallbackStrategy = OVIdentifierFallbackStrategy.Deterministic };
        Assert.Contains(Check(fallback, new OVIdentifierCapabilities(true, false)),
            issue => issue.Code == "fallback-policy");
        Assert.Empty(Check(fallback));
        // Inactive fallback draft is not treated as active when the source has no identifier.
        Assert.Empty(Check(Derived with { FallbackStrategy = OVIdentifierFallbackStrategy.Deterministic },
            new OVIdentifierCapabilities(false, false)));
    }

    [Fact]
    public async Task Identifier_toggle_emits_controlled_policy_without_erasing_inactive_drafts()
    {
        await using var context = Context();
        OVIdentifierDefinition? changed = null;
        var cut = context.Render<OVIdentifierConfiguration>(p => p
            .Add(x => x.Definition, Explicit)
            .Add(x => x.DefinitionChanged, (OVIdentifierDefinition value) => changed = value)
            .Add(x => x.EditState, OVIdentifierEditState.Editable)
            .Add(x => x.AvailableFields, Fields)
            .Add(x => x.AvailableStrategies, Strategies)
            .Add(x => x.Capabilities, AllCapabilities));
        Assert.True(cut.FindComponent<OVToggle>().Instance.Value);
        await cut.FindComponent<OVToggle>().Instance.ValueChanged.InvokeAsync(false);
        Assert.NotNull(changed);
        Assert.False(changed!.SourceProvidesIdentifier);
        Assert.Equal("record-id", changed.IdentifierFieldKey);
        Assert.Equal("secondary", changed.AlternativeFieldKey);
        Assert.Equal("description-hash", changed.DerivationStrategyKey);
        Assert.True(cut.FindComponent<OVToggle>().Instance.Value); // caller controls updates
    }

    [Theory]
    [InlineData(OVIdentifierEditState.Locked)]
    [InlineData(OVIdentifierEditState.ReadOnly)]
    [InlineData(OVIdentifierEditState.Unknown)]
    public async Task Locked_readonly_or_unknown_permission_blocks_identity_changes(
        OVIdentifierEditState editState)
    {
        await using var context = Context();
        var callbacks = 0;
        var cut = context.Render<OVIdentifierConfiguration>(p => p
            .Add(x => x.Definition, Explicit)
            .Add(x => x.EditState, editState)
            .Add(x => x.LockReason, "Records already ingested.")
            .Add(x => x.AvailableFields, Fields)
            .Add(x => x.DefinitionChanged, (OVIdentifierDefinition _) => callbacks++));
        Assert.True(cut.FindComponent<OVToggle>().Instance.Disabled);
        await cut.FindComponent<OVToggle>().Instance.ValueChanged.InvokeAsync(false);
        Assert.Equal(0, callbacks);
        Assert.Contains("Record ID", cut.Find(".ov-identifier-configuration").TextContent);
        Assert.NotEqual("editable", cut.Find(".ov-identifier-configuration").GetAttribute("data-edit-state"));
    }

    [Fact]
    public async Task Locked_policy_retains_saved_fields_and_inline_explanation()
    {
        await using var context = Context();
        var cut = context.Render<OVIdentifierConfiguration>(p => p
            .Add(x => x.Definition, Explicit)
            .Add(x => x.EditState, OVIdentifierEditState.Locked)
            .Add(x => x.LockReason, "Source has processed records since 2026.")
            .Add(x => x.AvailableFields, Fields));
        Assert.Contains("Identifier configuration locked", cut.Markup);
        Assert.Contains("Source has processed records since 2026.", cut.Markup);
        Assert.Contains("Record ID", cut.Markup);
        Assert.DoesNotContain("Unlock", cut.Markup);
    }

    [Fact]
    public async Task Stale_profile_and_saving_disable_controls_but_preserve_existing_values()
    {
        await using var context = Context();
        var cut = context.Render<OVIdentifierConfiguration>(p => p
            .Add(x => x.Definition, Explicit)
            .Add(x => x.EditState, OVIdentifierEditState.Editable)
            .Add(x => x.ProfileState, OVSourceProfileState.Stale)
            .Add(x => x.AvailableFields, Fields));
        Assert.True(cut.FindComponent<OVToggle>().Instance.Disabled);
        Assert.Contains("Profile is stale", cut.Markup);
        cut.Render(p => p
            .Add(x => x.Definition, Explicit)
            .Add(x => x.EditState, OVIdentifierEditState.Editable)
            .Add(x => x.ProfileState, OVSourceProfileState.Current)
            .Add(x => x.Saving, true)
            .Add(x => x.AvailableFields, Fields));
        Assert.True(cut.FindComponent<OVToggle>().Instance.Disabled);
        Assert.Contains("Saving identifier configuration", cut.Markup);
        cut.Render(p => p
            .Add(x => x.Definition, Explicit)
            .Add(x => x.EditState, OVIdentifierEditState.Editable)
            .Add(x => x.ProfileState, OVSourceProfileState.Current)
            .Add(x => x.AvailableFields, Fields));
        Assert.False(cut.FindComponent<OVToggle>().Instance.Disabled);
    }

    [Fact]
    public async Task Removed_field_is_inspectable_but_never_substituted()
    {
        await using var context = Context();
        var cut = context.Render<OVIdentifierConfiguration>(p => p
            .Add(x => x.Definition, Explicit with { IdentifierFieldKey = "old-id" })
            .Add(x => x.EditState, OVIdentifierEditState.Editable)
            .Add(x => x.AvailableFields, Fields)
            .Add(x => x.AvailableStrategies, Strategies));
        Assert.Contains("Unresolved saved field: old-id", cut.Markup);
        Assert.Contains("The configured primary identifier field is unavailable or ineligible.", cut.Markup);
        Assert.Equal("old-id", cut.Instance.Definition?.IdentifierFieldKey);
    }

    [Fact]
    public async Task Preview_never_displays_a_result_from_an_outdated_configuration()
    {
        await using var context = Context();
        var cut = context.Render<OVIdentifierConfiguration>(p => p
            .Add(x => x.Definition, Derived)
            .Add(x => x.AvailableFields, Fields)
            .Add(x => x.AvailableStrategies, Strategies)
            .Add(x => x.PreviewConfigurationKey, "revision-2")
            .Add(x => x.Preview, new OVIdentifierPreviewResult("revision-1",
                OVIdentifierPreviewState.Available, "Source value", "SHOULD-NOT-APPEAR")));
        Assert.DoesNotContain("SHOULD-NOT-APPEAR", cut.Markup);
        Assert.Contains("Awaiting current configuration preview", cut.Markup);
        cut.Render(p => p
            .Add(x => x.Definition, Derived)
            .Add(x => x.AvailableFields, Fields)
            .Add(x => x.AvailableStrategies, Strategies)
            .Add(x => x.PreviewConfigurationKey, "revision-2")
            .Add(x => x.Preview, new OVIdentifierPreviewResult("revision-2",
                OVIdentifierPreviewState.Available, "Description", "APP-GENERATED-1234")));
        Assert.Contains("APP-GENERATED-1234", cut.Markup);
        Assert.Single(cut.FindAll(".ov-copyable-link"));
    }

    [Fact]
    public async Task Source_pattern_opt_in_keeps_identifier_inspectable_even_when_mapping_is_stale()
    {
        await using var context = Context();
        var cut = context.Render<OVSourceConfiguration>(p => p
            .Add(x => x.Title, "Source")
            .Add(x => x.ActiveSection, "mapping")
            .Add(x => x.ProfileState, OVSourceProfileState.Stale)
            .Add(x => x.IdentifierConfigurationEnabled, true)
            .Add(x => x.IdentifierDefinition, Explicit)
            .Add(x => x.IdentifierEditState, OVIdentifierEditState.Editable)
            .Add(x => x.IdentifierFields, Fields)
            .Add<RenderFragment>(x => x.Mapping, b => b.AddContent(0, "Application editable mapping contents")));
        Assert.Single(cut.FindComponents<OVIdentifierConfiguration>());
        Assert.Contains("Identifier Configuration", cut.Markup);
        Assert.Contains("The source definition has changed since it was last profiled.", cut.Markup);
        Assert.DoesNotContain("Application editable mapping contents", cut.Markup);
        Assert.True(cut.FindComponent<OVIdentifierConfiguration>().FindComponent<OVToggle>().Instance.Disabled);
    }

    [Fact]
    public async Task Legacy_consumer_has_no_new_identifier_section_or_extra_tab()
    {
        await using var context = Context();
        var cut = context.Render<OVSourceConfiguration>(p => p
            .Add(x => x.Title, "Source")
            .Add(x => x.ActiveSection, "mapping")
            .Add(x => x.ProfileState, OVSourceProfileState.Current)
            .Add<RenderFragment>(x => x.Mapping, b => b.AddContent(0, "Existing mapping unchanged")));
        Assert.False(cut.Instance.IdentifierConfigurationEnabled);
        Assert.Empty(cut.FindComponents<OVIdentifierConfiguration>());
        Assert.Contains("Existing mapping unchanged", cut.Markup);
        Assert.DoesNotContain("Identifier Configuration", cut.Markup);
        Assert.Equal(0, cut.FindAll("[role='tab']").Count(tab => tab.TextContent.Contains("Identifier")));
    }
}
