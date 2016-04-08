using System.Runtime.Serialization;

namespace Com.Aurora.AuWeather.Core.Models.Caiyun.JsonContract
{
    [DataContract]
    public class PcpnTotal
    {
        [DataMember]
        public Pcpn nearest;
        [DataMember]
        public Pcpn1 local;
    }

    [DataContract]
    public class Pcpn
    {
        [DataMember]
        public string status;
        [DataMember]
        public double distance; //距离
        [DataMember]
        public double intensity; //角度

    }

    [DataContract]
    public class Pcpn1
    {
        [DataMember]
        public string status;
        [DataMember]
        public double intensity; //降水强度
        [DataMember]
        public string datasource; //数据源
    }
}
