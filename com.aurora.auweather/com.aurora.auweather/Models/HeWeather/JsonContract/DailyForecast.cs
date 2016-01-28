using System.Runtime.Serialization;

namespace com.aurora.auweather.Models.HeWeather.JsonContract
{
    [DataContract]
    public class DailyForecastContract
    {
        [DataMember]
        public AstronomyContract astro; //天文数值  
        [DataMember]
        public ConditionContract cond; //天气状况
        [DataMember]
        public string date; //预报日期
        [DataMember]
        public string hum; //相对湿度（%）
        [DataMember]
        public string pcpn; //降水量（mm）
        [DataMember]
        public string pop; //降水概率
        [DataMember]
        public string pres; //气压
        [DataMember]
        public TempratureContract tmp; //温度
        [DataMember]
        public string vis; //能见度（km）
        [DataMember]
        public WindContract wind; //风力风向
    }

    [DataContract]
    public class WindContract
    {
        [DataMember]
        public string deg; //风向（360度）
        [DataMember]
        public string dir; //风向
        [DataMember]
        public string sc; //风力等级
        [DataMember]
        public string spd; //风速（kmph）
    }

    [DataContract]
    public class TempratureContract
    {
        [DataMember]
        public string max; //最高温度
        [DataMember]
        public string min; //最低温度
    }

    [DataContract]
    public class ConditionContract
    {
        [DataMember]
        public string code_d; //白天天气状况代码
        [DataMember]
        public string code_n; //夜间天气状况代码
        [DataMember]
        public string txt_d; //白天天气状况描述
        [DataMember]
        public string txt_n; //夜间天气状况描述
    }

    [DataContract]
    public class AstronomyContract
    {
        [DataMember]
        public string sr; //日出时间
        [DataMember]
        public string ss; //日落时间
    }
}