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
    public async Task Builder_starts_with_semantic_action_choice_not_http_fields_or_empty_output_sections()
    {
        await using var context = CreateContext();
        var cut = Render(context);

        Assert.Contains("What action do you want to take?", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("Get information", cut.Markup, StringComparison.Ordinal);
        Assert.DoesNotContain("Resource path", cut.Markup, StringComparison.Ordinal);
        Assert.DoesNotContain("Method", cut.Markup, StringComparison.Ordinal);
        Assert.Empty(cut.FindAll(".ov-api-call-builder__request"));
        Assert.Empty(cut.FindAll(".ov-api-call-builder__response"));
    }

    [Fact]
    public async Task Decision_tree_progresses_recursively_to_terminal_operation()
    {
        await using var context = CreateContext();
        var cut = Render(context);

        await Choice(cut, "Get information").ClickAsync();
        Assert.Contains("What do you want to retrieve?", cut.Markup, StringComparison.Ordinal);

        await Choice(cut, "Entities").ClickAsync();
        Assert.Contains("How should entities be selected?", cut.Markup, StringComparison.Ordinal);

        await Choice(cut, "By entity type").ClickAsync();
        Assert.Contains("Get entities", cut.Markup, StringComparison.Ordinal);
        Assert.Contains(cut.FindComponents<OVApiParameterControl>(), component => component.Instance.Definition.Name == "type");
    }

    [Fact]
    public async Task Completed_decisions_remain_visible_with_change_actions()
    {
        await using var context = CreateContext();
        var cut = Render(context, state: TerminalState);

        var completed = cut.FindAll(".ov-api-decision-trail__item");
        Assert.Equal(3, completed.Count);
        Assert.Contains("Get information", completed[0].TextContent, StringComparison.Ordinal);
        Assert.Contains("Entities", completed[1].TextContent, StringComparison.Ordinal);
        Assert.Contains("By entity type", completed[2].TextContent, StringComparison.Ordinal);
        Assert.NotNull(completed[1].QuerySelector("button[aria-label='Change What do you want to retrieve?: Entities']"));
    }

    [Fact]
    public async Task Returning_to_previous_decision_clears_incompatible_downstream_branch()
    {
        await using var context = CreateContext();
        var cut = Render(context, state: TerminalState);

        var trail = cut.FindAll("button").First(button => button.GetAttribute("aria-label")?.StartsWith("Change What do you want to retrieve?", StringComparison.Ordinal) == true);
        await trail.ClickAsync();

        Assert.Contains("What do you want to retrieve?", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("Get information", cut.Markup, StringComparison.Ordinal);
        Assert.DoesNotContain("Entity type is required", cut.Markup, StringComparison.Ordinal);
        Assert.Empty(cut.FindAll(".ov-api-call-builder__request"));
        Assert.Empty(cut.FindAll(".ov-api-call-builder__response"));
    }

    [Fact]
    public async Task Metadata_selects_rich_controls_and_keeps_optional_fields_secondary()
    {
        await using var context = CreateContext();
        var cut = Render(context, state: TerminalState);

        Assert.Contains(cut.FindComponents<OVApiParameterControl>(), component => component.Instance.Definition.InputKind == OVApiParameterInputKind.Select);
        var additional = cut.Find("details.ov-api-call-builder__additional");
        Assert.DoesNotContain("open", additional.Attributes.Select(attribute => attribute.Name));
        Assert.Contains("Search value", additional.InnerHtml, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Resolved_request_exposes_governed_copyable_path_and_url_outputs()
    {
        await using var context = CreateContext();
        var state = TerminalState with
        {
            Parameters = [new("type", "ESR_NUMBER"), new("includeDetails", "true")]
        };
        var cut = Render(context, state: state);

        var request = cut.Find(".ov-api-call-builder__request");
        Assert.Contains("GET · Get entities", request.TextContent, StringComparison.Ordinal);
        Assert.Equal(3, cut.FindComponents<OVCopyableLink>().Count(component =>
            component.Instance.Label is "Copy API path" or "Copy resolved API path" or "Copy full URL"));
        Assert.Contains("/api/v1/entities/query", request.TextContent, StringComparison.Ordinal);
        Assert.Contains("type=ESR_NUMBER", request.TextContent, StringComparison.Ordinal);
        Assert.Contains("https://eam.example.invalid/api/v1/entities/query", request.TextContent, StringComparison.Ordinal);
        Assert.DoesNotContain("password", request.TextContent, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Request_details_and_advanced_information_are_closed_by_default()
    {
        await using var context = CreateContext();
        var cut = Render(context, state: TerminalState);

        var requestDetails = cut.Find("details.ov-api-call-builder__request-details");
        var advanced = cut.Find("details.ov-api-call-builder__advanced");
        Assert.Null(requestDetails.GetAttribute("open"));
        Assert.Null(advanced.GetAttribute("open"));
        Assert.Contains("Raw generated request", advanced.InnerHtml, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Valid_guided_request_compiles_to_existing_configuration_contract()
    {
        await using var context = CreateContext();
        OVApiCallConfiguration? compiled = null;
        var state = TerminalState with
        {
            Parameters = [new("type", "ESR_NUMBER"), new("includeDetails", "true")]
        };
        var cut = Render(context, state: state, configurationChanged: value => compiled = value);

        cut.Render();
        await Task.Yield();

        Assert.NotNull(compiled);
        Assert.Equal("eam-demo", compiled!.ConnectionId);
        Assert.Equal("GET", compiled.Method);
        Assert.Equal("/api/v1/entities/query", compiled.Resource);
        Assert.Contains(compiled.Parameters, parameter => parameter.Name == "type" && parameter.Value == "ESR_NUMBER");
    }

    [Fact]
    public async Task Structured_request_body_is_control_driven_and_compiled_as_json()
    {
        await using var context = CreateContext();
        var state = new OVApiGuidedState(
            [new("action", "create"), new("create-what", "allocation")],
            [new("allocation", "{\"entityId\":123,\"targetProvider\":\"IDM\",\"targetKey\":\"ABC\"}")],
            "create-allocation");
        var cut = Render(context, state: state);

        var objectEditor = cut.FindComponents<OVApiParameterControl>().First(component => component.Instance.Definition.InputKind == OVApiParameterInputKind.StructuredObject);
        Assert.NotNull(objectEditor);
        Assert.Contains("Allocation details", cut.Markup, StringComparison.Ordinal);
        Assert.DoesNotContain("raw JSON", cut.Markup, StringComparison.OrdinalIgnoreCase);
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
        var cut = Render(context, state: TerminalState, result: result);

        Assert.Equal("Structure", cut.Find("button[role='tab'][aria-selected='true']").TextContent.Trim());
        Assert.DoesNotContain("{\"name\":\"Example\"}", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Authentication_is_context_only_and_never_exposes_secret_inputs()
    {
        await using var context = CreateContext();
        var cut = Render(context, state: TerminalState);

        Assert.Contains("Windows Integrated", cut.Markup, StringComparison.Ordinal);
        Assert.DoesNotContain("type=\"password\"", cut.Markup, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("credential value", cut.Markup, StringComparison.OrdinalIgnoreCase);
    }

    private static AngleSharp.Dom.IElement Choice(IRenderedComponent<OVApiCallBuilder> cut, string text)
        => cut.FindAll("button").Single(button => button.TextContent.Contains(text, StringComparison.Ordinal));

    private static IRenderedComponent<OVApiCallBuilder> Render(
        BunitContext context,
        OVApiGuidedState? state = null,
        Action<OVApiCallConfiguration>? configurationChanged = null,
        OVApiCallTestResult? result = null)
        => context.Render<OVApiCallBuilder>(parameters => parameters
            .Add(component => component.Catalogue, Catalogue)
            .Add(component => component.GuidedState, state ?? OVApiGuidedState.Empty)
            .Add(component => component.Connections, Connections)
            .Add(component => component.Configuration, new OVApiCallConfiguration(null, "GET", string.Empty, [], []))
            .Add(component => component.ConfigurationChanged, EventCallback.Factory.Create<OVApiCallConfiguration>(new object(), value => configurationChanged?.Invoke(value)))
            .Add(component => component.TestResult, result));

    private static readonly OVApiGuidedState TerminalState = new(
        [new("action", "get"), new("get-what", "entities"), new("entity-selection", "type")],
        [],
        "get-entities");

    private static readonly IReadOnlyList<OVApiConnectionOption> Connections =
    [
        new("eam-demo", "EAM managed", "https://eam.example.invalid", "Windows Integrated")
    ];

    private static readonly OVApiCatalogue Catalogue = new(
        "action",
        [
            new("action", "What action do you want to take?", [new("get", "Get information", NextNodeId: "get-what"), new("create", "Create something", NextNodeId: "create-what")]),
            new("get-what", "What do you want to retrieve?", [new("entities", "Entities", NextNodeId: "entity-selection"), new("version", "Version", OperationId: "version")]),
            new("entity-selection", "How should entities be selected?", [new("type", "By entity type", OperationId: "get-entities")]),
            new("create-what", "What do you want to create?", [new("allocation", "Allocation", OperationId: "create-allocation")])
        ],
        [
            new("version", "Get version", "GET", "/api/v1/version", [], ConnectionId: "eam-demo"),
            new("get-entities", "Get entities", "GET", "/api/v1/entities/query",
            [
                new("type", "Entity type", OVApiParameterInputKind.Select, Options: [new("ESR_NUMBER", "ESR number")], Required: true),
                new("includeDetails", "Include details?", OVApiParameterInputKind.Boolean, DefaultValue: "true"),
                new("search", "Search", OVApiParameterInputKind.Text, Placeholder: "Search value", OptionalByDefault: true)
            ], ConnectionId: "eam-demo"),
            new("create-allocation", "Create allocation", "POST", "/api/v1/allocations",
            [
                new("allocation", "Allocation details", OVApiParameterInputKind.StructuredObject, Required: true, Location: OVApiParameterLocation.Body, Fields:
                [
                    new("entityId", "Entity", OVApiParameterInputKind.Integer, Required: true),
                    new("targetProvider", "Target provider", OVApiParameterInputKind.Select, Options: [new("IDM", "IDM")], Required: true),
                    new("targetKey", "Target", Required: true)
                ])
            ], ConnectionId: "eam-demo")
        ]);

    private static BunitContext CreateContext()
    {
        var context = new BunitContext();
        context.Services.AddMudServices();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }
}
