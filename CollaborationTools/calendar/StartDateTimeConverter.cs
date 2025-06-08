using System.Globalization;
using System.Windows.Data;

namespace CollaborationTools.calendar;

public class StartDateTimeConverter : IMultiValueConverter
{
    private static readonly string[] DayOfWeekKor = { "일", "월", "화", "수", "목", "금", "토" };

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is not [DateTime dateTime, bool isAllDayEvent])
            return "";

        var dayOfWeek = DayOfWeekKor[(int)dateTime.DayOfWeek];
        return isAllDayEvent
            ? $"{dateTime.Month}월 {dateTime.Day}일 ({dayOfWeek})"
            : $"{dateTime.Month}월 {dateTime.Day}일 ({dayOfWeek}) {dateTime.Hour:D2}:{dateTime.Minute:D2}";
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}