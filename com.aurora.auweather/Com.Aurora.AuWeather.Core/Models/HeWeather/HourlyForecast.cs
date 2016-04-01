// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Globalization;

namespace Com.Aurora.AuWeather.Models.HeWeather
{
    public class HourlyForecast
    {
        public DateTime DateTime
        {
            get; private set;
        }

        public uint Humidity
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

        public Temperature Temprature
        {
            get; private set;
        }

        public Wind Wind
        {
            get; private set;
        }

        public HourlyForecast(JsonContract.HourlyForecastContract hourly_forecast)
        {
            if (hourly_forecast == null)
            {
                return;
            }
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime = DateTime.ParseExact(hourly_forecast.date, "yyyy-MM-dd HH:mm", provider);
            Humidity = uint.Parse(hourly_forecast.hum);
            Pop = uint.Parse(hourly_forecast.pop);
            Pressure = Pressure.FromHPa(float.Parse(hourly_forecast.pres));
            Temprature = Temperature.FromCelsius(int.Parse(hourly_forecast.tmp));
            Wind = new Wind(hourly_forecast.wind);
        }
    }
}