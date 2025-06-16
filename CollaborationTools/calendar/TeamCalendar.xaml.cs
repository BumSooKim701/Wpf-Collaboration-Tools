using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CollaborationTools.team;

namespace CollaborationTools.calendar;

public partial class TeamCalendar : UserControl
{
    public static readonly DependencyProperty CurrentTeamProperty =
        DependencyProperty.Register(
            nameof(CurrentTeam),
            typeof(Team),
            typeof(TeamCalendar),
            new PropertyMetadata(null, OnCurrentTeamChanged));

    private readonly string _calendarId;
    private ObservableCollection<ScheduleItem> _oneDaySchedules;
    private ObservableCollection<ScheduleItem> _schedules;

    public TeamCalendar() : this("primary") { }

    public TeamCalendar(string calendarId = "primary")
    {
        InitializeComponent();
        _calendarId = calendarId;
        _schedules = new ObservableCollection<ScheduleItem>();
        _oneDaySchedules = new ObservableCollection<ScheduleItem>();
        _ = LoadScheduleItems();
    }

    public Team? CurrentTeam
    {
        get => (Team)GetValue(CurrentTeamProperty);
        set => SetValue(CurrentTeamProperty, value);
    }

    private static void OnCurrentTeamChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TeamCalendar control) control.LoadScheduleItems();
    }

    // 다가오는 일정 화면에 불러오기
    private async Task LoadScheduleItems()
    {
        if (_calendarId == "primary" && CurrentTeam == null)
        {
            Console.WriteLine("primary calendar");
            try
            {
                _schedules = await ScheduleService.GetScheduleItems("primary");
                if (_schedules != null && _schedules.Count > 0)
                {
                    CardListView.ItemsSource = _schedules;
                    NoScheduleMsg.Visibility = Visibility.Hidden;
                }
                else
                {
                    NoScheduleMsg.Visibility = Visibility.Visible;
                    CardListView.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"개인 캘린더 로드 오류: {ex.Message}");
                NoScheduleMsg.Visibility = Visibility.Visible;
                CardListView.ItemsSource = null;
            }

            return;
        }

        Console.WriteLine("team calendar" + CurrentTeam?.teamCalendarId);

        // 팀 캘린더 불러오기
        if (CurrentTeam?.teamCalendarId == null)
        {
            NoScheduleMsg.Visibility = Visibility.Visible;
            CardListView.ItemsSource = null;
            return;
        }

        if (CurrentTeam?.teamCalendarId == null)
        {
            NoScheduleMsg.Visibility = Visibility.Visible;
            CardListView.ItemsSource = null;
            return;
        }

        try
        {
            _schedules = await ScheduleService.GetScheduleItems(CurrentTeam.teamCalendarId);
            if (_schedules != null && _schedules.Count > 0)
            {
                CardListView.ItemsSource = _schedules;
                NoScheduleMsg.Visibility = Visibility.Hidden;
            }
            else
            {
                NoScheduleMsg.Visibility = Visibility.Visible;
                CardListView.ItemsSource = null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"스케줄 로드 오류: {ex.Message}");
            NoScheduleMsg.Visibility = Visibility.Visible;
            CardListView.ItemsSource = null;
        }
    }

    // 일정 상세보기
    private void ListView_MouseDown(object sender, MouseButtonEventArgs e)
    {
        var scheduleItem = ((FrameworkElement)e.OriginalSource).DataContext as ScheduleItem;

        if (scheduleItem != null)
        {
            var scheduleDetailsWindow = new ScheduleDetailsWindow(scheduleItem, CurrentTeam);

            scheduleDetailsWindow.ScheduleSaved += (s, args) => { LoadScheduleItems(); };

            ShowDialog(scheduleDetailsWindow);
        }
    }

    // 일정 생성
    private void RegisterButtonClicked(object sender, RoutedEventArgs e)
    {
        var startDateTime = Calendar.SelectedDate.HasValue
            ? Calendar.SelectedDate.Value
            : DateTime.Now;
        var scheduleRegisterWindow = new ScheduleRegisterWindow(
            new ScheduleItem
            {
                StartDateTime = startDateTime,
                CalendarId = CurrentTeam?.teamCalendarId ?? "primary"
            }, CurrentTeam);

        scheduleRegisterWindow.ScheduleSaved += (s, args) => { LoadScheduleItems(); };
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
        if (CurrentTeam?.teamCalendarId == null)
        {
            _oneDaySchedules = await ScheduleService.GetOneDayScheduleItems("primary", selectedDate);
            // NoScheduleMsgCalendar.Visibility = Visibility.Visible;
            CardListViewCalendar.ItemsSource = _oneDaySchedules;
            return;
        }

        try
        {
            _oneDaySchedules = await ScheduleService.GetOneDayScheduleItems(CurrentTeam.teamCalendarId, selectedDate);
            if (_oneDaySchedules.Count > 0)
            {
                CardListViewCalendar.ItemsSource = _oneDaySchedules;
                NoScheduleMsgCalendar.Visibility = Visibility.Hidden;
            }
            else
            {
                NoScheduleMsgCalendar.Visibility = Visibility.Visible;
                CardListViewCalendar.ItemsSource = null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"일별 스케줄 로드 오류: {ex.Message}");
            NoScheduleMsgCalendar.Visibility = Visibility.Visible;
            CardListViewCalendar.ItemsSource = null;
        }
    }

    private async void RefreshButton_Clicked(object sender, RoutedEventArgs e)
    {
        await LoadScheduleItems();
    }
}