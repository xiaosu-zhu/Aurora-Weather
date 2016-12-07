//// Copyright (c) Aurora Studio. All rights reserved.
////
//// Licensed under the MIT License. See LICENSE in the project root for license information.

//using System.Collections.Generic;
//using Com.Aurora.AuWeather.Models.HeWeather.JsonContract;
//using Com.Aurora.Shared.Extensions;
//using System.Globalization;

//namespace Com.Aurora.AuWeather.Models.HeWeather
//{
//    public class CityInfo
//    {
//        private string city;
//        private string country;
//        private string province;
//        private string id;
//        private Models.Location location;

//        public override string ToString()
//        {
//             return Province.IsNullorEmpty() ? City : (Province + " - " + City);
//        }

//        public string City
//        {
//            get
//            {
//                return city;
//            }

//            set
//            {
//                city = value;
//            }
//        }

//        public string Country
//        {
//            get
//            {
//                return country;
//            }

//            set
//            {
//                country = value;
//            }
//        }

//        public string Province
//        {
//            get
//            {
//                return province;
//            }

//            set
//            {
//                province = value;
//            }
//        }

//        public string Id
//        {
//            get
//            {
//                return id;
//            }

//            set
//            {
//                id = value;
//            }
//        }

//        public Models.Location Location
//        {
//            get
//            {
//                return location;
//            }

//            set
//            {
//                location = value;
//            }
//        }

//        public static List<CityInfo> CreateList(CityIdContract citys)
//        {
//            if (citys != null && !citys.city_info.IsNullorEmpty())
//            {
//                List<CityInfo> c = new List<CityInfo>();
//                c.Capacity = 80000;
//                foreach (var city in citys.city_info)
//                {
//                    if (city.id.EndsWith("A"))
//                    {
//                        continue;
//                    }
//                    c.Add(new CityInfo(city));
//                }
//                return c;
//            }
//            return null;
//        }

//        public CityInfo()
//        {

//        }

//        public CityInfo(CityInfoContract info)
//        {
//            City = info.city;
//            Country = info.cnty;
//            Province = info.prov;
//            Id = info.id;
//            CultureInfo provider = CultureInfo.InvariantCulture;
//            float lat;
//            if (float.TryParse(info.lat, NumberStyles.Any, provider, out lat))
//            {
//                float lon;
//                if (float.TryParse(info.lon, NumberStyles.Any, provider, out lon))
//                {
//                    FuckingshitLocation(ref lat, ref lon, Country);
//                    this.Location = new Models.Location(lat, lon);
//                }
//            }

//        }

//        private void FuckingshitLocation(ref float lat, ref float lon, string country)
//        {
//            if (lat > lon && country == "中国")
//            {
//                var a = lat;
//                lat = lon;
//                lon = a;
//            }
//        }
//    }
//}
