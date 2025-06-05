using System.Windows.Documents;
using CollaborationTools.team;
using CollaborationTools.user;
using MySqlConnector;

namespace CollaborationTools.database;

public class TeamRepository
{
    private ConnectionPool _connectionPool = ConnectionPool.GetInstance();

    public bool AddTeam(Team team)
    {
        MySqlConnection connection = null;
        bool result = true;
        
        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand("INSERT INTO team VALUES (@teamName, @teamMemberCount, @dateOfCreated, @teamCalendarName, @teamCalendarId)", connection))
            {
                command.Parameters.AddWithValue("@teamName", team.teamCalendarName);
                command.Parameters.AddWithValue("@teamMemberCount", team.teamMemberCount);
                command.Parameters.AddWithValue("@dateOfCreated", team.dateOfCreated);
                command.Parameters.AddWithValue("@teamCalendarName", team.teamCalendarName);
                command.Parameters.AddWithValue("@teamCalendarId", team.teamCalendarId);

                int executeResult = command.ExecuteNonQuery();
                
                if (executeResult == 0)
                {
                    result = false;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error adding team: {e.Message}");
        }
        finally
        {
            if (connection != null)
            {
                _connectionPool.ReleaseConnection(connection);
            }
        }

        return result;
    }

    public bool RemoveTeam(int teamId)
    {
        MySqlConnection connection = null;
        bool result = true;
        
        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand("DELETE FROM team WHERE id = @teamId", connection))
            {
                command.Parameters.AddWithValue("@teamId", teamId);

                int executeResult = command.ExecuteNonQuery();
                
                if (executeResult == 0)
                {
                    result = false;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error deleting team: {e.Message}");
        }
        finally
        {
            if (connection != null)
            {
                _connectionPool.ReleaseConnection(connection);
            }
        }

        return result;
    }
    
    public Team? FindTeamById (int id)
    {
        Team? team = null;
        MySqlConnection connection = null;

        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand("SELECT * FROM team WHERE id = @id", connection))
            {
                command.Parameters.AddWithValue("@id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        team = new Team
                        {
                            teamId = reader.GetInt32("id"),
                            teamName = reader.GetString("team_name"),
                            teamMemberCount = reader.GetInt32("team_member_count"),
                            dateOfCreated = reader.GetDateTime("date_of_created"),
                            teamCalendarName = reader.GetString("team_calendar_name"),
                            teamCalendarId = reader.GetString("team_calendar_id")
                        };
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"find team by id error: {e.Message}");
        }
        finally
        {
            if (connection != null)
            {
                _connectionPool.ReleaseConnection(connection);
            }
        }

        return team;
    }

    public List<Team?> FindTeamsByUser(User user)
    {
        List<Team?> teams = new List<Team?>();
        MySqlConnection connection = null;
        
        TeamMemberRepository teamMemberRepository = new TeamMemberRepository();
        List<TeamMember?> teamMembers = teamMemberRepository.FindTeamMemberByUserId(user.userId);

        try
        {
            connection = _connectionPool.GetConnection();

            foreach (TeamMember? teamMember in teamMembers)
            {
                using (var command = new MySqlCommand("SELECT * FROM team WHERE id = @id", connection))
                {
                    if (teamMember != null)
                    {
                        command.Parameters.AddWithValue("@id", teamMember.teamId);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                teams.Add(new Team
                                {
                                    teamId = teamMember.teamId,
                                    teamName = reader.GetString("team_name"),
                                    teamMemberCount = reader.GetInt32("team_member_count"),
                                    dateOfCreated = reader.GetDateTime("date_of_created"),
                                    teamCalendarName = reader.GetString("team_calendar_name"),
                                    teamCalendarId = reader.GetString("team_calendar_id")
                                });
                            }
                        }
                    }
                    else
                    {
                        teams.Add(null);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"find team by id error: {e.Message}");
        }
        finally
        {
            if (connection != null)
            {
                _connectionPool.ReleaseConnection(connection);
            }
        }

        return teams;
    }
}