using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            typeof(TeamMemo),
            new PropertyMetadata(null, OnCurrentTeamChanged));
    
    public Team CurrentTeam
    {
        get => (Team)GetValue(CurrentTeamProperty);
        set => SetValue(CurrentTeamProperty, value);
    }
    
    public TeamMemo()
    {
        InitializeComponent();
        _memoService = new();

        DataContext = _memoItems;
        ItemsControl.ItemsSource = _memoItems;
    }
    
    private void MemoDoubleClicked(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 1) 
        {
            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement?.DataContext is MemoItem memoItem)
            {
                var memoDetailsWindow = new MemoDetailsWindow(memoItem);
                Show(memoDetailsWindow);
            }
        }
    }
    
    private void Show(Window window)
    {
        window.Owner = Application.Current.MainWindow;
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        window.Show();
    }
    
    private void CreateButtonClicked(object sender, RoutedEventArgs e)
    {
        var memoCreateWindow = new MemoCreateWindow();
        
        memoCreateWindow.MemoCreated += (s,memoItem) =>
        {
            _memoItems.Add(memoItem);
        };
        
        Show(memoCreateWindow);
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
        ItemsControl.ItemsSource = _memoItems;
    }
}