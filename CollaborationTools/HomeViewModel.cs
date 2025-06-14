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