using System.Windows;
using System.Windows.Input;
using CollaborationTools.team;

namespace CollaborationTools.timeline
{
    public partial class FullTimeLineWindow : Window
    {
        public FullTimeLineWindow(Team team)
        {
            InitializeComponent();
            
            // 타임라인 뷰에 팀 설정
            MainTimelineView.CurrentTeam = team;
            
            // 제목 설정
            Title = $"{team?.teamName ?? "알 수 없는 팀"} 타임라인";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}