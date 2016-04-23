using System.Runtime.Serialization;

namespace Com.Aurora.AuWeather.Core.Models.WunderGround.JsonContract
{
    [DataContract]
    public class weather_station
    {
        [DataMember]
        public airport airport;
        [DataMember]
        public pws pws;
    }
}
