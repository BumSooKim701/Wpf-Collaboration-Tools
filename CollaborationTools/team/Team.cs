namespace CollaborationTools.team;

public class Team
{
    public static byte VISIBLE = 1;
    
    public static byte NON_VISIBLE = 0;
    
    public int teamId { get; set; }

    public string uuid { get; set; } = Guid.NewGuid().ToString();

    public string teamName { get; set; }

    public int teamMemberCount { get; set; } = 0;

    public DateTime dateOfCreated { get; set; } = DateTime.Now;

    public string teamCalendarName { get; set; }

    public string teamCalendarId { get; set; }

    public string? teamDescription { get; set; } = "_";

    public string teamFolderId { get; set; } = "_";
    
    public byte visibility { get; set; }
}