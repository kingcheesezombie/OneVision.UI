using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class ComponentCapabilityWorkbenchBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Input_component_modifiers_combine_with_selected_governed_state()
    {
        await Page.GotoAsync($"{BaseUrl}/components/text-field");
        var disabled = Page.GetByRole(AriaRole.Checkbox, new() { Name = "Disabled" });
        var validation = Page.GetByRole(AriaRole.Checkbox, new() { Name = "Validation error" });
        Assert.True(await disabled.IsVisibleAsync());
        Assert.True(await validation.IsVisibleAsync());
        await validation.CheckAsync();
        Assert.True(await Page.GetByText("Enter a valid name.", new() { Exact = true }).IsVisibleAsync());
        await disabled.CheckAsync();
        Assert.True(await Page.GetByRole(AriaRole.Textbox, new() { Name = "Name", Exact = true }).IsDisabledAsync());
        await disabled.UncheckAsync();
        Assert.True(await Page.GetByRole(AriaRole.Textbox, new() { Name = "Name", Exact = true }).IsEnabledAsync());
    }

    [Fact]
    public async Task Mapping_pair_supports_combining_constants_and_read_only_with_its_state()
    {
        await Page.GotoAsync($"{BaseUrl}/components/mapping-pair");
        var constants = Page.GetByRole(AriaRole.Checkbox, new() { Name = "Allow constants" });
        var standard = Page.GetByRole(AriaRole.Checkbox, new() { Name = "Standard Details" });
        Assert.True(await standard.IsCheckedAsync());
        Assert.False(await constants.IsCheckedAsync());
        await constants.CheckAsync();
        Assert.True(await constants.IsCheckedAsync());
        await standard.UncheckAsync();
        Assert.False(await standard.IsCheckedAsync());
        Assert.True(await Page.Locator(".ov-mapping-pair").IsVisibleAsync());
    }

    [Fact]
    public async Task Components_without_independent_modifiers_only_show_supported_states()
    {
        await Page.GotoAsync($"{BaseUrl}/components/pill");
        Assert.Equal(0, await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Disabled" }).CountAsync());
        Assert.True(await Page.Locator(".ov-component-workbench__preview").IsVisibleAsync());
    }
}
