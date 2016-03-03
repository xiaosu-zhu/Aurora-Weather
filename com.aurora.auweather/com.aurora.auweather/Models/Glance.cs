using Com.Aurora.AuWeather.Models.HeWeather;
using Com.Aurora.Shared.Converters;
using Com.Aurora.Shared.Extensions;
using Com.Aurora.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private static readonly string[] haotianqi = new string[] { "适宜外出" };
        private static readonly string[] huaitianqi = new string[] { "天气不好", "天气较差" };
        private static readonly string[] shiduda = new string[] { "湿度较大" };
        private static readonly string[] nengjiandudi = new string[] { "能见度低" };
        #endregion

        internal static string GenerateGlanceDescription(HeWeatherModel model, bool isNight, TempratureParameter parameter)
        {
            var 主语 = model.NowWeather.Now.Condition;
            string zhuyu = "现在";
            if (Tools.RandomBetween(0, 200) > 100)
            {
                if (isNight)
                {
                    主语 = model.DailyForecast[0].Condition.NightCond;
                    zhuyu = "晚间";
                }
                else
                {
                    主语 = model.DailyForecast[0].Condition.DayCond;
                    zhuyu = "今天";
                }
            }
            string binyu;
            StringBuilder decoration = new StringBuilder();
            if (Tools.RandomBetween(100, 300) > 200)
            {
                if (model.DailyForecast[0].Humidity > 80)
                {
                    decoration.Append(shiduda[0] + ", ");
                }
                if (model.NowWeather.Visibility.KM < 0.5)
                {
                    decoration.Append(nengjiandudi[0] + ", ");
                }
                switch (主语)
                {
                    case WeatherCondition.unknown:
                        break;
                    case WeatherCondition.sunny:
                    case WeatherCondition.few_clouds:
                    case WeatherCondition.partly_cloudy:
                    case WeatherCondition.overcast:
                        break;
                    case WeatherCondition.cloudy:
                        break;
                    case WeatherCondition.windy:
                        break;
                    case WeatherCondition.calm:
                        break;
                    case WeatherCondition.light_breeze:
                        break;
                    case WeatherCondition.moderate:
                        break;
                    case WeatherCondition.fresh_breeze:
                        break;
                    case WeatherCondition.strong_breeze:
                        break;
                    case WeatherCondition.high_wind:
                        break;
                    case WeatherCondition.gale:
                        break;
                    case WeatherCondition.strong_gale:
                        break;
                    case WeatherCondition.storm:
                        break;
                    case WeatherCondition.violent_storm:
                        break;
                    case WeatherCondition.hurricane:
                        break;
                    case WeatherCondition.tornado:
                        break;
                    case WeatherCondition.tropical_storm:
                        break;
                    case WeatherCondition.shower_rain:
                        break;
                    case WeatherCondition.heavy_shower_rain:
                        break;
                    case WeatherCondition.thundershower:
                        break;
                    case WeatherCondition.heavy_thunderstorm:
                        break;
                    case WeatherCondition.hail:
                        break;
                    case WeatherCondition.light_rain:
                        break;
                    case WeatherCondition.moderate_rain:
                        break;
                    case WeatherCondition.heavy_rain:
                        break;
                    case WeatherCondition.extreme_rain:
                        break;
                    case WeatherCondition.drizzle_rain:
                        break;
                    case WeatherCondition.storm_rain:
                        break;
                    case WeatherCondition.heavy_storm_rain:
                        break;
                    case WeatherCondition.severe_storm_rain:
                        break;
                    case WeatherCondition.freezing_rain:
                        break;
                    case WeatherCondition.light_snow:
                        break;
                    case WeatherCondition.moderate_snow:
                        break;
                    case WeatherCondition.heavy_snow:
                        break;
                    case WeatherCondition.snowstorm:
                        break;
                    case WeatherCondition.sleet:
                        break;
                    case WeatherCondition.rain_snow:
                        break;
                    case WeatherCondition.shower_snow:
                        break;
                    case WeatherCondition.snow_flurry:
                        break;
                    case WeatherCondition.mist:
                        break;
                    case WeatherCondition.foggy:
                        break;
                    case WeatherCondition.haze:
                        break;
                    case WeatherCondition.sand:
                        break;
                    case WeatherCondition.dust:
                        break;
                    case WeatherCondition.volcanic_ash:
                        break;
                    case WeatherCondition.duststorm:
                        break;
                    case WeatherCondition.sandstorm:
                        break;
                    case WeatherCondition.hot:
                        break;
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
                case TempratureParameter.Celsius:
                    nowtemp = model.DailyForecast[0].LowTemp.Celsius + "° ~" + model.DailyForecast[0].HighTemp.Celsius + "°";
                    tomorrowtemp = model.DailyForecast[1].LowTemp.Celsius + "° ~" + model.DailyForecast[1].HighTemp.Celsius + "°";
                    break;
                case TempratureParameter.Fahrenheit:
                    nowtemp = model.DailyForecast[0].LowTemp.Fahrenheit + "° ~" + model.DailyForecast[0].HighTemp.Fahrenheit + "°";
                    tomorrowtemp = model.DailyForecast[1].LowTemp.Fahrenheit + "° ~" + model.DailyForecast[1].HighTemp.Fahrenheit + "°";
                    break;
                case TempratureParameter.Kelvin:
                    nowtemp = model.DailyForecast[0].LowTemp.Kelvin + "K ~" + model.DailyForecast[0].HighTemp.Kelvin + "K";
                    tomorrowtemp = model.DailyForecast[1].LowTemp.Kelvin + "° ~" + model.DailyForecast[1].HighTemp.Kelvin + "°";
                    break;
                default:
                    nowtemp = model.DailyForecast[0].LowTemp.Celsius + "° ~" + model.DailyForecast[0].HighTemp.Celsius + "°";
                    tomorrowtemp = model.DailyForecast[1].LowTemp.Celsius + "° ~" + model.DailyForecast[1].HighTemp.Celsius + "°";
                    break;
            }
            string tomorrowcondition;
            var converter = new ConditiontoTextConverter();
            tomorrowcondition = (string)converter.Convert(model.DailyForecast[1].Condition.DayCond, null, null, null);
            return string.Format("{0}{1}, {2}, {3}. 明日: {5},{6}.", zhuyu, binyu, decoration, nowtemp, tomorrowcondition, tomorrowtemp);
        }
    }
}
