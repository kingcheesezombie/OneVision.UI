using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class MappingJoinBrowserTests : PageTest
{
    private static string BaseUrl => Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Asset_state_shows_dependent_definition_and_nested_fields_left_aligned()
    {
        await OpenStateAsync("Asset candidate");

        await Page.GetByText("ESR Number", new() { Exact = true }).WaitForAsync();
        var candidatePane = Page.Locator(".ov-mapping-join__pane").Nth(1);
        await candidatePane.GetByLabel("Search field name or path", new() { Exact = true }).FillAsync("esrNumber");
        await candidatePane.GetByText("esrNumber", new() { Exact = true }).WaitForAsync();

        var labels = candidatePane.Locator(".ov-tree__label > span");
        Assert.True(await labels.CountAsync() > 0);
        var alignment = await labels.First.EvaluateAsync<string>("element => getComputedStyle(element).textAlign");
        Assert.Equal("left", alignment);

        var root = candidatePane.GetByText("asset", new() { Exact = true }).First;
        var nested = candidatePane.GetByText("identity", new() { Exact = true }).First;
        var leaf = candidatePane.GetByText("esrNumber", new() { Exact = true }).First;
        var rootBox = await root.BoundingBoxAsync();
        var nestedBox = await nested.BoundingBoxAsync();
        var leafBox = await leaf.BoundingBoxAsync();
        Assert.NotNull(rootBox);
        Assert.NotNull(nestedBox);
        Assert.NotNull(leafBox);
        Assert.True(nestedBox!.X > rootBox!.X);
        Assert.True(leafBox!.X > nestedBox.X);
    }

    [Fact]
    public async Task Person_episode_state_uses_configurable_definition_label()
    {
        await OpenStateAsync("Person Episode candidate");
        await Page.GetByText("Employment Episode", new() { Exact = true }).WaitForAsync();
        var candidatePane = Page.Locator(".ov-mapping-join__pane").Nth(1);
        await candidatePane.GetByLabel("Search field name or path", new() { Exact = true }).FillAsync("forename");
        await candidatePane.GetByText("forename", new() { Exact = true }).WaitForAsync();
        Assert.True(await Page.GetByText("Person Episode", new() { Exact = true }).CountAsync() >= 1);
    }

    [Fact]
    public async Task Loading_and_empty_states_remain_local_to_candidate_pane()
    {
        await OpenStateAsync("Structure loading");
        await Page.GetByText("Loading fields…", new() { Exact = true }).WaitForAsync();
        Assert.True(await Page.GetByText("displayName", new() { Exact = true }).CountAsync() > 0);

        await SelectStateAsync("Structure empty");
        await Page.GetByText("No matchable fields are available for this selection.", new() { Exact = true }).WaitForAsync();
        Assert.True(await Page.GetByText("displayName", new() { Exact = true }).CountAsync() > 0);
    }

    [Theory]
    [InlineData(1280, 900, false)]
    [InlineData(760, 900, true)]
    [InlineData(390, 844, true)]
    public async Task Mapping_join_preserves_responsive_structure_layout(int width, int height, bool stacked)
    {
        await Page.SetViewportSizeAsync(width, height);
        await OpenStateAsync("Asset candidate");

        var root = Page.Locator(".ov-mapping-join");
        Assert.True(await root.EvaluateAsync<bool>("element => element.scrollWidth <= element.clientWidth + 1"));

        var panes = Page.Locator(".ov-mapping-join__pane");
        var left = await panes.Nth(0).BoundingBoxAsync();
        var right = await panes.Nth(1).BoundingBoxAsync();
        Assert.NotNull(left);
        Assert.NotNull(right);
        if (stacked)
            Assert.True(right!.Y > left!.Y);
        else
            Assert.True(right!.X > left!.X);
    }

    [Fact]
    public async Task Mapping_join_is_axe_clean_with_dependent_selector()
    {
        await OpenStateAsync("Asset candidate");
        var result = await Page.Locator(".ov-mapping-join").RunAxe();
        var violations = result.Violations?.ToArray() ?? [];
        Assert.True(violations.Length == 0, string.Join(Environment.NewLine, violations.Select(v => $"{v.Id}: {v.Description}")));
    }

    private async Task OpenStateAsync(string state)
    {
        await Page.GotoAsync($"{BaseUrl}/components/mapping-join");
        await Page.Locator("h1.ov-gallery-page-title", new() { HasText = "Mapping Join" }).WaitForAsync();
        await SelectStateAsync(state);
    }

    private async Task SelectStateAsync(string state)
    {
        await Page.Locator(".ov-state-choice", new() { HasText = state }).ClickAsync();
        await Page.Locator(".ov-state-choice--active", new() { HasText = state }).WaitForAsync();
    }
}
