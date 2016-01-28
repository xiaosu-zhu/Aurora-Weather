namespace com.aurora.auweather.Models.HeWeather
{
    internal class NowWeather
    {
        private NowCondition now;
        private Temprature bodyTemprature;
        private uint precipitation;
        private uint visibility;
        private Wind wind;
        private uint pressure;
        private Temprature temprature;

        public NowCondition Now
        {
            get
            {
                return now;
            }

            set
            {
                now = value;
            }
        }

        public Temprature BodyTemprature
        {
            get
            {
                return bodyTemprature;
            }

            set
            {
                bodyTemprature = value;
            }
        }

        public uint Precipitation
        {
            get
            {
                return precipitation;
            }

            set
            {
                precipitation = value;
            }
        }

        public uint Visibility
        {
            get
            {
                return visibility;
            }

            set
            {
                visibility = value;
            }
        }

        public Wind Wind
        {
            get
            {
                return wind;
            }

            set
            {
                wind = value;
            }
        }

        public uint Pressure
        {
            get
            {
                return pressure;
            }

            set
            {
                pressure = value;
            }
        }

        public Temprature Temprature
        {
            get
            {
                return temprature;
            }

            set
            {
                temprature = value;
            }
        }

        public NowWeather(JsonContract.NowWeatherContract now)
        {
            Now = new NowCondition(now.cond);
            BodyTemprature = Temprature.FromCelsius(int.Parse(now.fl));
            Precipitation = uint.Parse(now.pcpn);
            Visibility = uint.Parse(now.vis);
            Wind = new Wind(now.wind);
            Pressure = uint.Parse(now.pres);
            Temprature = Temprature.FromCelsius(int.Parse(now.tmp));
        }
    }

    internal class NowCondition : Condition
    {
        private WeatherCondition condition;

        public WeatherCondition Condition
        {
            get
            {
                return condition;
            }

            set
            {
                condition = value;
            }
        }
        public NowCondition(JsonContract.Condition_NowContract cond)
        {
            Condition = ParseCondition(cond.code);
        }
    }
}