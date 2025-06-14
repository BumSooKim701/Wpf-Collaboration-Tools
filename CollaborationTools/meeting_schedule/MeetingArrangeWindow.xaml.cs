using System.Windows;
using System.Windows.Input;

namespace CollaborationTools.meeting_schedule;

public partial class MeetingArrangeWindow : Window
{
    private readonly Meeting _meetingPlan;


    public MeetingArrangeWindow(int teamId)
    {
        InitializeComponent();
        _meetingPlan = new Meeting
        {
            Status = 0,
            TeamId = teamId
        };
        DataContext = _meetingPlan;
    }

    private async void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        var meetingService = new MeetingService();

        var isSucceed = meetingService.CreateMeeting(_meetingPlan);

        MessageBox.Show(isSucceed ? "미팅 조율 등록에 성공하였습니다." : "미팅 조율 등록에 실패하였습니다.");
        if (isSucceed)
            Close();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left) DragMove();
    }
}