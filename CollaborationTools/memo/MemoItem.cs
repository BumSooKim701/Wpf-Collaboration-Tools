using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CollaborationTools.memo;

public class MemoItem : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private int _memoId;

    public int MemoId
    {
        get => _memoId;
        set
        {
            if (_memoId != value)
            {
                _memoId = value;
                OnPropertyChanged();
            }
        }
    }
    
    private int _teamId;

    public int TeamId
    {
        get => _teamId;
        set
        {
            if (_teamId != value)
            {
                _teamId = value;
                OnPropertyChanged();
            }
        }
    }

    private string _title;

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
    
    private string _content;

    public string Content
    {
        get => _content;
        set
        {
            if (_content != value)
            {
                _content = value;
                OnPropertyChanged();
            }
        }
    }

    
    private DateTime _lastModifiedDate;

    public DateTime LastModifiedDate
    {
        get => _lastModifiedDate;
        set
        {
            if (_lastModifiedDate != value)
            {
                _lastModifiedDate = value;
                OnPropertyChanged();
            }
        }
    }

    private string _lastEditorName;

    public string LastEditorName
    {
        get => _lastEditorName;
        set
        {
            if (_lastEditorName != value)
            {
                _lastEditorName = value;
                OnPropertyChanged();
            }
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}