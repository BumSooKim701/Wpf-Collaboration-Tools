﻿using System.Collections.ObjectModel;
using CollaborationTools.database;
using CollaborationTools.team;

namespace CollaborationTools.meeting_schedule;

public class MeetingService
{
    private readonly MeetingRepository _meetingRepository;


    public MeetingService()
    {
        _meetingRepository = new MeetingRepository();
    }


    public Meeting GetMeeting(Team team)
    {
        return _meetingRepository.GetMeeting(team.teamId);
    }

    public ObservableCollection<DateItem> GetMeetingDates(int meetingId)
    {
        return _meetingRepository.GetMeetingDateItem(meetingId);
    }

    public bool CreateMeeting(Meeting meetingPlan)
    {
        return _meetingRepository.CreateMeeting(meetingPlan);
    }

    public bool RegisterPersonalSchedule(PersonalScheduleList personalSchedules)
    {
        return _meetingRepository.RegisterPersonalSchedule(personalSchedules);
    }

    public (ObservableCollection<Schedule>, int) GetAllPersonalSchedule(int teamId)
    {
        return _meetingRepository.GetAllPersonalSchedule(teamId);
    }
    
    public PersonalScheduleList GetPersonalSchedule(int userId, int teamId)
    {
        return _meetingRepository.GetPersonalSchedule(userId, teamId);
    }
    
    public bool DeleteMeeting(int meetingId)
    {
        return _meetingRepository.DeleteMeeting(meetingId);
    }
}