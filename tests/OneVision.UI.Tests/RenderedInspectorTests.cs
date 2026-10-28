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
