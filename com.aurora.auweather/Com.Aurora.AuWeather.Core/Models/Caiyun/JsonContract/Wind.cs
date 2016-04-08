using System.Runtime.Serialization;

namespace Com.Aurora.AuWeather.Core.Models.Caiyun.JsonContract
{
    [DataContract]
    public class WindTotal
    {
        [DataMember]
        public double direction;
        [DataMember]
        public double speed;
    }
}
