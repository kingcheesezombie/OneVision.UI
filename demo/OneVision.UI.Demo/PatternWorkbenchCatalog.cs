namespace OneVision.UI.Demo;

/// <summary>
/// Gallery-only descriptions of implemented pattern scenarios and optional capabilities.
/// Every option is wired to a real reference composition; this is not a runtime UI contract.
/// </summary>
public static class PatternWorkbenchCatalog
{
    public sealed record Option(string Key, string Label, bool DefaultEnabled = true);
    public sealed record Definition(IReadOnlyList<Option> Scenarios, IReadOnlyList<Option> Capabilities)
    {
        public string DefaultScenario => Scenarios[0].Key;
    }

    private static readonly IReadOnlyDictionary<string, Definition> Definitions =
        new Dictionary<string, Definition>(StringComparer.OrdinalIgnoreCase)
        {
            ["list-page"] = new(
                [new("populated", "Populated"), new("empty", "Empty results"), new("filtered", "Filtered results"),
                 new("loading", "Initial loading"), new("error", "Load error / Retry"),
                 new("refreshing", "Refreshing existing records"), new("refresh-error", "Refresh error / Retained records"),
                 new("read-only", "Read-only list")],
                [new("search", "Search"), new("filter", "Filter action"), new("add", "Primary Add action"), new("row-actions", "Row actions"), new("pagination", "Pagination")]),
            ["catalogue-management"] = new(
                [new("populated", "Populated"), new("empty", "Empty"), new("loading", "Loading"), new("error", "Load error / Retry"),
                 new("no-results", "No results"), new("read-only", "Read-only"), new("add", "Add editor"),
                 new("edit", "Edit editor"), new("refreshing", "Refreshing existing catalogue"),
                 new("refresh-error", "Refresh error / Retained catalogue"), new("save-error", "Editor save error")],
                [
                    new("search", "Search"),
                    new("filters", "Status filters"),
                    new("add", "Add action"),
                    new("enabled", "Enabled state"),
                    new("row-actions", "Row actions"),
                    new("pagination", "Pagination")
                ]),
            ["api-call-builder"] = new(
                [new("guided", "Guided / blank"), new("simple", "Simple"), new("medium", "Medium"), new("deep", "Deep"),
                 new("structured", "Structured body"), new("bulk", "Bulk body"), new("read-only", "Read-only"),
                 new("saving", "Saving request"), new("save-error", "Save failure / Retry"), new("saved", "Saved request")],
                []),
            ["source-configuration"] = new(
                [new("initial", "Not profiled"), new("current", "Current profile and mappings"),
                 new("stale", "Stale profile"), new("failed", "Failed profile"),
                 new("read-only", "Read-only"), new("saving", "Saving source"),
                 new("save-error", "Save failure / Retained mapping")],
                [new("display-value", "Display Value Builder", false), new("standard-details", "Include Standard Details", false)]),
            ["progressive-configuration-builder"] = new(
                [new("default", "Active step / pending"), new("initial", "Initial"), new("completed", "Completed"),
                 new("any", "Wildcard selections"), new("conditional", "Conditional target"), new("validation", "Validation error"),
                 new("long", "Long values"), new("read-only", "Read-only"),
                 new("submitting", "Submitting configuration"), new("submission-error", "Save failure / Retained steps"),
                 new("submitted", "Saved configuration")],
                []),
            ["operational-detail"] = new(
                [new("healthy", "Healthy"), new("disabled", "Disabled")],
                [new("health", "Health summary"), new("event", "Latest event"), new("stages", "Process stages"), new("activity", "Recent activity"), new("actions", "Actions")]),
            ["entity-inspector"] = new(
                [new("loaded", "Loaded"), new("loading", "Loading"), new("refreshing", "Refreshing"),
                 new("refresh-error", "Recoverable refresh error"), new("empty", "Empty"), new("error", "Initial load error"),
                 new("busy", "Processing action"), new("action-error", "Action error / Retained details"),
                 new("long", "Long content"), new("no-status", "No status"), new("multiple-actions", "Multiple actions")],
                [
                    new("status", "Status/type/source pills"),
                    new("summary", "Summary metadata"),
                    new("technical", "Technical metadata"),
                    new("actions", "Application actions"),
                    new("refresh", "Refresh action")
                ]),
            ["allocation"] = new(
                [
                    new("known", "Known target"),
                    new("finder", "Find target"),
                    new("rule-constrained", "Known target / rule-constrained source"),
                    new("overlay", "In OVOverlay"),
                    new("searching", "Searching / loading"),
                    new("empty", "No eligible source results"),
                    new("invalid", "Invalid selection"),
                    new("unavailable", "Eligibility unavailable"),
                    new("read-only", "Read-only"),
                    new("submitting", "Submitting"),
                    new("submission-error", "Submission error"),
                    new("completed", "Completed allocation")
                ],
                [
                    new("editable-source-system", "Editable source system"),
                    new("editable-source-type", "Editable source type", false),
                    new("selected-source", "Selected source", false),
                    new("selected-target", "Selected target"),
                    new("explicit-dates", "Explicit effective dates", false),
                    new("constrain-platforms", "Constrain target to EAM", false)
                ])
        };

    public static Definition For(string slug) => Definitions[slug];

    public static IReadOnlyCollection<string> DefaultCapabilities(string slug)
        => For(slug).Capabilities.Where(option => option.DefaultEnabled).Select(option => option.Key).ToArray();
}
