using System.Windows;
using System.Windows.Input;

namespace CollaborationTools.memo;

public partial class MemoDetailsWindow : Window
{
    
    public MemoDetailsWindow(MemoItem memoItem)
    {
        InitializeComponent();
        DataContext = memoItem;
    }

    private void EditButtonClicked(object sender, RoutedEventArgs e)
    {
        
    }

    private void ShowDialog(Window window)
    {
        window.Owner = Application.Current.MainWindow;
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        window.ShowDialog();
    }

    private async void DeleteButtonClicked(object sender, RoutedEventArgs e)
    {
        
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