using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Com.Aurora.Shared.Helpers;

namespace Com.Aurora.AuWeather.Models.HeWeather.JsonContract
{
    [DataContract]
    public class CityIdContract
    {
        [DataMember]
        public CityInfoContract[] city_info;
    }

    [DataContract]
    public class CityInfoContract
    {
        [DataMember]
        public string city;
        [DataMember]
        public string cnty;
        [DataMember]
        public string id;
        [DataMember]
        public string lat;
        [DataMember]
        public string lon;
        [DataMember]
        public string prov;
        [DataMember]
        public string pinyin;
        [DataMember]
        public string postcode;
    }
}
