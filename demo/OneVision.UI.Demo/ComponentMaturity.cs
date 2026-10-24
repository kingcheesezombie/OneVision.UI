using OneVision.UI.Components;

namespace OneVision.UI.Demo;

public enum ComponentMaturity
{
    Implemented,
    ContractDemo,
    Pending
}

public static class ComponentMaturityCatalog
{
    private static readonly HashSet<string> ImplementedReferences =
    [
        "OVC-BUTTON",
        "OVC-STATUS-PILL"
    ];

    private static readonly HashSet<string> ContractDemoReferences =
    [
        "OVC-TREE",
        "OVC-MULTI-SELECT",
        "OVC-STRUCTURED-FIELD-TREE"
    ];

    public static ComponentMaturity Get(string reference)
    {
        if (ImplementedReferences.Contains(reference)) return ComponentMaturity.Implemented;
        if (ContractDemoReferences.Contains(reference)) return ComponentMaturity.ContractDemo;
        return ComponentMaturity.Pending;
    }

    public static string Label(ComponentMaturity maturity) => maturity switch
    {
        ComponentMaturity.Implemented => "Implemented",
        ComponentMaturity.ContractDemo => "Contract demo",
        _ => "Contract defined · implementation pending"
    };

    public static OVStatusPill.OVStatusTone Tone(ComponentMaturity maturity) => maturity switch
    {
        ComponentMaturity.Implemented => OVStatusPill.OVStatusTone.Success,
        ComponentMaturity.ContractDemo => OVStatusPill.OVStatusTone.Info,
        _ => OVStatusPill.OVStatusTone.Neutral
    };
}
