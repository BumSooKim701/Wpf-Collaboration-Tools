namespace CollaborationTools.database;

using MySqlConnector;

public class LoginRepository
{
    private static MySqlConnection conn;

    private LoginRepository()
    {
        // Empty Constructor
    }
    
    public static MySqlConnection GetInstance()
    {
        if (conn == null)
        {
            conn = new MySqlConnection("Server=34.47.120.22; Uid=kimbumsoo; Pwd=rlaqjatn1026@; Database=collaboration-tools;");
        }
        
        return conn;
    }
}