using System.Collections.ObjectModel;
using CollaborationTools.database;

namespace CollaborationTools.memo;

public class MemoService
{
    private MemoRepository _memoRepository;

    public MemoService()
    {
        _memoRepository = new();
    }

    public ObservableCollection<MemoItem> GetMemoItems(int teamId)
    {
        return _memoRepository.GetMemosByTeamId(teamId);
    }

    public bool AddMemoItem(MemoItem memoItem)
    {
        return _memoRepository.AddMemo(memoItem);
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