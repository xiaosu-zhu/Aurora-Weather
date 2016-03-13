using Com.Aurora.AuWeather.Models.HeWeather;
using Com.Aurora.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.Aurora.AuWeather.Models
{
    public class Location
    {
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
                    throw new ArgumentException();
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
            var lat1 = Tools.DegreesToRadians(source.Latitude);
            var lat2 = Tools.DegreesToRadians(dest.Latitude);
            var a = lat1 - lat2;
            var b = Tools.DegreesToRadians(source.Longitude) - Tools.DegreesToRadians(dest.Longitude);
            var s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
             Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s *= EARTH_RADIUS;
            return (float)Math.Round(s * 10000) / 10000f;
        }

        public static IOrderedEnumerable<CityInfo> GetNearsetLocation(IEnumerable<CityInfo> cities, Location source)
        {
            var final = from m in cities
                        orderby CalcDistance(m.Location, source) ascending
                        select m;
            return final;
        }
    }
}
