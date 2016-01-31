using System.Runtime.Serialization;

namespace Com.Aurora.AuWeather.Models.HeWeather.JsonContract
{
    [DataContract]
    public class NowWeatherContract
    {
        [DataMember]
        public Condition_NowContract cond; //天气状况
        [DataMember]
        public string fl; //体感温度
        [DataMember]
        public string hum; //相对湿度（%）
        [DataMember]
        public string pcpn; //降水量（mm）
        [DataMember]
        public string pres; //气压
        [DataMember]
        public string tmp; //温度
        [DataMember]
        public string vis; //能见度（km）
        [DataMember]
        public WindContract wind; //风力风向
    }

    [DataContract]
    public class Condition_NowContract
    {
        [DataMember]
        public string code; //天气状况代码，所有天气代码和中英文对照以及图标请参见 http://www.heweather.com/documents/condition-code
        [DataMember]
        public string txt; //天气状况描述
    }
}