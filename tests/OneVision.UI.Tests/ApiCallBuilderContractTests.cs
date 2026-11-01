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
        Assert.Contains("Request options", cut.Markup, StringComparison.Ordinal);
        Assert.Contains(cut.FindComponents<OVApiParameterControl>(), component => component.Instance.Definition.Name == "type");
    }

    [Fact]
    public async Task Completed_decisions_remain_quiet_visible_and_editable()
    {
        await using var context = CreateContext();
        var cut = Render(context, state: TerminalState);

        var completed = cut.FindAll(".ov-api-decision-trail__item");
        Assert.Equal(3, completed.Count);
        Assert.Equal("Completed: What action do you want to take?, Get information", completed[0].GetAttribute("aria-label"));
        Assert.Contains("Entities", completed[1].TextContent, StringComparison.Ordinal);
        Assert.NotNull(completed[1].QuerySelector("button[aria-label='Change What do you want to retrieve?: Entities']"));
    }

    [Fact]
    public async Task Returning_to_previous_decision_clears_incompatible_downstream_branch_without_eager_errors()
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
    public async Task Parameter_groups_and_optional_configuration_state_are_governed()
    {
        await using var context = CreateContext();
        var state = TerminalState with
        {
            Parameters = [new("type", "ESR_NUMBER"), new("search", "620496")]
        };
        var cut = Render(context, state: state);

        Assert.Contains("Selection", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("Scope", cut.Markup, StringComparison.Ordinal);
        var additional = cut.Find("details.ov-api-call-builder__additional");
        Assert.Null(additional.GetAttribute("open"));
        Assert.Contains("1 configured", additional.TextContent, StringComparison.Ordinal);
        Assert.Contains("Filters", additional.InnerHtml, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Invalid_resolved_request_disables_test_and_save_and_validates_touched_field()
    {
        await using var context = CreateContext();
        var cut = Render(context, state: TerminalState, dirty: true);

        var type = cut.FindComponents<OVApiParameterControl>().Single(component => component.Instance.Definition.Name == "type");
        Assert.False(type.Instance.HasError);
        Assert.Contains("Choose entity type", cut.Find(".ov-api-call-builder__ready").TextContent, StringComparison.OrdinalIgnoreCase);
        Assert.True(cut.FindAll("button").Single(button => button.TextContent.Trim() == "Test Request").HasAttribute("disabled"));
        Assert.True(cut.FindAll("button").Single(button => button.TextContent.Trim() == "Save").HasAttribute("disabled"));

        await type.Instance.ValueChanged.InvokeAsync("ESR_NUMBER");
        Assert.False(cut.FindAll("button").Single(button => button.TextContent.Trim() == "Test Request").HasAttribute("disabled"));
        Assert.False(cut.FindAll("button").Single(button => button.TextContent.Trim() == "Save").HasAttribute("disabled"));

        type = cut.FindComponents<OVApiParameterControl>().Single(component => component.Instance.Definition.Name == "type");
        await type.Instance.ValueChanged.InvokeAsync(null);
        type = cut.FindComponents<OVApiParameterControl>().Single(component => component.Instance.Definition.Name == "type");
        Assert.True(type.Instance.HasError);
    }

    [Fact]
    public async Task Dependent_parameter_explains_why_it_is_unavailable_and_auto_layout_is_single_column()
    {
        await using var context = CreateContext();
        var catalogue = new OVApiCatalogue(
            "action",
            [new("action", "Choose", [new("go", "Go", OperationId: "dependent")])],
            [new("dependent", "Dependent request", "GET", "/api/dependent",
            [
                new("provider", "Target provider", OVApiParameterInputKind.Select, Options: [new("IDM", "IDM")], Required: true, Group: "Target"),
                new("type", "Target type", OVApiParameterInputKind.Select, Options: [new("PERSON", "Person")], Required: true, DependsOn: "provider", Group: "Target")
            ])]);
        var cut = Render(context, new OVApiGuidedState([new("action", "go")], [], "dependent"), catalogue: catalogue);

        var dependent = cut.FindComponents<OVApiParameterControl>().Single(component => component.Instance.Definition.Name == "type");
        Assert.Equal("Choose target provider first.", dependent.Instance.UnavailableMessage);
        Assert.NotNull(cut.Find(".ov-api-call-builder__parameters--single"));
    }

    [Fact]
    public async Task Explicit_parameter_group_layout_can_request_two_columns()
    {
        await using var context = CreateContext();
        var catalogue = new OVApiCatalogue(
            "action",
            [new("action", "Choose", [new("go", "Go", OperationId: "layout")])],
            [new("layout", "Layout request", "GET", "/api/layout",
            [
                new("one", "One", Group: "Pair", GroupLayout: OVApiParameterGroupLayout.TwoColumn),
                new("two", "Two", Group: "Pair", GroupLayout: OVApiParameterGroupLayout.TwoColumn)
            ])]);
        var cut = Render(context, new OVApiGuidedState([new("action", "go")], [], "layout"), catalogue: catalogue);

        Assert.NotNull(cut.Find(".ov-api-call-builder__parameters--two"));
    }

    [Fact]
    public async Task Resolved_request_prioritises_resolved_path_and_supports_copy_request()
    {
        await using var context = CreateContext();
        var state = TerminalState with
        {
            Parameters = [new("type", "ESR_NUMBER"), new("includeDetails", "true")]
        };
        var cut = Render(context, state: state);

        var request = cut.Find(".ov-api-call-builder__request");
        Assert.Contains("GET · Get entities", request.TextContent, StringComparison.Ordinal);
        Assert.Equal(4, cut.FindComponents<OVCopyableLink>().Count(component =>
            component.Instance.Label is "Copy API path" or "Copy resolved API path" or "Copy full URL" or "Copy request"));
        Assert.NotNull(request.QuerySelector(".ov-api-call-builder__copy-row--primary"));
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
        Assert.Contains("Generated request", advanced.InnerHtml, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Read_only_mode_is_a_value_summary_not_a_disabled_form()
    {
        await using var context = CreateContext();
        var state = TerminalState with { Parameters = [new("type", "ESR_NUMBER"), new("includeDetails", "true")] };
        var cut = Render(context, state: state, readOnly: true);

        Assert.Empty(cut.FindComponents<OVApiParameterControl>());
        Assert.Contains("Configured request values", cut.Markup, StringComparison.Ordinal);
        Assert.Contains("ESR number", cut.Markup, StringComparison.Ordinal);
        Assert.DoesNotContain(cut.FindAll("button"), button => button.TextContent.Trim() == "Save");
        Assert.Contains(cut.FindAll("button"), button => button.TextContent.Trim() == "Close");
        Assert.Contains(cut.FindComponents<OVCopyableLink>(), component => component.Instance.Label == "Copy resolved API path");
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
    public async Task Failure_response_distinguishes_authentication_and_keeps_diagnostics_secondary()
    {
        await using var context = CreateContext();
        var result = new OVApiCallTestResult(
            OVApiTestState.Failure,
            403,
            "Forbidden",
            DurationMilliseconds: 24,
            FailureKind: OVApiFailureKind.Authentication,
            Endpoint: "/api/v1/entities/query",
            ResponseHeaders: new Dictionary<string, string?> { ["x-request-id"] = "abc" },
            CorrelationId: "abc",
            RawResponse: "{\"error\":\"forbidden\"}");
        var cut = Render(context, state: TerminalState, result: result);

        Assert.Contains("Authentication is not available", cut.Markup, StringComparison.Ordinal);
        var details = cut.Find("details.ov-api-response-preview__technical");
        Assert.Null(details.GetAttribute("open"));
        Assert.Contains("Request ID", details.InnerHtml, StringComparison.Ordinal);
        Assert.Contains("Response headers", details.InnerHtml, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(400, "The API rejected one or more request values.")]
    [InlineData(401, "Authentication required.")]
    [InlineData(403, "You do not have permission to perform this request.")]
    [InlineData(404, "The requested resource was not found.")]
    [InlineData(500, "The API could not complete the request.")]
    public async Task Failure_response_uses_specific_common_http_messages(int statusCode, string expected)
    {
        await using var context = CreateContext();
        var result = new OVApiCallTestResult(OVApiTestState.Failure, statusCode, "Failure");
        var cut = Render(context, state: TerminalState, result: result);

        Assert.Contains(expected, cut.Markup, StringComparison.Ordinal);
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

    [Fact]
    public async Task Decision_step_uses_governed_auto_thresholds_and_explicit_hints()
    {
        await using var context = CreateContext();
        var simple = context.Render<OVApiDecisionStep>(parameters => parameters.Add(component => component.Node,
            new OVApiDecisionNode("simple", "Choose", Enumerable.Range(1, 4).Select(index => new OVApiDecisionChoice(index.ToString(), $"Choice {index}")).ToArray())));
        Assert.NotNull(simple.Find(".ov-api-decision-step__choices--buttons"));

        var descriptive = context.Render<OVApiDecisionStep>(parameters => parameters.Add(component => component.Node,
            new OVApiDecisionNode("descriptive", "Choose", Enumerable.Range(1, 4).Select(index => new OVApiDecisionChoice(index.ToString(), $"Choice {index}", $"Explanation {index}")).ToArray())));
        Assert.NotNull(descriptive.Find(".ov-api-decision-step__choices--cards"));

        var medium = context.Render<OVApiDecisionStep>(parameters => parameters.Add(component => component.Node,
            new OVApiDecisionNode("medium", "Choose", Enumerable.Range(1, 6).Select(index => new OVApiDecisionChoice(index.ToString(), $"Choice {index}")).ToArray())));
        Assert.NotNull(medium.Find("select.ov-api-decision-step__select"));

        var large = context.Render<OVApiDecisionStep>(parameters => parameters.Add(component => component.Node,
            new OVApiDecisionNode("large", "Choose", Enumerable.Range(1, 11).Select(index => new OVApiDecisionChoice(index.ToString(), $"Choice {index}")).ToArray())));
        Assert.Single(large.FindComponents<OVAutocomplete<string>>());

        var forced = context.Render<OVApiDecisionStep>(parameters => parameters.Add(component => component.Node,
            new OVApiDecisionNode("forced", "Choose", Enumerable.Range(1, 6).Select(index => new OVApiDecisionChoice(index.ToString(), $"Choice {index}")).ToArray(), Presentation: OVApiChoicePresentation.Cards)));
        Assert.Equal(6, forced.FindAll(".ov-api-decision-step__choice").Count);
    }

    private static AngleSharp.Dom.IElement Choice(IRenderedComponent<OVApiCallBuilder> cut, string text)
        => cut.FindAll("button").Single(button => button.TextContent.Contains(text, StringComparison.Ordinal));

    private static IRenderedComponent<OVApiCallBuilder> Render(
        BunitContext context,
        OVApiGuidedState? state = null,
        Action<OVApiCallConfiguration>? configurationChanged = null,
        OVApiCallTestResult? result = null,
        bool readOnly = false,
        bool dirty = false,
        OVApiCatalogue? catalogue = null)
        => context.Render<OVApiCallBuilder>(parameters => parameters
            .Add(component => component.Catalogue, catalogue ?? Catalogue)
            .Add(component => component.GuidedState, state ?? OVApiGuidedState.Empty)
            .Add(component => component.Connections, Connections)
            .Add(component => component.Configuration, new OVApiCallConfiguration(null, "GET", string.Empty, [], []))
            .Add(component => component.ConfigurationChanged, EventCallback.Factory.Create<OVApiCallConfiguration>(new object(), value => configurationChanged?.Invoke(value)))
            .Add(component => component.TestResult, result)
            .Add(component => component.ReadOnly, readOnly)
            .Add(component => component.Dirty, dirty));

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
                new("type", "Entity type", OVApiParameterInputKind.Select, Options: [new("ESR_NUMBER", "ESR number")], Required: true, Group: "Selection", GroupOrder: 1),
                new("includeDetails", "Include details?", OVApiParameterInputKind.Boolean, DefaultValue: "true", Group: "Scope", GroupOrder: 2),
                new("search", "Search", OVApiParameterInputKind.Text, Placeholder: "Search value", OptionalByDefault: true, Group: "Filters", GroupOrder: 3),
                new("includeInactive", "Include inactive?", OVApiParameterInputKind.Boolean, DefaultValue: "false", OptionalByDefault: true, Group: "Filters", GroupOrder: 3)
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