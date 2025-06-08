using CollaborationTools.team;
using CollaborationTools.user;
using MySqlConnector;

namespace CollaborationTools.database;

public class TeamRepository
{
    private readonly ConnectionPool _connectionPool = ConnectionPool.GetInstance();

    public bool AddTeam(string teamName, string uuid, int teamMemberCount, DateTime dateOfCreated, string teamCalName, string teamDescription)
    {
        MySqlConnection connection = null;
        var result = true;

        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand(
                       "INSERT INTO team (uuid, team_name, team_member_count, date_of_created, team_calendar_name, team_description) VALUES (@uuid, @teamName, @teamMemberCount, @dateOfCreated, @teamCalendarName, @teamDescription)",
                       connection))
            {
                command.Parameters.AddWithValue("@uuid", uuid);
                command.Parameters.AddWithValue("@teamName", teamName);
                command.Parameters.AddWithValue("@teamMemberCount", teamMemberCount);
                command.Parameters.AddWithValue("@dateOfCreated", dateOfCreated);
                command.Parameters.AddWithValue("@teamCalendarName", teamCalName);
                command.Parameters.AddWithValue("@teamDescription", teamDescription);

                var executeResult = command.ExecuteNonQuery();

                if (executeResult == 0) result = false;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error adding team: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return result;
    }
    
    public bool UpdateTeamCalendarId(int teamId, string teamCalId)
    {
        MySqlConnection connection = null;
        var result = true;
        
        try
        {
            connection = _connectionPool.GetConnection();
            using (var command = new MySqlCommand("UPDATE team SET team_calendar_id = @teamCalId WHERE id = @teamId", connection))
            {
                command.Parameters.AddWithValue("@teamCalId", teamCalId);
                command.Parameters.AddWithValue("@teamId", teamId);

                var executeResult = command.ExecuteNonQuery();

                if (executeResult == 0) result = false;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error adding team: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return result;
    }

    public bool RemoveTeam(int teamId)
    {
        MySqlConnection connection = null;
        var result = true;

        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand("DELETE FROM team WHERE id = @teamId", connection))
            {
                command.Parameters.AddWithValue("@teamId", teamId);

                var executeResult = command.ExecuteNonQuery();

                if (executeResult == 0) result = false;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error deleting team: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return result;
    }

    public Team? FindTeamById(int id)
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
                        team = new Team
                        {
                            teamId = reader.GetInt32("id"),
                            uuid = reader.GetString("uuid"),
                            teamName = reader.GetString("team_name"),
                            teamMemberCount = reader.GetInt32("team_member_count"),
                            dateOfCreated = reader.GetDateTime("date_of_created"),
                            teamCalendarName = reader.GetString("team_calendar_name"),
                            teamCalendarId = reader.GetString("team_calendar_id")
                        };
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"find team by id error: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return team;
    }

    public List<Team?> FindTeamsByUser(User user)
    {
        var teams = new List<Team?>();
        MySqlConnection connection = null;

        var teamMemberRepository = new TeamMemberRepository();
        var teamMembers = teamMemberRepository.FindTeamMemberByUserId(user.userId);

        try
        {
            connection = _connectionPool.GetConnection();

            foreach (var teamMember in teamMembers)
                using (var command = new MySqlCommand("SELECT * FROM team WHERE id = @id", connection))
                {
                    if (teamMember != null)
                    {
                        command.Parameters.AddWithValue("@id", teamMember.teamId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                                teams.Add(new Team
                                {
                                    teamId = teamMember.teamId,
                                    uuid = reader.GetString("uuid"),
                                    teamName = reader.GetString("team_name"),
                                    teamMemberCount = reader.GetInt32("team_member_count"),
                                    dateOfCreated = reader.GetDateTime("date_of_created"),
                                    teamCalendarName = reader.GetString("team_calendar_name"),
                                    teamCalendarId = reader.GetString("team_calendar_id"),
                                    teamDescription = reader.GetString("team_description")
                                });
                        }
                    }
                    else
                    {
                        teams.Add(null);
                    }
                }
        }
        catch (Exception e)
        {
            Console.WriteLine($"find team by user error: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return teams;
    }

    public List<Team?>? FindTeamByName(string teamName)
    {
        var teamList = new List<Team?>();
        MySqlConnection connection = null;

        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand("SELECT * FROM team WHERE team_name = @name", connection))
            {
                command.Parameters.AddWithValue("@name", teamName);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        teamList?.Add(new Team
                        {
                            teamId = reader.GetInt32("id"),
                            uuid = reader.GetString("uuid"),
                            teamName = reader.GetString("team_name"),
                            teamMemberCount = reader.GetInt32("team_member_count"),
                            dateOfCreated = reader.GetDateTime("date_of_created"),
                            teamCalendarName = reader.GetString("team_calendar_name"),
                            teamCalendarId = reader.GetString("team_calendar_id")
                        });
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"find team by name error: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return teamList;
    }

    public Team? FindTeamByUuid(string uuid)
    {
        Team? team = null;
        MySqlConnection connection = null;

        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand("SELECT * FROM team WHERE uuid = @uuid", connection))
            {
                command.Parameters.AddWithValue("@uuid", uuid);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                        team = new Team
                        {
                            teamId = reader.GetInt32("id"),
                            uuid = reader.GetString("uuid"),
                            teamName = reader.GetString("team_name"),
                            teamMemberCount = reader.GetInt32("team_member_count"),
                            dateOfCreated = reader.GetDateTime("date_of_created"),
                            teamCalendarName = reader.GetString("team_calendar_name"),
                            teamCalendarId = reader.GetString("team_calendar_id")
                        };
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"find team by uuid error: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return team;
    }
}