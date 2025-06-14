using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CollaborationTools.team;
using CollaborationTools.user;

namespace CollaborationTools.memo;

public partial class TeamMemo : UserControl
{
    public static readonly DependencyProperty CurrentTeamProperty =
        DependencyProperty.Register(
            nameof(CurrentTeam),
            typeof(Team),
            typeof(TeamMemo),
            new PropertyMetadata(null, OnCurrentTeamChanged));

    private readonly ObservableCollection<MemoItem> _memoItems;
    private readonly MemoService _memoService;
    private readonly TeamService _teamService = new();


    public TeamMemo()
    {
        InitializeComponent();
        _memoService = new MemoService();
        _memoItems = new ObservableCollection<MemoItem>();

        ItemsControl.ItemsSource = _memoItems;
        DataContext = this;
        LoadMemoItems();
    }

    public Team? CurrentTeam
    {
        get => (Team)GetValue(CurrentTeamProperty);
        set => SetValue(CurrentTeamProperty, value);
    }

    public bool IsPrimary { get; set; } = true;

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
                    var index = _memoItems.IndexOf(
                        _memoItems.FirstOrDefault(item => item.MemoId == memoItem.MemoId));

                    if (index >= 0) _memoItems.Move(index, 0); // 항목을 첫 번째 위치로 이동
                };
                memoDetailsWindow.MemoDeleted += (s, memoItem) => { _memoItems.Remove(memoItem); };

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

        if (CurrentTeam != null)
        {
            memoCreateWindow.MemoCreated += (s, memoItem) =>
            {
                memoItem.TeamId = CurrentTeam.teamId;
                memoItem.LastModifiedDate = DateTime.Now;
                memoItem.EditorUserId = UserSession.CurrentUser.userId;
                memoItem.LastEditorName = UserSession.CurrentUser.Name;

                var isSucceed = _memoService.AddMemoItem(memoItem);

                if (isSucceed)
                    _memoItems.Insert(0, memoItem);
                else
                    MessageBox.Show(Application.Current.MainWindow, "메모 생성 실패!\n 다시 시도해 주세요.");
            };
        }
        else
        {
            var primaryTeam = _teamService.FindUserPrimaryTeam(UserSession.CurrentUser);
            var member = _teamService.FindTeamMember(primaryTeam.teamId, UserSession.CurrentUser.userId);

            Console.WriteLine(primaryTeam.teamId + " " + member.memberId + " " + UserSession.CurrentUser.userId);

            memoCreateWindow.MemoCreated += (s, memoItem) =>
            {
                memoItem.TeamId = primaryTeam.teamId;
                memoItem.LastModifiedDate = DateTime.Now;
                memoItem.EditorUserId = UserSession.CurrentUser.userId;
                memoItem.LastEditorName = UserSession.CurrentUser.Name;

                var isSucceed = _memoService.AddMemoItemForUser(memoItem, member.memberId);

                if (isSucceed)
                {
                    _memoItems.Insert(0, memoItem);
                    MessageBox.Show(Application.Current.MainWindow, "메모 생성 성공!");
                }
                else
                {
                    MessageBox.Show(Application.Current.MainWindow, "메모 생성 실패!\n 다시 시도해 주세요.");
                }
            };
        }

        Show(memoCreateWindow);
    }

    private static void OnCurrentTeamChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TeamMemo control) control.LoadMemoItems();
    }

    private void LoadMemoItems()
    {
        if (CurrentTeam == null && IsPrimary)
        {
            var newItems = _memoService.GetMemoItems(UserSession.CurrentUser.TeamId);
            _memoItems.Clear();
            foreach (var item in newItems) _memoItems.Add(item);
        }
        else
        {
            var newItems = _memoService.GetMemoItems(CurrentTeam.teamId);
            _memoItems.Clear();
            foreach (var item in newItems) _memoItems.Add(item);
        }
    }
}