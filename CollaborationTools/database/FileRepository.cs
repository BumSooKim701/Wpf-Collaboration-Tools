using CollaborationTools.file;
using MySqlConnector;

namespace CollaborationTools.database
{
    public class FileRepository
    {
        private readonly ConnectionPool connectionPool = ConnectionPool.GetInstance();

        public bool AddFile(File file)
        {
            MySqlConnection connection = null;
            var result = true;
            
            try
            {
                connection = connectionPool.GetConnection();
                using var command = new MySqlCommand(
                    "INSERT INTO file (file_name, date_of_created, last_file_version_number, user_id, team_id, folder_id) " +
                    "VALUES (@fileName, @dateOfCreated, @lastFileVersion, @userId, @teamId, @folderId)", 
                    connection);
                
                command.Parameters.AddWithValue("@fileName", file.fileName);
                command.Parameters.AddWithValue("@dateOfCreated", file.dateOfCreated);
                command.Parameters.AddWithValue("@lastFileVersion", file.lastFileVersion);
                command.Parameters.AddWithValue("@userId", file.userId);
                command.Parameters.AddWithValue("@teamId", file.teamId);
                command.Parameters.AddWithValue("@folderId", file.folderId);
                
                var executeResult = command.ExecuteNonQuery();
                if (executeResult <= 0) result = false;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error adding file: {e.Message}");
                result = false;
            }
            finally
            {
                if (connection != null) connectionPool.ReleaseConnection(connection);
            }
            
            return result;
        }

        public List<File> GetFilesByTeamId(int teamId)
        {
            var files = new List<File>();
            MySqlConnection connection = null;
            
            try
            {
                connection = connectionPool.GetConnection();
                using var command = new MySqlCommand(
                    "SELECT * FROM file WHERE teamid = @teamId", connection);
                command.Parameters.AddWithValue("@teamId", teamId);
                
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    files.Add(new File
                    {
                        fileId = reader.GetString("file_id"),
                        fileName = reader.GetString("file_name"),
                        dateOfCreated = reader.GetDateTime("date_of_created"),
                        lastFileVersion = reader.GetInt32("last_file_version"),
                        userId = reader.GetInt32("user_id"),
                        teamId = reader.GetInt32("team_id"),
                        folderId = reader.GetString("folder_id")
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error getting files: {e.Message}");
            }
            finally
            {
                if (connection != null) connectionPool.ReleaseConnection(connection);
            }
            
            return files;
        }
    }
}