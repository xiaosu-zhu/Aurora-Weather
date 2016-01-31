using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Aurora.AuWeather.Models
{
    internal class TempratureRange
    {
        public Temprature Low { get; set; }
        public Temprature High { get; set; }

        public TempratureRange(Temprature low, Temprature high)
        {
            Low = low;
            High = high;
        }

        public static TempratureRange FromCelsius(int low, int high)
        {
            var Low = Temprature.FromCelsius(low);
            var High = Temprature.FromCelsius(high);
            return new TempratureRange(Low, High);
        }
        public static TempratureRange FromFahrenheit(int low, int high)
        {
            var Low = Temprature.FromFahrenheit(low);
            var High = Temprature.FromFahrenheit(high);
            return new TempratureRange(Low, High);
        }
        public static TempratureRange FromKelvin(int low, int high)
        {
            var Low = Temprature.FromKelvin(low);
            var High = Temprature.FromKelvin(high);
            return new TempratureRange(Low, High);
        }
    }
}
