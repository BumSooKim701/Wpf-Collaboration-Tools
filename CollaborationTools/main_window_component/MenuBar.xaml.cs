using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace CollaborationTools;

public partial class MenuBar : UserControl
{
    public MenuBar()
    {
        InitializeComponent();
        DataContext = this;

        MenuList = new List<MenuItem>
        {
            new()
            {
                Title = "홈",
                SelectedIcon = PackIconKind.Home,
                UnselectedIcon = PackIconKind.HomeOutline
            },
            new()
            {
                Title = "캘린더",
                SelectedIcon = PackIconKind.Calendar,
                UnselectedIcon = PackIconKind.CalendarOutline
            },
            new()
            {
                Title = "파일",
                SelectedIcon = PackIconKind.Folder,
                UnselectedIcon = PackIconKind.FolderOutline
            },
            new()
            {
                Title = "메모",
                SelectedIcon = PackIconKind.Note,
                UnselectedIcon = PackIconKind.NoteOutline
            }
        };
        NavigationBar.ItemsSource = MenuList;
    }

    private List<MenuItem> MenuList { get; }
}