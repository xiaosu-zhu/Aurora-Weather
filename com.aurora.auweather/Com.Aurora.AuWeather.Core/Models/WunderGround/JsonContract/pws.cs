using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Com.Aurora.AuWeather.Core.Models.WunderGround.JsonContract
{
    [DataContract]
    public class pws
    {
        [DataMember]
        public pwsstation[] station;
    }

    [DataContract]
    public class pwsstation
    {
        [DataMember]
        public string neighborhood;
        [DataMember]
        public string city;
        [DataMember]
        public string state;
        [DataMember]
        public string country;
        [DataMember]
        public string id;
        [DataMember]
        public double lat;
        [DataMember]
        public double lon;
        [DataMember]
        public double distance_km;
        [DataMember]
        public double distance_mi;
    }
}
