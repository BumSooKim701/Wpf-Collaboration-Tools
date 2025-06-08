using System.Windows;

namespace CollaborationTools.calendar;

public partial class ScheduleEditWindow : Window
{
    public ScheduleEditWindow()
    {
        InitializeComponent();
    }


    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        StartTimePicker.IsEnabled = false;
        EndTimePicker.IsEnabled = false;
        if (DataContext is ScheduleItem scheduleItem) scheduleItem.IsAllDayEventEnd = true;
    }

    private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        StartTimePicker.IsEnabled = true;
        EndTimePicker.IsEnabled = true;
        if (DataContext is ScheduleItem scheduleItem) scheduleItem.IsAllDayEventEnd = false;
    }
}