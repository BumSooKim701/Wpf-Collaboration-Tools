using System.Windows;
using System.Windows.Input;

namespace CollaborationTools.meeting_schedule
{
    public partial class MeetingAvailableTimeWindow : Window
    {
        private AvailableTimeViewModel _viewModel;
        
        public MeetingAvailableTimeWindow(int teamId)
        {
            InitializeComponent();
            _viewModel = new AvailableTimeViewModel(teamId);
            DataContext = _viewModel;
        }
        
        // 창 드래그 이동 기능 추가
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
        
        // 추가적인 메서드: 팀 ID로 데이터 로드
        public void LoadData(int teamId, int meetingId)
        {
            
        }
    }
}