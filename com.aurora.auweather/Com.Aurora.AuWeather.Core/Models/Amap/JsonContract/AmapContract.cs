using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Com.Aurora.AuWeather.Core.Models.Amap.JsonContract
{
    [DataContract]
    public class Neighborhood
    {
        [DataMember]
        public IList<object> name { get; set; }
        [DataMember]
        public IList<object> type { get; set; }
    }

    [DataContract]
    public class Building
    {
        [DataMember]
        public IList<object> name { get; set; }
        [DataMember]
        public IList<object> type { get; set; }
    }

    [DataContract]
    public class StreetNumber
    {
        [DataMember]
        public string street { get; set; }
        [DataMember]
        public string number { get; set; }
        [DataMember]
        public string location { get; set; }
        [DataMember]
        public string direction { get; set; }
        [DataMember]
        public string distance { get; set; }
    }

    [DataContract]
    public class BusinessAreas
    {
        [DataMember]
        public string location { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string id { get; set; }
    }

    [DataContract]
    public class AddressComponent
    {
        [DataMember]
        public string country { get; set; }
        [DataMember]
        public string province { get; set; }
        [DataMember]
        public string city { get; set; }
        [DataMember]
        public string citycode { get; set; }
        [DataMember]
        public string district { get; set; }
        [DataMember]
        public string adcode { get; set; }
        [DataMember]
        public string township { get; set; }
        [DataMember]
        public string towncode { get; set; }
        [DataMember]
        public Neighborhood neighborhood { get; set; }
        [DataMember]
        public Building building { get; set; }
        [DataMember]
        public StreetNumber streetNumber { get; set; }
        [DataMember]
        public IList<BusinessAreas> businessAreas { get; set; }
    }

    [DataContract]
    public class Pois
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public object tel { get; set; }
        [DataMember]
        public string direction { get; set; }
        [DataMember]
        public string distance { get; set; }
        [DataMember]
        public string location { get; set; }
        [DataMember]
        public string address { get; set; }
        [DataMember]
        public string poiweight { get; set; }
        [DataMember]
        public string businessarea { get; set; }
    }

    [DataContract]
    public class Roads
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string direction { get; set; }
        [DataMember]
        public string distance { get; set; }
        [DataMember]
        public string location { get; set; }
    }

    [DataContract]
    public class Roadinters
    {
        [DataMember]
        public string direction { get; set; }
        [DataMember]
        public string distance { get; set; }
        [DataMember]
        public string location { get; set; }
        [DataMember]
        public string first_id { get; set; }
        [DataMember]
        public string first_name { get; set; }
        [DataMember]
        public string second_id { get; set; }
        [DataMember]
        public string second_name { get; set; }
    }

    [DataContract]
    public class Regeocode
    {
        [DataMember]
        public string formatted_address { get; set; }
        [DataMember]
        public AddressComponent addressComponent { get; set; }
        [DataMember]
        public IList<Pois> pois { get; set; }
        [DataMember]
        public IList<Roads> roads { get; set; }
        [DataMember]
        public IList<Roadinters> roadinters { get; set; }
        [DataMember]
        public IList<object> aois { get; set; }
    }

    [DataContract]
    public class AmapContract
    {
        [DataMember]
        public string status { get; set; }
        [DataMember]
        public string info { get; set; }
        [DataMember]
        public string infocode { get; set; }
        [DataMember]
        public Regeocode regeocode { get; set; }
    }

}
