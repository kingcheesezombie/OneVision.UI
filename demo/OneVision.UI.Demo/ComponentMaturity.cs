using OneVision.UI.Components;

namespace OneVision.UI.Demo;

public enum ComponentMaturity
{
    Implemented,
    ContractDemo
}

public static class ComponentMaturityCatalog
{
    private static readonly HashSet<string> ImplementedReferences =
    [
        "OVC-BUTTON",
        "OVC-ICON-BUTTON",
        "OVC-PILL",
        "OVC-MULTI-SELECT",
        "OVC-TREE",
        "OVC-VALIDATION-SUMMARY",
        "OVC-PROGRESS",
        "OVC-EMPTY-STATE",
        "OVC-METRIC",
        "OVC-SELECTION-SUMMARY",
        "OVC-TOOLBAR",
        "OVC-ACTION-BAR",
        "OVC-STRUCTURED-FIELD-TREE"
    ];

    public static ComponentMaturity Get(string reference)
        => ImplementedReferences.Contains(reference) ? ComponentMaturity.Implemented : ComponentMaturity.ContractDemo;

    public static string Label(ComponentMaturity maturity)
        => maturity == ComponentMaturity.Implemented ? "Implemented" : "Behavioural contract demo";

    public static OVPillTone Tone(ComponentMaturity maturity)
        => maturity == ComponentMaturity.Implemented ? OVPillTone.Success : OVPillTone.Info;
}
