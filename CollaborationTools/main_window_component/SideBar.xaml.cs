using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
                Title = "팁 생성",
                MenuType = "Team",
                Action = "TeamCreate"
            },

            new MenuItem
            {
                Title = "팀 맴버",
                MenuType = "Team",
                Action = "Members"
            },

            new MenuItem
            {
                Title = "팀 프로젝트",
                MenuType = "Team",
                Action = "TeamSchedule"
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

    //INotifyPropertyChanged 구현
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    // RelayCommand 구현
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute; //실행할 메서드
        private readonly Func<object, bool> _canExecute; //실행 가능 여부 확인

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}