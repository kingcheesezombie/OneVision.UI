using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class SourceConfigurationGalleryActionsBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Current_source_save_and_cancel_are_functional_reference_actions()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/source-configuration");
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("current");

        var preview = Page.GetByRole(AriaRole.Region, new() { Name = "Live pattern preview" });
        var save = preview.GetByRole(AriaRole.Button, new() { Name = "Save source", Exact = true });
        Assert.True(await save.IsEnabledAsync());

        await save.ClickAsync();
        Assert.True(await preview.GetByText("Example source saved locally in this gallery preview. No external data was changed.", new() { Exact = true }).IsVisibleAsync());

        await preview.GetByRole(AriaRole.Button, new() { Name = "Cancel", Exact = true }).ClickAsync();
        Assert.True(await preview.GetByText("Example edits discarded. No external data was changed.", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal(0, await preview.GetByText("Example source saved locally in this gallery preview. No external data was changed.", new() { Exact = true }).CountAsync());
    }

    [Theory]
    [InlineData("read-only")]
    [InlineData("saving")]
    public async Task Read_only_or_busy_source_cannot_be_saved_from_gallery(string scenario)
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/source-configuration");
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync(scenario);

        var preview = Page.GetByRole(AriaRole.Region, new() { Name = "Live pattern preview" });
        var save = preview.GetByRole(AriaRole.Button, new() { Name = "Save source", Exact = true });
        Assert.False(await save.IsEnabledAsync());
    }
}
