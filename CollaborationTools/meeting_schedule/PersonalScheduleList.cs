namespace CollaborationTools.meeting_schedule;

public class PersonalScheduleList
{
    private int _userId;
    private int _teamId;
    private int _meetingScheduleId;
    private List<Schedule> _schedules;
    
    public PersonalScheduleList()
    {
        _schedules = new List<Schedule>();
    }
    
    
    public void AddSchedule(DateTime date, DateTime startTime, DateTime endTime)
    {
        _schedules.Add(new Schedule(date, startTime, endTime));
    }

    public List<Schedule> Schedules
    {
        get => _schedules;
    }
    
    public int UserId
    {
        get => _userId;
        set => _userId = value;
    }
    
    public int TeamId
    {
        get => _teamId;
        set => _teamId = value;
    }

    public int MeetingScheduleId
    {
        get => _meetingScheduleId;
        set => _meetingScheduleId = value;
    }
    
}
