using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using CollaborationTools.Common;
using CollaborationTools.user;

namespace CollaborationTools.team;

public partial class TeamInfoWindow : Window
{
    private readonly TeamService _teamService = new();
    private readonly UserService _userService = new();
    private Team _team;
    private ObservableCollection<User> _teamMembers;

    public TeamInfoWindow(Team team)
    {
        InitializeComponent();

        // 명령 초기화
        CloseCommand = new RelayCommand(Close);

        // 데이터 컨텍스트 설정
        DataContext = this;

        // 팀 정보 설정
        Team = team;
        Console.WriteLine(Team.teamDescription);

        // 팀 멤버 목록 초기화
        LoadTeamMembers();
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

    public ObservableCollection<User> TeamMembers
    {
        get => _teamMembers;
        set
        {
            _teamMembers = value;
            OnPropertyChanged();
        }
    }

    public ICommand CloseCommand { get; }

    private void LoadTeamMembers()
    {
        try
        {
            // 팀이 null인 경우 처리
            if (Team == null)
            {
                Console.WriteLine("Team is null");
                TeamMembers = new ObservableCollection<User>();
                return;
            }

            // 팀 서비스에서 팀 멤버 목록 가져오기
            var members = _teamService?.FindTeamMembersByTeamId(Team.teamId);
            var users = _userService.FindUsersByTeamMembers(members);

            // members가 null이 아닌 경우에만 ObservableCollection 생성
            if (users != null && users.Count > 0)
                TeamMembers = new ObservableCollection<User>(users!);
            else
                // null인 경우 빈 컬렉션 생성
                TeamMembers = new ObservableCollection<User>();
        }
        catch (Exception ex)
        {
            // 예외 처리 및 로깅
            Console.WriteLine($"Error loading team members: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            TeamMembers = new ObservableCollection<User>(); // 빈 컬렉션으로 초기화
        }
    }

    private void Close(object parameter)
    {
        // 창 닫기
        DialogResult = true;
    }

    // INotifyPropertyChanged 구현
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}