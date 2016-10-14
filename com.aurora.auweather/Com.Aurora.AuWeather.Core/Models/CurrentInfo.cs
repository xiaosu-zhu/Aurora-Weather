using System;

namespace Com.Aurora.AuWeather.Core.Models
{
    struct CurrentInfo
    {
        public int TodayIndex;
        public int NowHourIndex;
        public TimeSpan SunRise, SunSet;
    }
}
