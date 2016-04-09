// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Com.Aurora.AuWeather.Models.HeWeather
{
    public class Condition
    {
        protected static WeatherCondition ParseCondition(string code_s)
        {
            switch (code_s)
            {
                default: return WeatherCondition.unknown;
                case "100": return WeatherCondition.sunny;
                case "101": return WeatherCondition.cloudy;
                case "102": return WeatherCondition.few_clouds;
                case "103": return WeatherCondition.partly_cloudy;
                case "104": return WeatherCondition.overcast;
                case "200": return WeatherCondition.windy;
                case "201": return WeatherCondition.calm;
                case "202": return WeatherCondition.light_breeze;
                case "203": return WeatherCondition.moderate;
                case "204": return WeatherCondition.fresh_breeze;
                case "205": return WeatherCondition.strong_breeze;
                case "206": return WeatherCondition.high_wind;
                case "207": return WeatherCondition.gale;
                case "208": return WeatherCondition.strong_gale;
                case "209": return WeatherCondition.storm;
                case "210": return WeatherCondition.violent_storm;
                case "211": return WeatherCondition.hurricane;
                case "212": return WeatherCondition.tornado;
                case "213": return WeatherCondition.tropical_storm;
                case "300": return WeatherCondition.shower_rain;
                case "301": return WeatherCondition.heavy_shower_rain;
                case "302": return WeatherCondition.thundershower;
                case "303": return WeatherCondition.heavy_thunderstorm;
                case "304": return WeatherCondition.hail;
                case "305": return WeatherCondition.light_rain;
                case "306": return WeatherCondition.moderate_rain;
                case "307": return WeatherCondition.heavy_rain;
                case "308": return WeatherCondition.extreme_rain;
                case "309": return WeatherCondition.drizzle_rain;
                case "310": return WeatherCondition.storm_rain;
                case "311": return WeatherCondition.heavy_storm_rain;
                case "312": return WeatherCondition.severe_storm_rain;
                case "313": return WeatherCondition.freezing_rain;
                case "400": return WeatherCondition.light_snow;
                case "401": return WeatherCondition.moderate_snow;
                case "402": return WeatherCondition.heavy_snow;
                case "403": return WeatherCondition.snowstorm;
                case "404": return WeatherCondition.sleet;
                case "405": return WeatherCondition.rain_snow;
                case "406": return WeatherCondition.shower_snow;
                case "407": return WeatherCondition.snow_flurry;
                case "500": return WeatherCondition.mist;
                case "501": return WeatherCondition.foggy;
                case "502": return WeatherCondition.haze;
                case "503": return WeatherCondition.sand;
                case "504": return WeatherCondition.dust;
                case "506": return WeatherCondition.volcanic_ash;
                case "507": return WeatherCondition.duststorm;
                case "508": return WeatherCondition.sandstorm;
                case "900": return WeatherCondition.hot;
                case "901": return WeatherCondition.cold;
                case "999": return WeatherCondition.unknown;
            }
        }

        protected static WeatherCondition ParseCondition_C(string skycon)
        {
            //CLEAR_DAY：晴天
            //CLEAR_NIGHT：晴夜
            //PARTLY_CLOUDY_DAY：多云
            //PARTLY_CLOUDY_NIGHT：多云
            //CLOUDY：阴
            //RAIN： 雨
            //SLEET：冻雨
            //SNOW：雪
            //WIND：风
            //FOG：雾
            //HAZE：霾

            switch (skycon)
            {
                case "CLEAR_DAY":
                case "CLEAR_NIGHT":
                    return WeatherCondition.sunny;
                case "PARTLY_CLOUDY_DAY":
                case "PARTLY_CLOUDY_NIGHT":
                    return WeatherCondition.cloudy;
                case "CLOUDY":
                    return WeatherCondition.overcast;
                case "RAIN":
                    return WeatherCondition.light_rain;
                case "SLEET":
                    return WeatherCondition.sleet;
                case "SNOW":
                    return WeatherCondition.light_snow;
                case "WIND":
                    return WeatherCondition.windy;
                case "FOG":
                    return WeatherCondition.foggy;
                case "HAZE":
                    return WeatherCondition.haze;
                default:
                    return WeatherCondition.unknown;
            }
        }
    }
}