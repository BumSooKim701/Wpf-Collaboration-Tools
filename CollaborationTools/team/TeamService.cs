using CollaborationTools.database;
using CollaborationTools.user;

namespace CollaborationTools.team;

public class TeamService
{
    private TeamRepository _teamRepository =  new TeamRepository();
    private TeamMemberRepository _teamMemberRepository = new TeamMemberRepository();

    public bool CreateTeam(string teamName, string teamCalName, string teamCalId)
    {
        bool result = _teamRepository.AddTeam(new Team
        {
            teamName = teamName,
            teamCalendarName = teamCalName,
            teamCalendarId = teamCalId
        });
        
        return result;
    }

    public bool RemoveTeam(int teamId)
    {
        bool result = _teamRepository.RemoveTeam(teamId);
        
        return result;
    }

    public List<Team?> SearchUsersTeams(User user)
    {
        return _teamRepository.FindTeamsByUser(user);
    }
}