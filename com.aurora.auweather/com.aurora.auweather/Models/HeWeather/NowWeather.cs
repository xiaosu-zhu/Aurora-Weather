namespace Com.Aurora.AuWeather.Models.HeWeather
{
    internal class NowWeather
    {
        private NowCondition now;
        private Temprature bodyTemprature;
        private float precipitation;
        private Length visibility;
        private Wind wind;
        private Pressure pressure;
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

        public float Precipitation
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

        public Length Visibility
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

        public Pressure Pressure
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
            Precipitation = float.Parse(now.pcpn);
            Visibility = Length.FromKM(float.Parse(now.vis));
            Wind = new Wind(now.wind);
            Pressure = Pressure.FromHPa(float.Parse(now.pres));
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