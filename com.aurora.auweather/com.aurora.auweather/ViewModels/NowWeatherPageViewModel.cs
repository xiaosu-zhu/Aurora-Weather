using Com.Aurora.AuWeather.Models;
using Com.Aurora.AuWeather.Models.HeWeather;
using Com.Aurora.AuWeather.Models.HeWeather.JsonContract;
using Com.Aurora.Shared.Helpers;
using Com.Aurora.Shared.MVVM;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.System.Threading;
using Windows.UI.Core;

namespace Com.Aurora.AuWeather.ViewModels
{
    internal class NowWeatherPageViewModel : ViewModelBase
    {
        private Temprature temprature;
        private Wind wind;
        private WeatherCondition condition;
        private string city;
        private string id;
        private HeWeatherModel fetchresult;
        private float tempraturePath0;
        private float tempraturePath1;
        private float tempraturePath2;
        private float tempraturePath3;
        private float tempraturePath4;
        private float tempraturePath5;

        private Temprature hourlyTemp0;
        private Temprature hourlyTemp1;
        private Temprature hourlyTemp2;
        private Temprature hourlyTemp3;
        private Temprature hourlyTemp4;
        private Temprature hourlyTemp5;

        private DateTime hour0;
        private DateTime hour1;
        private DateTime hour2;
        private DateTime hour3;
        private DateTime hour4;
        private DateTime hour5;

        private float pop0;
        private float pop1;
        private float pop2;
        private float pop3;
        private float pop4;
        private float pop5;

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

        public Wind Wind
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

        public string City
        {
            get
            {
                return city;
            }

            set
            {
                SetProperty(ref city, value);
            }
        }

        public float TempraturePath0
        {
            get
            {
                return tempraturePath0;
            }

            set
            {
                SetProperty(ref tempraturePath0, value);
            }
        }

        public float TempraturePath1
        {
            get
            {
                return tempraturePath1;
            }

            set
            {
                SetProperty(ref tempraturePath1, value);
            }
        }

        public float TempraturePath2
        {
            get
            {
                return tempraturePath2;
            }

            set
            {
                SetProperty(ref tempraturePath2, value);
            }
        }

        public float TempraturePath3
        {
            get
            {
                return tempraturePath3;
            }

            set
            {
                SetProperty(ref tempraturePath3, value);
            }
        }

        public float TempraturePath4
        {
            get
            {
                return tempraturePath4;
            }

            set
            {
                SetProperty(ref tempraturePath4, value);
            }
        }

        public float TempraturePath5
        {
            get
            {
                return tempraturePath5;
            }

            set
            {
                SetProperty(ref tempraturePath5, value);
            }
        }

        public Temprature HourlyTemp0
        {
            get
            {
                return hourlyTemp0;
            }

            set
            {
                SetProperty(ref hourlyTemp0, value);
            }
        }

        public Temprature HourlyTemp1
        {
            get
            {
                return hourlyTemp1;
            }

            set
            {
                SetProperty(ref hourlyTemp1, value);
            }
        }

        public Temprature HourlyTemp2
        {
            get
            {
                return hourlyTemp2;
            }

            set
            {
                SetProperty(ref hourlyTemp2, value);
            }
        }

        public Temprature HourlyTemp3
        {
            get
            {
                return hourlyTemp3;
            }

            set
            {
                SetProperty(ref hourlyTemp3, value);
            }
        }

        public Temprature HourlyTemp4
        {
            get
            {
                return hourlyTemp4;
            }

            set
            {
                SetProperty(ref hourlyTemp4, value);
            }
        }

        public Temprature HourlyTemp5
        {
            get
            {
                return hourlyTemp5;
            }

            set
            {
                SetProperty(ref hourlyTemp5, value);
            }
        }

        public DateTime Hour0
        {
            get
            {
                return hour0;
            }

            set
            {
                SetProperty(ref hour0, value);
            }
        }

        public DateTime Hour1
        {
            get
            {
                return hour1;
            }

            set
            {
                SetProperty(ref hour1, value);
            }
        }

        public DateTime Hour2
        {
            get
            {
                return hour2;
            }

            set
            {
                SetProperty(ref hour2, value);
            }
        }

        public DateTime Hour3
        {
            get
            {
                return hour3;
            }

            set
            {
                SetProperty(ref hour3, value);
            }
        }

        public DateTime Hour4
        {
            get
            {
                return hour4;
            }

            set
            {
                SetProperty(ref hour4, value);
            }
        }

        public DateTime Hour5
        {
            get
            {
                return hour5;
            }

            set
            {
                SetProperty(ref hour5, value);
            }
        }

        public float Pop0
        {
            get
            {
                return pop0;
            }

            set
            {
                SetProperty(ref pop0, value);
            }
        }

        public float Pop1
        {
            get
            {
                return pop1;
            }

            set
            {
                SetProperty(ref pop1, value);
            }
        }

        public float Pop2
        {
            get
            {
                return pop2;
            }

            set
            {
                SetProperty(ref pop2, value);
            }
        }

        public float Pop3
        {
            get
            {
                return pop3;
            }

            set
            {
                SetProperty(ref pop3, value);
            }
        }

        public float Pop4
        {
            get
            {
                return pop4;
            }

            set
            {
                SetProperty(ref pop4, value);
            }
        }

        public float Pop5
        {
            get
            {
                return pop5;
            }

            set
            {
                SetProperty(ref pop5, value);
            }
        }

        public NowWeatherPageViewModel()
        {
            var task = ThreadPool.RunAsync(async (work) =>
            {
                try
                {
                    ReadSettings();
                    await FetchData();

                }
                catch (ArgumentNullException)
                {
                    return;
                }
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(() =>
                  {
                      InitialViewModel();
                  }));
            });
        }

        public event FetchDataCompleteEventHandler FetchDataComplete;

        private async Task FetchData()
        {



            if (id != null)
            {
                var keys = (await FileIOHelper.ReadStringFromAssets("Key")).Split(new string[] { ":|:" }, StringSplitOptions.RemoveEmptyEntries);
                var param = new string[] { "cityid=" + id };
#if DEBUG
                var resstr = await FileIOHelper.ReadStringFromAssets("testdata");
#else
                var resstr = await BaiduRequestHelper.RequestWithKey("http://apis.baidu.com/heweather/pro/weather", param, keys[0]);    
#endif
                var resjson = HeWeatherContract.Generate(resstr);
                fetchresult = new HeWeatherModel(resjson);
            }
            else throw new NullReferenceException();

        }

        private void NotifyFetchDataComplete()
        {
            var h = FetchDataComplete;
            if (h != null)
            {
                FetchDataComplete(this, new FetchDataCompleteEventArgs());
            }
        }

        private void InitialViewModel()
        {
            var c = city;
            city = null;
            City = c;
            Temprature = fetchresult.NowWeather.Temprature;
            Wind = fetchresult.NowWeather.Wind;
            Condition = fetchresult.NowWeather.Now.Condition;
            List<float> pathResults = new List<float>();
            for (int i = 0; i < 6; i++)
            {
                pathResults.Add(fetchresult.HourlyForecast[i].Temprature.Celsius);
            }
            var min = 0f;
            var max = min;
            foreach (var data in pathResults)
            {
                if (data < min)
                {
                    min = data;
                    continue;
                }
                if (data > max)
                {
                    max = data;
                }
            }
            var avg = (max + min) / 2;
            for (int j = 0; j < pathResults.Count; j++)
            {
                pathResults[j] -= avg;
            }

            TempraturePath0 = pathResults[0] / (max - min);
            TempraturePath1 = pathResults[1] / (max - min);
            TempraturePath2 = pathResults[2] / (max - min);
            TempraturePath3 = pathResults[3] / (max - min);
            TempraturePath4 = pathResults[4] / (max - min);
            TempraturePath5 = pathResults[5] / (max - min);

            HourlyTemp0 = fetchresult.HourlyForecast[0].Temprature;
            HourlyTemp1 = fetchresult.HourlyForecast[1].Temprature;
            HourlyTemp2 = fetchresult.HourlyForecast[2].Temprature;
            HourlyTemp3 = fetchresult.HourlyForecast[3].Temprature;
            HourlyTemp4 = fetchresult.HourlyForecast[4].Temprature;
            HourlyTemp5 = fetchresult.HourlyForecast[5].Temprature;

            Hour0 = fetchresult.HourlyForecast[0].DateTime;
            Hour1 = fetchresult.HourlyForecast[1].DateTime;
            Hour2 = fetchresult.HourlyForecast[2].DateTime;
            Hour3 = fetchresult.HourlyForecast[3].DateTime;
            Hour4 = fetchresult.HourlyForecast[4].DateTime;
            Hour5 = fetchresult.HourlyForecast[5].DateTime;

            Pop0 = fetchresult.HourlyForecast[0].Pop / 100f;
            Pop1 = fetchresult.HourlyForecast[1].Pop / 100f;
            Pop2 = fetchresult.HourlyForecast[2].Pop / 100f;
            Pop3 = fetchresult.HourlyForecast[3].Pop / 100f;
            Pop4 = fetchresult.HourlyForecast[4].Pop / 100f;
            Pop5 = fetchresult.HourlyForecast[5].Pop / 100f;

            this.NotifyFetchDataComplete();
        }

        private void ReadSettings()
        {
#if DEBUG
            city = "beijing";
            id = "CA1000011";
#else
            var settings = SettingsModel.ReadSettings();
            city = settings.SavedCities[0].City;
            id = settings.SavedCities[0].Id;
#endif
        }
    }
}
