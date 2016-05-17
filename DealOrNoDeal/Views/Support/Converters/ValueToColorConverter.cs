using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using DealOrNoDeal.Support.Constants;

namespace DealOrNoDeal.Views.Support.Converters
{
    public class ValueToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double price = (double) value;

            if (BoxPrices.BluePrices.Any(p => p == price))
            {
                return Brushes.RoyalBlue;
            }
            if (BoxPrices.RedPrices.Any(p => p == price))
            {
                return Brushes.DarkRed;
            }
            throw new ArgumentException("Unknown price");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
