using MudBlazor;

namespace OneVision.UI.Theming;

public static class OneVisionTheme
{
    public static MudTheme Default { get; } = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#005EB8",
            Secondary = "#425563",
            Success = "#007F3B",
            Warning = "#FFB81C",
            Error = "#D5281B",
            Info = "#41A7F2",
            Background = "#F4F6F8",
            Surface = "#FFFFFF",
            TextPrimary = "#212B32",
            TextSecondary = "#4C6272"
        }
    };
}
