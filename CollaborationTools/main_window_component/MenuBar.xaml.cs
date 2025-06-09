using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace CollaborationTools;

public partial class MenuBar : UserControl
{
    private List<MenuItem> MenuList { get; }
    
    public MenuBar()
    {
        InitializeComponent();
        DataContext = this;

        MenuList = new List<MenuItem>
        {
            new()
            {
                Title = "홈",
                MenuType = MenuType.Home,
                SelectedIcon = PackIconKind.Home,
                UnselectedIcon = PackIconKind.HomeOutline
            },
            new()
            {
                Title = "캘린더",
                MenuType = MenuType.Calendar,
                SelectedIcon = PackIconKind.Calendar,
                UnselectedIcon = PackIconKind.CalendarOutline
            },
            new()
            {
                Title = "파일",
                MenuType = MenuType.File,
                SelectedIcon = PackIconKind.Folder,
                UnselectedIcon = PackIconKind.FolderOutline
            },
            new()
            {
                Title = "메모",
                MenuType = MenuType.Memo,
                SelectedIcon = PackIconKind.Note,
                UnselectedIcon = PackIconKind.NoteOutline
            }
        };
        NavigationBar.ItemsSource = MenuList;
    }

    private void OnMenuChanged(object sender, SelectionChangedEventArgs e)
    {
        var menuBarListBox = sender as ListBox;
        var selectedMenu = menuBarListBox.SelectedItem;
        
        if (selectedMenu is MenuItem menuItem)
        {
            // menuItem.Title 등 원하는 속성 사용 가능
            MessageBox.Show($"선택된 메뉴: {menuItem.Title}");
        }
    }
}