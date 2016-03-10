using Com.Aurora.AuWeather.Models.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.System.Threading;
using Com.Aurora.Shared.MVVM;
using Com.Aurora.Shared.Extensions;
using Com.Aurora.AuWeather.ViewModels.Events;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.Devices.Geolocation;
using Com.Aurora.Shared.Helpers;
using Com.Aurora.AuWeather.Models.HeWeather.JsonContract;
using Com.Aurora.AuWeather.Models.HeWeather;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Foundation;
using System.Collections.ObjectModel;

namespace Com.Aurora.AuWeather.ViewModels
{
    internal class CitiesSettingViewModel : ViewModelBase
    {
        private Cities Cities;
        private bool enablePosition;
        private CitySettingsModel locatedCity;
        private List<CityInfo> cities;
        private bool? m_islocatedcurrent;
        private List<CitySettingsModel> newlist;

        public event EventHandler<FetchDataCompleteEventArgs> FetchDataComplete;
        public event EventHandler<FetchDataCompleteEventArgs> LocateComplete;

        public CitiesSettingViewModel()
        {
            var task = ThreadPool.RunAsync(async (work) =>
            {
                Cities = Cities.Get();
                Info = new CitiesInfo();
                if (!Cities.SavedCities.IsNullorEmpty())
                    foreach (var city in Cities.SavedCities)
                    {
                        Info.Add(new CitySettingsViewModel(city.City, city.Id, city.IsCurrent));
                    }
                enablePosition = Cities.EnableLocate;
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(() =>
                {
                    var e = enablePosition;
                    enablePosition = !enablePosition;
                    EnablePosition = e;
                    if (Cities.CurrentIndex == -1)
                    {
                        Is_Located_Current = true;
                    }
                }));
                var data = await FileIOHelper.ReadStringFromAssetsAsync("cityid.txt");
                var result = JsonHelper.FromJson<CityIdContract>(data);
                cities = CityInfo.CreateList(result);
                OnRefreshComplete();
                data = null;
                result = null;
            });
        }

        internal void SaveAll()
        {
            var task = ThreadPool.RunAsync((work) =>
             {
                 Cities.Save();
             });
        }

        private void OnRefreshComplete()
        {
            var h = FetchDataComplete;
            if (h != null)
            {
                FetchDataComplete(this, new FetchDataCompleteEventArgs());
            }
        }

        public bool EnablePosition
        {
            get
            {
                return enablePosition;
            }
            set
            {
                SetProperty(ref enablePosition, value);
            }
        }

        public CitySettingsModel LocatedCity
        {
            get
            {
                return locatedCity;
            }
            set
            {
                SetProperty(ref locatedCity, value);
            }
        }

        public CitiesInfo Info { get; private set; }
        public bool? Is_Located_Current
        {
            get
            {
                return m_islocatedcurrent;
            }
            set
            {
                m_islocatedcurrent = !value;
                SetProperty(ref m_islocatedcurrent, value);
            }
        }

        internal async Task CalcPosition(Geoposition pos)
        {

            var final = Models.Location.GetNearsetLocation(cities,
                new Models.Location((float)pos.Coordinate.Point.Position.Latitude, (float)pos.Coordinate.Point.Position.Longitude));
            Cities.LocatedCity = new CitySettingsModel(final.ToArray()[0]);
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(() =>
            {
                LocatedCity = Cities.LocatedCity;
                this.OnLocateComplete();
            }));
        }

        private void OnLocateComplete()
        {
            var h = LocateComplete;
            if (h != null)
            {
                LocateComplete(this, new FetchDataCompleteEventArgs());
            }
        }

        internal void ChangePosition(bool isOn)
        {
            EnablePosition = isOn;
            Cities.EnableLocate = isOn;
            SaveAll();
        }

        internal void SetCurrent_Locate()
        {
            Cities.CurrentIndex = -1;
            Is_Located_Current = true;
            SaveAll();
        }

        internal void SetCurrent(CitySettingsViewModel citySettingsViewModel)
        {
            Cities.CurrentIndex = Array.FindIndex(Cities.SavedCities, (x) =>
            {
                return x.Id == citySettingsViewModel.Id;
            });
            if (Cities.CurrentIndex != -1)
                Info[Cities.CurrentIndex].IsCurrent = true;

        }


        internal List<CityInfo> Search_TextChanged(string text)
        {
            var searcharray = text.Split(' ');
            StringBuilder searchsb = new StringBuilder(@".*");
            foreach (var search in searcharray)
            {
                searchsb.Append(search);
                searchsb.Append(@".*");
            }
            var pattern = searchsb.ToString();
            return cities.FindAll(x =>
            {
                if (x.Country == "中国")
                {
                    var pin = PinYinHelper.GetPinyin(x.City);
                    return (Regex.IsMatch(pin, pattern, RegexOptions.IgnoreCase) || Regex.IsMatch(x.City, pattern, RegexOptions.IgnoreCase));
                }
                return Regex.IsMatch(x.City, pattern, RegexOptions.IgnoreCase);
            });
        }

        internal void DeleteCity(CitySettingsViewModel citySettingsViewModel)
        {
            newlist = new List<CitySettingsModel>();
            newlist.AddRange(Cities.SavedCities);
            var removeIndex = newlist.FindIndex(x =>
           {
               return x.Id == citySettingsViewModel.Id;
           });
            Info.RemoveAt(removeIndex);
            var task = ThreadPool.RunAsync((work) =>
             {
                 newlist.RemoveAt(removeIndex);
                 Cities.Save(newlist.ToArray());
             });
            if (Cities.CurrentIndex >= removeIndex)
            {
                Cities.CurrentIndex -= 1;
            }
            if (Cities.CurrentIndex == -1)
            {
                Is_Located_Current = true;
            }
            else
            {
                Info[Cities.CurrentIndex].IsCurrent = true;
            }
        }

        internal void AddCity(CityInfo cityInfo)
        {
            var task = ThreadPool.RunAsync((work) =>
            {
                newlist.Clear();
                if (Array.Exists(Cities.SavedCities, x =>
            {
                return x.Id == cityInfo.Id;
            }))
                {
                    return;
                }
                else
                {
                    newlist = new List<CitySettingsModel>();
                    newlist.AddRange(Cities.SavedCities);
                    newlist.Add(new CitySettingsModel(cityInfo));
                    Cities.Save(newlist.ToArray());
                }
            });
            task.Completed = new AsyncActionCompletedHandler(AddCityComplete);
        }

        internal void AddCity(string queryText)
        {
            var task = ThreadPool.RunAsync((work) =>
            {
                var result = cities.Find(x =>
                {
                    return x.City == queryText;
                });
                if (result != null)
                {
                    if (Array.Exists(Cities.SavedCities, x =>
                    {
                        return x.Id == result.Id;
                    }))
                    {
                        return;
                    }
                    else
                    {
                        newlist = new List<CitySettingsModel>();
                        newlist.AddRange(Cities.SavedCities);
                        newlist.Add(new CitySettingsModel(result));
                        Cities.Save(newlist.ToArray());
                    }
                }
            });
            task.Completed = new AsyncActionCompletedHandler(AddCityComplete);
        }

        private async void AddCityComplete(IAsyncAction asyncInfo, AsyncStatus asyncStatus)
        {
            if (!newlist.IsNullorEmpty())
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(() =>
                {
                    Info.Add(new CitySettingsViewModel(newlist[newlist.Count - 1].City, newlist[newlist.Count - 1].Id, newlist[newlist.Count - 1].IsCurrent));
                }));
        }
    }

    internal class CitiesInfo : ObservableCollection<CitySettingsViewModel>
    {

    }

    internal class CitySettingsViewModel : ViewModelBase
    {
        private string city;
        private string id;
        private bool? isCurrent;

        public CitySettingsViewModel(string city, string id, bool isCurrent)
        {
            City = city;
            Id = id;
            IsCurrent = isCurrent;
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

        public bool? IsCurrent
        {
            get
            {
                return isCurrent;
            }

            set
            {
                isCurrent = !value;
                SetProperty(ref isCurrent, value);
            }
        }
    }

    internal class Info
    {
        public Info(string title, string iD)
        {
            Title = title;
            ID = iD;
        }

        public string Title { get; set; }
        public string ID { get; set; }
    }
}
