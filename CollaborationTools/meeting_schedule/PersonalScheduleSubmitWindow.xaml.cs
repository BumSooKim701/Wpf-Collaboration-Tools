using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CollaborationTools.Common;
using CollaborationTools.user;

namespace CollaborationTools.meeting_schedule;

public partial class PersonalScheduleSubmitWindow : Window
{
    private ScheduleMainViewModel _mainViewModel;
    private Meeting _meeting;
    public EventHandler PersonalScheduleSaved;
    
    public PersonalScheduleSubmitWindow(Meeting meeting)
    {
        InitializeComponent();
        _meeting = meeting;
        _mainViewModel = new ScheduleMainViewModel(meeting);
        DataContext = _mainViewModel;
    }


    private async void SubmitButton_Click(object sender, RoutedEventArgs e)
    {
        ObservableCollection<ScheduleRowViewModel> rowViewModels = _mainViewModel.ScheduleRows;
        PersonalScheduleList personalSchedules = new();
        
        foreach (var rowViewModel in rowViewModels)
        {
            var date = rowViewModel.Date;
            var isAllDay = rowViewModel.IsAllDay;
            var timeRanges = rowViewModel.TimeRanges; 
            personalSchedules.UserId = UserSession.CurrentUser.userId;
            
            if (isAllDay)
            {
                personalSchedules.AddSchedule(date, DateTime.MinValue, DateTime.MinValue);
            }
            else
            {
                foreach (var timeRange in timeRanges)
                {
                    personalSchedules.AddSchedule(date, timeRange.StartTime, timeRange.EndTime);   
                }
            }
        }

        personalSchedules.TeamId = _meeting.TeamId;
        personalSchedules.MeetingScheduleId = _meeting.MeetingId;

        var meetingService = new MeetingService();
        bool isSucceed = meetingService.RegisterPersonalSchedule(personalSchedules);

        MessageBox.Show(Application.Current.MainWindow, isSucceed ? "등록에 성공하였습니다." : "등록에 실패하였습니다");

        if (isSucceed)
        {
            PersonalScheduleSaved?.Invoke(this, EventArgs.Empty);
            this.Close();
        }
            
    }
    
    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            this.DragMove();
        }
    }
    
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}