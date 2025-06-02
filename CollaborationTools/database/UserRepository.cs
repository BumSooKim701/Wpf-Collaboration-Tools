using CollaborationTools.user;
using MySqlConnector;

namespace CollaborationTools.database;

public class UserRepository
{
    private ConnectionPool _connectionPool = ConnectionPool.GetInstance();
    
    public User FindUserById(string id)
    {
        User user = null;
        MySqlConnection connection = null;

        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand("SELECT * FROM user WHERE id = @id", connection))
            {
                command.Parameters.AddWithValue("@id", id);

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
    
    public List<User> FindUserByName(string name)
    {
        List<User> userList = null;
        MySqlConnection connection = null;

        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand("SELECT * FROM user WHERE name = @name", connection))
            {
                command.Parameters.AddWithValue("@name", name);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        userList.Add(new User());
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"사용자 아름으로 조회 중 오류: {e.Message}");
        }
        finally
        {
            if (connection != null)
            {
                _connectionPool.ReleaseConnection(connection);
            }
        }

        return userList;
    }

    public User? FindUserByEmail(string email)
    {
        User? user = null;
        MySqlConnection connection = null;

        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand("SELECT * FROM user WHERE email = @email", connection))
            {
                command.Parameters.AddWithValue("@email", email);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new User
                        {
                            GoogleId = reader.GetString("google_id"),
                            Email = reader.GetString("email"),
                            Name = reader.GetString("name"),
                            PictureUrl = reader.GetString("picture_uri"),
                            CreatedAt = reader.GetDateTime("created_at"),
                            LastLoginAt = reader.GetDateTime("last_login_at")
                        };
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"사용자 이메일로 조회 중 오류: {e.Message}");
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

    public bool AddUser(User user)
    {
        MySqlConnection connection = null;
        bool result = true;
        
        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand("INSERT INTO user VALUES (@googleId, @email, @name, @pictureUri, @refreshToken, @createdAt, @lastLoginAt)", connection))
            {
                command.Parameters.AddWithValue("@googleId", user.GoogleId);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@pictureUri", user.PictureUrl);
                command.Parameters.AddWithValue("@refreshToken", user.RefreshToken);
                command.Parameters.AddWithValue("@createdAt", user.CreatedAt);
                command.Parameters.AddWithValue("@lastLoginAt", user.LastLoginAt);

                int executeResult = command.ExecuteNonQuery();
                
                if (executeResult == 0)
                {
                    result = false;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"사용자 이메일로 조회 중 오류: {e.Message}");
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
}