using System;
using System.Globalization;

namespace Com.Aurora.AuWeather.Models.HeWeather
{


    internal class DailyForecast
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

        public Temprature HighTemp
        {
            get; private set;
        }

        public Temprature LowTemp
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
        internal WeatherCondition DayCond
        {
            get; private set;
        }

        internal WeatherCondition NightCond
        {
            get; private set;
        }

        public DailyCondition(JsonContract.ConditionContract cond)
        {
            DayCond = ParseCondition(cond.code_d);
            NightCond = ParseCondition(cond.code_n);
        }

    }
}