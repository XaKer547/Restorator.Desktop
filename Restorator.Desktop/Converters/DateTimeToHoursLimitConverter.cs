using System.Globalization;
using System.Windows.Data;

namespace Restorator.Desktop.Converters
{
    public class DateTimeToHoursLimitConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is DateTime beginLimit && values[1] is DateTime endLimit)
            {
                if (parameter is int hour)
                    beginLimit = beginLimit.AddHours(hour);

                var count = endLimit.AddHours(-beginLimit.Hour).Hour;

                var requiredRange = Enumerable.Range(beginLimit.Hour, count);

                if (beginLimit.Day != endLimit.Day)
                {
                    var length = endLimit.Hour == 0 ? 1 : endLimit.Hour;

                    var rangeToAppend = Enumerable.Range(0, length);

                    var range = requiredRange.ToList();

                    range.AddRange(rangeToAppend);

                    return range;
                }

                return requiredRange;
            }

            return Enumerable.Range(0, 23);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
