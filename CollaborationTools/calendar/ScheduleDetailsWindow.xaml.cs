using System.Windows;
using System.Windows.Input;

namespace CollaborationTools.calendar;

public partial class ScheduleDetailsWindow : Window
{
    public ScheduleDetailsWindow()
    {
        InitializeComponent();
    }

    private void EditButton_DoubleClick(object sender, RoutedEventArgs e)
    {
        var scheduleItem = DataContext as ScheduleItem;
        var clonedScheduleItem = scheduleItem.Clone();
        
        var scheduleEditWindow = new ScheduleEditWindow
        {
            DataContext = clonedScheduleItem
            // DataContext = scheduleItem
        };
        
        scheduleEditWindow.ScheduleSaved += (s, args) => {
            // ScheduleEditWindow에서 SaveButton_Click시 수행됨
            scheduleItem.Event = clonedScheduleItem.Event;
            scheduleItem.Title = clonedScheduleItem.Title;
            scheduleItem.StartDateTime = clonedScheduleItem.StartDateTime;
            scheduleItem.EndDateTime = clonedScheduleItem.EndDateTime;
            scheduleItem.Location = clonedScheduleItem.Location;
            scheduleItem.Description = clonedScheduleItem.Description;
        };
        
        ShowDialog(scheduleEditWindow);
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
    
    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            this.DragMove();
        }
    }
}