using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Vortex.GenerativeArtSuite.Common.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public Visibility OnTrue { get; set; } = Visibility.Visible;

        public Visibility OnFalse { get; set; } = Visibility.Collapsed;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool boolValue && boolValue ? OnTrue : OnFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
