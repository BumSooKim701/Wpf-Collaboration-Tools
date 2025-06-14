using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CollaborationTools.calendar;
using CollaborationTools.file;
using CollaborationTools.memo;
using CollaborationTools.team;

namespace CollaborationTools.timeline
{
    public static class TimelineService
    {
        public static async Task<ObservableCollection<TimelineItem>> GetTeamTimelineAsync(
            string teamCalendarId, DateTime startDate, DateTime endDate, int maxItems = 20)
        {
            var timelineItems = new ObservableCollection<TimelineItem>();
            
            try
            {
                // 1. 일정 데이터 가져오기 (기존 ScheduleService 활용)
                await ScheduleService.LoadScheduleItems(timelineItems, teamCalendarId, startDate, endDate);

                // 2. 파일 데이터 가져오기 (기존 FileService 활용)
                await FileService.LoadFileItems(timelineItems, teamCalendarId, startDate, endDate);

                // 3. 메모 데이터 가져오기 (구현 필요)
                await MemoService.LoadMemoItems(timelineItems, teamCalendarId, startDate, endDate);

                // 4. 시간순 정렬 및 개수 제한
                var sortedItems = timelineItems
                    .OrderByDescending(item => item.DateTime)
                    .Take(maxItems)
                    .ToList();

                timelineItems.Clear();
                foreach (var item in sortedItems)
                {
                    timelineItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Timeline 데이터 로드 오류: {ex.Message}");
            }

            return timelineItems;
        }
        
        private static async Task LoadScheduleItems(ObservableCollection<TimelineItem> timelineItems, 
            string teamCalendarId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var schedules = await ScheduleService.GetScheduleItems(teamCalendarId);
                foreach (var schedule in schedules.Where(s => 
                             s.StartDateTime >= startDate && s.StartDateTime <= endDate))
                {
                    timelineItems.Add(new TimelineItem
                    {
                        DateTime = schedule.StartDateTime,
                        Title = schedule.Title,
                        Description = schedule.Description ?? schedule.Location,
                        ItemType = TimelineItemType.Schedule,
                        TeamId = teamCalendarId,
                        OriginalItem = schedule,
                        CreatedBy = "팀원" // 실제로는 일정 생성자 정보
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"일정 로드 오류: {ex.Message}");
            }
        }

        public static ObservableCollection<TimelineItem> GroupItemsByPosition(
            ObservableCollection<TimelineItem> items)
        {
            var groupedItems = new ObservableCollection<TimelineItem>();
            bool isUpperPosition = true;

            foreach (var item in items)
            {
                // 교대로 위/아래 배치를 위한 속성 추가
                item.Description += $"|Position:{(isUpperPosition ? "Upper" : "Lower")}";
                groupedItems.Add(item);
                isUpperPosition = !isUpperPosition;
            }

            return groupedItems;
        }
        
        public static async Task<ObservableCollection<TimelineItem>> GetRecentTeamTimelineAsync(
            string teamId, int maxItems = 10)
        {
            var timelineItems = new ObservableCollection<TimelineItem>();
            var endDate = DateTime.Now;
            var startDate = endDate.AddDays(-30); // 최근 30일

            try
            {
                // 일정 데이터 (기존 ScheduleService 활용)
                var schedules = await ScheduleService.GetScheduleItems(teamId);
                foreach (var schedule in schedules.Where(s => 
                                 s.StartDateTime >= startDate && s.StartDateTime <= endDate)
                             .Take(maxItems / 3))
                {
                    timelineItems.Add(new TimelineItem
                    {
                        DateTime = schedule.StartDateTime,
                        Title = schedule.Title,
                        Description = schedule.Description,
                        ItemType = TimelineItemType.Schedule,
                        TeamId = teamId,
                        OriginalItem = schedule
                    });
                }

                // 파일과 메모 데이터도 유사하게 추가...
        
                // 시간순 정렬 후 최대 개수 제한
                var sortedItems = timelineItems.OrderByDescending(item => item.DateTime)
                    .Take(maxItems)
                    .ToList();
        
                timelineItems.Clear();
                foreach (var item in sortedItems)
                {
                    timelineItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Recent Timeline 데이터 로드 오류: {ex.Message}");
            }

            return timelineItems;
        }
    }
}
