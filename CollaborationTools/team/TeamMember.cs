namespace CollaborationTools.team;

public class TeamMember
{
    public static int TEAM_LEADER_AUTHORITY = 1;
    
    public static int TEAM_MEMBER_AUTHORITY = 0;
    
    public int memberId { get; set; }

    public int teamId {  get; set; }
    
    public int authority { get; set; }
}