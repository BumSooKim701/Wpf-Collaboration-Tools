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
        request.TimeMin = DateTime.Now;
        request.ShowDeleted = false;
        request.SingleEvents = true;
        request.MaxResults = 10;
        request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
        
        // 비동기로 이벤트 실행
        Events events = await request.ExecuteAsync();

        return GetScheduleItems(events);
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
        
        return GetScheduleItems(events);
    }

    private static List<ScheduleItem> GetScheduleItems(Events events)
    {
        List<ScheduleItem> schedules = [];
        
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
                schedules.Add(schedule);
                
            }
            return schedules;
        }
        
        return null;
    }
}