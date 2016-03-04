using Com.Aurora.Shared.Extensions;
using System;
using System.Collections.Generic;

namespace Com.Aurora.AuWeather.Models.HeWeather
{
    enum HeWeatherStatus : byte { ok, invalid_key, unknown_city, no_more_requests, no_response, permission_denied };
    internal class HeWeatherModel : WeatherModel, IWeather
    {

        public HeWeatherStatus Status
        {
            get; private set;
        }

        internal AQI Aqi
        {
            get; private set;
        }

        internal DailyForecast[] DailyForecast
        {
            get; private set;
        }

        internal HourlyForecast[] HourlyForecast
        {
            get; private set;
        }

        internal Location Location
        {
            get; private set;
        }

        internal NowWeather NowWeather
        {
            get; private set;
        }

        internal WeatherSuggestion WeatherSuggestion
        {
            get; private set;
        }

        internal WeatherAlarm[] Alarms
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
