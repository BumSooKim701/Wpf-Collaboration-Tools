using System.Windows;
using System.Windows.Controls;
using CollaborationTools.meeting_schedule;
using CollaborationTools.team;

namespace CollaborationTools;

public partial class HomeView : UserControl
{
    private HomeViewModel _viewModel;
    
    
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
            
            homeView.SwitchMeetingView();
        }
    }

    private void SwitchMeetingView()
    {
        // 미팅 일정잡기 화면 / 조율 중인 미팅 표시 화면 / 예정된 미팅 일정 화면
        switch (_viewModel.ViewType)
        {
            case MeetingViewType.NoPlan:
                MeetingView_TextBlock.Text = "예정된 미팅 일정";
                NoPlanView.Visibility = Visibility.Visible;
                ArrangingView.Visibility = Visibility.Collapsed;
                ScheduledView.Visibility = Visibility.Collapsed;
                break;
            case MeetingViewType.Arranging:
                MeetingView_TextBlock.Text = "조율 중인 일정";
                NoPlanView.Visibility = Visibility.Collapsed;
                ArrangingView.Visibility = Visibility.Visible;
                ScheduledView.Visibility = Visibility.Collapsed;
                break;
            case MeetingViewType.Scheduled:
                MeetingView_TextBlock.Text = "예정된 미팅 일정";
                NoPlanView.Visibility = Visibility.Collapsed;
                ArrangingView.Visibility = Visibility.Collapsed;
                ScheduledView.Visibility = Visibility.Visible;
                break;
        }
    }

    private void MeetingArrange_ButtonClicked(object sender, RoutedEventArgs e)
    {
        var meetingArrangeWindow = new MeetingArrangeWindow(_viewModel.CurrentTeam.teamId);
        meetingArrangeWindow.MeetingCreated += (s, meetingPlan) =>
        {
            _viewModel.Meeting.Title = meetingPlan.Title;
            _viewModel.Meeting.ToDo = meetingPlan.ToDo;
            _viewModel.Meeting.Status = meetingPlan.Status;
            _viewModel.Meeting.TeamId = meetingPlan.TeamId;
            _viewModel.ViewType = MeetingViewType.Arranging;
            SwitchMeetingView();
        };
        ShowDialog(meetingArrangeWindow);
    }
    
    private void ShowDialog(Window window)
    {
        window.Owner = Application.Current.MainWindow;
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        window.ShowDialog();
    }

    private void PersonalSubmit_ButtonClicked(object sender, RoutedEventArgs e)
    {
        PersonalScheduleSubmitWindow submitWindow = new(_viewModel.Meeting);
        
        ShowDialog(submitWindow);
    }
}