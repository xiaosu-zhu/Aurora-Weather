// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.Models.HeWeather;
using Com.Aurora.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Com.Aurora.AuWeather.Core.Models.Amap.JsonContract;
using Com.Aurora.AuWeather.Core.Models.IP;
using Com.Aurora.AuWeather.Core.Models;
using Com.Aurora.AuWeather.Core.Models.OpenStreetMap;
using Com.Aurora.Shared.Extensions;
using Com.Aurora.AuWeather.Core.SQL;
using System.Linq.Expressions;

namespace Com.Aurora.AuWeather.Models
{
    public class Location
    {
        public override string ToString()
        {
            return string.Format("{0}, {1}", Latitude > 0 ? Latitude.ToString("0.0") + "° N" : Latitude < 0 ? (-Latitude).ToString("0.0") + "° S" : "0.0°", Longitude > 0 ? Longitude.ToString("0.0") + "° E" : Longitude < 0 ? (-Longitude).ToString("0.0") + "° W" : "0.0°");
        }

        private float latitude;
        private float longitude;

        /// <summary>
        /// -90.0°~90.0°, negative-south, positive-north
        /// </summary>
        public float Latitude
        {
            get
            {
                return latitude;
            }

            set
            {
                if (value < -90 || value > 90)
                    latitude = 0;
                else
                    latitude = value;
            }
        }

        /// <summary>
        /// -180.0°~180.0°, any input will be converted into this range. negative-west, poitive-east
        /// </summary>
        public float Longitude
        {
            get
            {
                return longitude;
            }

            set
            {
                var p = value % 360;
                if (p > 180)
                {
                    p -= 360;
                }
                else if (p < -180)
                {
                    p += 360;
                }
                longitude = p;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lat">纬度</param>
        /// <param name="lon">经度</param>
        public Location(float lat, float lon)
        {
            this.Latitude = lat;
            this.Longitude = lon;
        }
        private const double EARTH_RADIUS = 6378.137;//地球半径

        /// <summary>
        /// 计算两点距离
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public static float CalcDistance(float lat, float lon, Location dest)
        {
            try
            {
                var lat1 = Tools.DegreesToRadians(lat);
                var lat2 = Tools.DegreesToRadians(dest.Latitude);
                var a = lat1 - lat2;
                var b = Tools.DegreesToRadians(lon) - Tools.DegreesToRadians(dest.Longitude);
                var s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
                 Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(b / 2), 2)));
                s *= EARTH_RADIUS;
                return (float)Math.Round(s * 10000) / 10000f;
            }
            catch (Exception)
            {
                return float.MaxValue;
            }
        }

        public static City GetNearsetLocation(Location source)
        {
            var list = SQLAction.GetAll();
            float dis = float.MaxValue;
            City fi = null;
            foreach (var item in list)
            {
                var cal = CalcDistance(item.Latitude, item.Longitude, source);
                if (dis > cal)
                {
                    dis = cal;
                    fi = item;
                }
            }
            return fi;
        }

        public static async Task<AmapContract> AmapReGeoAsync(double latitude, double longitude)
        {
            var requestUrl = "http://restapi.amap.com/v3/geocode/regeo?key={0}&location={1},{2}&radius=1000&extensions=all&batch=false&roadlevel=1";
            // 高德经纬度颠倒
            var result = await ApiRequestHelper.RequestWithFormattedUrlAsync(requestUrl, new string[] { Key.amap, longitude.ToString("0.######"), latitude.ToString("0.######") });
            // json 生成类
            if (result == null)
                return null;
            var amapRes = JsonHelper.FromJson<AmapContract>(result);
            return amapRes;
        }

        public static async Task<OpenCageContract> OpenMapReGeoAsync(double latitude, double longitude)
        {
            var OpenStreetUrl = "http://api.opencagedata.com/geocode/v1/json?q={0}+{1}&key={2}&language=zh-cn";
            var result = await ApiRequestHelper.RequestWithFormattedUrlAsync(OpenStreetUrl, new string[] { latitude.ToString("0.######"), longitude.ToString("0.######"), Key.OpenCage });
            // json 生成类
            if (result == null)
                return null;
            var omapRes = JsonHelper.FromJson<OpenCageContract>(result);
            return omapRes;
        }

        public async static Task<HeWeather.Location> ReGeobyIpAsync()
        {
            var ipUrl = "http://api.ip138.com/query/?token={0}";
            var result = await ApiRequestHelper.RequestWithFormattedUrlAsync(ipUrl, new string[] { Key.ip138 });
            if (result == null)
                return null;
            var ipRes = JsonHelper.FromJson<IpContract>(result);
            var city = await Request.RequestbyIpAsync(ipRes.ip);
            var fetchresult = HeWeatherModel.Generate(city, DataSource.HeWeather);
            return fetchresult.Location;
        }

        public async static Task<string> HeWeatherGeoLookup(string name)
        {
            var url = "https://free-api.heweather.com/v5/search?key={0}&city={1}";
            var result = await ApiRequestHelper.RequestWithFormattedUrlAsync(url, new string[] { Key.ip138, name });
            if (result == null)
                return null;
            var rees = JsonHelper.FromJson<HeWeatherCity>(result);
            if (!rees.HeWeather5.IsNullorEmpty())
                return rees.HeWeather5[0].basic.id;
            else
            {
                return "";
            }
        }

        public async static Task<City> ReverseGeoCode(float lat, float lon, LocateRoute[] routes)
        {
            City final = null;

            foreach (var r in routes)
            {
                switch (r)
                {
                    case LocateRoute.unknown:
                        var near = GetNearsetLocation(new Location(lat, lon));
                        final = near;
                        break;
                    case LocateRoute.Amap:
                        var acontract = await AmapReGeoAsync(lat, lon);
                        if (acontract != null)
                        {
                            var li = new City
                            {
                                CityEn = acontract.regeocode.addressComponent.district,
                                CityZh = acontract.regeocode.addressComponent.district,
                                LeaderEn = acontract.regeocode.addressComponent.city,
                                LeaderZh = acontract.regeocode.addressComponent.city,
                                CountryCode = "CN",
                                CountryEn = acontract.regeocode.addressComponent.country,
                                //Id = await Models.Location.HeWeatherGeoLookup(acontract.regeocode.addressComponent.district),
                                Latitude = (float)lat,
                                Longitude = (float)lon,
                                ProvinceEn = acontract.regeocode.addressComponent.province,
                                ProvinceZh = acontract.regeocode.addressComponent.province
                            };
                            final = li;
                        }
                        break;
                    case LocateRoute.Omap:
                        var ocontract = await OpenMapReGeoAsync(lat, lon);
                        if (ocontract != null && !ocontract.results.IsNullorEmpty())
                        {
                            var li = new City
                            {
                                CityEn = ocontract.results[0].components.county,
                                CityZh = ocontract.results[0].components.county,
                                LeaderEn = ocontract.results[0].components.region,
                                LeaderZh = ocontract.results[0].components.region,
                                CountryCode = ocontract.results[0].components.country_code,
                                CountryEn = ocontract.results[0].components.country,
                                //Id = await HeWeatherGeoLookup(ocontract.results[0].components.county),
                                Latitude = lat,
                                Longitude = lon,
                                ProvinceEn = ocontract.results[0].components.state,
                                ProvinceZh = ocontract.results[0].components.state
                            };
                            final = li;
                        }
                        break;
                    case LocateRoute.IP:
                        var id = await ReGeobyIpAsync();
                        if (id != null)
                        {
                            final = SQLAction.Find(id.CityId);
                        }
                        break;
                    case LocateRoute.Gmap:

                        break;
                    default:
                        break;
                }
                if (final != null)
                {
                    break;
                }
            }

            if (final != null)
            {
                return final;
            }
            else
            {
                throw new Exception("Locate Failed.");
            }
        }
    }
}
