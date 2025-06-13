namespace CollaborationTools.meeting_schedule;

public class Meeting
{
    private string _title;
    private string _toDo;

    public string Title
    {
        get => _title;
        set => _title = value;
    }

    public string ToDo
    {
        get => _toDo;
        set => _toDo = value;
    }
}