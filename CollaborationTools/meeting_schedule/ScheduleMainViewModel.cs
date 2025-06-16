using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CollaborationTools.meeting_schedule;

public class ScheduleMainViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private ObservableCollection<ScheduleRowViewModel> _scheduleRows;
    public ObservableCollection<ScheduleRowViewModel> ScheduleRows
    {
        get => _scheduleRows;
        set
        {
            _scheduleRows = value;
            OnPropertyChanged(); 
        }
    }
    
    public ScheduleMainViewModel(Meeting meeting)
    {
        var meetingPlan = meeting;
        var meetingService = new MeetingService();
        meetingPlan.DateList = meetingService.GetMeetingDates(meetingPlan.MeetingId);

        ScheduleRows = new ObservableCollection<ScheduleRowViewModel>();

        foreach (var dateItem in meetingPlan.DateList)
        {
            var scheduleRow = new ScheduleRowViewModel()
            {
                Date = dateItem.Date,
            };
            ScheduleRows.Add(scheduleRow);
        }
        

    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}