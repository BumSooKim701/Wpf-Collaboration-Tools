using System.Collections.ObjectModel;
using CollaborationTools.memo;
using CollaborationTools.team;
using MySqlConnector;

namespace CollaborationTools.database;

public class MemoRepository
{
    private readonly ConnectionPool _connectionPool;
    TeamMemberRepository teamMemberRepository = new TeamMemberRepository();

    public MemoRepository()
    {
        _connectionPool = ConnectionPool.GetInstance();
    }

    public MemoItem? GetMemosById(int memoId)
    {
        MySqlConnection connection = null;
        MemoItem? memoItem = null;

        try
        {
            connection = _connectionPool.GetConnection();
            
            using (var command = new MySqlCommand(
                       "SELECT * FROM team_memo WHERE id = @memoId", connection))
            {
                command.Parameters.AddWithValue("@memoId", memoId);
                
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        memoItem = new MemoItem
                        {
                            MemoId = reader.GetInt32("id"),
                            Title = reader.GetString("memo_title"),
                            Content = reader.GetString("memo_content"),
                            LastModifiedDate = reader.GetDateTime("date_of_modified"),
                            LastEditorName = " ",
                            TeamId = 0,
                            EditorUserId = reader.GetInt32("team_member_id")
                        };
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }
        
        return memoItem;
    }

    public async Task<ObservableCollection<MemoItem>> GetMemosByTeamId(int teamId)
    {
        MySqlConnection connection = null;
        var result = true;
        var memoItems = new ObservableCollection<MemoItem>();

        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand(
                       "SELECT * FROM vw_team_memo WHERE team_id=@team_id ORDER BY date_of_modified DESC, id DESC",
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
                            LastEditorName = reader.GetString("last_editor_name"),
                            TeamId = teamId
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
                       "SELECT ROW_COUNT() AS AffectedRows, LAST_INSERT_ID() AS NewId;",
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
                        UpdateMemoCountPlus(memoItem.EditorUserId, memoItem.TeamId);
                        var rowsAffected = Convert.ToInt32(reader["AffectedRows"]);
                        var newId = Convert.ToInt32(reader["NewId"]);

                        if (rowsAffected > 0)
                        {
                            memoItem.MemoId = newId;
                            result = true;
                        }
                    }
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

    public bool AddMemoForUser(MemoItem memoItem, int memberId)
    {
        MySqlConnection connection = null;
        var result = false;

        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand(
                       "INSERT INTO team_memo (memo_title, memo_content, date_of_modified, team_member_id) " +
                       "VALUES (@memoTitle, @memoContent, @dateOfModified, @memberId);" +
                       "SELECT ROW_COUNT() AS AffectedRows, LAST_INSERT_ID() AS NewId;",
                       connection))
            {
                
                command.Parameters.AddWithValue("@memoTitle", memoItem.Title);
                command.Parameters.AddWithValue("@memoContent", memoItem.Content);
                command.Parameters.AddWithValue("@dateOfModified", memoItem.LastModifiedDate);
                command.Parameters.AddWithValue("@memberId", memberId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        UpdateMemoCountPlus(memberId);
                        var rowsAffected = Convert.ToInt32(reader["AffectedRows"]);
                        var newId = Convert.ToInt32(reader["NewId"]);

                        if (rowsAffected > 0)
                        {
                            memoItem.MemoId = newId;
                            result = true;
                        }
                    }
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

    public bool UpdateMemo(MemoItem memoItem)
    {
        MySqlConnection connection = null;
        var result = false;

        try
        {
            connection = _connectionPool.GetConnection();
            using (var command = new MySqlCommand(
                       "UPDATE team_memo SET memo_title = @memoTitle, memo_content = @memoContent, date_of_modified = @dateOfModified, " +
                       "team_member_id = (SELECT id FROM team_member WHERE team_id = @teamId and user_id = @userId)" +
                       "WHERE id = @memoId", connection))
            {
                command.Parameters.AddWithValue("@memoTitle", memoItem.Title);
                command.Parameters.AddWithValue("@memoContent", memoItem.Content);
                command.Parameters.AddWithValue("@dateOfModified", memoItem.LastModifiedDate);
                command.Parameters.AddWithValue("@teamId", memoItem.TeamId);
                command.Parameters.AddWithValue("@userId", memoItem.EditorUserId);
                command.Parameters.AddWithValue("@memoId", memoItem.MemoId);

                var executeResult = command.ExecuteNonQuery();

                if (executeResult > 0) result = true;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error updating memo: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return result;
    }

    private bool UpdateMemoCountPlus(int memberId)
    {
        MySqlConnection connection = null;
        var result = false;

        try
        {
            
            connection = _connectionPool.GetConnection();
            using (var command = new MySqlCommand(
                       "UPDATE team_member SET memo_count = memo_count + 1 " +
                       "WHERE id = @memberId", connection))
            {
                command.Parameters.AddWithValue("@memberId", memberId);

                var executeResult = command.ExecuteNonQuery();

                if (executeResult > 0) result = true;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error updating memo Plus: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return result;
    }
    
    private bool UpdateMemoCountPlus(int userId, int teamId)
    {
        MySqlConnection connection = null;
        var result = false;

        try
        {
            connection = _connectionPool.GetConnection();
            using (var command = new MySqlCommand(
                       "UPDATE team_member SET memo_count = memo_count + 1 " +
                       "WHERE user_id = @userId and team_id = @teamId", connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@teamId", teamId);

                var executeResult = command.ExecuteNonQuery();

                if (executeResult > 0) result = true;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error updating memo Plus: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return result;
    }
    
    private bool UpdateMemoCountMinus(int memoId)
    {
        MySqlConnection connection = null;
        var result = false;

        try
        {
            connection = _connectionPool.GetConnection();
     
            MemoItem? memo = GetMemosById(memoId);
            
            TeamMember member =teamMemberRepository.FindTeamMemberId(memo.EditorUserId);
    
            
            using (var command = new MySqlCommand(
                       "UPDATE team_member SET memo_count = memo_count - 1 " +
                       "WHERE id = @memberId", connection))
            {
                command.Parameters.AddWithValue("@memberId", member.memberId);

                var executeResult = command.ExecuteNonQuery();

                if (executeResult > 0) result = true;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error updating memo Minus: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return result;
    }

    public bool DeleteMemo(int memoId)
    {
        MySqlConnection connection = null;
        var result = false;

        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand("DELETE FROM team_memo WHERE id = @memoId", connection))
            {
                command.Parameters.AddWithValue("@memoId", memoId);
                
                UpdateMemoCountMinus(memoId);
                var executeResult = command.ExecuteNonQuery();

                if (executeResult > 0)
                {
                    result = true;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error deleting memo: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return result;
    }
}