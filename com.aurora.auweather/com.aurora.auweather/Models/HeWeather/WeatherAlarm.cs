using System;
namespace Com.Aurora.AuWeather.Models.HeWeather
{
    internal class WeatherAlarm
    {
        private static string[] TypeParseString = { "台风", "暴雨", "暴雪", "寒潮", "大风", "沙尘暴", "高温",
            "干旱", "雷电", "冰雹", "霜冻", "大雾", "霾", "道路结冰", "森林火险", "雷雨大风" };
        private static string[] LevelParseString = { "蓝", "黄", "橙", "红" };
        private static WeatherAlarmType ParseType(string alarm_s)
        {
            byte i = 0;
            foreach (var s in TypeParseString)
            {
                if (alarm_s.Contains(s))
                {
                    return WeatherAlarmType.typhoon + i;
                }
                i++;
            }
            throw new ArgumentException();
        }
        private static WeatherAlarmLevel ParseLevel(string alarm_s)
        {
            byte i = 0;
            foreach (var s in LevelParseString)
            {
                if (alarm_s.Contains(s))
                {
                    return WeatherAlarmLevel.blue + i;
                }
                i++;
            }
            throw new ArgumentException();
        }

        private WeatherAlarmLevel level;
        private WeatherAlarmType type;
        private string title;
        private string text;

        public WeatherAlarmLevel Level
        {
            get
            {
                return level;
            }

            set
            {
                level = value;
            }
        }

        public WeatherAlarmType Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }

        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
            }
        }

        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                text = value;
            }
        }

        public WeatherAlarm(JsonContract.WeatherAlarmContract alarm)
        {
            Level = ParseLevel(alarm.level);
            Type = ParseType(alarm.type);
            Title = alarm.title;
            Text = alarm.txt;
        }
    }
}