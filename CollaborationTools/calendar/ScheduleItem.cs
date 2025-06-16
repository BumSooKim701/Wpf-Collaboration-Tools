using System.ComponentModel;
using System.Runtime.CompilerServices;
using Google.Apis.Calendar.v3.Data;

namespace CollaborationTools.calendar;

public class ScheduleItem : INotifyPropertyChanged
{
    private string _calendarId;

    private string _description;

    private DateTime _endDateTime;

    // calendar api로 일정을 생성,수정할 때 Event 타입으로 보내야 함
    private Event _event;

    private bool _isAllDayEvent;

    private bool _isOneDayEvent;

    private string _location;

    private DateTime _startDateTime;
    private string _title;

    public ScheduleItem()
    {
        _event = new Event();
    }

    public string Title
    {
        get => _title;
        set
        {
            if (_title != value)
            {
                _title = value;
                OnPropertyChanged();
            }
        }
    }

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

    public Event Event
    {
        get
        {
            UpdateEvent();
            return _event;
        }
        set => _event = value;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void UpdateEvent()
    {
        if (IsAllDayEvent == null) throw new NullReferenceException("ScheduleItem IsAllDayEvent property is not null");

        _event.Summary = Title;
        _event.Location = Location;
        _event.Description = Description;
        _event.Start = CreateEventDateTime(StartDateTime, IsAllDayEvent, false);
        _event.End = CreateEventDateTime(EndDateTime, IsAllDayEvent, true);
    }

    // google calendar api의 event에 저장하기 위해 EventDateTime 형식으로 저장
    private EventDateTime CreateEventDateTime(DateTime dateTime, bool isAllDayEvent, bool isEnd)
    {
        // 종일 이벤트의 종료날짜는 1일을 더해야 함
        return isAllDayEvent
            ? new EventDateTime
            {
                Date = isEnd
                    ? dateTime.AddDays(1).Date.ToString("yyyy-MM-dd")
                    : dateTime.Date.ToString("yyyy-MM-dd"),
                TimeZone = "Asia/Seoul"
            }
            : new EventDateTime
            {
                DateTimeDateTimeOffset = dateTime,
                TimeZone = "Asia/Seoul"
            };
    }
    
    public ScheduleItem Clone()
    {
        var clone = new ScheduleItem
        {
            Title = Title,
            StartDateTime = StartDateTime,
            EndDateTime = EndDateTime,
            IsAllDayEvent = IsAllDayEvent,
            IsOneDayEvent = IsOneDayEvent,
            Location = Location,
            Description = Description,
            CalendarId = CalendarId,
            Event = Event
        };

        return clone;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}