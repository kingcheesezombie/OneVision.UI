using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace OneVision.UI.BrowserTests;

/// <summary>
/// The active gallery must render all implemented public component definitions.
/// This is a cross-catalogue L0/L5 smoke check, not a claim of full interaction,
/// zoom, stress, or consuming-application certification for every control.
/// </summary>
public sealed class AllComponentGalleryBrowserTests : PageTest
{
    private static string BaseUrl =>
        Environment.GetEnvironmentVariable("ONEVISION_UI_TEST_URL")?.TrimEnd('/') ?? "http://127.0.0.1:5050";

    [Fact]
    public async Task Every_catalogued_component_has_a_real_preview_and_no_missing_demo_warning()
    {
        await Page.GotoAsync($"{BaseUrl}/components");
        var links = await Page.Locator("a.ov-catalog-card").EvaluateAllAsync<string[]>(
            "elements => elements.map(element => element.getAttribute('href')).filter(Boolean)");

        // All concrete catalogue entries, including restored Pagination/Dialog/
        // Confirm Dialog. Support-only children remain covered via owning patterns.
        Assert.True(links.Length >= 58, $"Expected the implemented manifest to expose at least 58 references; found {links.Length}.");
        Assert.Contains("components/pagination", links);
        Assert.Contains("components/dialog", links);
        Assert.Contains("components/confirm-dialog", links);
        Assert.Equal(links.Length, links.Distinct(StringComparer.OrdinalIgnoreCase).Count());

        var defects = new List<string>();
        foreach (var href in links)
        {
            try
            {
                var path = href.StartsWith('/') ? href : "/" + href;
                await Page.GotoAsync($"{BaseUrl}{path}");
                await Page.Locator(".ov-component-demo__body").WaitForAsync(new() { Timeout = 10000 });

                var markup = await Page.Locator(".ov-component-workbench__preview").InnerTextAsync();
                if (markup.Contains("No behavioural demo is registered", StringComparison.OrdinalIgnoreCase) ||
                    markup.Contains("No input/action demo is registered", StringComparison.OrdinalIgnoreCase) ||
                    markup.Contains("No feedback/surface demo is registered", StringComparison.OrdinalIgnoreCase) ||
                    markup.Contains("This component is not implemented", StringComparison.OrdinalIgnoreCase))
                {
                    defects.Add($"{href}: rendered placeholder instead of the governed component");
                }
            }
            catch (Exception exception)
            {
                defects.Add($"{href}: {exception.GetType().Name}: {exception.Message.Split('\n')[0]}");
            }
        }

        Assert.True(defects.Count == 0, "Gallery coverage defects:" + Environment.NewLine +
            string.Join(Environment.NewLine, defects));
    }
}
