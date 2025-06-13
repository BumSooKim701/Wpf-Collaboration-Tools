using CollaborationTools.team;
using CollaborationTools.user;
using MySqlConnector;

namespace CollaborationTools.database;

public class TeamMemberRepository
{
    private readonly ConnectionPool _connectionPool = ConnectionPool.GetInstance();
    private readonly TeamRepository _teamRepository = new();
    private readonly UserRepository _userRepository = new();

    public bool AddTeamMember(int teamId, int userId, byte authority)
    {
        MySqlConnection connection = null;
        var result = true;

        try
        {
            connection = _connectionPool.GetConnection();

            using (var command =
                   new MySqlCommand(
                       "INSERT INTO team_member (user_id, team_id, authority) VALUES (@userId, @teamId, @authority)",
                       connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@teamId", teamId);
                command.Parameters.AddWithValue("@authority", authority);

                var executeResult = command.ExecuteNonQuery();

                if (executeResult == 0) result = false;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error adding team member: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return result;
    }

    public bool AddTeamMember(string uuid, string userEmail, byte authority)
    {
        MySqlConnection connection = null;
        User? user = null;
        Team? team = null;
        var result = true;

        try
        {
            connection = _connectionPool.GetConnection();
            user = _userRepository.FindUserByEmail(userEmail);
            team = _teamRepository.FindTeamByUuid(uuid);
            Console.WriteLine(uuid + " " + userEmail + " " + authority);

            using (var command =
                   new MySqlCommand(
                       "INSERT INTO team_member (user_id, team_id, authority) VALUES (@userId, @teamId, @authority)",
                       connection))
            {
                if (user != null && team != null)
                {
                    command.Parameters.AddWithValue("@userId", user.userId);
                    command.Parameters.AddWithValue("@teamId", team.teamId);
                    command.Parameters.AddWithValue("@authority", authority);

                    var executeResult = command.ExecuteNonQuery();

                    if (executeResult == 0) result = false;
                }
                else
                {
                    result = false;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error adding team member: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return result;
    }

    public bool RemoveAllTeamMember(int teamId)
    {
        MySqlConnection connection = null;
        var result = true;

        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand("DELETE FROM team_member WHERE team_id = @teamId", connection))
            {
                command.Parameters.AddWithValue("@teamId", teamId);

                var executeResult = command.ExecuteNonQuery();

                if (executeResult == 0) result = false;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error deleting team member: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return result;
    }

    public List<TeamMember?> FindTeamMemberByUserId(int userId)
    {
        var teamMemberList = new List<TeamMember?>();
        MySqlConnection? connection = null;

        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand("SELECT * FROM team_member WHERE user_id = @id", connection))
            {
                command.Parameters.AddWithValue("@id", userId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        teamMemberList.Add(new TeamMember
                        {
                            memberId = reader.GetInt32("id"),
                            userId = reader.GetInt32("user_id"),
                            teamId = reader.GetInt32("team_id"),
                            authority = reader.GetByte("authority")
                        });
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"find team member by id error: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return teamMemberList;
    }

    public List<TeamMember?>? FindTeamMembersByTeamId(int teamId)
    {
        List<TeamMember?>? teamMemberList = new List<TeamMember?>();
        MySqlConnection connection = null;

        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand("SELECT * FROM team_member WHERE team_id = @id", connection))
            {
                command.Parameters.AddWithValue("@id", teamId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        teamMemberList?.Add(new TeamMember
                        {
                            memberId = reader.GetInt32("id"),
                            userId = reader.GetInt32("user_id"),
                            teamId = reader.GetInt32("team_id"),
                            authority = reader.GetByte("authority")
                        });
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"find team members by team id error: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return teamMemberList;
    }
    
    public TeamMember? FindTeamMember(int teamId, int userId)
    {
        TeamMember teamMember = new TeamMember();
        MySqlConnection connection = null;

        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand("SELECT * FROM team_member WHERE team_id = @teamId and user_id = @userId", connection))
            {
                command.Parameters.AddWithValue("@teamId", teamId);
                command.Parameters.AddWithValue("@userId", userId);;

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                        teamMember = new TeamMember
                        {
                            memberId = reader.GetInt32("id"),
                            userId = reader.GetInt32("user_id"),
                            teamId = reader.GetInt32("team_id"),
                            authority = reader.GetByte("authority")
                        };
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"find team members by team id error: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return teamMember;
    }
    
    public byte FindTeamMemberAuthority(int teamId, int userId)
    {
        byte authority = 0;
        MySqlConnection? connection = null;

        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand("SELECT authority from team_member where team_id = @teamId and user_id = @userId", connection))
            {
                command.Parameters.AddWithValue("@teamId", teamId);
                command.Parameters.AddWithValue("@userId", userId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                        authority = reader.GetByte("authority");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"find team member by id error: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return authority;
    }
    
    public TeamMember FindTeamMemberId(int userId, int teamId)
    {
        var teamMember = new TeamMember();
        MySqlConnection? connection = null;

        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand("SELECT id FROM team_member WHERE user_id = @userId and team_id = @teamId", connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@teamId", teamId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                        teamMember = new TeamMember
                        {
                            memberId = reader.GetInt32("id"),
                            userId = reader.GetInt32("user_id"),
                            teamId = reader.GetInt32("team_id"),
                            authority = reader.GetByte("authority")
                        };
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"find team member by id error: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return teamMember;
    }
}