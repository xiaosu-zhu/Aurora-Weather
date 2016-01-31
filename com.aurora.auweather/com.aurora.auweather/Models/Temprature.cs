using System;

namespace Com.Aurora.AuWeather.Models
{
    public class Temprature
    {
        private double temprature;

        public int Celsius
        {
            get
            {
                return Convert.ToInt32((temprature - 273.15));
            }
            set
            {
                temprature = value + 273.15 > 0 ? value + 273.15 : 0;
            }
        }
        public int Fahrenheit
        {
            get
            {
                return Convert.ToInt32((temprature - 273.15) * 9 / 5 + 32);
            }
            set
            {
                temprature = ((value - 32) * 5 / 9 + 273.15) > 0 ? ((value - 32) * 5 / 9 + 273.15) : 0;
            }
        }
        public int Kelvin
        {
            get
            {
                return (int)temprature;
            }
            set
            {
                temprature = value > 0 ? value : 0;
            }
        }

        public static Temprature FromCelsius(int celsius)
        {
            Temprature t = new Temprature();
            t.Celsius = celsius;
            return t;
        }
        public static Temprature FromFahrenheit(int fahrenheit)
        {
            Temprature t = new Temprature();
            t.Fahrenheit = fahrenheit;
            return t;
        }
        public static Temprature FromKelvin(int kelvin)
        {
            Temprature t = new Temprature();
            t.Kelvin = kelvin;
            return t;
        }
    }
}
