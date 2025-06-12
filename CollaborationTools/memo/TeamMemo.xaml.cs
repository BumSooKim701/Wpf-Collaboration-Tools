using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CollaborationTools.calendar;
using CollaborationTools.team;

namespace CollaborationTools.memo;

public partial class TeamMemo : UserControl
{
    private ObservableCollection<MemoItem> _memoItems;
    private MemoService _memoService;
    public static readonly DependencyProperty CurrentTeamProperty =
        DependencyProperty.Register(
            nameof(CurrentTeam),
            typeof(Team),
            typeof(TeamCalendar),
            new PropertyMetadata(null, OnCurrentTeamChanged));
    public Team CurrentTeam
    {
        get => (Team)GetValue(CurrentTeamProperty);
        set => SetValue(CurrentTeamProperty, value);
    }
    
    public TeamMemo(int teamId)
    {
        InitializeComponent();
        _memoService = new();

        DataContext = _memoItems;
        ItemsControl.ItemsSource = _memoItems;
        LoadMemoItems();
    }

    private static void OnCurrentTeamChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TeamMemo control)
        {
            control.LoadMemoItems();
        }
    }
    private void LoadMemoItems()
    {
        _memoItems = _memoService.GetMemoItems(CurrentTeam.teamId);
    }
}