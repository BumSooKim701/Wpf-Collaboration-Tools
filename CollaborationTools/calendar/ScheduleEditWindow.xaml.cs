using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CollaborationTools.calendar;

public partial class ScheduleEditWindow : Window
{
    public event EventHandler ScheduleSaved;
    
    public ScheduleEditWindow(ScheduleItem scheduleItem)
    {
        InitializeComponent();
        DataContext = scheduleItem;
    }


    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (this.DataContext is ScheduleItem scheduleItem)
        {
            await ScheduleService.UpdateSchedule(scheduleItem.Event, scheduleItem.CalendarId);
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
    
    // 종료날짜를 시작날짜 이후로 선택 가능하도록 하는 함수
    private void StartDatePickerChanged(object sender, SelectionChangedEventArgs e)
    {
        if (StartDatePicker.SelectedDate.HasValue)
        {
            EndDatePicker.DisplayDateStart = StartDatePicker.SelectedDate.Value;
            
            if (EndDatePicker.SelectedDate.HasValue && EndDatePicker.SelectedDate.Value < StartDatePicker.SelectedDate.Value)
            {
                EndDatePicker.SelectedDate = null;
            }
        }
        else
        {
            // 시작 날짜가 해제되면 제한도 해제
            EndDatePicker.DisplayDateStart = null;
        }
    }
    
    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            this.DragMove();
        }
    }

    private void StartTimePickerChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
    {
        if (StartTimePicker.SelectedTime != null && EndTimePicker.SelectedTime != null)
        {
            if (EndTimePicker.SelectedTime < StartTimePicker.SelectedTime)
            {
                EndTimePicker.SelectedTime = StartTimePicker.SelectedTime;
            }
        }
    }
    
    private void EndTimePickerChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
    {
        if (StartTimePicker.SelectedTime != null && EndTimePicker.SelectedTime != null)
        {
            if (EndTimePicker.SelectedTime < StartTimePicker.SelectedTime)
            {
                StartTimePicker.SelectedTime = EndTimePicker.SelectedTime;
            }
        }
    }
}