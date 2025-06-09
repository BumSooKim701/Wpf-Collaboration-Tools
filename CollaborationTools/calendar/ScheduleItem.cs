using System.ComponentModel;
using System.Runtime.CompilerServices;
using Google.Apis.Calendar.v3.Data;
using MySqlConnector;

namespace CollaborationTools.calendar;

public class ScheduleItem: INotifyPropertyChanged
{
    private string _title;
    public string Title
    {
        get => _title;
        set
        {
            if(_title != value)
            {
                _title = value; 
                OnPropertyChanged();
                
            }
        }
    }

    private DateTime _startDateTime;
    public DateTime StartDateTime
    {
        get => _startDateTime;
        set
        {
            if (_startDateTime != value)
            {
                _startDateTime = value;
                OnPropertyChanged();
            }
        }
    }
    
    private DateTime _endDateTime;
    public DateTime EndDateTime
    {
        get => _endDateTime;
        set
        {
            if (_endDateTime != value)
            {
                _endDateTime = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _isAllDayEvent;
    public bool IsAllDayEvent
    {
        get => _isAllDayEvent;
        set
        {
            if (_isAllDayEvent != value)
            {
                _isAllDayEvent = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _isOneDayEvent;
    public bool IsOneDayEvent
    {
        get => _isOneDayEvent;
        set
        {
            if (_isOneDayEvent != value)
            {
                _isOneDayEvent = value;
                OnPropertyChanged();
            }
        }
    }

    private string _location;
    public string Location
    {
        get => _location;
        set
        {
            if (_location != value)
            {
                _location = value;
                OnPropertyChanged();
            }
        }
    }
    
    private string _description;
    public string Description
    {
        get => _description;
        set
        {
            if (_description != value)
            {
                _description = value;
                OnPropertyChanged();
            }
        }
    }

    private string _calendarId;
    public string CalendarId
    {
        get => _calendarId;
        set
        {
            if (_calendarId != value)
            {
                _calendarId = value;
                OnPropertyChanged();
            }
        }
    }
    
    private Event _event;
    public Event Event
    {
        get
        {
            UpdateEvent();
            return _event;
        }
        set 
        {  
            _event = value;
        }
    }

    private void UpdateEvent()
    {
        _event.Summary = Title;
        _event.Location = Location;
        _event.Description = Description;
        _event.Start = CreateEventDateTime(StartDateTime, IsAllDayEvent, false);
        _event.End   = CreateEventDateTime(EndDateTime, IsAllDayEvent, true);
    }
    
    private EventDateTime CreateEventDateTime(DateTime dateTime, bool isAllDayEvent, bool isEnd)
    {
        var adjustedDateTime = isEnd ? dateTime.AddDays(1) : dateTime;

        return isAllDayEvent
            ? new EventDateTime
            {
                Date = adjustedDateTime.Date.ToString("yyyy-MM-dd"),
                TimeZone = "Asia/Seoul"
            }
            : new EventDateTime
            {
                DateTimeDateTimeOffset = isEnd ? dateTime.AddDays(1) : dateTime,
                TimeZone = "Asia/Seoul"
            };
    }


    public ScheduleItem Clone()
    {
        var clone = new ScheduleItem
        {
            Title = this.Title,
            StartDateTime = this.StartDateTime,
            EndDateTime = this.EndDateTime,
            IsAllDayEvent = this.IsAllDayEvent,
            IsOneDayEvent = this.IsOneDayEvent,
            Location = this.Location,
            Description = this.Description,
            CalendarId = this.CalendarId,
            Event = this.Event
        };

        return clone;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}