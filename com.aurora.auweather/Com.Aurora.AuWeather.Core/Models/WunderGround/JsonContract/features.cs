using System.Runtime.Serialization;

namespace Com.Aurora.AuWeather.Core.Models.WunderGround.JsonContract
{
    [DataContract]
    public class features
    {
        [DataMember]
        public uint geolookup;
        [DataMember]
        public uint conditions;
        [DataMember]
        public uint forecast;
        [DataMember]
        public uint hourly;
    }
}
