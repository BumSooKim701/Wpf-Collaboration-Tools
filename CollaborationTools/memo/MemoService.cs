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
}