using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DpiUtil
{
    public class DpiScaleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue && parameter is string paramStr)
            {
                if (double.TryParse(paramStr, out double designValue))
                {
                    return designValue * DpiManager.DpiScaleX;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DpiThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string paramStr)
            {
                var parts = paramStr.Split(',');
                if (parts.Length == 4 &&
                    double.TryParse(parts[0], out double top) &&
                    double.TryParse(parts[1], out double right) &&
                    double.TryParse(parts[2], out double bottom) &&
                    double.TryParse(parts[3], out double left))
                {
                    return DpiManager.ScaleThickness(top, right, bottom, left);
                }
            }
            return new Thickness(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
