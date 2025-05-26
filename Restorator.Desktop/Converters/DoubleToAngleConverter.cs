using System.Globalization;
using System.Windows.Data;

namespace Restorator.Desktop.Converters
{
    public class DoubleToAngleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double angle)
                return angle;

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
