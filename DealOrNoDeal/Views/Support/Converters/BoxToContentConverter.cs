using System;
using System.Globalization;
using System.Windows.Data;
using DealOrNoDeal.Models;

namespace DealOrNoDeal.Views.Support.Converters
{
    public class BoxToContentConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Box box = values[0] as Box;

            if (box == null)
                return "[ ? ]";

            return box.Revealed ? box.ToString() : box.Id.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
