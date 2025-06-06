namespace CollaborationTools.team;

public class TeamMember
{
    public static byte TEAM_LEADER_AUTHORITY = 1;
    
    public static byte TEAM_MEMBER_AUTHORITY = 0;
    
    public int memberId { get; set; }

    public int teamId {  get; set; }
    
    public int authority { get; set; }
}