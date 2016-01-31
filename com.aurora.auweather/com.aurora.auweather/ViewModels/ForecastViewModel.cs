using Com.Aurora.AuWeather.Models;
using System;
using System.ComponentModel;

namespace Com.Aurora.AuWeather.ViewModels
{
    internal class ForecastViewModel : WeatherViewModel
    {
        private DateTime stamp;

        public DateTime Stamp
        {
            get
            {
                return stamp;
            }

            set
            {
                SetProperty(ref stamp, value);
            }
        }

    }
}