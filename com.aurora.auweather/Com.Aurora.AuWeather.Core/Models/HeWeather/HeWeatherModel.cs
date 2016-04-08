// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.Shared.Extensions;
using System;
using System.Collections.Generic;
using Com.Aurora.AuWeather.Core.Models.Caiyun.JsonContract;

namespace Com.Aurora.AuWeather.Models.HeWeather
{
    public enum HeWeatherStatus : byte { ok, invalid_key, unknown_city, no_more_requests, no_response, permission_denied };
    public class HeWeatherModel : WeatherModel
    {

        public HeWeatherStatus Status
        {
            get; private set;
        }

        public AQI Aqi
        {
            get; private set;
        }

        public DailyForecast[] DailyForecast
        {
            get; private set;
        }

        public HourlyForecast[] HourlyForecast
        {
            get; private set;
        }

        public Location Location
        {
            get; private set;
        }

        public NowWeather NowWeather
        {
            get; private set;
        }

        public WeatherSuggestion WeatherSuggestion
        {
            get; private set;
        }

        public WeatherAlarm[] Alarms
        {
            get; private set;
        }

        private static HeWeatherStatus ParseStatus(string status_s)
        {
            switch (status_s)
            {
                case "invalid key": return HeWeatherStatus.invalid_key;
                case "unknown city": return HeWeatherStatus.unknown_city;
                case "ok": return HeWeatherStatus.ok;
                case "anr": return HeWeatherStatus.no_response;
                case "no more requests": return HeWeatherStatus.no_response;
                case "permission denied": return HeWeatherStatus.permission_denied;
                default: return HeWeatherStatus.invalid_key;
            }
        }

        private static HeWeatherStatus ParseStatus_C(string status_s)
        {
            if (status_s == "ok")
            {
                return HeWeatherStatus.ok;
            }
            else
            {
                return HeWeatherStatus.invalid_key;
            }
        }

        public HeWeatherModel(JsonContract.HeWeatherContract heweathercontract)
        {
            if (heweathercontract == null)
                throw new ArgumentException();
            Status = ParseStatus(heweathercontract.status);
            Aqi = new AQI(heweathercontract.aqi);
            DailyForecast = GenerateDailyForecast(heweathercontract.daily_forecast);
            HourlyForecast = GenerateHourlyForecast(heweathercontract.hourly_forecast);
            Alarms = GenerateWeatherAlarms(heweathercontract.alarms);
            Location = new Location(heweathercontract.basic);
            NowWeather = new NowWeather(heweathercontract.now);
            WeatherSuggestion = new WeatherSuggestion(heweathercontract.suggestion);
        }

        public HeWeatherModel(Result result, Forecast forecast)
        {
            if (result != null)
            {
                Status = ParseStatus_C(result.status);
                NowWeather = new NowWeather(result.temperature, result.skycon, result.humidity, result.precipitation, result.wind);
            }
        }

        private WeatherAlarm[] GenerateWeatherAlarms(JsonContract.WeatherAlarmContract[] alarms)
        {
            if (!alarms.IsNullorEmpty())
            {
                try
                {
                    List<WeatherAlarm> _alarms = new List<WeatherAlarm>();
                    foreach (var alarm in alarms)
                    {
                        _alarms.Add(new WeatherAlarm(alarm));
                    }
                    return _alarms.ToArray();
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else return null;
        }

        private HourlyForecast[] GenerateHourlyForecast(JsonContract.HourlyForecastContract[] hourly_forecast)
        {
            if (!hourly_forecast.IsNullorEmpty())
            {
                List<HourlyForecast> hours = new List<HourlyForecast>();
                foreach (var hour in hourly_forecast)
                {
                    hours.Add(new HourlyForecast(hour));
                }
                return hours.ToArray();
            }
            else return null;
        }

        private DailyForecast[] GenerateDailyForecast(JsonContract.DailyForecastContract[] daily_forecast)
        {
            if (!daily_forecast.IsNullorEmpty())
            {
                List<DailyForecast> dailys = new List<DailyForecast>();
                foreach (var daily in daily_forecast)
                {
                    dailys.Add(new DailyForecast(daily));
                }
                return dailys.ToArray();
            }
            else return null;
        }
    }
}
