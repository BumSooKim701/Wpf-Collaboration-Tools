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

    private string _startDate;
    public string startDate
    {
        get { return _startDate; }
        set
        {
            if (_startDate != value)
            {
                _startDate = value;
                NotifyPropertyChanged(nameof(startDate));   
            }
        }
    }
    
    private string _endDate;
    public string endDate
    {
        get { return _endDate; }
        set
        {
            if (_endDate != value)
            {
                _endDate = value;
                NotifyPropertyChanged(nameof(endDate));   
            }
        }
    }
    
    private string _startTime;
    public string startTime
    {
        get { return _startTime; }
        set
        {
            if (_startTime != value)
            {
                _startTime = value;
                NotifyPropertyChanged(nameof(startTime));   
            }
        }
    }
    
    private string _endTime;
    public string endTime
    {
        get { return _endTime; }
        set
        {
            if (_endTime != value)
            {
                _endTime = value;
                NotifyPropertyChanged(nameof(endTime));   
            }
        }
    }
    
    private string _location;
    public string location
    {
        get { return _location; }
        set
        {
            if (_location != value)
            {
                _location = value;
                NotifyPropertyChanged(nameof(location));   
            }
        }
    }
    
    private string _description;
    public string description
    {
        get { return _description; }
        set
        {
            if (_description != value)
            {
                _description = value;
                NotifyPropertyChanged(nameof(description));   
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    
    private void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}