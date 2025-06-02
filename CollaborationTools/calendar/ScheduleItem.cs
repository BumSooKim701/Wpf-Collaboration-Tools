using System.ComponentModel;

namespace CollaborationTools;

public class ScheduleItem : INotifyPropertyChanged
{
    private string _title;
    public string Title
    {
        get { return _title; }
        set
        {
            if (_title != value)
            {
                _title = value;
                NotifyPropertyChanged(nameof(Title));
            }
        }
    }

    private string _date;
    public string Date
    {
        get { return _date; }
        set
        {
            if (_date != value)
            {
                _date = value;
                NotifyPropertyChanged(nameof(Date));   
            }
        }
    }
    
    private string _dateDetails;
    public string DateDetails
    {
        get { return _dateDetails; }
        set
        {
            if (_dateDetails != value)
            {
                _dateDetails = value;
                NotifyPropertyChanged(nameof(DateDetails));   
            }
        }
    }
    
    private string _location;
    public string Location
    {
        get { return _location; }
        set
        {
            if (_location != value)
            {
                _location = value;
                NotifyPropertyChanged(nameof(Location));   
            }
        }
    }
    
    private string _description;
    public string Description
    {
        get { return _description; }
        set
        {
            if (_description != value)
            {
                _description = value;
                NotifyPropertyChanged(nameof(Description));   
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    
    private void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}