using MudBlazor;

namespace OneVision.UI.Theming;

public static class OneVisionTheme
{
    /// <summary>
    /// Compatibility alias for the operational OneVision presentation profile.
    /// </summary>
    public static MudTheme Default => Manager;

    /// <summary>
    /// Compact operational presentation for manager and administration experiences.
    /// </summary>
    public static MudTheme Manager { get; } = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#005EB8",
            Secondary = "#425563",
            Success = "#007F3B",
            Warning = "#FFB81C",
            Error = "#D5281B",
            Info = "#0072CE",
            Background = "#F7F9FB",
            Surface = "#FFFFFF",
            TextPrimary = "#1B2A34",
            TextSecondary = "#4C6272"
        }
    };

    /// <summary>
    /// More expressive presentation for user-facing and guided experiences.
    /// </summary>
    public static MudTheme Experience { get; } = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#005EB8",
            Secondary = "#0072CE",
            Success = "#007F3B",
            Warning = "#FFB81C",
            Error = "#D5281B",
            Info = "#00A5DF",
            Background = "#EEF6FB",
            Surface = "#FFFFFF",
            TextPrimary = "#17324A",
            TextSecondary = "#425B70"
        }
    };
}
