using Com.Aurora.AuWeather.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Com.Aurora.Shared.Converters
{
    public class ConditionConverter : IValueConverter
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

    public class TempratureConverterWithoutDecoration : IValueConverter
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

    public class WindSpeedConverter : IValueConverter
    {
        public static WindParameter WindParameter { get; private set; } = WindParameter.BeaufortandText;
        public static SpeedParameter SpeedParameter { get; private set; } = SpeedParameter.KMPH;
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != default(Wind))
            {
                var wind = value as Wind;
                StringBuilder sb = new StringBuilder();
                switch (WindParameter)
                {
                    case WindParameter.BeaufortandText:
                    case WindParameter.BeaufortandDegree:
                        sb = SetBeaufort(wind.Scale, sb);
                        break;
                    case WindParameter.SpeedandText:
                    case WindParameter.SpeedandDegree:
                        sb = SetSpeed(wind.Speed, sb);
                        break;
                    default:
                        break;
                }
                return sb.ToString();
            }
            return null;
        }
        private StringBuilder SetSpeed(Speed speed, StringBuilder sb)
        {
            switch (SpeedParameter)
            {
                case SpeedParameter.KMPH:
                    sb.Append(speed.KMPH.ToString("0.0") + " km/h");
                    break;
                case SpeedParameter.MPS:
                    sb.Append(speed.MPS.ToString("0.0") + " m/s");
                    break;
                case SpeedParameter.Knot:
                    sb.Append(speed.Knot.ToString("0.0") + " kn");
                    break;
                default:
                    break;
            }
            return sb;
        }
        private StringBuilder SetBeaufort(WindScale scale, StringBuilder sb)
        {
            switch (scale)
            {
                case WindScale.unknown:
                    sb.Append("...");
                    break;
                case WindScale.zero:
                    sb.Append("无风");
                    break;
                case WindScale.one:
                    sb.Append("平静");
                    break;
                case WindScale.two:
                    sb.Append("微风");
                    break;
                case WindScale.three:
                    sb.Append("轻风");
                    break;
                case WindScale.four:
                    sb.Append("和风");
                    break;
                case WindScale.five:
                    sb.Append("清风");
                    break;
                case WindScale.six:
                    sb.Append("强风");
                    break;
                case WindScale.seven:
                    sb.Append("疾风");
                    break;
                case WindScale.eight:
                    sb.Append("大风");
                    break;
                case WindScale.nine:
                    sb.Append("烈风");
                    break;
                case WindScale.ten:
                    sb.Append("狂风");
                    break;
                case WindScale.eleven:
                    sb.Append("暴风");
                    break;
                case WindScale.twelve:
                    sb.Append("飓风");
                    break;
                case WindScale.thirteen:
                    sb.Append("台风");
                    break;
                case WindScale.fourteen:
                case WindScale.fifteen:
                    sb.Append("强台飓风");
                    break;
                case WindScale.sixteen:
                case WindScale.seventeen:
                    sb.Append("超强台飓风");
                    break;
                case WindScale.eighteen:
                    sb.Append("极强台飓风");
                    break;
                default:
                    break;
            }
            return sb;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public static void ChangeParameter(WindParameter windFormat, SpeedParameter speedFormat)
        {
            WindParameter = windFormat;
            SpeedParameter = speedFormat;
        }
    }

    public class WindDirectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != default(Wind))
            {
                var wind = value as Wind;
                StringBuilder sb = new StringBuilder();
                switch (WindSpeedConverter.WindParameter)
                {
                    case WindParameter.BeaufortandText:
                    case WindParameter.SpeedandText:
                        sb = SetText(wind.Direction, sb);
                        break;
                    case WindParameter.BeaufortandDegree:
                    case WindParameter.SpeedandDegree:
                        sb = SetDegree(wind.Degree, sb);
                        break;
                    default:
                        break;
                }
                return sb.ToString();
            }
            return null;
        }
        private StringBuilder SetDegree(uint degree, StringBuilder sb)
        {
            sb.Append(degree);
            sb.Append('°');
            return sb;
        }

        private StringBuilder SetText(WindDirection direction, StringBuilder sb)
        {
            switch (direction)
            {
                case WindDirection.unknown:
                    sb.Append("...");
                    break;
                case WindDirection.north:
                    sb.Append("北风");
                    break;
                case WindDirection.east:
                    sb.Append("东风");
                    break;
                case WindDirection.west:
                    sb.Append("西风");
                    break;
                case WindDirection.south:
                    sb.Append("南风");
                    break;
                case WindDirection.northeast:
                    sb.Append("东北风");
                    break;
                case WindDirection.northwest:
                    sb.Append("西北风");
                    break;
                case WindDirection.southeast:
                    sb.Append("东南风");
                    break;
                case WindDirection.southwest:
                    sb.Append("西南风");
                    break;
                default:
                    sb.Append("...");
                    break;
            }
            return sb;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class TemraturePathConverter : IValueConverter
    {
        private const float _factor = -64;
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is float)
            {
                Point? point = new Point(0, (float)value * _factor);
                return point;
            }
            return new Point(0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class TempraturePathEndConverter : IValueConverter
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

    public class HourMinuteConverter : IValueConverter
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

    public class PoptoThicknessConverter : IValueConverter
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

    public class ScrollViewerConverter : IValueConverter
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

    public class ConditiontoTextConverter : IValueConverter
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

    public class BodyTempratureAniConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return 64;
            float temp = ((Temprature)value).Celsius;
            temp = temp < -15 ? -15 : temp;
            temp = temp > 40 ? 40 : temp;
            temp += 15;
            temp /= 55;
            return 56 * (1 - temp);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class HumidityAniConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return 64;
            float temp = (uint)value;
            temp /= 100;
            return 56 * (1 - temp);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PcpnTransAniConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return 64;
            float temp = (float)value;
            temp = temp > 150 ? 150 : temp;
            temp /= 150;
            return 56 * (1 - temp);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PrecipitationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((float)value).ToString("0.#") + "mm";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SunRiseAniConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return new DoubleCollection { 98.96666666, 0, (double)value * 98.96666666, 1000 };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SunRiseAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? HorizontalAlignment.Left : HorizontalAlignment.Right;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SunSetAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SunRiseTextAlignMentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? -72 : 72;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SunSetTextAlignMentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? 72 : -72;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
