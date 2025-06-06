using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CollaborationTools.Common;
using CollaborationTools.team;

namespace CollaborationTools;

public partial class SideBar : UserControl, INotifyPropertyChanged
{
    private ObservableCollection<MenuItem> _personalMenuList;
    private ObservableCollection<MenuItem> _teamMenuList;
    
    public ObservableCollection<MenuItem> PersonalMenuList
    {
        get => _personalMenuList;
        set
        {
            _personalMenuList = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<MenuItem> TeamMenuList
    {
        get => _teamMenuList;
        set
        {
            _teamMenuList = value;
            OnPropertyChanged();
        }
    }
    
    public ICommand MenuClickCommand { get; }
    
    public SideBar()
    {
        InitializeComponent();

        MenuClickCommand = new RelayCommand(OnMenuClick);
        InitializeMenuItems();
        
        DataContext = this;
    }
    
    private void InitializeMenuItems()
    {
        PersonalMenuList = new ObservableCollection<MenuItem>
        {
            new MenuItem
            {
                Title = "내 프로필",
                MenuType = "Personal",
                Action = "Profile"
            }
        };

        TeamMenuList = new ObservableCollection<MenuItem>
        {
            new MenuItem
            {
                Title = "팀 정보",
                MenuType = "Team",
                Action = "TeamInfo"
            },
            
            new MenuItem
            {
                Title = "팀 생성",
                MenuType = "Team",
                Action = "TeamCreate"
            },
            
            new MenuItem
            {
                Title = "팀 삭제",
                MenuType = "Team",
                Action = "TeamDelete"
            },

            new MenuItem
            {
                Title = "팀 맴버 등록",
                MenuType = "Team",
                Action = "MemberRegistration"
            },
            
            new MenuItem
            {
                Title = "팀 맴버 조회",
                MenuType = "Team",
                Action = "MemberSearch"
            },

            new MenuItem
            {
                Title = "팀 프로젝트",
                MenuType = "Team",
                Action = "TeamProject"
            }
        };
    }
    
    private void OnMenuClick(object parameter)
    {
        if (parameter is MenuItem menuItem)
        {
            // 메뉴 클릭 처리 로직
            MessageBox.Show($"{menuItem.MenuType} - {menuItem.Title} 클릭됨");
            
            // 실제 구현에서는 Navigation이나 다른 처리를 할 수 있습니다
            switch (menuItem.Action)
            {
                case "TeamCreate":
                    OpenTeamCreateWindow();
                    break;
                case "TeamDelete":
                    break;
            }
        }
    }
    
    private void OpenTeamCreateWindow()
    {
        // 팀 생성 창 인스턴스 생성
        var teamCreateWindow = new TeamCreateWindow();
    
        // 모달 대화상자로 표시
        bool? result = teamCreateWindow.ShowDialog();
    
        // 결과 처리 (팀이 생성되었을 경우)
        if (result == true)
        {
            
        }
    }
    
    private void OpenTeamDeleteWindow()
    {
        
    }

    
    // private void OnPersonalMenuClick(object sender, RoutedEventArgs e)
    // {
    //     if (sender is Button button)
    //     {
    //         MessageBox.Show($"개인 메뉴 - {button.Content} 클릭됨");
    //     }
    // }
    //
    // private void OnTeamMenuClick(object sender, RoutedEventArgs e)
    // {
    //     if (sender is Button button)
    //     {
    //         MessageBox.Show($"팀 메뉴 - {button.Content} 클릭됨");
    //     }
    // }
    
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}