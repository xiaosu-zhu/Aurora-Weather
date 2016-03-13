using System.Runtime.Serialization;

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
