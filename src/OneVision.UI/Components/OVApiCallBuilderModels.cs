namespace OneVision.UI.Components;

public enum OVApiParameterInputKind
{
    Text,
    Boolean,
    Select,
    Date
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
    string? Placeholder = null);

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
