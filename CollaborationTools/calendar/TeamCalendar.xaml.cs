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
    private ObservableCollection<ScheduleItem> _schedules = new ObservableCollection<ScheduleItem>();
    private ObservableCollection<ScheduleItem> _oneDaySchedules = new ObservableCollection<ScheduleItem>();
    private string _calendarId;
    private static string[] _dayOfWeek = { "일", "월", "화", "수", "목", "금", "토"};

    public TeamCalendar()
    {
        InitializeComponent();
        _calendarId = "primary";
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
                
                string startDateTimeStr = getDateTimeStr(startDateTime, isAllDayEventStart); 
                string endDateTimeStr = getDateTimeStr(endDateTime, isAllDayEventEnd); 
                
                bool isOneDayEvent = (startDateTime.Date == endDateTime.Date);
                
                ScheduleItem schedule = new ScheduleItem();
                schedule.Title = eventItem.Summary;
                schedule.Date = startDateTimeStr;  
                schedule.DateDetails = getDateTimeTermStr(startDateTimeStr, endDateTimeStr, endDateTime, isOneDayEvent);
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
                ScheduleDetailsDialog scheduleDetailsDialog = new ScheduleDetailsDialog();
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
                
                string startDateTimeStr = getDateTimeStr(startDateTime, isAllDayEventStart); 
                string endDateTimeStr = getDateTimeStr(endDateTime, isAllDayEventEnd); 
                
                bool isOneDayEvent = (startDateTime.Date == endDateTime.Date);
                
                ScheduleItem schedule = new ScheduleItem();
                schedule.Title = eventItem.Summary;
                schedule.Date = startDateTimeStr;  
                schedule.DateDetails = getDateTimeTermStr(startDateTimeStr, endDateTimeStr, endDateTime, isOneDayEvent);
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
    
    
    private string getDateTimeStr(DateTime dateTime, bool isAllDayEvent)
    {
        string dateTimeStr = isAllDayEvent ? 
            String.Format("{0}월 {1}일 ({2})", dateTime.Month, dateTime.Day, _dayOfWeek[(int)dateTime.DayOfWeek])
            : String.Format("{0}월 {1}일 ({2}) {3:D2}:{4:D2}", dateTime.Month, dateTime.Day, _dayOfWeek[(int)dateTime.DayOfWeek], dateTime.Hour, dateTime.Minute);

        return dateTimeStr;
    }

    private string getDateTimeTermStr(string startDateTimeStr, string endDateTimeStr, DateTime endDateTime, bool isOneDayEvent)
    {
        string dateTimeTermStr = isOneDayEvent ?  
            String.Format("{0} ~ {1:D2}:{2:D2}", startDateTimeStr, endDateTime.Hour, endDateTime.Minute)
            : String.Format("{0} ~ {1}", startDateTimeStr, endDateTimeStr);
        
        return dateTimeTermStr;
    }
}