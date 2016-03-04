namespace Com.Aurora.AuWeather.Models
{

    public class AQI
    {

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
            get; private set;
        }

        public uint Co
        {
            get; private set;
        }

        public uint No2
        {
            get; private set;
        }

        public uint O3
        {
            get; private set;
        }

        public uint Pm10
        {
            get; private set;
        }

        public uint Pm25
        {
            get; private set;
        }

        public uint So2
        {
            get; private set;
        }

        internal AQIQuality Qlty
        {
            get; private set;
        }
    }
}