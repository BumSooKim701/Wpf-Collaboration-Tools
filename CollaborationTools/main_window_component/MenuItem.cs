using MaterialDesignThemes.Wpf;

namespace CollaborationTools;

public class MenuItem
{
    public string? Title { get; set; }
    public PackIconKind SelectedIcon { get; set; }
    public PackIconKind UnselectedIcon { get; set; }
}