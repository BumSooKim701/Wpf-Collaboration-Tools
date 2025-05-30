using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace CollaborationTools;

public partial class TeamCalendar : UserControl
{
    private ObservableCollection<ScheduleItem> schedules = new ObservableCollection<ScheduleItem>();
    public TeamCalendar()
    {
        InitializeComponent();
        
        LoadScheduleItems();
    }

    private void LoadScheduleItems()
    {
        schedules.Add(new ScheduleItem { scheduleTitle = "미팅", scheduleDate = "2024-03-20" });
        schedules.Add(new ScheduleItem { scheduleTitle = "프로젝트 마감", scheduleDate ="2024-03-21" });
        schedules.Add(new ScheduleItem { scheduleTitle = "휴가", scheduleDate = "2024-03-22" });

        CardListView.ItemsSource = schedules;
    }
}