using Com.Aurora.AuWeather.Models.HeWeather;
using Com.Aurora.Shared.Converters;
using Com.Aurora.Shared.Extensions;
using Com.Aurora.Shared.Helpers;
using System.Text;
using System;

namespace Com.Aurora.AuWeather.Models
{
    public class Glance
    {
        #region Condition desiciption
        private static readonly string[] sunny = new string[] { "晴朗", "无云" };
        private static readonly string[] cloudy = new string[] { "多云", "有云" };
        private static readonly string[] few_clouds = new string[] { "少云", "飘云" };
        private static readonly string[] partly_cloudy = new string[] { "局部多云" };
        private static readonly string[] overcast = new string[] { "阴天", "没有太阳" };
        private static readonly string[] windy = new string[] { "有风" };
        private static readonly string[] breeze = new string[] { "微风", "和风" };
        private static readonly string[] gale = new string[] { "有强风", "大风天" };
        private static readonly string[] storm = new string[] { "注意风暴" };
        private static readonly string[] thundershower = new string[] { "有雷雨" };
        private static readonly string[] hail = new string[] { "有冰雹" };
        private static readonly string[] moderate_rain = new string[] { "小雨", "有雨", "雨天", "需要带伞" };
        private static readonly string[] heavy_rain = new string[] { "有大雨", "外出注意雨伞" };
        private static readonly string[] heavy_storm_rain = new string[] { "有暴风雨", "不宜外出" };
        private static readonly string[] shower = new string[] { "有阵雨", "间歇降雨" };
        private static readonly string[] freezing_rain = new string[] { "有冻雨" };
        private static readonly string[] moderate_snow = new string[] { "有雪", "雪天", "会下雪" };
        private static readonly string[] heavy_snow = new string[] { "有大雪", "大雪天" };
        private static readonly string[] shower_snow = new string[] { "有阵雪", "有短时降雪" };
        private static readonly string[] snowstorm = new string[] { "有暴风雪", "有恶劣降雪" };
        private static readonly string[] sleet = new string[] { "有雨夹雪", "雨雪" };
        private static readonly string[] mist = new string[] { "有薄雾" };
        private static readonly string[] foggy = new string[] { "有雾" };
        private static readonly string[] haze = new string[] { "有霾" };
        private static readonly string[] dust = new string[] { "有沙尘" };
        private static readonly string[] duststorm = new string[] { "有恶劣沙尘" };
        #endregion
        #region Condition decoration
        private static readonly string[] haotianqi = new string[] { "适宜外出", "天气很好" };
        private static readonly string[] huaitianqi = new string[] { "天气不好", "天气较差" };
        private static readonly string[] shiduda = new string[] { "湿度较大", "空气湿润" };
        private static readonly string[] nengjiandudi = new string[] { "能见度低", "能见度差" };
        private static readonly string[] wendugao = new string[] { "高温", "持续升温" };
        private static readonly string[] wendudi = new string[] { "低温", "持续降温" };
        #endregion
        public static string GenerateShortDescription(HeWeatherModel model, bool isNight)
        {
#if DEBUG
            var todayIndex = 0;
#else
            var todayIndex = Array.FindIndex(model.DailyForecast, x =>
            {
                return (x.Date - desiredDate).TotalSeconds > 0;
            }) - 1;
#endif
            var 主语 = model.NowWeather.Now.Condition;
            string zhuyu = "现在";
            if (Tools.Random.Next(200) >= 100)
            {
                if (isNight)
                {
                    主语 = model.DailyForecast[todayIndex].Condition.NightCond;
                    zhuyu = "晚间";
                }
                else
                {
                    主语 = model.DailyForecast[todayIndex].Condition.DayCond;
                    zhuyu = "今天";
                }
            }
            string binyu;
            StringBuilder decoration = new StringBuilder();
            if (Tools.Random.Next(200) >= 100)
            {
                if (Tools.Random.Next(200) >= 100 && model.NowWeather.Temprature.Celsius < 0)
                {
                    decoration.Append(wendudi.SelectRandomString() + ", ");
                }
                else if (Tools.Random.Next(200) >= 100 && model.NowWeather.Temprature.Celsius > 25)
                {
                    decoration.Append(wendugao.SelectRandomString() + ", ");
                }
                if (Tools.Random.Next(200) >= 100 && model.DailyForecast[todayIndex].Humidity > 80)
                {
                    decoration.Append(shiduda[0] + ", ");
                }
                if (Tools.Random.Next(200) >= 100 && model.NowWeather.Visibility.KM < 0.5)
                {
                    decoration.Append(nengjiandudi[0] + ", ");
                }
                if (Tools.Random.Next(200) >= 100)
                    switch (主语)
                    {
                        case WeatherCondition.unknown:
                            return "未知天气";
                        case WeatherCondition.sunny:
                        case WeatherCondition.few_clouds:
                        case WeatherCondition.partly_cloudy:
                        case WeatherCondition.calm:
                        case WeatherCondition.light_breeze:
                        case WeatherCondition.moderate:
                        case WeatherCondition.fresh_breeze:
                            decoration.Append(haotianqi.SelectRandomString() + ", ");
                            break;
                        case WeatherCondition.overcast:
                        case WeatherCondition.cloudy:
                        case WeatherCondition.windy:
                            break;
                        case WeatherCondition.strong_breeze:
                        case WeatherCondition.high_wind:
                            break;
                        case WeatherCondition.gale:
                        case WeatherCondition.strong_gale:
                        case WeatherCondition.storm:
                        case WeatherCondition.violent_storm:
                        case WeatherCondition.hurricane:
                        case WeatherCondition.tornado:
                        case WeatherCondition.tropical_storm:
                            decoration.Append(huaitianqi.SelectRandomString() + ", ");
                            break;
                        case WeatherCondition.shower_rain:
                            break;
                        case WeatherCondition.heavy_shower_rain:
                        case WeatherCondition.thundershower:
                        case WeatherCondition.heavy_thunderstorm:
                        case WeatherCondition.hail:
                            decoration.Append(huaitianqi.SelectRandomString() + ", ");
                            break;
                        case WeatherCondition.light_rain:
                        case WeatherCondition.moderate_rain:
                            break;
                        case WeatherCondition.heavy_rain:
                        case WeatherCondition.extreme_rain:
                            decoration.Append(huaitianqi.SelectRandomString() + ", ");
                            break;
                        case WeatherCondition.drizzle_rain:
                            break;
                        case WeatherCondition.storm_rain:
                        case WeatherCondition.heavy_storm_rain:
                        case WeatherCondition.severe_storm_rain:
                        case WeatherCondition.freezing_rain:
                            decoration.Append(huaitianqi.SelectRandomString() + ", ");
                            break;
                        case WeatherCondition.light_snow:
                            break;
                        case WeatherCondition.moderate_snow:
                        case WeatherCondition.heavy_snow:
                        case WeatherCondition.snowstorm:
                            decoration.Append(huaitianqi.SelectRandomString() + ", ");
                            break;
                        case WeatherCondition.sleet:
                        case WeatherCondition.rain_snow:
                        case WeatherCondition.shower_snow:
                        case WeatherCondition.snow_flurry:
                            break;
                        case WeatherCondition.mist:
                            break;
                        case WeatherCondition.foggy:
                        case WeatherCondition.haze:
                        case WeatherCondition.sand:
                        case WeatherCondition.dust:
                        case WeatherCondition.volcanic_ash:
                        case WeatherCondition.duststorm:
                        case WeatherCondition.sandstorm:
                            decoration.Append(huaitianqi.SelectRandomString() + ", ");
                            break;
                        case WeatherCondition.hot:
                        case WeatherCondition.cold:
                            break;
                        default:
                            break;
                    }
                if (decoration.ToString().EndsWith(", "))
                {
                    decoration.Remove(decoration.Length - 2, 2);
                }
            }
            switch (主语)
            {
                case WeatherCondition.unknown:
                    return "未知天气";
                case WeatherCondition.sunny:
                    binyu = sunny.SelectRandomString();
                    break;
                case WeatherCondition.cloudy:
                    binyu = cloudy.SelectRandomString();
                    break;
                case WeatherCondition.few_clouds:
                    binyu = few_clouds.SelectRandomString();
                    break;
                case WeatherCondition.partly_cloudy:
                    binyu = partly_cloudy.SelectRandomString();
                    break;
                case WeatherCondition.overcast:
                    binyu = overcast.SelectRandomString();
                    break;
                case WeatherCondition.windy:
                    binyu = windy.SelectRandomString();
                    break;
                case WeatherCondition.calm:
                case WeatherCondition.light_breeze:
                case WeatherCondition.moderate:
                case WeatherCondition.fresh_breeze:
                    binyu = breeze.SelectRandomString();
                    break;
                case WeatherCondition.strong_breeze:
                case WeatherCondition.high_wind:
                case WeatherCondition.gale:
                case WeatherCondition.strong_gale:
                    binyu = gale.SelectRandomString();
                    break;
                case WeatherCondition.storm:
                case WeatherCondition.violent_storm:
                case WeatherCondition.hurricane:
                case WeatherCondition.tornado:
                case WeatherCondition.tropical_storm:
                    binyu = storm.SelectRandomString();
                    break;
                case WeatherCondition.shower_rain:
                case WeatherCondition.heavy_shower_rain:
                    binyu = shower.SelectRandomString();
                    break;
                case WeatherCondition.thundershower:
                case WeatherCondition.heavy_thunderstorm:
                    binyu = thundershower.SelectRandomString();
                    break;
                case WeatherCondition.hail:
                    binyu = hail.SelectRandomString();
                    break;
                case WeatherCondition.drizzle_rain:
                case WeatherCondition.light_rain:
                case WeatherCondition.moderate_rain:
                    binyu = moderate_rain.SelectRandomString();
                    break;
                case WeatherCondition.heavy_rain:
                case WeatherCondition.extreme_rain:
                    binyu = heavy_rain.SelectRandomString();
                    break;
                case WeatherCondition.storm_rain:
                case WeatherCondition.heavy_storm_rain:
                case WeatherCondition.severe_storm_rain:
                    binyu = storm.SelectRandomString();
                    break;
                case WeatherCondition.freezing_rain:
                    binyu = freezing_rain.SelectRandomString();
                    break;
                case WeatherCondition.light_snow:
                case WeatherCondition.moderate_snow:
                    binyu = moderate_snow.SelectRandomString();
                    break;
                case WeatherCondition.heavy_snow:
                    binyu = heavy_snow.SelectRandomString();
                    break;
                case WeatherCondition.snowstorm:
                    binyu = snowstorm.SelectRandomString();
                    break;
                case WeatherCondition.sleet:
                case WeatherCondition.rain_snow:
                    binyu = sleet.SelectRandomString();
                    break;
                case WeatherCondition.shower_snow:
                case WeatherCondition.snow_flurry:
                    binyu = shower_snow.SelectRandomString();
                    break;
                case WeatherCondition.mist:
                    binyu = mist.SelectRandomString();
                    break;
                case WeatherCondition.foggy:
                    binyu = foggy.SelectRandomString();
                    break;
                case WeatherCondition.haze:
                    binyu = haze.SelectRandomString();
                    break;
                case WeatherCondition.sand:
                case WeatherCondition.dust:
                    binyu = dust.SelectRandomString();
                    break;
                case WeatherCondition.volcanic_ash:
                case WeatherCondition.duststorm:
                case WeatherCondition.sandstorm:
                    binyu = duststorm.SelectRandomString();
                    break;
                case WeatherCondition.hot:
                    binyu = "会变热";
                    break;
                case WeatherCondition.cold:
                    binyu = "会变冷";
                    break;
                default: return "未知天气";
            }
            return string.Format("{0}{1}  {2}.", zhuyu, binyu, decoration);
        }

        public static string GenerateGlanceDescription(HeWeatherModel model, bool isNight, TemperatureParameter parameter, DateTime desiredDate)
        {
#if DEBUG
            var todayIndex = 0;
#else
            var todayIndex = Array.FindIndex(model.DailyForecast, x =>
            {
                return (x.Date - desiredDate).TotalSeconds > 0;
            }) - 1;
#endif
            var 主语 = model.NowWeather.Now.Condition;
            string zhuyu = "现在";
            if (Tools.Random.Next(200) >= 100)
            {
                if (isNight)
                {
                    主语 = model.DailyForecast[todayIndex].Condition.NightCond;
                    zhuyu = "晚间";
                }
                else
                {
                    主语 = model.DailyForecast[todayIndex].Condition.DayCond;
                    zhuyu = "今天";
                }
            }
            string binyu;
            StringBuilder decoration = new StringBuilder();
            if (Tools.Random.Next(200) >= 100)
            {
                if (Tools.Random.Next(200) >= 100 && model.NowWeather.Temprature.Celsius < 0)
                {
                    decoration.Append(wendudi.SelectRandomString() + ", ");
                }
                else if (Tools.Random.Next(200) >= 100 && model.NowWeather.Temprature.Celsius > 25)
                {
                    decoration.Append(wendugao.SelectRandomString() + ", ");
                }
                if (Tools.Random.Next(200) >= 100 && model.DailyForecast[todayIndex].Humidity > 80)
                {
                    decoration.Append(shiduda[0] + ", ");
                }
                if (Tools.Random.Next(200) >= 100 && model.NowWeather.Visibility.KM < 0.5)
                {
                    decoration.Append(nengjiandudi[0] + ", ");
                }
                if (Tools.Random.Next(200) >= 100)
                    switch (主语)
                    {
                        case WeatherCondition.unknown:
                            return "未知天气";
                        case WeatherCondition.sunny:
                        case WeatherCondition.few_clouds:
                        case WeatherCondition.partly_cloudy:
                        case WeatherCondition.calm:
                        case WeatherCondition.light_breeze:
                        case WeatherCondition.moderate:
                        case WeatherCondition.fresh_breeze:
                            decoration.Append(haotianqi.SelectRandomString() + ", ");
                            break;
                        case WeatherCondition.overcast:
                        case WeatherCondition.cloudy:
                        case WeatherCondition.windy:
                            break;
                        case WeatherCondition.strong_breeze:
                        case WeatherCondition.high_wind:
                            break;
                        case WeatherCondition.gale:
                        case WeatherCondition.strong_gale:
                        case WeatherCondition.storm:
                        case WeatherCondition.violent_storm:
                        case WeatherCondition.hurricane:
                        case WeatherCondition.tornado:
                        case WeatherCondition.tropical_storm:
                            decoration.Append(huaitianqi.SelectRandomString() + ", ");
                            break;
                        case WeatherCondition.shower_rain:
                            break;
                        case WeatherCondition.heavy_shower_rain:
                        case WeatherCondition.thundershower:
                        case WeatherCondition.heavy_thunderstorm:
                        case WeatherCondition.hail:
                            decoration.Append(huaitianqi.SelectRandomString() + ", ");
                            break;
                        case WeatherCondition.light_rain:
                        case WeatherCondition.moderate_rain:
                            break;
                        case WeatherCondition.heavy_rain:
                        case WeatherCondition.extreme_rain:
                            decoration.Append(huaitianqi.SelectRandomString() + ", ");
                            break;
                        case WeatherCondition.drizzle_rain:
                            break;
                        case WeatherCondition.storm_rain:
                        case WeatherCondition.heavy_storm_rain:
                        case WeatherCondition.severe_storm_rain:
                        case WeatherCondition.freezing_rain:
                            decoration.Append(huaitianqi.SelectRandomString() + ", ");
                            break;
                        case WeatherCondition.light_snow:
                            break;
                        case WeatherCondition.moderate_snow:
                        case WeatherCondition.heavy_snow:
                        case WeatherCondition.snowstorm:
                            decoration.Append(huaitianqi.SelectRandomString() + ", ");
                            break;
                        case WeatherCondition.sleet:
                        case WeatherCondition.rain_snow:
                        case WeatherCondition.shower_snow:
                        case WeatherCondition.snow_flurry:
                            break;
                        case WeatherCondition.mist:
                            break;
                        case WeatherCondition.foggy:
                        case WeatherCondition.haze:
                        case WeatherCondition.sand:
                        case WeatherCondition.dust:
                        case WeatherCondition.volcanic_ash:
                        case WeatherCondition.duststorm:
                        case WeatherCondition.sandstorm:
                            decoration.Append(huaitianqi.SelectRandomString() + ", ");
                            break;
                        case WeatherCondition.hot:
                        case WeatherCondition.cold:
                            break;
                        default:
                            break;
                    }
            }
            switch (主语)
            {
                case WeatherCondition.unknown:
                    return "未知天气";
                case WeatherCondition.sunny:
                    binyu = sunny.SelectRandomString();
                    break;
                case WeatherCondition.cloudy:
                    binyu = cloudy.SelectRandomString();
                    break;
                case WeatherCondition.few_clouds:
                    binyu = few_clouds.SelectRandomString();
                    break;
                case WeatherCondition.partly_cloudy:
                    binyu = partly_cloudy.SelectRandomString();
                    break;
                case WeatherCondition.overcast:
                    binyu = overcast.SelectRandomString();
                    break;
                case WeatherCondition.windy:
                    binyu = windy.SelectRandomString();
                    break;
                case WeatherCondition.calm:
                case WeatherCondition.light_breeze:
                case WeatherCondition.moderate:
                case WeatherCondition.fresh_breeze:
                    binyu = breeze.SelectRandomString();
                    break;
                case WeatherCondition.strong_breeze:
                case WeatherCondition.high_wind:
                case WeatherCondition.gale:
                case WeatherCondition.strong_gale:
                    binyu = gale.SelectRandomString();
                    break;
                case WeatherCondition.storm:
                case WeatherCondition.violent_storm:
                case WeatherCondition.hurricane:
                case WeatherCondition.tornado:
                case WeatherCondition.tropical_storm:
                    binyu = storm.SelectRandomString();
                    break;
                case WeatherCondition.shower_rain:
                case WeatherCondition.heavy_shower_rain:
                    binyu = shower.SelectRandomString();
                    break;
                case WeatherCondition.thundershower:
                case WeatherCondition.heavy_thunderstorm:
                    binyu = thundershower.SelectRandomString();
                    break;
                case WeatherCondition.hail:
                    binyu = hail.SelectRandomString();
                    break;
                case WeatherCondition.drizzle_rain:
                case WeatherCondition.light_rain:
                case WeatherCondition.moderate_rain:
                    binyu = moderate_rain.SelectRandomString();
                    break;
                case WeatherCondition.heavy_rain:
                case WeatherCondition.extreme_rain:
                    binyu = heavy_rain.SelectRandomString();
                    break;
                case WeatherCondition.storm_rain:
                case WeatherCondition.heavy_storm_rain:
                case WeatherCondition.severe_storm_rain:
                    binyu = storm.SelectRandomString();
                    break;
                case WeatherCondition.freezing_rain:
                    binyu = freezing_rain.SelectRandomString();
                    break;
                case WeatherCondition.light_snow:
                case WeatherCondition.moderate_snow:
                    binyu = moderate_snow.SelectRandomString();
                    break;
                case WeatherCondition.heavy_snow:
                    binyu = heavy_snow.SelectRandomString();
                    break;
                case WeatherCondition.snowstorm:
                    binyu = snowstorm.SelectRandomString();
                    break;
                case WeatherCondition.sleet:
                case WeatherCondition.rain_snow:
                    binyu = sleet.SelectRandomString();
                    break;
                case WeatherCondition.shower_snow:
                case WeatherCondition.snow_flurry:
                    binyu = shower_snow.SelectRandomString();
                    break;
                case WeatherCondition.mist:
                    binyu = mist.SelectRandomString();
                    break;
                case WeatherCondition.foggy:
                    binyu = foggy.SelectRandomString();
                    break;
                case WeatherCondition.haze:
                    binyu = haze.SelectRandomString();
                    break;
                case WeatherCondition.sand:
                case WeatherCondition.dust:
                    binyu = dust.SelectRandomString();
                    break;
                case WeatherCondition.volcanic_ash:
                case WeatherCondition.duststorm:
                case WeatherCondition.sandstorm:
                    binyu = duststorm.SelectRandomString();
                    break;
                case WeatherCondition.hot:
                    binyu = "会变热";
                    break;
                case WeatherCondition.cold:
                    binyu = "会变冷";
                    break;
                default: return "未知天气";
            }
            string nowtemp;
            string tomorrowtemp;
            switch (parameter)
            {
                case TemperatureParameter.Celsius:
                    nowtemp = model.DailyForecast[todayIndex].LowTemp.Celsius + "°~" + model.DailyForecast[todayIndex].HighTemp.Celsius + "°";
                    tomorrowtemp = model.DailyForecast[todayIndex + 1].LowTemp.Celsius + "°~" + model.DailyForecast[todayIndex + 1].HighTemp.Celsius + "°";
                    break;
                case TemperatureParameter.Fahrenheit:
                    nowtemp = model.DailyForecast[todayIndex + 0].LowTemp.Fahrenheit + "°~" + model.DailyForecast[todayIndex + 0].HighTemp.Fahrenheit + "°";
                    tomorrowtemp = model.DailyForecast[todayIndex + 1].LowTemp.Fahrenheit + "°~" + model.DailyForecast[todayIndex + 1].HighTemp.Fahrenheit + "°";
                    break;
                case TemperatureParameter.Kelvin:
                    nowtemp = model.DailyForecast[todayIndex + 0].LowTemp.Kelvin + "K~" + model.DailyForecast[todayIndex + 0].HighTemp.Kelvin + "K";
                    tomorrowtemp = model.DailyForecast[todayIndex + 1].LowTemp.Kelvin + "K~" + model.DailyForecast[todayIndex + 1].HighTemp.Kelvin + "K";
                    break;
                default:
                    nowtemp = model.DailyForecast[todayIndex + 0].LowTemp.Celsius + "°~" + model.DailyForecast[todayIndex + 0].HighTemp.Celsius + "°";
                    tomorrowtemp = model.DailyForecast[todayIndex + 1].LowTemp.Celsius + "°~" + model.DailyForecast[todayIndex + 1].HighTemp.Celsius + "°";
                    break;
            }
            string tomorrowcondition;
            var converter = new ConditiontoTextConverter();
            tomorrowcondition = (string)converter.Convert(model.DailyForecast[todayIndex + 1].Condition.DayCond, null, null, null);
            return string.Format("{0}{1}, {2}{3}. 明日: {4},{5}.", zhuyu, binyu, decoration, nowtemp, tomorrowcondition, tomorrowtemp);
        }
    }
}
