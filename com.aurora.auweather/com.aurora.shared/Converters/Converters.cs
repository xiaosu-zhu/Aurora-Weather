using Com.Aurora.Shared.Extensions;
using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Com.Aurora.Shared.Converters
{
    public class TimeSpanConverter : IValueConverter
    {
        private static string format = @"h\:mm";
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((TimeSpan)value).ToString(format);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class StringtoPathConverter : IValueConverter
    {
        // Parse the path-format string into a Geometry
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;
            string xamlPath =
                "<Geometry xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>"
                + (string)value + "</Geometry>";

            return Windows.UI.Xaml.Markup.XamlReader.Load(xamlPath) as Windows.UI.Xaml.Media.Geometry;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ColortoSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return new SolidColorBrush((Color)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class DateTimeConverter : IValueConverter
    {
        public static string Parameter { get; private set; } = "MM-dd";

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var dateTime = (DateTime)(value);
            return dateTime.ToString(Parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public static void ChangeParameter(string newPar)
        {
            Parameter = newPar;
        }
    }

    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int)
            {
                return value.ToString() + "%";
            }
            else if (value is uint)
            {
                return value.ToString() + "%";
            }
            else if (value is double)
            {
                return ((double)value).ToString("P");
            }
            else if (value is float)
            {
                return ((float)value).ToString("P");
            }
            else if (value is long)
            {
                return value.ToString() + "%";
            }
            else if (value is short)
            {
                return value.ToString() + "%";
            }
            else if (value is ushort)
            {
                return value.ToString() + "%";
            }
            else if (value is ulong)
            {
                return value.ToString() + "%";
            }

            else return value.ToString() + "%";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class EnumtoStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((Enum)value).GetDisplayName();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
