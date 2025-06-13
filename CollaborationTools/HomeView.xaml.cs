using System.Windows;
using System.Windows.Controls;
using CollaborationTools.team;

namespace CollaborationTools;

public partial class HomeView : UserControl
{
    private HomeViewModel _viewModel;
    
    
    public HomeView()
    {
        InitializeComponent();
        // _meeting = new Meeting()
        // {
        //     Title = "윈프 종료 발표 회의",
        //     ToDo = "최종 보고서 작성\nppt 준비\n발표 내용 정하기\n시연 동영상",
        // };
        _viewModel = new HomeViewModel();
        DataContext = _viewModel;
    }

    public Team CurrentTeam
    {
        get => (Team)GetValue(CurrentTeamProperty);
        set => SetValue(CurrentTeamProperty, value);
    }

    
    public static readonly DependencyProperty CurrentTeamProperty =
        DependencyProperty.Register(
            nameof(CurrentTeam),
            typeof(Team),
            typeof(HomeView),
            new PropertyMetadata(null, OnCurrentTeamChanged));
    
    private static void OnCurrentTeamChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is HomeView homeView && homeView.DataContext is HomeViewModel homeViewModel)
        {
            homeViewModel.CurrentTeam = (Team)e.NewValue;
        }
    }

}