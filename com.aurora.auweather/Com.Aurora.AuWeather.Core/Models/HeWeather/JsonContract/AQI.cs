// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace Com.Aurora.AuWeather.Models.HeWeather.JsonContract
{
    [DataContract]
    public class AQIContract
    {
        [DataMember]
        public AQICityContract city; //json格式如此，必须整个类
    }

    [DataContract]
    public class AQICityContract
    {
        [DataMember]
        public string aqi; //空气质量指数
        [DataMember]
        public string co; //一氧化碳1小时平均值(ug/m³)
        [DataMember]
        public string no2; //二氧化氮1小时平均值(ug/m³)
        [DataMember]
        public string o3; //臭氧1小时平均值(ug/m³)
        [DataMember]
        public string pm10; //PM10 1小时平均值(ug/m³)
        [DataMember]
        public string pm25; //PM2.5 1小时平均值(ug/m³)
        [DataMember]
        public string qlty; //空气质量类别
        [DataMember]
        public string so2; //二氧化硫1小时平均值(ug/m³)
    }
}