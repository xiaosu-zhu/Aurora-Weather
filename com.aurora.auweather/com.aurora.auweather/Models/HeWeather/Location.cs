using System;
using System.Globalization;

namespace Com.Aurora.AuWeather.Models.HeWeather
{
    internal class Location
    {
        private string city;
        private string country;
        private string cityId;
        private float latitude;
        private float longitude;
        private DateTime updateTime;

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

        public string CityId
        {
            get
            {
                return cityId;
            }

            set
            {
                cityId = value;
            }
        }

        public float Latitude
        {
            get
            {
                return latitude;
            }

            set
            {
                latitude = value;
            }
        }

        public float Longitude
        {
            get
            {
                return longitude;
            }

            set
            {
                longitude = value;
            }
        }

        public DateTime UpdateTime
        {
            get
            {
                return updateTime;
            }

            set
            {
                updateTime = value;
            }
        }

        public Location(JsonContract.LocationContract basic)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            City = basic.city;
            Country = basic.cnty;
            CityId = basic.id;
            Latitude = float.Parse(basic.lat);
            Longitude = float.Parse(basic.lon);
            UpdateTime = DateTime.ParseExact(basic.update.loc, "yyyy-MM-dd HH:mm", provider);
        }
    }
}