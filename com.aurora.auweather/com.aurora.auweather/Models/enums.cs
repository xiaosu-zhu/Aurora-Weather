using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Aurora.AuWeather.Models
{
    public enum WeatherCondition : byte
    {
        unknown, sunny, cloudy, few_clouds, partly_cloudy, overcast, windy, calm, light_breeze,
        moderate, fresh_breeze, strong_breeze, high_wind, gale, strong_gale, storm, violent_storm, hurricane, tornado,
        tropical_storm, shower_rain, heavy_shower_rain, thundershower, heavy_thunderstorm, hail, light_rain, moderate_rain,
        heavy_rain, extreme_rain, drizzle_rain, storm_rain, heavy_storm_rain, severe_storm_rain, freezing_rain,
        light_snow, moderate_snow, heavy_snow, snowstorm, sleet, rain_snow, shower_snow, snow_flurry, mist,
        foggy, haze, sand, dust, volcanic_ash, duststorm, sandstorm, hot, cold
    }
    public enum WindDirection : byte { unknown, north, east, west, south, northeast, northwest, southeast, southwest }
    public enum WindScale : byte
    {
        unknown, zero, one, two, three, four, five, six, seven, eight, nine, ten, eleven, twelve, thirteen, fourteen,
        fifteen, sixteen, seventeen, eighteen
    }
    public enum AQIQuality : byte { unknown, one, two, three, four, five, six }; //one 为优，six 为严重污染
    public enum WeatherAlarmType : byte
    {
        typhoon, rain_storm, snow_storm, cold_wave, gale, sand_storm, heat_wave,
        drought, lightning, hail, frost, fog, haze, road_icing, forest_fire, lightning_gale
    };
    public enum WeatherAlarmLevel : byte { blue, yellow, orange, red };

    public enum RefreshState : byte { none, daily, six_hour, twelve_hour };

    public enum RainLevel : byte { light, moderate, heavy, extreme, sSnow, lSnow };

    public enum WindParameter : byte { BeaufortandText, BeaufortandDegree, SpeedandText, SpeedandDegree }

    public enum SpeedParameter : byte { KMPH, MPS, Knot };
}
