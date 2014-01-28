using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace wpcd.Converters {
    public class BrushDependentOnUnreadConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            var iv = (bool)value;
            if(iv) {
                return new SolidColorBrush(Colors.Blue) { Opacity = .5 };
            } else {
                return new SolidColorBrush(Colors.Green) { Opacity = .5 };
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
