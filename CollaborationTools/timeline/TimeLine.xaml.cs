using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CollaborationTools.team;

namespace CollaborationTools.timeline
{
    public partial class TimeLine : UserControl
    {
        public static readonly DependencyProperty CurrentTeamProperty =
            DependencyProperty.Register(nameof(CurrentTeam), typeof(Team), 
                typeof(TimeLine), new PropertyMetadata(null, OnCurrentTeamChanged));

        private readonly TimeLineViewModel viewModel;

        public TimeLine()
        {
            InitializeComponent();
            viewModel = new TimeLineViewModel();
            DataContext = viewModel;
        }

        public Team CurrentTeam
        {
            get => (Team)GetValue(CurrentTeamProperty);
            set => SetValue(CurrentTeamProperty, value);
        }

        private static void OnCurrentTeamChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TimeLine timelineView && e.NewValue is Team team)
            {
                timelineView.viewModel.CurrentTeam = team;
                timelineView.LoadTimelineData();
            }
        }

        private async Task LoadTimelineData()
        {
            if (CurrentTeam?.teamCalendarId == null)
            {
                Console.WriteLine("CurrentTeam 또는 teamCalendarId가 null입니다.");
                return;
            }

            try
            {
                Console.WriteLine($"타임라인 데이터 로딩 시작: {CurrentTeam.teamName}");
        
                var startDate = viewModel.StartDate;
                var endDate = viewModel.EndDate;
        
                var timelineItems = await TimelineService.GetTeamTimelineAsync(
                    CurrentTeam.teamCalendarId, startDate, endDate);
        
                Console.WriteLine($"로드된 타임라인 아이템 수: {timelineItems.Count}");
        
                // 위치 계산 (중요!)
                if (timelineItems.Count > 0)
                {
                    CalculateItemPositions(timelineItems);
                }
        
                // ViewModel에 데이터 설정
                viewModel.TimelineItems = timelineItems;
        
                Console.WriteLine("타임라인 데이터 로딩 완료");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"타임라인 로드 오류: {ex.Message}");
                MessageBox.Show($"타임라인 데이터 로드 실패: {ex.Message}");
            }
        }


        private void CalculateItemPositions(ObservableCollection<TimelineItem> items)
        {
            const double timelineY = 200; // 중앙 축 Y 좌표
            const double itemHeight = 80;
            const double timelineWidth = 700;
            const double startX = 50;

            if (!items.Any()) return;

            var minDate = items.Min(i => i.DateTime);
            var maxDate = items.Max(i => i.DateTime);
            var timeSpan = maxDate - minDate;

            bool isUpper = true;
    
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
        
                // X 좌표 계산 (시간 기준)
                double xPosition;
                if (timeSpan.TotalMinutes > 0)
                {
                    var timeRatio = (item.DateTime - minDate).TotalMinutes / timeSpan.TotalMinutes;
                    xPosition = startX + (timelineWidth * timeRatio);
                }
                else
                {
                    // 모든 아이템이 같은 시간인 경우 균등 배치
                    xPosition = startX + (timelineWidth / items.Count) * i;
                }

                // Y 좌표 계산 (위/아래 교대 배치)
                var yPosition = isUpper 
                    ? timelineY - itemHeight - 20 
                    : timelineY + 20;

                // TimelineItem에 위치 설정
                item.XPosition = xPosition;
                item.YPosition = yPosition;
        
                Console.WriteLine($"아이템 '{item.Title}' 위치: X={xPosition:F1}, Y={yPosition:F1}");
        
                isUpper = !isUpper;
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadTimelineData();
        }
    }
}
