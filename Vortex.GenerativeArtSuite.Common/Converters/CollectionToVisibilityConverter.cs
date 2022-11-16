using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Vortex.GenerativeArtSuite.Common.Converters
{
    public class CollectionToVisibilityConverter : IValueConverter
    {
        public Visibility OnNotEmpty { get; set; } = Visibility.Visible;

        public Visibility OnEmpty { get; set; } = Visibility.Collapsed;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is ICollection collection && collection.Count == 0 ? OnEmpty : OnNotEmpty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
