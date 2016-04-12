using System;
using Windows.UI.Xaml.Data;

namespace Com.Aurora.Calculator.Converters
{
    internal class StringtoFloatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double)
            {
                return ((double)value).ToString("0.################################");
            }
            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null || (string)value == "")
            {
                return 0f;
            }
            return double.Parse((string)value);
        }
    }
}
