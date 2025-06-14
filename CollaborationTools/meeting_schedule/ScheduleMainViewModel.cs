using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CollaborationTools.Common;

namespace CollaborationTools.meeting_schedule;

public class ScheduleMainViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public ObservableCollection<ScheduleRowViewModel> ScheduleRows { get; } = new();
    public ICommand AddDateRowCommand { get; }

    
    public ScheduleMainViewModel()
    {
        AddDateRowCommand = new RelayCommand(AddDateRow);
        AddDateRow(null); 
    }

    
    private void AddDateRow(object obj) => ScheduleRows.Add(new ScheduleRowViewModel());

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}