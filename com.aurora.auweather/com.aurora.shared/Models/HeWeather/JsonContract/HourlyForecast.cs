using System.Runtime.Serialization;

namespace Com.Aurora.AuWeather.Models.HeWeather.JsonContract
{
    [DataContract]
    public class HourlyForecastContract
    {
        [DataMember]
        public string date; //时间
        [DataMember]
        public string hum; //相对湿度（%）
        [DataMember]
        public string pop; //降水概率
        [DataMember]
        public string pres; //气压
        [DataMember]
        public string tmp; //温度
        [DataMember]
        public WindContract wind; //风力风向
    }
}