using Com.Aurora.AuWeather.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Com.Aurora.AuWeather.ViewModels
{
    class NowWeatherViewModel : WeatherViewModel
    {
        private DateTime updateTime;
        private TempratureRange nowTempRange;

        public DateTime UpdateTime
        {
            get
            {
                return updateTime;
            }

            set
            {
                SetProperty(ref updateTime, value);
            }
        }

        internal TempratureRange NowTempRange
        {
            get
            {
                return nowTempRange;
            }

            set
            {
                SetProperty(ref nowTempRange, value);
            }
        }

        public NowWeatherViewModel()
        {

        }
    }
}
