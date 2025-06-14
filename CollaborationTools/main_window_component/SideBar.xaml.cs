using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CollaborationTools.address;
using CollaborationTools.calendar;
using CollaborationTools.Common;
using CollaborationTools.file;
using CollaborationTools.team;
using CollaborationTools.user;
using MaterialDesignThemes.Wpf;

namespace CollaborationTools;

public partial class SideBar : UserControl, INotifyPropertyChanged
{
    private readonly CalendarService _calendarService = new();
    private readonly FolderService _folderService = new();
    private readonly TeamService _teamService = new();
    private List<Team> _curUserTeams;
    private Team _selectedTeam;
    private ObservableCollection<TabItem> _tabItems;

    public SideBar()
    {
        InitializeComponent();

        _curUserTeams = _teamService.FindUsersTeams(UserSession.CurrentUser);

        MenuClickCommand = new RelayCommand(OnMenuClick);
        SideTapCommand = new RelayCommand(OnSideTapClick);

        InitializeMenuItems();

        DataContext = this;
    }

    public ObservableCollection<TabItem> TabItems
    {
        get => _tabItems;
        set
        {
            _tabItems = value;
            OnPropertyChanged();
        }
    }

    public Team? SelectedTeam
    {
        get => _selectedTeam;
        set
        {
            _selectedTeam = value;
            OnPropertyChanged();
        }
    }

    private List<Team> CurUserTeams
    {
        get => _curUserTeams;
        set
        {
            _curUserTeams = value;
            OnPropertyChanged();
        }
    }

    private byte CurUserAuthority { get; set; }

    public ICommand MenuClickCommand { get; }
    public ICommand SideTapCommand { get; }

    public event PropertyChangedEventHandler PropertyChanged;
    public event EventHandler<SideBarChangedEventArgs>? SideBarChanged;

    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count > 0 && e.AddedItems[0] is TabItem tabItem) OnSideTapClick(tabItem);
    }

    private void InitializeMenuItems()
    {
        TabItems = new ObservableCollection<TabItem>();

        AddPersonalTab();

        foreach (var team in CurUserTeams)
            if (team.visibility == 1)
                AddTeamTab(team);

        AddPlusTab();
    }

    private void AddPersonalTab()
    {
        var personalTab = new TabItem
        {
            Header = "개인",
            IconKind = "AccountCircle",
            Title = "개인 메뉴",
            Type = "Personal",
            CurTeam = null,
            MenuItems = new ObservableCollection<MenuItem>
            {
                new()
                {
                    Title = "내 프로필",
                    MenuType = MenuType.Personal,
                    Action = "Profile"
                },
                new()
                {
                    Title = "내 주소록",
                    MenuType = MenuType.Personal,
                    Action = "AddressBook"
                }
            }
        };
        TabItems.Add(personalTab);
    }

    private void AddPlusTab()
    {
        var addTeamTab = new TabItem
        {
            Header = null,
            IconKind = "PlusBox",
            Title = null,
            CurTeam = null,
            Type = "Plus"
        };
        TabItems.Add(addTeamTab);
    }

    private void AddTeamTab(Team team)
    {
        var newTeamTab = new TabItem
        {
            Header = team.teamName,
            IconKind = "AccountGroup",
            Title = "팀 메뉴",
            Type = "Team",
            CurTeam = team,
            MenuItems = new ObservableCollection<MenuItem>
            {
                new()
                {
                    Title = "팀 정보",
                    MenuType = MenuType.Team,
                    Action = "TeamInfo"
                },
                new()
                {
                    Title = "팀 삭제",
                    MenuType = MenuType.Team,
                    Action = "TeamDelete"
                },
                new()
                {
                    Title = "팀 멤버 등록",
                    MenuType = MenuType.Team,
                    Action = "MemberRegistration"
                }
            }
        };

        var insertIndex = TabItems.Count;
        TabItems.Insert(insertIndex, newTeamTab);
    }

    public void DeleteTeam(Team team)
    {
        Console.WriteLine($"teamId: {team.teamCalendarId}" + " " + team.teamFolderId);

        var result1 = _teamService.RemoveAllTeamMember(team);

        _calendarService.DeleteCalendarAsync(team.teamCalendarId);

        if (!string.IsNullOrEmpty(team.teamFolderId))
        {
            Console.WriteLine($"teamFolderId: {team.teamFolderId}");
            _folderService.DeleteFolderWithContentsAsync(team.teamFolderId);
        }

        var result2 = _teamService.RemoveTeam(team);

        if (result1 && result2)
        {
            MessageBox.Show("팀 삭제 완료");
            RefreshTeamList();
        }
        else
        {
            MessageBox.Show("팀 삭제 실패");
        }
    }

    private void RefreshTeamList()
    {
        TabItems.Clear();

        AddPersonalTab();

        _curUserTeams = _teamService.FindUsersTeams(UserSession.CurrentUser);

        foreach (var team in _curUserTeams)
            if (team.visibility == 1)
                AddTeamTab(team);

        AddPlusTab();
    }

    private void OnMenuClick(object parameter)
    {
        if (parameter is MenuItem menuItem)
            // 메뉴 클릭 처리 로직
            // MessageBox.Show($"{menuItem.MenuType} - {menuItem.Title} 클릭됨");
            switch (menuItem.Action)
            {
                case "TeamInfo":
                    OpenTeamInfoWindow();
                    break;
                case "TeamDelete":
                    var result = MessageBox.Show(
                        "팀을 삭제하시겠습니까?",
                        "삭제 확인",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        if (CurUserAuthority == TeamMember.TEAM_LEADER_AUTHORITY)
                            DeleteTeam(SelectedTeam);
                        else
                            MessageBox.Show("권한이 없습니다.");
                    }

                    break;
                case "MemberRegistration":
                    if (CurUserAuthority == TeamMember.TEAM_LEADER_AUTHORITY)
                        OpenTeamMemberRegistrationWindow();
                    else
                        MessageBox.Show("권한이 없습니다.");
                    break;
                case "AddressBook":
                    OpenAddressBookWindow();
                    break;
            }
    }

    private void OnSideTapClick(object parameter)
    {
        if (parameter is TabItem tabItem)
            switch (tabItem.Type)
            {
                case "Team":
                    SelectedTeam = tabItem.CurTeam;
                    CurUserAuthority = _teamService.FindAuthority(SelectedTeam, UserSession.CurrentUser);
                    SideBarChanged?.Invoke(this, new SideBarChangedEventArgs
                    {
                        TabType = "Team",
                        Team = SelectedTeam,
                        User = null
                    });
                    break;
                case "Plus":
                    OpenTeamCreateWindow();
                    break;
                case "Personal":
                    SelectedTeam = null;
                    CurUserAuthority = 1;
                    SideBarChanged?.Invoke(this, new SideBarChangedEventArgs
                    {
                        TabType = "Personal",
                        Team = null,
                        User = UserSession.CurrentUser
                    });
                    break;
            }
    }

    private void OpenTeamInfoWindow()
    {
        var teamInfoWindow = new TeamInfoWindow(SelectedTeam);

        var result = teamInfoWindow.ShowDialog();

        if (result == true)
        {
        }
    }

    private async Task OpenTeamCreateWindow()
    {
        // 팀 생성 창 인스턴스 생성
        var teamCreateWindow = new TeamCreateWindow();

        // 모달 대화상자로 표시
        var result = teamCreateWindow.ShowDialog();

        // 결과 처리 (팀이 생성되었을 경우)
        if (result == true)
        {
            var calendar =
                await _calendarService.CreateCalendarAsync(teamCreateWindow.TeamName, teamCreateWindow.TeamDescription);

            var sharedFolder = await _folderService.CreateTeamFolderAsync(teamCreateWindow.TeamName);

            if (calendar != null && sharedFolder != null)
            {
                var calendarId = calendar.Id;

                var sharedDriveId = sharedFolder.Id;

                var newTeam = _teamService.FindTeamByUuid(teamCreateWindow.Uuid);

                _teamService.UpdateTeamCalendarId(newTeam, calendarId);

                _teamService.UpdateTeamDriveId(newTeam, sharedDriveId);

                foreach (var memberEmail in teamCreateWindow.TeamMembers)
                {
                    await _calendarService.AddCalendarMemberAsync(calendarId, memberEmail);

                    await _folderService.ShareFolderWithMemberAsync(sharedDriveId, memberEmail);
                }
            }
            else
            {
                Console.WriteLine("Calendar or Shared Drive is null");
            }

            RefreshTeamList();
        }
    }

    private void OpenTeamMemberRegistrationWindow()
    {
        var teamMemberRegistrationWindow = new TeamMemberRegistrationWindow(SelectedTeam);

        var result = teamMemberRegistrationWindow.ShowDialog();

        if (result == true)
        {
        }
    }

    public void OpenAddressBookWindow()
    {
        var addressBookWindow = new AddressBookWindow();

        addressBookWindow.Owner = Application.Current.MainWindow;
        addressBookWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        addressBookWindow.Show();
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}