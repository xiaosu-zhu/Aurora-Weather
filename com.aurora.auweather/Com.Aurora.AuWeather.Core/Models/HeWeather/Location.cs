// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Globalization;

namespace Com.Aurora.AuWeather.Models.HeWeather
{
    public class Location
    {
        public string City
        {
            get; private set;
        }

        public string Country
        {
            get; private set;
        }

        public string CityId
        {
            get; private set;
        }

        public float Latitude
        {
            get; private set;
        }

        public float Longitude
        {
            get; private set;
        }

        /// <summary>
        /// 更新时间 yyyy-MM-dd HH:mm (当地时间)
        /// </summary>
        public DateTime UpdateTime
        {
            get; private set;
        }

        public DateTime UtcTime
        {
            get; private set;
        }

        public Location(JsonContract.LocationContract basic)
        {
            if (basic == null)
            {
                return;
            }
            CultureInfo provider = CultureInfo.InvariantCulture;
            City = basic.city;
            Country = basic.cnty;
            CityId = basic.id;
            Latitude = float.Parse(basic.lat);
            Longitude = float.Parse(basic.lon);
            UpdateTime = DateTime.ParseExact(basic.update.loc, "yyyy-MM-dd H:mm", provider);
            UtcTime = DateTime.ParseExact(basic.update.utc, "yyyy-MM-dd H:mm", provider);
        }

        public Location(double lat, double lon, long timeTick, int timeZoneShift)
        {
            Latitude = (float)lat;
            Longitude = (float)lon;
            var t = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            UtcTime = t.AddSeconds(timeTick);
            UpdateTime = UtcTime.AddSeconds(timeZoneShift);
        }
    }
}