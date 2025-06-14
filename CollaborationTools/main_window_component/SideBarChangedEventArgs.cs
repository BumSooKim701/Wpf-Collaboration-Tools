using CollaborationTools.team;
using CollaborationTools.user;

namespace CollaborationTools;

public class SideBarChangedEventArgs : EventArgs
{
    public string TabType { get; set; } // "Team", "Personal" 등
    public Team? Team { get; set; }
    public User? User { get; set; }
}