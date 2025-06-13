using System.Windows;
using System.Windows.Input;
using CollaborationTools.user;

namespace CollaborationTools.memo;

public partial class MemoDetailsWindow : Window
{
    private MemoItem _memoItem;
    private string _lastSavedTitle;
    private string _LastSavedContent;
    
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
        SwitchButton(true);
    }
    
    private async void DeleteButtonClicked(object sender, RoutedEventArgs e)
    {
        
    }
    
    private async void RevertButtonClicked(object sender, RoutedEventArgs e)
    {
        // _memoItem이 ui와 데이터 바인딩 돼있으므로, 변경취소 시 수정하기 이전 값으로 돌려놓기.
        _memoItem.Title = _lastSavedTitle;
        _memoItem.Content = _LastSavedContent;
        SwitchButton(false);
    }
    
    private async void SaveButtonClicked(object sender, RoutedEventArgs e)
    {
        _memoItem.LastModifiedDate = DateTime.Now;
        _memoItem.LastEditorName = UserSession.CurrentUser.Name;
        
        _lastSavedTitle = _memoItem.Title;
        _LastSavedContent = _memoItem.Content;
        
        SwitchButton(false);
    }

    private void SwitchButton(bool isEditMode)
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
    
    private void ShowDialog(Window window)
    {
        window.Owner = Application.Current.MainWindow;
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        window.ShowDialog();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
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