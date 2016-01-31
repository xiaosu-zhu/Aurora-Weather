using System;

namespace Com.Aurora.AuWeather.Models
{

    internal class AQI
    {
        private uint aqi;
        private uint co;
        private uint no2;
        private uint o3;
        private uint pm10;
        private uint pm25;
        private AQIQuality qlty;
        private uint so2;

        public AQI(HeWeather.JsonContract.AQICpntract aqi)
        {
            Aqi = uint.Parse(aqi.city.aqi);
            Co = uint.Parse(aqi.city.co);
            No2 = uint.Parse(aqi.city.no2);
            O3 = uint.Parse(aqi.city.o3);
            Pm10 = uint.Parse(aqi.city.pm10);
            Pm25 = uint.Parse(aqi.city.pm25);
            Qlty = ParseQlty(aqi.city.qlty);
            So2 = uint.Parse(aqi.city.so2);
        }

        private static AQIQuality ParseQlty(string qlty)
        {
            switch (qlty)
            {
                default: return AQIQuality.unknown;
                case "优": return AQIQuality.one;
                case "良": return AQIQuality.two;
                case "轻度污染": return AQIQuality.three;
                case "中度污染": return AQIQuality.four;
                case "重度污染": return AQIQuality.five;
                case "严重污染": return AQIQuality.six;
            }
        }

        public uint Aqi
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

        public uint Co
        {
            get
            {
                return co;
            }

            set
            {
                co = value;
            }
        }

        public uint No2
        {
            get
            {
                return no2;
            }

            set
            {
                no2 = value;
            }
        }

        public uint O3
        {
            get
            {
                return o3;
            }

            set
            {
                o3 = value;
            }
        }

        public uint Pm10
        {
            get
            {
                return pm10;
            }

            set
            {
                pm10 = value;
            }
        }

        public uint Pm25
        {
            get
            {
                return pm25;
            }

            set
            {
                pm25 = value;
            }
        }

        public uint So2
        {
            get
            {
                return so2;
            }

            set
            {
                so2 = value;
            }
        }

        internal AQIQuality Qlty
        {
            get
            {
                return qlty;
            }

            set
            {
                qlty = value;
            }
        }
    }
}