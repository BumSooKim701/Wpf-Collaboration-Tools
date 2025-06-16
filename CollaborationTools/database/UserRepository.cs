using System.Data;
using System.Diagnostics;
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
                            PictureUri = reader.IsDBNull("picture_uri") ? null : reader.GetString("picture_uri"),
                            CreatedAt = reader.GetDateTime("created_at"),
                            LastLoginAt = reader.GetDateTime("last_login_at"),
                            TeamId = reader.GetInt32("team_id")
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
                            PictureUri = reader.IsDBNull("picture_uri") ? null : reader.GetString("picture_uri"),
                            CreatedAt = reader.GetDateTime("created_at"),
                            LastLoginAt = reader.GetDateTime("last_login_at"),
                            TeamId = reader.GetInt32("team_id")
                        };   
                }
            }
            using (var command =
                   new MySqlCommand(
                       "UPDATE user SET last_login_at = CURRENT_TIMESTAMP WHERE id = @userId;",
                       connection))
            {
                command.Parameters.AddWithValue("@userId", user.userId);

                var executeResult = command.ExecuteNonQuery();

                if (executeResult == 0) Debug.WriteLine("마지막 로그인날짜 갱신 실패");
                else Debug.WriteLine("마지막 로그인날짜 갱신 성공");
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
                       "INSERT INTO user (google_id, email, name, picture_uri, refresh_token, created_at, last_login_at, team_id) VALUES (@googleId, @email, @name, @pictureUri, @refreshToken, @createdAt, @lastLoginAt, @teamId)",
                       connection))
            {
                command.Parameters.AddWithValue("@googleId", user.GoogleId);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@pictureUri", user.PictureUri);
                command.Parameters.AddWithValue("@refreshToken", user.RefreshToken);
                command.Parameters.AddWithValue("@createdAt", user.CreatedAt);
                command.Parameters.AddWithValue("@lastLoginAt", user.LastLoginAt);
                command.Parameters.AddWithValue("@teamId", 0);

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

    public List<User> FindUsersInAddressBook(int userId)
    {
        var users = new List<User>();
        MySqlConnection connection = null;

        try
        {
            connection = _connectionPool.GetConnection();
            
            var query = "SELECT * FROM address_book WHERE owner = @ownerId";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ownerId", userId);
                
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var member = FindUserById(reader.GetInt32("member"));
                        
                        users.Add(member);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetAllUsers Error: {ex.Message}");
        }
        finally
        {
            // 반드시 연결을 풀에 반환
            if (connection != null)
            {
                _connectionPool.ReleaseConnection(connection);
            }
        }

        return users;
    }

    // UserRepository.cs에 추가할 메서드
    public List<User> GetAllUsers()
    {
        var users = new List<User>();
        MySqlConnection connection = null;

        try
        {
            connection = _connectionPool.GetConnection();
            
            var query = "SELECT * FROM user";

            using (var command = new MySqlCommand(query, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                    users.Add(new User
                    {
                        userId = reader.GetInt32("id"),
                        GoogleId = reader.GetString("google_id"),
                        Name = reader.GetString("name"),
                        Email = reader.GetString("email"),
                        PictureUri = reader.IsDBNull("picture_uri") ? null : reader.GetString("picture_uri"),
                        RefreshToken = reader.GetString("refresh_token"),
                        CreatedAt = reader.GetDateTime("created_at"),
                        LastLoginAt = reader.GetDateTime("last_login_at"),
                        TeamId = reader.GetInt32("team_id")
                    });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetAllUsers Error: {ex.Message}");
        }
        finally
        {
            // 반드시 연결을 풀에 반환
            if (connection != null)
            {
                _connectionPool.ReleaseConnection(connection);
            }
        }

        return users;
    }

    // public  GetPersonalActivityCount(int userId)
    // {
    //     
    // }
}