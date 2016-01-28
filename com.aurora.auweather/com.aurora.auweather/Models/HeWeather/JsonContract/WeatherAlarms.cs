using System.Runtime.Serialization;

namespace com.aurora.auweather.Models.HeWeather.JsonContract
{
    [DataContract]
    public class WeatherAlarmContract
    {
        [DataMember]
        public string level; //预警等级
        [DataMember]
        public string stat; //预警状态
        [DataMember]
        public string title; //预警信息标题
        [DataMember]
        public string txt; //预警信息详情
        [DataMember]
        public string type; //预警天气类型
    }
}