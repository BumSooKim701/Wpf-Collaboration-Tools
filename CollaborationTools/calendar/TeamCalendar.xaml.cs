using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CollaborationTools.calendar;

public partial class TeamCalendar : UserControl
{
    private readonly string _calendarId;
    private ObservableCollection<ScheduleItem> _oneDaySchedules;
    private ObservableCollection<ScheduleItem> _schedules;

    public TeamCalendar() : this("primary")
    {
    }

    public TeamCalendar(string calendarId = "primary")
    {
        InitializeComponent();
        _calendarId = calendarId;
        _schedules = new ObservableCollection<ScheduleItem>();
        _oneDaySchedules = new ObservableCollection<ScheduleItem>();
        _ = LoadScheduleItems();
    }

    // 다가오는 일정 화면에 불러오기
    private async Task LoadScheduleItems()
    {
        _schedules = await ScheduleService.GetScheduleItems(_calendarId);

        if (_schedules != null)
            CardListView.ItemsSource = _schedules;
        else
            NoScheduleMsg.Visibility = Visibility.Visible;
    }

    // 일정 상세보기
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
                
                scheduleDetailsWindow.ScheduleSaved += (s,args) =>
                {
                    LoadScheduleItems();
                };
                
                ShowDialog(scheduleDetailsWindow);
            }
        }
    }
    
    // 일정 생성
    private void RegisterButtonClicked(object sender, RoutedEventArgs e)
    {
        DateTime startDateTime = Calendar.SelectedDate.HasValue 
            ? Calendar.SelectedDate.Value 
            : DateTime.Now;
        var scheduleRegisterWindow = new ScheduleRegisterWindow(
            new ScheduleItem()
            {
                StartDateTime = startDateTime, 
                CalendarId = _calendarId
            });
        
        scheduleRegisterWindow.ScheduleSaved += (s,args) =>
        {
            LoadScheduleItems();
        };
        ShowDialog(scheduleRegisterWindow);
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
            var selectedDate = Calendar.SelectedDate.Value;
            _oneDaySchedules.Clear();
            _ = DisplayCalendarSchedule(selectedDate);
        }
    }

    private async Task DisplayCalendarSchedule(DateTime selectedDate) // 캘린더에서 선택된 날짜 일정
    {
        _oneDaySchedules = await ScheduleService.GetOneDayScheduleItems(_calendarId, selectedDate);

        if (_oneDaySchedules.Count > 0)
            CardListViewCalendar.ItemsSource = _oneDaySchedules;
        else
            NoScheduleMsgCalendar.Visibility = Visibility.Visible;
    }
    
}