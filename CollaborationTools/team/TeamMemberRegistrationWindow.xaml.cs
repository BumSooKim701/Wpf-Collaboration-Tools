using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using CollaborationTools.Common;
using CollaborationTools.file;
using CollaborationTools.user;
using CalendarService = CollaborationTools.calendar.CalendarService;

namespace CollaborationTools.team;

public partial class TeamMemberRegistrationWindow : Window, INotifyPropertyChanged
{
    private readonly CalendarService _calendarService = new();
    private readonly FolderService _folderService = new();
    private readonly TeamService _teamService = new();
    private readonly UserService _userService = new();
    private string _newMemberEmail;
    private Team _team;
    private ObservableCollection<string> _teamMembers;

    public TeamMemberRegistrationWindow(Team team)
    {
        InitializeComponent();

        Team = team;

        TeamMembers = new ObservableCollection<string>();

        AddMemberCommand = new RelayCommand(AddMember, CanAddMember);
        RemoveMemberCommand = new RelayCommand(RemoveMember);
        RegisterMembersCommand = new RelayCommand(RegisterMembers, CanRegisterMembers);
        CancelCommand = new RelayCommand(Cancel);

        // 데이터 컨텍스트 설정
        DataContext = this;
    }

    public Team Team
    {
        get => _team;
        set
        {
            _team = value;
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

    public ICommand AddMemberCommand { get; }
    public ICommand RemoveMemberCommand { get; }
    public ICommand RegisterMembersCommand { get; }
    public ICommand CancelCommand { get; }

    public event PropertyChangedEventHandler PropertyChanged;

    // 멤버 추가 가능 여부 검사
    private bool CanAddMember(object parameter)
    {
        return !string.IsNullOrWhiteSpace(NewMemberEmail) && !TeamMembers.Contains(NewMemberEmail)
                                                          && _teamService.IsValidEmail(NewMemberEmail);
    }

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
            MessageBox.Show("존재하지 않는 사용자입니다.", "팀원 추가 실패", MessageBoxButton.OK, MessageBoxImage.Error);
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

    private bool CanRegisterMembers(object parameter)
    {
        return TeamMembers.Count > 0;
    }

    private void RegisterMembers(object parameter)
    {
        var success = _teamService.AddTeamMembersByEmail(Team, TeamMembers, TeamMember.TEAM_MEMBER_AUTHORITY);

        if (success)
        {
            _calendarService.CreateCalendarAsync(Team.teamCalendarId, Team.teamName);

            _folderService.ShareFolderWithMemberAsync(Team.teamCalendarId, NewMemberEmail);

            MessageBox.Show($"'{Team.teamName}' 팀에 {TeamMembers.Count}명의 팀원이 등록되었습니다.",
                "팀원 등록 완료", MessageBoxButton.OK, MessageBoxImage.Information);

            // 창 닫기
            DialogResult = true;
        }
        else
        {
            MessageBox.Show("팀원 등록 중 오류가 발생했습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
        }
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