// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.Shared.Extensions;
using System;
using System.Collections.Generic;
using Com.Aurora.AuWeather.Core.Models.Caiyun.JsonContract;
using Com.Aurora.AuWeather.Models.HeWeather.JsonContract;
using Com.Aurora.AuWeather.Core.Models.WunderGround.JsonContract;

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
                return HeWeatherStatus.no_response;
            }
        }

        private static HeWeatherStatus ParseStatus_W(WunderGroundContract w)
        {
            if (w != null && w.response != null && w.current_observation != null)
            {
                return HeWeatherStatus.ok;
            }
            else
            {
                return HeWeatherStatus.no_response;
            }
        }

        public static HeWeatherModel Generate(string resstr, DataSource dataSource)
        {
            switch (dataSource)
            {
                case DataSource.HeWeather:
                    var he = HeWeatherContract.Generate(resstr);
                    try
                    {
                        return new HeWeatherModel(he);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                case DataSource.Caiyun:
                    var cai = CaiyunContract.Generate(resstr);
                    return new HeWeatherModel(cai.now, cai.forecast);
                case DataSource.Wunderground:
                    var wun = WunderGroundContract.Generate(resstr);
                    return new HeWeatherModel(wun);
                default:
                    return null;
            }
        }

        public HeWeatherModel(HeWeatherContract heweathercontract)
        {
            if (heweathercontract == null)
                throw new ArgumentException("Value can't be null.");
            Status = ParseStatus(heweathercontract.status);
            Aqi = new AQI(heweathercontract.aqi);
            DailyForecast = GenerateDailyForecast(heweathercontract.daily_forecast);
            HourlyForecast = GenerateHourlyForecast(heweathercontract.hourly_forecast);
            Alarms = GenerateWeatherAlarms(heweathercontract.alarms);
            Location = new Location(heweathercontract.basic);
            NowWeather = new NowWeather(heweathercontract.now);
            WeatherSuggestion = new WeatherSuggestion(heweathercontract.suggestion);
        }

        public HeWeatherModel(Now now, Forecast forecast)
        {
            if (now != null)
            {
                Status = ParseStatus_C(now.status);
                if (Status == HeWeatherStatus.ok)
                {
                    NowWeather = new NowWeather(now.result.temperature, now.result.skycon, now.result.precipitation, now.result.wind);
                    Location = new Location(now.location[0], now.location[1], now.server_time, now.tzshift);
                }
            }
            if (forecast != null)
            {
                if (forecast.status == "ok")
                {
                    Aqi = new AQI(forecast.result.hourly.aqi[0], forecast.result.hourly.pm25[0]);
                    DailyForecast = GenerateDailyForecast(forecast.result.daily);
                    HourlyForecast = GenerateHourlyForecast(forecast.result.hourly);
                }
            }
        }

        public HeWeatherModel(WunderGroundContract wun)
        {
            if (null == wun)
            {
                return;
            }
            Status = ParseStatus_W(wun);
            NowWeather = new NowWeather(wun.current_observation);
            DailyForecast = GenerateDailyForecast(wun.forecast.simpleforecast.forecastday);
            HourlyForecast = GenerateHourlyForecast(wun.hourly_forecast);
            Location = new Location(wun.current_observation);
        }


        private WeatherAlarm[] GenerateWeatherAlarms(WeatherAlarmContract[] alarms)
        {
            if (!alarms.IsNullorEmpty())
            {
                try
                {
                    List<WeatherAlarm> _alarms = new List<WeatherAlarm>();
                    foreach (var alarm in alarms)
                    {
                        if (!alarm.title.IsNullorEmpty())
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

        private HourlyForecast[] GenerateHourlyForecast(HourlyForecastContract[] hourly_forecast)
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

        private HourlyForecast[] GenerateHourlyForecast(Hourly hourly)
        {
            if (hourly.status == "ok")
            {
                List<HourlyForecast> hours = new List<HourlyForecast>();
                for (int i = 0; i < hourly.temperature.Length; i++)
                {
                    hours.Add(new HourlyForecast(hourly.temperature[i], hourly.precipitation[i], hourly.humidity[i], hourly.wind[i]));
                }
                return hours.ToArray();
            }
            return null;
        }

        private HourlyForecast[] GenerateHourlyForecast(hourly[] hourly_forecast)
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

        private DailyForecast[] GenerateDailyForecast(DailyForecastContract[] daily_forecast)
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

        private DailyForecast[] GenerateDailyForecast(Daily daily)
        {
            if (daily.status == "ok")
            {
                List<DailyForecast> days = new List<DailyForecast>();
                for (int i = 0; i < daily.temperature.Length; i++)
                {
                    days.Add(new DailyForecast(daily.skycon[i], daily.temperature[i], daily.humidity[i], daily.precipitation[i], daily.wind[i], daily.astro[i]));
                }
                return days.ToArray();
            }
            else return null;
        }


        private DailyForecast[] GenerateDailyForecast(forecastday[] forecast)
        {
            if (!forecast.IsNullorEmpty())
            {
                List<DailyForecast> dailys = new List<DailyForecast>();
                foreach (var daily in forecast)
                {
                    dailys.Add(new DailyForecast(daily));
                }
                return dailys.ToArray();
            }
            else return null;
        }
    }
}
