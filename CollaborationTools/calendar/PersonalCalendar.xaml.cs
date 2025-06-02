using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CollaborationTools.authentication;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;

namespace CollaborationTools.calendar;

public partial class PersonalCalendar : UserControl
{
    private ObservableCollection<ScheduleItem> schedules = new ObservableCollection<ScheduleItem>();
    public PersonalCalendar()
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
                ScheduleItem schedule = new ScheduleItem();
                schedule.Title = eventItem.Summary;
                schedule.startDate = eventItem.Start.Date;
                schedule.endDate = eventItem.End.Date;
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
    
    // private void LoadTestScheduleItems()
    // {
    //     schedules.Add(new ScheduleItem { scheduleTitle = "미팅", scheduleDate = "2024-03-20" });
    //     schedules.Add(new ScheduleItem { scheduleTitle = "프로젝트 마감", scheduleDate ="2024-03-21" });
    //     schedules.Add(new ScheduleItem { scheduleTitle = "휴가", scheduleDate = "2024-03-22" });
    //
    //     CardListView.ItemsSource = schedules;
    //     CardListViewSelected.ItemsSource = schedules;
    // }
}