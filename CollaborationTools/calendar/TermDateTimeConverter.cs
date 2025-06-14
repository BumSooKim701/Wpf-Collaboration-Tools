using System.Globalization;
using System.Windows.Data;

namespace CollaborationTools.calendar;

public class TermDateTimeConverter : IMultiValueConverter
{
    private static readonly string[] DayOfWeekKor = { "일", "월", "화", "수", "목", "금", "토" };

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is not
            [DateTime start, DateTime end, bool isAllDayEvent, bool isOneDayEvent])
            return string.Empty;

        var startDateTimeStr = FormatDateTime(start, isAllDayEvent);
        var endDateTimeStr = FormatDateTime(end, isAllDayEvent);

        //  1: 시간 지정 안된 이벤트 or 하루 이상의 이벤트 / 2: 하루 이상 이면서, 시간 지정된 이벤트
        return isAllDayEvent || !isOneDayEvent
            ? $"{startDateTimeStr} ~ {endDateTimeStr}"
            : $"{startDateTimeStr} ~ {end.Hour:D2}:{end.Minute:D2}";
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    /// 날짜와 시간을 포맷팅
    private static string FormatDateTime(DateTime dateTime, bool isAllDayEvent)
    {
        var dayOfWeek = DayOfWeekKor[(int)dateTime.DayOfWeek];
        var datePart = $"{dateTime.Month}월 {dateTime.Day}일 ({dayOfWeek})";

        return isAllDayEvent ? datePart : $"{datePart} {dateTime.Hour:D2}:{dateTime.Minute:D2}";
    }
}