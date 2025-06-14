using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CollaborationTools.Common;

namespace CollaborationTools.meeting_schedule;

public class Meeting : INotifyPropertyChanged
{
    private int _meetingId;
    private byte _status;
    private int _teamId;
    private string _title;
    private string _toDo;
    public ObservableCollection<DateItem> DateList { get; set; } = new ();
    public ICommand AddDateRowCommand { get; }
    public ICommand RemoveDateRowCommand { get; }

    
    public Meeting()
    {
        AddDateRowCommand = new RelayCommand(AddDateRow);
        RemoveDateRowCommand = new RelayCommand(RemoveDateRow);
        AddDateRow(null); 
    }
    

    public int MeetingId
    {
        get => _meetingId;
        set => _meetingId = value;
    }

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

    // 0: 조율중, 1: 조율완료
    public byte Status
    {
        get => _status;
        set
        {
            _status = value;
            OnPropertyChanged();
        }
    }

    public int TeamId
    {
        get => _teamId;
        set
        {
            _teamId = value;
            OnPropertyChanged();
        }
    }
    
    private void AddDateRow(object obj) => DateList.Add(new DateItem());

    private void RemoveDateRow(object obj)
    {
        if (DateList.Count > 1)
            DateList.RemoveAt(DateList.Count - 1);   
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}