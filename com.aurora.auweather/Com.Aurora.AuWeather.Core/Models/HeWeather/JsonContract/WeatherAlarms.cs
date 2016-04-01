// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace Com.Aurora.AuWeather.Models.HeWeather.JsonContract
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