using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

/// <summary>
/// Actual gallery controls, including independently toggled input limits.
/// </summary>
public sealed class SingleDateAndNumberBrowserTests : PageTest
{
    private static string BaseUrl =>
        Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Date_picker_exposes_required_empty_bounded_and_out_of_bounds_states()
    {
        await Page.GotoAsync($"{BaseUrl}/components/date-picker");
        var result = Page.Locator(".ov-component-demo__body [role=status]");
        await result.WaitForAsync();
        Assert.Contains("Valid selected date", await result.InnerTextAsync());

        await SelectStateAsync("Required empty");
        await Page.GetByText("A date is required.", new() { Exact = true }).Last.WaitForAsync();

        await SelectStateAsync("Out of bounds");
        await Page.GetByText("The selected date cannot be before the minimum date.", new() { Exact = true }).Last.WaitForAsync();

        await SelectStateAsync("Default");
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "October 2026 limits" }).CheckAsync();
        Assert.Equal("Valid selected date", (await result.InnerTextAsync()).Trim());
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Read-only" }).CheckAsync();
        Assert.False(await Page.Locator(".ov-component-demo__body input").First.IsEditableAsync());
    }

    [Fact]
    public async Task Number_field_accepts_independent_bounds_and_retains_displayed_value()
    {
        await Page.GotoAsync($"{BaseUrl}/components/number-field");
        await Page.Locator(".ov-component-demo__body input").First.WaitForAsync();

        var input = Page.Locator(".ov-component-demo__body input").First;
        Assert.Equal("25", await input.InputValueAsync());

        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "0–100 limits" }).CheckAsync();
        await Page.GetByText("Allowed range: 0–100", new() { Exact = true }).WaitForAsync();
        Assert.Equal("25", await input.InputValueAsync());

        await SelectStateAsync("Below minimum");
        await Page.GetByText("Allowed range: 0–100", new() { Exact = true }).WaitForAsync();

        await SelectStateAsync("Default");
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Read-only" }).CheckAsync();
        Assert.False(await input.IsEditableAsync());
    }

    private async Task SelectStateAsync(string name)
    {
        await Page.Locator(".ov-state-choice", new() { HasText = name }).ClickAsync();
        await Page.Locator(".ov-state-choice--active", new() { HasText = name }).WaitForAsync();
    }
}
