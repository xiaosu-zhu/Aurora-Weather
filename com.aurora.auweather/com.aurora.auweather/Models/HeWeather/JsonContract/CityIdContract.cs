using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using com.aurora.shared.Helpers;

namespace com.aurora.auweather.Models.HeWeather.JsonContract
{
    [DataContract]
    class CityIdContract
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
