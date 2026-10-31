using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class SourceConfigurationBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Standard_details_are_opt_in_and_added_without_exposing_technical_ids()
    {
        await OpenMappingAsync();

        Assert.Equal(0, await Page.GetByRole(AriaRole.Heading, new() { Name = "Standard Details", Exact = true }).CountAsync());

        await Page.GetByRole(AriaRole.Combobox, new() { Name = "Demo state", Exact = true }).SelectOptionAsync("standard-empty");
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Standard Details", Exact = true }).WaitForAsync();
        Assert.True(await Page.GetByText("No Standard Details added to this source.", new() { Exact = true }).IsVisibleAsync());

        await Page.GetByRole(AriaRole.Button, new() { Name = "Add standard detail", Exact = true }).First.ClickAsync();
        var picker = Page.GetByRole(AriaRole.Region, new() { Name = "Available Standard Details", Exact = true });
        await picker.WaitForAsync();
        Assert.True(await picker.GetByText("Email Address", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await picker.GetByText("Contact email address supplied by the source.", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal(0, await picker.GetByText("email-detail", new() { Exact = true }).CountAsync());

        await picker.GetByRole(AriaRole.Checkbox, new() { Name = "Email Address", Exact = true }).CheckAsync();
        await picker.GetByRole(AriaRole.Button, new() { Name = "Add selected", Exact = true }).ClickAsync();
        await Page.GetByText("Mapping required", new() { Exact = true }).WaitForAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Add standard detail", Exact = true }).ClickAsync();
        picker = Page.GetByRole(AriaRole.Region, new() { Name = "Available Standard Details", Exact = true });
        Assert.Equal(0, await picker.GetByRole(AriaRole.Checkbox, new() { Name = "Email Address", Exact = true }).CountAsync());
    }

    [Fact]
    public async Task Read_only_standard_details_preserve_rows_and_suppress_mutation_actions()
    {
        await OpenMappingAsync();
        await Page.GetByRole(AriaRole.Combobox, new() { Name = "Demo state", Exact = true }).SelectOptionAsync("read-only");

        await Page.GetByRole(AriaRole.Heading, new() { Name = "Standard Details", Exact = true }).WaitForAsync();
        Assert.True(await Page.GetByText("Email Address", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await Page.GetByText("Position", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal(0, await Page.GetByRole(AriaRole.Button, new() { Name = "Add standard detail", Exact = true }).CountAsync());
        Assert.Equal(0, await Page.GetByRole(AriaRole.Button, new() { Name = "Remove Email Address standard detail", Exact = true }).CountAsync());
    }

    [Theory]
    [InlineData(1280, 900)]
    [InlineData(760, 900)]
    [InlineData(390, 844)]
    public async Task Standard_details_mapping_reflows_without_horizontal_overflow(int width, int height)
    {
        await Page.SetViewportSizeAsync(width, height);
        await OpenMappingAsync();
        await Page.GetByRole(AriaRole.Combobox, new() { Name = "Demo state", Exact = true }).SelectOptionAsync("standard-selected");
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Standard Details", Exact = true }).WaitForAsync();

        var mapping = Page.Locator(".ov-mapping-pair");
        Assert.True(await mapping.EvaluateAsync<bool>("element => element.scrollWidth <= element.clientWidth + 1"));
        Assert.True(await Page.EvaluateAsync<bool>("document.documentElement.scrollWidth <= window.innerWidth + 1"));
    }

    [Fact]
    public async Task Standard_details_mapping_is_axe_clean()
    {
        await OpenMappingAsync();
        await Page.GetByRole(AriaRole.Combobox, new() { Name = "Demo state", Exact = true }).SelectOptionAsync("standard-selected");
        var mapping = Page.Locator(".ov-mapping-pair");
        var result = await mapping.RunAxe();
        var violations = result.Violations?.ToArray() ?? [];
        Assert.True(violations.Length == 0, string.Join(Environment.NewLine, violations.Select(v => $"{v.Id}: {v.Description}")));
    }

    private async Task OpenMappingAsync()
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/source-configuration");
        await Page.GetByRole(AriaRole.Tab, new() { Name = "Mapping", Exact = true }).ClickAsync();
        await Page.GetByRole(AriaRole.Combobox, new() { Name = "Demo state", Exact = true }).WaitForAsync();
    }
}
