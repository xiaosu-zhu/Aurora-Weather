// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Com.Aurora.AuWeather.Models
{
    public class Length
    {
        private double length;

        public double KM
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

        public double M
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

        public double Mile
        {
            get
            {
                return length / 1.609344;
            }
            set
            {
                length = value * 1.609344;
            }
        }

        public double NM
        {
            get
            {
                return length / 1.85318;
            }
            set
            {
                length = value * 1.85318;
            }
        }

        public static Length FromKM(double l)
        {
            var m = new Length();
            m.length = l;
            return m;
        }
        public static Length FromM(double l)
        {
            var m = new Length();
            m.M = l;
            return m;
        }
        public static Length FromMile(double l)
        {
            var m = new Length();
            m.Mile = l;
            return m;
        }
        public static Length FromNM(double l)
        {
            var m = new Length();
            m.NM = l;
            return m;
        }
    }
}
