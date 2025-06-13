using System.Windows;
using System.Windows.Input;
using CollaborationTools.user;

namespace CollaborationTools.memo;

public partial class MemoDetailsWindow : Window
{
    private MemoItem _memoItem;
    private string _lastSavedTitle;
    private string _LastSavedContent;
    public event EventHandler<MemoItem> MemoUpdated;
    public event EventHandler<MemoItem> MemoDeleted;
    
    public MemoDetailsWindow(MemoItem memoItem)
    {
        InitializeComponent();
        _memoItem = memoItem;
        DataContext = memoItem;
        
        _lastSavedTitle = memoItem.Title;
        _LastSavedContent = memoItem.Content;
    }

    private void EditButtonClicked(object sender, RoutedEventArgs e)
    {
        _memoItem.ResetDirtyFlag();
        SwitchMode(true);
    }
    
    private async void DeleteButtonClicked(object sender, RoutedEventArgs e)
    {
        var memoService = new MemoService();
        bool isSucceed = memoService.DeleteMemoItem(_memoItem);
        
        MessageBox.Show(isSucceed ? "메모가 정상적으로 삭제되었습니다" : "삭제에 실패하였습니다");
        if (isSucceed) 
            MemoDeleted?.Invoke(this, _memoItem);;

        Close();
    }
    
    private void RevertButtonClicked(object sender, RoutedEventArgs e)
    {
        RevertChanged();
        SwitchMode(false);
    }

    // _memoItem이 ui와 데이터 바인딩 돼있으므로, 변경취소 시 수정하기 이전 값으로 돌려놓기.
    private void RevertChanged()
    {
        _memoItem.Title = _lastSavedTitle;
        _memoItem.Content = _LastSavedContent;
        _memoItem.ResetDirtyFlag();
    }
    
    private async void SaveButtonClicked(object sender, RoutedEventArgs e)
    {
        if (_memoItem.IsDirty)
        {
            _memoItem.LastModifiedDate = DateTime.Now;
            _memoItem.LastEditorName = UserSession.CurrentUser.Name;
            _memoItem.EditorUserId = UserSession.CurrentUser.userId;
            _memoItem.ResetDirtyFlag();
            
            _lastSavedTitle = _memoItem.Title;
            _LastSavedContent = _memoItem.Content;

            var memoService = new MemoService();
            bool isSucceed = memoService.UpdateMemoItem(_memoItem);

            MessageBox.Show(isSucceed ? "저장에 성공하였습니다" : "저장에 실패하였습니다");
            if (isSucceed)
                MemoUpdated?.Invoke(this, _memoItem);
        }
        else
        {
            MessageBox.Show("변경사항이 없습니다.");
        }
        
        SwitchMode(false);
    }

    private void SwitchMode(bool isEditMode)
    {
        TitleBox.IsReadOnly = !isEditMode;
        ContentBox.IsReadOnly = !isEditMode;

        if (isEditMode)
        {
            EditButton.Visibility = Visibility.Collapsed;
            DeleteButton.Visibility = Visibility.Collapsed;
            RevertButton.Visibility = Visibility.Visible;
            SaveButton.Visibility = Visibility.Visible;
        }
        else
        {
            EditButton.Visibility = Visibility.Visible;
            DeleteButton.Visibility = Visibility.Visible;
            RevertButton.Visibility = Visibility.Collapsed;
            SaveButton.Visibility = Visibility.Collapsed;
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        RevertChanged();
        
        Close();
    }
    
    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            this.DragMove();
        }
    }
}