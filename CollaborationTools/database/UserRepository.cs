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
                        user = new User(reader.GetString("id"), reader.GetString("name"),
                            reader.GetString("phoneNumber"), reader.GetString("email"),
                            reader.GetDateTime("registrationDate"));
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
                        userList.Add(new User(reader.GetString("id"), reader.GetString("name"),
                            reader.GetString("phoneNumber"), reader.GetString("email"),
                            reader.GetDateTime("registrationDate")));
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

    public User FindUserByEmail(string email)
    {
        User user = null;
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
                        user = new User(reader.GetString("id"), reader.GetString("name"),
                            reader.GetString("phoneNumber"), reader.GetString("email"),
                            reader.GetDateTime("registrationDate"));
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
}