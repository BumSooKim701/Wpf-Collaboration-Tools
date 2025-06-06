using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CollaborationTools.authentication;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using MaterialDesignThemes.Wpf;
using Calendar = System.Globalization.Calendar;

namespace CollaborationTools.calendar;

public partial class TeamCalendar : UserControl
{
    private List<ScheduleItem> _schedules = [];
    private List<ScheduleItem> _oneDaySchedules = [];
    private string _calendarId;
    private static string[] _dayOfWeek = { "일", "월", "화", "수", "목", "금", "토"};

    public TeamCalendar()
    {
        InitializeComponent();
        // _calendarId = "primary";
        _calendarId = "34e62ffc970cfcdeebd447c1d975cda54f3b1fa43673b6e53c65e8bf6b808cf9@group.calendar.google.com";
        LoadScheduleItems();
    }
    public TeamCalendar(string calendarId = "primary")
    {
        InitializeComponent();
        _calendarId = calendarId;
        LoadScheduleItems();
    }

    private async void LoadScheduleItems()
    {
        var calendarService = GoogleAuthentication.CalendarService;
        
        if (calendarService == null)
        {
            throw new InvalidOperationException("먼저 Google에 로그인하세요.");
        }

        // 이벤트 요청 설정
        EventsResource.ListRequest request = calendarService.Events.List(_calendarId);
        request.TimeMin = DateTime.Now;
        request.ShowDeleted = false;
        request.SingleEvents = true;
        request.MaxResults = 10;
        request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
        
        // 비동기로 이벤트 실행
        Events events = await request.ExecuteAsync();

        if (events.Items != null && events.Items.Count > 0)
        {
            foreach (var eventItem in events.Items)
            {
                bool isAllDayEventStart = false;
                string start = eventItem.Start.DateTimeRaw;
                if (start == null)
                {
                    isAllDayEventStart = true;
                    start = eventItem.Start.Date;
                }
                
                bool isAllDayEventEnd = false;
                string end = eventItem.End.DateTimeRaw;
                if (end == null)
                {
                    isAllDayEventEnd = true;
                    end = eventItem.End.Date;
                }
                
                DateTime startDateTime = DateTime.Parse(start);
                DateTime endDateTime = DateTime.Parse(end);
                
                bool isOneDayEvent = (startDateTime.Date == endDateTime.Date);
                
                ScheduleItem schedule = new ScheduleItem();
                schedule.Title = eventItem.Summary;
                schedule.StartDateTime = startDateTime;
                schedule.EndDateTime = endDateTime;
                schedule.IsAllDayEventStart = isAllDayEventStart;
                schedule.IsAllDayEventEnd = isAllDayEventEnd;
                schedule.IsOneDayEvent = isOneDayEvent;
                schedule.Location = eventItem.Location;
                schedule.Description = eventItem.Description;
                _schedules.Add(schedule);
                
            }
            CardListView.ItemsSource = _schedules;
        }
        else
        {
            NoScheduleMsg.Visibility = Visibility.Visible;
        }
        
    }

    private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is ListView listView)
        {
            if (listView.SelectedItem is ScheduleItem scheduleItem)
            {
                ScheduleDetailsWindow scheduleDetailsDialog = new ScheduleDetailsWindow();
                scheduleDetailsDialog.DataContext = scheduleItem;
                ShowDialog(scheduleDetailsDialog);
            }
        }
    }

    private void ShowDialog(Window dialog)
    {
        dialog.Owner = Application.Current.MainWindow;
        dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        dialog.ShowDialog();
    }

    private void CalendarDateChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Calendar.SelectedDate.HasValue)
        {
            DateTime selectedDate = Calendar.SelectedDate.Value;
            _oneDaySchedules.Clear();
            DisplayCalendarSchedule(selectedDate);
        }
    }
    
    private async void DisplayCalendarSchedule(DateTime selectedDate) // 캘린더에서 선택된 날짜 일정
    {
        var calendarService = GoogleAuthentication.CalendarService;
        
        if (calendarService == null)
        {
            throw new InvalidOperationException("먼저 Google에 로그인하세요.");
        }
    
        // 이벤트 요청 설정
        EventsResource.ListRequest request = calendarService.Events.List(_calendarId);
        request.TimeMinDateTimeOffset = selectedDate;
        request.TimeMaxDateTimeOffset = selectedDate.AddDays(1);;
        request.ShowDeleted = false;
        request.SingleEvents = true;
        request.MaxResults = 20;
        request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
        
        // 비동기로 이벤트 실행
        Events events = await request.ExecuteAsync();
    
        if (events.Items != null && events.Items.Count > 0)
        {
            foreach (var eventItem in events.Items)
            {
                bool isAllDayEventStart = false;
                string start = eventItem.Start.DateTimeRaw;
                if (start == null)
                {
                    isAllDayEventStart = true;
                    start = eventItem.Start.Date;
                }
                
                bool isAllDayEventEnd = false;
                string end = eventItem.End.DateTimeRaw;
                if (end == null)
                {
                    isAllDayEventEnd = true;
                    end = eventItem.End.Date;
                }
                
                DateTime startDateTime = DateTime.Parse(start);
                DateTime endDateTime = DateTime.Parse(end);
                
                bool isOneDayEvent = (startDateTime.Date == endDateTime.Date);
                
                ScheduleItem schedule = new ScheduleItem();
                schedule.Title = eventItem.Summary;
                schedule.StartDateTime = startDateTime;  
                schedule.EndDateTime = endDateTime;
                schedule.IsAllDayEventStart = isAllDayEventStart;
                schedule.IsAllDayEventEnd = isAllDayEventEnd;
                schedule.IsOneDayEvent = isOneDayEvent;
                schedule.Location = eventItem.Location;
                schedule.Description = eventItem.Description;
                _oneDaySchedules.Add(schedule);
                
            }
            CardListViewCalendar.ItemsSource = _oneDaySchedules;
        }
        else
        {
            NoScheduleMsgCalendar.Visibility = Visibility.Visible;
        }
    
    }
    
}