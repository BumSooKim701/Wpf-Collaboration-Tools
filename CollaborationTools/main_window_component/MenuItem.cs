using MaterialDesignThemes.Wpf;

namespace CollaborationTools;

public class MenuItem
{
    public string? Title { get; set; }
    public string? MenuType { get; set; }

    public string? Action { get; set; }
    public PackIconKind SelectedIcon { get; set; }
    public PackIconKind UnselectedIcon { get; set; }
}