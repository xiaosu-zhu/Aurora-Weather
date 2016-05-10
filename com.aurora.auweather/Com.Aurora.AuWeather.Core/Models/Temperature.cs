// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Com.Aurora.AuWeather.Models
{
    public class Temperature
    {
        private double temperature;

        public int Celsius
        {
            get
            {
                return Convert.ToInt32((temperature - 273.15));
            }
            set
            {
                temperature = value + 273.15f > 0 ? value + 273.15f : 0;
            }
        }
        public int Fahrenheit
        {
            get
            {
                return Convert.ToInt32((temperature - 273.15) * 9 / 5 + 32);
            }
            set
            {
                temperature = (value - 32) * 5 / 9 + 273.15f > 0 ? ((value - 32) * 5 / 9 + 273.15f) : 0;
            }
        }
        public int Kelvin
        {
            get
            {
                return (int)temperature;
            }
            set
            {
                temperature = value > 0 ? value : 0;
            }
        }

        public static Temperature FromCelsius(double celsius)
        {
            Temperature t = new Temperature();
            t.temperature = celsius + 273.15f > 0 ? celsius + 273.15f : 0; ;
            return t;
        }
        public static Temperature FromFahrenheit(double fahrenheit)
        {
            Temperature t = new Temperature();
            t.temperature = (fahrenheit - 32) * 5 / 9 + 273.15f > 0 ? ((fahrenheit - 32) * 5 / 9 + 273.15f) : 0;
            return t;
        }
        public static Temperature FromKelvin(double kelvin)
        {
            Temperature t = new Temperature();
            t.temperature = kelvin;
            return t;
        }

        public static Temperature operator +(Temperature left, Temperature right)
        {
            var t = new Temperature();
            t.temperature = left.temperature + right.temperature;
            return t;
        }

        public static Temperature operator -(Temperature left, Temperature right)
        {
            var t = new Temperature();
            t.temperature = left.temperature - right.temperature;
            return t;
        }

        public static Temperature operator *(Temperature left, Temperature right)
        {
            var t = new Temperature();
            t.temperature = left.temperature * right.temperature;
            return t;
        }

        public static Temperature operator /(Temperature left, Temperature right)
        {
            var t = new Temperature();
            t.temperature = left.temperature / right.temperature;
            return t;
        }

        public static Temperature operator /(Temperature left, double right)
        {
            var t = new Temperature();
            t.temperature = left.temperature / right;
            return t;
        }

        public string Actual(TemperatureParameter temperatureParameter)
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

        public double ActualDouble(TemperatureParameter parameter)
        {
            switch (parameter)
            {
                case TemperatureParameter.Celsius:
                    return Celsius;
                case TemperatureParameter.Fahrenheit:
                    return Fahrenheit;
                case TemperatureParameter.Kelvin:
                    return Kelvin;
                default:
                    return Celsius;
            }
        }

        public static string GetFormat(TemperatureParameter parameter)
        {
            switch (parameter)
            {
                case TemperatureParameter.Celsius:
                case TemperatureParameter.Fahrenheit:
                    return "°";
                case TemperatureParameter.Kelvin:
                    return "K";
                default:
                    return "°";
            }
        }
    }
}
