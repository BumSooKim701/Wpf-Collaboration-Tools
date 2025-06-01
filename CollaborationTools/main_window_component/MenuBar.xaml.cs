using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace CollaborationTools;

public partial class MenuBar : UserControl
{
    private List<MenuItem> MenuList { get; set; }
    public MenuBar()
    {
        InitializeComponent();
        DataContext = this;
        
        MenuList = new()
        {
            new MenuItem
            {
                Title = "홈",
                SelectedIcon = PackIconKind.Home,
                UnselectedIcon = PackIconKind.HomeOutline,
            },
            new MenuItem
            {
                Title = "캘린더",
                SelectedIcon = PackIconKind.Calendar,
                UnselectedIcon = PackIconKind.CalendarOutline,
            },
            new MenuItem
            {
                Title = "파일",
                SelectedIcon = PackIconKind.Folder,
                UnselectedIcon = PackIconKind.FolderOutline,
            },
            new MenuItem
            {
                Title = "메모",
                SelectedIcon = PackIconKind.Note,
                UnselectedIcon = PackIconKind.NoteOutline,
            },
        };
        NavigationBar.ItemsSource = MenuList;
    }
}