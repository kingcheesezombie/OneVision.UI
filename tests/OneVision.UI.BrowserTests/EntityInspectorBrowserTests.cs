using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class EntityInspectorBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Right_side_overlay_traps_focus_closes_on_escape_restores_trigger_and_is_axe_clean()
    {
        await OpenPatternAsync();
        var trigger = Page.GetByRole(AriaRole.Button, new() { Name = "Open right-side inspector" });
        await trigger.FocusAsync();
        await trigger.ClickAsync();

        var dialog = Page.GetByRole(AriaRole.Dialog);
        await dialog.WaitForAsync();
        Assert.Equal("true", await dialog.GetAttributeAsync("aria-modal"));
        Assert.False(string.IsNullOrWhiteSpace(await dialog.GetAttributeAsync("aria-labelledby")));
        Assert.False(string.IsNullOrWhiteSpace(await dialog.GetAttributeAsync("aria-describedby")));
        Assert.Equal("hidden", await Page.EvaluateAsync<string>("document.body.style.overflow"));
        Assert.True(await Page.EvaluateAsync<bool>("document.querySelector('[role=dialog]').contains(document.activeElement)"));
        Assert.False(await Page.EvaluateAsync<bool>("document.activeElement?.classList.contains('ov-overlay__close')"));

        await AssertFocusTrapAsync(dialog);
        await AssertAxeCleanAsync(dialog);

        await Page.Keyboard.PressAsync("Escape");
        await dialog.WaitForAsync(new() { State = WaitForSelectorState.Detached });
        Assert.NotEqual("hidden", await Page.EvaluateAsync<string>("document.body.style.overflow"));
        Assert.True(await Page.EvaluateAsync<bool>("document.activeElement?.textContent?.includes('Open right-side inspector')"));
    }

    [Fact]
    public async Task Workspace_overlay_has_the_same_keyboard_and_accessibility_contract()
    {
        await OpenPatternAsync();
        var trigger = Page.GetByRole(AriaRole.Button, new() { Name = "Open workspace inspector" });
        await trigger.FocusAsync();
        await trigger.ClickAsync();

        var dialog = Page.GetByRole(AriaRole.Dialog);
        await dialog.WaitForAsync();
        Assert.Equal(1, await dialog.Locator(".ov-entity-inspector__content").CountAsync());
        Assert.True(await dialog.EvaluateAsync<bool>("element => element.classList.contains('ov-overlay--workspace')"));
        await AssertFocusTrapAsync(dialog);
        await AssertAxeCleanAsync(dialog);

        await Page.Keyboard.PressAsync("Escape");
        await dialog.WaitForAsync(new() { State = WaitForSelectorState.Detached });
        Assert.True(await Page.EvaluateAsync<bool>("document.activeElement?.textContent?.includes('Open workspace inspector')"));
    }

    [Fact]
    public async Task Nested_overlay_escape_closes_only_topmost_preserves_scroll_lock_and_restores_focus_through_stack()
    {
        await OpenPatternAsync();
        var outerTrigger = Page.GetByRole(AriaRole.Button, new() { Name = "Open right-side inspector" });
        await outerTrigger.FocusAsync();
        await outerTrigger.ClickAsync();

        var inspector = Page.GetByRole(AriaRole.Dialog);
        await inspector.WaitForAsync();
        var nestedTrigger = inspector.GetByRole(AriaRole.Button, new() { Name = "Open nested confirmation" });
        await nestedTrigger.FocusAsync();
        await nestedTrigger.ClickAsync();

        var dialogs = Page.GetByRole(AriaRole.Dialog);
        Assert.Equal(2, await dialogs.CountAsync());
        var nested = dialogs.Nth(1);
        await nested.WaitForAsync();
        Assert.Equal("hidden", await Page.EvaluateAsync<string>("document.body.style.overflow"));
        Assert.True(await nested.EvaluateAsync<bool>("element => element.contains(document.activeElement)"));

        await Page.Keyboard.PressAsync("Escape");
        await nested.WaitForAsync(new() { State = WaitForSelectorState.Detached });
        Assert.Equal(1, await Page.GetByRole(AriaRole.Dialog).CountAsync());
        Assert.True(await inspector.IsVisibleAsync());
        Assert.Equal("hidden", await Page.EvaluateAsync<string>("document.body.style.overflow"));
        Assert.True(await Page.EvaluateAsync<bool>("document.activeElement?.textContent?.includes('Open nested confirmation')"));

        await Page.Keyboard.PressAsync("Escape");
        await inspector.WaitForAsync(new() { State = WaitForSelectorState.Detached });
        Assert.NotEqual("hidden", await Page.EvaluateAsync<string>("document.body.style.overflow"));
        Assert.True(await Page.EvaluateAsync<bool>("document.activeElement?.textContent?.includes('Open right-side inspector')"));
    }

    [Fact]
    public async Task Structured_metadata_disclosure_exposes_stable_programmatic_state()
    {
        await OpenPatternAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Large structured payload", Exact = true }).ClickAsync();
        var dialog = Page.GetByRole(AriaRole.Dialog);
        await dialog.WaitForAsync();
        var toggle = dialog.GetByRole(AriaRole.Button, new() { Name = "Show more", Exact = true });
        await toggle.WaitForAsync();

        Assert.Equal("false", await toggle.GetAttributeAsync("aria-expanded"));
        var controls = await toggle.GetAttributeAsync("aria-controls");
        Assert.False(string.IsNullOrWhiteSpace(controls));
        Assert.Equal(1, await dialog.Locator($"#{controls}").CountAsync());

        await toggle.ClickAsync();
        var expanded = dialog.GetByRole(AriaRole.Button, new() { Name = "Show less", Exact = true });
        Assert.Equal("true", await expanded.GetAttributeAsync("aria-expanded"));
        Assert.Equal(controls, await expanded.GetAttributeAsync("aria-controls"));
    }

    [Theory]
    [InlineData("Loading")]
    [InlineData("Error")]
    public async Task Loading_and_error_states_remain_accessible(string state)
    {
        await OpenPatternAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = state, Exact = true }).ClickAsync();
        var dialog = Page.GetByRole(AriaRole.Dialog);
        await dialog.WaitForAsync();
        await AssertAxeCleanAsync(dialog);

        if (state == "Loading")
            Assert.True(await dialog.GetByLabel("Loading entity details").IsVisibleAsync());
        else
            Assert.True(await dialog.GetByText("Entity details could not be loaded.").IsVisibleAsync());
    }

    [Theory]
    [InlineData(1920, 1080)]
    [InlineData(1366, 768)]
    [InlineData(1024, 768)]
    [InlineData(768, 1024)]
    [InlineData(390, 844)]
    public async Task Right_side_overlay_and_stress_states_do_not_create_viewport_overflow(int width, int height)
    {
        await Page.SetViewportSizeAsync(width, height);
        await OpenPatternAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Open right-side inspector" }).ClickAsync();

        var dialog = Page.GetByRole(AriaRole.Dialog);
        await AssertOverlayBoundsAsync(dialog, width, height);
        await AssertNoHorizontalOverflowAsync(dialog);
        await Page.Keyboard.PressAsync("Escape");

        foreach (var state in new[] { "Long header", "Large structured payload", "Dense action footer" })
        {
            await Page.GetByRole(AriaRole.Button, new() { Name = state, Exact = true }).ClickAsync();
            dialog = Page.GetByRole(AriaRole.Dialog);
            await dialog.WaitForAsync();
            await AssertOverlayBoundsAsync(dialog, width, height);
            await AssertNoHorizontalOverflowAsync(dialog);

            if (state == "Long header")
            {
                var close = dialog.GetByRole(AriaRole.Button, new() { Name = "Close overlay" });
                Assert.True(await close.IsVisibleAsync());
                var closeBox = await close.BoundingBoxAsync();
                Assert.NotNull(closeBox);
                Assert.True(closeBox!.X + closeBox.Width <= width + 1);
            }
            else if (state == "Dense action footer")
            {
                Assert.True(await dialog.GetByRole(AriaRole.Button, new() { Name = "Primary action" }).IsVisibleAsync());
                Assert.True(await dialog.GetByRole(AriaRole.Button, new() { Name = "More entity actions" }).IsVisibleAsync());
            }

            await Page.Keyboard.PressAsync("Escape");
            await dialog.WaitForAsync(new() { State = WaitForSelectorState.Detached });
        }
    }

    private async Task OpenPatternAsync()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/entity-inspector", new() { WaitUntil = WaitUntilState.NetworkIdle });
        await Page.GetByText("Entity Inspector", new() { Exact = true }).First.WaitForAsync();
    }

    private async Task AssertFocusTrapAsync(ILocator dialog)
    {
        const string selector = "a[href],button:not([disabled]),input:not([disabled]):not([type=hidden]),select:not([disabled]),textarea:not([disabled]),[tabindex]:not([tabindex='-1']),[contenteditable=true]";
        var focusables = dialog.Locator(selector);
        var count = await focusables.CountAsync();
        Assert.True(count >= 2);

        await focusables.Nth(count - 1).FocusAsync();
        await Page.Keyboard.PressAsync("Tab");
        Assert.True(await dialog.EvaluateAsync<bool>("element => element.contains(document.activeElement)"));
        Assert.True(await focusables.Nth(0).EvaluateAsync<bool>("element => element === document.activeElement"));

        await focusables.Nth(0).FocusAsync();
        await Page.Keyboard.PressAsync("Shift+Tab");
        Assert.True(await focusables.Nth(count - 1).EvaluateAsync<bool>("element => element === document.activeElement"));
    }

    private async Task AssertOverlayBoundsAsync(ILocator dialog, int width, int height)
    {
        await dialog.WaitForAsync();
        var box = await dialog.BoundingBoxAsync();
        Assert.NotNull(box);
        Assert.True(box!.Width <= width + 1);
        Assert.True(box.Height <= height + 1);

        if (width <= 640)
            Assert.True(box.Width >= width - 2);
        else
            Assert.True(box.Width <= 602);
    }

    private async Task AssertNoHorizontalOverflowAsync(ILocator dialog)
    {
        Assert.True(await Page.EvaluateAsync<bool>("document.documentElement.scrollWidth <= window.innerWidth"));
        Assert.True(await dialog.EvaluateAsync<bool>("element => element.scrollWidth <= element.clientWidth"));
    }

    private static async Task AssertAxeCleanAsync(ILocator dialog)
    {
        var result = await dialog.RunAxe();
        var violations = result.Violations?.ToArray() ?? [];
        Assert.True(violations.Length == 0,
            string.Join(Environment.NewLine, violations.Select(v => $"{v.Id}: {v.Help}")));
    }
}
