using System.Globalization;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class RenderedInspectorTests
{
    [Fact]
    public async Task Overlay_renders_labelled_described_busy_dialog_and_governed_escape_close()
    {
        await using var context = CreateContext();
        bool? open = null;
        var cut = context.Render<OVOverlay>(parameters => parameters
            .Add(component => component.Open, true)
            .Add(component => component.Title, "Entity details")
            .Add(component => component.Description, "Reference EX-42")
            .Add(component => component.Busy, false)
            .Add(component => component.OpenChanged, value => open = value)
            .Add(component => component.ChildContent, ButtonFragment("Inside")));

        var dialog = cut.Find("[role='dialog']");
        var labelledBy = dialog.GetAttribute("aria-labelledby");
        var describedBy = dialog.GetAttribute("aria-describedby");
        Assert.False(string.IsNullOrWhiteSpace(labelledBy));
        Assert.False(string.IsNullOrWhiteSpace(describedBy));
        Assert.Equal("Entity details", cut.Find($"#{labelledBy}").TextContent);
        Assert.Equal("Reference EX-42", cut.Find($"#{describedBy}").TextContent);
        Assert.Null(dialog.GetAttribute("aria-busy"));

        await cut.InvokeAsync(cut.Instance.HandleEscapeFromJs);
        Assert.False(open);
    }

    [Fact]
    public async Task Overlay_backdrop_busy_close_disabled_and_dirty_use_the_same_close_contract()
    {
        await using var context = CreateContext();
        var closeCount = 0;
        var cut = context.Render<OVOverlay>(parameters => parameters
            .Add(component => component.Open, true)
            .Add(component => component.Title, "Details")
            .Add(component => component.OpenChanged, _ => closeCount++)
            .Add(component => component.ChildContent, TextFragment("Content")));

        await cut.Find(".ov-overlay__backdrop").ClickAsync();
        Assert.Equal(1, closeCount);

        cut.Render(parameters => parameters
            .Add(component => component.Open, true)
            .Add(component => component.Busy, true));
        Assert.Equal("true", cut.Find("[role='dialog']").GetAttribute("aria-busy"));
        await cut.InvokeAsync(cut.Instance.HandleEscapeFromJs);
        Assert.Equal(1, closeCount);

        cut.Render(parameters => parameters
            .Add(component => component.Busy, false)
            .Add(component => component.CloseDisabled, true));
        await cut.InvokeAsync(cut.Instance.HandleEscapeFromJs);
        Assert.Equal(1, closeCount);

        cut.Render(parameters => parameters
            .Add(component => component.CloseDisabled, false)
            .Add(component => component.CloseOnBackdrop, false));
        await cut.Find(".ov-overlay__backdrop").ClickAsync();
        Assert.Equal(1, closeCount);

        cut.Render(parameters => parameters
            .Add(component => component.CloseOnBackdrop, true)
            .Add(component => component.Dirty, true));
        await cut.InvokeAsync(cut.Instance.HandleEscapeFromJs);
        Assert.Equal(1, closeCount);
        Assert.Contains("Discard changes?", cut.Markup, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(true, false, false, false, "Loading entity details", "loaded-content")]
    [InlineData(false, true, false, false, "initial error", "loaded-content")]
    [InlineData(false, false, true, false, "No details available", "loaded-content")]
    [InlineData(false, false, false, false, "loaded-content", null)]
    public async Task Inspector_enforces_state_precedence(bool loading, bool error, bool empty, bool refreshing, string expected, string? absent)
    {
        await using var context = CreateContext();
        var cut = context.Render<OVEntityInspector>(parameters => parameters
            .Add(component => component.Open, true)
            .Add(component => component.Title, "Entity")
            .Add(component => component.Loading, loading)
            .Add(component => component.ErrorMessage, error ? "initial error" : null)
            .Add(component => component.Empty, empty)
            .Add(component => component.Refreshing, refreshing)
            .Add(component => component.Sections, TextFragment("loaded-content")));

        Assert.Contains(expected, cut.Markup, StringComparison.OrdinalIgnoreCase);
        if (absent is not null) Assert.DoesNotContain(absent, cut.Markup, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Inspector_refresh_keeps_loaded_content_visible_and_surfaces_recoverable_error()
    {
        await using var context = CreateContext();
        var cut = context.Render<OVEntityInspector>(parameters => parameters
            .Add(component => component.Open, true)
            .Add(component => component.Title, "Entity")
            .Add(component => component.Refreshing, true)
            .Add(component => component.RefreshErrorMessage, "Could not refresh entity details.")
            .Add(component => component.Sections, TextFragment("current metadata")));

        Assert.Contains("current metadata", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("Could not refresh entity details.", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("Refreshing entity details", cut.Markup, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Inspector_renders_workspace_and_governed_action_hierarchy()
    {
        await using var context = CreateContext();
        var cut = context.Render<OVEntityInspector>(parameters => parameters
            .Add(component => component.Open, true)
            .Add(component => component.Title, "Entity")
            .Add(component => component.Placement, OVOverlayPlacement.Workspace)
            .Add(component => component.SecondaryActions, ButtonFragment("Secondary"))
            .Add(component => component.PrimaryAction, ButtonFragment("Primary"))
            .Add(component => component.OverflowActions, new[] { new OVOverflowMenuItem("delete", "Delete") }));

        Assert.NotNull(cut.Find(".ov-overlay--workspace"));
        var markup = cut.Markup;
        Assert.True(markup.IndexOf("Secondary", StringComparison.Ordinal) < markup.IndexOf("Primary", StringComparison.Ordinal));
        Assert.Contains("More entity actions", markup, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Metadata_list_preserves_dl_semantics_and_value_kind_presentation()
    {
        await using var context = CreateContext();
        var timestamp = new DateTimeOffset(2026, 10, 28, 20, 30, 0, TimeSpan.Zero);
        var items = new[]
        {
            new OVMetadataItem("Name", "Example", "Identity", SecondaryText: "Public-safe example"),
            new OVMetadataItem("Missing", null, "Identity"),
            new OVMetadataItem("Enabled", null, "State", OVMetadataValueKind.Boolean, RawValue: true),
            new OVMetadataItem("Count", null, "State", OVMetadataValueKind.Number, RawValue: 12480),
            new OVMetadataItem("Refreshed", null, "State", OVMetadataValueKind.DateTime, RawValue: timestamp),
            new OVMetadataItem("Payload", "{\n  \"state\": \"ready\"\n}", "Technical", OVMetadataValueKind.Structured, Copyable: true)
        };

        var cut = context.Render<OVMetadataList>(parameters => parameters.Add(component => component.Items, items));

        Assert.Equal(items.Length, cut.FindAll("dt").Count);
        Assert.Equal(items.Length, cut.FindAll("dd").Count);
        Assert.Contains("Identity", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("Public-safe example", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("—", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("Yes", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("ov-metadata-list__value--number", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("ov-metadata-list__value--datetime", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("ov-metadata-list__value--structured", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("ov-metadata-list__value--mono", cut.Markup, StringComparison.Ordinal);
        Assert.NotEmpty(cut.FindAll("pre"));
        Assert.Contains("Copy", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Duplicate_metadata_labels_expand_independently_and_expose_disclosure_state()
    {
        await using var context = CreateContext();
        var items = new[]
        {
            new OVMetadataItem("Payload", LongStructured("first"), "Technical", OVMetadataValueKind.Structured),
            new OVMetadataItem("Payload", LongStructured("second"), "Technical", OVMetadataValueKind.Structured)
        };
        var cut = context.Render<OVMetadataList>(parameters => parameters.Add(component => component.Items, items));

        var toggles = ShowMoreButtons(cut);
        Assert.Equal(2, toggles.Count);
        Assert.All(toggles, button => Assert.Equal("false", button.GetAttribute("aria-expanded")));
        var firstControls = toggles[0].GetAttribute("aria-controls");
        var secondControls = toggles[1].GetAttribute("aria-controls");
        Assert.NotEqual(firstControls, secondControls);
        Assert.NotNull(cut.Find($"#{firstControls}"));
        Assert.NotNull(cut.Find($"#{secondControls}"));

        await toggles[0].ClickAsync();
        var refreshed = cut.FindAll("button").Where(button => button.TextContent.Contains("Show ", StringComparison.Ordinal)).ToArray();
        Assert.Equal("true", refreshed[0].GetAttribute("aria-expanded"));
        Assert.Equal("false", refreshed[1].GetAttribute("aria-expanded"));
        Assert.Equal(firstControls, refreshed[0].GetAttribute("aria-controls"));
    }

    [Fact]
    public async Task Explicit_metadata_key_preserves_expansion_across_rerender_and_stale_keys_are_pruned()
    {
        await using var context = CreateContext();
        var cut = context.Render<OVMetadataList>(parameters => parameters.Add(component => component.Items,
            new[] { new OVMetadataItem("Payload", LongStructured("one"), Kind: OVMetadataValueKind.Structured, Key: "payload-a") }));

        await ShowMoreButtons(cut).Single().ClickAsync();
        Assert.Equal("true", cut.Find("button[aria-expanded]").GetAttribute("aria-expanded"));

        cut.Render(parameters => parameters.Add(component => component.Items,
            new[] { new OVMetadataItem("Payload refreshed", LongStructured("two"), Kind: OVMetadataValueKind.Structured, Key: "payload-a") }));
        Assert.Equal("true", cut.Find("button[aria-expanded]").GetAttribute("aria-expanded"));

        cut.Render(parameters => parameters.Add(component => component.Items,
            new[] { new OVMetadataItem("Other", LongStructured("other"), Kind: OVMetadataValueKind.Structured, Key: "payload-b") }));
        cut.Render(parameters => parameters.Add(component => component.Items,
            new[] { new OVMetadataItem("Payload again", LongStructured("three"), Kind: OVMetadataValueKind.Structured, Key: "payload-a") }));
        Assert.Equal("false", cut.Find("button[aria-expanded]").GetAttribute("aria-expanded"));
    }

    [Fact]
    public async Task Number_format_hints_are_optional_safe_and_value_override_wins()
    {
        await using var context = CreateContext();
        var previous = CultureInfo.CurrentCulture;
        var culture = CultureInfo.GetCultureInfo("en-GB");
        CultureInfo.CurrentCulture = culture;
        try
        {
            var items = new[]
            {
                new OVMetadataItem("N0", null, Kind: OVMetadataValueKind.Number, RawValue: 12480, Format: "N0"),
                new OVMetadataItem("N2", null, Kind: OVMetadataValueKind.Number, RawValue: 12480, Format: "N2"),
                new OVMetadataItem("P1", null, Kind: OVMetadataValueKind.Number, RawValue: .125m, Format: "P1"),
                new OVMetadataItem("Invalid", null, Kind: OVMetadataValueKind.Number, RawValue: 12480, Format: "Q"),
                new OVMetadataItem("Default", null, Kind: OVMetadataValueKind.Number, RawValue: 12480),
                new OVMetadataItem("Override", "Application formatted", Kind: OVMetadataValueKind.Number, RawValue: 12480, Format: "N0")
            };
            var cut = context.Render<OVMetadataList>(parameters => parameters.Add(component => component.Items, items));
            var values = cut.FindAll("dd").Select(value => value.TextContent.Trim()).ToArray();

            Assert.Equal(12480.ToString("N0", culture), values[0]);
            Assert.Equal(12480.ToString("N2", culture), values[1]);
            Assert.Equal(.125m.ToString("P1", culture), values[2]);
            Assert.Equal(12480.ToString(null, culture), values[3]);
            Assert.Equal(12480.ToString(null, culture), values[4]);
            Assert.Equal("Application formatted", values[5]);
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public async Task Metadata_empty_message_is_explicit_and_opt_in()
    {
        await using var context = CreateContext();
        var cut = context.Render<OVMetadataList>(parameters => parameters
            .Add(component => component.Items, Array.Empty<OVMetadataItem>()));
        Assert.Equal(string.Empty, cut.Markup);

        cut.Render(parameters => parameters
            .Add(component => component.Items, Array.Empty<OVMetadataItem>())
            .Add(component => component.ShowEmptyState, true)
            .Add(component => component.EmptyMessage, "No diagnostic metadata available."));
        Assert.Contains("No diagnostic metadata available.", cut.Markup, StringComparison.Ordinal);
        Assert.NotNull(cut.Find(".ov-metadata-list__empty"));
    }

    private static IReadOnlyList<AngleSharp.Dom.IElement> ShowMoreButtons(IRenderedComponent<OVMetadataList> cut)
        => cut.FindAll("button").Where(button => button.TextContent.Contains("Show more", StringComparison.Ordinal)).ToArray();

    private static string LongStructured(string prefix)
        => string.Join('\n', Enumerable.Range(1, 12).Select(index => $"{prefix}-{index}"));

    private static BunitContext CreateContext()
    {
        var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        var module = context.JSInterop.SetupModule("./_content/OneVision.UI/ov-overlay.js");
        module.Mode = JSRuntimeMode.Loose;
        return context;
    }

    private static RenderFragment TextFragment(string text) => builder => builder.AddContent(0, text);

    private static RenderFragment ButtonFragment(string text) => builder =>
    {
        builder.OpenElement(0, "button");
        builder.AddContent(1, text);
        builder.CloseElement();
    };
}
