// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Globalization;
using Com.Aurora.AuWeather.Core.Models.Caiyun.JsonContract;
using Com.Aurora.AuWeather.Core.Models.WunderGround.JsonContract;

namespace Com.Aurora.AuWeather.Models.HeWeather
{


    public class DailyForecast
    {
        /// <summary>
        /// 预报的日期 yyyy-MM-dd
        /// </summary>
        public DateTime Date
        {
            get; private set;
        }

        /// <summary>
        /// 日出时刻 HH:mm
        /// </summary>
        public TimeSpan SunRise
        {
            get; private set;
        }

        /// <summary>
        /// 日落时刻 HH:mm
        /// </summary>
        public TimeSpan SunSet
        {
            get; private set;
        }

        public DailyCondition Condition
        {
            get; private set;
        }

        public uint Humidity
        {
            get; private set;
        }

        public float Precipitation
        {
            get; private set;
        }

        public uint Pop
        {
            get; private set;
        }

        public Pressure Pressure
        {
            get; private set;
        }

        public Temperature HighTemp
        {
            get; private set;
        }

        public Temperature LowTemp
        {
            get; private set;
        }

        public Length Visibility
        {
            get; private set;
        }

        public Wind Wind
        {
            get; private set;
        }

        public DailyForecast(JsonContract.DailyForecastContract daily_forecast)
        {
            if (daily_forecast == null)
            {
                return;
            }
            CultureInfo provider = CultureInfo.InvariantCulture;
            Date = DateTime.ParseExact(daily_forecast.date, "yyyy-MM-dd", provider);
            SunRise = TimeSpan.Parse(daily_forecast.astro.sr);
            SunSet = TimeSpan.Parse(daily_forecast.astro.ss);
            Condition = new DailyCondition(daily_forecast.cond);
            Humidity = uint.Parse(daily_forecast.hum);
            Precipitation = float.Parse(daily_forecast.pcpn);
            Pop = uint.Parse(daily_forecast.pop);
            Pressure = Pressure.FromHPa(float.Parse(daily_forecast.pres));
            HighTemp = Temperature.FromCelsius(int.Parse(daily_forecast.tmp.max));
            LowTemp = Temperature.FromCelsius(int.Parse(daily_forecast.tmp.min));
            Visibility = Length.FromKM(float.Parse(daily_forecast.vis));
            Wind = new Wind(daily_forecast.wind);
        }

        public DailyForecast(Skycon skycon, Range temperature, Range humidity, Range precipitation, RangeWind wind, Astro astro)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            Date = DateTime.ParseExact(temperature.date, "yyyy-MM-dd", provider);
            SunRise = TimeSpan.Parse(astro.sunrise.time);
            SunSet = TimeSpan.Parse(astro.sunset.time);
            Condition = new DailyCondition(skycon);
            HighTemp = Temperature.FromCelsius((float)temperature.max);
            LowTemp = Temperature.FromCelsius((float)temperature.min);
            Humidity = (uint)(humidity.avg * 100);
            Precipitation = (uint)precipitation.avg;
            Wind = new Wind(wind.avg);
        }

        public DailyForecast(forecastday daily)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            Date = new DateTime(daily.date.year, daily.date.month, daily.date.day);
            Condition = new DailyCondition(daily.icon);
            float m;
            if (float.TryParse(daily.high.celsius, out m))
            {
                HighTemp = Temperature.FromCelsius(m);
            }
            if (float.TryParse(daily.low.celsius, out m))
            {
                LowTemp = Temperature.FromCelsius(m);
            }
            Humidity = Convert.ToUInt32(daily.avehumidity);
            Precipitation = Convert.ToSingle(daily.qpf_allday.mm);
            Wind = new Wind(Convert.ToUInt32(daily.avewind.kph), Convert.ToUInt32(daily.avewind.degrees));
        }
    }



    public class DailyCondition : Condition
    {
        public WeatherCondition DayCond
        {
            get; private set;
        }

        public WeatherCondition NightCond
        {
            get; private set;
        }

        public DailyCondition(JsonContract.ConditionContract cond)
        {
            DayCond = ParseCondition(cond.code_d);
            NightCond = ParseCondition(cond.code_n);
        }

        public DailyCondition(Skycon skycon)
        {
            DayCond = ParseCondition_C(skycon.value);
            NightCond = DayCond;
        }

        public DailyCondition(string icon)
        {
            DayCond = ParseCondition_W(icon);
            NightCond = DayCond;
        }
    }
}