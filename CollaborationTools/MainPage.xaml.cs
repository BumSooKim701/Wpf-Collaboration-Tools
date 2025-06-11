using System.Windows;
using System.Windows.Controls;
using CollaborationTools.team;

namespace CollaborationTools;

public partial class MainPage : Page
{
    private Team _currentTeam;
    
    public MainPage()
    {
        InitializeComponent();
        MenuBar.MenuChanged += MenuBar_MenuChanged;
        SideBar.SideBarChanged += SideBar_SideBarChanged;
    }
    
    private void MenuBar_MenuChanged(object? sender, MenuType menuType)
    {
        MessageBox.Show($"{menuType.ToString()} 선택됨.");
        
        switch (menuType)
        {
            case MenuType.Home:
                break;
            case MenuType.Calendar:
                MainFrame.Content = new calendar.TeamCalendar();
                break;
            case MenuType.File:
                break;
            case MenuType.Memo:
                if (_currentTeam == null)
                    MessageBox.Show("팀을 선택해 주세요.");
                else
                    MainFrame.Content = new memo.TeamMemo(_currentTeam.teamId);
                break;
        }
    }

    private void SideBar_SideBarChanged(object? sender, Team team)
    {
        _currentTeam = team;
    }
}