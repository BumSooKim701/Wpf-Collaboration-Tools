using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CollaborationTools.team;
using CollaborationTools.user;

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
            LoadTimelineData();
            DataContext = viewModel;
            
            
        }

        public Team? CurrentTeam
        {
            get => (Team)GetValue(CurrentTeamProperty);
            set => SetValue(CurrentTeamProperty, value);
        }

        private static void OnCurrentTeamChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TimeLine timelineView)
            {
                var team = e.NewValue as Team;
                timelineView.viewModel.CurrentTeam = team;
        
                if (team != null)
                {
                    Console.WriteLine($"team mode: {team.teamName}");
                }
                else
                {
                    Console.WriteLine("personal mode: Primary Team");
                }
        
                try
                {
                    timelineView.LoadTimelineData();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"time line loading error: {ex.Message}");
                }
            }
        }

        private async Task LoadTimelineData()
        {
            try
            {
                var startDate = viewModel.StartDate;
                var endDate = viewModel.EndDate;
                string targetCalendarId;
                
                if (CurrentTeam == null)
                {
                    targetCalendarId = "primary";
                }
                else
                {
                    targetCalendarId = CurrentTeam.teamCalendarId;
                }
                
                Console.WriteLine("calId is " + targetCalendarId + "");
                
                var daysDiff = (endDate - startDate).Days;
                var maxItems = Math.Max(100, daysDiff * 5);
                
                var timelineItems = await TimelineService.GetTeamTimelineAsync(
                    targetCalendarId, startDate, endDate, maxItems);
        
                Console.WriteLine($"loaded timeline item count: {timelineItems.Count}");
        
                if (timelineItems.Count > 0)
                {
                    CalculateItemPositions(timelineItems);
                }
        
                viewModel.TimelineItems = timelineItems;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"timeline error: {ex.Message}");
                // 오류 발생 시 빈 컬렉션으로 설정
                viewModel.TimelineItems = new ObservableCollection<TimelineItem>();
            }
            
            Dispatcher.BeginInvoke(new Action(() => UpdateTimelineLayout()));
        }


        private void CalculateItemPositions(ObservableCollection<TimelineItem> items)
        {
            const double timelineY = 200;
            const double itemHeight = 80;
            const double itemWidth = 150;
            const double startX = 50;
            const double minItemSpacing = 15; // 간격 증가
            const double laneSpacing = 90; // 레인 간격

            if (!items.Any()) return;

            var minDate = items.Min(i => i.DateTime);
            var maxDate = items.Max(i => i.DateTime);
            var timeSpan = maxDate - minDate;
            var availableWidth = TimelineScrollViewer.ActualWidth > 0 ? 
                TimelineScrollViewer.ActualWidth - 100 : 1000;

            // 동적 타임라인 너비 계산
            var timelineWidth = Math.Max(availableWidth, items.Count * (itemWidth + minItemSpacing) / 3);

            // 6개 레인 관리 (위쪽 3개, 아래쪽 3개)
            var lanes = new List<List<(double X, double Width)>>();
            for (var i = 0; i < 6; i++) lanes.Add(new List<(double X, double Width)>());

            foreach (var item in items)
            {
                // X 좌표 계산
                double xPosition;
                if (timeSpan.TotalMinutes > 0)
                {
                    var timeRatio = (item.DateTime - minDate).TotalMinutes / timeSpan.TotalMinutes;
                    xPosition = startX + timelineWidth * timeRatio;
                }
                else
                {
                    xPosition = startX + timelineWidth / items.Count * items.IndexOf(item);
                }

                // 사용 가능한 레인 찾기
                var selectedLane = FindAvailableLane(lanes, xPosition, itemWidth, minItemSpacing);

                // Y 좌표 계산
                double yPosition;
                if (selectedLane < 3) // 위쪽 레인들
                    yPosition = timelineY - itemHeight - 20 - selectedLane * laneSpacing;
                else // 아래쪽 레인들
                    yPosition = timelineY + 20 + (selectedLane - 3) * laneSpacing;

                item.XPosition = xPosition;
                item.YPosition = yPosition;

                // 레인에 아이템 추가
                lanes[selectedLane].Add((xPosition, itemWidth));
            }

            // 캔버스 크기 조정
            TimelineCanvas.MinWidth = timelineWidth + startX + 100;
            TimelineCanvas.MinHeight = Math.Max(600, 400 + 3 * laneSpacing * 2);
            TimelineAxis.X2 = timelineWidth + startX;
        }

        private int FindAvailableLane(List<List<(double X, double Width)>> lanes,
            double xPosition, double itemWidth, double minSpacing)
        {
            for (var i = 0; i < lanes.Count; i++)
                if (!HasOverlap(lanes[i], xPosition, itemWidth, minSpacing))
                    return i;

            // 모든 레인이 차있으면 가장 적게 사용된 레인 선택
            return lanes.Select((lane, index) => new { Lane = lane, Index = index })
                .OrderBy(x => x.Lane.Count)
                .First().Index;
        }
        
        private bool HasOverlap(List<(double X, double Width)> laneItems, double newX, double newWidth, double minSpacing)
        {
            foreach (var (x, width) in laneItems)
            {
                // 새 아이템의 범위
                double newStart = newX;
                double newEnd = newX + newWidth;
        
                // 기존 아이템의 범위 (여백 포함)
                double existingStart = x - minSpacing;
                double existingEnd = x + width + minSpacing;
        
                // 겹침 검사
                if (newStart < existingEnd && newEnd > existingStart)
                {
                    return true;
                }
            }
            return false;
        }
        
        private void TimelineScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateTimelineLayout();
        }
        
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateTimelineLayout();
        }

        private void UpdateTimelineLayout()
        {
            if (TimelineScrollViewer == null || TimelineCanvas == null || TimelineAxis == null)
                return;

            // ScrollViewer의 실제 크기 가져오기
            var availableWidth = TimelineScrollViewer.ActualWidth;
            var availableHeight = TimelineScrollViewer.ActualHeight;

            if (availableWidth > 0 && availableHeight > 0)
            {
                // Canvas 크기를 ScrollViewer에 맞춤 (여백 고려)
                var canvasWidth = Math.Max(800, availableWidth - 20);
                var canvasHeight = Math.Max(400, availableHeight - 20);
        
                TimelineCanvas.MinWidth = canvasWidth;
                TimelineCanvas.MinHeight = canvasHeight;

                // 타임라인 축 길이를 동적으로 조정
                TimelineAxis.X2 = canvasWidth - 50;
        
                // 아이템 위치 재계산
                if (viewModel.TimelineItems?.Count > 0)
                {
                    CalculateItemPositions(viewModel.TimelineItems);
                }
            }
        }
        
        private void TimelineCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (viewModel.TimelineItems?.Any() == true)
            {
                CalculateItemPositions(viewModel.TimelineItems);
            }
        }


        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadTimelineData();
        }
    }
}
