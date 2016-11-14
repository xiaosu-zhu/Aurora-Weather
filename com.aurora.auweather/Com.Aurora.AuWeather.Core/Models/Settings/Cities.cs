// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.Models.HeWeather;
using Com.Aurora.Shared.Extensions;
using Com.Aurora.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Aurora.AuWeather.Models.Settings
{
    public class Cities
    {
        public CitySettingsModel[] SavedCities { get; private set; }

        public LocateRoute[] Routes { get; private set; } = new LocateRoute[4] { LocateRoute.Amap, LocateRoute.Omap, LocateRoute.IP, LocateRoute.unknown };

        public int CurrentIndex { get; set; }

        public bool EnableLocate { get; set; } = true;
        public CitySettingsModel LocatedCity { get; set; }

        public static Cities Get()
        {
            Cities c = new Cities();
            try
            {
                var container = RoamingSettingsHelper.GetContainer("Cities");
                var subContainer = container.GetContainer("Locate");
                var enumContainer = container.GetContainer("Routes");
                c.CurrentIndex = (int)container.Values["CurrentIndex"];
                c.EnableLocate = (bool)container.Values["EnableLocate"];
                CitySettingsModel loc;

                if (subContainer.ReadGroupSettings(out loc))
                {
                    if (loc.City != null && loc.Id != null)
                    {
                        c.LocatedCity = loc;
                        if (c.CurrentIndex == -1)
                        {
                            if (c.EnableLocate == true)
                                c.LocatedCity.IsCurrent = true;
                            else
                            {
                                c.CurrentIndex = 0;
                            }
                        }
                    }
                }

                int i = (int)container.Values["Count"];
                List<CitySettingsModel> cs = new List<CitySettingsModel>();
                for (int j = 0; j < i; j++)
                {
                    CitySettingsModel m;
                    var sub = RoamingSettingsHelper.GetContainer(j.ToString());
                    if (sub.ReadGroupSettings(out m))
                    {
                        cs.Add(m);
                    }
                }
                c.SavedCities = cs.ToArray();
                if (c.CurrentIndex != -1 && !c.SavedCities.IsNullorEmpty())
                {
                    c.SavedCities[c.CurrentIndex].IsCurrent = true;
                }
                int l = (int)enumContainer.Values["Count"];
                List<LocateRoute> li = new List<LocateRoute>();
                for (int m = 0; m < l; m++)
                {
                    var toure = (LocateRoute)Enum.Parse(typeof(LocateRoute), (string)enumContainer.ReadSettingsValue(m.ToString()));
                    li.Add(toure);
                }
                c.Routes = li.ToArray();
                return c;
            }
            catch (Exception)
            {
                if (c.SavedCities == null)
                {
                    c.SavedCities = new CitySettingsModel[] { };
                }
                return c;
            }
        }

        public void ChangeRoute(LocateRoute[] locateRoute)
        {
            this.Routes = locateRoute;
        }

        public void Pick(int index)
        {
            CurrentIndex = index;
        }

        public void Save()
        {
            var container = RoamingSettingsHelper.GetContainer("Cities");
            var subContainer = container.GetContainer("Locate");
            container.Values["CurrentIndex"] = CurrentIndex;
            container.Values["EnableLocate"] = EnableLocate;
            if (LocatedCity != null)
            {
                try
                {
                    subContainer.WriteGroupSettings(LocatedCity);
                }
                catch (Exception)
                {
                }
            }
            else
            {
                try
                {
                    container.DeleteContainer("Locate");
                }
                catch (Exception)
                {
                }

            }
            var enumContainer = container.GetContainer("Routes");
            try
            {
                if (!Routes.IsNullorEmpty())
                {
                    int i = 0;
                    for (; i < Routes.Length;)
                    {
                        try
                        {
                            enumContainer.WriteSettingsValue(i.ToString(), Routes[i]);
                            i++;
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                    enumContainer.Values["Count"] = i;
                }
            }
            catch (Exception)
            {

            }
            SaveCities(container);
        }

        public CitySettingsModel GetCurrentCity()
        {
            if (EnableLocate && CurrentIndex == -1 && LocatedCity != null)
            {
                return LocatedCity;
            }
            else if (EnableLocate && CurrentIndex == -1 && LocatedCity == null)
            {
                throw new NullReferenceException("Locate city null");
            }
            else if (!EnableLocate && CurrentIndex == -1 && !SavedCities.IsNullorEmpty())
            {
                CurrentIndex = 0;
                return SavedCities[CurrentIndex];
            }
            else if (!SavedCities.IsNullorEmpty() && CurrentIndex >= 0 && SavedCities.Length > CurrentIndex)
            {
                return SavedCities[CurrentIndex];
            }
            else
            {
                CurrentIndex = SavedCities.Length - 1;
                return SavedCities[CurrentIndex];
            }
        }

        private void SaveCities(Windows.Storage.ApplicationDataContainer container)
        {
            int i = 0;
            if (!SavedCities.IsNullorEmpty())
            {
                foreach (var item in SavedCities)
                {
                    var sub = RoamingSettingsHelper.GetContainer(i.ToString());
                    try
                    {
                        sub.WriteGroupSettings(item);
                        i++;
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            container.Values["Count"] = i;
        }

        public void Save(CitySettingsModel[] citys)
        {
            this.SavedCities = citys;
            Save();
        }

        public void Set(CitySettingsModel[] citys)
        {
            SavedCities = citys;
        }

        public async Task SaveDataAsync(string currentId, string resstr, DataSource source)
        {
            try
            {
                switch (source)
                {
                    case DataSource.HeWeather:
                        await FileIOHelper.SaveStringtoStorageAsync(currentId + "_H", resstr);
                        break;
                    case DataSource.Caiyun:
                        await FileIOHelper.SaveStringtoStorageAsync(currentId + "_C", resstr);
                        break;
                    case DataSource.Wunderground:
                        await FileIOHelper.SaveStringtoStorageAsync(currentId + "_W", resstr);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {

            }

        }

        public void ChangeCurrent(string args)
        {
            var index = Array.FindIndex(SavedCities, x =>
            {
                return x.Id == args;
            });
            if (index >= 0)
            {
                CurrentIndex = index;
            }

            if (EnableLocate)
            {
                if (SavedCities.IsNullorEmpty() || CurrentIndex < 0)
                {
                    CurrentIndex = -1;
                }
            }
        }


        public async Task<string> ReadDataAsync(string id, DataSource dataSource)
        {
            switch (dataSource)
            {
                case DataSource.HeWeather:
                    return await FileIOHelper.ReadStringFromStorageAsync(id + "_H");
                case DataSource.Caiyun:
                    return await FileIOHelper.ReadStringFromStorageAsync(id + "_C");
                case DataSource.Wunderground:
                    return await FileIOHelper.ReadStringFromStorageAsync(id + "_W");
                default:
                    return null;
            }
        }
    }
    public class CitySettingsModel
    {
        public string City { get; set; }
        public string Id { get; set; }
        public bool IsPinned { get; set; } = false;
        public float Longitude { get; set; } = 0;
        public float Latitude { get; set; } = 0;

        public bool IsCurrent = false;

        public DateTime LastUpdate { get; set; } = new DateTime(1970, 1, 1);
        public string ZMW { get; internal set; } = "";

        public CitySettingsModel(CityInfo info)
        {
            City = info.City;
            Id = info.Id;
            Longitude = info.Location.Longitude;
            Latitude = info.Location.Latitude;
        }

        public CitySettingsModel()
        {
            //Use this to Create Default Instance. Otherwise you can't use reflection.
        }

        public void Update()
        {
            LastUpdate = DateTime.Now;
        }

        internal void RequestZMW(string s)
        {
            if (s.IsNullorEmpty())
            {
                ZMW = "";
            }
            else
            {
                ZMW = s;
            }

        }
    }
}
