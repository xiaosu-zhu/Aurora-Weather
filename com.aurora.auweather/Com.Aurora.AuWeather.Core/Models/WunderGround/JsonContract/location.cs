using System.Runtime.Serialization;

namespace Com.Aurora.AuWeather.Core.Models.WunderGround.JsonContract
{
    [DataContract]
    public class location
    {
        [DataMember]
        public string type;
        [DataMember]
        public string country;
        [DataMember]
        public string country_iso3166;
        [DataMember]
        public string country_name;
        [DataMember]
        public string state;
        [DataMember]
        public string city;
        [DataMember]
        public string tz_short;
        [DataMember]
        public string tz_long;
        [DataMember]
        public string lat;
        [DataMember]
        public string lon;
        [DataMember]
        public string zip;
        [DataMember]
        public string magic;
        [DataMember]
        public string wmo;
        [DataMember]
        public string l;
        [DataMember]
        public string requesturl;
        [DataMember]
        public string wuiurl;
        [DataMember]
        public weather_station nearby_weather_stations;
    }
}
