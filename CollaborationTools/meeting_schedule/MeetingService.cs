using CollaborationTools.database;
using CollaborationTools.team;

namespace CollaborationTools.meeting_schedule;

public class MeetingService
{
    private readonly MeetingRepository _meetingRepository;


    public MeetingService()
    {
        _meetingRepository = new MeetingRepository();
    }


    public Meeting GetMeeting(Team team)
    {
        return _meetingRepository.GetMeeting(team.teamId);
    }

    public bool CreateMeeting(Meeting meetingPlan)
    {
        return _meetingRepository.CreateMeeting(meetingPlan);
    }

    public bool RegisterPersonalSchedule(PersonalScheduleList personalSchedules)
    {
        return _meetingRepository.RegisterPersonalSchedule(personalSchedules);
    }

    public PersonalScheduleList GetPersonalSchedule(int userId, int teamId)
    {
        return _meetingRepository.getPersonalSchedule(userId, teamId);
    }
}