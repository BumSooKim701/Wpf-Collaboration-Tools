using System.Collections.ObjectModel;
using CollaborationTools.memo;
using CollaborationTools.team;
using MySqlConnector;

namespace CollaborationTools.database;

public class MemoRepository
{
    private readonly ConnectionPool _connectionPool;
    
    public MemoRepository()
    {
        _connectionPool = ConnectionPool.GetInstance();
    }

    public ObservableCollection<MemoItem> GetMemosByTeamId(int teamId)
    {
        MySqlConnection connection = null;
        var result = true;
        var memoItems = new ObservableCollection<MemoItem>();
        
        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand(
                       "SELECT * FROM vw_team_memo WHERE team_id=@team_id",
                       connection))
            {
                command.Parameters.AddWithValue("@team_id", teamId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        memoItems.Add(new MemoItem
                        {
                            MemoId = reader.GetInt32("id"),
                            Title = reader.GetString("memo_title"),
                            Content = reader.GetString("memo_content"),
                            LastModifiedDate = reader.GetDateTime("date_of_modified"),
                            TeamId = reader.GetInt32("team_id"),
                            LastEditorName = reader.GetString("last_editor_name"),
                        });
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching memo: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }
        
        return memoItems;
    }
}