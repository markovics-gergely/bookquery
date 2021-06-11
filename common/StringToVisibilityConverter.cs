using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace bookquery.common
{
    public class StringToVisibilityConverter : IValueConverter
    {
        /*public object Convert(object value, Type targetType, object parameter, string language) =>
            ((string)value != string.Empty && (string)value != "") ^ (parameter as string ?? string.Empty).Equals("Reverse") ?
                Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, string language) =>
            (Visibility)value == Visibility.Visible ^ (parameter as string ?? string.Empty).Equals("Reverse") ? "NotEmpty" : "";*/
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return string.IsNullOrEmpty(value?.ToString()) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (Visibility)value == Visibility.Visible ^ 
                (parameter as string ?? string.Empty).Equals("Reverse") ? "NotEmpty" : "";
        }

    }
}
