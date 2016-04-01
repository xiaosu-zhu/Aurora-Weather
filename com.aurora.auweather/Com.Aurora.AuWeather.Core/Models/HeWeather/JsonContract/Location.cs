// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace Com.Aurora.AuWeather.Models.HeWeather.JsonContract
{
    [DataContract]
    public class LocationContract
    {
        [DataMember]
        public string city; //城市名称
        [DataMember]
        public string cnty; //国家
        [DataMember]
        public string id; //城市ID，所有城市ID请参见 http://www.heweather.com/documents/cn-city-list
        [DataMember]
        public string lat; //城市纬度
        [DataMember]
        public string lon; //城市经度
        [DataMember]
        public UpdateTimeContract update; //更新时间
    }

    [DataContract]
    public class UpdateTimeContract
    {
        [DataMember]
        public string loc; //当地时间
        [DataMember]
        public string utc; //UTC时间
    }
}