namespace OneVision.UI.Foundation;

/// <summary>
/// Machine-readable identity for the governed OneVision application foundation contract.
/// Applications can expose these values in diagnostics or contract tests to prove which
/// baseline they have adopted.
/// </summary>
public static class OneVisionApplicationFoundation
{
    public const string Reference = "OVP-000";
    public const string Name = "OneVision Application Foundation";
    public const string ContractVersion = "1.5";

    public const string ApplicationShellReference = "OVP-001";
    public const string StandardPageReference = "OVP-010";
    public const string ListPageReference = "OVP-011";
    public const string DetailPageReference = "OVP-012";
    public const string BuilderPageReference = "OVP-013";
    public const string GuidedLandingPageReference = "OVP-014";

    public const string DefaultPresentationProfile = "Manager";
    public const string ManagerPresentationProfile = "Manager";
    public const string ExperiencePresentationProfile = "Experience";

    public const string Iconography = "Fluent System Icons";
    public const string ServiceRegistrationMethod = "AddOneVisionUI";
    public const string StylesheetPath = "_content/OneVision.UI/onevision.css";
    public const string VisualLanguageStylesheetPath = "_content/OneVision.UI/onevision-fluent.css";
}
