using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace CollaborationTools.team;

public partial class TeamCreateWindow : Window, INotifyPropertyChanged
{
    private string _teamName;
    private string _teamDescription;
    private string _newMemberEmail;
    private ObservableCollection<string> _teamMembers;

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

    public ICommand AddMemberCommand { get; }
    public ICommand RemoveMemberCommand { get; }
    public ICommand CreateTeamCommand { get; }
    public ICommand CancelCommand { get; }

    public TeamCreateWindow()
    {
        InitializeComponent();
        
        // 데이터 초기화
        TeamMembers = new ObservableCollection<string>();
        
        // 명령 초기화
        AddMemberCommand = new SideBar.RelayCommand(AddMember, CanAddMember);
        RemoveMemberCommand = new SideBar.RelayCommand(RemoveMember);
        CreateTeamCommand = new SideBar.RelayCommand(CreateTeam, CanCreateTeam);
        CancelCommand = new SideBar.RelayCommand(Cancel);
        
        // 데이터 컨텍스트 설정
        DataContext = this;
    }

    private bool CanAddMember(object parameter)
    {
        return !string.IsNullOrWhiteSpace(NewMemberEmail) && 
               !TeamMembers.Contains(NewMemberEmail);
    }

    private void AddMember(object parameter)
    {
        if (!string.IsNullOrWhiteSpace(NewMemberEmail) && 
            !TeamMembers.Contains(NewMemberEmail))
        {
            TeamMembers.Add(NewMemberEmail);
            NewMemberEmail = string.Empty;
        }
    }

    private void RemoveMember(object parameter)
    {
        if (parameter is string email)
        {
            TeamMembers.Remove(email);
        }
    }

    private bool CanCreateTeam(object parameter)
    {
        return !string.IsNullOrWhiteSpace(TeamName) && TeamMembers.Count > 0;
    }

    private void CreateTeam(object parameter)
    {
        // 실제로는 데이터베이스나 서비스를 통해 팀 생성 로직 구현
        MessageBox.Show($"팀 '{TeamName}'이(가) 생성되었습니다.\n팀원: {string.Join(", ", TeamMembers)}", 
            "팀 생성 완료", MessageBoxButton.OK, MessageBoxImage.Information);
        
        // 창 닫기
        DialogResult = true;
    }

    private void Cancel(object parameter)
    {
        // 창 닫기
        DialogResult = false;
    }

    // INotifyPropertyChanged 구현
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}