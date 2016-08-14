// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.LunarCalendar;
using Com.Aurora.AuWeather.Models;
using Com.Aurora.AuWeather.Models.HeWeather;
using Com.Aurora.AuWeather.Models.Settings;
using Com.Aurora.Shared.Extensions;
using Com.Aurora.Shared.Helpers;
using System;
using System.Text;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Com.Aurora.Shared.Converters
{

    public class TempratureConverterWithDegree : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "...";
            }
            var t = (Temperature)value;
            switch (TempratureandDegreeConverter.Parameter)
            {
                case TemperatureParameter.Celsius: return t.Celsius.ToString("0") + '°';
                case TemperatureParameter.Fahrenheit: return t.Fahrenheit.ToString("0") + '°';
                case TemperatureParameter.Kelvin: return t.Kelvin.ToString("0");
                default: return t.Celsius.ToString("0") + '°';
            }
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
            if (value == null)
            {
                return "...";
            }
            var t = (Temperature)value;
            switch (TempratureandDegreeConverter.Parameter)
            {
                case TemperatureParameter.Celsius: return t.Celsius.ToString("0");
                case TemperatureParameter.Fahrenheit: return t.Fahrenheit.ToString("0");
                case TemperatureParameter.Kelvin: return t.Kelvin.ToString("0");
                default: return t.Celsius.ToString("0");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }


    public class TempratureandDegreeConverter : IValueConverter
    {
        public static TemperatureParameter Parameter { get; private set; } = TemperatureParameter.Celsius;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            switch (Parameter)
            {
                case TemperatureParameter.Celsius:
                    return "\u2103";
                case TemperatureParameter.Fahrenheit:
                    return "\u2109";
                case TemperatureParameter.Kelvin:
                    return " K";
                default:
                    return "\u2103";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public static void ChangeParameter(TemperatureParameter newPar)
        {
            Parameter = newPar;
        }
    }

    public class WindSpeedConverter : IValueConverter
    {
        public static WindParameter WindParameter { get; private set; } = WindParameter.BeaufortandText;
        public static SpeedParameter SpeedParameter { get; private set; } = SpeedParameter.KMPH;
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return null;
            }
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
            sb.Append(scale.GetDisplayName());
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
            if (value == null)
            {
                return null;
            }
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
            sb.Append(direction.GetDisplayName());
            return sb;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class TemraturePathConverter : IValueConverter
    {
        private const float _factor = -32;
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var p = int.Parse((string)parameter);
            p *= 10;
            if (value == null)
            {
                return new Point(p, 0);
            }
            if (value is float)
            {
                return new Point(p, (float)value * _factor);
            }
            return new Point(p, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class HourMinuteConverter : IValueConverter
    {
        private static string Parameter = "HH:mm";
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return null;
            }
            if (value is DateTime)
            {
                return ((DateTime)value).ToString(Parameter);
            }
            if (Parameter == null || Parameter.Length == 0)
            {
                return "";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public static void ChangeParameter(string parameter)
        {
            Parameter = parameter;
        }
    }

    public class PoptoThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return 2d;
            }
            if (value is float)
            {
                return (double)(2 + ((float)value) * 4);
            }
            return 2d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ScrollViewerConverter : IValueConverter
    {
        public static double WeatherCanvasHeight = 640d;
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return WeatherCanvasHeight;
            }
            return WeatherCanvasHeight - (double)value < 160 ? 160d : WeatherCanvasHeight - (double)value;
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
            if (value == null)
            {
                return "...";
            }
            var condition = (WeatherCondition)value;
            return condition.GetDisplayName();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ConditiontoImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return null;
            }
            var condition = (WeatherCondition)value;
            var isNight = (bool)parameter;
            switch (condition)
            {
                case WeatherCondition.unknown:
                    return null;
                case WeatherCondition.sunny:
                    return isNight ? "Moon.png" : "Sun.png";
                case WeatherCondition.cloudy:
                case WeatherCondition.few_clouds:
                case WeatherCondition.partly_cloudy:
                    return isNight ? "Moon Cloud.png" : "Sun Cloud.png";
                case WeatherCondition.overcast:
                    return "Cloud.png";
                case WeatherCondition.windy:
                case WeatherCondition.calm:
                case WeatherCondition.light_breeze:
                    return isNight ? "Moon.png" : "Sun.png";
                case WeatherCondition.moderate:
                case WeatherCondition.fresh_breeze:
                case WeatherCondition.strong_breeze:
                case WeatherCondition.high_wind:
                case WeatherCondition.gale:
                case WeatherCondition.strong_gale:
                case WeatherCondition.storm:
                case WeatherCondition.violent_storm:
                case WeatherCondition.hurricane:
                case WeatherCondition.tornado:
                case WeatherCondition.tropical_storm:
                    return "Wind.png";
                case WeatherCondition.shower_rain:
                case WeatherCondition.heavy_shower_rain:
                    return isNight ? "Moon Cloud Rain.png" : "Sun Cloud Rain.png";
                case WeatherCondition.thundershower:
                case WeatherCondition.heavy_thunderstorm:
                case WeatherCondition.hail:
                    return "Thunder Rain.png";
                case WeatherCondition.light_rain:
                case WeatherCondition.moderate_rain:
                case WeatherCondition.drizzle_rain:
                    return "Small Rain.png";
                case WeatherCondition.heavy_rain:
                case WeatherCondition.extreme_rain:
                case WeatherCondition.storm_rain:
                case WeatherCondition.heavy_storm_rain:
                case WeatherCondition.severe_storm_rain:
                    return "Rain.png";
                case WeatherCondition.sleet:
                case WeatherCondition.rain_snow:
                case WeatherCondition.freezing_rain:
                    return "Snow Rain.png";
                case WeatherCondition.light_snow:
                case WeatherCondition.moderate_snow:
                case WeatherCondition.heavy_snow:
                case WeatherCondition.snowstorm:
                case WeatherCondition.shower_snow:
                case WeatherCondition.snow_flurry:
                    return "Snow.png";
                case WeatherCondition.mist:
                case WeatherCondition.foggy:
                    return "Fog.png";
                case WeatherCondition.haze:
                case WeatherCondition.sand:
                case WeatherCondition.dust:
                case WeatherCondition.volcanic_ash:
                case WeatherCondition.duststorm:
                case WeatherCondition.sandstorm:
                    return "Haze.png";
                case WeatherCondition.hot:
                    return "Hot.png";
                case WeatherCondition.cold:
                    return "Cold.png";
                default:
                    return null;
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
                return 64d;
            double temp = ((Temperature)value).Celsius;
            temp = temp < -15 ? -15d : temp;
            temp = temp > 40 ? 40d : temp;
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
            double temp = (uint)value;
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
                return 64d;
            double temp = (float)value;
            temp = temp > 150d ? 150d : temp;
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
            if (value == null)
            {
                return "...";
            }
            return ((float)value).ToString("0.#") + " mm";
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
            if (value == null)
            {
                return new DoubleCollection { 98.96666666, 0, 0, 1000 };
            }
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
            if (value == null)
            {
                return HorizontalAlignment.Left;
            }
            return (bool)value ? HorizontalAlignment.Right : HorizontalAlignment.Left;
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
            if (value == null)
            {
                return HorizontalAlignment.Right;
            }
            return (bool)value ? HorizontalAlignment.Left : HorizontalAlignment.Right;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class MoonPhaseProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return null;
            }
            var p = (value as CalendarInfo).LunarDay;
            string uri;
            if (p < 2)
            {
                uri = "ms-appx:///Assets/MoonPhase/moon0.png";
            }
            else if (p < 6)
            {
                uri = "ms-appx:///Assets/MoonPhase/moon1.png";
            }
            else if (p < 9)
            {
                uri = "ms-appx:///Assets/MoonPhase/moon2.png";
            }
            else if (p < 13)
            {
                uri = "ms-appx:///Assets/MoonPhase/moon3.png";
            }
            else if (p < 17)
            {
                uri = "ms-appx:///Assets/MoonPhase/moon4.png";
            }
            else if (p < 21)
            {
                uri = "ms-appx:///Assets/MoonPhase/moon5.png";
            }
            else if (p < 24)
            {
                uri = "ms-appx:///Assets/MoonPhase/moon6.png";
            }
            else if (p < 28)
            {
                uri = "ms-appx:///Assets/MoonPhase/moon7.png";
            }
            else
            {
                uri = "ms-appx:///Assets/MoonPhase/moon0.png";
            }
            return new BitmapImage(new Uri(uri));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class MoonPhaseTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "...";
            }
            var loader = new ResourceLoader();
            var p = (value as CalendarInfo).LunarDay;
            string result;
            if (p < 2)
            {
                result = loader.GetString("New_Moon");
            }
            else if (p < 6)
            {
                result = loader.GetString("Waxing_crescent");
            }
            else if (p < 9)
            {
                result = loader.GetString("First_quarter");
            }
            else if (p < 13)
            {
                result = loader.GetString("Waxing_gibbous");
            }
            else if (p < 17)
            {
                result = loader.GetString("Full_moon");
            }
            else if (p < 21)
            {
                result = loader.GetString("Waning_gibbous");
            }
            else if (p < 24)
            {
                result = loader.GetString("Third_quarter");
            }
            else if (p < 28)
            {
                result = loader.GetString("Waning_crescent");
            }
            else
            {
                result = loader.GetString("New_Moon");
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class VisibilityConverter : IValueConverter
    {
        public static LengthParameter LengthParameter { get; private set; } = LengthParameter.KM;
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "...";
            }
            var l = value as Length;
            switch (LengthParameter)
            {
                case LengthParameter.KM: return l.KM.ToString("0.##") + " km";
                case LengthParameter.M: return l.M.ToString("0.##") + " m";
                case LengthParameter.Mile: return l.Mile.ToString("0.##") + " mile";
                case LengthParameter.NM: return l.NM.ToString("0.##") + " nm";
                default: return "0km";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public static void ChangeParameter(LengthParameter lengthParameter)
        {
            LengthParameter = lengthParameter;
        }
    }

    public class PressureConverter : IValueConverter
    {
        public static PressureParameter PressureParameter { get; private set; } = PressureParameter.Atm;
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "...";
            }
            var l = value as Pressure;
            switch (PressureParameter)
            {
                case PressureParameter.Atm: return l.Atm.ToString("0.####") + " Atm";
                case PressureParameter.Hpa: return l.HPa.ToString("0.####") + " Hpa";
                case PressureParameter.Torr: return l.Torr.ToString("0.####") + " Torr";
                case PressureParameter.CmHg: return l.CmHg.ToString("0.####") + " CmHg";
                default:
                    return "0 Atm";
            }
        }

        public static void ChangeParameter(PressureParameter parameter)
        {
            PressureParameter = parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PressureAniConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return 0d;
            }
            var p = value as Pressure;
            return (p.Atm - 1) * 7200;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class VisibilityAniConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return 1d;
            }
            var vis = (value as Length).KM;
            if (vis > 15)
            {
                return 1d;
            }
            else if (vis > 8)
            {
                return 0.8;
            }
            else if (vis > 4)
            {
                return 0.6;
            }
            else if (vis > 1)
            {
                return 0.4;
            }
            else if (vis > 0.5)
            {
                return 0.2;
            }
            else
            {
                return 0.1;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class MainPaneBGConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var month = DateTime.Now.Month;
            var uri = new Uri("ms-appx:///Assets/MonthlyPic/" + month + ".png");
            return new BitmapImage(uri);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class DateNowConverter : IValueConverter
    {
        public DateNowConverter()
        {
            var p = Preferences.Get();
            format = p.GetDateFormat();
        }
        private static string format = "yyyy-M-d dddd";
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (DateTime.Now.ToString(format)).Trim('\n');
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public static void Refresh()
        {
            var p = Preferences.Get();
            format = p.GetDateFormat();
        }
    }

    public class LunarCalendarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                var calendar = value as CalendarInfo;
                return ("农历 " + calendar.LunarYearSexagenary + "年" + calendar.LunarMonthText + "月" + calendar.LunarDayText + "    " + calendar.SolarTermStr).Trim();
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PaneHamburgerForeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var month = DateTime.Now.Month;
            if (month % 2 == 0 || month == 3)
            {
                return Color.FromArgb(255, 0, 0, 0);
            }
            return Color.FromArgb(255, 240, 240, 240);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class AqiCircleAniConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return new DoubleCollection { 0, 1000 };
            }
            return new DoubleCollection { (double)value * 109.97333333, 1000 };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class AqiCircleColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return Color.FromArgb(255, 240, 240, 240);
            }
            var q = (value as AQI).Qlty;
            switch (q)
            {
                case AQIQuality.unknown:
                    return Color.FromArgb(255, 240, 240, 240);
                case AQIQuality.one:
                    return Palette.Green;
                case AQIQuality.two:
                    return Palette.Teal;
                case AQIQuality.three:
                    return Palette.Lime;
                case AQIQuality.four:
                    return Palette.Amber;
                case AQIQuality.five:
                    return Palette.DeepOrange;
                case AQIQuality.six:
                    return Palette.Red;
                default:
                    return Color.FromArgb(255, 240, 240, 240);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class AQIQualityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "...";
            }
            var q = (value as AQI).Qlty;
            switch (q)
            {
                case AQIQuality.unknown:
                    return "...";
                case AQIQuality.one:
                    return "优";
                case AQIQuality.two:
                    return "良";
                case AQIQuality.three:
                    return "轻度污染";
                case AQIQuality.four:
                    return "中度污染";
                case AQIQuality.five:
                    return "重度污染";
                case AQIQuality.six:
                    return "严重污染";
                default:
                    return "...";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class AQIValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "0";
            }
            return (value as AQI).Aqi.ToString("0");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PM25Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "0";
            }
            return (value as AQI).Pm25.ToString("0");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PM10Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "0";
            }
            return (value as AQI).Pm10.ToString("0");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SO2Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "0";
            }
            return (value as AQI).So2.ToString("0");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class COConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "0";
            }
            return (value as AQI).Co.ToString("0");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class NO2Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "0";
            }
            return (value as AQI).No2.ToString("0");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class O3Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "0";
            }
            return (value as AQI).O3.ToString("0");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SO2ProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return 0d;
            }
            return (value as AQI).So2 > 2620 ? 100d : (value as AQI).So2 / 2.620;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class NO2ProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return 0d;
            }
            return (value as AQI).No2 > 940 ? 100d : (value as AQI).No2 / 9.4;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class COProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return 0d;
            }
            return (value as AQI).Co > 60 ? 100d : (value as AQI).Co / 0.6;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class O3ProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return 0d;
            }
            return (value as AQI).O3 > 1200 ? 100d : (value as AQI).O3 / 12d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PM25ProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return 0d;
            }
            return (value as AQI).Pm25 > 500 ? 100d : (value as AQI).Pm25 / 5d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PM10ProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return 0d;
            }
            return (value as AQI).Pm10 > 600 ? 100d : (value as AQI).Pm10 / 6d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class AqiProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return 0d;
            }
            return (value as AQI).Aqi > 500 ? 1d : (value as AQI).Aqi / 500d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ImmersiveHourConverter : IValueConverter
    {
        public static string DateTimeConverterParameter { get; private set; } = "H";
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "...";
            }
            if (DateTimeConverterParameter == null || DateTimeConverterParameter.Length == 0)
            {
                return "";
            }
            try
            {
                return (((DateTime)value).ToString(DateTimeConverterParameter)).Trim('\n');
            }
            catch (Exception)
            {
                return "...";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public static void ChangeParameter(string par)
        {
            DateTimeConverterParameter = par;
        }
    }

    public class ImmersiveMinConverter : IValueConverter
    {
        public static string DateTimeConverterParameter { get; private set; } = "mm";
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "...";
            }
            if (DateTimeConverterParameter == null || DateTimeConverterParameter.Length == 0)
            {
                return "";
            }
            try
            {
                return ((DateTime)value).ToString(DateTimeConverterParameter).Trim('\n');
            }
            catch (Exception)
            {
                return "...";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public static void ChangeParameter(string par)
        {
            DateTimeConverterParameter = par;
        }
    }

    public class ImmersiveSecConverter : IValueConverter
    {
        public static string DateTimeConverterParameter { get; private set; } = "ss";
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "...";
            }
            try
            {
                return ((DateTime)value).ToString(DateTimeConverterParameter).Trim('\n');
            }
            catch (Exception)
            {
                return "...";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SuggestionBrfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "...";
            }
            return (value as Suggestion).Brief;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SuggestionTxtConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "...";
            }
            return (value as Suggestion).Text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class CitySettingsModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "...";
            }
            return ((CitySettingsModel)value).City;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ImmersiveStatetoSwitchConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return false;
            }
            var v = (ImmersiveBackgroundState)value;
            switch (v)
            {
                case ImmersiveBackgroundState.Assets:
                case ImmersiveBackgroundState.Local:
                    return false;
                case ImmersiveBackgroundState.Transparent:
                    return true;
                default: return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var v = (bool)value;
            if (v)
            {
                return ImmersiveBackgroundState.Transparent;
            }
            else
            {
                return ImmersiveBackgroundState.Fallback;
            }
        }
    }
    public class ImmersiveStatetoSelectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return ListViewSelectionMode.None;
            }
            var v = (ImmersiveBackgroundState)value;
            switch (v)
            {
                case ImmersiveBackgroundState.Assets:
                case ImmersiveBackgroundState.Local:
                    return ListViewSelectionMode.Single;
                case ImmersiveBackgroundState.Transparent:
                    return ListViewSelectionMode.None;
                default: return ListViewSelectionMode.None;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ImmersiveStatetoVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }
            var v = (ImmersiveBackgroundState)value;
            switch (v)
            {
                case ImmersiveBackgroundState.Assets:
                    return Visibility.Visible;
                case ImmersiveBackgroundState.Local:
                    return Visibility.Collapsed;
                case ImmersiveBackgroundState.Transparent:
                    return Visibility.Collapsed;
                default: return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ImmersiveStatetoEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return false;
            }
            var v = (ImmersiveBackgroundState)value;
            switch (v)
            {
                case ImmersiveBackgroundState.Assets:
                case ImmersiveBackgroundState.Local:
                    return true;
                case ImmersiveBackgroundState.Transparent:
                    return false;
                default: return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }


    public class shiliubijiuConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return 0d;
            }
            return (double)value * 9 / 16;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class UpdateTimeConverter : IValueConverter
    {
        private static string Parameter = "HH:mm";
        public object Convert(object value, Type targetType, object parameter, string language)
        {

            if (value == null)
            {
                return "...";
            }
            var loader = new ResourceLoader();
            var updateTime = (DateTime)value;
            if ((DateTime.Now - updateTime).TotalMinutes < 30)
            {
                return string.Format(loader.GetString("MinutesIn"), ((DateTime.Now - updateTime).TotalMinutes.ToString("0")).Trim('\n'));
            }
            else
            {
                return ((updateTime).ToString(Parameter)).Trim('\n') + loader.GetString("Update");
            }
        }

        public static void ChangeParameter(string parameter)
        {
            Parameter = parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class RefreshCompleteConverter : IValueConverter
    {
        private static string Parameter = "HH:mm";
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "";
            }
            if (value is DateTime)
            {
                var loader = new ResourceLoader();
                return string.Format(loader.GetString("RefreshStamp"), (((DateTime)value).ToString(Parameter)).Trim('\n'));
            }
            return "";
        }

        public static void ChangeParameter(string parameter)
        {
            Parameter = parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class AlarmLeveltoColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return new SolidColorBrush(Colors.Black);
            }
            var v = (WeatherAlarmLevel)value;
            switch (v)
            {
                case WeatherAlarmLevel.blue:
                    return new SolidColorBrush(Colors.Blue);
                case WeatherAlarmLevel.yellow:
                    return new SolidColorBrush(Colors.Yellow);
                case WeatherAlarmLevel.orange:
                    return new SolidColorBrush(Colors.Orange);
                case WeatherAlarmLevel.red:
                    return new SolidColorBrush(Colors.Red);
                default:
                    return new SolidColorBrush(Colors.Black);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class FontSizeConverter : IValueConverter
    {
        public static double FIXED_TITLE_FONTSIZE = 96;
        private const int WEATHERCANVAS_HEADEROFFSET = 528;
        public static double offset = 0;
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;
            var verticalOffset = (double)value;
            offset = verticalOffset > WEATHERCANVAS_HEADEROFFSET ? WEATHERCANVAS_HEADEROFFSET : verticalOffset;
            offset /= WEATHERCANVAS_HEADEROFFSET;
            offset = EasingHelper.QuinticEase(Windows.UI.Xaml.Media.Animation.EasingMode.EaseOut, offset);
            var k = FIXED_TITLE_FONTSIZE - FIXED_TITLE_FONTSIZE / 2 * offset;
            if (k < 48d)
                return 48d;
            else
            {
                return k;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class FreqConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double)
            {
                return ((double)value / 60).ToString("0.#") + (" hours");
            }
            return "...";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class NowStyleMinConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool)
            {
                if ((bool)value)
                {
                    return 96d;
                }
                else
                {
                    return 192d;
                }
            }
            return 192d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class NowStyleMaxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool)
            {
                if ((bool)value)
                {
                    return 256d;
                }
                else
                {
                    return 420d;
                }
            }
            return 420d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ScrollablePaddingWideConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double)
            {
                return (double)value - 48d;
            }
            return 444d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
