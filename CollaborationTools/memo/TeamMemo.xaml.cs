using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CollaborationTools.calendar;
using CollaborationTools.team;
using CollaborationTools.user;

namespace CollaborationTools.memo;

public partial class TeamMemo : UserControl
{
    private ObservableCollection<MemoItem> _memoItems;
    private MemoService _memoService;
    private bool isPrimary = true;
    
    public static readonly DependencyProperty CurrentTeamProperty =
        DependencyProperty.Register(
            nameof(CurrentTeam),
            typeof(Team),
            typeof(TeamMemo),
            new PropertyMetadata(null, OnCurrentTeamChanged));
    
    public Team? CurrentTeam
    {
        get => (Team)GetValue(CurrentTeamProperty);
        set => SetValue(CurrentTeamProperty, value);
    }

    public bool IsPrimary
    {
        get => isPrimary;
        set
        {
            isPrimary = value;
        }
    }
    
    
    public TeamMemo()
    {
        InitializeComponent();
        _memoService = new();

        DataContext = _memoItems;
        ItemsControl.ItemsSource = _memoItems;
    }
    
    private void MemoClicked(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 1) 
        {
            var frameworkElement = sender as FrameworkElement;
            if (frameworkElement?.DataContext is MemoItem memoItem)
            {
                var memoDetailsWindow = new MemoDetailsWindow(memoItem);

                memoDetailsWindow.MemoUpdated += (s, memoItem) =>
                {
                    int index = _memoItems.IndexOf(
                        _memoItems.FirstOrDefault(item => item.MemoId == memoItem.MemoId));
                
                    if (index >= 0)
                    {
                        _memoItems.Move(index, 0);  // 항목을 첫 번째 위치로 이동
                    }
                };
                memoDetailsWindow.MemoDeleted += (s, memoItem) =>
                {
                    _memoItems.Remove(memoItem);
                };

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
        Console.WriteLine(isPrimary);
        var memoCreateWindow = new MemoCreateWindow();
        int id = 0;
        
        if (CurrentTeam != null)
            id = CurrentTeam.teamId;
        else
            id = 0;
        
        memoCreateWindow.MemoCreated += (s,memoItem) =>
        {
            memoItem.TeamId = id;
            memoItem.LastModifiedDate = DateTime.Now;
            memoItem.EditorUserId = UserSession.CurrentUser.userId;
            memoItem.LastEditorName = UserSession.CurrentUser.Name;

            bool isSucceed;
                isSucceed = _memoService.AddMemoItem(memoItem);

            if (isSucceed)
                _memoItems.Insert(0, memoItem);
            else
                MessageBox.Show("메모 생성 실패!\n 다시 시도해 주세요.");
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