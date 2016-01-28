using com.aurora.auweather.Models;
using com.aurora.shared.MVVM;

namespace com.aurora.auweather.ViewModels
{
    internal class WeatherViewModel : ViewModelBase
    {
        private Temprature temprature;
        private Wind wind;
        private WeatherCondition condition;
        public Temprature Temprature
        {
            get
            {
                return temprature;
            }

            set
            {
                SetProperty(ref temprature, value);
            }
        }

        internal Wind Wind
        {
            get
            {
                return wind;
            }

            set
            {
                SetProperty(ref wind, value);
            }
        }

        public WeatherCondition Condition
        {
            get
            {
                return condition;
            }

            set
            {
                SetProperty(ref condition, value);
            }
        }
    }
}