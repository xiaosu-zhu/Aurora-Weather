// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.Models.Settings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.System.Threading;
using Com.Aurora.Shared.MVVM;
using Com.Aurora.Shared.Extensions;
using Com.Aurora.AuWeather.ViewModels.Events;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.Devices.Geolocation;
using Com.Aurora.Shared.Helpers;
using Com.Aurora.AuWeather.Models.HeWeather;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Foundation;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using System.Linq;
using Com.Aurora.AuWeather.Models;
using Com.Aurora.AuWeather.Core.SQL;
using System.Linq.Expressions;

namespace Com.Aurora.AuWeather.ViewModels
{
    internal class CitiesSettingViewModel : ViewModelBase
    {
        private Models.Settings.Cities Cities;
        private bool enablePosition;
        private CitySettingsModel locatedCity;
        private bool? m_islocatedcurrent;
        private List<CitySettingsModel> newlist;
        private ElementTheme theme;
        private object lockable;

        public event EventHandler<FetchDataCompleteEventArgs> FetchDataComplete;
        public event EventHandler<FetchDataCompleteEventArgs> LocateComplete;

        public CitiesSettingViewModel()
        {
            lockable = new object();
            var p = SettingsModel.Current.Preferences;
            Theme = p.GetTheme();
            var task = ThreadPool.RunAsync(async (work) =>
            {
                Cities = SettingsModel.Current.Cities;
                Info = new CitiesInfo();
                if (!Cities.SavedCities.IsNullorEmpty())
                    foreach (var city in Cities.SavedCities)
                    {
                        Info.Add(new CitySettingsViewModel(city.City, city.Id, city.IsCurrent));
                    }
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(() =>
                {
                    EnablePosition = Cities.EnableLocate;
                    if (Cities.CurrentIndex == -1)
                    {
                        Is_Located_Current = true;
                    }
                }));
                OnRefreshComplete();
            });
        }

        internal void SaveAll()
        {
            Cities.Save();
        }

        private void OnRefreshComplete()
        {
            this.FetchDataComplete?.Invoke(this, new FetchDataCompleteEventArgs());
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

        public ElementTheme Theme
        {
            get
            {
                return theme;
            }

            set
            {
                SetProperty(ref theme, value);
            }
        }

        internal void Complete()
        {
            Cities.Save();
            LocalSettingsHelper.WriteSettingsValue("Inited", true);
        }

        internal bool CheckCompleted()
        {
            return !Cities.SavedCities.IsNullorEmpty() || Cities.EnableLocate;
        }

        internal async Task CalcPosition(Geoposition pos)
        {
            try
            {
                var final = await Models.Location.ReverseGeoCode((float)pos.Coordinate.Point.Position.Latitude, (float)pos.Coordinate.Point.Position.Longitude, SettingsModel.Current.Cities.Routes);
                if (Cities.LocatedCity != null && Cities.LocatedCity.Id == final.Id)
                {
                    if (Cities.SavedCities.IsNullorEmpty())
                    {
                        Cities.CurrentIndex = -1;
                    }
                    Cities.Save();
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(() =>
                    {
                        LocatedCity = Cities.LocatedCity;
                        if (Cities.CurrentIndex == -1)
                        {
                            Is_Located_Current = true;
                        }
                        this.OnLocateComplete();
                    }));
                    return;
                }
                var p = final;
                p.Latitude = (float)pos.Coordinate.Point.Position.Latitude;
                p.Longitude = (float)pos.Coordinate.Point.Position.Longitude;
                Cities.LocatedCity = new CitySettingsModel(p);
                if (Cities.SavedCities.IsNullorEmpty())
                {
                    Cities.CurrentIndex = -1;
                }
                Cities.Save();
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(() =>
                {
                    LocatedCity = Cities.LocatedCity;
                    if (Cities.CurrentIndex == -1)
                    {
                        Is_Located_Current = true;
                    }
                    this.OnLocateComplete();
                }));
            }
            catch (Exception)
            {
                FailtoPos();
            }

        }

        private void FailtoPos()
        {

        }

        private void OnLocateComplete()
        {
            this.LocateComplete?.Invoke(this, new FetchDataCompleteEventArgs());
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
            SaveAll();
        }


        internal List<City> Search_TextChanged(string text)
        {
            var searcharray = text.Split(' ');
            StringBuilder searchsb = new StringBuilder(@".*");
            foreach (var search in searcharray)
            {
                searchsb.Append(search);
                searchsb.Append(@".*");
            }
            var pattern = searchsb.ToString();
            var list = SQLAction.GetAll();
            return list.FindAll((x) =>
            {
                return x.CountryCode == "CN" ? (Regex.IsMatch(x.CityZh, pattern, RegexOptions.IgnoreCase) || Regex.IsMatch(x.CityEn, pattern, RegexOptions.IgnoreCase)) : Regex.IsMatch(x.CityEn, pattern, RegexOptions.IgnoreCase);
            });
        }

        internal int GetIndex()
        {
            return (Cities.EnableLocate && Cities.LocatedCity != null) ? Cities.CurrentIndex + 1 : Cities.CurrentIndex;
        }

        internal ICollection<CitySettingsViewModel> GetCities()
        {
            var c = Cities.SavedCities;
            List<CitySettingsViewModel> l = new List<CitySettingsViewModel>();
            if (Cities.EnableLocate && Cities.LocatedCity != null)
            {
                l.Add(new CitySettingsViewModel(Cities.LocatedCity.City, Cities.LocatedCity.Id, Cities.LocatedCity.IsCurrent));
            }
            if (!c.IsNullorEmpty())
            {
                foreach (var item in c)
                {
                    l.Add(new CitySettingsViewModel(item.City, item.Id, item.IsCurrent));
                }
            }

            return l;
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

        internal void AddCity(City cityInfo)
        {
            var task = ThreadPool.RunAsync((work) =>
            {
                lock (lockable)
                {
                    newlist = null;
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
                }
            });
            task.Completed = new AsyncActionCompletedHandler(AddCityComplete);
        }

        internal void AddCity(string queryText)
        {
            var task = ThreadPool.RunAsync((work) =>
            {
                lock (lockable)
                {
                    newlist = null;
                    var list = SQLAction.GetAll();
                    var result = list.Find((x) =>
                    {
                        return x.CountryCode == "CN" ? x.CityZh.Equals(queryText, StringComparison.OrdinalIgnoreCase) : x.CityEn.Equals(queryText, StringComparison.OrdinalIgnoreCase);
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
                }
            });
            task.Completed = new AsyncActionCompletedHandler(AddCityComplete);
        }

        internal void ReArrange()
        {
            var list = new List<CitySettingsModel>();
            foreach (var item in Info)
            {
                list.Add(Array.Find(Cities.SavedCities, x =>
                {
                    return x.Id == item.Id;
                }));
            }
            Cities.CurrentIndex = list.FindIndex(x =>
            {
                return x.IsCurrent;
            });
            Cities.Save(list.ToArray());
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
