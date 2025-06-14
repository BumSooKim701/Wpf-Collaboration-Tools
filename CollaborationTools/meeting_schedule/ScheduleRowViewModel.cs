using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CollaborationTools.Common;

namespace CollaborationTools.meeting_schedule;

public class ScheduleRowViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private DateTime _date;
    private bool _isAllDay;
    private bool _isButtonEnabled = true;
    
    public DateTime Date
    {
        get => _date;
        set
        {
            _date = value;
            OnPropertyChanged();
        }
    }

    public bool IsAllDay
    {
        get => _isAllDay;
        set
        {
            _isAllDay = value;
            OnPropertyChanged();
            IsButtonEnabled = !value;
            if (value && TimeRanges.Count > 1)
            {
                TimeRanges.Clear();
                TimeRanges.Add(new TimeRange());    
            }
            TimeRanges[0].IsEnabled = !value;
            TimeRanges[0].StartTime = DateTime.MinValue;
            TimeRanges[0].EndTime = DateTime.MinValue;
        }
    }

    public bool IsButtonEnabled
    {
        get => _isButtonEnabled;
        set
        {
            _isButtonEnabled = value;
            OnPropertyChanged();
        }
    }
        
    public ObservableCollection<TimeRange> TimeRanges { get; } = new ObservableCollection<TimeRange>();
    public ICommand AddTimeRangeCommand { get; }
    public ICommand RemoveTimeRangeCommand { get; }

    public ScheduleRowViewModel()
    {
        AddTimeRangeCommand = new RelayCommand(AddTimeRange);
        RemoveTimeRangeCommand = new RelayCommand(RemoveTimeRange);
        TimeRanges.Add(new TimeRange());
        Date = DateTime.Now;
        IsAllDay = false;
    }

    private void AddTimeRange(object obj) => TimeRanges.Add(new TimeRange());

    private void RemoveTimeRange(object obj)
    {
        if (TimeRanges.Count > 1)
            TimeRanges.RemoveAt(TimeRanges.Count - 1);
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}

public class TimeRange : INotifyPropertyChanged
{
    private DateTime _startTime;
    private DateTime _endTime;
    private bool _isEnabled = true; // 기본값은 활성화
    
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            _isEnabled = value;
            OnPropertyChanged();
        }
    }
    public DateTime StartTime
    {
        get => _startTime;
        set
        {
            _startTime = value;
            OnPropertyChanged();
        }
    }

    public DateTime EndTime
    {
        get => _endTime;
        set
        {
            _endTime = value;
            OnPropertyChanged();
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;

    public TimeRange()
    {
        StartTime = DateTime.MinValue;
        EndTime = DateTime.MinValue;
    }
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
}
