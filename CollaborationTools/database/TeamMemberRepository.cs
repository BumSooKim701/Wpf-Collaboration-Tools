using System.Data;
using CollaborationTools.team;
using CollaborationTools.user;
using MySqlConnector;

namespace CollaborationTools.database;

public class TeamMemberRepository
{
    private ConnectionPool _connectionPool = ConnectionPool.GetInstance();

    public bool AddTeamMember(Team team, User user)
    {
        MySqlConnection connection = null;
        bool result = true;
        
        try
        {
            connection = _connectionPool.GetConnection();
            
            using (var command = new MySqlCommand("INSERT INTO team_member VALUES (@userId, @teamId, @authority)", connection))
            {
                command.Parameters.AddWithValue("@userId", user.userId);
                command.Parameters.AddWithValue("@teamId", team.teamId);
                command.Parameters.AddWithValue("@authority", TeamMember.TEAM_MEMBER_AUTHORITY);

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

    public bool RemoveTeamMember(TeamMember teamMember)
    {
        MySqlConnection connection = null;
        bool result = true;
        
        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand("DELETE FROM teammember WHERE id = @memberId", connection))
            {
                command.Parameters.AddWithValue("@memberId", teamMember.memberId);

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

    public List<TeamMember?> FindTeamMemberByUserId(int userId)
    {
        List<TeamMember?> teamMemberList =  new List<TeamMember?>();
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
                    {
                        teamMemberList.Add(new TeamMember
                        {
                            memberId = reader.GetInt32("member_id"),
                            teamId = reader.GetInt32("team_id"),
                            authority = reader.GetInt32("authority")
                        });
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"find team member by id error: {e.Message}");
        }
        finally
        {
            if (connection != null)
            {
                _connectionPool.ReleaseConnection(connection);
            }
        }
        
        return teamMemberList;
    }
}