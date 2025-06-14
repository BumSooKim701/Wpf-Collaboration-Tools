using CollaborationTools.user;
using MySqlConnector;

namespace CollaborationTools.database;

public class AddressRepository
{
    private readonly ConnectionPool _connectionPool = ConnectionPool.GetInstance();
    
    public bool AddAddress(int memberId)
    {
        MySqlConnection connection = null;
        var result = true;

        try
        {
            connection = _connectionPool.GetConnection();

            using (var command =
                   new MySqlCommand(
                       "INSERT INTO address_book (owner, member) VALUES (@userId, @memberId)",
                       connection))
            {
                command.Parameters.AddWithValue("@userId", UserSession.CurrentUser.userId);
                command.Parameters.AddWithValue("@memberId", memberId);

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
    
    public bool RemoveAddress(int ownerId, int memberId)
    {
        MySqlConnection connection = null;
        var result = true;

        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand("DELETE FROM address_book WHERE owner = @ownerId and member = @memberId", connection))
            {
                command.Parameters.AddWithValue("@ownerId", ownerId);
                command.Parameters.AddWithValue("@memberId", memberId);

                var executeResult = command.ExecuteNonQuery();

                if (executeResult == 0) result = false;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error deleting team: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return result;
    }
}