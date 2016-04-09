// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Com.Aurora.AuWeather.Models
{
    public class Temperature
    {
        private float temperature;

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

        public static Temperature FromCelsius(float celsius)
        {
            Temperature t = new Temperature();
            t.temperature = celsius + 273.15f > 0 ? celsius + 273.15f : 0; ;
            return t;
        }
        public static Temperature FromFahrenheit(float fahrenheit)
        {
            Temperature t = new Temperature();
            t.temperature = (fahrenheit - 32) * 5 / 9 + 273.15f > 0 ? ((fahrenheit - 32) * 5 / 9 + 273.15f) : 0;
            return t;
        }
        public static Temperature FromKelvin(float kelvin)
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

        public static Temperature operator /(Temperature left, float right)
        {
            var t = new Temperature();
            t.temperature = left.temperature / right;
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
