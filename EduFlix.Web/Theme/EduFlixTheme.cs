using MudBlazor;

namespace EduFlix.Web.Theme;

// De EduFlix look and feel op 1 plek zodat de hele app consistent blijft.
public static class EduFlixTheme
{
    public static readonly MudTheme Default = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#6D4AFF",
            Secondary = "#00C2A8",
            AppbarBackground = "#6D4AFF",
            AppbarText = "#FFFFFF",
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#8B6BFF",
            Secondary = "#00D9BC",
            Background = "#16151D",
            Surface = "#1E1D28",
            AppbarBackground = "#1E1D28",
            DrawerBackground = "#16151D",
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "10px",
        },
    };
}
