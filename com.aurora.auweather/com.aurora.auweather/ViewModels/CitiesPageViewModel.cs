using Com.Aurora.AuWeather.Models;
using Com.Aurora.Shared.MVVM;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.System.Threading;
using Com.Aurora.AuWeather.Models.Settings;
using Com.Aurora.AuWeather.ViewModels.Events;
using Com.Aurora.Shared.Helpers;
using Com.Aurora.AuWeather.Models.HeWeather.JsonContract;
using Com.Aurora.AuWeather.Models.HeWeather;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;
using Com.Aurora.AuWeather.Tile;
using System.Collections.Generic;
using Com.Aurora.Shared.Extensions;
using Windows.UI.StartScreen;

namespace Com.Aurora.AuWeather.ViewModels
{
    public class CitiesPageViewModel : ViewModelBase
    {
        private SettingsModel settings = SettingsModel.Get();

        public event EventHandler<LocationUpdateEventArgs> LocationUpdate;

        public Cities Cities { get; set; } = new Cities();

        private int currentIndex;

        public int CurrentIndex
        {
            get
            {
                return currentIndex;
            }
            set
            {
                SetProperty(ref currentIndex, value);
            }
        }

        public CitiesPageViewModel()
        {
            if (settings.Cities.EnableLocate)
            {
                if (settings.Cities.LocatedCity != null)
                    Cities.Add(new CityViewModel(settings.Cities.LocatedCity));
            }
            foreach (var city in settings.Cities.SavedCities)
            {
                Cities.Add(new CityViewModel(city));
            }
            var task = ThreadPool.RunAsync(async (work) =>
            {
                await SearchExisitingData();

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(async () =>
                {
                    await Init();
                    var t = ThreadPool.RunAsync((x) =>
                    {
                        Update();
                        RequireLocationUpdate();
                    });
                }));
            });
        }


        internal void DeniePos()
        {
            if (settings.Cities.LocatedCity != null && settings.Cities.EnableLocate)
            {
                Cities.RemoveAt(0);
            }
            settings.Cities.LocatedCity = null;
        }

        private void Update()
        {
            foreach (var item in Cities)
            {
                if (!item.Updated)
                {
                    var task = ThreadPool.RunAsync(async (work) =>
                    {
                        var keys = (await FileIOHelper.ReadStringFromAssetsAsync("Key")).Split(new string[] { ":|:" }, StringSplitOptions.RemoveEmptyEntries);
                        var param = new string[] { "cityid=" + item.Id };
                        var resstr = await BaiduRequestHelper.RequestWithKeyAsync("http://apis.baidu.com/heweather/pro/weather", param, keys[0]);
                        item.data = resstr;
                        await settings.Cities.SaveDataAsync(item.Id, resstr);
                        var index = Array.FindIndex(settings.Cities.SavedCities, x =>
                        {
                            return x.Id == item.Id;
                        });
                        if (index != -1)
                        {
                            settings.Cities.SavedCities[index].Update();
                        }
                        else
                        {
                            settings.Cities.LocatedCity.Update();
                        }
                        settings.Cities.Save();
                        await Complete(item);
                    });
                }
            }
        }

        internal void UpdateLocation(CityInfo cityInfo)
        {
            if (cityInfo.Id == settings.Cities.LocatedCity.Id)
            {
                return;
            }
            else
            {
                var c = new CitySettingsModel(cityInfo);
                if (settings.Cities.LocatedCity != null)
                {
                    Cities.RemoveAt(0);
                    Cities.Insert(0, new CityViewModel(c));
                }
                else
                {
                    Cities.Insert(0, new CityViewModel(c));
                }
                settings.Cities.LocatedCity = c;
                UpdateLocated();
            }
        }

        internal async void Refresh()
        {
            foreach (var item in Cities)
            {
                item.Updated = false;
            }
            await SearchExisitingData();
            Update();
        }


        internal void Delete(IList<object> selectedItems)
        {
            var its = new object[selectedItems.Count];
            selectedItems.CopyTo(its, 0);
            if (its.IsNullorEmpty())
            {
                return;
            }
            foreach (CityViewModel item in its)
            {
                Cities.Remove(item);
            }

            var t = ThreadPool.RunAsync((work) =>
            {
                var citys = new List<CitySettingsModel>();
                citys.AddRange(settings.Cities.SavedCities);
                foreach (CityViewModel c in its)
                {
                    var index = citys.FindIndex(x =>
                    {
                        return x.Id == c.Id;
                    });
                    if (index == -1 && settings.Cities.LocatedCity != null)
                    {
                        settings.Cities.LocatedCity = null;
                    }
                    else
                    {
                        citys.RemoveAt(index);
                    }
                }
                settings.Cities.Save(citys.ToArray());
            });
        }

        internal async void Pin(IList<object> selectedItems)
        {
            if (selectedItems == null || selectedItems.Count == 0)
            {
                return;
            }

            foreach (CityViewModel item in selectedItems)
            {
                SecondaryTile s = new SecondaryTile(item.Id, item.City, item.City, item.Id,
                    TileOptions.CopyOnDeployment | TileOptions.ShowNameOnLogo | TileOptions.ShowNameOnWideLogo,
                    new Uri("ms-appx:///Assets/Square150x150Logo.png"), new Uri("ms-appx:///Assets/Wide310x150Logo.png"));
                await s.RequestCreateAsync();
                var index = Array.FindIndex(settings.Cities.SavedCities, x =>
                {
                    return x.Id == item.Id;
                });
                CitySettingsModel currentCity;
                if (index == -1 && settings.Cities.LocatedCity != null)
                {
                    settings.Cities.LocatedCity.IsPinned = true;
                    currentCity = settings.Cities.LocatedCity;
                }
                else
                {
                    settings.Cities.SavedCities[index].IsPinned = true;
                    currentCity = settings.Cities.SavedCities[index];
                }
                var t = ThreadPool.RunAsync(async (work) =>
                {
                    string resstr;
                    try
                    {
                        resstr = await FileIOHelper.ReadStringFromStorageAsync(item.Id);
                    }
                    catch (Exception)
                    {
                        var keys = (await FileIOHelper.ReadStringFromAssetsAsync("Key")).Split(new string[] { ":|:" }, StringSplitOptions.RemoveEmptyEntries);
                        var param = new string[] { "cityid=" + item.Id };
                        resstr = await BaiduRequestHelper.RequestWithKeyAsync("http://apis.baidu.com/heweather/pro/weather", param, keys[0]);
                    }
                    var resjson = HeWeatherContract.Generate(resstr);
                    var weather = new HeWeatherModel(resjson);
                    var todayIndex = Array.FindIndex(weather.DailyForecast, x =>
                    {
                        return x.Date.Date == DateTime.Today.Date;
                    });
                    var hourIndex = Array.FindIndex(weather.HourlyForecast, x =>
                    {
                        return (x.DateTime - DateTime.Now).TotalSeconds > 0;
                    });
                    var isNight = Generator.CalcIsNight(weather.Location.UpdateTime, weather.DailyForecast[todayIndex].SunRise, weather.DailyForecast[todayIndex].SunSet);
                    var glanceFull = Glance.GenerateGlanceDescription(weather, isNight, settings.Preferences.TemperatureParameter, DateTime.Now);
                    var glance = Glance.GenerateShortDescription(weather, isNight);
                    var uri = await settings.Immersive.GetCurrentBackgroundAsync(weather.NowWeather.Now.Condition, isNight);
                    Sender.CreateSubTileNotification(Generator.GenerateNormalTile(weather, isNight, glance, glanceFull, uri, todayIndex, currentCity, settings), item.Id);
                });
            }
        }

        private void UpdateLocated()
        {
            var t = ThreadPool.RunAsync(async (work) =>
            {
                var keys = (await FileIOHelper.ReadStringFromAssetsAsync("Key")).Split(new string[] { ":|:" }, StringSplitOptions.RemoveEmptyEntries);
                var param = new string[] { "cityid=" + settings.Cities.LocatedCity.Id };
                var resstr = await BaiduRequestHelper.RequestWithKeyAsync("http://apis.baidu.com/heweather/pro/weather", param, keys[0]);
                Cities[0].data = resstr;
                await settings.Cities.SaveDataAsync(Cities[0].Id, resstr);
                settings.Cities.LocatedCity.Update();
                settings.Cities.Save();
                await Complete(Cities[0]);
            });
        }

        internal void ChangeCurrent(int selectedIndex)
        {
            if (settings.Cities.LocatedCity != null)
            {
                settings.Cities.CurrentIndex = selectedIndex - 1;
            }
            else
            {
                settings.Cities.CurrentIndex = selectedIndex;
            }
            var t = ThreadPool.RunAsync((work) =>
            {
                settings.Cities.Save();
            });
        }

        private async Task Complete(CityViewModel item)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(async () =>
            {
                if (item.data != null)
                {
                    var resjson = HeWeatherContract.Generate(item.data);
                    var weather = new HeWeatherModel(resjson);
                    await itemInit(item, weather);
                    item.Updated = true;
                    item.LastUpdate = DateTime.Now;
                }
            }));
        }

        private async Task itemInit(CityViewModel item, HeWeatherModel weather)
        {
            item.NowCondition = weather.NowWeather.Now.Condition;
            var todayIndex = Array.FindIndex(weather.DailyForecast, x =>
            {
                return x.Date.Date == DateTime.Today.Date;
            });
            var hourIndex = Array.FindIndex(weather.HourlyForecast, x =>
            {
                return (x.DateTime - DateTime.Now).TotalSeconds > 0;
            });
            var isNight = Generator.CalcIsNight(weather.Location.UpdateTime, weather.DailyForecast[todayIndex].SunRise, weather.DailyForecast[todayIndex].SunSet);
            item.Glance = Glance.GenerateGlanceDescription(weather, isNight, settings.Preferences.TemperatureParameter, DateTime.Now);
            item.Background = new BitmapImage(await settings.Immersive.GetCurrentBackgroundAsync(weather.NowWeather.Now.Condition, isNight));
            item.data = null;
        }

        private async Task Init()
        {
            foreach (var item in Cities)
            {
                if (item.data != null)
                {
                    var resjson = HeWeatherContract.Generate(item.data);
                    var weather = new HeWeatherModel(resjson);
                    await itemInit(item, weather);
                    item.Updated = true;

                }
            }
            if (settings.Cities.EnableLocate && settings.Cities.LocatedCity != null)
            {
                CurrentIndex = settings.Cities.CurrentIndex + 1;
            }
            else
            {
                CurrentIndex = settings.Cities.CurrentIndex;
            }
        }

        private async Task SearchExisitingData()
        {
            foreach (var item in Cities)
            {
                var currentTime = DateTime.Now;
                if ((currentTime - item.LastUpdate).TotalMinutes < 60)
                {
                    try
                    {
                        var data = await FileIOHelper.ReadStringFromStorageAsync(item.Id);
                        item.data = data;
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
        }

        private void RequireLocationUpdate()
        {
            var h = this.LocationUpdate;
            if (h != null)
            {
                h(this, new LocationUpdateEventArgs());
            }
        }
    }

    public class Cities : ObservableCollection<CityViewModel>
    {

    }

    public class CityViewModel : ViewModelBase
    {
        private string city;
        private string id;
        private WeatherCondition nowCondition;
        private bool updated;
        private DateTime lastUpdate;
        internal string data;
        private BitmapImage background;
        private string glance;

        public CityViewModel(CitySettingsModel locatedCity)
        {
            Id = locatedCity.Id;
            City = locatedCity.City;
            LastUpdate = locatedCity.LastUpdate;
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
        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                SetProperty(ref id, value);
            }
        }
        public WeatherCondition NowCondition
        {
            get
            {
                return nowCondition;
            }
            set
            {
                SetProperty(ref nowCondition, value);
            }
        }
        public bool Updated
        {
            get
            {
                return updated;
            }
            set
            {
                SetProperty(ref updated, value);
            }
        }

        public DateTime LastUpdate
        {
            get
            {
                return lastUpdate;
            }

            set
            {
                SetProperty(ref lastUpdate, value);
            }
        }

        public BitmapImage Background
        {
            get
            {
                return background;
            }

            set
            {
                SetProperty(ref background, value);
            }
        }

        public string Glance
        {
            get
            {
                return glance;
            }

            set
            {
                SetProperty(ref glance, value);
            }
        }
    }
}
