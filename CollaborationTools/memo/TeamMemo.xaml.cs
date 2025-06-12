using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace CollaborationTools.memo;

public partial class TeamMemo : UserControl
{
    private ObservableCollection<MemoItem> _memoItems;
    private int _teamId;
    private MemoService _memoService;

    
    public TeamMemo(int teamId)
    {
        InitializeComponent();
        _teamId = teamId;
        _memoService = new();

        DataContext = _memoItems;
        ItemsControl.ItemsSource = _memoItems;
        LoadMemoItems();
    }

    private void LoadMemoItems()
    {
        _memoItems = _memoService.GetMemoItems(_teamId);
    }
}