﻿using System;
using System.Globalization;

namespace Com.Aurora.AuWeather.Models.HeWeather
{


    internal class DailyForecast
    {
        private DateTime date;
        private TimeSpan sunRise;
        private TimeSpan sunSet;
        private DailyCondition condition;
        private uint humidity;
        private float precipitation;
        private uint pop;
        private Pressure pressure;
        private Temprature highTemp;
        private Temprature lowTemp;
        private Length visibility;
        private Wind wind;

        /// <summary>
        /// 预报的日期 yyyy-MM-dd
        /// </summary>
        public DateTime Date
        {
            get
            {
                return date;
            }

            set
            {
                date = value;
            }
        }

        /// <summary>
        /// 日出时刻 HH:mm
        /// </summary>
        public TimeSpan SunRise
        {
            get
            {
                return sunRise;
            }

            set
            {
                sunRise = value;
            }
        }

        /// <summary>
        /// 日落时刻 HH:mm
        /// </summary>
        public TimeSpan SunSet
        {
            get
            {
                return sunSet;
            }

            set
            {
                sunSet = value;
            }
        }

        public DailyCondition Condition
        {
            get
            {
                return condition;
            }

            set
            {
                condition = value;
            }
        }

        public uint Humidity
        {
            get
            {
                return humidity;
            }

            set
            {
                humidity = value;
            }
        }

        public float Precipitation
        {
            get
            {
                return precipitation;
            }

            set
            {
                precipitation = value;
            }
        }

        public uint Pop
        {
            get
            {
                return pop;
            }

            set
            {
                pop = value;
            }
        }

        public Pressure Pressure
        {
            get
            {
                return pressure;
            }

            set
            {
                pressure = value;
            }
        }

        public Temprature HighTemp
        {
            get
            {
                return highTemp;
            }

            set
            {
                highTemp = value;
            }
        }

        public Temprature LowTemp
        {
            get
            {
                return lowTemp;
            }

            set
            {
                lowTemp = value;
            }
        }

        public Length Visibility
        {
            get
            {
                return visibility;
            }

            set
            {
                visibility = value;
            }
        }

        public Wind Wind
        {
            get
            {
                return wind;
            }

            set
            {
                wind = value;
            }
        }

        public DailyForecast(JsonContract.DailyForecastContract daily_forecast)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            Date = DateTime.ParseExact(daily_forecast.date, "yyyy-MM-dd", provider);
            SunRise = TimeSpan.Parse(daily_forecast.astro.sr);
            SunSet = TimeSpan.Parse(daily_forecast.astro.ss);
            Condition = new DailyCondition(daily_forecast.cond);
            Humidity = uint.Parse(daily_forecast.hum);
            Precipitation = float.Parse(daily_forecast.pcpn);
            Pop = uint.Parse(daily_forecast.pop);
            Pressure = Pressure.FromHPa(float.Parse(daily_forecast.pres));
            HighTemp = Temprature.FromCelsius(int.Parse(daily_forecast.tmp.max));
            LowTemp = Temprature.FromCelsius(int.Parse(daily_forecast.tmp.min));
            Visibility = Length.FromKM(float.Parse(daily_forecast.vis));
            Wind = new Wind(daily_forecast.wind);
        }
    }



    internal class DailyCondition : Condition
    {
        private WeatherCondition dayCond;
        private WeatherCondition nightCond;

        internal WeatherCondition DayCond
        {
            get
            {
                return dayCond;
            }

            set
            {
                dayCond = value;
            }
        }

        internal WeatherCondition NightCond
        {
            get
            {
                return nightCond;
            }

            set
            {
                nightCond = value;
            }
        }

        public DailyCondition(JsonContract.ConditionContract cond)
        {
            DayCond = ParseCondition(cond.code_d);
            NightCond = ParseCondition(cond.code_n);
        }

    }
}