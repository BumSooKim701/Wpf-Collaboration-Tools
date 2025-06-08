using System.ComponentModel;

namespace CollaborationTools;

public class TeamItem : INotifyPropertyChanged
{
    private string _TeamName;

    public TeamItem(string teamName)
    {
        TeamName = teamName;
    }

    public string TeamName
    {
        get => _TeamName;
        set
        {
            if (_TeamName != value)
            {
                _TeamName = value;
                NotifyPropertyChanged(nameof(TeamName));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}