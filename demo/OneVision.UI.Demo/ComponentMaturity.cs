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
        "OVC-OVERFLOW-MENU",
        "OVC-PILL",
        "OVC-TEXT-FIELD",
        "OVC-TEXT-AREA",
        "OVC-NUMBER-FIELD",
        "OVC-SEARCH-BOX",
        "OVC-SELECT",
        "OVC-MULTI-SELECT",
        "OVC-AUTOCOMPLETE",
        "OVC-CHECKBOX",
        "OVC-RADIO-GROUP",
        "OVC-TOGGLE",
        "OVC-DATE-PICKER",
        "OVC-DATE-RANGE",
        "OVC-TIME-PICKER",
        "OVC-PAGINATION",
        "OVC-TABS",
        "OVC-BREADCRUMB",
        "OVC-TREE",
        "OVC-ALERT",
        "OVC-VALIDATION-SUMMARY",
        "OVC-TOOLTIP",
        "OVC-PROGRESS",
        "OVC-EMPTY-STATE",
        "OVC-CARD",
        "OVC-PANEL",
        "OVC-ACCORDION",
        "OVC-DIALOG",
        "OVC-OVERLAY",
        "OVC-CONFIRM-DIALOG",
        "OVC-KEY-VALUE",
        "OVC-METRIC",
        "OVC-SELECTION-SUMMARY",
        "OVC-COPYABLE-LINK",
        "OVC-TOOLBAR",
        "OVC-ACTION-BAR",
        "OVC-STRUCTURED-FIELD-TREE",
        "OVC-MAPPING-PAIR",
        "OVC-MAPPING-JOIN",
        "OVC-MAPPING-RULE",
        "OVC-PROCESS-STAGES",
        "OVC-NOTIFICATION",
        "OVC-FILE-UPLOAD",
        "OVC-TIMELINE",
        "OVC-SKELETON",
        "OVC-LIST",
        "OVC-SELECTION-LIST",
        "OVC-DIVIDER",
        "OVC-BADGE",
        "OVC-STEPPER",
        "OVC-DATE-TIME"
    ];

    public static ComponentMaturity Get(string reference)
        => ImplementedReferences.Contains(reference) ? ComponentMaturity.Implemented : ComponentMaturity.ContractDemo;

    public static string Label(ComponentMaturity maturity)
        => maturity == ComponentMaturity.Implemented ? "Implemented" : "Behavioural contract demo";

    public static OVPillTone Tone(ComponentMaturity maturity)
        => maturity == ComponentMaturity.Implemented ? OVPillTone.Success : OVPillTone.Info;
}
