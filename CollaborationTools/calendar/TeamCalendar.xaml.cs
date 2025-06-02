using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using CollaborationTools.google_login;
using CollaborationTools.google_user;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;

namespace CollaborationTools;

public partial class TeamCalendar : UserControl
{
    private ObservableCollection<ScheduleItem> schedules = new ObservableCollection<ScheduleItem>();
    private static string[] dayOfWeek = { "일", "월", "화", "수", "목", "금", "토"};
    public TeamCalendar()
    {
        InitializeComponent();
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
        EventsResource.ListRequest request = calendarService.Events.List("primary");
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
                
                ScheduleItem schedule = new ScheduleItem();
                schedule.Title = eventItem.Summary;
                schedule.startDate = isAllDayEventStart ? 
                    String.Format("{0}월 {1}일 ({2})", startDateTime.Month, startDateTime.Day, dayOfWeek[(int)startDateTime.DayOfWeek])
                    : String.Format("{0}월 {1}일 ({2}) {3:D2}:{4:D2}", startDateTime.Month, startDateTime.Day, dayOfWeek[(int)startDateTime.DayOfWeek], startDateTime.Hour, startDateTime.Minute);  
                schedule.endDate = isAllDayEventEnd ? 
                    String.Format("{0}월 {1}일 ({2})", endDateTime.Month, endDateTime.Day, dayOfWeek[(int)endDateTime.DayOfWeek])
                    : String.Format("{0}월 {1}일 ({2}) {3:D2}:{4:D2}", endDateTime.Month, endDateTime.Day, dayOfWeek[(int)endDateTime.DayOfWeek], endDateTime.Hour, endDateTime.Minute);
                schedule.location = eventItem.Location;
                schedule.description = eventItem.Description;
                schedules.Add(schedule);
                
            }
            CardListView.ItemsSource = schedules;
        }
        else
        {
            NoScheduleMsg.Visibility = Visibility.Visible;
        }


    }
    
    // private async void Load() // 캘린더에서 선택된 날짜 일정
    // {
    //     var calendarService = GoogleAuthentication.CalendarService;
    //     
    //     if (calendarService == null)
    //     {
    //         throw new InvalidOperationException("먼저 Google에 로그인하세요.");
    //     }
    //
    //     // 이벤트 요청 설정
    //     EventsResource.ListRequest request = calendarService.Events.List("primary");
    //     request.TimeMin = DateTime.Now;
    //     request.ShowDeleted = false;
    //     request.SingleEvents = true;
    //     request.MaxResults = 10;
    //     request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
    //     
    //     // 비동기로 이벤트 실행
    //     Events events = await request.ExecuteAsync();
    //
    //     if (events.Items != null && events.Items.Count > 0)
    //     {
    //         foreach (var eventItem in events.Items)
    //         {
    //             bool isAllDayEventStart = false;
    //             string start = eventItem.Start.DateTimeRaw;
    //             if (start == null)
    //             {
    //                 isAllDayEventStart = true;
    //                 start = eventItem.Start.Date;
    //             }
    //             
    //             bool isAllDayEventEnd = false;
    //             string end = eventItem.End.DateTimeRaw;
    //             if (end == null)
    //             {
    //                 isAllDayEventEnd = true;
    //                 end = eventItem.End.Date;
    //             }
    //             
    //             DateTime startDateTime = DateTime.Parse(start);
    //             DateTime endDateTime = DateTime.Parse(end);
    //             
    //             ScheduleItem schedule = new ScheduleItem();
    //             schedule.Title = eventItem.Summary;
    //             schedule.startDate = isAllDayEventStart ? 
    //                 String.Format("{0}월 {1}일 ({2})", startDateTime.Month, startDateTime.Day, dayOfWeek[(int)startDateTime.DayOfWeek])
    //                 : String.Format("{0}월 {1}일 ({2}) {3:D2}:{4:D2}", startDateTime.Month, startDateTime.Day, dayOfWeek[(int)startDateTime.DayOfWeek], startDateTime.Hour, startDateTime.Minute);  
    //             schedule.endDate = isAllDayEventEnd ? 
    //                 String.Format("{0}월 {1}일 ({2})", endDateTime.Month, endDateTime.Day, dayOfWeek[(int)endDateTime.DayOfWeek])
    //                 : String.Format("{0}월 {1}일 ({2}) {3:D2}:{4:D2}", endDateTime.Month, endDateTime.Day, dayOfWeek[(int)endDateTime.DayOfWeek], endDateTime.Hour, endDateTime.Minute);
    //             schedule.location = eventItem.Location;
    //             schedule.description = eventItem.Description;
    //             schedules.Add(schedule);
    //             
    //         }
    //         CardListViewSelected.ItemsSource = schedules;
    //     }
    //     else
    //     {
    //         CalendarNoScheduleMsg.Visibility = Visibility.Visible;
    //     }
    //
    // }
}