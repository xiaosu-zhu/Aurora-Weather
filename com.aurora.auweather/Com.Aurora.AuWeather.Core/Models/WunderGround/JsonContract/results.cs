using System.Runtime.Serialization;

namespace Com.Aurora.AuWeather.Core.Models.WunderGround.JsonContract
{
    [DataContract]
    public class results
    {
        [DataMember]
        public string name;
        [DataMember]
        public string city;
        [DataMember]
        public string state;
        [DataMember]
        public string country;
        [DataMember]
        public string country_iso3166;
        [DataMember]
        public string country_name;
        [DataMember]
        public string zmw;
        [DataMember]
        public string l;
    }
}
