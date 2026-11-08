using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class SourceConfigurationBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Theory]
    [InlineData(1280, 900)]
    [InlineData(760, 900)]
    [InlineData(390, 844)]
    public async Task Profile_uses_two_contained_sections_and_places_run_below_definition(int width, int height)
    {
        await Page.SetViewportSizeAsync(width, height);
        await Page.GotoAsync($"{BaseUrl}/patterns/source-configuration");
        await Page.GetByRole(AriaRole.Tab, new() { Name = "Profile", Exact = true }).ClickAsync();

        var source = Page.Locator(".ov-source-profile__definition");
        var results = Page.Locator(".ov-source-profile__result");
        Assert.True(await source.GetByRole(AriaRole.Heading, new() { Name = "Source Configuration" }).IsVisibleAsync());
        Assert.True(await results.GetByRole(AriaRole.Heading, new() { Name = "Profile Results" }).IsVisibleAsync());
        Assert.True(await source.GetByRole(AriaRole.Button, new() { Name = "Run & Profile", Exact = true }).IsVisibleAsync());
        Assert.Equal(0, await results.GetByRole(AriaRole.Button, new() { Name = "Run & Profile", Exact = true }).CountAsync());

        var bounds = await Page.Locator(".ov-source-profile").EvaluateAsync<double[]>(@"element => {
            const definition = element.querySelector('.ov-source-profile__definition-content').getBoundingClientRect();
            const runbar = element.querySelector('.ov-source-profile__runbar').getBoundingClientRect();
            const results = element.querySelector('.ov-source-profile__result').getBoundingClientRect();
            return [definition.bottom, runbar.top, runbar.bottom, results.top];
        }");
        Assert.True(bounds[0] <= bounds[1] + 1 && bounds[2] <= bounds[3] + 1);
        Assert.True(await source.EvaluateAsync<bool>("element => element.scrollWidth <= element.clientWidth + 2"));
        Assert.True(await Page.EvaluateAsync<bool>("() => document.documentElement.scrollWidth <= innerWidth + 2"));
    }

    [Fact]
    public async Task Direct_web_table_has_same_contained_profile_sections()
    {
        await OpenDirectSourceAsync("profile");
        var source = Page.Locator(".ov-source-profile__definition");
        var results = Page.Locator(".ov-source-profile__result");
        Assert.True(await source.GetByRole(AriaRole.Heading, new() { Name = "Source Configuration" }).IsVisibleAsync());
        Assert.True(await results.GetByRole(AriaRole.Heading, new() { Name = "Profile Results" }).IsVisibleAsync());
        Assert.Equal(1, await source.Locator(".ov-source-profile__runbar").CountAsync());
        Assert.Equal(0, await results.Locator(".ov-source-profile__runbar").CountAsync());
    }

    [Fact]
    public async Task Web_table_direct_source_keeps_connection_stage_and_marks_it_intentionally_satisfied()
    {
        await OpenDirectSourceAsync("connection");

        Assert.True(await Page.GetByText("No managed connection required", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await Page.GetByText("This source is configured directly in Profile.", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal(0, await Page.GetByRole(AriaRole.Combobox, new() { Name = "Managed connection", Exact = true }).CountAsync());
    }

    [Fact]
    public async Task Web_table_user_can_inspect_real_candidate_data_before_committing_selection()
    {
        await OpenDirectSourceAsync("profile");

        Assert.True(await Page.GetByText("Population by neighbourhood", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await Page.GetByText("Service activity", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await Page.GetByText("Choose a table to preview", new() { Exact = true }).IsVisibleAsync());
        Assert.False(await Page.GetByRole(AriaRole.Button, new() { Name = "Run & Profile", Exact = true }).IsEnabledAsync());

        await Page.GetByText("Population by neighbourhood", new() { Exact = true }).ClickAsync();

        var preview = Page.Locator(".ov-source-discovery__preview");
        Assert.True(await preview.GetByText("North Ward", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await preview.GetByText("8,412", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await preview.GetByRole(AriaRole.Columnheader, new() { Name = "Population", Exact = true }).IsVisibleAsync());
        Assert.Equal(0, await Page.GetByText("First row contains column headings", new() { Exact = true }).CountAsync());
        Assert.False(await Page.GetByRole(AriaRole.Button, new() { Name = "Run & Profile", Exact = true }).IsEnabledAsync());

        await preview.GetByRole(AriaRole.Button, new() { Name = "Use this source", Exact = true }).ClickAsync();

        Assert.True(await Page.GetByText("First row contains column headings", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await Page.GetByRole(AriaRole.Button, new() { Name = "Run & Profile", Exact = true }).IsEnabledAsync());
        Assert.True(await preview.GetByText("Selected", new() { Exact = true }).IsVisibleAsync());
    }

    [Fact]
    public async Task Web_table_stale_profile_cannot_drive_mapping()
    {
        await OpenDirectSourceAsync("profile");
        await ChooseWebDemoStateAsync("Stale result after definition change");

        Assert.True(await Page.GetByText("Stale", new() { Exact = true }).IsVisibleAsync());
        Assert.True(await Page.GetByText("The source definition has changed. This profile result is retained for context only and must be refreshed before Mapping can use it.", new() { Exact = true }).IsVisibleAsync());

        await Page.GetByRole(AriaRole.Tab, new() { Name = "Mapping", Exact = true }).ClickAsync();

        Assert.True(await Page.GetByText("The source definition has changed since it was last profiled. Run & Profile again before using the discovered structure for Mapping.", new() { Exact = true }).IsVisibleAsync());
        Assert.Equal(0, await Page.GetByText("Pair mapping", new() { Exact = true }).CountAsync());
    }

    [Theory]
    [InlineData(1280, 900)]
    [InlineData(760, 900)]
    [InlineData(390, 844)]
    public async Task Web_table_discovery_and_preview_add_no_page_horizontal_overflow(int width, int height)
    {
        await Page.SetViewportSizeAsync(width, height);
        await OpenDirectSourceAsync("profile");

        var baselineDocumentWidth = await Page.EvaluateAsync<int>("document.documentElement.scrollWidth");
        await Page.GetByText("Population by neighbourhood", new() { Exact = true }).ClickAsync();

        var selectedDocumentWidth = await Page.EvaluateAsync<int>("document.documentElement.scrollWidth");
        Assert.True(selectedDocumentWidth <= Math.Max(width + 1, baselineDocumentWidth + 1),
            $"Expected preview to add no page-level horizontal overflow at {width}px; baseline was {baselineDocumentWidth}px and preview was {selectedDocumentWidth}px.");

        var discovery = Page.Locator(".ov-source-discovery");
        Assert.True(await discovery.EvaluateAsync<bool>("element => element.scrollWidth <= element.clientWidth + 1"));

        var preview = Page.Locator(".ov-source-discovery__table-wrap");
        Assert.True(await preview.IsVisibleAsync());
        Assert.True(await preview.EvaluateAsync<bool>("element => element.scrollWidth >= element.clientWidth"));
    }

    [Fact]
    public async Task Web_table_profile_is_axe_clean()
    {
        await OpenDirectSourceAsync("profile");
        await Page.GetByText("Population by neighbourhood", new() { Exact = true }).ClickAsync();

        var profile = Page.Locator(".ov-source-profile");
        var result = await profile.RunAxe();
        var violations = result.Violations?.ToArray() ?? [];
        Assert.True(violations.Length == 0, string.Join(Environment.NewLine, violations.Select(v => $"{v.Id}: {v.Description}")));
    }

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

        Assert.True(await Page.GetByText("Email Address", new() { Exact = true }).CountAsync() >= 1);
        Assert.Equal(1, await Page.GetByText("Position", new() { Exact = true }).CountAsync());
        Assert.Equal(1, await Page.GetByText("Forename", new() { Exact = true }).CountAsync());
        Assert.Equal(0, await Page.GetByText("email-detail", new() { Exact = true }).CountAsync());
    }

    [Fact]
    public async Task Required_mapping_rows_receive_governed_isolated_styles()
    {
        await Page.SetViewportSizeAsync(1280, 900);
        await OpenMappingAsync();
        await ChooseDemoStateAsync("Standard Details enabled · empty");

        var row = Page.Locator(".ov-mapping-pair__target-row--required").First;
        await row.WaitForAsync();
        Assert.Equal("grid", await row.EvaluateAsync<string>("element => getComputedStyle(element).display"));

        var dropZone = row.Locator(".ov-mapping-pair__drop-zone");
        Assert.Equal("2px", await dropZone.EvaluateAsync<string>("element => getComputedStyle(element).borderTopWidth"));
        Assert.NotEqual("none", await dropZone.EvaluateAsync<string>("element => getComputedStyle(element).borderTopStyle"));
        Assert.NotEqual("rgba(0, 0, 0, 0)", await dropZone.EvaluateAsync<string>("element => getComputedStyle(element).borderTopColor"));
    }

    [Fact]
    public async Task Standard_detail_committed_rows_receive_horizontal_governed_layout_and_mapped_surface()
    {
        await Page.SetViewportSizeAsync(1280, 900);
        await OpenMappingAsync();
        await ChooseDemoStateAsync("Standard Details enabled · selected");

        var row = Page.GetByRole(AriaRole.Listitem, new() { Name = "Standard Detail Email Address", Exact = true });
        await row.WaitForAsync();
        Assert.Equal("grid", await row.EvaluateAsync<string>("element => getComputedStyle(element).display"));

        var identity = row.Locator(".ov-mapping-pair__standard-detail-info");
        var mapping = row.Locator(".ov-mapping-pair__drop-zone--mapped");
        var actions = row.Locator(".ov-mapping-pair__actions");
        var identityBox = await identity.BoundingBoxAsync();
        var mappingBox = await mapping.BoundingBoxAsync();
        var actionsBox = await actions.BoundingBoxAsync();

        Assert.NotNull(identityBox);
        Assert.NotNull(mappingBox);
        Assert.NotNull(actionsBox);
        Assert.True(identityBox!.X < mappingBox!.X);
        Assert.True(mappingBox.X < actionsBox!.X);
        Assert.Equal("solid", await mapping.EvaluateAsync<string>("element => getComputedStyle(element).borderTopStyle"));
        Assert.Equal("2px", await mapping.EvaluateAsync<string>("element => getComputedStyle(element).borderTopWidth"));
    }

    [Fact]
    public async Task Standard_detail_draft_row_receives_dashed_row_and_bounded_pending_surface()
    {
        await Page.SetViewportSizeAsync(1280, 900);
        await OpenMappingAsync();
        await ChooseDemoStateAsync("Standard Details enabled · empty");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Add standard detail", Exact = true }).ClickAsync();

        var draft = Page.Locator(".ov-mapping-pair__target-row--draft");
        await draft.WaitForAsync();
        Assert.Equal("grid", await draft.EvaluateAsync<string>("element => getComputedStyle(element).display"));
        Assert.Equal("dashed", await draft.EvaluateAsync<string>("element => getComputedStyle(element).borderTopStyle"));

        var pending = draft.Locator(".ov-mapping-pair__drop-zone--pending");
        Assert.Equal("2px", await pending.EvaluateAsync<string>("element => getComputedStyle(element).borderTopWidth"));
        Assert.NotEqual("none", await pending.EvaluateAsync<string>("element => getComputedStyle(element).borderTopStyle"));
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

    [Theory]
    [InlineData(1280, "repeat")]
    [InlineData(760, "single")]
    public async Task Mapping_rows_follow_governed_desktop_and_narrow_column_contract(int width, string expectedLayout)
    {
        await Page.SetViewportSizeAsync(width, 900);
        await OpenMappingAsync();
        await ChooseDemoStateAsync("Standard Details enabled · selected");

        var row = Page.GetByRole(AriaRole.Listitem, new() { Name = "Standard Detail Email Address", Exact = true });
        await row.WaitForAsync();
        var columns = await row.EvaluateAsync<string>("element => getComputedStyle(element).gridTemplateColumns");

        if (expectedLayout == "single")
        {
            Assert.DoesNotContain(" ", columns.Trim());
        }
        else
        {
            Assert.True(columns.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length >= 3, $"Expected three desktop grid columns but got '{columns}'.");
        }
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

    private async Task OpenDirectSourceAsync(string section)
    {
        await Page.GotoAsync($"{BaseUrl}/patterns/source-configuration");
        var source = Page.GetByRole(AriaRole.Combobox, new() { Name = "Reference source", Exact = true });
        await source.ClickAsync();
        await Page.GetByText("Direct Web Table source", new() { Exact = true }).ClickAsync();
        await Page.GetByRole(AriaRole.Tab, new() { Name = char.ToUpperInvariant(section[0]) + section[1..], Exact = true }).ClickAsync();
    }

    private async Task ChooseWebDemoStateAsync(string label)
    {
        var select = Page.GetByRole(AriaRole.Combobox, new() { Name = "Web Table demo state", Exact = true });
        await select.ClickAsync();
        await Page.GetByText(label, new() { Exact = true }).ClickAsync();
    }

    private async Task ChooseDemoStateAsync(string label)
    {
        var index = label switch
        {
            "Standard Details enabled · empty" => 1,
            "Standard Details enabled · selected" => 2,
            "Standard Details · validation" => 3,
            "Standard Details · read-only" => 4,
            "Standard Details · committed unmapped" => 5,
            "Standard Details · exhausted catalogue" => 6,
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
