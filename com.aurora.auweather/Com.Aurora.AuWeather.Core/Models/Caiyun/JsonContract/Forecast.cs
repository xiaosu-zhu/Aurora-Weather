using System.Runtime.Serialization;

namespace Com.Aurora.AuWeather.Core.Models.Caiyun.JsonContract
{
    [DataContract]
    public class Forecast
    {
        [DataMember]
        public string status;
        [DataMember]
        public string lang;
        [DataMember]
        public long server_time;
        [DataMember]
        public int tzshift;
        [DataMember]
        public double[] location;
        [DataMember]
        public string unit;
        [DataMember]
        public ForecastResult result;
    }

    [DataContract]
    public class ForecastResult
    {
        [DataMember]
        public Hourly hourly;
        [DataMember]
        public Minutely minutely;
        [DataMember]
        public Daily daily;
    }

    [DataContract]
    public class Daily
    {
        [DataMember]
        public string status;
        [DataMember]
        public Astro[] astro;
        [DataMember]
        public Range[] temperature;
        [DataMember]
        public Range[] pm25;
        [DataMember]
        public Skycon[] skycon;
        [DataMember]
        public Range[] cloudrate;
        [DataMember]
        public Range[] aqi;
        [DataMember]
        public Range[] precipitation;
        [DataMember]
        public RangeWind[] wind;
        [DataMember]
        public Range[] humidity;
    }

    [DataContract]
    public class Hourly
    {
        [DataMember]
        public string status;
        [DataMember]
        public string description;
        [DataMember]
        public Value[] pm25;
        [DataMember]
        public Skycon[] skycon;
        [DataMember]
        public Value[] cloudrate;
        [DataMember]
        public Value[] aqi;
        [DataMember]
        public Value[] humidity;
        [DataMember]
        public Value[] precipitation;
        [DataMember]
        public Wind[] wind;
        [DataMember]
        public Value[] temperature;
    }

    [DataContract]
    public class Minutely
    {
        [DataMember]
        public string status;
        [DataMember]
        public string description;
        [DataMember]
        public double[] precipitation;
        [DataMember]
        public string datasource;
    }

    [DataContract]
    public class Value
    {
        [DataMember]
        public double value;
        [DataMember]
        public string datetime;
    }

    [DataContract]
    public class Range
    {
        [DataMember]
        public string date;
        [DataMember]
        public double max;
        [DataMember]
        public double avg;
        [DataMember]
        public double min;
    }

    [DataContract]
    public class Skycon
    {
        [DataMember]
        public string value;
        [DataMember]
        public string datetime;
    }

    [DataContract]
    public class Astro
    {
        [DataMember]
        public string date;
        [DataMember]
        public Time sunset;
        [DataMember]
        public Time sunrise;
    }

    [DataContract]
    public class Time
    {
        [DataMember]
        public string time;
    }

    [DataContract]
    public class Wind
    {
        [DataMember]
        public double direction;
        [DataMember]
        public double speed;
        [DataMember]
        public string datetime;
    }
    [DataContract]
    public class RangeWind
    {
        [DataMember]
        public WindTotal max;
        [DataMember]
        public WindTotal min;
        [DataMember]
        public WindTotal avg;
        [DataMember]
        public string date;
    }
}
