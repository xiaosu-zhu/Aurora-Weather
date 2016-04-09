// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.Core.Models.Caiyun.JsonContract;

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
            BodyTemprature = Temperature.FromCelsius(int.Parse(now.fl));
            Precipitation = float.Parse(now.pcpn);
            Visibility = Length.FromKM(float.Parse(now.vis));
            Wind = new Wind(now.wind);
            Pressure = Pressure.FromHPa(float.Parse(now.pres));
            Temprature = Temperature.FromCelsius(int.Parse(now.tmp));
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
    }
}