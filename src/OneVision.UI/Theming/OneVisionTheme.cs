using MudBlazor;

namespace OneVision.UI.Theming;

public static class OneVisionTheme
{
    public static MudTheme Default => Manager;

    public static MudTheme Manager { get; } = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#005EB8", Secondary = "#0F6CBD", Success = "#107C10", Warning = "#F7630C", Error = "#D13438", Info = "#0F6CBD",
            Background = "#F5F5F5", Surface = "#FFFFFF", TextPrimary = "#242424", TextSecondary = "#616161", Divider = "#D1D1D1"
        },
        Typography = new Typography
        {
            Default = new DefaultTypography { FontFamily = ["Segoe UI Variable Text", "Segoe UI", "Arial", "sans-serif"], FontSize = ".8125rem", FontWeight = "400", LineHeight = "1.54" },
            Body1 = new Body1Typography { FontSize = ".8125rem", FontWeight = "400", LineHeight = "1.54" },
            Body2 = new Body2Typography { FontSize = ".8125rem", FontWeight = "400", LineHeight = "1.54" },
            H6 = new H6Typography { FontSize = "1.125rem", FontWeight = "600", LineHeight = "1.35" }
        }
    };

    public static MudTheme Experience { get; } = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#005EB8", Secondary = "#007F9E", Tertiary = "#5C2D91", Success = "#107C10", Warning = "#CA5010", Error = "#D13438", Info = "#2886DE",
            Background = "#F3F8FD", Surface = "#FFFFFF", TextPrimary = "#202020", TextSecondary = "#565656", Divider = "#D6E2EE"
        },
        Typography = new Typography
        {
            Default = new DefaultTypography { FontFamily = ["Segoe UI Variable Text", "Segoe UI", "Arial", "sans-serif"], FontSize = ".9375rem", FontWeight = "400", LineHeight = "1.47" },
            Body1 = new Body1Typography { FontSize = ".9375rem", FontWeight = "400", LineHeight = "1.47" },
            Body2 = new Body2Typography { FontSize = ".875rem", FontWeight = "400", LineHeight = "1.5" },
            H6 = new H6Typography { FontSize = "1.375rem", FontWeight = "600", LineHeight = "1.3" }
        }
    };
}
