// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace Com.Aurora.AuWeather.Models.HeWeather.JsonContract
{
    [DataContract]
    public class WeatherSuggestionContract
    {
        [DataMember]
        public SuggestionContract comf; //舒适度指数
        [DataMember]
        public SuggestionContract cw; //洗车指数
        [DataMember]
        public SuggestionContract drsg; //穿衣指数
        [DataMember]
        public SuggestionContract flu; //感冒指数
        [DataMember]
        public SuggestionContract sport; //运动指数
        [DataMember]
        public SuggestionContract trav; //旅游指数
        [DataMember]
        public SuggestionContract uv; //紫外线指数
    }

    [DataContract]
    public class SuggestionContract
    {
        [DataMember]
        public string brf; //简介
        [DataMember]
        public string txt; //详细描述
    }
}