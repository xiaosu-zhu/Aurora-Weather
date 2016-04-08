using Com.Aurora.AuWeather.Models;
using Com.Aurora.AuWeather.Models.HeWeather;
using Com.Aurora.Shared.MVVM;

namespace Com.Aurora.AuWeather.ViewModels
{
    public class WeatherAlarmViewModel : ViewModelBase
    {

        private string title;
        private string description;
        private WeatherAlarmLevel level;

        public WeatherAlarmViewModel(string title, string description, WeatherAlarmLevel level)
        {
            Title = title;
            Description = description;
            Level = level;
        }

        public WeatherAlarmViewModel(WeatherAlarm alarm)
            : this(alarm.Title, alarm.Text, alarm.Level)
        {

        }

        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                SetProperty(ref title, value);
            }
        }

        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                SetProperty(ref description, value);
            }
        }

        public WeatherAlarmLevel Level
        {
            get
            {
                return level;
            }

            set
            {
                SetProperty(ref level, value);
            }
        }
    }
}
