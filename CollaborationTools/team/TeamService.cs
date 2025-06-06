using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using CollaborationTools.database;
using CollaborationTools.user;

namespace CollaborationTools.team;

public class TeamService
{
    private TeamRepository _teamRepository =  new TeamRepository();
    private TeamMemberRepository _teamMemberRepository = new TeamMemberRepository();

    public bool CreateTeam(Team team)
    {
        bool result = _teamRepository.AddTeam(
            team.teamName,
            team.uuid,
            team.teamMemberCount,
            team.dateOfCreated,
            team.teamCalendarName,
            team.teamCalendarId,
            team.teamDescription);
            
        return result;
    }

    public bool RemoveTeam(int teamId)
    {
        bool result = _teamRepository.RemoveTeam(teamId);
        
        return result;
    }

    public List<Team?> FindUsersTeams(User user)
    {
        return _teamRepository.FindTeamsByUser(user);
    }

    public Team? FindTeamByUuid(string uuid)
    {
        return _teamRepository.FindTeamByUuid(uuid);
    }
    
    public bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;
            
        string pattern = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]";
        return Regex.IsMatch(email, pattern);
    }
    
    public bool AddTeamMemberByEmail(Team team, string userEmail, byte authority)
    {
        bool result = _teamMemberRepository.AddTeamMember(team.uuid, userEmail, authority);
        
        return result;
    }

    public bool AddTeamMembersByEmail(Team team, ObservableCollection<string> userEmailList, byte authority)
    {
        bool result = true;
        
        foreach (var userEmail in userEmailList)
        {
            result = _teamMemberRepository.AddTeamMember(team.uuid, userEmail, authority);
        }

        return result;
    }
}