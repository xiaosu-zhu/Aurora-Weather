using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.aurora.auweather.Models.HeWeather.JsonContract;

namespace com.aurora.auweather.Models.HeWeather
{
    public class CityInfo
    {
        private string city;
        private string country;
        private string province;
        private string id;
        private Models.Location location;

        public string City
        {
            get
            {
                return city;
            }

            set
            {
                city = value;
            }
        }

        public string Country
        {
            get
            {
                return country;
            }

            set
            {
                country = value;
            }
        }

        public string Province
        {
            get
            {
                return province;
            }

            set
            {
                province = value;
            }
        }

        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        internal Models.Location Location
        {
            get
            {
                return location;
            }

            set
            {
                location = value;
            }
        }

        internal static List<CityInfo> CreateList(CityIdContract citys)
        {
            if (citys != null && citys.city_info != null & citys.city_info.Length > 0)
            {
                List<CityInfo> c = new List<CityInfo>();
                foreach (var city in citys.city_info)
                {
                    c.Add(new CityInfo(city));
                }
                return c;
            }
            return null;
        }

        public CityInfo()
        {

        }

        public CityInfo(JsonContract.CityInfoContract info)
        {
            City = info.city;
            Country = info.cnty;
            Province = info.prov;
            Id = info.id;
            double lat;
            if (double.TryParse(info.lat, out lat))
            {
                double lon;
                if (double.TryParse(info.lon, out lon))
                {
                    FuckingshitLocation(ref lat, ref lon, Country);
                    this.Location = new Models.Location(lat, lon);
                }
            }

        }

        private void FuckingshitLocation(ref double lat, ref double lon, string country)
        {
            if (lat > lon && country == "中国")
            {
                var a = lat;
                lat = lon;
                lon = a;
            }
        }
    }
}
