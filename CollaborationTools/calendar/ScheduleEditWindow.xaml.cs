using System.Windows;
using System.Windows.Input;

namespace CollaborationTools.calendar;

public partial class ScheduleEditWindow : Window
{
    public event EventHandler ScheduleSaved;
    
    public ScheduleEditWindow()
    {
        InitializeComponent();
    }


    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (this.DataContext is ScheduleItem scheduleItem)
        {
            await ScheduleService.UpdateScheduleItem(scheduleItem.Event, scheduleItem.CalendarId);
            ScheduleSaved?.Invoke(this, EventArgs.Empty);
            this.Close();
        }
    }
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        StartTimePicker.IsEnabled = false;
        EndTimePicker.IsEnabled = false;
        if (this.DataContext is ScheduleItem scheduleItem)
        {
            scheduleItem.IsAllDayEvent = true;
        }
    }
    private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        StartTimePicker.IsEnabled = true;
        EndTimePicker.IsEnabled = true;
        if (this.DataContext is ScheduleItem scheduleItem)
        {
            scheduleItem.IsAllDayEvent = false;
        }
    }
    
    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            this.DragMove();
        }
    }
}