namespace CollaborationTools.team;

public class Team
{
    public int teamId { get; set; }

    public string uuid { get; set; } = Guid.NewGuid().ToString();

    public string teamName { get; set; }

    public int teamMemberCount { get; set; } = 0;

    public DateTime dateOfCreated { get; set; } = DateTime.Now;

    public string teamCalendarName { get; set; }

    public string teamCalendarId { get; set; }

    public string? teamDescription { get; set; } = null;
}