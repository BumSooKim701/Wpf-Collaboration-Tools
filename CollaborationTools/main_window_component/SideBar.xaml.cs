using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CollaborationTools.calendar;
using CollaborationTools.Common;
using CollaborationTools.database;
using CollaborationTools.team;
using CollaborationTools.user;

namespace CollaborationTools;

public partial class SideBar : UserControl, INotifyPropertyChanged
{
    private readonly TeamService _teamService = new();
    private readonly CalendarService _calendarService = new();
    private readonly TeamRepository _teamRepository = new();
    private ObservableCollection<TabItem> _tabItems;
    private Team _selectedTeam;
    private List<Team> _curUserTeams;
    private byte _curUserAuthority;
    public event EventHandler<Team>? SideBarChanged;

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

    public Team SelectedTeam
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
    
    private byte CurUserAuthority
    {
        get => _curUserAuthority;
        set
        {
            _curUserAuthority = value;
        }
    }
    
    public ICommand MenuClickCommand { get; }
    public ICommand SideTapCommand { get; }
    
    public event PropertyChangedEventHandler PropertyChanged;
    
    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count > 0 && e.AddedItems[0] is TabItem tabItem)
        {
            OnSideTapClick(tabItem);
        }
    }


    private void InitializeMenuItems()
    {
        TabItems = new ObservableCollection<TabItem>();
        
        AddPersonalTab();
        
        foreach (var team in CurUserTeams)
        {
            AddTeamTab(team);
        }
        
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
        TabItem newTeamTab = new TabItem
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
                    Title = "팀 맴버 등록",
                    MenuType = MenuType.Team,
                    Action = "MemberRegistration"
                }
            }
        };
        
        int insertIndex = TabItems.Count;
        TabItems.Insert(insertIndex, newTeamTab);
    }

    public void DeleteTeam(Team team)
    {
        bool result1 = _teamService.RemoveAllTeamMember(team);
        _ = _calendarService.DeleteCalendarAsync(team.teamCalendarId);
        bool result2 = _teamService.RemoveTeam(team);
        
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
        // 현재 TabItems를 완전히 비움
        TabItems.Clear();
        
        // 개인 탭 다시 추가 (첫 번째 위치)
        AddPersonalTab();
        
        // 사용자의 팀 목록 다시 가져오기
        _curUserTeams = _teamService.FindUsersTeams(UserSession.CurrentUser);
        
        // 팀 탭들 다시 추가
        foreach (var team in _curUserTeams)
        {
            AddTeamTab(team);
        }
        
        // 플러스 탭 다시 추가 (마지막 위치)
        AddPlusTab();
    }
    
    private void OnMenuClick(object parameter)
    {
        if (parameter is MenuItem menuItem)
        {
            // 메뉴 클릭 처리 로직
            MessageBox.Show($"{menuItem.MenuType} - {menuItem.Title} 클릭됨");
            
            switch (menuItem.Action)
            {
                case "TeamInfo":
                    OpenTeamInfoWindow();
                    break;
                case "TeamDelete":
                    if (CurUserAuthority == TeamMember.TEAM_LEADER_AUTHORITY)
                    {
                        DeleteTeam(SelectedTeam);
                    }
                    else
                    {
                        MessageBox.Show("권한이 없습니다.");
                    }
                    break;
                case "MemberRegistration":
                    if (CurUserAuthority == TeamMember.TEAM_LEADER_AUTHORITY)
                    {
                        OpenTeamMemberRegistrationWindow();
                    }
                    else
                    {
                        MessageBox.Show("권한이 없습니다.");
                    }
                    break;
            }
        }
    }

    private void OnSideTapClick(object parameter)
    {
        if (parameter is TabItem tabItem)
        {
            switch (tabItem.Type)
            {
                case "Team":
                    SelectedTeam = tabItem.CurTeam;
                    CurUserAuthority = _teamService.FindAuthority(SelectedTeam, UserSession.CurrentUser);
                    SideBarChanged?.Invoke(this, SelectedTeam);
                    break;
                case "Plus":
                    OpenTeamCreateWindow();
                    break;
            }
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
            var calendar = await _calendarService.CreateCalendarAsync(teamCreateWindow.TeamName, teamCreateWindow.TeamDescription);
            
            if (calendar != null)
            {
                string calendarId = calendar.Id;
                
                Team newTeam = _teamService.FindTeamByUuid(teamCreateWindow.Uuid);
                
                // _teamRepository.UpdateTeamCalendarId(newTeam.teamId, calendarId);

                _teamService.UpdateTeamCalendarId(newTeam, calendarId);
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

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}