namespace OneVision.UI.Demo;

/// <summary>
/// Independent, implemented modifiers for the component workbench.
/// Components not listed retain their existing state-only reference demos.
/// No options are inferred from unrelated controls or imaginary combinations.
/// </summary>
public static class ComponentCapabilityCatalog
{
    public sealed record Option(string Key, string Label, bool DefaultEnabled = false);

    private static readonly Option Disabled = new("disabled", "Disabled");
    private static readonly Option ReadOnly = new("read-only", "Read-only");
    private static readonly Option Validation = new("validation", "Validation error");
    private static readonly Option Loading = new("loading", "Loading");
    private static readonly IReadOnlyList<Option> NoOptions = [];

    private static readonly HashSet<string> DisabledInputs = new(StringComparer.Ordinal)
    {
        "OVC-BUTTON", "OVC-ICON-BUTTON", "OVC-TEXT-FIELD", "OVC-TEXT-AREA",
        "OVC-NUMBER-FIELD", "OVC-SEARCH-BOX", "OVC-SELECT", "OVC-MULTI-SELECT",
        "OVC-AUTOCOMPLETE", "OVC-CHECKBOX", "OVC-RADIO-GROUP", "OVC-TOGGLE",
        "OVC-DATE-PICKER", "OVC-DATE-RANGE", "OVC-TIME-PICKER"
    };

    private static readonly HashSet<string> ReadOnlyInputs = new(StringComparer.Ordinal)
    {
        "OVC-TEXT-FIELD", "OVC-TEXT-AREA", "OVC-NUMBER-FIELD", "OVC-SELECT",
        "OVC-MULTI-SELECT", "OVC-DATE-PICKER", "OVC-DATE-RANGE", "OVC-TIME-PICKER"
    };

    private static readonly HashSet<string> ValidatedInputs = new(StringComparer.Ordinal)
    {
        "OVC-TEXT-FIELD", "OVC-TEXT-AREA", "OVC-NUMBER-FIELD", "OVC-SELECT",
        "OVC-RADIO-GROUP", "OVC-DATE-PICKER", "OVC-DATE-RANGE", "OVC-TIME-PICKER"
    };

    public static IReadOnlyList<Option> For(string reference)
    {
        if (reference == "OVC-TREE")
            return [new("search", "Hierarchy search", true), new("selection-count", "Selection count", true),
                ReadOnly, new("refresh", "Refresh in progress"), new("refresh-error", "Recoverable refresh failure")];
        if (reference == "OVC-MAPPING-JOIN")
            return [ReadOnly, new("refresh", "Refresh in progress"), new("refresh-error", "Recoverable refresh failure")];
        if (reference == "OVC-MAPPING-RULE")
            return [ReadOnly, new("nested-groups", "Nested groups"), new("condition-disabling", "Condition enablement"),
                new("reordering", "Condition ordering"), new("case-policy", "Case sensitivity"),
                new("test-rule", "Test Rule", true), new("classification", "Classification", true),
                new("target-resolution", "Target resolution", true)];
        if (reference == "OVC-CONNECTION-EDITOR")
            return [new("test-action", "Test connection action", true), new("save-actions", "Save and cancel actions", true),
                ReadOnly, new("busy", "Saving"), new("save-error", "Save error")];
        if (reference == "OVC-SCHEDULE-BUILDER")
            return [new("items", "Item assignment", true), new("actions", "Save and cancel actions", true),
                ReadOnly, new("busy", "Saving"), new("save-error", "Save error")];
        if (reference == "OVC-PAGINATION")
            return [new("busy", "Page request in progress")];
        if (reference == "OVC-SELECTION-LIST")
            return [Disabled, new("multiple", "Multiple selection")];
        if (reference == "OVC-FILE-UPLOAD")
            return [Disabled, ReadOnly, new("multiple", "Multiple files"), new("progress", "Upload in progress")];
        if (reference == "OVC-DATE-RANGE")
            return [Disabled, ReadOnly, Validation, new("complete", "Require both dates"), new("limits", "October 2026 limits")];
                if (reference == "OVC-MAPPING-PAIR")
            return [ReadOnly, new("constants", "Allow constants"), new("standard-details", "Standard Details", true), new("refresh", "Refresh in progress"), new("refresh-error", "Recoverable refresh failure")];
        if (reference == "OVC-ALERT")
            return [new("dismissible", "Dismissible")];
        if (reference == "OVC-DATA-GRID")
            return [ReadOnly, new("refresh", "Refresh in progress"), new("refresh-error", "Recoverable refresh failure")];
        if (reference == "OVC-TOOLBAR") return [ReadOnly];
        if (!DisabledInputs.Contains(reference)) return NoOptions;

        var result = new List<Option> { Disabled };
        if (ReadOnlyInputs.Contains(reference)) result.Add(ReadOnly);
        if (ValidatedInputs.Contains(reference)) result.Add(Validation);
        if (reference is "OVC-BUTTON" or "OVC-ICON-BUTTON" or "OVC-AUTOCOMPLETE") result.Add(Loading);
        return result;
    }

    public static IReadOnlyCollection<string> Defaults(string reference)
        => For(reference).Where(option => option.DefaultEnabled).Select(option => option.Key).ToArray();
}
