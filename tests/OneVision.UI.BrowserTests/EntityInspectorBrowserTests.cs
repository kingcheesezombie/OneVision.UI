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
        Assert.True(await Page.EvaluateAsync<bool>("document.querySelector('[role=dialog]').contains(document.activeElement)"));
        Assert.False(await Page.EvaluateAsync<bool>("document.activeElement?.classList.contains('ov-overlay__close')"));

        await AssertFocusTrapAsync(dialog);
        await AssertAxeCleanAsync(dialog);

        await Page.Keyboard.PressAsync("Escape");
        await dialog.WaitForAsync(new() { State = WaitForSelectorState.Detached });
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
    public async Task Right_side_overlay_is_usable_at_governed_viewports(int width, int height)
    {
        await Page.SetViewportSizeAsync(width, height);
        await OpenPatternAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Open right-side inspector" }).ClickAsync();

        var dialog = Page.GetByRole(AriaRole.Dialog);
        var box = await dialog.BoundingBoxAsync();
        Assert.NotNull(box);
        Assert.True(box!.Width <= width + 1);
        Assert.True(box.Height <= height + 1);

        if (width <= 640)
            Assert.True(box.Width >= width - 2);
        else
            Assert.True(box.Width <= 602);
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

    private static async Task AssertAxeCleanAsync(ILocator dialog)
    {
        var result = await dialog.RunAxe();
        Assert.True(result.Violations is null || result.Violations.Count == 0,
            result.Violations is null ? string.Empty : string.Join(Environment.NewLine, result.Violations.Select(v => $"{v.Id}: {v.Help}")));
    }
}
