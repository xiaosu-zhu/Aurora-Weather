using Com.Aurora.AuWeather.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace Com.Aurora.Shared.Converters
{
    class ConditionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is WeatherCondition)
            {
                var va = (WeatherCondition)value;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    class TempratureConverterWithoutDecoration : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Temprature)
            {
                switch ((string)parameter)
                {
                    default: return (value as Temprature).Celsius;
                    case "Celsius": return (value as Temprature).Celsius;
                    case "Fahrenheit": return (value as Temprature).Fahrenheit;
                    case "Kelvin": return (value as Temprature).Kelvin;
                }
            }
            return "X";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    class TempratureConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Temprature)
            {
                switch ((string)parameter)
                {
                    default: return (value as Temprature).Celsius + "°C";
                    case "Celsius": return (value as Temprature).Celsius + "°C";
                    case "Fahrenheit": return (value as Temprature).Fahrenheit + "°F";
                    case "Kelvin": return (value as Temprature).Kelvin + "K";
                }
            }
            return "X";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    class WindConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Wind)
            {
                return (value as Wind).Direction.ToString();
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    class TemraturePathConverter : IValueConverter
    {
        private const float _factor = -64;
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is float)
            {
                Nullable<Point> point = new Point(0, (float)value * _factor);
                return point;
            }
            return new Point(0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    class TempraturePathEndConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch ((string)parameter)
            {
                case "0": return new Point(0, (double)value);
                case "1": return new Point(((Size)value).Width, ((Size)value).Height);
                default: return new Point(0, 0);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
