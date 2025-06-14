namespace CollaborationTools.meeting_schedule;

public class Schedule
{
    private DateTime _date;
    private DateTime _startDateTime;
    private DateTime _endDateTime;

    public Schedule(DateTime date, DateTime startTime, DateTime endTime)
    {
        _date = date;
        _startDateTime = startTime;
        _endDateTime = endTime;
    }

    
    public DateTime Date
    {
        get => _date;
    }
    
    public DateTime StartDateTime
    {
        get => _startDateTime;
    }
    public DateTime EndDateTime
    {
        get => _endDateTime;
    }
}