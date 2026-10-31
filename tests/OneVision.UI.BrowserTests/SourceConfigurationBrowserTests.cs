using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class SourceConfigurationBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Standard_details_add_as_draft_then_commit_to_compact_rows()
    {
        await OpenMappingAsync();

        Assert.Equal(0, await Page.GetByText("Standard Details", new() { Exact = true }).CountAsync());

        await ChooseDemoStateAsync("Standard Details enabled · empty");
        await Page.GetByText("Standard Details", new() { Exact = true }).WaitForAsync();
        Assert.True(await Page.GetByText("No Standard Details added to this source.", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal(1, await Page.GetByRole(AriaRole.Button, new() { Name = "Add standard detail", Exact = true }).CountAsync());

        await Page.GetByRole(AriaRole.Button, new() { Name = "Add standard detail", Exact = true }).ClickAsync();
        var draft = Page.Locator(".ov-mapping-pair__target-row--draft");
        await draft.WaitForAsync();
        Assert.Equal(1, await Page.Locator(".ov-mapping-pair__target-row--draft").CountAsync());
        Assert.True(await draft.GetByText("Choose a Standard Detail to enable mapping", new() { Exact = true }).IsVisibleAsync());

        var selector = draft.GetByRole(AriaRole.Combobox, new() { Name = "Standard Detail", Exact = true });
        await selector.ClickAsync();
        Assert.Equal(0, await Page.GetByText("email-detail", new() { Exact = true }).CountAsync());
        await Page.GetByText("Email Address", new() { Exact = true }).ClickAsync();

        var committed = Page.GetByRole(AriaRole.Listitem, new() { Name = "Standard Detail Email Address", Exact = true });
        await committed.WaitForAsync();
        Assert.True(await committed.GetByText("Email Address", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await committed.GetByText("Contact email address supplied by the source.", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await committed.GetByText("Mapping required", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await committed.GetByText("Drop source field here", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal(0, await committed.GetByRole(AriaRole.Combobox, new() { Name = "Standard Detail", Exact = true }).CountAsync());
        Assert.True(await committed.GetByRole(AriaRole.Button, new() { Name = "Change Email Address standard detail", Exact = true }).IsVisibleAsync());
        Assert.True(await committed.GetByRole(AriaRole.Button, new() { Name = "Remove Email Address standard detail", Exact = true }).IsVisibleAsync());
    }

    [Fact]
    public async Task Change_exposes_current_and_eligible_choices_without_duplicate_catalogue_rows()
    {
        await OpenMappingAsync();
        await ChooseDemoStateAsync("Standard Details enabled · selected");

        var emailRow = Page.GetByRole(AriaRole.Listitem, new() { Name = "Standard Detail Email Address", Exact = true });
        await emailRow.WaitForAsync();
        Assert.Equal(0, await emailRow.GetByRole(AriaRole.Combobox, new() { Name = "Standard Detail", Exact = true }).CountAsync());

        await emailRow.GetByRole(AriaRole.Button, new() { Name = "Change Email Address standard detail", Exact = true }).ClickAsync();
        var selector = emailRow.GetByRole(AriaRole.Combobox, new() { Name = "Standard Detail", Exact = true });
        await selector.WaitForAsync();
        await selector.ClickAsync();

        Assert.Equal(1, await Page.GetByText("Email Address", new() { Exact = true }).CountAsync());
        Assert.Equal(0, await Page.GetByText("Position", new() { Exact = true }).CountAsync());
        Assert.Equal(0, await Page.GetByText("email-detail", new() { Exact = true }).CountAsync());
    }

    [Fact]
    public async Task Read_only_standard_details_preserve_compact_rows_and_suppress_mutation_actions()
    {
        await OpenMappingAsync();
        await ChooseDemoStateAsync("Standard Details · read-only");

        await Page.GetByText("Standard Details", new() { Exact = true }).WaitForAsync();
        Assert.True(await Page.GetByText("Email Address", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await Page.GetByText("Position", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal(0, await Page.GetByRole(AriaRole.Button, new() { Name = "Add standard detail", Exact = true }).CountAsync());
        Assert.Equal(0, await Page.GetByRole(AriaRole.Button, new() { Name = "Change Email Address standard detail", Exact = true }).CountAsync());
        Assert.Equal(0, await Page.GetByRole(AriaRole.Button, new() { Name = "Remove Email Address standard detail", Exact = true }).CountAsync());
        Assert.Equal(0, await Page.GetByRole(AriaRole.Combobox, new() { Name = "Standard Detail", Exact = true }).CountAsync());
    }

    [Theory]
    [InlineData(1280, 900)]
    [InlineData(760, 900)]
    [InlineData(390, 844)]
    public async Task Standard_details_mapping_reflows_without_horizontal_overflow(int width, int height)
    {
        await Page.SetViewportSizeAsync(width, height);
        await OpenMappingAsync();

        var baselineDocumentWidth = await Page.EvaluateAsync<int>("document.documentElement.scrollWidth");

        await ChooseDemoStateAsync("Standard Details enabled · selected");
        await Page.GetByText("Standard Details", new() { Exact = true }).WaitForAsync();

        var mapping = Page.Locator(".ov-mapping-pair");
        Assert.True(await mapping.EvaluateAsync<bool>("element => element.scrollWidth <= element.clientWidth + 1"));

        var selectedDocumentWidth = await Page.EvaluateAsync<int>("document.documentElement.scrollWidth");
        Assert.True(selectedDocumentWidth <= Math.Max(width + 1, baselineDocumentWidth + 1));
    }

    [Fact]
    public async Task Standard_details_mapping_is_axe_clean()
    {
        await OpenMappingAsync();
        await ChooseDemoStateAsync("Standard Details enabled · selected");
        await Page.GetByText("Standard Details", new() { Exact = true }).WaitForAsync();
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

    private async Task ChooseDemoStateAsync(string label)
    {
        var index = label switch
        {
            "Standard Details enabled · empty" => 1,
            "Standard Details enabled · selected" => 2,
            "Standard Details · validation" => 3,
            "Standard Details · read-only" => 4,
            _ => 0
        };

        var select = Page.GetByRole(AriaRole.Combobox, new() { Name = "Demo state", Exact = true });
        await select.ClickAsync();
        await Page.Keyboard.PressAsync("Home");
        for (var step = 0; step < index; step++)
            await Page.Keyboard.PressAsync("ArrowDown");
        await Page.Keyboard.PressAsync("Enter");
        await select.GetByText(label, new() { Exact = true }).WaitForAsync();
    }
}
