using System.Runtime.Serialization;

namespace Com.Aurora.AuWeather.Core.Models.WunderGround.JsonContract
{
    [DataContract]
    public class forecast
    {
        [DataMember]
        public simpleforecast simpleforecast;
    }

    [DataContract]
    public class simpleforecast
    {
        [DataMember]
        public forecastday[] forecastday;
    }

    [DataContract]
    public class forecastday
    {
        [DataMember]
        public date date;
        [DataMember]
        public int period;
        [DataMember]
        public temp high;
        [DataMember]
        public temp low;
        [DataMember]
        public string conditions;
        [DataMember]
        public string icon;
        [DataMember]
        public string icon_url;
        [DataMember]
        public string skyicon;
        [DataMember]
        public float pop;
        [DataMember]
        public quantitymm qpf_allday;
        [DataMember]
        public quantitymm qpf_day;
        [DataMember]
        public quantitymm qpf_night;
        [DataMember]
        public quantitycm snow_allday;
        [DataMember]
        public quantitycm snow_day;
        [DataMember]
        public quantitycm snow_night;
        [DataMember]
        public wind maxwind;
        [DataMember]
        public wind avewind;
        [DataMember]
        public float avehumidity;
        [DataMember]
        public float maxhumidity;
        [DataMember]
        public float minhumidity;
    }

    [DataContract]
    public class temp
    {
        [DataMember]
        public string fahrenheit;
        [DataMember]
        public string celsius;
    }

    [DataContract]
    public class wind
    {
        [DataMember]
        public float? mph;
        [DataMember]
        public float? kph;
        [DataMember]
        public string dir;
        [DataMember]
        public float? degrees;
    }

    [DataContract]
    public class quantitymm
    {
        [DataMember]
        public float? mm;
    }

    [DataContract]
    public class quantitycm
    {
        [DataMember]
        public float? cm;
    }

    [DataContract]
    public class date
    {
        [DataMember]
        public string epoch;
        [DataMember]
        public string pretty;
        [DataMember]
        public int day;
        [DataMember]
        public int month;
        [DataMember]
        public int year;
        [DataMember]
        public int yday;
        [DataMember]
        public int hour;
        [DataMember]
        public string min;
        [DataMember]
        public int sec;
        [DataMember]
        public string isdst;
        [DataMember]
        public string monthname;
        [DataMember]
        public string monthname_short;
        [DataMember]
        public string weekday_short;
        [DataMember]
        public string weekday;
        [DataMember]
        public string ampm;
        [DataMember]
        public string tz_short;
        [DataMember]
        public string tz_long;
    }
}
