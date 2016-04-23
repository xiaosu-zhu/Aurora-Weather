using System.Runtime.Serialization;

namespace Com.Aurora.AuWeather.Core.Models.WunderGround.JsonContract
{
    [DataContract]
    public class airport
    {
        [DataMember]
        public station[] station;
    }

    [DataContract]
    public class station
    {
        [DataMember]
        public string city;
        [DataMember]
        public string state;
        [DataMember]
        public string country;
        [DataMember]
        public string icao;
        [DataMember]
        public string lat;
        [DataMember]
        public string lon;
    }
}
