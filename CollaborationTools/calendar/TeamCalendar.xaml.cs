using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CollaborationTools.authentication;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using MaterialDesignThemes.Wpf;
using Calendar = System.Globalization.Calendar;

namespace CollaborationTools.calendar;

public partial class TeamCalendar : UserControl
{
    private List<ScheduleItem> _schedules;
    private List<ScheduleItem> _oneDaySchedules;
    private string _calendarId;

    public TeamCalendar() : this("primary") { }
    public TeamCalendar(string calendarId = "primary")
    {
        InitializeComponent();
        _calendarId = calendarId;
        _schedules = new List<ScheduleItem>();
        _oneDaySchedules = new List<ScheduleItem>();
        _ = LoadScheduleItems();
    }

    private async Task LoadScheduleItems()
    {
        _schedules = await ScheduleService.GetScheduleItems(_calendarId);
        
        if(_schedules != null)
        {
            CardListView.ItemsSource = _schedules;
        }
        else
        {
            NoScheduleMsg.Visibility = Visibility.Visible;
        }
        
    }

    private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is ListView listView)
        {
            if (listView.SelectedItem is ScheduleItem scheduleItem)
            {
                var scheduleDetailsWindow = new ScheduleDetailsWindow
                {
                    DataContext = scheduleItem
                    
                };
                ShowDialog(scheduleDetailsWindow);
            }
        }
    }

    private void ShowDialog(Window window)
    {
        window.Owner = Application.Current.MainWindow;
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        window.ShowDialog();
    }

    private void CalendarDateChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Calendar.SelectedDate.HasValue)
        {
            DateTime selectedDate = Calendar.SelectedDate.Value;
            _oneDaySchedules.Clear();
            _ = DisplayCalendarSchedule(selectedDate);
        }
    }
    
    private async Task DisplayCalendarSchedule(DateTime selectedDate) // 캘린더에서 선택된 날짜 일정
    {
        _oneDaySchedules = await ScheduleService.GetOneDayScheduleItems(_calendarId, selectedDate);
        
        if(_oneDaySchedules != null)
        {
            CardListViewCalendar.ItemsSource = _oneDaySchedules;
        }
        else
        {
            NoScheduleMsgCalendar.Visibility = Visibility.Visible;
        }
    
    }
    
}