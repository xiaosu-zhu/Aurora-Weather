using Com.Aurora.Shared.Helpers;
using System;

namespace Com.Aurora.AuWeather.Core.Models.Caiyun.JsonContract
{
    public class CaiyunContract
    {
        public Now now;
        public Forecast forecast;

        public static CaiyunContract Generate(string s)
        {
            var m = s.Split(new string[] { ":|:" }, StringSplitOptions.RemoveEmptyEntries);
            Now r;
            r = JsonHelper.FromJson<Now>(m[0]);
            Forecast f;
            f = JsonHelper.FromJson<Forecast>(m[1]);
            return new CaiyunContract
            {
                now = r,
                forecast = f
            };
        }
    }
}
