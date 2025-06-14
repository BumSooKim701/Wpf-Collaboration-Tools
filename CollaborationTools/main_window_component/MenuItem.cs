using MaterialDesignThemes.Wpf;

namespace CollaborationTools;

public class MenuItem
{
    public string? Title { get; set; }
    public MenuType MenuType { get; set; }
    public string? Action { get; set; }
    public PackIconKind SelectedIcon { get; set; }
    public PackIconKind UnselectedIcon { get; set; }
}

public enum MenuType
{
    Home,
    Calendar,
    File,
    Memo,
    Personal,
    Team
}