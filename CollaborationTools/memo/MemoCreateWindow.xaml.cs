using System.Windows;
using System.Windows.Input;

namespace CollaborationTools.memo;

public partial class MemoCreateWindow : Window
{
    public event EventHandler<MemoItem> MemoCreated;
    private MemoItem _memoItem = new();
    
    public MemoCreateWindow()
    {
        InitializeComponent();
        DataContext = _memoItem;
    }

    private void ShowDialog(Window window)
    {
        window.Owner = Application.Current.MainWindow;
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        window.ShowDialog();
    }
    
    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        MemoCreated?.Invoke(this, _memoItem);
        this.Close();
    }
    
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
    
    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            this.DragMove();
        }
    }
}