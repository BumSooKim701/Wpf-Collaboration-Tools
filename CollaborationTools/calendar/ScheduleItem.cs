using System.ComponentModel;
using Google.Apis.Calendar.v3.Data;
using MySqlConnector;

namespace CollaborationTools.calendar;

public class ScheduleItem
{
    public string Title { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public bool IsAllDayEvent { get; set; }
    public bool IsOneDayEvent { get; set; }
    public string Location { get; set; }
    public string Description { get; set; }
    public string CalendarId { get; set; }
    
    private Event _event;
    public Event Event
    {
        get
        {
            UpdateEvent();
            return _event;
        }
        set 
        {  
            _event = value;
        }
    }

    private void UpdateEvent()
    {
        _event.Summary = Title;
        _event.Location = Location;
        _event.Description = Description;
        _event.Start = CreateEventDateTime(StartDateTime, IsAllDayEvent, false);
        _event.End   = CreateEventDateTime(EndDateTime, IsAllDayEvent, true);
    }
    
    private EventDateTime CreateEventDateTime(DateTime dateTime, bool isAllDayEvent, bool isEnd)
    {
        var adjustedDateTime = isEnd ? dateTime.AddDays(1) : dateTime;

        return isAllDayEvent
            ? new EventDateTime
            {
                Date = adjustedDateTime.Date.ToString("yyyy-MM-dd"),
                TimeZone = "Asia/Seoul"
            }
            : new EventDateTime
            {
                DateTimeDateTimeOffset = isEnd ? dateTime.AddDays(1) : dateTime,
                TimeZone = "Asia/Seoul"
            };
    }


    public ScheduleItem Clone()
    {
        var clone = new ScheduleItem
        {
            Title = this.Title,
            StartDateTime = this.StartDateTime,
            EndDateTime = this.EndDateTime,
            IsAllDayEvent = this.IsAllDayEvent,
            IsOneDayEvent = this.IsOneDayEvent,
            Location = this.Location,
            Description = this.Description,
            CalendarId = this.CalendarId,
            Event = this.Event
        };

        return clone;
    }
}