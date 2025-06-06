using System;
using System.Globalization;
using System.Windows.Data;

public class TermDateTimeConverter : IMultiValueConverter
{
    private static readonly string[] DayOfWeekKor = { "일", "월", "화", "수", "목", "금", "토" };

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is not [DateTime startDateTime, DateTime endDateTime, bool isAllDayEventStart, bool isAllDayEventEnd, bool isOneDayEvent])
            return "";

        var startDayOfWeek = DayOfWeekKor[(int)startDateTime.DayOfWeek];
        var startDateTimeStr = isAllDayEventStart ? 
            $"{startDateTime.Month}월 {startDateTime.Day}일 ({startDayOfWeek})"
            : $"{startDateTime.Month}월 {startDateTime.Day}일 ({startDayOfWeek}) {startDateTime.Hour:D2}:{startDateTime.Minute:D2}";
        
        var endDayOfWeek = DayOfWeekKor[(int)endDateTime.DayOfWeek];
        var endDateTimeStr = isAllDayEventEnd ? 
            $"{endDateTime.Month}월 {endDateTime.Day}일 ({endDayOfWeek})"
            : $"{endDateTime.Month}월 {endDateTime.Day}일 ({endDayOfWeek}) {endDateTime.Hour:D2}:{endDateTime.Minute:D2}";
        
        return isOneDayEvent ? 
            $"{startDateTimeStr} ~ {endDateTime.Hour:D2}:{endDateTime.Minute:D2}"
            : $"{startDateTimeStr} ~ {endDateTimeStr}";
        
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}