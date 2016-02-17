using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Aurora.AuWeather.Models
{
    class Location
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

        public Location(float lat, float lon)
        {
            this.Latitude = lat;
            this.Longitude = lon;
        }
    }
}
