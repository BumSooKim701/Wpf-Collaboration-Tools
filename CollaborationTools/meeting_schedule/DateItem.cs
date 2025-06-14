using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CollaborationTools.meeting_schedule;

public class DateItem : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private DateTime _date;

    public DateItem()
    {
        Date = DateTime.Now;
    }
    public DateItem(DateTime date)
    {
        Date = date;
    }

    public DateTime Date
    {
        get => _date;
        set
        {
            _date = value;
            OnPropertyChanged();
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}