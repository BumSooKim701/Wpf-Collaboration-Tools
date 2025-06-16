using CollaborationTools.team;
using CollaborationTools.user;
using MySqlConnector;

namespace CollaborationTools.database;

public class TeamRepository
{
    private readonly ConnectionPool _connectionPool = ConnectionPool.GetInstance();

    public bool AddTeam(string teamName, string uuid, int teamMemberCount, DateTime dateOfCreated, string teamCalName,
        string teamDescription, byte visibility)
    {
        MySqlConnection connection = null;
        var result = true;

        if (teamDescription == null) teamDescription = " ";
        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand(
                       "INSERT INTO team (uuid, team_name, team_member_count, date_of_created, team_calendar_name, team_description, visibility) VALUES (@uuid, @teamName, @teamMemberCount, @dateOfCreated, @teamCalendarName, @teamDescription, @visibility)",
                       connection))
            {
                command.Parameters.AddWithValue("@uuid", uuid);
                command.Parameters.AddWithValue("@teamName", teamName);
                command.Parameters.AddWithValue("@teamMemberCount", teamMemberCount);
                command.Parameters.AddWithValue("@dateOfCreated", dateOfCreated);
                command.Parameters.AddWithValue("@teamCalendarName", teamCalName);
                command.Parameters.AddWithValue("@teamDescription", teamDescription);
                command.Parameters.AddWithValue("@visibility", visibility);

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
            using (var command = new MySqlCommand("UPDATE team SET team_calendar_id = @teamCalId WHERE id = @teamId",
                       connection))
            {
                command.Parameters.AddWithValue("@teamCalId", teamCalId);
                command.Parameters.AddWithValue("@teamId", teamId);

                var executeResult = command.ExecuteNonQuery();

                if (executeResult == 0) result = false;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error updating team calendar id: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return result;
    }

    public bool UpdateTeamDriveId(int teamId, string teamDriveId)
    {
        MySqlConnection connection = null;
        var result = true;

        try
        {
            connection = _connectionPool.GetConnection();
            using (var command = new MySqlCommand("UPDATE team SET shared_drive_id = @teamDriveId WHERE id = @teamId",
                       connection))
            {
                command.Parameters.AddWithValue("@teamDriveId", teamDriveId);
                command.Parameters.AddWithValue("@teamId", teamId);

                var executeResult = command.ExecuteNonQuery();

                if (executeResult == 0) result = false;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error updating team drive id: {e.Message}");
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
                            teamCalendarId = reader.GetString("team_calendar_id"),
                            teamDescription = reader.GetString("team_description"),
                            teamFolderId = reader.GetString("shared_drive_id")
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
    
    public Team? FindTeamByCalId(string id)
    {
        Team? team = null;
        MySqlConnection connection = null;

        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand("SELECT * FROM team WHERE team_calendar_id = @id", connection))
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
                            teamCalendarId = reader.GetString("team_calendar_id"),
                            teamDescription = reader.GetString("team_description"),
                            teamFolderId = reader.GetString("shared_drive_id")
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
                                    teamDescription = reader.GetString("team_description"),
                                    teamFolderId = reader.GetString("shared_drive_id"),
                                    visibility = reader.GetByte("visibility")
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

    public Team? FindPrimaryTeam(int teamId)
    {
        Team team = new();
        MySqlConnection connection = null;

        try
        {
            connection = _connectionPool.GetConnection();

            using (var command =
                   new MySqlCommand("SELECT * FROM team WHERE id = @teamId and visibility = 0", connection))
            {
                command.Parameters.AddWithValue("@teamId", teamId);

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
                            teamCalendarId = reader.GetString("team_calendar_id"),
                            teamDescription = reader.GetString("team_description"),
                            teamFolderId = reader.GetString("shared_drive_id"),
                            visibility = reader.GetByte("visibility")
                        };
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

        return team;
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
                            teamCalendarId = reader.GetString("team_calendar_id"),
                            teamDescription = reader.GetString("team_description"),
                            teamFolderId = reader.GetString("shared_drive_id"),
                            visibility = reader.GetByte("visibility")
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

    public int FindTeamIdFromFolderId(string folderId)
    {
        var result = -1;
        MySqlConnection connection = null;

        try
        {
            connection = _connectionPool.GetConnection();

            using var command = new MySqlCommand("SELECT id FROM team WHERE shared_drive_id = @folder_id", connection);
            command.Parameters.AddWithValue("@folder_id", folderId);

            using var reader = command.ExecuteReader();
            if (reader.Read()) result = reader.GetInt32("id");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error getting teamId: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return result;
    }
    
    
}