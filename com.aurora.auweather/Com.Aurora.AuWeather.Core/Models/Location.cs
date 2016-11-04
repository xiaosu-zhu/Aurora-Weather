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
        public static float CalcDistance(Location source, Location dest)
        {
            try
            {
                var lat1 = Tools.DegreesToRadians(source.Latitude);
                var lat2 = Tools.DegreesToRadians(dest.Latitude);
                var a = lat1 - lat2;
                var b = Tools.DegreesToRadians(source.Longitude) - Tools.DegreesToRadians(dest.Longitude);
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

        public static IOrderedEnumerable<CityInfo> GetNearsetLocation(IEnumerable<CityInfo> cities, Location source)
        {
            var final = from m in cities
                        orderby CalcDistance(m.Location, source) ascending
                        select m;
            return final;
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

        public static async Task<OpenStreetMapContract> OpenMapReGeoAsync(double latitude, double longitude)
        {
            var OpenStreetUrl = "http://nominatim.openstreetmap.org/reverse?format=json&accept-language=chinese&lat={0}&lon={1}&zoom=18";
            var result = await ApiRequestHelper.RequestWithFormattedUrlAsync(OpenStreetUrl, new string[] { latitude.ToString("0.######"), longitude.ToString("0.######") });
            // json 生成类
            if (result == null)
                return null;
            var omapRes = JsonHelper.FromJson<OpenStreetMapContract>(result);
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
    }
}
