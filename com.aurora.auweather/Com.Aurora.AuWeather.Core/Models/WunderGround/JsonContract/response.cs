using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Com.Aurora.AuWeather.Core.Models.WunderGround.JsonContract
{
    [DataContract]
    class response
    {
        [DataMember]
        public string version;
        [DataMember]
        public string termsofService;
        //[DataMember]
        //public Features features;
        //[DataMember]
        //public Results[] results;
    }
}
