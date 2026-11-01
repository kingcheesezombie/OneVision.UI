using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using OneVision.UI.Components;
using Xunit;

namespace OneVision.UI.Tests;

public sealed class ApiCallBuilderContractTests
{
    [Fact]
    public async Task Builder_presents_guided_common_case_and_safe_authentication_context()
    {
        await using var context = CreateContext();
        var cut = Render(context);

        Assert.Contains("Connection", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("Request", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("Parameters", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("Request preview", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("Test Call", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("Windows Integrated", cut.Markup, StringComparison.Ordinal);
        Assert.DoesNotContain("password", cut.Markup, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("credential value", cut.Markup, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Metadata_uses_governed_boolean_select_and_date_picker_controls()
    {
        await using var context = CreateContext();
        var configuration = Configuration([
            new("includeDetails", "true"),
            new("effectiveFrom", "2026-11-01")
        ]);
        var cut = Render(context, configuration);

        Assert.Contains(cut.FindComponents<OVSelect<string>>(), component => component.Instance.Label == "Include details");
        Assert.Contains(cut.FindComponents<OVDatePicker>(), component => component.Instance.Label == "Effective from");
    }

    [Fact]
    public async Task Raw_request_is_generated_and_url_encoded_without_manual_query_string_editing()
    {
        await using var context = CreateContext();
        var cut = Render(context, Configuration([new("search", "A B&C")]));

        var toggle = cut.FindAll("button").Single(button => button.TextContent.Contains("View raw request", StringComparison.Ordinal));
        await toggle.ClickAsync();

        Assert.Contains("/api/v1/entities/query?search=A%20B%26C", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Disabled_parameters_are_omitted_from_generated_request()
    {
        await using var context = CreateContext();
        var cut = Render(context, Configuration([new("search", "visible"), new("includeDetails", "true", Enabled: false)]));

        var toggle = cut.FindAll("button").Single(button => button.TextContent.Contains("View raw request", StringComparison.Ordinal));
        await toggle.ClickAsync();

        Assert.Contains("search=visible", cut.Markup, StringComparison.Ordinal);
        Assert.DoesNotContain("includeDetails=true", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Dirty_cancel_uses_governed_unsaved_change_confirmation()
    {
        await using var context = CreateContext();
        var cut = Render(context, dirty: true);

        var cancel = cut.FindAll("button").Last(button => button.TextContent.Trim().Equals("Cancel", StringComparison.Ordinal));
        await cancel.ClickAsync();

        var dialog = cut.Find("section[role='alertdialog']");
        Assert.Contains("Discard changes?", dialog.TextContent, StringComparison.Ordinal);
        Assert.Contains("Unsaved API call configuration will be lost.", dialog.TextContent, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Test_call_emits_structured_configuration_to_the_consuming_application()
    {
        await using var context = CreateContext();
        OVApiCallConfiguration? requested = null;
        var cut = Render(context, testRequested: configuration => requested = configuration);

        var test = cut.FindAll("button").Single(button => button.TextContent.Contains("Test Call", StringComparison.Ordinal));
        await test.ClickAsync();

        Assert.NotNull(requested);
        Assert.Equal("eam-dev", requested!.ConnectionId);
        Assert.Equal("GET", requested.Method);
        Assert.Equal("/api/v1/entities/query", requested.Resource);
    }

    [Fact]
    public async Task Response_structure_is_default_and_raw_is_secondary()
    {
        await using var context = CreateContext();
        var result = new OVApiCallTestResult(
            OVApiTestState.Success,
            200,
            "OK",
            1,
            10,
            Structure: [new("name", "name", "$.name", OVStructuredNodeKind.Primitive, OVStructuredDataType.String)],
            RawResponse: "{\"name\":\"Example\"}");
        var cut = Render(context, result: result);

        Assert.Equal("true", cut.Find("button[role='tab'][aria-selected='true']").GetAttribute("aria-selected"));
        Assert.Equal("Structure", cut.Find("button[role='tab'][aria-selected='true']").TextContent.Trim());
        Assert.DoesNotContain("{\"name\":\"Example\"}", cut.Markup, StringComparison.Ordinal);
    }

    private static IRenderedComponent<OVApiCallBuilder> Render(
        BunitContext context,
        OVApiCallConfiguration? configuration = null,
        bool dirty = false,
        Action<OVApiCallConfiguration>? testRequested = null,
        OVApiCallTestResult? result = null)
        => context.Render<OVApiCallBuilder>(parameters => parameters
            .Add(component => component.Connections, Connections)
            .Add(component => component.ParameterDefinitions, Definitions)
            .Add(component => component.Configuration, configuration ?? Configuration([]))
            .Add(component => component.Dirty, dirty)
            .Add(component => component.TestResult, result)
            .Add(component => component.TestRequested, EventCallback.Factory.Create<OVApiCallConfiguration>(new object(), value => testRequested?.Invoke(value))));

    private static OVApiCallConfiguration Configuration(IReadOnlyList<OVApiParameterValue> parameters)
        => new("eam-dev", "GET", "/api/v1/entities/query", parameters, []);

    private static readonly IReadOnlyList<OVApiConnectionOption> Connections =
    [
        new("eam-dev", "EAM DEV", "https://eam.example.invalid", "Windows Integrated")
    ];

    private static readonly IReadOnlyList<OVApiParameterDefinition> Definitions =
    [
        new("search", "Search"),
        new("includeDetails", "Include details", OVApiParameterInputKind.Boolean),
        new("effectiveFrom", "Effective from", OVApiParameterInputKind.Date)
    ];

    private static BunitContext CreateContext()
    {
        var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }
}
