using System.Collections.ObjectModel;
using CollaborationTools.authentication;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;

namespace CollaborationTools.calendar;

public static class ScheduleService
{
    // 다가오는 일정 불러오기
    public static async Task<ObservableCollection<ScheduleItem>> GetScheduleItems(string calendarId)
    {
        var calendarService = GoogleAuthentication.CalendarService;

        // 이벤트 요청 설정
        var request = calendarService.Events.List(calendarId);
        request.TimeMinDateTimeOffset = DateTime.Now;
        request.TimeMaxDateTimeOffset = DateTime.Now.AddYears(1);
        request.ShowDeleted = false;
        request.SingleEvents = true;
        request.MaxResults = 20;
        request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

        // 비동기로 이벤트 실행
        var events = await request.ExecuteAsync();

        return GetScheduleItems(events, calendarId);
    }

    // 캘린더로 선택한 날짜의 일정 불러오기
    public static async Task<ObservableCollection<ScheduleItem>> GetOneDayScheduleItems(string calendarId,
        DateTime selectedDate)
    {
        var calendarService = GoogleAuthentication.CalendarService;

        if (calendarService == null) throw new InvalidOperationException("먼저 Google에 로그인하세요.");

        // 이벤트 요청 설정
        var request = calendarService.Events.List(calendarId);
        request.TimeMinDateTimeOffset = selectedDate;
        request.TimeMaxDateTimeOffset = selectedDate.AddDays(1);
        ;
        request.ShowDeleted = false;
        request.SingleEvents = true;
        request.MaxResults = 20;
        request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

        // 비동기로 이벤트 실행
        var events = await request.ExecuteAsync();

        return GetScheduleItems(events, calendarId);
    }

    // Events 객체로 부터 ScheduleItem 리스트 생성하여 반환
    private static ObservableCollection<ScheduleItem> GetScheduleItems(Events events, string calendarId)
    {
        var schedules = new ObservableCollection<ScheduleItem>();
        // 일정이 없는 경우 함수 종료
        if (events?.Items == null || events.Items.Count == 0)
            return schedules;

        foreach (var eventItem in events.Items)
        {
            var (startDateTime, isAllDayEvent) = ParseEventDateTime(eventItem.Start, false);
            var (endDateTime, _) = ParseEventDateTime(eventItem.End, true);

            var isOneDayEvent = startDateTime.Date == endDateTime.Date;

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
                CalendarId = calendarId
            };
            schedules.Add(schedule);
        }

        return schedules;
    }

    // 종일 이벤트 고려하여 날짜 시간 포매팅하여 반환
    private static (DateTime dateTime, bool isAllDayEvent) ParseEventDateTime(EventDateTime eventDateTime, bool isEnd)
    {
        // 종일 이벤트인 경우
        if (string.IsNullOrEmpty(eventDateTime.DateTimeRaw))
        {
            var date = DateTime.Parse(eventDateTime.Date).Date;

            if (isEnd) date = date.AddDays(-1);

            return (date, true);
        }

        // 시간 지정 이벤트인 경우
        return (DateTime.Parse(eventDateTime.DateTimeRaw), false);
    }


    // 일정 수정 요청
    public static async Task UpdateSchedule(Event eventItem, string calendarId)
    {
        var calendarService = GoogleAuthentication.CalendarService;

        var request = new EventsResource.UpdateRequest(calendarService, eventItem, calendarId, eventItem.Id);

        try
        {
            _ = await request.ExecuteAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    // 일정 등록 요청
    public static async Task RegisterSchedule(ScheduleItem scheduleItem)
    {
        var calendarService = GoogleAuthentication.CalendarService;

        var request = calendarService.Events.Insert(scheduleItem.Event, scheduleItem.CalendarId);
        _ = await request.ExecuteAsync();
    }

    // 일정 삭제 요청
    public static async Task DeleteSchedule(ScheduleItem scheduleItem)
    {
        var calendarService = GoogleAuthentication.CalendarService;

        var request = calendarService.Events.Delete(scheduleItem.CalendarId, scheduleItem.Event.Id);
        _ = await request.ExecuteAsync();
    }
}