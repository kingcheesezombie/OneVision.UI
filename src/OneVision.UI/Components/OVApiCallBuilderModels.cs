namespace OneVision.UI.Components;

public enum OVApiParameterInputKind
{
    Text,
    Boolean,
    Select,
    Date,
    DateTime,
    Integer,
    Number,
    MultiSelect,
    TextArea,
    Lookup,
    StructuredObject,
    Collection
}

public enum OVApiParameterLocation
{
    Query,
    Path,
    Body,
    Header
}

public enum OVApiParameterSourceState
{
    Ready,
    Loading,
    Error
}

public enum OVApiTestState
{
    NotTested,
    Testing,
    Success,
    Empty,
    Failure
}

public sealed record OVApiConnectionOption(
    string Id,
    string Name,
    string BaseAddress,
    string AuthenticationDisplay,
    bool AuthenticationConfigured = true,
    string? Description = null);

public sealed record OVApiParameterDefinition(
    string Name,
    string Label,
    OVApiParameterInputKind InputKind = OVApiParameterInputKind.Text,
    string? Description = null,
    IReadOnlyList<OVOption<string>>? Options = null,
    bool Required = false,
    string? Placeholder = null,
    OVApiParameterLocation Location = OVApiParameterLocation.Query,
    string? DefaultValue = null,
    string? SourceKey = null,
    string? DependsOn = null,
    bool Hidden = false,
    bool OptionalByDefault = false,
    string? TechnicalName = null,
    string? ValidationMessage = null,
    double? Minimum = null,
    double? Maximum = null,
    string? Format = null,
    IReadOnlyList<OVApiParameterDefinition>? Fields = null);

public sealed record OVApiParameterValue(
    string Name,
    string? Value = null,
    bool Enabled = true);

public sealed record OVApiTokenOption(
    string Value,
    string Label,
    string Group);

public sealed record OVApiHeaderValue(
    string Name,
    string? Value = null,
    bool Enabled = true);

public sealed record OVApiCallConfiguration(
    string? ConnectionId,
    string Method,
    string Resource,
    IReadOnlyList<OVApiParameterValue> Parameters,
    IReadOnlyList<OVApiHeaderValue> Headers,
    string? RequestBody = null,
    int? TimeoutSeconds = null,
    string? ContentType = null);

public sealed record OVApiCallPreset(
    string Id,
    string Label,
    string? Description,
    OVApiCallConfiguration Configuration);

public sealed record OVApiDecisionChoice(
    string Value,
    string Label,
    string? Description = null,
    string? NextNodeId = null,
    string? OperationId = null);

public sealed record OVApiDecisionNode(
    string Id,
    string Prompt,
    IReadOnlyList<OVApiDecisionChoice> Choices,
    string? Description = null);

public sealed record OVApiOperationDefinition(
    string Id,
    string Label,
    string Method,
    string Resource,
    IReadOnlyList<OVApiParameterDefinition> Parameters,
    string? Description = null,
    string? ConnectionId = null,
    IReadOnlyList<OVApiHeaderValue>? Headers = null,
    string? ContentType = null,
    int? TimeoutSeconds = null);

public sealed record OVApiCatalogue(
    string InitialNodeId,
    IReadOnlyList<OVApiDecisionNode> Nodes,
    IReadOnlyList<OVApiOperationDefinition> Operations,
    string? Name = null,
    string? Description = null);

public sealed record OVApiDecisionSelection(
    string NodeId,
    string ChoiceValue);

public sealed record OVApiDecisionTrailItem(
    string Question,
    string Answer);

public sealed record OVApiGuidedState(
    IReadOnlyList<OVApiDecisionSelection> Decisions,
    IReadOnlyList<OVApiParameterValue> Parameters,
    string? OperationId = null)
{
    public static OVApiGuidedState Empty { get; } = new([], []);
}

public sealed record OVApiParameterSourceRequest(
    string OperationId,
    string ParameterName,
    string? SearchText,
    IReadOnlyDictionary<string, string?> CurrentValues);

public sealed record OVApiParameterSourceResult(
    IReadOnlyList<OVOption<string>> Options,
    OVApiParameterSourceState State = OVApiParameterSourceState.Ready,
    string? Message = null);

public sealed record OVApiResponseSampleRow(
    IReadOnlyDictionary<string, string?> Values);

public sealed record OVApiCallTestResult(
    OVApiTestState State,
    int? StatusCode = null,
    string? StatusText = null,
    long? RecordCount = null,
    long? DurationMilliseconds = null,
    string? FriendlyMessage = null,
    string? TechnicalDetails = null,
    IReadOnlyList<OVStructuredFieldNode>? Structure = null,
    IReadOnlyList<OVApiResponseSampleRow>? SampleRows = null,
    string? RawResponse = null);
