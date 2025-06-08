using System.Windows;

namespace CollaborationTools.calendar;

public partial class ScheduleDetailsWindow : Window
{
    public ScheduleDetailsWindow()
    {
        InitializeComponent();
    }

    private void EditButton_DoubleClick(object sender, RoutedEventArgs e)
    {
        var scheduleEditWindow = new ScheduleEditWindow
        {
            DataContext = DataContext as ScheduleItem
        };
        ShowDialog(scheduleEditWindow);
        Close();
    }

    private void ShowDialog(Window window)
    {
        window.Owner = Application.Current.MainWindow;
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        window.ShowDialog();
    }

    private void DeleteButton_DoubleClick(object sender, RoutedEventArgs e)
    {
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}