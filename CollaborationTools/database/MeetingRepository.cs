using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using CollaborationTools.meeting_schedule;
using MySqlConnector;

namespace CollaborationTools.database;

public class MeetingRepository
{
    private readonly ConnectionPool _connectionPool;


    public MeetingRepository()
    {
        _connectionPool = ConnectionPool.GetInstance();
    }


    // 팀의 조율 중인 미팅 조회
    public Meeting GetMeeting(int teamId)
    {
        MySqlConnection connection = null;
        var hasData = false;
        var meetingPlan = new Meeting();

        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand(
                       "SELECT * FROM meeting_schedule WHERE team_id=@team_id", connection))
            {
                command.Parameters.AddWithValue("@team_id", teamId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        hasData = true;
                        meetingPlan.MeetingId = reader.GetInt32("id");
                        meetingPlan.Title = reader.GetString("title");
                        meetingPlan.ToDo = reader.GetString("todo");
                        meetingPlan.Status = reader.GetByte("status");
                        meetingPlan.TeamId = reader.GetInt32("team_id");
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching meeting plan: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        if (!hasData) return null;

        return meetingPlan;
    }

    // 조율할 미팅을 테이블에 삽입
    public bool CreateMeeting(Meeting meetingPlan)
    {
        MySqlConnection connection = null;
        var result = false;

        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand(
                       "INSERT INTO meeting_schedule (title, todo, status, team_id) " +
                       "VALUES (@title, @todo, @status, @teamId);" +
                       "SELECT ROW_COUNT() AS AffectedRows, LAST_INSERT_ID() AS NewId;",
                       connection))
            {
                command.Parameters.AddWithValue("@title", meetingPlan.Title);
                command.Parameters.AddWithValue("@todo", meetingPlan.ToDo);
                command.Parameters.AddWithValue("@status", meetingPlan.Status);
                command.Parameters.AddWithValue("@teamId", meetingPlan.TeamId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var rowsAffected = Convert.ToInt32(reader["AffectedRows"]);
                        var newId = Convert.ToInt32(reader["NewId"]);

                        if (rowsAffected > 0)
                        {
                            meetingPlan.MeetingId = newId;
                            result = true;
                            Debug.WriteLine("meeting_schedule 테이블에 삽입 성공");
                        }
                    }
                }
            }

            if (result)
            {
                result = false;
                
                var queryBuilder = new StringBuilder(
                    "INSERT INTO meeting_date (date, meeting_schedule_id) VALUES ");

                foreach (var dateItem in meetingPlan.DateList)
                {
                    queryBuilder.Append($"('{dateItem.Date.ToString("yyyy-MM-dd")}', {meetingPlan.MeetingId}),");
                }
                queryBuilder.Remove(queryBuilder.Length - 1, 1);

                Debug.WriteLine(queryBuilder.ToString());
                using (var command = new MySqlCommand(queryBuilder.ToString(), connection))
                {
                    var executeResult = command.ExecuteNonQuery();

                    if (executeResult == meetingPlan.DateList.Count)
                    {
                        result = true;
                        Debug.WriteLine("meeting_date 테이블에 삽입 성공");
                    }    
                }
            }
            
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error creating meeting: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return result;
    }

    public ObservableCollection<DateItem> GetMeetingDateItem(int meetingId)
    {
        MySqlConnection connection = null;
        var dateList = new ObservableCollection<DateItem>();

        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand(
                       "SELECT * FROM meeting_date WHERE meeting_schedule_id = @meetingId", connection))
            {
                command.Parameters.AddWithValue("@meetingId", meetingId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var dateTime = reader.GetDateTime("date");
                        dateList.Add(new DateItem(dateTime));
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching meeting plan: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        if (dateList.Count == 0) return null;

        return dateList;
    }

    // 조율할 미팅에 대한 개인 일정을 등록
    public bool RegisterPersonalSchedule(PersonalScheduleList personalSchedules)
    {
        MySqlConnection connection = null;
        var result = false;

        List<Schedule> schedules = personalSchedules.Schedules;
        int teamMemberId;
        
        try
        {
            connection = _connectionPool.GetConnection();
            
            using (var command = new MySqlCommand(
                       "SELECT id FROM team_member WHERE team_id = @teamId and user_id = @userId", connection))
            {
                command.Parameters.AddWithValue("@teamId", personalSchedules.TeamId);
                command.Parameters.AddWithValue("@userId", personalSchedules.UserId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        teamMemberId = reader.GetInt32("id");
                    }
                    else
                    {
                        throw new Exception("Team member id not found");   
                    }
                }
            }
            
            // 등록한 개인 일정이 있다면 전부 지운다.
            using (var command = new MySqlCommand("DELETE FROM personal_schedule WHERE team_member_id = @teamMemberId", connection))
            {
                command.Parameters.AddWithValue("@teamMemberId", teamMemberId);

                var executeResult = command.ExecuteNonQuery();
            }
            
            var queryBuilder = new StringBuilder(
                "INSERT INTO personal_schedule (date, starttime_unavailable, endtime_unavailable, team_member_id, meeting_schedule_id) VALUES ");

            foreach (var schedule in schedules)
            {
                queryBuilder.Append($"('{schedule.Date.ToString("yyyy-MM-dd")}', '{schedule.StartDateTime.ToString("HH':'mm':'ss''")}', '{schedule.EndDateTime.ToString("HH':'mm':'ss''")}', {teamMemberId}, {personalSchedules.MeetingScheduleId}),");
            }
            queryBuilder.Remove(queryBuilder.Length - 1, 1);

            Debug.WriteLine(queryBuilder.ToString());
            using (var command = new MySqlCommand(queryBuilder.ToString(), connection))
            {
                var executeResult = command.ExecuteNonQuery();

                if (executeResult == schedules.Count) result = true;    
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error creating meeting: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }

        return result;
    }

    // 조율 중인 팀 미팅에 대한 개인 일정 조회 
    public PersonalScheduleList getPersonalSchedule(int userId, int teamId)
    {
        MySqlConnection connection = null;
        var result = false;
        var personalSchedules = new PersonalScheduleList();
        
        int teamMemberId;

        try
        {
            connection = _connectionPool.GetConnection();

            using (var command = new MySqlCommand(
                       "SELECT id FROM team_member WHERE team_id = @teamId and user_id = @userId", connection))
            {
                command.Parameters.AddWithValue("@teamId", teamId);
                command.Parameters.AddWithValue("@userId", userId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        teamMemberId = reader.GetInt32("id");
                    }
                    else
                    {
                        throw new Exception("Team member id not found");
                    }
                }
            }
            using (var command = new MySqlCommand(
                       "SELECT * FROM personal_schedule WHERE team_member_id = @teamMemberId", connection))
            {
                command.Parameters.AddWithValue("@teamMemberId", teamMemberId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        personalSchedules.TeamId = teamId;
                        personalSchedules.UserId = userId;
                        personalSchedules.MeetingScheduleId = reader.GetInt32("meeting_schedule_id");
                        var date = reader.GetDateTime("date");
                        var startTime = reader.GetDateTime("starttime_unavailable");
                        var endTime = reader.GetDateTime("endtime_unavailable");
                        personalSchedules.AddSchedule(date, startTime, endTime);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching personal schedule: {e.Message}");
        }
        finally
        {
            if (connection != null) _connectionPool.ReleaseConnection(connection);
        }
        
        return personalSchedules;
    }

}