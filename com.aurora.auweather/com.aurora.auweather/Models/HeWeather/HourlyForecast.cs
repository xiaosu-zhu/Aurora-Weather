using System;
using System.Globalization;

namespace Com.Aurora.AuWeather.Models.HeWeather
{
    internal class HourlyForecast
    {
        private DateTime dateTime;
        private uint humidity;
        private uint pop;
        private uint pressure;
        private Temprature temprature;
        private Wind wind;

        public DateTime DateTime
        {
            get
            {
                return dateTime;
            }

            set
            {
                dateTime = value;
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

        public uint Pressure
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

        public Temprature Temprature
        {
            get
            {
                return temprature;
            }

            set
            {
                temprature = value;
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

        public HourlyForecast(JsonContract.HourlyForecastContract hourly_forecast)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime = DateTime.ParseExact(hourly_forecast.date, "yyyy-MM-dd HH:mm", provider);
            Humidity = uint.Parse(hourly_forecast.hum);
            Pop = uint.Parse(hourly_forecast.pop);
            Pressure = uint.Parse(hourly_forecast.pres);
            Temprature = Temprature.FromCelsius(int.Parse(hourly_forecast.tmp));
            Wind = new Wind(hourly_forecast.wind);
        }
    }
}