using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using CollaborationTools.Common;
using CollaborationTools.user;

namespace CollaborationTools.team;

public partial class TeamCreateWindow : Window, INotifyPropertyChanged
{
    private readonly TeamService _teamService = new();
    private readonly UserService _userService = new();
    private string _newMemberEmail;
    private string _teamDescription;
    private ObservableCollection<string> _teamMembers;
    private string _teamName;

    public TeamCreateWindow()
    {
        InitializeComponent();

        // 데이터 초기화
        TeamMembers = new ObservableCollection<string>();

        // 명령 초기화
        AddMemberCommand = new RelayCommand(AddMember, CanAddMember);
        RemoveMemberCommand = new RelayCommand(RemoveMember);
        CreateTeamCommand = new RelayCommand(CreateTeam, CanCreateTeam);
        CancelCommand = new RelayCommand(Cancel);

        // 데이터 컨텍스트 설정
        DataContext = this;
    }

    public string TeamName
    {
        get => _teamName;
        set
        {
            _teamName = value;
            OnPropertyChanged();
        }
    }

    public string TeamDescription
    {
        get => _teamDescription;
        set
        {
            _teamDescription = value;
            OnPropertyChanged();
        }
    }

    public string NewMemberEmail
    {
        get => _newMemberEmail;
        set
        {
            _newMemberEmail = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<string> TeamMembers
    {
        get => _teamMembers;
        set
        {
            _teamMembers = value;
            OnPropertyChanged();
        }
    }

    public string Uuid { get; set; }

    public ICommand AddMemberCommand { get; }
    public ICommand RemoveMemberCommand { get; }
    public ICommand CreateTeamCommand { get; }
    public ICommand CancelCommand { get; }

    // INotifyPropertyChanged 구현
    public event PropertyChangedEventHandler PropertyChanged;

    //멤버 추가 가능 여부 검사
    private bool CanAddMember(object parameter)
    {
        return !string.IsNullOrWhiteSpace(NewMemberEmail) && !TeamMembers.Contains(NewMemberEmail)
                                                          && _teamService.IsValidEmail(NewMemberEmail);
    }

    //멤버 추가 기능
    private void AddMember(object parameter)
    {
        var isValidUser = _userService.IsExistUser(NewMemberEmail);

        if (!string.IsNullOrWhiteSpace(NewMemberEmail) && !TeamMembers.Contains(NewMemberEmail) && isValidUser)
        {
            TeamMembers.Add(NewMemberEmail);
            NewMemberEmail = string.Empty;
        }
        else if (!isValidUser)
        {
            MessageBox.Show("존재하지 않는 사용자 입니다.", "팀원 추가 실패", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else
        {
            MessageBox.Show("팀원 추가를 실패했습니다.", "팀원 추가 실패", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void RemoveMember(object parameter)
    {
        if (parameter is string email) TeamMembers.Remove(email);
    }

    private bool CanCreateTeam(object parameter)
    {
        return !string.IsNullOrWhiteSpace(TeamName);
    }

    private void CreateTeam(object parameter)
    {
        Team newTeam;
        var success = _teamService.CreateTeam(
            newTeam = new Team
            {
                teamName = _teamName,
                teamMemberCount = 1,
                teamCalendarName = _teamName,
                teamCalendarId = _teamName,
                teamDescription = _teamDescription,
                visibility = 1
            });

        if (success)
        {
            Uuid = newTeam.uuid;
            var curUserSuccess = _teamService.AddTeamMemberByEmail(newTeam, UserSession.CurrentUser.Email,
                TeamMember.TEAM_LEADER_AUTHORITY);
            var teamMemberSuccess =
                _teamService.AddTeamMembersByEmail(newTeam, TeamMembers, TeamMember.TEAM_MEMBER_AUTHORITY);

            if (curUserSuccess && teamMemberSuccess)
                MessageBox.Show($"팀 '{newTeam.teamName}'이(가) 생성되었습니다.\n팀원: {string.Join(",\n", TeamMembers)}",
                    "팀 생성 완료", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("팀원 등록 중 오류가 발생했습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else
        {
            MessageBox.Show("팀 생성 중 오류가 발생했습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        // 창 닫기
        DialogResult = true;
    }

    private void Cancel(object parameter)
    {
        // 창 닫기
        DialogResult = false;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}