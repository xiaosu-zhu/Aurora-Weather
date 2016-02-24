using Com.Aurora.AuWeather.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
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
                switch (TempratureConverter.Parameter)
                {
                    default: return (value as Temprature).Celsius + "°";
                    case 0: return (value as Temprature).Celsius + "°";
                    case 1: return (value as Temprature).Fahrenheit + "°";
                    case 2: return (value as Temprature).Kelvin;
                }
            }
            return "X";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class TempratureConverter : IValueConverter
    {
        public static int Parameter { get; private set; } = 0;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch (Parameter)
            {
                default: return "C";
                case 0: return "C";
                case 1: return "F";
                case 2: return "K";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public static void ChangeParameter(int newPar)
        {
            if (newPar < 3 && newPar > -1)
            {
                Parameter = newPar;
            }
        }
    }

    class WindConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //TODO
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

    class HourMinuteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //TODO
            if (value is DateTime)
            {
                return ((DateTime)value).ToString("HH:mm");
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    class PoptoThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is float)
            {
                return 2 + ((float)value) * 4;
            }
            return 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    class ScrollViewerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return 480 - (double)value < 112 ? 112 : 480 - (double)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    class ConditiontoTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var condition = (WeatherCondition)value;
            switch (condition)
            {
                case WeatherCondition.unknown:
                    return "...";
                case WeatherCondition.sunny:
                    return "晴";
                case WeatherCondition.cloudy:
                    return "多云";
                case WeatherCondition.few_clouds:
                    return "少云";
                case WeatherCondition.partly_cloudy:
                    return "大部多云";
                case WeatherCondition.overcast:
                    return "阴";
                case WeatherCondition.windy:
                    return "有风";
                case WeatherCondition.calm:
                    return "轻风";
                case WeatherCondition.light_breeze:
                    return "清风";
                case WeatherCondition.moderate:
                    return "和风";
                case WeatherCondition.fresh_breeze:
                    return "劲风";
                case WeatherCondition.strong_breeze:
                    return "强风";
                case WeatherCondition.high_wind:
                    return "疾风";
                case WeatherCondition.gale:
                    return "大风";
                case WeatherCondition.strong_gale:
                    return "烈风";
                case WeatherCondition.storm:
                    return "暴风";
                case WeatherCondition.violent_storm:
                    return "飓风";
                case WeatherCondition.hurricane:
                    return "台风";
                case WeatherCondition.tornado:
                    return "龙卷风";
                case WeatherCondition.tropical_storm:
                    return "热带风暴";
                case WeatherCondition.shower_rain:
                    return "阵雨";
                case WeatherCondition.heavy_shower_rain:
                    return "强阵雨";
                case WeatherCondition.thundershower:
                    return "雷阵雨";
                case WeatherCondition.heavy_thunderstorm:
                    return "雷暴";
                case WeatherCondition.hail:
                    return "冰雹";
                case WeatherCondition.light_rain:
                    return "小雨";
                case WeatherCondition.moderate_rain:
                    return "中雨";
                case WeatherCondition.heavy_rain:
                    return "大雨";
                case WeatherCondition.extreme_rain:
                    return "暴雨";
                case WeatherCondition.drizzle_rain:
                    return "毛毛雨";
                case WeatherCondition.storm_rain:
                    return "暴风雨";
                case WeatherCondition.heavy_storm_rain:
                    return "大暴雨";
                case WeatherCondition.severe_storm_rain:
                    return "严重降水";
                case WeatherCondition.freezing_rain:
                    return "冻雨";
                case WeatherCondition.light_snow:
                    return "小雪";
                case WeatherCondition.moderate_snow:
                    return "中雪";
                case WeatherCondition.heavy_snow:
                    return "大雪";
                case WeatherCondition.snowstorm:
                    return "暴风雪";
                case WeatherCondition.sleet:
                    return "雨夹雪";
                case WeatherCondition.rain_snow:
                    return "雨雪";
                case WeatherCondition.shower_snow:
                    return "阵雪";
                case WeatherCondition.snow_flurry:
                    return "短时小雪";
                case WeatherCondition.mist:
                    return "薄雾";
                case WeatherCondition.foggy:
                    return "雾";
                case WeatherCondition.haze:
                    return "霾";
                case WeatherCondition.sand:
                    return "沙尘";
                case WeatherCondition.dust:
                    return "扬尘";
                case WeatherCondition.volcanic_ash:
                    return "火山灰";
                case WeatherCondition.duststorm:
                    return "尘暴";
                case WeatherCondition.sandstorm:
                    return "沙尘暴";
                case WeatherCondition.hot:
                    return "变热";
                case WeatherCondition.cold:
                    return "变冷";
                default:
                    return "...";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    class BodyTempratureAniConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return 0;
            float temp = ((Temprature)value).Celsius;
            temp = temp < -15 ? -15 : temp;
            temp = temp > 45 ? 45 : temp;
            temp /= 60;
            return 56 * (1 - temp);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
