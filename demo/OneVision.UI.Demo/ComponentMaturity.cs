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
        "OVC-STATUS-PILL"
    ];

    public static ComponentMaturity Get(string reference)
        => ImplementedReferences.Contains(reference) ? ComponentMaturity.Implemented : ComponentMaturity.ContractDemo;

    public static string Label(ComponentMaturity maturity)
        => maturity == ComponentMaturity.Implemented ? "Implemented" : "Behavioural contract demo";

    public static OVStatusPill.OVStatusTone Tone(ComponentMaturity maturity)
        => maturity == ComponentMaturity.Implemented ? OVStatusPill.OVStatusTone.Success : OVStatusPill.OVStatusTone.Info;
}
