namespace CollaborationTools.file;

public class File
{
    public string fileId { get; set;}
    public string fileName { get; set;}
    public DateTime dateOfCreated { get; set;}
    public int lastFileVersion { get; set;}
    public int userId { get; set;}
    public int teamId { get; set;}
    public string folderId { get; set;}
}