using CollaborationTools.authentication;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;

namespace CollaborationTools.calendar;

public class ScheduleService
{
    public static async Task<List<ScheduleItem>> GetScheduleItems(string calendarId)
    {
        var calendarService = GoogleAuthentication.CalendarService;
        
        if (calendarService == null)
        {
            throw new InvalidOperationException("먼저 Google에 로그인하세요.");
        }

        // 이벤트 요청 설정
        EventsResource.ListRequest request = calendarService.Events.List(calendarId);
        request.TimeMinDateTimeOffset = DateTime.Now;
        request.TimeMaxDateTimeOffset = DateTime.Now.AddYears(1);
        request.ShowDeleted = false;
        request.SingleEvents = true;
        request.MaxResults = 20;
        request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
        
        // 비동기로 이벤트 실행
        Events events = await request.ExecuteAsync();

        return GetScheduleItems(events, calendarId);
    }

    public static async Task<List<ScheduleItem>> GetOneDayScheduleItems(string calendarId, DateTime selectedDate)
    {
        var calendarService = GoogleAuthentication.CalendarService;
        
        if (calendarService == null)
        {
            throw new InvalidOperationException("먼저 Google에 로그인하세요.");
        }
    
        // 이벤트 요청 설정
        EventsResource.ListRequest request = calendarService.Events.List(calendarId);
        request.TimeMinDateTimeOffset = selectedDate;
        request.TimeMaxDateTimeOffset = selectedDate.AddDays(1);;
        request.ShowDeleted = false;
        request.SingleEvents = true;
        request.MaxResults = 20;
        request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
        
        // 비동기로 이벤트 실행
        Events events = await request.ExecuteAsync();
        
        return GetScheduleItems(events, calendarId);
    }

    private static List<ScheduleItem> GetScheduleItems(Events events, string calendarId)
    {
        var schedules = new List<ScheduleItem>();
        // 일정이 없는 경우 함수 종료
        if (events?.Items == null || events.Items.Count == 0)
            return schedules;

        foreach (var eventItem in events.Items)
        {
            var (startDateTime, isAllDayEvent) = ParseEventDateTime(eventItem.Start, isEnd: false);
            var (endDateTime, _) = ParseEventDateTime(eventItem.End, isEnd: true);

            bool isOneDayEvent = startDateTime.Date == endDateTime.Date;

            var schedule = new ScheduleItem
            {
                Event = eventItem,
                Title = eventItem.Summary,
                StartDateTime = startDateTime,
                EndDateTime = endDateTime,
                IsAllDayEvent = isAllDayEvent,
                IsOneDayEvent = isOneDayEvent,
                Location = eventItem.Location,
                Description = eventItem.Description,
                CalendarId = calendarId,
            };
            schedules.Add(schedule);
        }
        return schedules;
    }
    
    private static (DateTime dateTime, bool isAllDayEvent) ParseEventDateTime(EventDateTime eventDateTime, bool isEnd)
    {
        // 종일 이벤트인 경우
        if (string.IsNullOrEmpty(eventDateTime.DateTimeRaw))
        {
            var date = DateTime.Parse(eventDateTime.Date).Date;
            
            if (isEnd) 
            { date = date.AddDays(-1);}
            
            return (date, true);
        }

        // 시간 지정 이벤트인 경우
        return (DateTime.Parse(eventDateTime.DateTimeRaw), false);
    }


    public static async Task UpdateScheduleItem(Event eventItem, string calendarId)
    {
        var calendarService = GoogleAuthentication.CalendarService;
        
        if (calendarService == null)
        {
            throw new InvalidOperationException("먼저 Google에 로그인하세요.");
        }
        
        EventsResource.UpdateRequest request = new EventsResource.UpdateRequest(calendarService, eventItem, calendarId, eventItem.Id);

        try
        {
            _ = await request.ExecuteAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace); 
        }
        
    }
}