using System.Windows;

namespace CollaborationTools.calendar;

public partial class ScheduleEditWindow : Window
{
    public ScheduleEditWindow()
    {
        InitializeComponent();
    }


    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (this.DataContext is ScheduleItem scheduleItem)
        {
            await ScheduleService.UpdateScheduleItem(scheduleItem.Event, scheduleItem.CalendarId);
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
            scheduleItem.IsAllDayEventEnd = true;
        }
    }
    private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        StartTimePicker.IsEnabled = true;
        EndTimePicker.IsEnabled = true;
        if (this.DataContext is ScheduleItem scheduleItem)
        {
            scheduleItem.IsAllDayEventEnd = false;
        }
    }
}