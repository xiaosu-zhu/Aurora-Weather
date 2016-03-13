using System;

namespace Com.Aurora.AuWeather.Models
{
    public class Temperature
    {
        private float temprature;

        public int Celsius
        {
            get
            {
                return Convert.ToInt32((temprature - 273.15));
            }
            set
            {
                temprature = value + 273.15f > 0 ? value + 273.15f : 0;
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
                temprature = ((value - 32) * 5 / 9 + 273.15f) > 0 ? ((value - 32) * 5 / 9 + 273.15f) : 0;
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

        public static Temperature FromCelsius(int celsius)
        {
            Temperature t = new Temperature();
            t.Celsius = celsius;
            return t;
        }
        public static Temperature FromFahrenheit(int fahrenheit)
        {
            Temperature t = new Temperature();
            t.Fahrenheit = fahrenheit;
            return t;
        }
        public static Temperature FromKelvin(int kelvin)
        {
            Temperature t = new Temperature();
            t.Kelvin = kelvin;
            return t;
        }

        internal string Actual(TemperatureParameter temperatureParameter)
        {
            switch (temperatureParameter)
            {
                case TemperatureParameter.Celsius:
                    return Celsius.ToString("0") + '°';
                case TemperatureParameter.Fahrenheit:
                    return Fahrenheit.ToString("0") + '°';
                case TemperatureParameter.Kelvin:
                    return Kelvin.ToString("0");
                default:
                    return Celsius.ToString("0") + '°';
            }
        }
    }
}
