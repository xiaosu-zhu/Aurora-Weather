// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
        static Windows.ApplicationModel.Resources.ResourceLoader loader = new Windows.ApplicationModel.Resources.ResourceLoader();
        #region Condition desiciption
        private static readonly string[] sunny = new string[] { loader.GetString("GlanceSunny0"), loader.GetString("GlanceSunny1") };
        private static readonly string[] cloudy = new string[] { loader.GetString("Glancecloudy0"), loader.GetString("Glancecloudy1") };
        private static readonly string[] few_clouds = new string[] { loader.GetString("few_clouds0"), loader.GetString("few_clouds1") };
        private static readonly string[] partly_cloudy = new string[] { loader.GetString("partly_cloudy0") };
        private static readonly string[] overcast = new string[] { loader.GetString("overcast0"), loader.GetString("overcast1") };
        private static readonly string[] windy = new string[] { loader.GetString("windy0") };
        private static readonly string[] breeze = new string[] { loader.GetString("breeze0"), loader.GetString("breeze1") };
        private static readonly string[] gale = new string[] { loader.GetString("gale0"), loader.GetString("gale1") };
        private static readonly string[] storm = new string[] { loader.GetString("storm0") };
        private static readonly string[] thundershower = new string[] { loader.GetString("thundershower0") };
        private static readonly string[] hail = new string[] { loader.GetString("hail0") };
        private static readonly string[] moderate_rain = new string[] { loader.GetString("moderate_rain0"), loader.GetString("moderate_rain1"), loader.GetString("moderate_rain2"), loader.GetString("moderate_rain3") };
        private static readonly string[] heavy_rain = new string[] { loader.GetString("heavy_rain0"), loader.GetString("heavy_rain1") };
        private static readonly string[] heavy_storm_rain = new string[] { loader.GetString("heavy_storm_rain0"), loader.GetString("heavy_storm_rain1") };
        private static readonly string[] shower = new string[] { loader.GetString("shower0"), loader.GetString("shower1") };
        private static readonly string[] freezing_rain = new string[] { loader.GetString("freezing_rain0") };
        private static readonly string[] moderate_snow = new string[] { loader.GetString("moderate_snow0"), loader.GetString("moderate_snow1"), loader.GetString("moderate_snow2") };
        private static readonly string[] heavy_snow = new string[] { loader.GetString("heavy_snow0"), loader.GetString("heavy_snow1") };
        private static readonly string[] shower_snow = new string[] { loader.GetString("shower_snow0"), loader.GetString("shower_snow1") };
        private static readonly string[] snowstorm = new string[] { loader.GetString("snowstorm0"), loader.GetString("snowstorm1") };
        private static readonly string[] sleet = new string[] { loader.GetString("sleet0"), loader.GetString("sleet1") };
        private static readonly string[] mist = new string[] { loader.GetString("mist0") };
        private static readonly string[] foggy = new string[] { loader.GetString("foggy0") };
        private static readonly string[] haze = new string[] { loader.GetString("haze0") };
        private static readonly string[] dust = new string[] { loader.GetString("dust0") };
        private static readonly string[] duststorm = new string[] { loader.GetString("duststorm0") };
        #endregion
        #region Condition decoration
        private static readonly string[] haotianqi = new string[] { loader.GetString("haotianqi0"), loader.GetString("haotianqi1") };
        private static readonly string[] huaitianqi = new string[] { loader.GetString("huaitianqi0"), loader.GetString("huaitianqi1") };
        private static readonly string[] shiduda = new string[] { loader.GetString("shiduda0"), loader.GetString("shiduda1") };
        private static readonly string[] nengjiandudi = new string[] { loader.GetString("nengjiandudi0"), loader.GetString("nengjiandudi1") };
        private static readonly string[] wendugao = new string[] { loader.GetString("wendugao0"), loader.GetString("wendugao1") };
        private static readonly string[] wendudi = new string[] { loader.GetString("wendudi0"), loader.GetString("wendudi1") };
        #endregion
        public static string GenerateShortDescription(HeWeatherModel model, bool isNight)
        {
            var todayIndex = Array.FindIndex(model.DailyForecast, x =>
            {
                return x.Date.Date == DateTime.Today.Date;
            });
            if (todayIndex < 0)
            {
                todayIndex = 0;
            }
            var 主语 = model.NowWeather.Now.Condition;
            string zhuyu = loader.GetString("xianzai");
            if (Tools.Random.Next(200) >= 100)
            {
                if (isNight)
                {
                    主语 = model.DailyForecast[todayIndex].Condition.NightCond;
                    zhuyu = loader.GetString("wanjian");
                }
                else
                {
                    主语 = model.DailyForecast[todayIndex].Condition.DayCond;
                    zhuyu = loader.GetString("jintian");
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
                if (model.NowWeather.Visibility != null)
                    if (Tools.Random.Next(200) >= 100 && model.NowWeather.Visibility.KM < 0.5)
                    {
                        decoration.Append(nengjiandudi[0] + ", ");
                    }
                if (Tools.Random.Next(200) >= 100)
                    switch (主语)
                    {
                        case WeatherCondition.unknown:
                            return loader.GetString("unknown_weather");
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
                    return loader.GetString("unknown_weather");
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
                    binyu = loader.GetString("bianre");
                    break;
                case WeatherCondition.cold:
                    binyu = loader.GetString("bianleng");
                    break;
                default: return loader.GetString("unknown_weather");
            }
            return string.Format(loader.GetString("Glance"), zhuyu, binyu, decoration);
        }

        public static string GenerateGlanceDescription(HeWeatherModel model, bool isNight, TemperatureParameter parameter, DateTime desiredDate)
        {
            var todayIndex = Array.FindIndex(model.DailyForecast, x =>
            {
                return x.Date.Date == desiredDate.Date;
            });
            if (todayIndex < 0)
            {
                todayIndex = 0;
            }
            var 主语 = model.NowWeather.Now.Condition;
            string zhuyu = loader.GetString("xianzai");
            if (Tools.Random.Next(200) >= 100)
            {
                if (isNight)
                {
                    主语 = model.DailyForecast[todayIndex].Condition.NightCond;
                    zhuyu = loader.GetString("wanjian");
                }
                else
                {
                    主语 = model.DailyForecast[todayIndex].Condition.DayCond;
                    zhuyu = loader.GetString("jintian");
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
                if (model.NowWeather.Visibility != null)
                    if (Tools.Random.Next(200) >= 100 && model.NowWeather.Visibility.KM < 0.5)
                    {
                        decoration.Append(nengjiandudi[0] + ", ");
                    }
                if (Tools.Random.Next(200) >= 100)
                    switch (主语)
                    {
                        case WeatherCondition.unknown:
                            return loader.GetString("unknown_weather");
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
                    return loader.GetString("unknown_weather");
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
                    binyu = loader.GetString("bianre");
                    break;
                case WeatherCondition.cold:
                    binyu = loader.GetString("bianleng");
                    break;
                default: return loader.GetString("unknown_weather");
            }
            string nowtemp;
            string tomorrowtemp;
            switch (parameter)
            {
                case TemperatureParameter.Celsius:
                    nowtemp = model.DailyForecast[todayIndex].LowTemp.Celsius + "°~" + model.DailyForecast[todayIndex].HighTemp.Celsius + '°';
                    tomorrowtemp = model.DailyForecast[todayIndex + 1].LowTemp.Celsius + "°~" + model.DailyForecast[todayIndex + 1].HighTemp.Celsius + '°';
                    break;
                case TemperatureParameter.Fahrenheit:
                    nowtemp = model.DailyForecast[todayIndex + 0].LowTemp.Fahrenheit + "°~" + model.DailyForecast[todayIndex + 0].HighTemp.Fahrenheit + '°';
                    tomorrowtemp = model.DailyForecast[todayIndex + 1].LowTemp.Fahrenheit + "°~" + model.DailyForecast[todayIndex + 1].HighTemp.Fahrenheit + '°';
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
            return string.Format(loader.GetString("GlanceFull"), zhuyu, binyu, decoration, nowtemp, tomorrowcondition, tomorrowtemp);
        }
    }
}
