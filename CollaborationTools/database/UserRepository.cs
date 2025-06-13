using CollaborationTools.team;
using CollaborationTools.user;
using MySqlConnector;

namespace CollaborationTools.database;

public class UserRepository
{
    private readonly ConnectionPool _connectionPool = ConnectionPool.GetInstance();

    public User? FindUserById(int id)
    {
        User? user = null;
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
                        user = new User
                        {
                            userId = reader.GetInt32("id"),
                            GoogleId = reader.GetString("google_id"),
                            Email = reader.GetString("email"),
                            Name = reader.GetString("name"),
                            PictureUri = reader.GetString("picture_uri"),
                            CreatedAt = reader.GetDateTime("created_at"),
                            LastLoginAt = reader.GetDateTime("last_login_at")
                        };
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"사용자 아이디로 조회 중 오류: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return user;
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
                        user = new User
                        {
                            userId = reader.GetInt32("id"),
                            GoogleId = reader.GetString("google_id"),
                            Email = reader.GetString("email"),
                            Name = reader.GetString("name"),
                            PictureUri = reader.GetString("picture_uri"),
                            CreatedAt = reader.GetDateTime("created_at"),
                            LastLoginAt = reader.GetDateTime("last_login_at"),
                            TeamId = reader.GetInt32("team_id")
                        };
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"searching to email error: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return user;
    }

    public bool AddUser(User user)
    {
        MySqlConnection connection = null;
        var result = true;

        try
        {
            connection = _connectionPool.GetConnection();

            using (var command =
                   new MySqlCommand(
                       "INSERT INTO user (google_id, email, name, picture_uri, refresh_token, created_at, last_login_at) VALUES (@googleId, @email, @name, @pictureUri, @refreshToken, @createdAt, @lastLoginAt)",
                       connection))
            {
                command.Parameters.AddWithValue("@googleId", user.GoogleId);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@pictureUri", user.PictureUri);
                command.Parameters.AddWithValue("@refreshToken", user.RefreshToken);
                command.Parameters.AddWithValue("@createdAt", user.CreatedAt);
                command.Parameters.AddWithValue("@lastLoginAt", user.LastLoginAt);

                var executeResult = command.ExecuteNonQuery();

                if (executeResult == 0) result = false;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"사용자 이메일로 조회 중 오류: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return result;
    }
    
    public bool UpdatePrimaryTeamId(User user, int teamId)
    {
        MySqlConnection connection = null;
        var result = true;

        try
        {
            connection = _connectionPool.GetConnection();
            
            Console.WriteLine(user.userId + " " + teamId);
            using (var command =
                   new MySqlCommand(
                       "UPDATE user SET team_id = @teamId WHERE id = @userId",
                       connection))
            {
                command.Parameters.AddWithValue("@teamId", teamId);
                command.Parameters.AddWithValue("@userId", user.userId);

                var executeResult = command.ExecuteNonQuery();

                if (executeResult == 0) result = false;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"사용자 이메일로 조회 중 오류: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return result;
    }
}