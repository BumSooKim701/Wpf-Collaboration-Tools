using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CollaborationTools.memo;

public class MemoItem : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private string _title;
    private string _content;
    private DateTime _lastModifiedDate;
    private int _editorUserId;
    private string _lastEditorName;
    private int _memoId;
    private int _teamId;
    private bool _isDirty = false; // 변경 추적 플래그
    


    public string Title
    {
        get => _title;
        set
        {
            if (_title != value)
            {
                _title = value;
                _isDirty = true;
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
                _isDirty = true;
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

    public int EditorUserId
    {
        get => _editorUserId;
        set => _editorUserId = value;
    }

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

    public int MemoId
    {
        get => _memoId;
        set => _memoId = value;
    }
    
    public int TeamId
    {
        get => _teamId;
        set => _teamId = value;
    }

    public bool IsDirty
    {
        get => _isDirty;
    }

    public void ResetDirtyFlag()
    {
        _isDirty = false;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}