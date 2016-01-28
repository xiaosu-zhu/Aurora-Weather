﻿using com.aurora.auweather.Models;
using com.aurora.auweather.Models.HeWeather;
using com.aurora.auweather.Models.HeWeather.JsonContract;
using com.aurora.shared.Helpers;
using com.aurora.shared.MVVM;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.System.Threading;
using Windows.UI.Core;

namespace com.aurora.auweather.ViewModels
{
    internal class MainPageViewModel : ViewModelBase
    {
        private Temprature temprature;
        private Wind wind;
        private WeatherCondition condition;
        private string city;
        private string id;
        private HeWeatherModel fetchresult;
        private double[] tempraturePath = new double[] { 0, 0, 0, 0, 0, 0, 0 };


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

        public double[] TempraturePath
        {
            get
            {
                return tempraturePath;
            }

            set
            {
                SetProperty(ref tempraturePath, value);
            }
        }

        public MainPageViewModel()
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

        private async Task FetchData()
        {
            if (id != null)
            {
                var keys = (await FileIOHelper.ReadStringFromAssets("Key")).Split(new string[] { ":|:" }, StringSplitOptions.RemoveEmptyEntries);
                var param = new string[] { "cityid=" + id };
                var resstr = //await BaiduRequestHelper.RequestWithKey("http://apis.baidu.com/heweather/pro/weather", param, keys[0]);
                             await FileIOHelper.ReadStringFromAssets("testdata");
                var resjson = HeWeatherContract.Generate(resstr);
                fetchresult = new HeWeatherModel(resjson);
            }
            else throw new NullReferenceException();
        }

        private void InitialViewModel()
        {
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
            TempraturePath = pathResults.ToArray();
        }

        private void ReadSettings()
        {
            var settings = SettingsModel.ReadSettings();
            City = settings.SavedCities[0].City;
            id = settings.SavedCities[0].Id;
        }
    }
}
