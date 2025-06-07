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
                
                var startDateTime = DateTime.Parse(start);
                var endDateTime = DateTime.Parse(end);
                
                bool isOneDayEvent = (startDateTime.Date == endDateTime.Date);
                var schedule = new ScheduleItem
                {
                    Event = eventItem,
                    Title = eventItem.Summary,
                    StartDateTime = startDateTime,
                    EndDateTime = endDateTime,
                    IsAllDayEventStart = isAllDayEventStart,
                    IsAllDayEventEnd = isAllDayEventEnd,
                    IsOneDayEvent = isOneDayEvent,
                    Location = eventItem.Location,
                    Description = eventItem.Description,
                    CalendarId = calendarId,
                };
                schedules.Add(schedule);
            }
            return schedules;
        }
        
        return null;
    }

    public static async Task UpdateScheduleItem(Event eventItem, string calendarId)
    {
        var calendarService = GoogleAuthentication.CalendarService;
        
        if (calendarService == null)
        {
            throw new InvalidOperationException("먼저 Google에 로그인하세요.");
        }
        
        EventsResource.UpdateRequest request = new EventsResource.UpdateRequest(calendarService, eventItem, calendarId, eventItem.Id);
        
        _ = await request.ExecuteAsync();
    }
}