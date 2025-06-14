using System.Collections.ObjectModel;
using CollaborationTools.database;
using CollaborationTools.team;
using CollaborationTools.timeline;

namespace CollaborationTools.memo;

public class MemoService
{
    private readonly MemoRepository _memoRepository;

    public MemoService()
    {
        _memoRepository = new MemoRepository();
    }

    public async Task<ObservableCollection<MemoItem>> GetMemoItems(int teamId)
    {
        return await _memoRepository.GetMemosByTeamId(teamId);
    }

    public bool AddMemoItem(MemoItem memoItem)
    {
        return _memoRepository.AddMemo(memoItem);
    }

    public bool AddMemoItemForUser(MemoItem memoItem, int memberId)
    {
        return _memoRepository.AddMemoForUser(memoItem, memberId);
    }

    public bool UpdateMemoItem(MemoItem memoItem)
    {
        return _memoRepository.UpdateMemo(memoItem);
    }

    public bool DeleteMemoItem(MemoItem memoItem)
    {
        return _memoRepository.DeleteMemo(memoItem.MemoId);
    }

    public static async Task LoadMemoItems(ObservableCollection<TimelineItem> timelineItems,
        string teamId, DateTime startDate, DateTime endDate)
    {
        try
        {
            // MemoService 구현이 필요하므로 임시로 더미 데이터 사용
            // 실제로는 데이터베이스나 파일에서 메모 데이터를 조회해야 함
        
            var memoService = new MemoService(); // 구현 필요
            var teamService = new TeamService();
            var currentTeam = teamService.FindTeamByCalId(teamId);
            
            var memos = await memoService.GetMemoItems(currentTeam.teamId);
        
            foreach (var memo in memos)
            {
                timelineItems.Add(new TimelineItem
                {
                    DateTime = memo.LastModifiedDate,
                    Title = memo.Title,
                    Description = memo.Content.Length > 100 
                        ? memo.Content.Substring(0, 100) + "..." 
                        : memo.Content,
                    ItemType = TimelineItemType.Memo,
                    TeamId = teamId,
                    OriginalItem = memo,
                    CreatedBy = memo.LastEditorName ?? "팀원"
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"메모 로드 오류: {ex.Message}");
        }
    }
}