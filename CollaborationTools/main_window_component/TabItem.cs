using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CollaborationTools.team;

namespace CollaborationTools;

public class TabItem : INotifyPropertyChanged
{
    private Team? _curTeam;
    private string? _header;
    private string _iconKind;
    private ObservableCollection<MenuItem> _menuItems;
    private string? _title;
    private string? _type;

    public string? Header
    {
        get => _header;
        set
        {
            _header = value;
            OnPropertyChanged();
        }
    }

    public string IconKind
    {
        get => _iconKind;
        set
        {
            _iconKind = value;
            OnPropertyChanged();
        }
    }

    public string? Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }

    public string? Type
    {
        get => _type;
        set
        {
            _type = value;
            OnPropertyChanged();
        }
    }

    public Team? CurTeam
    {
        get => _curTeam;
        set
        {
            _curTeam = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<MenuItem> MenuItems
    {
        get => _menuItems;
        set
        {
            _menuItems = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}