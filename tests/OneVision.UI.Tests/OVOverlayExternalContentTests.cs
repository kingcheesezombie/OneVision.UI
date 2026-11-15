using Bunit;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class OVOverlayExternalContentTests : BunitContext
{
    public OVOverlayExternalContentTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Native_mode_remains_default_and_uses_child_content()
    {
        var cut = Render<OVOverlay>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Title, "Native")
            .AddChildContent("<p>Native body</p>"));

        Assert.Contains("Native body", cut.Markup);
        Assert.DoesNotContain("<iframe", cut.Markup);
    }

    [Fact]
    public void External_only_overlay_renders_approved_iframe_without_child_content()
    {
        var cut = Render<OVOverlay>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Title, "Viewer")
            .Add(x => x.ContentMode, OVOverlayContentMode.External)
            .Add(x => x.ExternalContent, new OVOverlayExternalContent
            {
                Url = "https://viewer.example.org/item/123",
                PermittedOrigins = new[] { "https://viewer.example.org" },
                FrameTitle = "Item detail"
            }));

        Assert.Contains("<iframe", cut.Markup);
        Assert.Contains("title=\"Item detail\"", cut.Markup);
        Assert.Contains("https://viewer.example.org/item/123", cut.Markup);
    }

    [Fact]
    public void External_overlay_rejects_unapproved_destination()
    {
        var cut = Render<OVOverlay>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Title, "Viewer")
            .Add(x => x.ContentMode, OVOverlayContentMode.External)
            .Add(x => x.ExternalContent, new OVOverlayExternalContent
            {
                Url = "javascript:alert(1)",
                PermittedOrigins = new[] { "https://viewer.example.org" }
            }));

        Assert.DoesNotContain("<iframe", cut.Markup);
        Assert.Contains("not permitted", cut.Markup);
    }
}
