// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Com.Aurora.AuWeather.Models
{
    public class Pressure
    {
        private float atm;

        public float Atm
        {
            get
            {
                return atm;
            }
            set
            {
                atm = value;
            }
        }

        public float CmHg
        {
            get
            {
                return atm / 0.013157896611399f;
            }
            set
            {
                atm = value * 0.013157896611399f;
            }
        }

        public float Torr
        {
            get
            {
                return atm * 760;
            }
            set
            {
                atm = value / 760;
            }
        }

        public float HPa
        {
            get
            {
                return atm * 1013.25f;
            }
            set
            {
                atm = value / 1013.25f;
            }
        }

        public static Pressure FromHPa(float hpa)
        {
            Pressure p = new Pressure();
            p.HPa = hpa;
            return p;
        }
        public static Pressure FromAtm(float hpa)
        {
            Pressure p = new Pressure();
            p.Atm = hpa;
            return p;
        }
        public static Pressure FromTorr(float hpa)
        {
            Pressure p = new Pressure();
            p.Torr = hpa;
            return p;
        }
        public static Pressure FromCmHg(float hpa)
        {
            Pressure p = new Pressure();
            p.CmHg = hpa;
            return p;
        }
    }
}
