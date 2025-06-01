using System.ComponentModel;

namespace CollaborationTools;

public class TeamItem : INotifyPropertyChanged
{
    
    private string _TeamName;
    public string TeamName { 
        get {return _TeamName;}
        set
        {
            if (_TeamName != value)
            {
                _TeamName = value;
                NotifyPropertyChanged(nameof(TeamName));
            }
        }
    }
    
    public TeamItem(string teamName)
    {
        this.TeamName = teamName;
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}