using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Com.Aurora.Shared.Helpers;

namespace Com.Aurora.AuWeather.Models.HeWeather.JsonContract
{
    [DataContract]
    class HeWeatherContract
    {
        [DataMember]
        public string status { get; set; } //接口状态 
        [DataMember]
        public WeatherAlarmContract[] alarms { get; set; } //灾害预警，若所在城市无预警则不显示该字段，仅限国内城市
        [DataMember]
        public AQIContract aqi { get; set; } //空气质量，仅限国内城市
        [DataMember]
        public LocationContract basic { get; set; } //基本信息
        [DataMember]
        public DailyForecastContract[] daily_forecast { get; set; } //天气预报，国内7天，国际10天
        [DataMember]
        public HourlyForecastContract[] hourly_forecast { get; set; } //当天每小时天气预报
        [DataMember]
        public NowWeatherContract now { get; set; } //实况天气
        [DataMember]
        public WeatherSuggestionContract suggestion { get; set; } //生活指数，仅限国内城市

        private static string TrimResult(string origin)
        {
            var re = origin.Trim();
            re = re.Remove(0, 31);                //需要找一个好方法把开头的：  {"HeWeather data service 3.0":[
            return re.Remove(re.Length - 2, 2);   //和结尾的： ]}  去掉 
        }

        public static HeWeatherContract Generate(string source)
        {
            source = TrimResult(source);
            return JsonHelper.FromJson<HeWeatherContract>(source);
        }
    }
}
