using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace wpcd.Converters {
    public class StarDependontOnFavoriteConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if((bool)value) {
                return new BitmapImage(new Uri("/Assets/filledbuttonstar.png", UriKind.Relative));
            } else {
                return new BitmapImage(new Uri("/Assets/buttonstar.png", UriKind.Relative));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return null;
        }
    }
}
