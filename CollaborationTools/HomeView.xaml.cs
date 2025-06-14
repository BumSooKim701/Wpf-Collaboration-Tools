using System.Windows;
using System.Windows.Controls;
using CollaborationTools.meeting_schedule;
using CollaborationTools.team;

namespace CollaborationTools;

public partial class HomeView : UserControl
{
    public static readonly DependencyProperty CurrentTeamProperty =
        DependencyProperty.Register(
            nameof(CurrentTeam),
            typeof(Team),
            typeof(HomeView),
            new PropertyMetadata(null, OnCurrentTeamChanged));

    private readonly HomeViewModel _viewModel;


    public HomeView()
    {
        InitializeComponent();
        _viewModel = new HomeViewModel();
        DataContext = _viewModel;
    }

    public Team CurrentTeam
    {
        get => (Team)GetValue(CurrentTeamProperty);
        set => SetValue(CurrentTeamProperty, value);
    }

    private static void OnCurrentTeamChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is HomeView homeView && homeView.DataContext is HomeViewModel homeViewModel)
        {
            homeViewModel.CurrentTeam = (Team)e.NewValue;

            // 미팅 일정잡기 화면 / 조율 중인 미팅 표시 화면 / 예정된 미팅 일정 화면
            switch (homeViewModel.ViewType)
            {
                case MeetingViewType.NoPlan:
                    homeView.MeetingView_TextBlock.Text = "예정된 미팅 일정";
                    homeView.NoPlanView.Visibility = Visibility.Visible;
                    homeView.ArrangingView.Visibility = Visibility.Collapsed;
                    homeView.ScheduledView.Visibility = Visibility.Collapsed;
                    break;
                case MeetingViewType.Arranging:
                    homeView.MeetingView_TextBlock.Text = "조율 중인 일정";
                    homeView.NoPlanView.Visibility = Visibility.Collapsed;
                    homeView.ArrangingView.Visibility = Visibility.Visible;
                    homeView.ScheduledView.Visibility = Visibility.Collapsed;
                    break;
                case MeetingViewType.Scheduled:
                    homeView.MeetingView_TextBlock.Text = "예정된 미팅 일정";
                    homeView.NoPlanView.Visibility = Visibility.Collapsed;
                    homeView.ArrangingView.Visibility = Visibility.Collapsed;
                    homeView.ScheduledView.Visibility = Visibility.Visible;
                    break;
            }
        }
    }

    private void MeetingArrange_ButtonClicked(object sender, RoutedEventArgs e)
    {
        var meetingArrangeWindow = new MeetingArrangeWindow(_viewModel.CurrentTeam.teamId);
        Show(meetingArrangeWindow);
    }

    private void Show(Window window)
    {
        window.Owner = Application.Current.MainWindow;
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        window.Show();
    }
}