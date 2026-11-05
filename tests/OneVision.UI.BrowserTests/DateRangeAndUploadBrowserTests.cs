using System.Text;
using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

public sealed class DateRangeAndUploadBrowserTests : PageTest
{
    private static string BaseUrl =>
        Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Date_range_open_ended_and_bounded_validation_are_independently_explorable()
    {
        await Page.GotoAsync($"{BaseUrl}/components/date-range");

        await StateAsync("Open-ended");
        var result = Page.Locator(".ov-component-demo .ov-component-demo-stack [role='status']");
        Assert.Equal("Valid effective date range", (await result.InnerTextAsync()).Trim());

        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Require both dates" }).CheckAsync();
        await Page.GetByText("Select both a start and an end date.", new() { Exact = true }).Last.WaitForAsync();

        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Require both dates" }).UncheckAsync();
        await StateAsync("Invalid range");
        await Page.GetByText("End date cannot be before start date.", new() { Exact = true }).Last.WaitForAsync();

        await StateAsync("Out of bounds");
        await Page.GetByText("The selected dates cannot be before the minimum date.", new() { Exact = true }).Last.WaitForAsync();
    }

    [Fact]
    public async Task File_upload_can_distinguish_duplicate_names_and_honours_read_only_capability()
    {
        await Page.GotoAsync($"{BaseUrl}/components/file-upload");
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Multiple files" }).CheckAsync();

        var input = Page.Locator(".ov-file-upload input[type=file]");
        await input.SetInputFilesAsync(new[]
        {
            new FilePayload { Name = "same.csv", MimeType = "text/csv", Buffer = Encoding.UTF8.GetBytes("a") },
            new FilePayload { Name = "same.csv", MimeType = "text/csv", Buffer = Encoding.UTF8.GetBytes("b") }
        });

        var files = Page.Locator(".ov-file-upload .ov-file-upload__file");
        await files.Nth(1).WaitForAsync();
        Assert.Equal(2, await files.CountAsync());
        await Page.GetByRole(AriaRole.Button, new() { Name = "Remove same.csv (item 2)" }).ClickAsync();
        Assert.Equal(1, await files.CountAsync());

        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "Read-only" }).CheckAsync();
        Assert.True(await input.IsDisabledAsync());
        Assert.Equal(0, await Page.Locator(".ov-file-upload button[aria-label^='Remove']").CountAsync());
        Assert.Equal(1, await files.CountAsync());

        var result = await Page.Locator(".ov-file-upload").RunAxe();
        var violations = result.Violations?.ToArray() ?? [];
        Assert.True(violations.Length == 0, string.Join(Environment.NewLine, violations.Select(v => $"{v.Id}: {v.Description}")));
    }

    private async Task StateAsync(string name)
    {
        await Page.Locator(".ov-state-choice", new() { HasText = name }).ClickAsync();
        await Page.Locator(".ov-state-choice--active", new() { HasText = name }).WaitForAsync();
    }
}
