using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace bookquery.common
{
    public class BooleanToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) =>
            (bool)value ^ (parameter as string ?? string.Empty).Equals("Reverse") ?
                new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Gray);

        public object ConvertBack(object value, Type targetType, object parameter, string language) =>
            (SolidColorBrush)value == new SolidColorBrush(Colors.White) ^ (parameter as string ?? string.Empty).Equals("Reverse");

    }
}
