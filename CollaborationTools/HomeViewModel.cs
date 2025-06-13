using System.ComponentModel;
using System.Runtime.CompilerServices;
using CollaborationTools.meeting_schedule;
using CollaborationTools.team;

namespace CollaborationTools;

public class HomeViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private Meeting _meeting = new Meeting();
    private Team _currentTeam;


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
    
    private void UpdateMeeting()
    {
        if (CurrentTeam != null)
        {
            MeetingService meetingService = new();
            Meeting meetingPlan = meetingService.GetMeeting(CurrentTeam);
            Meeting.Title = meetingPlan.Title;
            Meeting.ToDo = meetingPlan.ToDo;
        }
    }
    

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}