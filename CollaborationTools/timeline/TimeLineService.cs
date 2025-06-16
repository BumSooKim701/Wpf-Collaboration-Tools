using System.Collections.ObjectModel;
using CollaborationTools.calendar;
using CollaborationTools.file;
using CollaborationTools.memo;
using CollaborationTools.team;
using CollaborationTools.user;

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
                await LoadScheduleItems(timelineItems, teamCalendarId, startDate, endDate);

                // 2. 파일 데이터 가져오기 (기존 FileService 활용)
                await LoadFileItems(timelineItems, teamCalendarId, startDate, endDate);

                // 3. 메모 데이터 가져오기 (구현 필요)
                await LoadMemoItems(timelineItems, teamCalendarId, startDate, endDate);

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
        
        public static async Task LoadScheduleItems(ObservableCollection<TimelineItem> timelineItems, 
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
                        Description = !string.IsNullOrEmpty(schedule.Description) 
                            ? schedule.Description 
                            : schedule.Location ?? "일정",
                        ItemType = TimelineItemType.Schedule,
                        TeamId = teamCalendarId,
                        OriginalItem = schedule,
                        CreatedBy = "팀원"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"일정 로드 오류: {ex.Message}");
            }
        }
        
        public static async Task LoadMemoItems(ObservableCollection<TimelineItem> timelineItems,
            string teamCalendarId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var memoService = new MemoService();
                var teamService = new TeamService();
                var currentTeam = teamService.FindTeamByCalId(teamCalendarId);
                
                ObservableCollection<MemoItem> memos = new();
                
                if (teamCalendarId == "primary")
                {
                    // 개인 메모 로드 (Primary Team 사용)
                    memos = await memoService.GetMemoItems(UserSession.CurrentUser.TeamId);
                    Console.WriteLine($"개인 메모 로드: {memos.Count}개");
                }
                else
                {
                    // 팀 메모 로드
                    memos = await memoService.GetMemoItems(currentTeam.teamId);
                    Console.WriteLine($"팀 메모 로드: {memos.Count}개");
                }
        
                foreach (var memo in memos.Where(m => 
                             m.LastModifiedDate >= startDate && m.LastModifiedDate <= endDate))
                {
                    timelineItems.Add(new TimelineItem
                    {
                        DateTime = memo.LastModifiedDate,
                        Title = memo.Title,
                        Description = memo.Content.Length > 100 
                            ? memo.Content.Substring(0, 100) + "..." 
                            : memo.Content,
                        ItemType = TimelineItemType.Memo,
                        TeamId = teamCalendarId,
                        OriginalItem = memo,
                        CreatedBy = memo.LastEditorName ?? "나"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"메모 로드 오류: {ex.Message}");
            }
        }
        
        public static async Task LoadFileItems(ObservableCollection<TimelineItem> timelineItems,
            string teamCalendarId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var folderService = new FolderService();
                var teamService = new TeamService();
                var currentTeam = teamService.FindTeamByCalId(teamCalendarId);
                
                var teamFolderId = currentTeam?.teamFolderId;

                if (teamCalendarId == "primary")
                    teamFolderId = "root";

                var files = await folderService.GetFilesInFolderAsync(teamFolderId);
                
                Console.WriteLine("f count: " + files.Count);
        
                foreach (var file in files.Where(f => 
                             f.ModifiedTime.HasValue && 
                             f.ModifiedTime.Value.Date >= startDate && 
                             f.ModifiedTime.Value.Date <= endDate))
                {
                    timelineItems.Add(new TimelineItem
                    {
                        DateTime = file.ModifiedTime?.Date ?? DateTime.Now,
                        Title = file.Name,
                        Description = $"파일이 업로드되었습니다. ({FormatFileSize(file.Size ?? 0)})",
                        ItemType = TimelineItemType.File,
                        TeamId = teamCalendarId,
                        OriginalItem = file,
                        CreatedBy = "팀원"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"파일 로드 오류: {ex.Message}");
            }
        }

// 파일 크기 포맷팅 (FileManagerWindow에서 가져온 로직)
        private static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            var order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
