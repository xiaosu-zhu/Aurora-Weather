using System.Runtime.Serialization;

namespace Com.Aurora.AuWeather.Core.Models.WunderGround.JsonContract
{
    [DataContract]
    public class response
    {
        [DataMember]
        public string version;
        [DataMember]
        public string termsofService;
        [DataMember]
        public features features;
        [DataMember]
        public results[] results;
    }
}
