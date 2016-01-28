using com.aurora.auweather.Models;
using System;
using System.ComponentModel;

namespace com.aurora.auweather.ViewModels
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