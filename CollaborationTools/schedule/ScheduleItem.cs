using System.ComponentModel;

namespace CollaborationTools;

public class ScheduleItem : INotifyPropertyChanged
{
    private string _scheduleTitle;
    public string scheduleTitle
    {
        get { return _scheduleTitle; }
        set
        {
            if (_scheduleTitle != value)
            {
                _scheduleTitle = value;
                NotifyPropertyChanged(nameof(scheduleTitle));
            }
        }
    }

    private string _scheduleDate;
    public string scheduleDate
    {
        get { return _scheduleDate; }
        set
        {
            if (_scheduleDate != value)
            {
                _scheduleDate = value;
                NotifyPropertyChanged(nameof(scheduleDate));   
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    
    private void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}