using System.Windows;
using CollaborationTools.authentication;
using Google.Apis.Calendar.v3.Data;

namespace CollaborationTools.calendar;

public class CalendarService
{
    public async Task<Calendar> CreateCalendarAsync(string calendarName, string description,
        string timeZone = "Asia/Seoul")
    {
        var calendarService = GoogleAuthentication.CalendarService;

        try
        {
            // 새 캘린더 객체 생성
            var calendar = new Calendar
            {
                Summary = calendarName,
                Description = description,
                TimeZone = timeZone
            };

            // 캘린더 생성 API 호출
            var createdCalendar = await calendarService.Calendars.Insert(calendar).ExecuteAsync();
            return createdCalendar;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"캘린더 생성 오류: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            return null;
        }
    }

    public async Task<List<CalendarListEntry>> GetCalendarListAsync()
    {
        var calendarService = GoogleAuthentication.CalendarService;

        try
        {
            // 캘린더 목록 요청
            var calendarList = await calendarService.CalendarList.List().ExecuteAsync();
            return calendarList.Items != null
                ? new List<CalendarListEntry>(calendarList.Items)
                : new List<CalendarListEntry>();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"캘린더 목록 조회 오류: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            return new List<CalendarListEntry>();
        }
    }

    public async Task<bool> DeleteCalendarAsync(string calendarId)
    {
        var calendarService = GoogleAuthentication.CalendarService;

        try
        {
            // 캘린더 삭제 API 호출
            await calendarService.Calendars.Delete(calendarId).ExecuteAsync();
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"캘린더 삭제 오류: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    // 캘린더에 멤버 추가
    public async Task<bool> AddCalendarMemberAsync(string calendarId, string email, string role = "writer")
    {
        try
        {
            var calendarService = GoogleAuthentication.CalendarService;

            var rule = new AclRule
            {
                Scope = new AclRule.ScopeData
                {
                    Type = "user",
                    Value = email
                },
                Role = role // "reader", "writer", "owner"
            };

            await calendarService.Acl.Insert(rule, calendarId).ExecuteAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Add Member in Calendar Error: {ex.Message}");
            return false;
        }
    }
}