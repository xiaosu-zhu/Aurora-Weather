// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.Core.Models.Caiyun.JsonContract;
using Com.Aurora.AuWeather.Core.Models.WunderGround.JsonContract;
using System;
using System.Globalization;

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
            CultureInfo provider = CultureInfo.InvariantCulture;
            Now = new NowCondition(now.cond);
            int fl;
            if (int.TryParse(now.fl, NumberStyles.Any, provider, out fl))
                BodyTemprature = Temperature.FromCelsius(fl);
            float pcpn;
            if (float.TryParse(now.pcpn, NumberStyles.Any, provider, out pcpn))
                Precipitation = pcpn;
            if (float.TryParse(now.vis, NumberStyles.Any, provider, out pcpn))
                Visibility = Length.FromKM(pcpn);
            Wind = new Wind(now.wind);
            if (float.TryParse(now.pres, NumberStyles.Any, provider, out pcpn))
                Pressure = Pressure.FromHPa(pcpn);
            if (int.TryParse(now.tmp, NumberStyles.Any, provider, out fl))
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
            CultureInfo provider = CultureInfo.InvariantCulture;
            float i;
            Temprature = Temperature.FromCelsius(current_observation.temp_c);
            if (float.TryParse(current_observation.feelslike_c, NumberStyles.Any, provider, out i))
            {
                BodyTemprature = Temperature.FromCelsius(i);
            }
            if (float.TryParse(current_observation.precip_today_metric, NumberStyles.Any, provider, out i))
            {
                Precipitation = i;
            }
            if (float.TryParse(current_observation.visibility_km, NumberStyles.Any, provider, out i))
            {
                Visibility = Length.FromKM(i);
            }
            Wind = new Wind(Convert.ToUInt32(current_observation.wind_kph), Convert.ToUInt32(current_observation.wind_degrees));
            if (float.TryParse(current_observation.pressure_mb, NumberStyles.Any, provider, out i))
            {
                Pressure = Pressure.FromHPa(i);
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