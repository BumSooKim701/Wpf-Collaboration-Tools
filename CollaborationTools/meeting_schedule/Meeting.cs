using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CollaborationTools.meeting_schedule;

public class Meeting : INotifyPropertyChanged
{
    private string _title;
    private string _toDo;

    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged();   
        }
    }

    public string ToDo
    {
        get => _toDo;
        set
        {
            _toDo = value;
            OnPropertyChanged();  
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}