using CollaborationTools.user;
using MySqlConnector;

namespace CollaborationTools.database;

public class LoginRepository
{
    private ConnectionPool _connectionPool = ConnectionPool.GetInstance();

    public User CheckUserId(string userId)
    {
        MySqlConnection connection = null;
        User user = null;
        
        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand("SELECT user_account_id FROM user_account WHERE user_account_id = @id", connection))
            {
                command.Parameters.AddWithValue("@id", userId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new User();
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"사용자 아이디로 조회 중 오류: {e.Message}");
        }
        finally
        {
            if (connection != null)
            {
                _connectionPool.ReleaseConnection(connection);
            }
        }
        
        return user;
    }
    
    public bool CheckUserpassword()
    {
     
        return true;
    }
}