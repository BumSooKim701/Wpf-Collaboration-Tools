using System.ComponentModel;
using Google.Apis.Calendar.v3.Data;
using MySqlConnector;

namespace CollaborationTools.calendar;

public class ScheduleItem
{
    public string Title { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public bool IsAllDayEventStart { get; set; }
    public bool IsAllDayEventEnd { get; set; }
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
        _event.Start = new EventDateTime()
        {
            DateTimeDateTimeOffset = StartDateTime,
            TimeZone = "Asia/Seoul",
        };
        _event.End = new EventDateTime()
        {
            DateTimeDateTimeOffset = EndDateTime,
            TimeZone = "Asia/Seoul",
        };
    }
}