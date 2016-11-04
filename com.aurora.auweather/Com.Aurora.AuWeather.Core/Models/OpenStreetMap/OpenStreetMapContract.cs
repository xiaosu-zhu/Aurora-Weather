using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Com.Aurora.AuWeather.Core.Models.OpenStreetMap
{
    [DataContract]
    public class Address
    {
        [DataMember]
        public string house_number { get; set; }
        [DataMember]
        public string road { get; set; }
        [DataMember]
        public string suburb { get; set; }
        [DataMember]
        public string hamlet { get; set; }
        [DataMember]
        public string city { get; set; }
        [DataMember]
        public string state_district { get; set; }
        [DataMember]
        public string state { get; set; }
        [DataMember]
        public string postcode { get; set; }
        [DataMember]
        public string country { get; set; }
        [DataMember]
        public string country_code { get; set; }
    }

    [DataContract]
    public class OpenStreetMapContract
    {
        [DataMember]
        public string place_id { get; set; }
        [DataMember]
        public string licence { get; set; }
        [DataMember]
        public string osm_type { get; set; }
        [DataMember]
        public string osm_id { get; set; }
        [DataMember]
        public string lat { get; set; }
        [DataMember]
        public string lon { get; set; }
        [DataMember]
        public string display_name { get; set; }
        [DataMember]
        public Address address { get; set; }
        [DataMember]
        public IList<string> boundingbox { get; set; }
    }

}
