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
}