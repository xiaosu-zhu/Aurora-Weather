using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Com.Aurora.AuWeather.Core.Models.IP
{
    [DataContract]
    public class IpContract
    {
        [DataMember]
        public string ret { get; set; }
        [DataMember]
        public string ip { get; set; }
        [DataMember]
        public IList<string> data { get; set; }
    }
}
