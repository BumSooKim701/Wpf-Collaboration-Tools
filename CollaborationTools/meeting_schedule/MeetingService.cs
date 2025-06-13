using CollaborationTools.database;
using CollaborationTools.team;

namespace CollaborationTools.meeting_schedule;

public class MeetingService
{
    private MeetingRepository _meetingRepository;
    
    
    public MeetingService()
    {
        _meetingRepository = new();
    }


    public Meeting GetMeeting(Team team)
    {
        return _meetingRepository.GetMeeting(team.teamId);
    }
}