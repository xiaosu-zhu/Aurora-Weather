using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Aurora.AuWeather.Models.HeWeather
{
    enum HeWeatherStatus : byte { ok, invalid_key, unknown_city, no_more_requests, no_response, permission_denied };
    internal class HeWeatherModel : WeatherModel, IWeather
    {
        private HeWeatherStatus status;
        private AQI aqi;
        private DailyForecast[] dailyForecast;
        private HourlyForecast[] hourlyForecast;
        private Location location;
        private NowWeather nowWeather;
        private WeatherSuggestion weatherSuggestion;
        private WeatherAlarm[] alarms;

        public HeWeatherStatus Status
        {
            get
            {
                return status;
            }

            set
            {
                status = value;
            }
        }

        internal AQI Aqi
        {
            get
            {
                return aqi;
            }

            set
            {
                aqi = value;
            }
        }

        internal DailyForecast[] DailyForecast
        {
            get
            {
                return dailyForecast;
            }

            set
            {
                dailyForecast = value;
            }
        }

        internal HourlyForecast[] HourlyForecast
        {
            get
            {
                return hourlyForecast;
            }

            set
            {
                hourlyForecast = value;
            }
        }

        internal Location Location
        {
            get
            {
                return location;
            }

            set
            {
                location = value;
            }
        }

        internal NowWeather NowWeather
        {
            get
            {
                return nowWeather;
            }

            set
            {
                nowWeather = value;
            }
        }

        internal WeatherSuggestion WeatherSuggestion
        {
            get
            {
                return weatherSuggestion;
            }

            set
            {
                weatherSuggestion = value;
            }
        }

        internal WeatherAlarm[] Alarms
        {
            get
            {
                return alarms;
            }

            set
            {
                alarms = value;
            }
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
            if (alarms != null && alarms.Length > 0)
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
            if (hourly_forecast != null && hourly_forecast.Length > 0)
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
            if (daily_forecast != null && daily_forecast.Length > 0)
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
