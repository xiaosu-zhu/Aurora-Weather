using com.aurora.auweather.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace com.aurora.auweather.Converters
{
    class TempratureConverter : IValueConverter
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
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
