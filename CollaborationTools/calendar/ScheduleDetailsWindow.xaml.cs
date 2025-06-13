using System.Windows;
using System.Windows.Input;

namespace CollaborationTools.calendar;

public partial class ScheduleDetailsWindow : Window
{
    public event EventHandler ScheduleSaved;
    
    public ScheduleDetailsWindow(ScheduleItem scheduleItem)
    {
        InitializeComponent();
        DataContext = scheduleItem;
    }

    private void EditButtonClicked(object sender, RoutedEventArgs e)
    {
        var scheduleItem = DataContext as ScheduleItem;
        var clonedScheduleItem = scheduleItem.Clone();
        
        var scheduleEditWindow = new ScheduleEditWindow(clonedScheduleItem);
        
        scheduleEditWindow.ScheduleSaved += (s, args) => {
            // ScheduleEditWindow에서 SaveButton_Click시 수행됨
            scheduleItem.Event = clonedScheduleItem.Event;
            scheduleItem.Title = clonedScheduleItem.Title;
            scheduleItem.StartDateTime = clonedScheduleItem.StartDateTime;
            scheduleItem.EndDateTime = clonedScheduleItem.EndDateTime;
            scheduleItem.Location = clonedScheduleItem.Location;
            scheduleItem.Description = clonedScheduleItem.Description;
            scheduleItem.IsAllDayEvent = clonedScheduleItem.IsAllDayEvent;
            scheduleItem.IsOneDayEvent = clonedScheduleItem.IsOneDayEvent;
            
            ScheduleSaved?.Invoke(this, EventArgs.Empty);
        };
        
        ShowDialog(scheduleEditWindow);
    }

    private void ShowDialog(Window window)
    {
        window.Owner = Application.Current.MainWindow;
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        window.ShowDialog();
    }

    private async void DeleteButtonClicked(object sender, RoutedEventArgs e)
    {
        var scheduleItem = DataContext as ScheduleItem;

        MessageBoxResult result = MessageBox.Show(
            "일정을 삭제하시겠습니까?", 
            "삭제 확인", 
            MessageBoxButton.YesNo, 
            MessageBoxImage.Question);
        
        if (result == MessageBoxResult.Yes)
        {
            await ScheduleService.DeleteSchedule(scheduleItem);
        
            ScheduleSaved?.Invoke(this, EventArgs.Empty);
            MessageBox.Show("일정이 삭제되었습니다.");
            Close();
        }
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