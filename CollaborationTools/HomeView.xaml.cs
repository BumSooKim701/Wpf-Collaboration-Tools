using System.Windows.Controls;
using CollaborationTools.meeting_schedule;

namespace CollaborationTools;

public partial class HomeView : UserControl
{
    private Meeting _meeting;
    
    public HomeView()
    {
        InitializeComponent();
        _meeting = new Meeting()
        {
            Title = "윈프 종료 발표 회의",
            ToDo = "최종 보고서 작성\nppt 준비\n발표 내용 정하기\n시연 동영상",
        };
        DataContext = _meeting;
    }
}