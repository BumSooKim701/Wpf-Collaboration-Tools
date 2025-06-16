using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace CollaborationTools.meeting_schedule
{
    public class ScheduleColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 3 || !(values[0] is DateTime date) || !(values[1] is string hourString) || !(values[2] is ObservableCollection<Schedule> allSchedules))
            {
                return Brushes.Transparent; // 기본값 (오류 또는 데이터 부족 시)
            }

            // 현재 셀의 시간 (시작) 파싱
            if (!TimeSpan.TryParse(hourString, out TimeSpan cellStartTime))
            {
                return Brushes.Transparent;
            }
            
            // 현재 셀의 시간 (끝, 다음 시간까지)
            TimeSpan cellEndTime = cellStartTime.Add(TimeSpan.FromHours(1));
            
            // 24:00 셀 처리
            if (cellStartTime.Hours == 24)
            {
                cellStartTime = new TimeSpan(23, 0, 0);
                cellEndTime = new TimeSpan(24, 0, 0);
            }

            // 현재 셀 시간에 스케줄이 있는지 확인
            bool hasScheduleInTimeSlot = false;
            
            foreach (var schedule in allSchedules)
            {
                TimeSpan scheduleStartTime = schedule.StartDateTime.TimeOfDay;
                TimeSpan scheduleEndTime = schedule.EndDateTime.TimeOfDay;
                
                // 일정이 날짜는 다르지만 시간 범위가 겹치는지 확인
                bool timeOverlap = IsTimeOverlapping(cellStartTime, cellEndTime, scheduleStartTime, scheduleEndTime);
                
                if (timeOverlap && schedule.Date.Date == date.Date)
                {
                    hasScheduleInTimeSlot = true;
                    break;
                }
            }
            
            // 스케줄이 있으면 투명색(Transparent), 없으면 연두색(LightGreen)
            return hasScheduleInTimeSlot ? Brushes.Transparent : Brushes.LightGreen;
        }

   
        private bool IsTimeOverlapping(TimeSpan cellStart, TimeSpan cellEnd, TimeSpan scheduleStart, TimeSpan scheduleEnd)
        {
            return (cellStart < scheduleEnd && cellEnd > scheduleStart);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}