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
}