using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class SourceConfigurationSaveStateBrowserTests : PageTest
{
    private static string BaseUrl =>
        Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Saving_locks_display_value_without_removing_existing_composition_then_restores_editability()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/source-configuration");
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("saving");
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Display Value Builder" }).CheckAsync();

        var preview = Page.GetByRole(AriaRole.Region, new() { Name = "Live pattern preview" });
        var editor = preview.Locator(".ov-display-value-builder");
        await editor.WaitForAsync();
        Assert.Equal("true", await preview.Locator(".ov-source-configuration").GetAttributeAsync("aria-busy"));
        Assert.True(await editor.GetByText("Read only", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal(0, await editor.GetByRole(AriaRole.Button, new() { Name = "Add mapped field", Exact = false }).CountAsync());

        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("current");
        Assert.True(await editor.GetByText("Available mapped fields", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal(0, await editor.GetByText("Read only", new() { Exact = true }).CountAsync());
        Assert.NotEqual("true", await preview.Locator(".ov-source-configuration").GetAttributeAsync("aria-busy"));
    }

    [Fact]
    public async Task Save_error_keeps_editable_display_value_and_the_retained_error()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/source-configuration");
        await Page.Locator("#ov-pattern-scenario").SelectOptionAsync("save-error");
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Display Value Builder" }).CheckAsync();

        var preview = Page.GetByRole(AriaRole.Region, new() { Name = "Live pattern preview" });
        await preview.Locator(".ov-display-value-builder").WaitForAsync();
        Assert.Equal(0, await preview.GetByText("Read only", new() { Exact = true }).CountAsync());
        Assert.True(await preview.GetByText("Could not save this source.", new() { Exact = false }).IsVisibleAsync());
    }
}
