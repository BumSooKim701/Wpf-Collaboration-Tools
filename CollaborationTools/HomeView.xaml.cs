using System.Windows;
using System.Windows.Controls;
using CollaborationTools.meeting_schedule;
using CollaborationTools.team;
using CollaborationTools.user;

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
            if (homeViewModel.CurrentTeam == null)
            {
                homeView.MeetingSection.Visibility = Visibility.Collapsed;
            }
            else
            {
                homeView.MeetingSection.Visibility = Visibility.Visible;
                homeView.SwitchMeetingView();
            }
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
                MeetingDeleteButton.Visibility = Visibility.Collapsed;
                MeetingRefreshButton.Visibility = Visibility.Visible;
                break;
            case MeetingViewType.Arranging:
                MeetingView_TextBlock.Text = "조율 중인 일정";
                NoPlanView.Visibility = Visibility.Collapsed;
                ArrangingView.Visibility = Visibility.Visible;
                ScheduledView.Visibility = Visibility.Collapsed;
                MeetingDeleteButton.Visibility = Visibility.Visible;
                MeetingRefreshButton.Visibility = Visibility.Collapsed;
                LoadPersonalSchedule();
                break;
            case MeetingViewType.Scheduled:
                MeetingView_TextBlock.Text = "예정된 미팅 일정";
                NoPlanView.Visibility = Visibility.Collapsed;
                ArrangingView.Visibility = Visibility.Collapsed;
                ScheduledView.Visibility = Visibility.Visible;
                MeetingDeleteButton.Visibility = Visibility.Collapsed;
                MeetingRefreshButton.Visibility = Visibility.Collapsed;
                break;
        }
    }

    private void LoadPersonalSchedule()
    {
        var meetingService = new MeetingService();
        var personalScheduleList = meetingService.GetPersonalSchedule(UserSession.CurrentUser.userId, CurrentTeam.teamId);
        
        var scheduleList = personalScheduleList.Schedules;
        
        _viewModel.FormattedSchedules.Clear();
        foreach (var schedule in scheduleList)
        {
            var formattedSchedule = new FormattedSchedule
            {
                Date = schedule.Date.ToString("yyyy-MM-dd"),
                StartTime = schedule.StartDateTime.ToString("HH:mm"),
                EndTime = schedule.EndDateTime.ToString("HH:mm")
            };
            _viewModel.FormattedSchedules.Add(formattedSchedule);
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
            _viewModel.Meeting.MeetingId = meetingPlan.MeetingId;
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
    
    private void Show(Window window)
    {
        window.Owner = Application.Current.MainWindow;
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        window.Show();
    }

    private void PersonalSubmit_ButtonClicked(object sender, RoutedEventArgs e)
    {
        PersonalScheduleSubmitWindow submitWindow = new(_viewModel.Meeting);
        submitWindow.PersonalScheduleSaved += (s, args) =>
        {
            LoadPersonalSchedule();
        };
        ShowDialog(submitWindow);
    }

    private void CheckAvailable_ButtonClicked(object sender, RoutedEventArgs e)
    {
        MeetingAvailableTimeWindow availableTimeWindow = new(CurrentTeam.teamId);
        
        Show(availableTimeWindow);
    }

    private void MeetingDelete_ButtonClicked(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "등록된 미팅 조율하기를 삭제하시겠습니까?",
            "미팅 조율 삭제",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            var meetingService = new MeetingService();
            bool isSucceed = meetingService.DeleteMeeting(_viewModel.Meeting.MeetingId);

            MessageBox.Show(Application.Current.MainWindow, isSucceed ? "정상적으로 삭제되었습니다." : "삭제에 실패하였습니다.");
            if (isSucceed)
            {
                _viewModel.ViewType = MeetingViewType.NoPlan; 
                SwitchMeetingView();
            }
        }
        
    }

    private void MeetingRefresh_ButtonClicked(object sender, RoutedEventArgs e)
    {
        _viewModel.UpdateMeeting();
        SwitchMeetingView();
    }
}