using System.Runtime.Serialization;

namespace Com.Aurora.AuWeather.Core.Models.Caiyun.JsonContract
{
    [DataContract]
    public class Now
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
        public Result result;
    }
}
