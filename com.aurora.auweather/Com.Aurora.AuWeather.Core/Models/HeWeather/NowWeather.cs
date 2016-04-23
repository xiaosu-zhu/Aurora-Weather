// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.Core.Models.Caiyun.JsonContract;
using Com.Aurora.AuWeather.Core.Models.WunderGround.JsonContract;
using System;

namespace Com.Aurora.AuWeather.Models.HeWeather
{
    public class NowWeather
    {

        public NowCondition Now
        {
            get; private set;
        }

        public Temperature BodyTemprature
        {
            get; private set;
        }

        public float Precipitation
        {
            get; private set;
        }

        public Length Visibility
        {
            get; private set;
        }

        public Wind Wind
        {
            get; private set;
        }

        public Pressure Pressure
        {
            get; private set;
        }

        public Temperature Temprature
        {
            get; private set;
        }

        public NowWeather(JsonContract.NowWeatherContract now)
        {
            if (now == null)
            {
                return;
            }
            Now = new NowCondition(now.cond);
            int fl;
            if (int.TryParse(now.fl, out fl))
                BodyTemprature = Temperature.FromCelsius(fl);
            float pcpn;
            if (float.TryParse(now.pcpn, out pcpn))
                Precipitation = pcpn;
            if (float.TryParse(now.vis, out pcpn))
                Visibility = Length.FromKM(pcpn);
            Wind = new Wind(now.wind);
            if (float.TryParse(now.pres, out pcpn))
                Pressure = Pressure.FromHPa(pcpn);
            if (int.TryParse(now.tmp, out fl))
                Temprature = Temperature.FromCelsius(fl);
        }

        public NowWeather(double temp, string con, PcpnTotal pcpn, WindTotal wind)
        {
            Now = new NowCondition(con);
            BodyTemprature = null;
            Precipitation = (float)pcpn.local.intensity;
            Wind = new Wind(wind);
            Pressure = null;
            Temprature = Temperature.FromCelsius((float)temp);
        }

        public NowWeather(observation current_observation)
        {
            if (current_observation == null)
            {
                return;
            }
            Now = new NowCondition(current_observation);
            float i;
            Temprature = Temperature.FromCelsius(current_observation.temp_c);
            if (float.TryParse(current_observation.feelslike_c, out i))
            {
                BodyTemprature = Temperature.FromCelsius(i);
            }
            float f;
            if (float.TryParse(current_observation.precip_today_metric, out f))
            {
                Precipitation = f;
            }
            if (float.TryParse(current_observation.visibility_km, out f))
            {
                Visibility = Length.FromKM(f);
            }
            Wind = new Wind(Convert.ToUInt32(current_observation.wind_kph), Convert.ToUInt32(current_observation.wind_degrees));
            if (float.TryParse(current_observation.pressure_mb, out f))
            {
                Pressure = Pressure.FromHPa(f);
            }
        }
    }

    public class NowCondition : Condition
    {

        public WeatherCondition Condition
        {
            get; private set;
        }
        public NowCondition(JsonContract.Condition_NowContract cond)
        {
            Condition = ParseCondition(cond.code);
        }

        public NowCondition(string con)
        {
            Condition = ParseCondition_C(con);
        }

        public NowCondition(observation current_observation)
        {
            Condition = ParseCondition_W(current_observation.icon);
        }
    }
}