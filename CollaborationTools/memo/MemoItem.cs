using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CollaborationTools.memo;

public class MemoItem : INotifyPropertyChanged
{
    private string _content;
    private string _lastEditorName;
    private DateTime _lastModifiedDate;

    private string _title;

    public MemoItem()
    {
        _content = "";
    }

    public string Title
    {
        get => _title;
        set
        {
            if (_title != value)
            {
                _title = value;
                IsDirty = true;
                OnPropertyChanged();
            }
        }
    }

    public string Content
    {
        get => _content;
        set
        {
            if (_content != value)
            {
                _content = value;
                IsDirty = true;
                OnPropertyChanged();
            }
        }
    }


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

    public int EditorUserId { get; set; }

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

    public int MemoId { get; set; }

    public int TeamId { get; set; }

    public bool IsDirty { get; private set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void ResetDirtyFlag()
    {
        IsDirty = false;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}