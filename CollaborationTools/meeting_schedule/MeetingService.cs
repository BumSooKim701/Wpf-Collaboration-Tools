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
}