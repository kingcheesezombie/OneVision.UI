using MudBlazor;

namespace OneVision.UI.Theming;

public static class OneVisionTheme
{
    /// <summary>
    /// Compatibility alias for the operational OneVision presentation profile.
    /// </summary>
    public static MudTheme Default => Manager;

    /// <summary>
    /// Fluent-inspired compact operational presentation for manager and administration experiences.
    /// </summary>
    public static MudTheme Manager { get; } = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#005EB8",
            Secondary = "#0F6CBD",
            Success = "#107C10",
            Warning = "#F7630C",
            Error = "#D13438",
            Info = "#0F6CBD",
            Background = "#F5F5F5",
            Surface = "#FFFFFF",
            TextPrimary = "#242424",
            TextSecondary = "#616161",
            Divider = "#D1D1D1"
        }
    };

    /// <summary>
    /// Fluent-inspired expressive presentation for user-facing and guided experiences.
    /// </summary>
    public static MudTheme Experience { get; } = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#005EB8",
            Secondary = "#0F6CBD",
            Success = "#107C10",
            Warning = "#F7630C",
            Error = "#D13438",
            Info = "#2886DE",
            Background = "#F3F8FD",
            Surface = "#FFFFFF",
            TextPrimary = "#242424",
            TextSecondary = "#5C5C5C",
            Divider = "#D6E2EE"
        }
    };
}
