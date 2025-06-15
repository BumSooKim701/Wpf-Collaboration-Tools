using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CollaborationTools.meeting_schedule;
using CollaborationTools.team;

namespace CollaborationTools;

public class HomeViewModel : INotifyPropertyChanged
{
    private Team _currentTeam;
    private Meeting _meeting = new();
    private MeetingViewType _viewType = MeetingViewType.NoPlan;
    private ObservableCollection<FormattedSchedule> _formatedSchedules = new();
    
    
    
    public Meeting Meeting
    {
        get => _meeting;
        set
        {
            _meeting = value;
            OnPropertyChanged();
        }
    }

    public Team CurrentTeam
    {
        get => _currentTeam;
        set
        {
            _currentTeam = value;
            UpdateMeeting();
            OnPropertyChanged();
        }
    }

    public MeetingViewType ViewType
    {
        get => _viewType;
        set
        {
            _viewType = value;
            OnPropertyChanged();
        }
    }
    
    
    public ObservableCollection<FormattedSchedule> FormattedSchedules
    {
        get => _formatedSchedules;
        set
        {
            _formatedSchedules = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void UpdateMeeting()
    {
        if (CurrentTeam != null)
        {
            MeetingService meetingService = new();
            var meetingPlan = meetingService.GetMeeting(CurrentTeam);

            if (meetingPlan == null)
            {
                _viewType = MeetingViewType.NoPlan;
            }
            else
            {
                Meeting.MeetingId = meetingPlan.MeetingId;
                Meeting.Title = meetingPlan.Title;
                Meeting.ToDo = meetingPlan.ToDo;
                Meeting.Status = meetingPlan.Status;
                Meeting.TeamId = meetingPlan.TeamId;

                if (Meeting.Status == 0)
                    _viewType = MeetingViewType.Arranging;
                else if (Meeting.Status == 1) _viewType = MeetingViewType.Scheduled;
            }
        }
    }


    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public enum MeetingViewType
{
    NoPlan,
    Arranging,
    Scheduled
}

public class FormattedSchedule : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private string _date;
    private string _startTime;
    private string _endTime;

    public string Date
    {
        get => _date;
        set
        {
            _date = value;
            OnPropertyChanged();
        }
    }

    public string StartTime
    {
        get => _startTime;
        set
        {
            _startTime = value;
            OnPropertyChanged();
        }
    }

    public string EndTime
    {
        get => _endTime;
        set
        {
            _endTime = value;
            OnPropertyChanged();
        }
    }

    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}