using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using CollaborationTools.database;
using CollaborationTools.user;

namespace CollaborationTools.team;

public class TeamService
{
    private readonly TeamMemberRepository _teamMemberRepository = new();
    private readonly TeamRepository _teamRepository = new();

    public bool CreateTeam(Team team)
    {
        var result = _teamRepository.AddTeam(
            team.teamName,
            team.uuid,
            team.teamMemberCount,
            team.dateOfCreated,
            team.teamCalendarName,
            team.teamDescription,
            team.visibility);

        return result;
    }

    public bool RemoveTeam(Team team)
    {
        var result = _teamRepository.RemoveTeam(team.teamId);

        return result;
    }

    public bool RemoveAllTeamMember(Team team)
    {
        var result = _teamMemberRepository.RemoveAllTeamMember(team.teamId);

        return result;
    }

    public List<Team?> FindUsersTeams(User user)
    {
        return _teamRepository.FindTeamsByUser(user);
    }
    
    public Team? FindUserPrimaryTeam(User user)
    {
        return _teamRepository.FindPrimaryTeam(user.TeamId);
    }

    public Team? FindTeamByUuid(string uuid)
    {
        return _teamRepository.FindTeamByUuid(uuid);
    }

    public List<TeamMember?>? FindTeamMembersByTeamId(int teamId)
    {
        return _teamMemberRepository.FindTeamMembersByTeamId(teamId);
    }
    
    public TeamMember? FindTeamMember(int teamId, int userId)
    {
        return _teamMemberRepository.FindTeamMember(teamId, userId);
    }

    public bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var pattern = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]";
        return Regex.IsMatch(email, pattern);
    }

    public bool AddTeamMemberByEmail(Team team, string userEmail, byte authority)
    {
        var result = _teamMemberRepository.AddTeamMember(team.uuid, userEmail, authority);

        return result;
    }

    public bool AddTeamMembersByEmail(Team team, ObservableCollection<string> userEmailList, byte authority)
    {
        var result = true;

        foreach (var userEmail in userEmailList)
            result = _teamMemberRepository.AddTeamMember(team.uuid, userEmail, authority);

        return result;
    }

    public bool UpdateTeamCalendarId(Team team, string calendarId)
    {
        bool result = _teamRepository.UpdateTeamCalendarId(team.teamId, calendarId);

        return result;
    }

    public bool UpdateTeamDriveId(Team team, string driveId)
    {
        bool result = _teamRepository.UpdateTeamDriveId(team.teamId, driveId);

        return result;
    }
    
    public byte FindAuthority(Team team, User user)
    {
        
        return _teamMemberRepository.FindTeamMemberAuthority(team.teamId, user.userId);;
    }
}