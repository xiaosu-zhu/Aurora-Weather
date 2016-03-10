using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Aurora.AuWeather.Models
{
    public class Length
    {
        private float length;

        public float KM
        {
            get
            {
                return length;
            }
            set
            {
                length = value;
            }
        }

        public float M
        {
            get
            {
                return length * 1000;
            }
            set
            {
                length = value / 1000;
            }
        }

        public float Mile
        {
            get
            {
                return length / 1.609344f;
            }
            set
            {
                length = value * 1.609344f;
            }
        }

        public float NM
        {
            get
            {
                return length / 1.852f;
            }
            set
            {
                length = value * 1.852f;
            }
        }

        public static Length FromKM(float l)
        {
            var m = new Length();
            m.length = l;
            return m;
        }
        public static Length FromM(float l)
        {
            var m = new Length();
            m.M = l;
            return m;
        }
        public static Length FromMile(float l)
        {
            var m = new Length();
            m.Mile = l;
            return m;
        }
        public static Length FromNM(float l)
        {
            var m = new Length();
            m.NM = l;
            return m;
        }
    }
}
