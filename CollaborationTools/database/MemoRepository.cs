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
                            EditorUserId = reader.GetInt32("editor_user_id"),
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

    public bool AddMemo(MemoItem memoItem)
    {
        MySqlConnection connection = null;
        var result = false;
        
        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand(
                       "INSERT INTO team_memo (memo_title, memo_content, date_of_modified, team_member_id) " +
                       "VALUES (@memoTitle, @memoContent, @dateOfModified, " +
                       "(SELECT id FROM team_member WHERE user_id = @userId AND team_id = @teamId)\n);" +
                       "SELECT ROW_COUNT() AS AffectedRows, LAST_INSERT_ID() AS NewId;"
                       ,
                       connection))
            {
                command.Parameters.AddWithValue("@memoTitle", memoItem.Title);
                command.Parameters.AddWithValue("@memoContent", memoItem.Content);
                command.Parameters.AddWithValue("@dateOfModified", memoItem.LastModifiedDate);
                command.Parameters.AddWithValue("@userId", memoItem.EditorUserId);
                command.Parameters.AddWithValue("@teamId", memoItem.TeamId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int rowsAffected = Convert.ToInt32(reader["AffectedRows"]);
                        int newId = Convert.ToInt32(reader["NewId"]);
                        
                        if (rowsAffected > 0)
                        {
                            memoItem.MemoId = newId;
                            return true;
                        }
                    }

                    return false;
                }

            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error adding memo: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return result;
    }
}