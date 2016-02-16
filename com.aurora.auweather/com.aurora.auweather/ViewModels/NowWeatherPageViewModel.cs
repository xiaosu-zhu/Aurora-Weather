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
        private double tempraturePath0;
        private double tempraturePath1;
        private double tempraturePath2;
        private double tempraturePath3;
        private double tempraturePath4;
        private double tempraturePath5;
        private double tempraturePath6;

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

        public double TempraturePath0
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

        public double TempraturePath1
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

        public double TempraturePath2
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

        public double TempraturePath3
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

        public double TempraturePath4
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

        public double TempraturePath5
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

        public double TempraturePath6
        {
            get
            {
                return tempraturePath6;
            }

            set
            {
                SetProperty(ref tempraturePath6, value);
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
            List<double> pathResults = new List<double>();
            for (int i = 0; i < 7; i++)
            {
                pathResults.Add(fetchresult.HourlyForecast[i].Temprature.Celsius);
            }
            var min = 0d;
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
            TempraturePath6 = pathResults[6] / (max - min);

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
