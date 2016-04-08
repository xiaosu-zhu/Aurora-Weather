using System.Runtime.Serialization;

namespace Com.Aurora.AuWeather.Core.Models.Caiyun.JsonContract
{
    [DataContract]
    public class Result
    {
        [DataMember]
        public string status;
        [DataMember]
        public double temperature;
        [DataMember]
        public string skycon;
        [DataMember]
        public double cloudrate;
        [DataMember]
        public double humidity;
        [DataMember]
        public PcpnTotal precipitation;
        [DataMember]
        public WindTotal wind;
    }
}