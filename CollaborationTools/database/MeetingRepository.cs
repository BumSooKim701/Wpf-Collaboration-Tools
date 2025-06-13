using CollaborationTools.meeting_schedule;
using MySqlConnector;

namespace CollaborationTools.database;

public class MeetingRepository
{
    private readonly ConnectionPool _connectionPool;

    
    public MeetingRepository()
    {
        _connectionPool = ConnectionPool.GetInstance();
    }


    public Meeting GetMeeting(int teamId)
    {
        MySqlConnection connection = null;
        var result = true;
        var meetingPlan = new Meeting();
        
        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand(
                       "SELECT * FROM meeting_schedule WHERE team_id=@team_id", connection))
            {
                command.Parameters.AddWithValue("@team_id", teamId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        meetingPlan.Title = reader.GetString("title");
                        meetingPlan.ToDo = reader.GetString("todo");
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching meeting plan: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }
        
        return meetingPlan;
    }
}